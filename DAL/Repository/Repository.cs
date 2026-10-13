using DAL.Context;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly CinemaContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(CinemaContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> pred)
            => await _dbSet.AnyAsync(pred);

        public async Task CreateAsync(T item)
            => await _dbSet.AddAsync(item);

        public async Task DeleteAsync(T item)
            => _dbSet.Remove(item);

        public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> pred)
            => await _dbSet.FirstOrDefaultAsync(pred);

        public virtual async Task<IEnumerable<T>> GetAllAsync()
            => await _dbSet.ToListAsync();

        public virtual async Task<T?> GetAsync(int id)
            => await _dbSet.FindAsync(id);

        public async Task SaveChangeAsync()
            => await _context.SaveChangesAsync();

        public void Update(T item)
            => _dbSet.Update(item);
    }
}
