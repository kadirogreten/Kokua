using DataAccessLayer;
using Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceLayer.Repository
{
    public class OrderRepository:GenericRepository<Order>,IOrderRepository
    {
        public OrderRepository(IKokuaDbContext context):base(context)
        {

        }

        public IKokuaDbContext context { get { return _context as IKokuaDbContext; } }
    }
}
