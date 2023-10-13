using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Channels;
using webapi.Models;

namespace webapi.Utilities.ControllerUtils
{
    public class ControllerHelper : Controller
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

        public bool IsSuperuserClaim
        {
            get
            {
                var user = _httpContextAccessor!.HttpContext!.User;
                var isSuperuserClaim = user.Claims.FirstOrDefault(c => c.Type == "IsSuperuser")?.Value;
                return bool.Parse(isSuperuserClaim!);
            }
        }

        public IActionResult NotFoundRespond<T>()
        {
            return NotFound($"No {typeof(T).Name} found.");
        }

        public IActionResult CheckNullAndRespond<T>(T item)
        {
            return item is null ? NotFoundRespond<T>() : Ok(item);
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
