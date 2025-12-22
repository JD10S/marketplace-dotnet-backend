using Marketplace.Business.Interfaces;
using Marketplace.Data.Interfaces;
using Marketplace.Entities.Entities;

namespace Marketplace.Business.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;

        public CartService(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        public IEnumerable<CartItem> GetCart(int userId)
        {
            return _cartRepository.GetByUser(userId);
        }

        public void AddToCart(int userId, CartItem item)
        {
         

            var cartId = _cartRepository.GetOrCreateCartId(userId);
            Console.WriteLine($"CARRITO CREADO/BUSCADO PARA USER {userId}: cartId = {cartId}");

            _cartRepository.Add(cartId, item);
        }

        public void UpdateQuantity(int id, int quantity)
        {
            if (id <= 0)
                throw new Exception("Invalid cart item id");

            if (quantity <= 0)
                throw new Exception("Quantity must be greater than zero");

            _cartRepository.UpdateQuantity(id, quantity);
        }

        public void Update(CartItem item)
        {
            if (item == null)
                throw new Exception("Cart item is required");

            if (item.Id <= 0)
                throw new Exception("Invalid cart item id");

            if (item.Quantity <= 0)
                throw new Exception("Quantity must be greater than zero");

            _cartRepository.Update(item);
        }

        public void Remove(int id)
        {
            if (id <= 0)
                throw new Exception("Invalid cart item id");

            _cartRepository.Remove(id);
        }

        public decimal GetTotal(int userId)
        {
            var items = _cartRepository.GetByUser(userId);

            return items.Sum(i => (i.UnitPrice ?? 0m) * i.Quantity);
        }
    }
}
