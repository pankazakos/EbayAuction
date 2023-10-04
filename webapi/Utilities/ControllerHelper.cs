using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace webapi.Utilities
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
                var user = _httpContextAccessor!.HttpContext!.User;
                var usernameClaim = user.Claims.FirstOrDefault(c => c.Type == "username")?.Value;
                return usernameClaim!;
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
    }
}
