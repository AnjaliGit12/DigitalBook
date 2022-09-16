using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuthService.Models;

namespace AuthService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserTablesController : ControllerBase
    {
        private readonly DigitalBooksContext _context;

        public UserTablesController(DigitalBooksContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserTable>>> GetUsers()
        {
            if (_context.UserTables == null)
            {
                return NotFound();
            }
            return await _context.UserTables.ToListAsync();
        }
        // GET: api/Users/Roles
        [HttpGet]
        [Route("GetRoles")]
        public async Task<ActionResult<IEnumerable<RoleMaster>>> GetRoles()
        {
            if (_context.UserTables == null)
            {
                return NotFound();
            }
            return await _context.RoleMasters.ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserTable>> GetUser(int id)
        {
            if (_context.UserTables == null)
            {
                return NotFound();
            }
            var user = await _context.UserTables.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, UserTable user)
        {
            if (id != user.UserId)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<UserTable>> PostUser(UserTable user)
        {
            if (_context.UserTables == null)
            {
                return Problem("Entity set 'DigitalBooksContext.Users'  is null.");
            }
            _context.UserTables.Add(user);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (UserExists(user.UserId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetUser", new { id = user.UserId }, user);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            if (_context.UserTables == null)
            {
                return NotFound();
            }
            var user = await _context.UserTables.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.UserTables.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return (_context.UserTables?.Any(e => e.UserId == id)).GetValueOrDefault();
        }
        // GET: api/Users/GetAuthors/1
        [HttpGet]
        [Route("GetAuthors")]
        public async Task<ActionResult<IEnumerable<UserTable>>> GetAuthors(int roleId)
        {
            if (_context.UserTables == null)
            {
                return NotFound();
            }
            var user = _context.UserTables.Where(r => r.RoleId == roleId).ToListAsync();

            if (user == null)
            {
                return NotFound();
            }
            return await user;
        }
    }
}
