using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserService.Models;
using UserService.ViewModels;

namespace UserService.Controllers
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
            if (_context == null)
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
            var Book = await _context.Books.FindAsync(id);

            if (Book == null)
            {
                return NotFound();
            }

            return Book;
        }

        // PUT: api/Books/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBook(int id, Book Book)
        {
            if (id != Book.BookId)
            {
                return BadRequest();
            }
            Book.ModifiedDate = DateTime.Now;

            if (BookWithAuthorExists(id, (int)Book.UserId))
                _context.Entry(Book).State = EntityState.Modified;
            else
                return NotFound();

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
        public async Task<ActionResult<Book>> PostBook(Book Book)
        {
            if (_context.Books == null)
            {
                return Problem("Entity set 'DBDigitalBooksContext.Books'  is null.");
            }

            Book.CreatedDate = DateTime.Now;

            _context.Books.Add(Book);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBook", new { id = Book.BookId }, Book);
        }

        // DELETE: api/Books/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            if (_context.Books == null)
            {
                return NotFound();
            }
            var Book = await _context.Books.FindAsync(id);
            if (Book == null)
            {
                return NotFound();
            }

            _context.Books.Remove(Book);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BookExists(int id)
        {
            return (_context.Books?.Any(e => e.BookId == id)).GetValueOrDefault();
        }
        private bool BookWithAuthorExists(int id, int userID)
        {
            return (_context.Books?.Any(e => e.BookId == id && e.UserId == userID)).GetValueOrDefault();
        }
        [HttpGet]
        [Route("GetAuthorBook")]
        public  List<Book> GetAuthorBook(int userId)
        {
            List<Book> lsBookHistory = new List<Book>();
            if (_context.Books == null)
            {
                return lsBookHistory;
            }
            lsBookHistory = (from  book in _context.Books
                             join cat in _context.Categories on book.CategoryId equals cat.CategoryId
                             select new
                             {
                                 bookId = book.BookId,
                                 bookName = book.BookName,
                                 price = book.Price,
                                 category = cat.CategoryName,
                                 active= book.Active,
                                 publisher = book.Publisher,
                                 publisheddate= book.PublishedDate
                             }).ToList()
                     .Select(x => new Book()
                     {
                         BookId = x.bookId,
                         BookName = x.bookName,
                         Price = x.price,
                         CategoryName = x.category,
                         Active = x.active,
                         Publisher = x.publisher,
                         PublishedDate = x.publisheddate,
                         UserId = userId
                     }).ToList();
            lsBookHistory = lsBookHistory.Where(x => x.UserId == userId).ToList();


            return lsBookHistory;
        }

        //[HttpGet]
        //[Route("SearchBook")]
        //public List<Book> SearchBook(string title, int authorID, string publisher, DateTime publishedDate)
        //{
        //    List<Book> lsBook = new List<Book>();
        //    if (_context.Books == null)
        //    {
        //        return lsBook;
        //    }

        //    try
        //    {
        //        lsBook = (from b in _context.Books
        //                        join users in _context.UserTables on b.UserId equals users.UserId
        //                        where b.BookName == title && b.UserId == authorID
        //                        && b.Publisher == publisher && b.PublishedDate == publishedDate
        //                        && b.Active == true
        //                        select new
        //                        {
        //                            BookId = b.BookId,
        //                            BookName = b.BookName,
        //                            Author = users.FirstName + " " + users.LastName,
        //                            Publisher = b.Publisher,
        //                            Price = b.Price,
        //                            PublishedDate = b.PublishedDate

        //                        }).ToList()
        //                        .Select(x => new Book()
        //                        {
        //                            BookId = x.BookId,
        //                            BookName = x.BookName,
        //                            Author = x.Author,
        //                            Publisher = x.Publisher,
        //                            Price = (decimal?)Convert.ToDouble(x.Price),
        //                            PublishedDate = x.PublishedDate
        //                        }).ToList();
        //    }
        //    catch (Exception ex)
        //    {
        //        return lsBook;
        //    }

        //    return lsBook;
        //}

        [HttpGet]
        [Route("SearchBooks")]
        public List<Book> SearchBooks(int categoryID,int authorID, decimal price )
        {
            List<Book> lsBook = new List<Book>();
            if (_context.Books == null)
            {
                return lsBook;
            }

            try
            {
                lsBook = (from b in _context.Books
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
                                .Select(x => new Book()
                                {
                                    BookId = x.BookId,
                                    BookName = x.BookName,
                                    Author = x.Author,
                                    Publisher = x.Publisher,
                                    Price = (decimal?)Convert.ToDouble(x.Price),
                                    PublishedDate = x.PublishedDate,
                                    CategoryName = x.CategoryName,
                                    Active = x.Active
                                }).ToList();
            }
            catch (Exception ex)
            {
                return lsBook;
            }

            return lsBook;
        }
        [HttpPut("UpdateBookStatus/{BookId}/{UserID}/{Status}")]
        [Authorize]
        public async Task<IActionResult> UpdateBookStatus(int BookId, int UserID, bool Status)
        {
            if (BookId < 1)
            {
                return BadRequest();
            }

            if (BookWithAuthorExists(BookId, UserID))
            {
                var book = _context.Books.Find(BookId);
                book.Active = Status;
                book.ModifiedDate = DateTime.Now;
                _context.Entry(book).State = EntityState.Modified;
                //context.Entry(user).State = Entitystate.Modified;
            }               
            else
                return NotFound();

            try
            {
                _context.SaveChanges();
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(BookId))
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
    }
}
