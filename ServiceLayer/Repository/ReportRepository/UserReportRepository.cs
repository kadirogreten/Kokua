using DataAccessLayer;
using Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceLayer.Repository
{
    public class UserReportRepository:GenericRepository<UserReport>,IUserReportRepository
    {
        public UserReportRepository(IKokuaDbContext context) : base(context)
        {

        }
        public IKokuaDbContext context { get { return _context as IKokuaDbContext; } }
    }
}
