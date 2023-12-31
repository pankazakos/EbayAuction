﻿using contracts.Requests.Category;
using webapi.Database;
using webapi.Models;
using webapi.Repository.Interfaces;
using webapi.Services.Interfaces;

namespace webapi.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(ICategoryRepository categoryRepository, ILogger<CategoryService> logger)
        {
            _categoryRepository = categoryRepository;
            _logger = logger;
        }

        public async Task<Category?> GetById(int id, CancellationToken cancel = default)
        {
            var category = await _categoryRepository.GetById(id, cancel);

            if (category is null)
            {
                _logger.LogWarning("Category {categoryId} not found", id);
                return null;
            }

            _logger.LogInformation("Retrieved category {categoryName}", category.Name);

            return category;
        }

        public async Task<IEnumerable<Category>> GetAll(CancellationToken cancel = default)
        {
            var categories = (await _categoryRepository.GetAll(cancel));

            _logger.LogInformation("Retrieved all categories");
            return categories;
        }

        public async Task<IEnumerable<Category>> FilterWithIds(List<int> ids, CancellationToken cancel = default)
        {
            var categories = (await _categoryRepository.FilterWithIds(ids, cancel)).ToList();

            if (categories.Any())
            {
                _logger.LogInformation("Categories {categoryIds} retrieved", string.Join(", ", ids));
                return categories;
            }

            _logger.LogWarning("No Categories with ids: {categoryIds} found", string.Join(", ", ids));

            return Enumerable.Empty<Category>();
        }

        public async Task<Category> Create(AddCategoryRequest input, CancellationToken cancel = default)
        {
            var createdCategory = await _categoryRepository.Create(input, cancel);

            _logger.LogInformation("Category {categoryName} created", createdCategory.Name);

            return createdCategory;
        }

        public async Task Delete(int id, CancellationToken cancel = default)
        {
            var category = await _categoryRepository.GetById(id, cancel);

            if (category is null)
            {
                _logger.LogWarning("Category {categoryId} not found", id);
                throw new InvalidOperationException($"Category {id} not found");
            }

            await _categoryRepository.Delete(category, cancel);

            _logger.LogInformation("Deleted category {categoryId}", id);
        }
    }
}
