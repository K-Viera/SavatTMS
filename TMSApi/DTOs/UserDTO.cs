namespace TMSApi;

public class UserDTO
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public RoleDTO Role { get; set; }
    public UserDTO(string email, string password, RoleDTO role)
    {
        Email = email;
        Password = password;
        Role = role;
    }
}
public enum RoleDTO
{
    Admin,
    User
}
