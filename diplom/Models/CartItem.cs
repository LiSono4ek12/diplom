namespace diplom.Models;

public class CartItem
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public ApplicationUser? User { get; set; }

    public int BookId { get; set; }
    public Book? Book { get; set; }

    public int Quantity { get; set; }
}