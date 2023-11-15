using AutoMapper;
using contracts.Endpoints;
using contracts.Policies;
using contracts.Requests.Category;
using contracts.Responses.Category;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using webapi.Models;
using webapi.Services.Interfaces;
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
            return await _controllerHelper.GetAndRespond<Category?, BasicCategoryResponse>(
                () => _categoryService.GetById(id, cancel), _mapper);
        }


        [HttpGet(CategoryEndpoints.GetAll)]
        public async Task<IActionResult> GetAll(CancellationToken cancel = default)
        {
            return await _controllerHelper.GetAllAndRespond<Category, BasicCategoryResponse>(() => _categoryService.GetAll(cancel), _mapper);
        }


        [Authorize(Policy = Policies.Admin)]
        [HttpPost(CategoryEndpoints.Create)]
        public async Task<IActionResult> Create([FromBody] AddCategoryRequest request, CancellationToken cancel = default)
        {
            return await _controllerHelper.CreateAndRespond<Category, BasicCategoryResponse>(
                () => _categoryService.Create(request, cancel), _mapper);
        }


        [Authorize(Policy = Policies.Admin)]
        [HttpDelete(CategoryEndpoints.Delete)]
        public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancel = default)
        {
            return await _controllerHelper.DeleteAndRespond<Category>(() => _categoryService.Delete(id, cancel));
        }
    }
}
