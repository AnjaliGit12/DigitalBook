using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuthService.Models;
using UserService.ViewModels;

namespace AuthService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly DigitalBooksContext _context;

        public BooksController(DigitalBooksContext context)
        {
            _context = context;
        }

        // GET: api/Books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
        {
            if (_context.Books == null)
            {
                return NotFound();
            }
            return await _context.Books.ToListAsync();
        }

        // GET: api/Books/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBook(int id)
        {
            if (_context.Books == null)
            {
                return NotFound();
            }
            var book = await _context.Books.FindAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            return book;
        }

        // PUT: api/Books/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBook(int id, Book book)
        {
            if (id != book.BookId)
            {
                return BadRequest();
            }

            _context.Entry(book).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Books
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Book>> PostBook(Book book)
        {
            if (_context.Books == null)
            {
                return Problem("Entity set 'DigitalBooksContext.Books'  is null.");
            }
            _context.Books.Add(book);
            try
            {
                book.CreatedDate = DateTime.Now;
                _context.Books.Add(book);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (BookExists(book.BookId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetBook", new { id = book.BookId }, book);
        }

        // DELETE: api/Books/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            if (_context.Books == null)
            {
                return NotFound();
            }
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BookExists(int id)
        {
            return (_context.Books?.Any(e => e.BookId == id)).GetValueOrDefault();
        }

        [HttpGet]
        [Route("SearchBooks")]
        public List<BookMasterViewModel> SearchBooks(int categoryID, int authorID, decimal price)
        {
            List<BookMasterViewModel> lsBookMaster = new List<BookMasterViewModel>();
            if (_context.Books == null)
            {
                return lsBookMaster;
            }

            try
            {
                lsBookMaster = (from b in _context.Books
                                join users in _context.UserTables on b.UserId equals users.UserId
                                join category in _context.Categories on b.CategoryId equals category.CategoryId
                                where b.CategoryId == categoryID || b.UserId == authorID
                                || b.Price == price
                                && b.Active == true
                                select new
                                {
                                    BookId = b.BookId,
                                    BookName = b.BookName,
                                    Author = users.FirstName + " " + users.LastName,
                                    Publisher = b.Publisher,
                                    Price = b.Price,
                                    PublishedDate = b.PublishedDate,
                                    CategoryName = category.CategoryName,
                                    Active = b.Active

                                }).ToList()
                                .Select(x => new BookMasterViewModel()
                                {
                                    BookId = x.BookId,
                                    Title = x.BookName,
                                    Author = x.Author,
                                    Publisher = x.Publisher,
                                    Price = Convert.ToDouble(x.Price),
                                    PublishedDate = (DateTime)x.PublishedDate,
                                    CategoryName = x.CategoryName,
                                    Active = (bool)x.Active
                                }).ToList();
            }
            catch (Exception ex)
            {
                return lsBookMaster;
            }

            return lsBookMaster;
        }
    }
}
