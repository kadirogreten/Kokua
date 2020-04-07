using DataAccessLayer;
using Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceLayer
{
    public class NeedProductsRepository : GenericRepository<NeedProducts>, INeedProductsRepository
    {

        public NeedProductsRepository(IKokuaDbContext context) : base(context)
        {

        }
        public IKokuaDbContext context { get { return _context as IKokuaDbContext; } }

        //Ekstra bir DTO veya model oluşturmamak için şimdilik değerlerimi geriye tuple olarak dönüyorum.

    }
}
