using System.Linq.Expressions;

namespace OracleMES.Core.Interfaces.Repositories;

public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(decimal id);
    Task<T> AddAsync(T entity);
    Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities);
    Task<T> UpdateAsync(T entity);
    Task<IEnumerable<T>> UpdateRangeAsync(IEnumerable<T> entities);
    Task RemoveAsync(T entity);
    Task RemoveRangeAsync(IEnumerable<T> entities);
    Task SaveChangesAsync();

    Task<bool> ExistsAsync(int id);


    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    // * 뜻: Expression = 표현식, Func = 함수, T 를 인자로 받아 bool 을 반환하는 람다식. 
    // * 예시: FindAsync(x => x.Id == 1)

}
