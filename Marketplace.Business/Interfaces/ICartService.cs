using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marketplace.Entities.Entities;

namespace Marketplace.Business.Interfaces
{
    public interface ICartService
    {
        IEnumerable<CartItem> GetCart(int userId);
        void AddToCart(int userId, CartItem item);   
        void UpdateQuantity(int id, int quantity);
        void Remove(int id);
        decimal GetTotal(int userId);
        void Update(CartItem item);
        
    }
}
