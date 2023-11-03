﻿using AutoMapper;
using contracts.Responses;
using Microsoft.AspNetCore.Mvc;
using webapi.Models;

namespace webapi.Utilities.ControllerUtils;

public interface IControllerHelper
{
    string UsernameClaim { get; }
    IActionResult NotFoundRespond<T>();

    Task<IActionResult> CreateAndRespond<TModel, TResponse>(
        Func<Task<TModel>> createFunc, IMapper mapper)
        where TModel : IModel
        where TResponse : IEntityResponse;

    Task<IActionResult> GetAllAndRespond<TModel, TResponse>(
        Func<Task<IEnumerable<TModel>>> getAllPagedFunc, IMapper mapper)
        where TModel : IModel
        where TResponse : IEntityResponse;

    Task<IActionResult> GetAllPagedAndRespond<TModel, TResponse>(
        Func<Task<(IEnumerable<TModel>, int)>> getAllPagedFunc, int page, int limit, IMapper mapper)
        where TModel : IModel
        where TResponse : IEntityResponse;

    Task<IActionResult> GetAndRespond<TModel, TResponse>(Func<Task<TModel>> getFunc, IMapper mapper)
            where TModel : IModel?
            where TResponse : IEntityResponse;

    Task<IActionResult> DeleteAndRespond<TModel>(Func<Task> deleteFunc) where TModel : IModel;
}