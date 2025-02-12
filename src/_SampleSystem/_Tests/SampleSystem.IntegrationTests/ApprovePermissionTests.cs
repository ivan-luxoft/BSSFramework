﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Automation.ServiceEnvironment;
using FluentAssertions;

using Framework.Authorization.ApproveWorkflow;
using Framework.Authorization.Domain;
using Framework.Authorization.Generated.DTO;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SampleSystem.IntegrationTests.__Support.TestData;
using SampleSystem.ServiceEnvironment;
using SampleSystem.WebApiCore.Controllers;

using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace SampleSystem.IntegrationTests.Workflow
{
    [TestClass]
    public class ApprovePermissionTests : TestBase
    {
        private OperationSimpleDTO approveOperation;

        private BusinessRoleIdentityDTO approvingRole;

        private BusinessRoleIdentityDTO approveRole;

        private BusinessRoleIdentityDTO grandRole;

        private const string GrandUser = "GrandUser";

        private const string UserWithApprove = "ApproveWfUser";

        private const string TestUserForApproving = "ApprovingWfUser";

        private WorkflowManager workflowManager;

        private ControllerEvaluator<AuthSLJsonController> authFacade;

        [TestInitialize]
        public void SetUp()
        {
            this.authFacade = this.GetAuthControllerEvaluator();

            this.approveOperation = this.authFacade.Evaluate(c => c.GetSimpleOperationByName(nameof(SampleSystemSecurityOperationCode.ApproveWorkflowOperation)));

            this.approveRole = this.authFacade.Evaluate(c => c.SaveBusinessRole(new BusinessRoleStrictDTO
            {
                Name = "Approve Role",
                BusinessRoleOperationLinks =
                {
                    new BusinessRoleOperationLinkStrictDTO { Operation = this.approveOperation.Identity }
                }
            }));

            var approverPrincipal = this.authFacade.Evaluate(c => c.SavePrincipal(new PrincipalStrictDTO
            {
                Name = UserWithApprove,
                Permissions =
                {
                    new PermissionStrictDTO
                    {
                        Role = this.approveRole,
                    }
                }
            }));

            var approvingOperation = this.authFacade.Evaluate(c => c.GetSimpleOperationByName(nameof(SampleSystemSecurityOperationCode.ApprovingWorkflowOperation)));

            this.approvingRole = this.authFacade.Evaluate(c => c.SaveBusinessRole(new BusinessRoleStrictDTO
            {
                Name = "Approving Role",
                BusinessRoleOperationLinks =
                {
                    new BusinessRoleOperationLinkStrictDTO { Operation = approvingOperation.Identity }
                }
            }));


            this.grandRole = this.authFacade.Evaluate(c => c.SaveBusinessRole(new BusinessRoleStrictDTO
            {
                Name = "Grand Approve Role",
                SubBusinessRoleLinks =
                {
                    new SubBusinessRoleLinkStrictDTO { SubBusinessRole = this.approveRole },
                    new SubBusinessRoleLinkStrictDTO { SubBusinessRole = this.approvingRole },
                }
            }));

            var grandPrincipal = this.authFacade.Evaluate(c => c.SavePrincipal(new PrincipalStrictDTO
            {
                Name = GrandUser,
                Permissions =
                {
                    new PermissionStrictDTO
                    {
                        Role = this.grandRole,
                    }
                }
            }));

            this.workflowManager = this.RootServiceProvider.GetRequiredService<WorkflowManager>();
            this.workflowManager.Start();
        }

        [TestCleanup]
        public void Cleanup()
        {
            this.workflowManager.Stop();
        }

        [TestMethod]
        public async Task CreatePermissionWithApprove_PermissionApproved()
        {
            // Arrange
            var wfController = this.GetControllerEvaluator<WorkflowController>(UserWithApprove);

            // Act
            var approvingPrincipal = this.CreateTestPermission();

            var permissionIdentity = approvingPrincipal.Permissions.Single().Identity;

            var startedWf = await wfController
                .WithImpersonate(DefaultConstants.INTEGRATION_USER).EvaluateAsync(c => c.StartJob());
            var rootInstanceId = startedWf[permissionIdentity.Id];

            await Task.Delay(3000);

            var wfObjects = await WaitToCompleteHelper.Retry(
                () => wfController.EvaluateAsync(c => c.GetMyPendingApproveOperationWorkflowObjects(permissionIdentity)),
                res => !res.Any(),
                TimeSpan.FromSeconds(10));

            foreach (var wfObj in wfObjects)
            {
                await wfController.EvaluateAsync(c => c.ApproveOperation(permissionIdentity, wfObj.ApproveEventId));
            }

            var wiStatus = await this.RootServiceProvider.GetRequiredService<IPersistenceProvider>().WaitForWorkflowToComplete(rootInstanceId.ToString(), TimeSpan.FromSeconds(10));

            var postApprovePrincipal = this.authFacade.Evaluate(c => c.GetRichPrincipal(approvingPrincipal.Identity));

            // Assert

            approvingPrincipal.Permissions.Single().Status.Should().Be(PermissionStatus.Approving);
            wiStatus.Should().Be(WorkflowStatus.Complete);
            postApprovePrincipal.Permissions.Single().Status.Should().Be(PermissionStatus.Approved);
        }

        [TestMethod]
        public async Task CreatePermissionWithApprove_PermissionRejected()
        {
            // Arrange
            var wfController = this.GetControllerEvaluator<WorkflowController>(UserWithApprove);

            // Act
            var approvingPrincipal = this.CreateTestPermission();

            var permissionIdentity = approvingPrincipal.Permissions.Single().Identity;

            var startedWf = await wfController.WithImpersonate(DefaultConstants.INTEGRATION_USER)
                .EvaluateAsync(c => c.StartJob());
            var rootInstanceId = startedWf[permissionIdentity.Id];

            await Task.Delay(3000);

            var wfObjects = await WaitToCompleteHelper.Retry(
                () => wfController.EvaluateAsync(c => c.GetMyPendingApproveOperationWorkflowObjects(permissionIdentity)),
                res => !res.Any(),
                TimeSpan.FromSeconds(10));

            foreach (var wfObj in wfObjects)
            {
                await wfController.EvaluateAsync(c => c.RejectOperation(permissionIdentity, wfObj.RejectEventId));
            }

            var wiStatus = await this.RootServiceProvider.GetRequiredService<IPersistenceProvider>().WaitForWorkflowToComplete(rootInstanceId.ToString(), TimeSpan.FromSeconds(10));

            var postApprovePrincipal = this.authFacade.Evaluate(c => c.GetRichPrincipal(approvingPrincipal.Identity));

            // Assert

            approvingPrincipal.Permissions.Single().Status.Should().Be(PermissionStatus.Approving);
            wiStatus.Should().Be(WorkflowStatus.Complete);
            postApprovePrincipal.Permissions.Single().Status.Should().Be(PermissionStatus.Rejected);
        }

        [TestMethod]
        public async Task CreatePermissionWithAutoApprove_PermissionApproved()
        {
            // Arrange


            // Act
            var approvingPrincipal = this.DelegateTestPermission();

            var permissionIdentity = approvingPrincipal.Permissions.Single().Identity;

            var startedWf = await this.GetControllerEvaluator<WorkflowController>()
                .WithImpersonate(DefaultConstants.INTEGRATION_USER).EvaluateAsync(c => c.StartJob());
            var rootInstanceId = startedWf[permissionIdentity.Id];

            await Task.Delay(3000);

            var wiStatus = await this.RootServiceProvider.GetRequiredService<IPersistenceProvider>().WaitForWorkflowToComplete(rootInstanceId.ToString(), TimeSpan.FromSeconds(20));

            var postApprovePrincipal = this.authFacade.Evaluate(c => c.GetRichPrincipal(approvingPrincipal.Identity));

            // Assert
            wiStatus.Should().Be(WorkflowStatus.Complete);
            postApprovePrincipal.Permissions.Single().Status.Should().Be(PermissionStatus.Approved);
        }

        [TestMethod]
        public async Task CreatePermission_TryApproveWithoutAccess_AccessDeniedExceptionRaised()
        {
            // Arrange
            var wfController = this.GetControllerEvaluator<WorkflowController>(UserWithApprove);

            // Act
            var approvingPrincipal = this.CreateTestPermission();

            var permissionIdentity = approvingPrincipal.Permissions.Single().Identity;

            var startedWf = await wfController
                .WithImpersonate(DefaultConstants.INTEGRATION_USER)
                .EvaluateAsync(c => c.StartJob());
            var rootInstanceId = startedWf[permissionIdentity.Id];

            await Task.Delay(3000);

            var wfObjects = await WaitToCompleteHelper.Retry(
                                                             () => wfController.EvaluateAsync(c => c.GetMyPendingApproveOperationWorkflowObjects(permissionIdentity)),
                                                             res => !res.Any(),
                                                             TimeSpan.FromSeconds(10));

            var tryApprove = async () =>
            {
                foreach (var wfObj in wfObjects)
                {
                    await wfController.WithImpersonate("NoName").EvaluateAsync(c => c.ApproveOperation(permissionIdentity, wfObj.ApproveEventId));
                }
            };

            // Assert
            await tryApprove.Should().ThrowAsync<Exception>($"Permission:{permissionIdentity.Id:D} | Access denied with eventId {wfObjects.Single().ApproveEventId}");
        }

        private PrincipalRichDTO CreateTestPermission()
        {
            var approvingPrincipal = this.authFacade.Evaluate(c => c.SavePrincipal(new PrincipalStrictDTO
            {
                Name = TestUserForApproving,
                Permissions =
                    {
                            new PermissionStrictDTO
                            {
                                    Role = this.approvingRole,
                            }
                    }
            }));

            return this.authFacade.Evaluate(c => c.GetRichPrincipal(approvingPrincipal));
        }

        private PrincipalRichDTO DelegateTestPermission()
        {
            var approvingPrincipal = this.authFacade.Evaluate(c => c.SavePrincipal(new PrincipalStrictDTO
            {
                Name = TestUserForApproving
            }));

            var myPrincipal = this.authFacade.WithImpersonate(GrandUser).Evaluate(c => c.GetRichPrincipalByName(GrandUser));

            this.authFacade.WithImpersonate(GrandUser).Evaluate(c => c.ChangeDelegatePermissions(new ChangePermissionDelegatesModelStrictDTO
            {
                DelegateFromPermission = myPrincipal.Permissions.Single().Identity,
                Items =
                    {
                            new DelegateToItemModelStrictDTO
                            {
                                    Principal = approvingPrincipal,
                                    Permission = new PermissionStrictDTO
                                                 {
                                                         Role = this.approvingRole,
                                                 }
                            }
                    }
            }));

            return this.authFacade.Evaluate(c => c.GetRichPrincipal(approvingPrincipal));
        }
    }
}
