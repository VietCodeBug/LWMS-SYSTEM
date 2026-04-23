using System.Linq.Expressions;
using LWMS.Application.Common.Interfaces;
using LWMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LWMS.Infrastructure.Repositories;
public class RepositoryBase<T> :IRepository<T> where T :class
{
    protected readonly AppDbContext _Context;
    protected readonly DbSet<T> _dbSet;
    public RepositoryBase(AppDbContext context)
    {
        _Context = context;
        _dbSet = _Context.Set<T>();
    }
    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }
    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }
    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.Where(predicate).ToListAsync();
    }
    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
       
    }
    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }
    public void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }

    public IQueryable<T> Query()
    {
        return _dbSet.AsQueryable();
    }

}