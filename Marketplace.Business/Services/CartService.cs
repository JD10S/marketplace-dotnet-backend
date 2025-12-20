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

        public void AddToCart(CartItem item)
        {
            if (item.Quantity <= 0)
                throw new Exception("Quantity must be greater than zero");

            _cartRepository.Add(item);
        }

        public void UpdateQuantity(int id, int quantity)
        {
            if (quantity <= 0)
                throw new Exception("Quantity must be greater than zero");

            _cartRepository.UpdateQuantity(id, quantity);
        }

        public void Remove(int id)
        {
            _cartRepository.Remove(id);
        }

        public decimal GetTotal(int userId)
        {
            var items = _cartRepository.GetByUser(userId);
            return items.Sum(i => (i.UnitPrice ?? 0m) * i.Quantity);
        }

        public void Update(CartItem item)
        {
            if (item.Quantity <= 0)
                throw new Exception("Quantity must be greater than zero");

            _cartRepository.Update(item);
        }

    }
}
