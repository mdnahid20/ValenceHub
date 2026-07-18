using Microsoft.EntityFrameworkCore;

namespace ValenceHub.Persistence.Specifications;

public static class SpecificationEvaluator<TEntity> where TEntity : class
{
    public static IQueryable<TEntity> GetQuery(IQueryable<TEntity> inputQuery, ISpecification<TEntity>? spec)
    {
        if (spec == null) return inputQuery;

        var query = inputQuery;

        if (spec.Criteria != null)
            query = query.Where(spec.Criteria);

        if (spec.OrderBy != null)
            query = spec.OrderBy(query);

        if (spec.OrderByDescending != null)
            query = spec.OrderByDescending(query);

        if (spec.Includes != null)
            spec.Includes.ForEach(include => query = query.Include(include));

        if (spec.IsPagingEnabled)
        {
            if (spec.Skip.HasValue)
                query = query.Skip(spec.Skip.Value);
            if (spec.Take.HasValue)
                query = query.Take(spec.Take.Value);
        }

        return query;
    }
}
