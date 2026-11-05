namespace diplom.DTOs;

// DTOs Корзины
public class CartItemDto
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public string BookTitle { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice => Price * Quantity;
}

public class AddToCartDto
{
    public int BookId { get; set; }
    public int Quantity { get; set; } = 1;
}

public class UpdateCartQuantityDto
{
    public int Quantity { get; set; }
}

// DTOs Заказов
public class CreateOrderDto
{
    public string ShippingAddress { get; set; } = string.Empty;
}

public class OrderItemDto
{
    public int BookId { get; set; }
    public string BookTitle { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice => UnitPrice * Quantity;
}

public class OrderDto
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string ShippingAddress { get; set; } = string.Empty;
    public List<OrderItemDto> Items { get; set; } = new();
}