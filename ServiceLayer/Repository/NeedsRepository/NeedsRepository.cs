using DataAccessLayer;
using Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer
{
    public class NeedsRepository : GenericRepository<Needs>, INeedsRepository
    {

        public NeedsRepository(IKokuaDbContext context) : base(context)
        {

        }
        public IKokuaDbContext context { get { return _context as IKokuaDbContext; } }

        public async Task<IAsyncCursor<IEnumerable<Needs>>> GetAllAsyncWithProducts(Expression<Func<Needs, bool>> predicate)
        {
            var dbset = _context.GetCollection<Needs>("Needs");

            var data = await dbset.FindAsync<IEnumerable<Needs>>(predicate);

            return data;
        }

        //Ekstra bir DTO veya model oluşturmamak için şimdilik değerlerimi geriye tuple olarak dönüyorum.

    }
}
