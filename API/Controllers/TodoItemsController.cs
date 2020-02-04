using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoAPI.API.Helpers.Filter.Models;
using ToDoAPI.API.Helpers.Filter.Extensions;
using ToDoAPI.Domain.Models;
using ToDoAPI.Data.Repository;
using Microsoft.AspNetCore.Http;
using System.Linq;
using AutoMapper;
using API.Dto;
using System.Collections.Generic;
using System.Security.Claims;

namespace ToDoAPI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TodoItemsController : ControllerBase
    {
        private readonly TodoContext _context;
        private readonly IMapper _mapper;

        public TodoItemsController(TodoContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/TodoItems
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<TodoItemDto>> GetTodoItems([FromQuery] TodoItemFilter filter)
        {
            // var count = _context.TodoItems.CountAsync();
            // var user = User.Claims.Where(c => c.Type == ClaimTypes.Name).FirstOrDefault();
            var todoItems = await _context.TodoItems.Apply(filter).ToListAsync();
            var todoItemDto = _mapper.Map<List<TodoItemDto>>(todoItems);
            return Ok(todoItemDto);
        }

        // GET: api/TodoItems/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TodoItemDto>> GetTodoItem(long id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);
            if (todoItem is null) return NotFound();
            var result = _mapper.Map<TodoItemDto>(todoItem);
            return Ok(result);
        }

        // PUT: api/TodoItems/5
        [HttpPut("{id}")]
        [ProducesResponseTypeAttribute(StatusCodes.Status204NoContent)]
        [ProducesResponseTypeAttribute(StatusCodes.Status400BadRequest)]
        [ProducesResponseTypeAttribute(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TodoItemDto>> PutTodoItem(long id, TodoItemDto todoItemDto)
        {
            if (id != todoItemDto.Id || todoItemDto is null) return BadRequest();

            TodoItem todoItem = _mapper.Map<TodoItem>(todoItemDto);
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

        // POST: api/TodoItems
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TodoItemDto>> PostTodoItem(TodoItemDto todoItemDto)
        {
            if (!ModelState.IsValid) return BadRequest();
            TodoItem todoItem = _mapper.Map<TodoItem>(todoItemDto);
            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();
            todoItemDto = _mapper.Map<TodoItemDto>(todoItem);
            return CreatedAtAction("GetTodoItem", new { id = todoItem.Id }, todoItemDto);
        }

        // DELETE: api/TodoItems/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TodoItemDto>> DeleteTodoItem(long id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);
            if (todoItem is null) return NotFound();

            _context.TodoItems.Remove(todoItem);
            await _context.SaveChangesAsync();

            return Ok(todoItem);
        }

        private bool TodoItemExists(long id)
        {
            return _context.TodoItems.Any(e => e.Id == id);
        }
    }
}
