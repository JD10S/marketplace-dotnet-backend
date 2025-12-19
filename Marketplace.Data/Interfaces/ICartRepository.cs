using Marketplace.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Data.Interfaces
{
    public interface ICartRepository
    {
        IEnumerable<CartItem> GetByUser(int userId);
        void Add(CartItem item);
        void UpdateQuantity(int id, int quantity);
        void Remove(int id);
        void Update(CartItem item);
    }
}
