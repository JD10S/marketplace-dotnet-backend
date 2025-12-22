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
        void Add(int cartId, CartItem item);
        void Update(CartItem item);
        void Remove(int id);
        Cart? GetCartByUser(int userId);
        void CreateCart(int userId);
        void UpdateQuantity(int id, int quantity);

        int GetOrCreateCartId(int userId);
    }
}
