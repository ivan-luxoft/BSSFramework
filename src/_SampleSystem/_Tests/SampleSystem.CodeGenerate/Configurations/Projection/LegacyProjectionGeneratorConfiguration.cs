﻿using System;

namespace SampleSystem.CodeGenerate
{
    public class LegacyProjectionGeneratorConfiguration : Framework.DomainDriven.ProjectionGenerator.GeneratorConfigurationBase<ServerGenerationEnvironment>
    {
        public LegacyProjectionGeneratorConfiguration(ServerGenerationEnvironment environment)
            : base(environment, environment.LegacyProjectionEnvironment)
        {
        }
    }
}
