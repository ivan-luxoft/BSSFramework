﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Framework.Core;
using Framework.Persistent;


namespace Framework.SecuritySystem.Rules.Builders.MaterializedPermissions
{
    public class SecurityExpressionFilter<TPersistentDomainObjectBase, TDomainObject, TSecurityOperationCode, TIdent> : ISecurityExpressionFilter<TDomainObject>

        where TPersistentDomainObjectBase : class, IIdentityObject<TIdent>
        where TDomainObject : class, TPersistentDomainObjectBase
        where TSecurityOperationCode : struct, Enum
    {
        private readonly Lazy<Expression<Func<TDomainObject, bool>>> optimizedLazyExpression;

        private readonly Lazy<Func<TDomainObject, IEnumerable<string>>> getAccessorsFunc;

        private static readonly ILambdaCompileCache LambdaCompileCache = new LambdaCompileCache(LambdaCompileMode.All);

        public SecurityExpressionFilter(
            SecurityExpressionBuilderBase<TPersistentDomainObjectBase, TDomainObject, TIdent> builder,
            ContextSecurityOperation<TSecurityOperationCode> securityOperation)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (securityOperation == null) throw new ArgumentNullException(nameof(securityOperation));

            var usedTypes = builder.GetUsedTypes().Distinct();

            var permissions = builder.Factory.AuthorizationSystem.GetPermissions(securityOperation, usedTypes);

            var filterExpression = builder.GetSecurityFilterExpression(permissions);

            this.InjectFunc = q => q.Where(filterExpression);

            this.optimizedLazyExpression = LazyHelper.Create(() => filterExpression.UpdateBody(OptimizeContainsCallVisitor<TIdent>.Value));

            this.HasAccessFunc = filterExpression.Compile(LambdaCompileCache);

            this.getAccessorsFunc = LazyHelper.Create(() => FuncHelper.Create((TDomainObject domainObject) =>
            {
                var baseFilter = builder.GetAccessorsFilterMany(domainObject, securityOperation.SecurityExpandType);

                var filter = baseFilter.OverrideInput((IPrincipal<TIdent> principal) => principal.Permissions);

                return builder.Factory.AuthorizationSystem.GetAccessors(securityOperation.Code, filter);
            }));
        }


        public Expression<Func<TDomainObject, bool>> Expression => this.optimizedLazyExpression.Value;

        public Func<IQueryable<TDomainObject>, IQueryable<TDomainObject>> InjectFunc { get; }

        public Func<TDomainObject, bool> HasAccessFunc { get; }

        public IEnumerable<string> GetAccessors(TDomainObject domainObject)
        {
            return this.getAccessorsFunc.Value(domainObject);
        }
    }
}
