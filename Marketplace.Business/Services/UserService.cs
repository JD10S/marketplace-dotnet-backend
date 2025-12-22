using Marketplace.Business.Interfaces;
using Marketplace.Data.Interfaces;
using Marketplace.Entities.Entities;
using System.Security.Cryptography;
using System.Text;
namespace Marketplace.Business.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICartRepository _cartRepository;

        public UserService(IUserRepository userRepository, ICartRepository cartRepository)
        {
            _userRepository = userRepository;
            _cartRepository = cartRepository;
        }

        public void Register(User user)
        {
            if (string.IsNullOrWhiteSpace(user.Email))
                throw new Exception("Email is required");

            if (string.IsNullOrWhiteSpace(user.Password))
                throw new Exception("Password is required");

            var existing = _userRepository.GetByEmail(user.Email);
            if (existing != null)
                throw new Exception("User already exists");

            user.CreatedAt = DateTime.UtcNow;

            var userId = _userRepository.Create(user);

            _cartRepository.CreateCart(userId);
        }

        public User Login(string email, string password)
        {
            var user = _userRepository.GetByEmail(email);
            if (user == null)
                throw new Exception("Invalid credentials");

            Console.WriteLine($"PASSWORD RECIBIDO: '{password}'");

            if (user.Password    != password)
                throw new Exception("Invalid credentials");

            return user;
        }

        public void AddToCart(int userId, CartItem item)
        {
            var cartId = _cartRepository.GetOrCreateCartId(userId);
            _cartRepository.Add(cartId, item);
        }

        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}
