using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using YouWeMovie.Data;
using Microsoft.EntityFrameworkCore;
using YouWeMovie.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace YouWeMovie.Controllers;

public class AccountController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _um;

    public AccountController(ApplicationDbContext context, UserManager<ApplicationUser> um)
    {
        _context = context;
        _um = um;

    }

    [Authorize]
    public async Task<IActionResult> Index(string? id)
    {
        ApplicationUser? user;
        if (id == null)
            user = await _um.GetUserAsync(User);
        else
        {
            try
            {
                user = await _context.Users.FirstOrDefaultAsync(t=> t.Id == id);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return RedirectToAction("Index", "Home");
            }
        }
        
        if (user == null)
            return RedirectToAction("Index", "Home");

        var query = _context.Contents
            .Include(t => t.ApplicationUsers)
            .Where(t => t.ApplicationUsers.Any(e=>e.Id == user.Id))
            .AsQueryable();

        var movies = query.Where(t => t.IsSeries == false).ToList();
        var series = query.Where(t => t.IsSeries == true).ToList();
        
        
        ViewBag.PersonalMovies = movies;
        ViewBag.PersonalSeries = series;
        if (user.Email != null) ViewBag.UserEmail = user.Email;
        ViewBag.NickName = user.NickName;
        ViewBag.RegistrationDate = user.RegTime;
        ViewBag.Age = user.Age;
        ViewBag.UserId = user.Id;
        
        return View();
    }
    
    
    
    //to convert the byte array from pfp to a displayable image
    [SuppressMessage("ReSharper.DPA", "DPA0006: Large number of DB commands", MessageId = "count: 115")]
    public async Task<IActionResult> GetImage(string? userid)
    {
        if (userid == null)
            return Empty;
        
        //get the user from the inputted id
        var user = await _context.Users.FirstOrDefaultAsync(t=> t.Id == userid);
        if (user?.Pic == null)
            return Empty;

        //checks the file signature to find what image type we are dealing with
        //we dont need more than the 5 first bytes.
        var type = CheckSignature(user.Pic.AsSpan(0, 5));
        
        //finally returns the converted image
        return File(user.Pic, "image/" + type);
    }
    
    
    //checks file signature also known as magic number
    private static string CheckSignature(Span<byte> pic) // 'span' is used to get only the first few bytes
    {
        // makes sure file is big enough to be checked
        if (pic.Length != 5)
            return string.Empty;

        // actual checking of the first bytes:
        return pic[0] switch
        {
            0xFF when pic[1] == 0xD8 => "jpeg",                                    //jpeg and jpg
            0x89 when pic[1] == 0x50 && pic[2] == 0x4E && pic[3] == 0x47 => "png", //png
            0x47 when pic[1] == 0x49 && pic[2] == 0x46 => "gif",                   //gif
            _ => ""
        };
    }
}
