using DataAccessLayer;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ServiceStack;
using MongoDB.Bson;

namespace ServiceLayer
{
    public abstract class GenericRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly IKokuaDbContext _context;
        public IMongoCollection<TEntity> DbSet { get; private set; }

        /// constructor dependency injection
        public GenericRepository(IKokuaDbContext context)
        {
            _context = context;
        }
        private void ConfigDbSet()
        {
            DbSet = _context.GetCollection<TEntity>(typeof(TEntity).Name);
        }

        public IEnumerable<TEntity> GetAll()
        {
            ConfigDbSet();
            var liste = DbSet.FindSync(Builders<TEntity>.Filter.Empty);
            return liste.ToEnumerable();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            ConfigDbSet();
            var liste = await DbSet.FindAsync(Builders<TEntity>.Filter.Empty);

            return await liste.ToListAsync();
        }

        public IEnumerable<TEntity> Where(Expression<Func<TEntity, bool>> predicate)
        {
            ConfigDbSet();
            var data = DbSet.Find(predicate);
            return data.ToEnumerable();
        }
        public async Task<IEnumerable<TEntity>> WhereAsync(Expression<Func<TEntity, bool>> predicate)
        {
            ConfigDbSet();
            var data = await DbSet.FindAsync(predicate);
            return await data.ToListAsync();
        }

        public void Insert(TEntity entity)
        {

            ConfigDbSet();

            _context.AddCommand(() => DbSet.InsertOneAsync(entity));
        }

        public virtual void Update(TEntity entity)
        {

            var id = entity.GetId();
            var docId = new ObjectId(id.ToString());

            ConfigDbSet();
            _context.AddCommand(() => DbSet.ReplaceOneAsync(Builders<TEntity>.Filter.Eq("_id", docId), entity));
        }

        public void Delete(TEntity entity)
        {
            var id = entity.GetId();
            var docId = new ObjectId(id.ToString());
            ConfigDbSet();
            _context.AddCommand(() => DbSet.DeleteOneAsync(Builders<TEntity>.Filter.Eq("_id", docId)));
        }

        public async Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            ConfigDbSet();
            var data = await DbSet.FindAsync(predicate);

            return await data.FirstOrDefaultAsync();
        }

        public TEntity Find(Expression<Func<TEntity, bool>> predicate)
        {
            ConfigDbSet();
            var data = DbSet.Find(predicate);

            return data.FirstOrDefault();
        }

    }
}