using System.Security.Claims;
using diplom.Data;
using diplom.DTOs;
using diplom.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace diplom.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CartController : ControllerBase
{
    private readonly AppDbContext _context;

    public CartController(AppDbContext context)
    {
        _context = context;
    }

    private Guid GetUserId()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.Parse(userIdStr!);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CartItemDto>>> GetCart()
    {
        var userId = GetUserId();

        return await _context.CartItems
            .Where(c => c.UserId == userId)
            .Include(c => c.Book)
            .Select(c => new CartItemDto
            {
                Id = c.Id,
                BookId = c.BookId,
                BookTitle = c.Book != null ? c.Book.Title : string.Empty,
                Price = c.Book != null ? c.Book.Price : 0,
                Quantity = c.Quantity
            })
            .ToListAsync();
    }

    [HttpPost]
    public async Task<IActionResult> AddToCart([FromBody] AddToCartDto dto)
    {
        var userId = GetUserId();

        var book = await _context.Books.FindAsync(dto.BookId);
        if (book == null)
            return NotFound("Книга не найдена");

        var cartItem = await _context.CartItems
            .FirstOrDefaultAsync(c => c.UserId == userId && c.BookId == dto.BookId);

        if (cartItem != null)
        {
            cartItem.Quantity += dto.Quantity;
        }
        else
        {
            cartItem = new CartItem
            {
                UserId = userId,
                BookId = dto.BookId,
                Quantity = dto.Quantity
            };
            _context.CartItems.Add(cartItem);
        }

        await _context.SaveChangesAsync();
        return Ok(new { message = "Товар добавлен в корзину" });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateQuantity(int id, [FromBody] UpdateCartQuantityDto dto)
    {
        var userId = GetUserId();

        var cartItem = await _context.CartItems
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

        if (cartItem == null)
            return NotFound("Элемент корзины не найден");

        if (dto.Quantity <= 0)
        {
            _context.CartItems.Remove(cartItem);
        }
        else
        {
            cartItem.Quantity = dto.Quantity;
        }

        await _context.SaveChangesAsync();
        return Ok(new { message = "Количество обновлено" });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoveFromCart(int id)
    {
        var userId = GetUserId();

        var cartItem = await _context.CartItems
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

        if (cartItem == null)
            return NotFound("Элемент корзины не найден");

        _context.CartItems.Remove(cartItem);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Товар удален из корзины" });
    }
}