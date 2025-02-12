﻿using System;
using System.CodeDom;
using System.Linq;
using System.Reflection;

using Framework.Core;
using Framework.Security;

namespace Framework.DomainDriven.DTOGenerator.Server
{
    public class PropertyAssignerConfigurator<TConfiguration> : PropertyAssignerConfiguratorBase<TConfiguration>
        where TConfiguration : class, IServerGeneratorConfigurationBase<IServerGenerationEnvironmentBase>
    {
        public PropertyAssignerConfigurator(TConfiguration configuration)
            : base(configuration)
        {
        }


        protected override CodeExpression GetPropertyHasAccessCondition(IServerPropertyAssigner propertyAssigner, PropertyInfo property, bool isEdit)
        {
            if (propertyAssigner == null) throw new ArgumentNullException(nameof(propertyAssigner));
            if (property == null) throw new ArgumentNullException(nameof(property));

            var attr = this.GetDomainObjectAttribute(propertyAssigner, property, isEdit);

            if ((attr as ViewDomainObjectAttribute).Maybe(viewAttr => viewAttr.SecondaryOperations.Any()))
            {
                throw new Exception("Secondary operations not allowed for column security");
            }

            return this.Configuration.ToHasAccessMethod(propertyAssigner.ContextRef, attr.SecurityOperationCode, propertyAssigner.DomainType, propertyAssigner.DomainParameter);
        }

        private DomainObjectAccessAttribute GetDomainObjectAttribute(IServerPropertyAssigner propertyAssigner, PropertyInfo property, bool isEdit)
        {
            if (propertyAssigner == null) throw new ArgumentNullException(nameof(propertyAssigner));
            if (property == null) throw new ArgumentNullException(nameof(property));

            if (isEdit)
            {
                return property.GetEditDomainObjectAttribute().FromMaybe(() => $"Edit operation for property \"{property.Name}\" in domainObject \"{propertyAssigner.DomainType.Name}\" not found");
            }
            else
            {
                return (property.GetViewDomainObjectAttribute() ?? propertyAssigner.DomainType.GetViewDomainObjectAttribute())

                         .FromMaybe(() => $"View operation for property \"{property.Name}\" in domainObject \"{propertyAssigner.DomainType.Name}\" not found");
            }
        }
    }
}