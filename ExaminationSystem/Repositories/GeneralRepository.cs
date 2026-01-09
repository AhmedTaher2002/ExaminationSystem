using ExaminationSystem.Data;
using ExaminationSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace ExaminationSystem.Repositories
{
    public class GeneralRepository<T> where T : BaseModel
    {
        private readonly Context _context;
        private readonly DbSet<T> _dbSet; 
        public GeneralRepository()
        {
            _context = new Context();
            _dbSet = _context.Set<T>();
        }

        public  IQueryable<T> GetAll()
        {
            var res =  _dbSet.Where(x => !x.IsDeleted);
            return res;
        }
        public IQueryable<T> GetByID(int id)
        {
            var res = _dbSet.Where(c => c.ID == id);
            return res;
        }
        public IQueryable<T> Get(Expression<Func<T,bool>> expression)
        {
            var res =  GetAll().Where(expression);
            return res;
        }

        public async Task<T> GetByIDWithTracking(int id)
        {
            var res = await _dbSet.AsTracking().Where(c => c.ID == id).FirstOrDefaultAsync();
            return res;
        }
        public async Task AddAsync(T entity)
        {
            _dbSet.Add(entity);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(T entity)
        {
            _context.Update(entity);
            await _context.SaveChangesAsync();
        }
        public async Task SoftDeleteAsync(int id)
        {
            var res = await GetByIDWithTracking(id);
            if (res == null)
                { return; }
            res.IsDeleted = true;   

            await _context.SaveChangesAsync();
        }

        public async Task HardDelete(int id)
        {

            var res = await GetByIDWithTracking(id);
            if (res == null)
                { return; }
            _dbSet.Remove(res);
            await _context.SaveChangesAsync();
        }
        public void UpdateInclude(T entity , params string[] modifiedParams)
        {
            if(!_dbSet.Any(x => x.ID == entity.ID))
                { return; }

            var local = _dbSet.Local.FirstOrDefault(x => x.ID == entity.ID);
            EntityEntry entityEntry;

            if(local == null)
            {
                entityEntry = _context.Entry(entity);
            }
            else
            {
                entityEntry = _context.ChangeTracker.Entries<T>().FirstOrDefault(x => x.Entity.ID == entity.ID);
            }

            foreach( var prop in entityEntry.Properties)
            {
                if(modifiedParams.Contains(prop.Metadata.Name))
                {
                    prop.CurrentValue = entity.GetType().GetProperty(prop.Metadata.Name).GetValue(entity);
                    prop.IsModified = true; 
                }
            }
            _context.SaveChanges();
        }
        public async Task<T> GetAllSoftDeleted()
        {
            var res = await _dbSet.Where(e => e.IsDeleted).FirstOrDefaultAsync();
            return res;
        }
        
        public async Task<T> ReturnSoftDeletedObject(int id)
        {
            var res = await _dbSet.Where(e => e.ID == id && e.IsDeleted).FirstOrDefaultAsync();
            if (res != null)
            {
                res.IsDeleted = false;
                await _context.SaveChangesAsync();
            }
            return res;
        }
        
        public async Task<T> GetSoftDeletedObject(int id)
        {
            var res = await _dbSet.Where(e => e.ID == id && e.IsDeleted).FirstOrDefaultAsync();
            return res;
        }
        public bool IsExists(int id)
        {
            var res = _dbSet.Any(e => e.ID == id && !e.IsDeleted);
            return res;
        }
        
    }
}

