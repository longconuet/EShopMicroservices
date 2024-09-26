namespace Shopping.Web.Dtos;

public record UpdateProductRequestDto(
    Guid Id,
    string Name,
    List<string> Category,
    string Description,
    string ImageFile,
    decimal Price
);
