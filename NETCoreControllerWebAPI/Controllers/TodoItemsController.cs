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
            => await _context.TodoItems.OrderBy(tdi => tdi.Priority ).ToListAsync();

        // GET: /TodoItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItem>> GetTodoItem(long id)
        {
            var todoItem = GetTodoItemByID(id);

            if (todoItem == null)
            {
                return NotFound();
            }

            return await todoItem;
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
            TodoItem todoItem = await GetTodoItemByID(id);

            if (todoItemPatch.Priority.HasValue) IncrementItemPriorities(todoItemPatch.Priority.Value, todoItem.Priority);

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
//TODO: Adjust item priorities after delete
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

        private int GetNextPriorityValue()
        {
            if (!_context.TodoItems.Any()) return 1;

            return _context.TodoItems.Max(tdi => tdi.Priority) + 1;
        }

        private void UpdateTodoItem (ref TodoItem toDoItem, TodoItemPatchDTO patchData)
        {
            toDoItem.Name = patchData.Name ?? toDoItem.Name;
            toDoItem.Priority = patchData.Priority ?? toDoItem.Priority;
            toDoItem.IsComplete = patchData.IsComplete ?? toDoItem.IsComplete;
        }
//TODO: update increment to only increment needed items
        private void IncrementItemPriorities(int priority)
        {
            if (isPriorityConflict(priority))
            {
                foreach (var item in _context.TodoItems.Where(tdi => tdi.Priority >= priority))
                {
                    item.Priority++;
                }
            }
        }
        private void IncrementItemPriorities(int newPriority, int oldPriority)
        {
            if (newPriority == oldPriority) return;

            bool priorityIncrease = newPriority < oldPriority;
            if (isPriorityConflict(newPriority))
            {
                if (priorityIncrease)
                {
                    foreach (var item in _context.TodoItems.Where(tdi => tdi.Priority >= newPriority && tdi.Priority < oldPriority))
                    {
                        item.Priority++;
                    }
                }
                else
                {
                    foreach (var item in _context.TodoItems.Where(tdi => tdi.Priority <= newPriority && tdi.Priority > oldPriority))
                    {
                        item.Priority--;
                    }
                }
            }
        }
        private bool TodoItemExists(long id)
            => _context.TodoItems.Any(e => e.Id == id);
        private async Task<TodoItem> GetTodoItemByID(long id)
            => await _context.TodoItems.FindAsync(id);
        private bool isPriorityConflict(int priority) 
            => _context.TodoItems.Where(tdi => tdi.Priority == priority).Any();
    }
}
