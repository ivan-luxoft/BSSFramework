﻿using System;

using Framework.DomainDriven.Generation.Domain;

namespace Framework.DomainDriven.BLLGenerator
{
    public abstract class FileFactory<TConfiguration> : CodeFileFactory<TConfiguration, FileType>
        where TConfiguration : class, IGeneratorConfigurationBase<IGenerationEnvironmentBase>
    {
        protected FileFactory(TConfiguration configuration, Type domainType)
            : base(configuration, domainType)
        {
        }
    }
}