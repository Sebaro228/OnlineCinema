using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IRepository<T>
    {
        Task CreateAsync(T item);
        void Update(T item);
        Task DeleteAsync(T item);
        Task<T?> GetAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> pred);
        Task<bool> AnyAsync(Expression<Func<T, bool>> pred);
        Task SaveChangeAsync();
    }
}
