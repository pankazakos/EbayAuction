using Microsoft.AspNetCore.Mvc;

namespace webapi.Utilities
{
    public class ControllerHelper : Controller
    {
        public string UsernameClaim => User.Claims.First(c => c.Type == "username").Value;

        public IActionResult NotFoundRespond<T>()
        {
            return NotFound($"No {typeof(T).Name} found.");
        }

        public IActionResult CheckNullAndRespond<T>(T item)
        {
            return item is null ? NotFoundRespond<T>() : Ok(item);
        }

        public async Task<IActionResult> CreateAndRespond<TEntity, TResponse>(Func<Task<TEntity>> createFunc, Func<TEntity, TResponse> mapFunc)
        {
            try
            {
                var newEntity = await createFunc();
                var entityName = typeof(TEntity).Name;
                var response = mapFunc(newEntity);

                return Created(entityName, response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
