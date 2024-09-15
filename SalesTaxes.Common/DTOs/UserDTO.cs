namespace SalesTaxes.Common.DTOs;

public class UserDTO
{
    public Guid Id { get; }
    public string Name { get; }
    public string State { get; }

    public UserDTO(string name, string state)
    {
        Id = Guid.NewGuid();
        Name = name;
        State = state;
    }
}
