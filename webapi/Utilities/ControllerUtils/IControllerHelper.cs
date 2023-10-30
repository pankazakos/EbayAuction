using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace webapi.Utilities.ControllerUtils;

public interface IControllerHelper
{
    string UsernameClaim { get; }
    IActionResult NotFoundRespond<T>();

    Task<IActionResult> CreateAndRespond<TEntity, TResponse>(
        Func<Task<TEntity>> createFunc, Func<TEntity, IMapper, TResponse> mapFunc, IMapper mapper);

    Task<IActionResult> GetAndRespond<TEntity, TResponse>(Func<Task<TEntity>> getFunc,
        Func<TEntity, IMapper, TResponse> mapFunc, IMapper mapper);

    Task<IActionResult> DeleteAndRespond<TEntity>(Func<Task> deleteFunc);
}