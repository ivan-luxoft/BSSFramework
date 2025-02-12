﻿namespace SampleSystem.WebApiCore.Controllers.Main
{
    using SampleSystem.Generated.DTO;
    
    
    [Microsoft.AspNetCore.Mvc.ApiControllerAttribute()]
    [Microsoft.AspNetCore.Mvc.ApiVersionAttribute("1.0")]
    [Microsoft.AspNetCore.Mvc.RouteAttribute("api/v{version:apiVersion}/[controller]")]
    public partial class BusinessUnitManagerCommissionLinkController : Framework.DomainDriven.WebApiNetCore.ApiControllerBase<SampleSystem.BLL.ISampleSystemBLLContext, Framework.DomainDriven.ServiceModel.Service.EvaluatedData<SampleSystem.BLL.ISampleSystemBLLContext, SampleSystem.Generated.DTO.ISampleSystemDTOMappingService>>
    {
        
        /// <summary>
        /// Check BusinessUnitManagerCommissionLink access
        /// </summary>
        [Microsoft.AspNetCore.Mvc.HttpPostAttribute()]
        [Microsoft.AspNetCore.Mvc.RouteAttribute("CheckBusinessUnitManagerCommissionLinkAccess")]
        public virtual void CheckBusinessUnitManagerCommissionLinkAccess(CheckBusinessUnitManagerCommissionLinkAccessAutoRequest checkBusinessUnitManagerCommissionLinkAccessAutoRequest)
        {
            SampleSystem.SampleSystemSecurityOperationCode securityOperationCode = checkBusinessUnitManagerCommissionLinkAccessAutoRequest.securityOperationCode;
            SampleSystem.Generated.DTO.BusinessUnitManagerCommissionLinkIdentityDTO businessUnitManagerCommissionLinkIdent = checkBusinessUnitManagerCommissionLinkAccessAutoRequest.businessUnitManagerCommissionLinkIdent;
            this.Evaluate(Framework.DomainDriven.DBSessionMode.Read, evaluateData => this.CheckBusinessUnitManagerCommissionLinkAccessInternal(businessUnitManagerCommissionLinkIdent, securityOperationCode, evaluateData));
        }
        
        protected virtual void CheckBusinessUnitManagerCommissionLinkAccessInternal(SampleSystem.Generated.DTO.BusinessUnitManagerCommissionLinkIdentityDTO businessUnitManagerCommissionLinkIdent, SampleSystem.SampleSystemSecurityOperationCode securityOperationCode, Framework.DomainDriven.ServiceModel.Service.EvaluatedData<SampleSystem.BLL.ISampleSystemBLLContext, SampleSystem.Generated.DTO.ISampleSystemDTOMappingService> evaluateData)
        {
            SampleSystem.BLL.IBusinessUnitManagerCommissionLinkBLL bll = evaluateData.Context.Logics.BusinessUnitManagerCommissionLink;
            Framework.Security.TransferEnumHelper.Check(securityOperationCode);
            SampleSystem.Domain.BusinessUnitManagerCommissionLink domainObject = bll.GetById(businessUnitManagerCommissionLinkIdent.Id, true);
            Framework.SecuritySystem.SecurityProviderExtensions.CheckAccess(evaluateData.Context.SecurityService.GetSecurityProvider<SampleSystem.Domain.BusinessUnitManagerCommissionLink>(securityOperationCode), domainObject);
        }
        
        /// <summary>
        /// Get BusinessUnitManagerCommissionLink (FullDTO) by identity
        /// </summary>
        [Microsoft.AspNetCore.Mvc.HttpPostAttribute()]
        [Microsoft.AspNetCore.Mvc.RouteAttribute("GetFullBusinessUnitManagerCommissionLink")]
        public virtual SampleSystem.Generated.DTO.BusinessUnitManagerCommissionLinkFullDTO GetFullBusinessUnitManagerCommissionLink([Microsoft.AspNetCore.Mvc.FromBodyAttribute()] SampleSystem.Generated.DTO.BusinessUnitManagerCommissionLinkIdentityDTO businessUnitManagerCommissionLinkIdentity)
        {
            return this.Evaluate(Framework.DomainDriven.DBSessionMode.Read, evaluateData => this.GetFullBusinessUnitManagerCommissionLinkInternal(businessUnitManagerCommissionLinkIdentity, evaluateData));
        }
        
        protected virtual SampleSystem.Generated.DTO.BusinessUnitManagerCommissionLinkFullDTO GetFullBusinessUnitManagerCommissionLinkInternal(SampleSystem.Generated.DTO.BusinessUnitManagerCommissionLinkIdentityDTO businessUnitManagerCommissionLinkIdentity, Framework.DomainDriven.ServiceModel.Service.EvaluatedData<SampleSystem.BLL.ISampleSystemBLLContext, SampleSystem.Generated.DTO.ISampleSystemDTOMappingService> evaluateData)
        {
            SampleSystem.BLL.IBusinessUnitManagerCommissionLinkBLL bll = evaluateData.Context.Logics.BusinessUnitManagerCommissionLinkFactory.Create(Framework.SecuritySystem.BLLSecurityMode.View);
            SampleSystem.Domain.BusinessUnitManagerCommissionLink domainObject = bll.GetById(businessUnitManagerCommissionLinkIdentity.Id, true, evaluateData.Context.FetchService.GetContainer<SampleSystem.Domain.BusinessUnitManagerCommissionLink>(Framework.Transfering.ViewDTOType.FullDTO));
            return SampleSystem.Generated.DTO.LambdaHelper.ToFullDTO(domainObject, evaluateData.MappingService);
        }
        
        /// <summary>
        /// Get full list of BusinessUnitManagerCommissionLinks (FullDTO)
        /// </summary>
        [Microsoft.AspNetCore.Mvc.HttpPostAttribute()]
        [Microsoft.AspNetCore.Mvc.RouteAttribute("GetFullBusinessUnitManagerCommissionLinks")]
        public virtual System.Collections.Generic.IEnumerable<SampleSystem.Generated.DTO.BusinessUnitManagerCommissionLinkFullDTO> GetFullBusinessUnitManagerCommissionLinks()
        {
            return this.Evaluate(Framework.DomainDriven.DBSessionMode.Read, evaluateData => this.GetFullBusinessUnitManagerCommissionLinksInternal(evaluateData));
        }
        
        /// <summary>
        /// Get BusinessUnitManagerCommissionLinks (FullDTO) by idents
        /// </summary>
        [Microsoft.AspNetCore.Mvc.HttpPostAttribute()]
        [Microsoft.AspNetCore.Mvc.RouteAttribute("GetFullBusinessUnitManagerCommissionLinksByIdents")]
        public virtual System.Collections.Generic.IEnumerable<SampleSystem.Generated.DTO.BusinessUnitManagerCommissionLinkFullDTO> GetFullBusinessUnitManagerCommissionLinksByIdents([Microsoft.AspNetCore.Mvc.FromBodyAttribute()] SampleSystem.Generated.DTO.BusinessUnitManagerCommissionLinkIdentityDTO[] businessUnitManagerCommissionLinkIdents)
        {
            return this.Evaluate(Framework.DomainDriven.DBSessionMode.Read, evaluateData => this.GetFullBusinessUnitManagerCommissionLinksByIdentsInternal(businessUnitManagerCommissionLinkIdents, evaluateData));
        }
        
        protected virtual System.Collections.Generic.IEnumerable<SampleSystem.Generated.DTO.BusinessUnitManagerCommissionLinkFullDTO> GetFullBusinessUnitManagerCommissionLinksByIdentsInternal(SampleSystem.Generated.DTO.BusinessUnitManagerCommissionLinkIdentityDTO[] businessUnitManagerCommissionLinkIdents, Framework.DomainDriven.ServiceModel.Service.EvaluatedData<SampleSystem.BLL.ISampleSystemBLLContext, SampleSystem.Generated.DTO.ISampleSystemDTOMappingService> evaluateData)
        {
            SampleSystem.BLL.IBusinessUnitManagerCommissionLinkBLL bll = evaluateData.Context.Logics.BusinessUnitManagerCommissionLinkFactory.Create(Framework.SecuritySystem.BLLSecurityMode.View);
            return SampleSystem.Generated.DTO.LambdaHelper.ToFullDTOList(bll.GetListByIdents(businessUnitManagerCommissionLinkIdents, evaluateData.Context.FetchService.GetContainer<SampleSystem.Domain.BusinessUnitManagerCommissionLink>(Framework.Transfering.ViewDTOType.FullDTO)), evaluateData.MappingService);
        }
        
        protected virtual System.Collections.Generic.IEnumerable<SampleSystem.Generated.DTO.BusinessUnitManagerCommissionLinkFullDTO> GetFullBusinessUnitManagerCommissionLinksInternal(Framework.DomainDriven.ServiceModel.Service.EvaluatedData<SampleSystem.BLL.ISampleSystemBLLContext, SampleSystem.Generated.DTO.ISampleSystemDTOMappingService> evaluateData)
        {
            SampleSystem.BLL.IBusinessUnitManagerCommissionLinkBLL bll = evaluateData.Context.Logics.BusinessUnitManagerCommissionLinkFactory.Create(Framework.SecuritySystem.BLLSecurityMode.View);
            return SampleSystem.Generated.DTO.LambdaHelper.ToFullDTOList(bll.GetFullList(evaluateData.Context.FetchService.GetContainer<SampleSystem.Domain.BusinessUnitManagerCommissionLink>(Framework.Transfering.ViewDTOType.FullDTO)), evaluateData.MappingService);
        }
        
        /// <summary>
        /// Get BusinessUnitManagerCommissionLink (RichDTO) by identity
        /// </summary>
        [Microsoft.AspNetCore.Mvc.HttpPostAttribute()]
        [Microsoft.AspNetCore.Mvc.RouteAttribute("GetRichBusinessUnitManagerCommissionLink")]
        public virtual SampleSystem.Generated.DTO.BusinessUnitManagerCommissionLinkRichDTO GetRichBusinessUnitManagerCommissionLink([Microsoft.AspNetCore.Mvc.FromBodyAttribute()] SampleSystem.Generated.DTO.BusinessUnitManagerCommissionLinkIdentityDTO businessUnitManagerCommissionLinkIdentity)
        {
            return this.Evaluate(Framework.DomainDriven.DBSessionMode.Read, evaluateData => this.GetRichBusinessUnitManagerCommissionLinkInternal(businessUnitManagerCommissionLinkIdentity, evaluateData));
        }
        
        protected virtual SampleSystem.Generated.DTO.BusinessUnitManagerCommissionLinkRichDTO GetRichBusinessUnitManagerCommissionLinkInternal(SampleSystem.Generated.DTO.BusinessUnitManagerCommissionLinkIdentityDTO businessUnitManagerCommissionLinkIdentity, Framework.DomainDriven.ServiceModel.Service.EvaluatedData<SampleSystem.BLL.ISampleSystemBLLContext, SampleSystem.Generated.DTO.ISampleSystemDTOMappingService> evaluateData)
        {
            SampleSystem.BLL.IBusinessUnitManagerCommissionLinkBLL bll = evaluateData.Context.Logics.BusinessUnitManagerCommissionLinkFactory.Create(Framework.SecuritySystem.BLLSecurityMode.View);
            SampleSystem.Domain.BusinessUnitManagerCommissionLink domainObject = bll.GetById(businessUnitManagerCommissionLinkIdentity.Id, true, evaluateData.Context.FetchService.GetContainer<SampleSystem.Domain.BusinessUnitManagerCommissionLink>(Framework.Transfering.ViewDTOType.FullDTO));
            return SampleSystem.Generated.DTO.LambdaHelper.ToRichDTO(domainObject, evaluateData.MappingService);
        }
        
        /// <summary>
        /// Get BusinessUnitManagerCommissionLink (SimpleDTO) by identity
        /// </summary>
        [Microsoft.AspNetCore.Mvc.HttpPostAttribute()]
        [Microsoft.AspNetCore.Mvc.RouteAttribute("GetSimpleBusinessUnitManagerCommissionLink")]
        public virtual SampleSystem.Generated.DTO.BusinessUnitManagerCommissionLinkSimpleDTO GetSimpleBusinessUnitManagerCommissionLink([Microsoft.AspNetCore.Mvc.FromBodyAttribute()] SampleSystem.Generated.DTO.BusinessUnitManagerCommissionLinkIdentityDTO businessUnitManagerCommissionLinkIdentity)
        {
            return this.Evaluate(Framework.DomainDriven.DBSessionMode.Read, evaluateData => this.GetSimpleBusinessUnitManagerCommissionLinkInternal(businessUnitManagerCommissionLinkIdentity, evaluateData));
        }
        
        protected virtual SampleSystem.Generated.DTO.BusinessUnitManagerCommissionLinkSimpleDTO GetSimpleBusinessUnitManagerCommissionLinkInternal(SampleSystem.Generated.DTO.BusinessUnitManagerCommissionLinkIdentityDTO businessUnitManagerCommissionLinkIdentity, Framework.DomainDriven.ServiceModel.Service.EvaluatedData<SampleSystem.BLL.ISampleSystemBLLContext, SampleSystem.Generated.DTO.ISampleSystemDTOMappingService> evaluateData)
        {
            SampleSystem.BLL.IBusinessUnitManagerCommissionLinkBLL bll = evaluateData.Context.Logics.BusinessUnitManagerCommissionLinkFactory.Create(Framework.SecuritySystem.BLLSecurityMode.View);
            SampleSystem.Domain.BusinessUnitManagerCommissionLink domainObject = bll.GetById(businessUnitManagerCommissionLinkIdentity.Id, true, evaluateData.Context.FetchService.GetContainer<SampleSystem.Domain.BusinessUnitManagerCommissionLink>(Framework.Transfering.ViewDTOType.SimpleDTO));
            return SampleSystem.Generated.DTO.LambdaHelper.ToSimpleDTO(domainObject, evaluateData.MappingService);
        }
        
        /// <summary>
        /// Get full list of BusinessUnitManagerCommissionLinks (SimpleDTO)
        /// </summary>
        [Microsoft.AspNetCore.Mvc.HttpPostAttribute()]
        [Microsoft.AspNetCore.Mvc.RouteAttribute("GetSimpleBusinessUnitManagerCommissionLinks")]
        public virtual System.Collections.Generic.IEnumerable<SampleSystem.Generated.DTO.BusinessUnitManagerCommissionLinkSimpleDTO> GetSimpleBusinessUnitManagerCommissionLinks()
        {
            return this.Evaluate(Framework.DomainDriven.DBSessionMode.Read, evaluateData => this.GetSimpleBusinessUnitManagerCommissionLinksInternal(evaluateData));
        }
        
        /// <summary>
        /// Get BusinessUnitManagerCommissionLinks (SimpleDTO) by idents
        /// </summary>
        [Microsoft.AspNetCore.Mvc.HttpPostAttribute()]
        [Microsoft.AspNetCore.Mvc.RouteAttribute("GetSimpleBusinessUnitManagerCommissionLinksByIdents")]
        public virtual System.Collections.Generic.IEnumerable<SampleSystem.Generated.DTO.BusinessUnitManagerCommissionLinkSimpleDTO> GetSimpleBusinessUnitManagerCommissionLinksByIdents([Microsoft.AspNetCore.Mvc.FromBodyAttribute()] SampleSystem.Generated.DTO.BusinessUnitManagerCommissionLinkIdentityDTO[] businessUnitManagerCommissionLinkIdents)
        {
            return this.Evaluate(Framework.DomainDriven.DBSessionMode.Read, evaluateData => this.GetSimpleBusinessUnitManagerCommissionLinksByIdentsInternal(businessUnitManagerCommissionLinkIdents, evaluateData));
        }
        
        protected virtual System.Collections.Generic.IEnumerable<SampleSystem.Generated.DTO.BusinessUnitManagerCommissionLinkSimpleDTO> GetSimpleBusinessUnitManagerCommissionLinksByIdentsInternal(SampleSystem.Generated.DTO.BusinessUnitManagerCommissionLinkIdentityDTO[] businessUnitManagerCommissionLinkIdents, Framework.DomainDriven.ServiceModel.Service.EvaluatedData<SampleSystem.BLL.ISampleSystemBLLContext, SampleSystem.Generated.DTO.ISampleSystemDTOMappingService> evaluateData)
        {
            SampleSystem.BLL.IBusinessUnitManagerCommissionLinkBLL bll = evaluateData.Context.Logics.BusinessUnitManagerCommissionLinkFactory.Create(Framework.SecuritySystem.BLLSecurityMode.View);
            return SampleSystem.Generated.DTO.LambdaHelper.ToSimpleDTOList(bll.GetListByIdents(businessUnitManagerCommissionLinkIdents, evaluateData.Context.FetchService.GetContainer<SampleSystem.Domain.BusinessUnitManagerCommissionLink>(Framework.Transfering.ViewDTOType.SimpleDTO)), evaluateData.MappingService);
        }
        
        protected virtual System.Collections.Generic.IEnumerable<SampleSystem.Generated.DTO.BusinessUnitManagerCommissionLinkSimpleDTO> GetSimpleBusinessUnitManagerCommissionLinksInternal(Framework.DomainDriven.ServiceModel.Service.EvaluatedData<SampleSystem.BLL.ISampleSystemBLLContext, SampleSystem.Generated.DTO.ISampleSystemDTOMappingService> evaluateData)
        {
            SampleSystem.BLL.IBusinessUnitManagerCommissionLinkBLL bll = evaluateData.Context.Logics.BusinessUnitManagerCommissionLinkFactory.Create(Framework.SecuritySystem.BLLSecurityMode.View);
            return SampleSystem.Generated.DTO.LambdaHelper.ToSimpleDTOList(bll.GetFullList(evaluateData.Context.FetchService.GetContainer<SampleSystem.Domain.BusinessUnitManagerCommissionLink>(Framework.Transfering.ViewDTOType.SimpleDTO)), evaluateData.MappingService);
        }
        
        /// <summary>
        /// Check access for BusinessUnitManagerCommissionLink
        /// </summary>
        [Microsoft.AspNetCore.Mvc.HttpPostAttribute()]
        [Microsoft.AspNetCore.Mvc.RouteAttribute("HasBusinessUnitManagerCommissionLinkAccess")]
        public virtual bool HasBusinessUnitManagerCommissionLinkAccess(HasBusinessUnitManagerCommissionLinkAccessAutoRequest hasBusinessUnitManagerCommissionLinkAccessAutoRequest)
        {
            SampleSystem.SampleSystemSecurityOperationCode securityOperationCode = hasBusinessUnitManagerCommissionLinkAccessAutoRequest.securityOperationCode;
            SampleSystem.Generated.DTO.BusinessUnitManagerCommissionLinkIdentityDTO businessUnitManagerCommissionLinkIdent = hasBusinessUnitManagerCommissionLinkAccessAutoRequest.businessUnitManagerCommissionLinkIdent;
            return this.Evaluate(Framework.DomainDriven.DBSessionMode.Read, evaluateData => this.HasBusinessUnitManagerCommissionLinkAccessInternal(businessUnitManagerCommissionLinkIdent, securityOperationCode, evaluateData));
        }
        
        protected virtual bool HasBusinessUnitManagerCommissionLinkAccessInternal(SampleSystem.Generated.DTO.BusinessUnitManagerCommissionLinkIdentityDTO businessUnitManagerCommissionLinkIdent, SampleSystem.SampleSystemSecurityOperationCode securityOperationCode, Framework.DomainDriven.ServiceModel.Service.EvaluatedData<SampleSystem.BLL.ISampleSystemBLLContext, SampleSystem.Generated.DTO.ISampleSystemDTOMappingService> evaluateData)
        {
            SampleSystem.BLL.IBusinessUnitManagerCommissionLinkBLL bll = evaluateData.Context.Logics.BusinessUnitManagerCommissionLink;
            Framework.Security.TransferEnumHelper.Check(securityOperationCode);
            SampleSystem.Domain.BusinessUnitManagerCommissionLink domainObject = bll.GetById(businessUnitManagerCommissionLinkIdent.Id, true);
            return evaluateData.Context.SecurityService.GetSecurityProvider<SampleSystem.Domain.BusinessUnitManagerCommissionLink>(securityOperationCode).HasAccess(domainObject);
        }
    }
    
    [System.Runtime.Serialization.DataContractAttribute()]
    [Framework.DomainDriven.ServiceModel.IAD.AutoRequestAttribute()]
    public partial class CheckBusinessUnitManagerCommissionLinkAccessAutoRequest
    {
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        [Framework.DomainDriven.ServiceModel.IAD.AutoRequestPropertyAttribute(OrderIndex=0)]
        public SampleSystem.Generated.DTO.BusinessUnitManagerCommissionLinkIdentityDTO businessUnitManagerCommissionLinkIdent;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        [Framework.DomainDriven.ServiceModel.IAD.AutoRequestPropertyAttribute(OrderIndex=1)]
        public SampleSystem.SampleSystemSecurityOperationCode securityOperationCode;
    }
    
    [System.Runtime.Serialization.DataContractAttribute()]
    [Framework.DomainDriven.ServiceModel.IAD.AutoRequestAttribute()]
    public partial class HasBusinessUnitManagerCommissionLinkAccessAutoRequest
    {
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        [Framework.DomainDriven.ServiceModel.IAD.AutoRequestPropertyAttribute(OrderIndex=0)]
        public SampleSystem.Generated.DTO.BusinessUnitManagerCommissionLinkIdentityDTO businessUnitManagerCommissionLinkIdent;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        [Framework.DomainDriven.ServiceModel.IAD.AutoRequestPropertyAttribute(OrderIndex=1)]
        public SampleSystem.SampleSystemSecurityOperationCode securityOperationCode;
    }
}
