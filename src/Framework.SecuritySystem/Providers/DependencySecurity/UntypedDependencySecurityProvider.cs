﻿using System;
using System.Collections.Generic;
using System.Linq;

using Framework.Core;
using Framework.Persistent;
using Framework.QueryableSource;

namespace Framework.SecuritySystem
{
    public class UntypedDependencySecurityProvider<TPersistentDomainObjectBase, TDomainObject, TBaseDomainObject, TIdent> : SecurityProviderBase<TDomainObject>
        where TDomainObject : class, TPersistentDomainObjectBase
        where TBaseDomainObject : class, TPersistentDomainObjectBase
        where TPersistentDomainObjectBase : class, IIdentityObject<TIdent>
    {
        private readonly ISecurityProvider<TBaseDomainObject> baseSecurityProvider;

        private readonly IQueryableSource<TPersistentDomainObjectBase> queryableSource;

        private readonly Lazy<HashSet<TIdent>> lazyAvailableIdents;

        public UntypedDependencySecurityProvider(IAccessDeniedExceptionService<TPersistentDomainObjectBase> accessDeniedExceptionService, ISecurityProvider<TBaseDomainObject> baseSecurityProvider, IQueryableSource<TPersistentDomainObjectBase> queryableSource)
            : base(accessDeniedExceptionService)
        {
            this.baseSecurityProvider = baseSecurityProvider;
            this.queryableSource = queryableSource;

            this.lazyAvailableIdents = LazyHelper.Create(() => this.GetAvailableIdents().ToHashSet());
        }

        public override IQueryable<TDomainObject> InjectFilter(IQueryable<TDomainObject> queryable)
        {
            var availableIdents = this.GetAvailableIdents();

            return queryable.Where(domainObj => availableIdents.Contains(domainObj.Id));
        }

        public override bool HasAccess(TDomainObject domainObject)
        {
            return this.lazyAvailableIdents.Value.Contains(domainObject.Id);
        }

        public override UnboundedList<string> GetAccessors(TDomainObject domainObject)
        {
            var baseDomainObj = this.queryableSource.GetQueryable<TBaseDomainObject>().SingleOrDefault(v => v.Id.Equals(domainObject.Id)).FromMaybe(() => $"Object with id = '{domainObject.Id}' not found");

            return this.baseSecurityProvider.GetAccessors(baseDomainObj);
        }

        protected virtual IQueryable<TIdent> GetAvailableIdents()
        {
            return this.queryableSource.GetQueryable<TBaseDomainObject>().Pipe(this.baseSecurityProvider.InjectFilter).Select(v => v.Id);
        }
    }
}
