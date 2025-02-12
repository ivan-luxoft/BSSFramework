﻿using System;

namespace Framework.DomainDriven.BLLCoreGenerator
{
    public enum FileType
    {
        BLLContext,

        BLLContextInterface,

        SecurityOperation,

        RootSecurityServicePathContainerInterface,

        RootSecurityServiceInterface,

        RootSecurityService,

        RootSecurityServiceBase,



        DomainSecurityService,




        SecurityPath,


        DomainBLLBase,

        SecurityDomainBLLBase,



        DefaultOperationDomainBLLBase,

        DefaultOperationSecurityDomainBLLBase,




        BLLInterface,

        BLLFactoryInterface,

        BLLFactoryContainerInterface,


        DomainObjectMappingService,

        SecurityHierarchyDomainBLLBase,


        ValidationMapBase,

        ValidationMap,


        ValidatorBase,

        Validator,



        MainFetchServiceBase,

        MainFetchService
    }
}
