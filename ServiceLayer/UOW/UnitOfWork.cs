using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer;
using ServiceLayer.Repository;
using ServiceLayer;
using Models;

namespace ServiceLayer
{
    public class UnitOfWork : IUnitOfWork
    {

        protected readonly IKokuaDbContext _context;

        public UnitOfWork(IKokuaDbContext context)
        {
            _context = context;
            

            KokuaUser = new UserRepository(_context);
            KokuaRole = new UserRoleRepository(_context);

        }


        public IUserRepository KokuaUser { get; private set; }
        public IUserRoleRepository KokuaRole { get; private set; }

        public async Task<int> Complete()
        {
            return await _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();

        }
    }
}