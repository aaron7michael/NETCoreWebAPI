using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NETCoreControllerWebAPI.Models;

namespace NETCoreControllerWebAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private readonly TodoContext _context;

        public TodoItemsController(TodoContext context)
        {
            _context = context;
        }

        // GET: /TodoItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodoItems()
        {
            return await _context.TodoItems.OrderBy(tdi => tdi.Priority ).ToListAsync();
        }

        // GET: /TodoItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItem>> GetTodoItem(long id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);

            if (todoItem == null)
            {
                return NotFound();
            }

            return todoItem;
        }

        // PATCH: /TodoItems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchTodoItem(long id, TodoItemPatchDTO todoItemPatch)
        {
            if (!TodoItemExists(id))
            {
                return NotFound();
            }

            if (todoItemPatch.Priority.HasValue) IncrementItemPriorities(todoItemPatch.Priority.Value);

            TodoItem todoItem = await _context.TodoItems.FindAsync(id);
            
            UpdateTodoItem(ref todoItem, todoItemPatch);
            
            _context.Entry(todoItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TodoItemExists(id))
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

        // POST: /TodoItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TodoItem>> PostTodoItem(TodoItemDTO todoItemDTO)
        {
            if (todoItemDTO.Priority.HasValue) IncrementItemPriorities(todoItemDTO.Priority.Value);
            TodoItem todoItem = new()
            {
                Name = todoItemDTO.Name,
                Priority = todoItemDTO.Priority ?? GetNextPriorityValue(),
                IsComplete = todoItemDTO.IsComplete,
            };
            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTodoItem), new { id = todoItem.Id }, todoItem);
        }

        // DELETE: /TodoItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(long id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);
            if (todoItem == null)
            {
                return NotFound();
            }

            _context.TodoItems.Remove(todoItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TodoItemExists(long id)
        {
            return _context.TodoItems.Any(e => e.Id == id);
        }
        private int GetNextPriorityValue()
        {
            if (!_context.TodoItems.Any()) return 1;

            return _context.TodoItems.OrderByDescending(tdi => tdi.Priority).First().Priority + 1;
        }

        private void UpdateTodoItem (ref TodoItem toDoItem, TodoItemPatchDTO patchData)
        {
            toDoItem.Name = patchData.Name ?? toDoItem.Name;
            toDoItem.Priority = patchData.Priority ?? toDoItem.Priority;
            toDoItem.IsComplete = patchData.IsComplete ?? toDoItem.IsComplete;
        }
//TODO: update increment to only increment needed items
        private void IncrementItemPriorities(int newPriority, int oldPriority)
        {
            bool priorityConflict = _context.TodoItems.Where(tdi => tdi.Priority == newPriority).Any();
            if (priorityConflict)
            {
                foreach (var item in _context.TodoItems.Where(tdi => tdi.Priority <= newPriority && tdi.Priority < oldPriority))
                {
                    item.Priority++;
                }
            }
        }
    }
}
