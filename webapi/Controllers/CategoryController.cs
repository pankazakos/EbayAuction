using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using webapi.Contracts.Endpoints;
using webapi.Contracts.Policies;
using webapi.Contracts.Requests;
using webapi.Models;
using webapi.Services;
using webapi.Utilities;

namespace webapi.Controllers
{
    [ApiController]
    [Route(CategoryEndpoints.BaseUrl)]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly ControllerHelper _controllerHelper;

        public CategoryController(ICategoryService categoryService, ControllerHelper controllerHelper)
        {
            _categoryService = categoryService;
            _controllerHelper = controllerHelper;
        }

        [HttpGet(CategoryEndpoints.GetById)]
        public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancel = default)
        {
            var category = await _categoryService.GetById(id, cancel);

            return _controllerHelper.CheckNullAndRespond(category);
        }

        [HttpGet(CategoryEndpoints.GetAll)]
        public async Task<IActionResult> GetAll(CancellationToken cancel = default)
        {
            var categories = await _categoryService.GetAll(cancel);

            return Ok(categories);
        }

        [Authorize(Policy = Policies.Admin)]
        [HttpPost(CategoryEndpoints.Create)]
        public async Task<IActionResult> Create([FromBody] AddCategoryRequest body, CancellationToken cancel = default)
        {
            try
            {
                var category = await _categoryService.Create(body, cancel);

                return Created(nameof(category), category);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Policy = Policies.Admin)]
        [HttpDelete(CategoryEndpoints.Delete)]
        public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancel = default)
        {
            try
            {
                await _categoryService.Delete(id, cancel);

                return NoContent();
            }
            catch (Exception)
            {
                return _controllerHelper.NotFoundRespond<Category>();
            }
        }
    }
}
