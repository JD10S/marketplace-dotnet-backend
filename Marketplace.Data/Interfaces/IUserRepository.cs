using Marketplace.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Data.Interfaces
{
    public interface IUserRepository
    {
        User? GetByEmail(string email);
        int Create(User user);

        int CreateUserWithCart(User user);
    }
}
