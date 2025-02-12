﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Framework.DomainDriven.Generation.Domain;

using JetBrains.Annotations;

namespace Framework.DomainDriven.DTOGenerator.TypeScript.Facade
{
    /// <summary>
    /// Политика для генерации DTO на основе фильтров методов клиентского сервиса
    /// </summary>
    /// <typeparam name="TConfiguration"></typeparam>
    public class DTOTypeScriptServiceGeneratePolicy<TConfiguration> : GeneratorConfigurationContainer<TConfiguration>, IGeneratePolicy<RoleFileType>
        where TConfiguration : class, ITypeScriptFacadeGeneratorConfiguration<ITypeScriptGenerationEnvironmentBase>
    {
        private readonly HashSet<Tuple<Type, RoleFileType>> cache;

        public DTOTypeScriptServiceGeneratePolicy(TConfiguration configuration)
            : base(configuration)
        {
            var methodArgTypes = from method in this.Configuration.GetFacadeMethods()

                                 from type in method.GetParametersWithExpandAutoRequest().Select(param => param.ParameterType).Concat(method.GetReturnTypeWithUnpackTask() == typeof(void) ? Type.EmptyTypes : new[] { method.GetReturnTypeWithUnpackTask() })

                                 select type;


            var unpackTypes = methodArgTypes.Distinct().GetFacadeMethodDTOTypes().ToArray();

            var request = from type in unpackTypes

                          let res = TryExtractRoleFileType(type)

                          where res != null

                          select res;

            this.cache = request.ToHashSet();
        }

        private static Tuple<Type, RoleFileType> TryExtractRoleFileType([NotNull] Type type)
        {
            if (type == null) { throw new ArgumentNullException(nameof(type)); }

            var dtoFileTypeAttr = type.GetCustomAttribute<DTOFileTypeAttribute>();

            if (dtoFileTypeAttr != null)
            {
                return Tuple.Create(dtoFileTypeAttr.DomainType, new RoleFileType(dtoFileTypeAttr.Name, dtoFileTypeAttr.Role));
            }
            else if (type.IsEnum)
            {
                return Tuple.Create(type, ClientFileType.Enum);
            }
            else
            {
                return null;
            }
        }

        public bool Used(Type domainType, RoleFileType fileType)
        {
            if (domainType == null) throw new ArgumentNullException(nameof(domainType));
            if (fileType == null) throw new ArgumentNullException(nameof(fileType));

            return this.cache.Contains(new Tuple<Type, RoleFileType>(domainType, fileType));
        }
    }
}
