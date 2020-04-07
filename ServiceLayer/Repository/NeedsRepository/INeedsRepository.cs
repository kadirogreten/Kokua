using Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer
{
    public interface INeedsRepository : IRepository<Needs>
    {
        Task<IAsyncCursor<IEnumerable<Needs>>> GetAllAsyncWithProducts(Expression<Func<Needs, bool>> predicate);
    }
}
