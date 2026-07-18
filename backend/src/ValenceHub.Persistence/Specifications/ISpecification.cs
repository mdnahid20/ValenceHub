using System.Linq.Expressions;

namespace ValenceHub.Persistence.Specifications;

public interface ISpecification<T>
{
    Expression<Func<T, bool>>? Criteria { get; }
    List<Expression<Func<T, object>>> Includes { get; }
    Func<IQueryable<T>, IOrderedQueryable<T>>? OrderBy { get; }
    Func<IQueryable<T>, IOrderedQueryable<T>>? OrderByDescending { get; }

    int? Skip { get; }
    int? Take { get; }
    bool IsPagingEnabled { get; }
}
