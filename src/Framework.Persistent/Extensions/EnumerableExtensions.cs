﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Framework.Persistent
{
    public static class EnumerableExtensions
    {
        public static TSource GetById<TSource, TIdent>(this IEnumerable<TSource> source, TIdent id, bool raiseIfNotFound = true)
            where TSource : class, IIdentityObject<TIdent>
            where TIdent : IEquatable<TIdent>
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            var res = source.SingleOrDefault(obj => EqualityComparer<TIdent>.Default.Equals(obj.Id, id));

            if (res == null && raiseIfNotFound)
            {
                throw GetNotFoundException<TSource>(id.ToString(), nameof(id));
            }

            return res;
        }

        public static TSource GetByIdentity<TSource, TIdentity>(this IEnumerable<TSource> source, TIdentity identity, bool raiseIfNotFound = true)
            where TSource : class, IIdentityObjectContainer<TIdentity>
            where TIdentity : IEquatable<TIdentity>
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            var res = source.SingleOrDefault(obj => EqualityComparer<TIdentity>.Default.Equals(obj.Identity, identity));

            if (res == null && raiseIfNotFound)
            {
                throw GetNotFoundException<TSource>(identity.ToString(), nameof(identity));
            }

            return res;
        }

        public static TSource GetByType<TSource, TType>(this IEnumerable<TSource> source, TType type, bool raiseIfNotFound = true)
            where TSource : class, ITypeObject<TType>
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            var res = source.SingleOrDefault(obj => EqualityComparer<TType>.Default.Equals(obj.Type, type));

            if (res == null && raiseIfNotFound)
            {
                throw GetNotFoundException<TSource>(type.ToString(), nameof(type));
            }

            return res;
        }


        public static TSource GetByName<TSource>(this IEnumerable<TSource> source, string name, bool raiseIfNotFound = true)
           where TSource : class, IVisualIdentityObject
        {
            return source.GetByName(name, StringComparison.CurrentCultureIgnoreCase, raiseIfNotFound);
        }

        public static TSource GetByName<TSource>(this IEnumerable<TSource> source, string name, Func<Exception> getFaultException)
            where TSource : class, IVisualIdentityObject
        {
            return source.GetByName(name, StringComparison.CurrentCultureIgnoreCase, getFaultException);
        }


        public static TSource GetByName<TSource>(this IEnumerable<TSource> source, string name, StringComparison stringComparison, bool raiseIfNotFound = true)
            where TSource : class, IVisualIdentityObject
        {
            return source.GetByName(name, stringComparison, TryGetNotFoundByNameExceptionFactory<TSource>(name, raiseIfNotFound));
        }

        public static TSource GetByName<TSource>(this IEnumerable<TSource> source, string name, StringComparison stringComparison, Func<Exception> getFaultException)
            where TSource : class, IVisualIdentityObject
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (name == null) throw new ArgumentNullException(nameof(name));

            var res = source.SingleOrDefault(obj => string.Equals(obj.Name, name, stringComparison));

            if (res == null && getFaultException != null)
            {
                throw getFaultException();
            }

            return res;
        }


        private static Func<Exception> TryGetNotFoundByNameExceptionFactory<TSource>(string name, bool raiseIfNotFound)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            return raiseIfNotFound ? () => GetNotFoundException<TSource>(name, nameof(name)) : default(Func<Exception>);
        }

        private static Exception GetNotFoundException<TSource>(string identity, string identityName)
        {
            throw new Exception($"{typeof (TSource).Name} with {identityName} \"{identity}\" not found");
        }
    }
}
