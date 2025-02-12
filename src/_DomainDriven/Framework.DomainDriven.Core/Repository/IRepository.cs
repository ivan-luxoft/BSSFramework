﻿#nullable enable

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using nuSpec.Abstraction;

namespace Framework.DomainDriven.Repository;

public interface IRepository<TDomainObject, in TIdent>
{
    Task SaveAsync(TDomainObject domainObject, CancellationToken cancellationToken);

    Task InsertAsync(TDomainObject domainObject, TIdent id, CancellationToken cancellationToken);

    Task RemoveAsync(TDomainObject domainObject, CancellationToken cancellationToken);

    IQueryable<TDomainObject> GetQueryable();

    /// <summary>
    /// Get Queryable by Specification https://github.com/NikitaEgorov/nuSpec
    /// </summary>
    IQueryable<TProjection> GetQueryable<TProjection>(Specification<TDomainObject, TProjection> specification);

    /// <summary>
    /// Get Single or default by Specification https://github.com/NikitaEgorov/nuSpec
    /// </summary>
    Task<TProjection?> SingleOrDefaultAsync<TProjection>(
            Specification<TDomainObject, TProjection> specification,
            CancellationToken cancellationToken);

    /// <summary>
    /// Get Single by Specification https://github.com/NikitaEgorov/nuSpec
    /// </summary>
    Task<TProjection> SingleAsync<TProjection>(
            Specification<TDomainObject, TProjection> specification,
            CancellationToken cancellationToken);

    /// <summary>
    /// Get First or default by Specification https://github.com/NikitaEgorov/nuSpec
    /// </summary>
    Task<TProjection?> FirstOrDefaultAsync<TProjection>(
            Specification<TDomainObject, TProjection> specification,
            CancellationToken cancellationToken);

    /// <summary>
    /// Get First by Specification https://github.com/NikitaEgorov/nuSpec
    /// </summary>
    Task<TProjection> FirstAsync<TProjection>(
            Specification<TDomainObject, TProjection> specification,
            CancellationToken cancellationToken);

    /// <summary>
    /// Get Count by Specification https://github.com/NikitaEgorov/nuSpec
    /// </summary>
    Task<int> CountAsync<TProjection>(
            Specification<TDomainObject, TProjection> specification,
            CancellationToken cancellationToken);

    /// <summary>
    /// Get Future query by Specification https://github.com/NikitaEgorov/nuSpec
    /// </summary>
    INuFutureEnumerable<TProjection> GetFuture<TProjection>(Specification<TDomainObject, TProjection> specification);

    /// <summary>
    /// Get Future value by Specification https://github.com/NikitaEgorov/nuSpec
    /// </summary>
    INuFutureValue<TProjection> GetFutureValue<TProjection>(Specification<TDomainObject, TProjection> specification);

    /// <summary>
    /// Get Future count by Specification https://github.com/NikitaEgorov/nuSpec
    /// </summary>
    INuFutureValue<int> GetFutureCount<TProjection>(Specification<TDomainObject, TProjection> specification);

    /// <summary>
    /// Get list by Specification https://github.com/NikitaEgorov/nuSpec
    /// </summary>
    Task<IList<TProjection>> GetListAsync<TProjection>(
            Specification<TDomainObject, TProjection> specification,
            CancellationToken cancellationToken);
}
