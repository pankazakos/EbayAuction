using AutoMapper;
using contracts.Endpoints;
using contracts.Policies;
using contracts.Requests.Category;
using contracts.Responses.Category;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using webapi.Models;
using webapi.Services;
using webapi.Utilities.ControllerUtils;


namespace webapi.Controllers
{
    [ApiController]
    [Route(CategoryEndpoints.BaseUrl)]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IControllerHelper _controllerHelper;
        private readonly IMapper _mapper;

        public CategoryController(ICategoryService categoryService, IControllerHelper controllerHelper, IMapper mapper)
        {
            _categoryService = categoryService;
            _controllerHelper = controllerHelper;
            _mapper = mapper;
        }


        [HttpGet(CategoryEndpoints.GetById)]
        public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancel = default)
        {
            var category = await _categoryService.GetById(id, cancel);

            if (category is null)
            {
                return _controllerHelper.NotFoundRespond<Category>();
            }

            return Ok(category);
        }


        [HttpGet(CategoryEndpoints.GetAll)]
        public async Task<IActionResult> GetAll(CancellationToken cancel = default)
        {
            return await _controllerHelper.GetAllAndRespond<Category, BasicCategoryResponse>(() => _categoryService.GetAll(cancel), _mapper);
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
            return await _controllerHelper.DeleteAndRespond<Category>(() => _categoryService.Delete(id, cancel));
        }
    }
}
