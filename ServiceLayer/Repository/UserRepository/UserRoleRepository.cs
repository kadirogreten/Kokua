using DataAccessLayer;
using Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceLayer
{
    public class UserRoleRepository : GenericRepository<KokuaRole>, IUserRoleRepository
    {
        public UserRoleRepository(IKokuaDbContext context) : base(context)
        {

        }
        public IKokuaDbContext context { get { return _context as IKokuaDbContext; } }
    }
}
