using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using webapi.Contracts.Mapping;
using webapi.Contracts.Responses;
using webapi.Models;

namespace webapi.Utilities.ControllerUtils
{
    public class ControllerHelper : Controller, IControllerHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ControllerHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string UsernameClaim
        {
            get
            {
                var user = _httpContextAccessor.HttpContext!.User;
                var usernameClaim = user.Claims.FirstOrDefault(c => c.Type == "username")!.Value;
                return usernameClaim!;
            }
        }

        public IActionResult NotFoundRespond<T>()
        {
            return NotFound($"No {typeof(T).Name} found.");
        }

        public async Task<IActionResult> CreateAndRespond<TEntity, TResponse>(
            Func<Task<TEntity>> createFunc, Func<TEntity, IMapper, TResponse> mapFunc, IMapper mapper)
        {
            try
            {
                var entity = await createFunc();
                var response = mapFunc(entity, mapper);
                return Created(nameof(TEntity), response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while saving the data to the database. Please try again later.");
            }
        }

        public async Task<IActionResult> GetAllAndRespond<TEntity, TResponse>(
            Func<Task<IEnumerable<TEntity>>> getAllFunc, IMapper mapper) 
            where TEntity : IModel
            where TResponse : IEntityResponse
        {
            var entities = await getAllFunc();

            var mappedEntities = entities.Select(entity => entity.MapToResponse<TResponse>(mapper));

            var castEntities = mappedEntities.Cast<TResponse>();

            return Ok(castEntities);
        }

        public async Task<IActionResult> GetAndRespond<TEntity, TResponse>(Func<Task<TEntity>> getFunc,
            Func<TEntity, IMapper, TResponse> mapFunc, IMapper mapper)
        {
            var entity = await getFunc();

            if (entity is null)
            {
                return NotFoundRespond<TEntity>();
            }
            
            var response = mapFunc(entity, mapper);
            return Ok(response);
        }

        public async Task<IActionResult> DeleteAndRespond<TEntity>(Func<Task> deleteFunc)
        {
            try
            {
                await deleteFunc();
                return NoContent();
            }
            catch (InvalidOperationException)
            {
                return NotFoundRespond<TEntity>();
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while saving the data to the database. Please try again later.");
            }
        }
    }
}
