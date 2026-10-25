using BLL.Interfaces;
using DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class Service<T>(IRepository<T> repository) : IService<T>
    {
        public virtual async Task CreateAsync(T item)
        {
            await repository.CreateAsync(item);
            await repository.SaveChangeAsync();
        }

        public virtual async Task DeleteAsync(int id)
        {
            T? i = await repository.GetAsync(id);
            if (i is null)
                return;
            await repository.DeleteAsync(i);
            await repository.SaveChangeAsync();
        }

        public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> pred)
            => await repository.FirstOrDefaultAsync(pred);

        public virtual async Task<IEnumerable<T>> GetAllAsync()
            => await repository.GetAllAsync();

        public virtual async Task<T?> GetAsync(int id)
            => await repository.GetAsync(id);


        public virtual async Task UpdateAsync(T item)
        {
            repository.Update(item);
            await repository.SaveChangeAsync();
        }
    }
}
