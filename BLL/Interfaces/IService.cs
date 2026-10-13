using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IService<T>
    {
        Task CreateAsync(T item);
        Task UpdateAsync(T item);
        Task<(bool, string)> DeleteAsync(int id);
        Task<T?> GetAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> pred);
    }
}
