﻿using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Framework.Core;

namespace Framework.DomainDriven.BLL
{
    public class OverrideHasFlagVisitor : ExpressionVisitor
    {
        private static readonly MethodInfo EnumHasFlagMethod = new Func<Enum, bool>(BLLBaseOperation.Save.HasFlag).Method;


        private OverrideHasFlagVisitor()
        {

        }


        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            var request = from _ in Maybe.Return()

                          where node.Method == EnumHasFlagMethod

                          from argEnumConstValue in node.Arguments.Single().ExtractBoxingValue().GetDeepMemberConstValue()

                          let underType = node.Object.Type.GetEnumUnderlyingType()

                          let argUnderConstValue = Convert.ChangeType(argEnumConstValue, underType)

                          let argUnderConst = Expression.Constant(argUnderConstValue)

                          select Expression.MakeBinary(ExpressionType.Equal, Expression.MakeBinary(ExpressionType.And, Expression.Convert(node.Object, underType), argUnderConst), argUnderConst);


            return request.GetValueOrDefault(() => base.VisitMethodCall(node));
        }


        public readonly static OverrideHasFlagVisitor Value = new OverrideHasFlagVisitor();
    }
}