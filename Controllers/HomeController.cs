using Microsoft.AspNetCore.Mvc;
using YouWeMovie.Models;
using YouWeMovie.Data;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
namespace YouWeMovie.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly ApplicationDbContext _db;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext dbContext, ApplicationDbContext db)
        {
            _logger = logger;
            _dbContext = dbContext;
            _db = db;
        }

        public IActionResult Index()
        {
            //Retrieve information from database:
            var movieContents = _dbContext.Contents.Where(c => c.IsSeries == false).Take(25).ToList();
            var seriesContents = _dbContext.Contents.Where(c => c.IsSeries == true).Take(25).ToList();
            var reviews = _dbContext.Reviews.Include(r => r.Content).Include(r => r.ApplicationUser).Take(10).ToList();


            ViewBag.MovieContents = movieContents;
            ViewBag.SeriesContents = seriesContents;
            ViewBag.Reviews = reviews;

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}