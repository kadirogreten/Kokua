using System;
using System.Threading.Tasks;
using ServiceLayer.Repository;

namespace ServiceLayer
{
    public interface IUnitOfWork : IDisposable
    {
        public IUserRepository KokuaUser { get; }
        public IUserRoleRepository KokuaRole { get; }

        Task<int> Complete();
    }
}