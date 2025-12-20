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
            if (item == null)
                throw new Exception("Cart item is required");

            if (item.Quantity <= 0)
                throw new Exception("Quantity must be greater than zero");

            if (item.ProductId <= 0)
                throw new Exception("ProductId is required");

            var cart = _cartRepository.GetCartByUser(userId);

            if (cart == null)
            {
                _cartRepository.CreateCart(userId);
                cart = _cartRepository.GetCartByUser(userId);

                if (cart == null)
                    throw new Exception("Unable to create cart for user");
            }

            _cartRepository.Add(cart.Id, item);
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
