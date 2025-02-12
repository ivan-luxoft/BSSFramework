﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Framework.Core;
using Framework.Persistent;

namespace Framework.Authorization.BLL
{
    public class NotificationFilterGroup<TDomainObject> : NotificationFilterGroup
        where TDomainObject : IIdentityObject<Guid>
    {
        public NotificationFilterGroup(IEnumerable<TDomainObject> domainObjects, NotificationExpandType expandType)
            : base(typeof(TDomainObject), domainObjects.FromMaybe(() => new ArgumentNullException(nameof(domainObjects))).Select(domainObject => domainObject.Id), expandType)
        {

        }
    }

    public class NotificationFilterGroup
    {
        public NotificationFilterGroup(Type entityType, IEnumerable<Guid> idents, NotificationExpandType expandType)
        {
            if (entityType == null) throw new ArgumentNullException(nameof(entityType));
            if (idents == null) throw new ArgumentNullException(nameof(idents));

            this.EntityType = entityType;
            this.Idents = idents.ToReadOnlyCollection();
            this.ExpandType = expandType;
        }


        public Type EntityType { get; private set; }

        public ReadOnlyCollection<Guid> Idents { get; private set; }

        public NotificationExpandType ExpandType { get; private set; }
    }


    public static class NotificationFilterGroupExtensions
    {
        private static readonly HashSet<NotificationExpandType> OverrideExpandTypes =

            EnumHelper.GetValues<NotificationExpandType>().Where(expandType => expandType.IsHierarchical()).ToHashSet();


        public static NotificationFilterGroup OverrideExpand(this NotificationFilterGroup group, NotificationExpandType newExpandType)
        {
            return newExpandType == group.ExpandType ? group
                                                     : new NotificationFilterGroup(group.EntityType, group.Idents, newExpandType);
        }

        public static IEnumerable<NotificationFilterGroup[]> PermuteByExpand(this IEnumerable<NotificationFilterGroup> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            var cachedSource = source.ToArray(true);

            if (cachedSource.Any())
            {
                var authExpandGroups = cachedSource.Where(g => OverrideExpandTypes.Contains(g.ExpandType)).ToArray();

                if (authExpandGroups.Any())
                {
                    var request = from currentGroup in authExpandGroups

                                  select cachedSource.OrderBy(g => g != currentGroup)
                                                     .ToArray(g => g == currentGroup ? g : g.OverrideExpand(g.ExpandType.WithoutHierarchical()));

                    foreach (var permute in request)
                    {
                        yield return permute;
                    }
                }
                else
                {
                    yield return cachedSource;
                }
            }
        }
    }
}
