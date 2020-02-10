using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoAPI.API.Helpers.Filter.Models;
using ToDoAPI.API.Helpers.Filter.Extensions;
using ToDoAPI.Domain.Models;
using Microsoft.AspNetCore.Http;
using AutoMapper;
using API.Dto;
using System.Collections.Generic;
using ToDoAPI.Repository.Data;

namespace ToDoAPI.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class TodoItemsController : ControllerBase
    {
        private readonly TodoItemRepository _repo;
        private readonly IMapper _mapper;

        public TodoItemsController(TodoItemRepository repository, IMapper mapper)
        {
            _repo = repository;
            _mapper = mapper;
        }

        // GET: api/TodoItems
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<TodoItemDto>> GetTodoItems([FromQuery] TodoItemFilter filter)
        {
            // var count = await _repo.All.CountAsync();
            // var user = User.Claims.Where(c => c.Type == ClaimTypes.Name).FirstOrDefault();
            var todoItems = await _repo.All.Apply(filter).ToListAsync();
            var todoItemsDto = _mapper.Map<List<TodoItemDto>>(todoItems);
            return Ok(todoItemsDto);
        }

        // GET: api/TodoItems/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TodoItemDto>> GetTodoItem(long id)
        {
            var todoItem = await _repo.FindAsync(id);
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
            if (id != todoItemDto.Id || !ModelState.IsValid) return BadRequest();

            var todoItem = _mapper.Map<TodoItem>(todoItemDto);
            _repo.Update(todoItem);
            try
            {
                await _repo.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!_repo.TodoItemExists(id)) return NotFound();
                return this.StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                throw;
            }
            return NoContent();
        }

        // POST: api/TodoItems
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TodoItemDto>> PostTodoItem(TodoItemDto todoItemDto, ApiVersion apiVersion)
        {
            if (!ModelState.IsValid) return BadRequest();
            var todoItem = _mapper.Map<TodoItem>(todoItemDto);
            _repo.Add(todoItem);
            await _repo.SaveChangesAsync();
            return CreatedAtAction(nameof(GetTodoItem), new { id = todoItem.Id, version = apiVersion.ToString() }, todoItem);
        }

        // DELETE: api/TodoItems/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TodoItemDto>> DeleteTodoItem(long id)
        {
            var todoItem = await _repo.FindAsync(id);
            if (todoItem is null) return NotFound();
            _repo.Remove(todoItem);
            await _repo.SaveChangesAsync();
            var todoItemDto = _mapper.Map<TodoItemDto>(todoItem);
            return Ok(todoItemDto);
        }
    }
}
