﻿using System;
using System.Collections.Generic;
using System.Linq;

using Framework.Core;

using JetBrains.Annotations;

namespace Framework.Projection.Lambda
{
    internal static class ProjectionExtensions
    {
        public static T GetSecurityProjection<T>([NotNull] this IEnumerable<T> source, [NotNull] Type arg)
            where T : IProjection
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (arg == null) throw new ArgumentNullException(nameof(arg));

            return source.GetProjectionByRole(arg, ProjectionRole.SecurityNode);
        }

        public static T GetProjectionByRole<T>([NotNull] this IEnumerable<T> source, [NotNull] Type sourceType, ProjectionRole role)
            where T : IProjection
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (sourceType == null) throw new ArgumentNullException(nameof(sourceType));

            return source.SingleOrDefault(projection => projection.SourceType == sourceType && projection.Role == role);
        }

        public static IEnumerable<IProjection> GetAllProjections(this IEnumerable<IProjection> projections)
        {
            if (projections == null) throw new ArgumentNullException(nameof(projections));

            var allProjections = new HashSet<IProjection>();

            projections.Foreach(p => p.FillProjectionList(allProjections));

            return allProjections;
        }

        private static void FillProjectionList([NotNull] this IProjection projection, HashSet<IProjection> allProjections)
        {
            if (projection == null) throw new ArgumentNullException(nameof(projection));
            if (allProjections == null) throw new ArgumentNullException(nameof(allProjections));

            if (allProjections.Add(projection))
            {
                projection.Properties.Where(prop => prop.Type.ElementProjection != null).Foreach(prop => prop.Type.ElementProjection.FillProjectionList(allProjections));
            }
        }
    }
}
