using System.Linq.Expressions;
using OracleMES.Core.Interfaces.Repositories;
using OracleMES.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace OracleMES.Infrastructure.Repositories;

public class Repository<T>(MesDbContext context) : IRepository<T> where T : class
{
    protected readonly MesDbContext _context = context;
    protected readonly DbSet<T> _entities = context.Set<T>();

    public Task AddArrangeAsync(IEnumerable<T> entities)
    {
        throw new NotImplementedException();
    }

    public Task<T> CreateAsync(T entity)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<T>> CreateRangeAsync(IEnumerable<T> entities)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(T entity)
    {
        throw new NotImplementedException();
    }

    public Task DeleteRangeAsync(IEnumerable<T> entities)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<T>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<T> GetByIdAsync(int id)
    {
        return await _context.Set<T>().FindAsync(id);
    }

    public Task<bool> SaveChangesAsync()
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(T entity)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<T>> UpdateRangeAsync(IEnumerable<T> entities)
    {
        throw new NotImplementedException();
    }
}