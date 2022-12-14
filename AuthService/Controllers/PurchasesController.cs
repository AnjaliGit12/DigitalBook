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
    public class PurchasesController : ControllerBase
    {
        private readonly DigitalBooksContext _context;

        public PurchasesController(DigitalBooksContext context)
        {
            _context = context;
        }

        // GET: api/Purchases
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Purchase>>> GetPurchases()
        {
            if (_context.Purchases == null)
            {
                return NotFound();
            }
            return await _context.Purchases.ToListAsync();
        }

        // GET: api/Purchases/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Purchase>> GetPurchase(int id)
        {
            if (_context.Purchases == null)
            {
                return NotFound();
            }
            var purchase = await _context.Purchases.FindAsync(id);

            if (purchase == null)
            {
                return NotFound();
            }

            return purchase;
        }

        // PUT: api/Purchases/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPurchase(int id, Purchase purchase)
        {
            if (id != purchase.PurchaseId)
            {
                return BadRequest();
            }

            _context.Entry(purchase).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PurchaseExists(id))
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

        // POST: api/Purchases
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Purchase>> PostPurchase(Purchase purchase)
        {
            if (_context.Purchases == null)
            {
                return Problem("Entity set 'DigitalBooksContext.Purchases'  is null.");
            }
            _context.Purchases.Add(purchase);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (PurchaseExists(purchase.PurchaseId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetPurchase", new { id = purchase.PurchaseId }, purchase);
        }

        // DELETE: api/Purchases/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePurchase(int id)
        {
            if (_context.Purchases == null)
            {
                return NotFound();
            }
            var purchase = await _context.Purchases.FindAsync(id);
            if (purchase == null)
            {
                return NotFound();
            }

            _context.Purchases.Remove(purchase);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PurchaseExists(int id)
        {
            return (_context.Purchases?.Any(e => e.PurchaseId == id)).GetValueOrDefault();
        }
        // Get purchased book history
        [HttpGet]
        [Route("GetPurchasedBookHistory")]
        public List<Book> GetPurchasedBookHistory(string EmailId)
        {
            List<Book> lsBookHistory = new List<Book>();
            if (_context.Purchases == null)
            {
                return lsBookHistory;
            }

            lsBookHistory = (from purchase in _context.Purchases
                             join UserTable in _context.UserTables on purchase.EmailId equals UserTable.EmailId
                             join book in _context.Books on purchase.BookId equals book.BookId
                             join aut in _context.UserTables on book.UserId  equals aut.UserId
                             join cat in _context.Categories on book.CategoryId equals cat.CategoryId   
                             where purchase.EmailId == EmailId && book.Active == true
                             select new
                             {
                                 purchaseId = purchase.PurchaseId,
                                 bookId = book.BookId,
                                 bookName = book.BookName,
                                 price = book.Price,
                                 author = book.author,
                                 publisher = book.Publisher,
                                 category = cat.CategoryName,
                                 username=UserTable.UserName,
                                 content = book.Content
                                 
                             }).ToList()
                     .Select(x => new Book()
                     {
                         BookId = x.bookId,
                         BookName = x.bookName,
                         Price=x.price,
                         Publisher =x.publisher,
                         author =x.author,
                         Content=x.content

                     }).ToList();

            return lsBookHistory;
        }
        // Get All Books With Status Purchase or not
        [HttpGet]
        [Route("GetBooksWithStatus")]
        public List<Book> GetBooksWithStatus(int EmailId)
        {
            List<Book> lsBookHistory = new List<Book>();
            if (_context.Purchases == null)
            {
                return lsBookHistory;
            }


            lsBookHistory = (from purchase in _context.Purchases
                             join book in _context.Books on purchase.BookId equals book.BookId
                             select new
                             {
                                 purchaseId = purchase.PurchaseId,
                                 bookId = book.BookId,
                                 bookName = book.BookName
                             }).ToList()
                     .Select(x => new Book()
                     {
                         BookId = x.bookId,
                         BookName = x.bookName
                     }).ToList();
            lsBookHistory = lsBookHistory.Where(x => x.UserId == EmailId).ToList();

            return lsBookHistory;
        }

    }
}
