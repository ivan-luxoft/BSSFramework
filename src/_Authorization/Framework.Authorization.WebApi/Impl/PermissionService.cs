﻿using System;
using System.Collections.Generic;

using Framework.Authorization.Domain;
using Framework.Authorization.Generated.DTO;
using Framework.DomainDriven;
using Framework.HierarchicalExpand;
using Framework.SecuritySystem;

using Microsoft.AspNetCore.Mvc;

namespace Framework.Authorization.WebApi
{
    public partial class AuthSLJsonController
    {
        [Microsoft.AspNetCore.Mvc.HttpPost(nameof(ChangeDelegatePermissions))]
        public void ChangeDelegatePermissions([FromForm] ChangePermissionDelegatesModelStrictDTO changePermissionDelegatesModelStrictDTO)
        {
            if (changePermissionDelegatesModelStrictDTO == null) throw new ArgumentNullException(nameof(changePermissionDelegatesModelStrictDTO));

            this.Evaluate(DBSessionMode.Write, evaluateData =>
            {
                var changePermissionDelegatesModel = changePermissionDelegatesModelStrictDTO.ToDomainObject(evaluateData.MappingService);

                var securityProvider = evaluateData.Context.GetPrincipalSecurityProvider<Permission>(permission => permission.Principal)
                                   .Or(evaluateData.Context.SecurityService.GetSecurityProvider<Permission>(AuthorizationSecurityOperationCode.PrincipalEdit), evaluateData.Context.AccessDeniedExceptionService);

                var bll = evaluateData.Context.Logics.PermissionFactory.Create(securityProvider);

                bll.ChangeDelegatePermissions(changePermissionDelegatesModel);
            });
        }

        [Microsoft.AspNetCore.Mvc.HttpPost(nameof(GetVisualBusinessRolesByPermission))]
        public IEnumerable<BusinessRoleVisualDTO> GetVisualBusinessRolesByPermission([FromForm] PermissionIdentityDTO permission)
        {
            return this.Evaluate(DBSessionMode.Write, evaluateData =>
                evaluateData.Context.Logics.PermissionFactory
                                   .Create(BLLSecurityMode.View)
                                   .GetById(permission.Id, true)
                                   .Role
                                   .GetAllChildren()
                                   .ToVisualDTOList(evaluateData.MappingService));
        }
    }
}
