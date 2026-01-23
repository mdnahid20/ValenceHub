using System.Linq.Expressions;

namespace ValenceHub.Persistence.Specifications;

public abstract class Specification<T> : ISpecification<T>
{
    protected Specification()
    {
        Includes = new List<Expression<Func<T, object>>>();
    }

    public Expression<Func<T, bool>>? Criteria { get; protected set; }
    public List<Expression<Func<T, object>>> Includes { get; }
    public Func<IQueryable<T>, IOrderedQueryable<T>>? OrderBy { get; protected set; }
    public Func<IQueryable<T>, IOrderedQueryable<T>>? OrderByDescending { get; protected set; }
    public int? Skip { get; protected set; }
    public int? Take { get; protected set; }
    public bool IsPagingEnabled => Skip.HasValue || Take.HasValue;

    protected void AddInclude(Expression<Func<T, object>> includeExpression) => Includes.Add(includeExpression);
    protected void ApplyCriteria(Expression<Func<T, bool>> criteria) => Criteria = criteria;
    protected void ApplyOrderBy(Func<IQueryable<T>, IOrderedQueryable<T>> orderByExpression) => OrderBy = orderByExpression;
    protected void ApplyOrderByDescending(Func<IQueryable<T>, IOrderedQueryable<T>> orderByDescExpression) => OrderByDescending = orderByDescExpression;

    protected void ApplyPaging(int skip, int take)
    {
        Skip = skip;
        Take = take;
    }
}