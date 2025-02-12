﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Framework.Configuration.Domain;
using Framework.Core;
using Framework.DomainDriven;
using Framework.DomainDriven.BLL;
using Framework.Persistent;
using Framework.Transfering;

using JetBrains.Annotations;

namespace Framework.Configuration.BLL
{
    public partial class TargetSystemBLL
    {
        /// <inheritdoc />
        public TargetSystem RegisterBase(IReadOnlyDictionary<Guid, Type> extBaseTypes = null)
        {
            return this.Register(PersistentHelper.BaseTargetSystemName, true, false, false, PersistentHelper.BaseTargetSystemId, new[] { typeof(string), typeof(int), typeof(Guid), typeof(DateTime), typeof(bool), typeof(decimal) }.ToDictionary(type => type.GetDomainTypeId(true)).Concat(extBaseTypes ?? new Dictionary<Guid, Type>()));
        }

        /// <inheritdoc />
        public TargetSystem Register<TPersistentDomainObjectBase>(bool isMain, bool isRevision, IEnumerable<Assembly> assemblies = null, IReadOnlyDictionary<Guid, Type> extTypes = null)
        {
            this.Context.Logics.NamedLock.Lock(NamedLockOperation.UpdateDomainTypeLock, LockRole.Update);

            var request = from type in (assemblies ?? new[] { typeof(TPersistentDomainObjectBase).Assembly }).SelectMany(z => z.GetTypes())

                          where !type.IsAbstract && !type.IsGenericTypeDefinition && typeof(TPersistentDomainObjectBase).IsAssignableFrom(type)

                          let id = type.GetDomainTypeId()

                          where !id.IsDefault()

                          select id.ToKeyValuePair(type);

            return this.Register(typeof(TPersistentDomainObjectBase).GetTargetSystemName(), false, isMain, isRevision, typeof(TPersistentDomainObjectBase).GetTargetSystemId(), request.ToDictionary().Concat(extTypes ?? new Dictionary<Guid, Type>()));
        }

        private TargetSystem Register([NotNull] string targetSystemName, bool isBase, bool isMain, bool isRevision, Guid id, [NotNull] IReadOnlyDictionary<Guid, Type> domainTypes)
        {
            if (targetSystemName == null) throw new ArgumentNullException(nameof(targetSystemName));
            if (domainTypes == null) throw new ArgumentNullException(nameof(domainTypes));

            var targetSystem = this.GetById(id, false, this.Context.FetchService.GetContainer<TargetSystem>(MainDTOType.RichDTO)) ?? new TargetSystem(isBase, isMain, isRevision) { Name = targetSystemName, SubscriptionEnabled = !isBase, Id = id }.Self(this.Insert);

            var mergeResult = targetSystem.DomainTypes.GetMergeResult(domainTypes, t => t.Id, t => t.Key);

            foreach (var newItem in mergeResult.AddingItems)
            {
                var newDomainType = new DomainType(targetSystem)
                {
                    Id = newItem.Key,
                    Name = newItem.Value.Name,
                    NameSpace = newItem.Value.Namespace
                };

                if (!isBase)
                {
                    newItem.Value.GetEventOperations(true).Foreach(value => new DomainTypeEventOperation(newDomainType, value));
                }

                this.Context.Logics.DomainType.Insert(newDomainType);
            }

            foreach (var mergePair in mergeResult.CombineItems)
            {
                var domainType = mergePair.Item1;

                var type = mergePair.Item2.Value;

                var changedName = domainType.Name != type.Name || domainType.NameSpace != type.Namespace;

                var mergeEventResult = domainType.EventOperations.GetMergeResult(type.GetEventOperations(true), operation => operation.Name, value => value.ToString());

                if (changedName || (!isBase && !mergeEventResult.IsEmpty))
                {
                    domainType.Name = type.Name;
                    domainType.NameSpace = type.Namespace;

                    if (!isBase)
                    {
                        domainType.RemoveDetails(mergeEventResult.RemovingItems);

                        mergeEventResult.AddingItems.Foreach(value => new DomainTypeEventOperation(domainType, value));
                    }

                    this.Save(targetSystem);
                }
            }

            if (mergeResult.RemovingItems.Any())
            {
                targetSystem.RemoveDetails(mergeResult.RemovingItems);
                this.Save(targetSystem);
            }

            return targetSystem;
        }
    }
}
