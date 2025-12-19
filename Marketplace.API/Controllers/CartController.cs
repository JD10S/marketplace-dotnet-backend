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

    [HttpGet("{userId}")]
    public IActionResult GetCart(int userId)
    {
        return Ok(_cartService.GetCart(userId));
    }

    [HttpPost]
    public IActionResult Add([FromBody] CartItem item)
    {
        _cartService.AddToCart(item);
        return Ok();
    }

    [HttpPut("{id}")]
    public IActionResult UpdateQuantity(int id, [FromQuery] int quantity)
    {
        _cartService.UpdateQuantity(id, quantity);
        return Ok();
    }


    [HttpPut]
    public IActionResult Update([FromBody] CartItem item)
    {
        _cartService.Update(item);
        return Ok();
    }





    [HttpDelete("{id}")]
    public IActionResult Remove(int id)
    {
        _cartService.Remove(id);
        return Ok();
    }

    [HttpGet("total/{userId}")]
    public IActionResult GetTotal(int userId)
    {
        return Ok(_cartService.GetTotal(userId));
    }
}
