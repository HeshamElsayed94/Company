using System.Linq.Expressions;
using Contracts;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public abstract class RepositoryBase<T>(RepositoryContext context) : IRepositoryBase<T> where T : class
{

    public void Create(T entity) => context.Add(entity);

    public void Delete(T entity) => context.Remove(entity);

    public IQueryable<T> FindAll(bool trackChanges)
        => !trackChanges
        ? context.Set<T>().AsNoTrackingWithIdentityResolution()
        : context.Set<T>();



    public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges)
        => !trackChanges
        ? context.Set<T>().Where(expression)
        .AsNoTrackingWithIdentityResolution()
        : context.Set<T>().Where(expression);



    public void Update(T entity) => context.Update(entity);
}