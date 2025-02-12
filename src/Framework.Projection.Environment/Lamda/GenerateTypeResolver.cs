﻿using System;
using System.Collections.Generic;

using Framework.Core;

using JetBrains.Annotations;

namespace Framework.Projection.Lambda
{
    internal class GenerateTypeResolver : ITypeResolver<IProjection>
    {
        private readonly ProjectionLambdaEnvironment environment;

        private readonly Dictionary<IProjection, GeneratedType> generateTypes = new Dictionary<IProjection, GeneratedType>();


        public GenerateTypeResolver([NotNull] ProjectionLambdaEnvironment environment)
        {
            this.environment = environment ?? throw new ArgumentNullException(nameof(environment));
        }


        public Type Resolve([NotNull] IProjection projection)
        {
            if (projection == null) throw new ArgumentNullException(nameof(projection));

            return this.generateTypes.GetValueOrDefault(projection) ?? new GeneratedType(this.environment, projection, this.generateTypes);
        }

        public IEnumerable<Type> GetTypes()
        {
            return this.generateTypes.Values;
        }
    }
}
