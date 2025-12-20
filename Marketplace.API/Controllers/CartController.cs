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
        var cartItems = _cartService.GetCart(userId);
        return Ok(cartItems);
    }

    [HttpPost("{userId:int}")]
    public IActionResult Add(int userId, [FromBody] CartItem item)
    {
        if (item == null)
            return BadRequest("Item inválido");

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
