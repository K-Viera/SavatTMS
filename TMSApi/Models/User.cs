using System.ComponentModel.DataAnnotations;

namespace TMSApi;

public class User
{
    [Key]
    public int UserId { get; set; }
    
    [Required]
    [StringLength(255)]
    [EmailAddress]
    public string Email { get; set; } = null!;
    
    [Required]
    [StringLength(255)]
    public string Password { get; set; } = null!;
    
    [Required]
    [StringLength(50)]
    public string Role { get; set; } = null!;
    public User()
    {
        
    }
    public UserDTO ToDTO()
    {
        return new UserDTO(Email, Password, Role == "Admin" ? RoleDTO.Admin : RoleDTO.User);
    }
}
