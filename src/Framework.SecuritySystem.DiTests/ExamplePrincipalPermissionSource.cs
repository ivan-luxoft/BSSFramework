﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Framework.SecuritySystem.DiTests
{
    public class ExamplePrincipalPermissionSource : IPrincipalPermissionSource<Guid>
    {
        private readonly List<Dictionary<Type, List<Guid>>> permissions;

        public ExamplePrincipalPermissionSource(List<Dictionary<Type, List<Guid>>> permissions)
        {
            this.permissions = permissions;
        }

        public List<Dictionary<Type, List<Guid>>> GetPermissions()
        {
            return this.permissions;
        }

        public IQueryable<IPermission<Guid>> GetPermissionQuery<TSecurityOperationCode>(ContextSecurityOperation<TSecurityOperationCode> securityOperation)
                where TSecurityOperationCode : struct, Enum
        {
            throw new NotImplementedException();
        }
    }
}
