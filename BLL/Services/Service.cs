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
        public async Task CreateAsync(T item)
        {
            await repository.CreateAsync(item);
            await repository.SaveChangeAsync();
        }

        public async Task<(bool, string)> DeleteAsync(int id)
        {
            var i = await repository.GetAsync(id);
            if (i == null)
                return (false, "Error");

            await repository.DeleteAsync(i);
            await repository.SaveChangeAsync();
            return (true, "Success");
        }

        public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> pred)
            => await repository.FirstOrDefaultAsync(pred);

        public async Task<IEnumerable<T>> GetAllAsync()
            => await repository.GetAllAsync();

        public async Task<T?> GetAsync(int id)
            => await repository.GetAsync(id);


        public async Task UpdateAsync(T item)
        {
            repository.Update(item);
            await repository.SaveChangeAsync();
        }
    }
}
