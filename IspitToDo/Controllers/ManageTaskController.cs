using IspitToDo.Data;
using IspitToDo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IspitToDo.Controllers
{
    [Authorize]
    public class ManageTaskController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ManageTaskController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(int? todolistId)
        {
            var tasksQuery = _context.Tasks.AsQueryable();

            if (todolistId.HasValue)
            {
                tasksQuery = tasksQuery.Where(t => t.TodolistId == todolistId.Value);
                ViewBag.TodolistId = todolistId.Value;
            }

            var tasks = await tasksQuery.Include(t => t.Todolist).ToListAsync();

            return View(tasks);
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

        
            var task = await _context.Tasks
                                     .Include(t => t.Todolist) 
                                     .FirstOrDefaultAsync(m => m.Id == id);

            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

    
        public IActionResult Create(int? todolistId, bool fromTodolistDetails = false)
        {
            var task = new Tasks();
            if (todolistId.HasValue)
            {
                task.TodolistId = todolistId.Value;
            }

            ViewBag.TodolistId = new SelectList(_context.Todolists, "Id", "Title", task.TodolistId);
            ViewBag.FromTodolistDetails = fromTodolistDetails;
            return View(task);
        }

     
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TaskName,Status,TodolistId,IsVisible")] Tasks task)
        {
            if (ModelState.IsValid)
            {
                _context.Add(task);
                await _context.SaveChangesAsync();
      
                return RedirectToAction("Details", "ManageTodo", new { id = task.TodolistId });
            }

           
            ViewBag.TodolistId = new SelectList(_context.Todolists, "Id", "Title", task.TodolistId);
            return View(task);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            ViewBag.TodolistId = new SelectList(_context.Todolists, "Id", "Title");
            if (id == null)
            {
                return NotFound();
            }

            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }
            return View(task);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TaskName,Status,TodolistId,IsVisible")] Tasks task)
        {
            if (id != task.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(task);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaskExists(task.Id))
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
            return View(task);
        }
        public async Task<IActionResult> Delete(int id)
        {

            var task = await _context.Tasks.Include(t => t.Todolist).FirstOrDefaultAsync(c => c.Id == id);
            if (task == null)
            {
                return NotFound();

            }

            return View(task);
        }

        [HttpPost, ActionName(
            "Delete")]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            var task = await _context.Tasks.FindAsync(id);
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }
        private bool TaskExists(int id)
        {
            return _context.Tasks.Any(e => e.Id == id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateMultipleTasksStatus(int TodolistId, int[] taskIds, int[] taskStatus)
        {
            var checkedIds = new HashSet<int>(taskStatus ?? new int[] { });

            foreach (var taskId in taskIds)
            {
                var task = await _context.Tasks.FindAsync(taskId);
                if (task != null)
                {
                    task.Status = checkedIds.Contains(taskId);
                    _context.Update(task);
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "ManageTodo", new { id = TodolistId });
        }
    }
}