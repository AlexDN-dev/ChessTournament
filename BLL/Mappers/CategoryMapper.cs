using BLL.Dtos;
using Domain.Entities;

namespace BLL.Mappers;

internal static class CategoryMapper
{
    public static CategoryDto ToDto(this Category c)
        => new(c.Id, c.Name);

    public static Category ToEntity(this CreateCategoryDto dto)
        => new() { Name = dto.Name };
}
