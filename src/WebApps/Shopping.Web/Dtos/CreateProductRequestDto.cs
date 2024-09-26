namespace Shopping.Web.Dtos;

public record CreateProductRequestDto(
    string Name,
    List<string> Category,
    string Description,
    string ImageFile,
    decimal Price
);
