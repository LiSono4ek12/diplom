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
public class OrdersController : ControllerBase
{
    private readonly AppDbContext _context;

    public OrdersController(AppDbContext context)
    {
        _context = context;
    }

    private Guid GetUserId()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.Parse(userIdStr!);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetUserOrders()
    {
        var userId = GetUserId();

        return await _context.Orders
            .Where(o => o.UserId == userId)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Book)
            .OrderByDescending(o => o.OrderDate)
            .Select(o => new OrderDto
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount,
                Status = o.Status.ToString(),
                ShippingAddress = o.ShippingAddress,
                Items = o.OrderItems.Select(oi => new OrderItemDto
                {
                    BookId = oi.BookId,
                    BookTitle = oi.Book != null ? oi.Book.Title : string.Empty,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice
                }).ToList()
            })
            .ToListAsync();
    }

    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout([FromBody] CreateOrderDto dto)
    {
        var userId = GetUserId();

        var cartItems = await _context.CartItems
            .Where(c => c.UserId == userId)
            .Include(c => c.Book)
            .ToListAsync();

        if (!cartItems.Any())
            return BadRequest("Ваша корзина пуста");

        var order = new Order
        {
            UserId = userId,
            OrderDate = DateTime.UtcNow,
            ShippingAddress = dto.ShippingAddress,
            Status = OrderStatus.Pending,
            TotalAmount = cartItems.Sum(c => (c.Book?.Price ?? 0) * c.Quantity)
        };

        foreach (var item in cartItems)
        {
            order.OrderItems.Add(new OrderItem
            {
                BookId = item.BookId,
                Quantity = item.Quantity,
                UnitPrice = item.Book?.Price ?? 0
            });
        }

        _context.Orders.Add(order);
        _context.CartItems.RemoveRange(cartItems);

        await _context.SaveChangesAsync();

        return Ok(new { message = "Заказ успешно оформлен", orderId = order.Id });
    }
}