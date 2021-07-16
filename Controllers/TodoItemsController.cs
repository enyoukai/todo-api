using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;
using Microsoft.Azure.Cosmos.Table;


namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private readonly TodoContext _context;
        private CloudTable table = Common.CreateTable("table");

        public TodoItemsController(TodoContext context)
        {
            _context = context;
            // https://stackoverflow.com/questions/8145479/can-constructors-be-async
        }

        // GET: api/TodoItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodoItems()
        {
            // https://stackoverflow.com/questions/38748426/get-all-records-from-azure-table-storage
            TableQuery<TodoEntity> query = new TableQuery<TodoEntity>();
            List<TodoItem> items = new List<TodoItem>();

            // foreach (TodoEntity entity in table.ExecuteQuery(query)) { // how do i make this asynchronous
            //     items.Add(new TodoItem() {Id = entity.Uuid, Content = entity.Content});
            //     Console.WriteLine("here");
            // }

            TableContinuationToken continuationToken = null;
            do
            {
                var page = await table.ExecuteQuerySegmentedAsync(query, continuationToken);
                continuationToken = page.ContinuationToken;
                foreach (TodoEntity entity in page.Results) {
                    items.Add(new TodoItem() {Id = entity.Uuid, Content = entity.Content});
                }

            }
            while (continuationToken != null);

            return items;

            // return await _context.TodoItems.ToListAsync();
        }

        // GET: api/TodoItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItem>> GetTodoItem(string id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);

            if (todoItem == null)
            {
                return NotFound();
            }

            return todoItem;
        }

        // POST: api/TodoItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TodoItem>> PostTodoItem(TodoItem todoItem)
        {
            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();

            TodoEntity entity = new TodoEntity(todoItem.Id, todoItem.Content) {
                Uuid = todoItem.Id,
                Content = todoItem.Content
            };
            await Table.InsertOrMergeEntityAsync(table, entity);

            return CreatedAtAction("GetTodoItem", new { id = todoItem.Id }, todoItem);
        }

        // DELETE: api/TodoItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(string id)
        {
            TodoEntity entity = await Table.RetrieveEntityUsingPointQueryAsync(table, id, id);
            if (entity == null) return NotFound();

            await Table.DeleteEntityAsync(table, entity);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // private bool TodoItemExists(long id)
        // {
        //     return _context.TodoItems.Any(e => e.Id == id);
        // }
    }
}
