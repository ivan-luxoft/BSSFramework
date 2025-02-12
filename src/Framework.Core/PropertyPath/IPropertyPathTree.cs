﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;

using JetBrains.Annotations;

namespace Framework.Core
{
    public interface IPropertyPathTree<TDomainObject> : IEnumerable<Expression<Action<IPropertyPathNode<TDomainObject>>>>
    {
    }

    public class PropertyPathTree<TDomainObject> : ReadOnlyCollection<Expression<Action<IPropertyPathNode<TDomainObject>>>>, IPropertyPathTree<TDomainObject>
    {
        public PropertyPathTree([NotNull] IEnumerable<Expression<Action<IPropertyPathNode<TDomainObject>>>> source)
            : base(source.ToList())
        {
        }

        public static readonly PropertyPathTree<TDomainObject> Empty = new PropertyPathTree<TDomainObject>(new Expression<Action<IPropertyPathNode<TDomainObject>>>[0]);
    }

    public static class PropertyPathTreeExtensions
    {
        public static IPropertyPathTree<TDomainObject> ToTree<TDomainObject>([NotNull] this IEnumerable<Expression<Action<IPropertyPathNode<TDomainObject>>>> nodes)
        {
            if (nodes == null) throw new ArgumentNullException(nameof(nodes));

            return new PropertyPathTree<TDomainObject>(nodes);
        }

        public static IPropertyPathTree<TOutput> Covariance<TDomainObject, TOutput>([NotNull] this IPropertyPathTree<TDomainObject> propertyPathTree)
            where TOutput : TDomainObject
        {
            if (propertyPathTree == null) throw new ArgumentNullException(nameof(propertyPathTree));

            return propertyPathTree.SelectMany(node => node.ToPropertyPaths())
                                   .Select(path => path.ToNode<TOutput>())
                                   .ToTree();
        }
    }
}
