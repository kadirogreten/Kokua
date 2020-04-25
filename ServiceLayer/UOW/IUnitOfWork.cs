using System;
using System.Threading.Tasks;
using ServiceLayer.Repository;

namespace ServiceLayer
{
    public interface IUnitOfWork : IDisposable
    {
        public IUserRepository KokuaUser { get; }
        public IUserRoleRepository KokuaRole { get; }
        public INeedProductsRepository NeedProducts { get; }
        public INeedsRepository Needs { get; }
        public IUserReportRepository UserReport { get; }
        public IOrderRepository Order { get; }

        Task<int> Complete();
    }
}