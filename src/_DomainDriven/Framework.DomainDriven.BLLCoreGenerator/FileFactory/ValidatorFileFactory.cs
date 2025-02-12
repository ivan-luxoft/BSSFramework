﻿using System;
using System.CodeDom;
using System.Collections.Generic;

using Framework.CodeDom;
using Framework.Core;
using Framework.Validation;

namespace Framework.DomainDriven.BLLCoreGenerator
{
    public class ValidatorFileFactory<TConfiguration> : FileFactory<TConfiguration>
        where TConfiguration : class, IGeneratorConfigurationBase<IGenerationEnvironmentBase>
    {
        public ValidatorFileFactory(TConfiguration configuration)
            : base(configuration, null)
        {
        }

        public override FileType FileType => FileType.Validator;

        protected override CodeTypeDeclaration GetCodeTypeDeclaration()
        {
            return new CodeTypeDeclaration
            {
                Name = this.Name,
                Attributes = MemberAttributes.Public,
                IsPartial = true,
            };
        }

        protected override IEnumerable<CodeTypeReference> GetBaseTypes()
        {
            yield return this.Configuration.GetCodeTypeReference(this.DomainType, FileType.ValidatorBase);
        }

        protected override IEnumerable<CodeTypeMember> GetMembers()
        {
            foreach (var member in base.GetMembers())
            {
                yield return member;
            }

            {
                var contextParameter = this.Configuration.BLLContextInterfaceTypeReference.ToParameterDeclarationExpression("context");
                var cacheParameter = typeof(ValidatorCompileCache).ToTypeReference().ToParameterDeclarationExpression("cache");

                yield return new CodeConstructor
                {
                    Attributes = MemberAttributes.Public,
                    Parameters = { contextParameter, cacheParameter },
                    BaseConstructorArgs = { contextParameter.ToVariableReferenceExpression(), cacheParameter.ToVariableReferenceExpression() }
                };
            }
        }
    }
}