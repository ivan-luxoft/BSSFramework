﻿using System;
using System.CodeDom;
using System.Collections.Generic;

using Framework.CodeDom;
using Framework.DomainDriven.Generation;
using Framework.DomainDriven.Generation.Domain;

namespace Framework.DomainDriven.BLLCoreGenerator
{
    public class BLLFactoryContainerInterfaceGeneratorConfiguration<TConfiguration> : GeneratorConfigurationContainer<TConfiguration>, IBLLFactoryContainerInterfaceGeneratorConfiguration
        where TConfiguration : class, IGeneratorConfigurationBase<IGenerationEnvironmentBase>
    {
        public BLLFactoryContainerInterfaceGeneratorConfiguration(TConfiguration configuration)
            : base(configuration)
        {

        }


        public IEnumerable<ICodeFile> GetFileFactories()
        {
            yield return new BLLFactoryContainerInterfaceFileFactory<TConfiguration>(this.Configuration);

            foreach (var domainType in this.Configuration.BLLDomainTypes)
            {
                yield return new BLLInterfaceFileFactory<TConfiguration>(this.Configuration, domainType);
                yield return new BLLFactoryInterfaceFileFactory<TConfiguration>(this.Configuration, domainType);
            }
        }

        public CodeExpression GetCreateSecurityBLLExpr(CodeExpression logicExpressionSource, Type domainType, object securitySource)
        {
            if (logicExpressionSource == null) throw new ArgumentNullException(nameof(logicExpressionSource));
            if (domainType == null) throw new ArgumentNullException(nameof(domainType));

            if (securitySource == null)
            {
                return logicExpressionSource.ToPropertyReference(domainType.Name);
            }
            else if (securitySource is Enum)
            {
                return logicExpressionSource.ToPropertyReference(domainType.Name + "Factory")
                                            .ToMethodInvokeExpression("Create", (securitySource as Enum).ToPrimitiveExpression());
            }
            else if (securitySource is CodeExpression)
            {
                return logicExpressionSource.ToPropertyReference(domainType.Name + "Factory")
                                            .ToMethodInvokeExpression("Create", securitySource as CodeExpression);
            }
            else
            {
                throw new System.NotImplementedException();
            }
        }
    }
}