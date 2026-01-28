namespace NanoDMSRightsService.DTO.Menu
{
    public record MenuCreateDto(
    string Name,
    string Code,
    string Route,
    string Icon,
    int Order,
    Guid? Parent_Id
);

}
