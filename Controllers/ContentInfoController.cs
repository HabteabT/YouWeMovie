using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using YouWeMovie.Models;
using YouWeMovie.Data;


namespace YouWeMovie.Controllers;

public class ContentInfoController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _um;


    public ContentInfoController(ApplicationDbContext db, UserManager<ApplicationUser> um)
    {
        _db = db;
        _um = um;
    }

    // GET
    public async Task<IActionResult> Index(int id = 1)
    {
        ViewBag.isInList = false;
        Content? content;
        
        try
        {
            content = await _db.Contents.FirstOrDefaultAsync(t => t.Id == id);
            if (content == null)
            {
                return RedirectToAction("Index", "Home");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return RedirectToAction("Index", "Home");
        }


        if (User.Identity is { IsAuthenticated: false }) return View(content);

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return View(content);
        
        ApplicationUser? user;

        try
        {
            user = await _db.Users
                .Include(t => t.Contents)
                .FirstOrDefaultAsync(t => t.Id == userId);
            if (user == null) return View(content);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return RedirectToAction("Index", "Home");
        }
        
        if (user.Contents.Contains(content))
        {
            ViewBag.isInList = true;
        }
        
        return View(content);
    }


    [Authorize]
    [HttpPost]
    public async Task<IActionResult> AddOrDelete(int id = 0)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        ApplicationUser? user;
        
        try
        {
            user = await _db.Users
                .Include(t => t.Contents)
                .FirstOrDefaultAsync(t => t.Id == userId);
            if (user == null || !Exists(user.Id)) return RedirectToAction("Index");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return RedirectToAction("Index");
        }       

        Content? content;
        try
        {
            content = await _db.Contents.FirstOrDefaultAsync(t => t.Id == id);
            if (content == null) return BadRequest();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return BadRequest();
        }
        
        try
        {
            if (user.Contents.Contains(content))
                user.Contents.Remove(content);
            else
                user.Contents.Add(content);
            await _db.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return BadRequest();
        }

        return Json(new { success = true, message = "Database updated" });
    }

    private bool Exists(string id)
    {
        return _db.Users.Any(t => t.Id == id);
    }
}
