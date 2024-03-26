using IspitToDo.Data;
using IspitToDo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace IspitToDo.Controllers
{
    [Authorize]
    public class ManageTodoController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ManageTodoController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

    
        public async Task<IActionResult> Index()
        {
            var todo= _context.Todolists.Include(t => t.User);
            return View(await todo.ToListAsync());
        }
        public IActionResult Create()
        {
            return View();
        }

      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,UserId")] Todolist todolist)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            todolist.UserId = userId;
            if (ModelState.IsValid)
            {
                _context.Add(todolist);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(todolist);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var todolist = await _context.Todolists.FindAsync(id);
            if (todolist == null)
            {
                return NotFound();
            }
            return View(todolist);
        }

   
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,UserId")] Todolist todolist)
        {
            if (id != todolist.Id) 
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            todolist.UserId = userId; 

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(todolist);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TodolistExists(todolist.Id)) 
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(todolist);
        }


        public async Task<IActionResult> Delete(int id)
        {

            var todo = await _context.Todolists.Include(d =>d.User).FirstOrDefaultAsync(c => c.Id == id);
            if (todo == null)
            {
                return NotFound();

            }

            return View(todo);
        }

        [HttpPost, ActionName(
            "Delete")]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            var todo = await _context.Todolists.FindAsync(id);
            _context.Todolists.Remove(todo);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var todolist = await _context.Todolists
                .Include(t => t.User)
                .Include(t => t.Tasks) 
                .FirstOrDefaultAsync(m => m.Id == id);

            if (todolist == null)
            {
                return NotFound();
            }

            return View(todolist);
        }

        private bool TodolistExists(int id)
        {
            return _context.Todolists.Any(e => e.Id == id);
        }
    }
}
  