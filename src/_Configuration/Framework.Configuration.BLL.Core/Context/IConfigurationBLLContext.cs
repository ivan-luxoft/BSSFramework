﻿using System;
using System.Collections.Generic;

using Framework.Authorization.BLL;
using Framework.Core;
using Framework.Core.Serialization;
using Framework.DomainDriven.BLL.Configuration;
using Framework.DomainDriven.BLL.Security;
using Framework.DomainDriven.BLL.Tracking;

using Framework.Notification;
using Framework.Configuration.Domain;
using Framework.Persistent;

namespace Framework.Configuration.BLL
{
    public partial interface IConfigurationBLLContext :

        Framework.DomainDriven.BLL.Configuration.IConfigurationBLLContext,

        ISecurityBLLContext<IAuthorizationBLLContext, PersistentDomainObjectBase, DomainObjectBase, Guid>,

        ITypeResolverContainer<string>,

        ITrackingServiceContainer<PersistentDomainObjectBase>,

        IConfigurationBLLContextContainer<IConfigurationBLLContext>
    {
        IMessageSender<MessageTemplateNotification> SubscriptionSender { get; }

        bool SubscriptionEnabled { get; }

        ISerializerFactory<string> SystemConstantSerializerFactory { get; }

        ITypeResolver<DomainType> ComplexDomainTypeResolver { get; }

        DomainType GetDomainType(Type type, bool throwOnNotFound);

        DomainType GetDomainType(IDomainType type, bool throwOnNotFound = true);

        ISubscriptionSystemService GetSubscriptionSystemService(Type domainType);

        IPersistentTargetSystemService GetPersistentTargetSystemService(TargetSystem targetSystem);

        IEnumerable<IPersistentTargetSystemService> GetPersistentTargetSystemServices();

        ITargetSystemService GetTargetSystemService(TargetSystem targetSystem);

        ITargetSystemService GetTargetSystemService(Type domainType, bool throwOnNotFound);

        ITargetSystemService GetTargetSystemService(string name);

        ITargetSystemService GetMainTargetSystemService();

        IEnumerable<ITargetSystemService> GetTargetSystemServices();
    }
}
