namespace BLL.Dtos;

public record CategoryDto(Guid Id, string Name);

public record CreateCategoryDto(string Name, int MinAge, int MaxAge);
