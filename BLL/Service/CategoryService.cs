using BLL.Dtos;
using BLL.Interfaces;
using BLL.Mappers;
using BLL.Service;
using DAL.Interfaces;
using Domain.Entities;

public class CategoryService : BaseService<Category, CategoryDto, CreateCategoryDto>, ICategoryService
{
    public CategoryService(ICategoryRepository repository) : base(repository) { }

    protected override CategoryDto ToDto(Category c) => c.ToDto();
    protected override Category ToEntity(CreateCategoryDto dto) => dto.ToEntity();
}