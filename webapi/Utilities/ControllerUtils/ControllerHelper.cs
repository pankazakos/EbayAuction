﻿using AutoMapper;
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

        public async Task<IActionResult> CreateAndRespond<TModel, TResponse>(
            Func<Task<TModel>> createFunc, IMapper mapper)
            where TModel : IModel
            where TResponse : IEntityResponse
        {
            try
            {
                var entity = await createFunc();
                return Created(nameof(TModel), entity.MapToResponse<TResponse>(mapper));
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

        public async Task<IActionResult> GetAllAndRespond<TModel, TResponse>(
            Func<Task<IEnumerable<TModel>>> getAllFunc, IMapper mapper)
            where TModel : IModel
            where TResponse : IEntityResponse
        {
            var entities = await getAllFunc();

            var mappedEntities = entities.Select(entity => entity.MapToResponse<TResponse>(mapper));

            var castEntities = mappedEntities.Cast<TResponse>();

            return Ok(castEntities);
        }

        public async Task<IActionResult> GetAndRespond<TModel, TResponse>(
            Func<Task<TModel>> getFunc, IMapper mapper)
            where TModel : IModel?
            where TResponse : IEntityResponse
        {
            var entity = await getFunc();

            return entity is null ? NotFoundRespond<TModel>() : Ok(entity.MapToResponse<TResponse>(mapper));
        }

        public async Task<IActionResult> DeleteAndRespond<TModel>(Func<Task> deleteFunc) where TModel : IModel
        {
            try
            {
                await deleteFunc();
                return NoContent();
            }
            catch (InvalidOperationException)
            {
                return NotFoundRespond<TModel>();
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while saving the data to the database. Please try again later.");
            }
        }
    }
}
