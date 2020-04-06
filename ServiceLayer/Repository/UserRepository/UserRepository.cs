using System;
using System.Linq;
using System.Text;
using DataAccessLayer;
using Models;

namespace ServiceLayer.Repository {
    public class UserRepository : GenericRepository<KokuaUser>, IUserRepository {

        public UserRepository (IKokuaDbContext context) : base (context) {

        }
        public IKokuaDbContext context { get { return _context as IKokuaDbContext; } }

        //Ekstra bir DTO veya model oluşturmamak için şimdilik değerlerimi geriye tuple olarak dönüyorum.

    }
}