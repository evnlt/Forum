namespace Forum.WebApi.Entities;

public class User
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public string Name { get; set; } = default!;
}