using Microsoft.Identity.Client;

namespace TMSApi;

public interface IUserService
{
    public UserDTO? CheckUser(string email, string password);
}

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;
    public UserService(ApplicationDbContext context)
    {
        _context = context;
    }

    public UserDTO? CheckUser(string email, string password)
    {
        var user = _context.Users.FirstOrDefault(user => user.Email == email && user.Password == password);
        return user?.ToDTO();
    }
}
