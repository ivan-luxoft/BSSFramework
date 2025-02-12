﻿using System;
using System.Collections.Generic;
using System.Linq;
using Framework.Authorization.Domain;
using Framework.Core;
using Framework.DomainDriven;
using Framework.DomainDriven.BLL;
using Framework.DomainDriven.BLL.Security;
using Framework.DomainDriven.BLL.Tracking;
using Framework.DomainDriven.UnitTest.Mock;
using Framework.DomainDriven.UnitTest.Mock.StubProxy;
using Framework.Validation;

using Microsoft.Extensions.DependencyInjection;

using NSubstitute;

namespace Framework.Authorization.BLL.Tests.Unit.Support
{
    public class AuthorizationTestConfiguration : BLLContextConfiguration<IAuthorizationBLLContext, PersistentDomainObjectBase>
    {
        private readonly BLLFactoryContainerConfiguration _bllFactoryConfiguration;

        public AuthorizationTestConfiguration()
            : base(new[] { typeof(Permission).Assembly })
        {
            this._bllFactoryConfiguration = new BLLFactoryContainerConfiguration();
        }

        public AuthorizationTestConfiguration WithValidatorMock()
        {
            this.Context.Validator.Returns(ValidatorBase.Success);

            return this;
        }

        public AuthorizationTestConfiguration WithExternalSourceMock(Guid[] result)
        {
            return this.WithExternalSourceMock(new AuthorizationTypedExternalSourceStub(result));
        }

        public AuthorizationTestConfiguration WithExternalSourceMock(SecurityEntity[] result)
        {
            return this.WithExternalSourceMock(new AuthorizationTypedExternalSourceStub(result));
        }

        private AuthorizationTestConfiguration WithExternalSourceMock(AuthorizationTypedExternalSourceStub stub)
        {
            var authorizationExternalSourceMock = Substitute.For<IAuthorizationExternalSource>();
            authorizationExternalSourceMock.GetTyped(Arg.Any<EntityType>()).Returns(stub);

            this.Context.ExternalSource.Returns(authorizationExternalSourceMock);
            return this;
        }

        public AuthorizationTestConfiguration WithTrackingChange(IObjectStateService objectStateService = null)
        {
            this.Context.TrackingService
                .Returns(new TrackingService<PersistentDomainObjectBase>(objectStateService ?? Substitute.For<IObjectStateService>()))
                ;

            return this;
        }

        private class AuthorizationTypedExternalSourceStub : IAuthorizationTypedExternalSource
        {
            private readonly Guid[] result;
            private readonly SecurityEntity[] result2;

            public AuthorizationTypedExternalSourceStub(Guid[] result)
            {
                this.result = result;
            }

            public AuthorizationTypedExternalSourceStub(SecurityEntity[] result2)
            {
                this.result2 = result2;
            }

            public IEnumerable<SecurityEntity> GetSecurityEntities()
            {
                throw new NotImplementedException();
            }

            public IEnumerable<SecurityEntity> GetSecurityEntitiesByIdents(IEnumerable<Guid> securityEntityIdents)
            {
                throw new NotImplementedException();
            }

            public IEnumerable<SecurityEntity> GetSecurityEntitiesWithMasterExpand(Guid startSecurityEntityId)
            {
                if (this.result != null)
                {
                    return this.result.Select(x => new SecurityEntity { Id = x });
                }

                if (this.result2 != null)
                {
                    return this.GetChildren(this.result2, startSecurityEntityId);
                }

                throw new NotImplementedException();
            }

            private IEnumerable<SecurityEntity> GetChildren(SecurityEntity[] all, Guid parentId)
            {
                yield return all.First(x => x.Id == parentId);

                var children = all.Where(x => x.ParentId == parentId).ToList();
                if (children.Any())
                {
                    foreach (var securityEntity in children)
                    {
                        yield return securityEntity;

                        foreach (var e in this.GetChildren(all, securityEntity.Id))
                        {
                            yield return e;
                        }
                    }
                }
            }

            public PermissionFilterEntity AddSecurityEntity(SecurityEntity securityEntity, bool disableExistsCheck = false)
            {
                throw new NotImplementedException();
            }
        }

        protected override void Initialize<T>(T result)
        {
            var bllFactoryContainer = StubProxyFactory.CreateStub(new AuthorizationBLLFactoryContainer(result), this.BLLFactoryConfiguration.InitializeActions);

            result.Logics.Returns(bllFactoryContainer);

            ((IBLLFactoryContainerContext<IAuthorizationBLLFactoryContainer>)result).Logics.Returns(bllFactoryContainer);

            result.SecurityService.Returns(new AuthorizationSecurityService(result));
            result.OperationSenders.Returns(new OperationEventSenderContainer<DomainObjectBase>(new List<IOperationEventListener<DomainObjectBase>>()));

            var authContext = this.AuthorizationBLLContext;

            result.Authorization.Returns(authContext);

            result.Validator.Returns(ValidatorBase.Success);

            var serviceProvider = new ServiceCollection().AddScoped(_ => result)
                                                         .Self(AuthorizationBLLFactoryContainer.RegisterBLLFactory)
                                                         .BuildServiceProvider();

            result.ServiceProvider.Returns(serviceProvider);
        }

        private IAuthorizationBLLContext AuthorizationBLLContext
        {
            get
            {
                var result = Substitute.For<IAuthorizationBLLContext>();
                var runAsManager = Substitute.For<IRunAsManager>();

                runAsManager.PrincipalName.Returns("testUser");

                result.RunAsManager.Returns(runAsManager);

                return result;
            }
        }

        public BLLFactoryContainerConfiguration BLLFactoryConfiguration => this._bllFactoryConfiguration;
    }
}
