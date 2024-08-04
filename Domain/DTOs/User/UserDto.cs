namespace Domain.DTOs.User;

public class UserDto
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string FirstName {  get; set; } 

    public string LastName { get; set; }
    public string Email { get; set; }

}