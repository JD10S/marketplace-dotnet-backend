using Marketplace.Business.Interfaces;
using Marketplace.Entities.Entities;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/cart")]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    [HttpGet("{userId:int}")]
    public IActionResult GetCart(int userId)
    {
        try
        {
            var cartItems = _cartService.GetCart(userId);
            return Ok(cartItems ?? new List<CartItem>());
        }
        catch
        {
            return Ok(new List<CartItem>());
        }
    }

    [HttpPost("{userId:int}")]
    public IActionResult Add(int userId, [FromBody] CartItem item)
    {
        if (item == null || item.ProductId <= 0 || item.Quantity <= 0)
            return BadRequest("Datos del ítem inválidos");

        _cartService.AddToCart(userId, item); 
        return Ok();
    }

    [HttpPut]
    public IActionResult Update([FromBody] CartItem item)
    {
        if (item == null || item.Id <= 0)
            return BadRequest("Ítem inválido");

        _cartService.Update(item);
        return Ok();
    }

    [HttpDelete("{id:int}")]
    public IActionResult Remove(int id)
    {
        if (id <= 0)
            return BadRequest("ID inválido");

        _cartService.Remove(id);
        return Ok();
    }

    [HttpGet("total/{userId:int}")]
    public IActionResult GetTotal(int userId)
    {
        var total = _cartService.GetTotal(userId);
        return Ok(total);
    }
}