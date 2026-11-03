namespace diplom.Models;

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public string? CoverImageUrl { get; set; }

    // Связь с Категорией
    public int CategoryId { get; set; }
    public Category? Category { get; set; }

    // Связь с Автором
    public int AuthorId { get; set; }
    public Author? Author { get; set; }
}