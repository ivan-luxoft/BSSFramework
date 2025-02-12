﻿using System;
using System.Dynamic;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;

namespace Framework.Core
{
    public static class CoreObjectExtensions
    {
        private static readonly MethodInfo DefaultToStringMethod = new Func<string>(new object().ToString).Method;

        private static readonly IDictionaryCache<Type, bool> IsBaseToStringDict =
            new DictionaryCache<Type, bool>(
                    t => t.GetMethods()
                          .Where(m => m.DeclaringType == typeof(object))
                          .Select(m => m.GetBaseDefinition())
                          .Contains(DefaultToStringMethod))
                .WithLock();

        [Obsolete("v10 This method will be protected in future")]
        public static string ToFormattedString(this object source, string typeName = null)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            var sourceType = source.GetType();
            var printTypeName = typeName ?? sourceType.Name;

            return IsBaseToStringDict[sourceType]
                       ? printTypeName
                       : $"{printTypeName} ({source})";
        }
    }
}
