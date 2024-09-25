using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace YouWeMovie.Models;

public class ApplicationUser : IdentityUser
{
    
    [Required] 
    [MaxLength(50)]
    public string NickName { get; set; } = string.Empty; //string.Empty;
    
    [Required]
    [Range(12, 120)]
    public int Age { get; set; }

    
    [Required]
    public DateTime RegTime { get; set; }

    
    // for the picture. might be changed if we use different database or something 
    [MaxLength(250_000)] // Max size of image is 250kB
    public byte[]? Pic { get; set; }
    
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<Content> Contents { get; set; } = new List<Content>();

    // If we later should add befriending system??:
    // public ICollection<ApplicationUser> ApplicationUsers { get; set; } = new List<ApplicationUser>();
    
    
}