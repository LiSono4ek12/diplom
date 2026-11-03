using diplom.Data;
using diplom.DTOs;
using diplom.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace diplom.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly AppDbContext _context;

    public BooksController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookDto>>> GetBooks([FromQuery] int? categoryId, [FromQuery] int? authorId)
    {
        var query = _context.Books
            .Include(b => b.Category)
            .Include(b => b.Author)
            .AsQueryable();

        if (categoryId.HasValue)
            query = query.Where(b => b.CategoryId == categoryId.Value);

        if (authorId.HasValue)
            query = query.Where(b => b.AuthorId == authorId.Value);

        return await query
            .Select(b => new BookDto
            {
                Id = b.Id,
                Title = b.Title,
                Description = b.Description,
                Price = b.Price,
                StockQuantity = b.StockQuantity,
                CoverImageUrl = b.CoverImageUrl,
                CategoryId = b.CategoryId,
                CategoryName = b.Category != null ? b.Category.Name : string.Empty,
                AuthorId = b.AuthorId,
                AuthorFullName = b.Author != null ? $"{b.Author.FirstName} {b.Author.LastName}" : string.Empty
            })
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BookDto>> GetBook(int id)
    {
        var book = await _context.Books
            .Include(b => b.Category)
            .Include(b => b.Author)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (book == null)
            return NotFound("Книга не найдена");

        return new BookDto
        {
            Id = book.Id,
            Title = book.Title,
            Description = book.Description,
            Price = book.Price,
            StockQuantity = book.StockQuantity,
            CoverImageUrl = book.CoverImageUrl,
            CategoryId = book.CategoryId,
            CategoryName = book.Category?.Name ?? string.Empty,
            AuthorId = book.AuthorId,
            AuthorFullName = book.Author != null ? $"{book.Author.FirstName} {book.Author.LastName}" : string.Empty
        };
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<BookDto>> CreateBook([FromBody] CreateBookDto dto)
    {
        var categoryExists = await _context.Categories.AnyAsync(c => c.Id == dto.CategoryId);
        if (!categoryExists)
            return BadRequest("Указанная категория не существует");

        var authorExists = await _context.Authors.AnyAsync(a => a.Id == dto.AuthorId);
        if (!authorExists)
            return BadRequest("Указанный автор не существует");

        var book = new Book
        {
            Title = dto.Title,
            Description = dto.Description,
            Price = dto.Price,
            StockQuantity = dto.StockQuantity,
            CoverImageUrl = dto.CoverImageUrl,
            CategoryId = dto.CategoryId,
            AuthorId = dto.AuthorId
        };

        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetBook), new { id = book.Id }, dto);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateBook(int id, [FromBody] CreateBookDto dto)
    {
        var book = await _context.Books.FindAsync(id);
        if (book == null)
            return NotFound("Книга не найдена");

        book.Title = dto.Title;
        book.Description = dto.Description;
        book.Price = dto.Price;
        book.StockQuantity = dto.StockQuantity;
        book.CoverImageUrl = dto.CoverImageUrl;
        book.CategoryId = dto.CategoryId;
        book.AuthorId = dto.AuthorId;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteBook(int id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book == null)
            return NotFound("Книга не найдена");

        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}