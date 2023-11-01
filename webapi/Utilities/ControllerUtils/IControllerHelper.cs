using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using webapi.Contracts.Responses;
using webapi.Models;

namespace webapi.Utilities.ControllerUtils;

public interface IControllerHelper
{
    string UsernameClaim { get; }
    IActionResult NotFoundRespond<T>();

    Task<IActionResult> CreateAndRespond<TEntity, TResponse>(
        Func<Task<TEntity>> createFunc, Func<TEntity, IMapper, TResponse> mapFunc, IMapper mapper);

    Task<IActionResult> GetAllAndRespond<TEntity, TResponse>(
        Func<Task<IEnumerable<TEntity>>> getAllFunc, IMapper mapper)
        where TEntity : IModel
        where TResponse : IEntityResponse;

    Task<IActionResult> GetAndRespond<TEntity, TResponse>(Func<Task<TEntity>> getFunc,
        Func<TEntity, IMapper, TResponse> mapFunc, IMapper mapper);

    Task<IActionResult> DeleteAndRespond<TEntity>(Func<Task> deleteFunc);
}