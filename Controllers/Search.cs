using YouWeMovie.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YouWeMovie.Models;

namespace YouWeMovie.Controllers;

public class SearchController : Controller
{
    private readonly ApplicationDbContext _db;

    public SearchController(ApplicationDbContext db)
    {
        _db = db;
    }
    
    [HttpGet]
    public IActionResult Search(string input, Category cat = Category.All)
    {
        //makes all input lower so it is easier to match with titles from database
        input = input.ToLower();

        // depending on the category entered differend querys will be made
        switch (cat)
        {
            //for series and movies
            case Category.Series:
            case Category.Movies:
                var content = SearchContent(input, cat); // searches in database
                return Json(content);
                
            //for users
            case Category.Users:
                var users = SearchUsers(input); // searches in database
                return Json(users);
            
            //for everything
            case Category.All: default: 
                break;
        }
        var userResults = SearchUsers(input);
        var contentResults = SearchContent(input, cat);
        var results = new { Users = userResults, Contents = contentResults }; // combined result for both users and contents
        return Json(results);
    }


    // return the partial view of one content
    [HttpPost]
    public IActionResult ContentResult([FromBody]Content content)
    {
        return PartialView("_SearchResultPartial", content);
    }
    
    
    //return the partial view of one user
    [HttpPost]
    public IActionResult UserResult([FromBody]ApplicationUser user)
    {
        ViewBag.user = user;
        return PartialView("_SearchResultPartial");
    }
    

    public enum Category
    {
        All,
        Movies,
        Series,
        Users
    }
    
    
    // Searches the database to retreive movies or sereies
    //SQL: SELECT * FROM users WHERE title LIKE "%input%"
    private async Task<List<Content>> SearchContent(string searchInput, Category cat)
    {
        List<Content>? content;
        try
        {
            //Gets both series and movies
            if (cat == Category.All)
            {
                content = await _db.Contents
                    .Where(t => t.Title != null && t.Title.ToLower().Contains(searchInput))
                    .Take(15)   // adds a limit as there is no need for 100++ movies in the search results.
                    .ToListAsync();
            }
            //gets only one of them based on the input category
            else
            {
                content = await _db.Contents
                    .Where(t => t.IsSeries == (cat == Category.Series))
                    .Where(t => t.Title != null && t.Title.ToLower().Contains(searchInput))
                    .Take(15)   // limits request
                    .ToListAsync();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new List<Content>();
        }

        return content;
    }
    
    //searches through the database for users 
    //SQL: SELECT * FROM users WHERE name LIKE "%input%"
    private async Task<List<ApplicationUser>> SearchUsers(string searchInput)
    {
        List<ApplicationUser>? users;
        try
        {
            users = await _db.Users
                .Where(t => t.NickName.ToLower().Contains(searchInput))
                .Take(15)   // limits request
                .ToListAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new List<ApplicationUser>();
        }

        return users;
    }
}