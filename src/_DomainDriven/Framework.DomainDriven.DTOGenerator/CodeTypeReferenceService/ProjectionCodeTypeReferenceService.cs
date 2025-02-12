﻿using System;
using System.Linq;
using System.Reflection;

using Framework.Core;

using JetBrains.Annotations;

namespace Framework.DomainDriven.DTOGenerator
{
    public class ProjectionCodeTypeReferenceService<TConfiguration> : MainCodeTypeReferenceService<TConfiguration>
        where TConfiguration : class, IGeneratorConfigurationBase<IGenerationEnvironmentBase>
    {
        public ProjectionCodeTypeReferenceService(TConfiguration configuration)
            : base(configuration)
        {
        }


        public override Type CollectionType => this.Configuration.ClientEditCollectionType;


        public override RoleFileType GetReferenceFileType(PropertyInfo property)
        {
            if (property == null) throw new ArgumentNullException(nameof(property));

            if (this.Configuration.ProjectionTypes.Contains(property.PropertyType))
            {
                return FileType.ProjectionDTO;
            }
            else
            {
                return base.GetReferenceFileType(property);
            }
        }

        public override RoleFileType GetCollectionFileType([NotNull] PropertyInfo property)
        {
            if (property == null) throw new ArgumentNullException(nameof(property));

            if (this.Configuration.ProjectionTypes.Contains(property.PropertyType.GetCollectionElementType()))
            {
                return FileType.ProjectionDTO;
            }
            else
            {
                return base.GetReferenceFileType(property);
            }
        }
    }
}
