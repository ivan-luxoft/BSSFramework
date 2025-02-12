﻿using System;

using Framework.Authorization.Domain;

using JetBrains.Annotations;

namespace Framework.Authorization.ApproveWorkflow;

public interface IWorkflowApproveProcessor
{
    bool CanAutoApprove([NotNull] Permission permission, [NotNull] Operation approveOperation);

    ApprovePermissionWorkflowObject GetPermissionStartupObject([NotNull] Permission permission);
}
