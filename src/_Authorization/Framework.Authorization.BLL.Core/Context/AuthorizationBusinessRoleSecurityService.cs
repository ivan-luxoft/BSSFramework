﻿using System;

using Framework.Authorization.Domain;
using Framework.SecuritySystem;

using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;

namespace Framework.Authorization.BLL
{
    public partial class AuthorizationBusinessRoleSecurityService
    {
        public AuthorizationBusinessRoleSecurityService(
            IAccessDeniedExceptionService<PersistentDomainObjectBase> accessDeniedExceptionService,
            IDisabledSecurityProviderContainer<PersistentDomainObjectBase> disabledSecurityProviderContainer,
            ISecurityOperationResolver<PersistentDomainObjectBase, AuthorizationSecurityOperationCode> securityOperationResolver,
            IAuthorizationSystem<Guid> authorizationSystem,
            [NotNull] IAuthorizationBLLContext context)

            : base(accessDeniedExceptionService, disabledSecurityProviderContainer, securityOperationResolver, authorizationSystem)
        {
            this.Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IAuthorizationBLLContext Context { get; }

        protected override ISecurityProvider<BusinessRole> CreateSecurityProvider(BLLSecurityMode securityMode)
        {
            var baseProvider = base.CreateSecurityProvider(securityMode);

            switch (securityMode)
            {
                case BLLSecurityMode.View:
                    return this.Context.GetBusinessRoleSecurityProvider().Or(baseProvider, this.AccessDeniedExceptionService);

                default:
                    return baseProvider;
            }
        }
    }
}
