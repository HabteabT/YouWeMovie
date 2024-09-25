using Microsoft.AspNetCore.Mvc;
using YouWeMovie.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using YouWeMovie.Models;

namespace YouWeMovie.Controllers;

public class ReviewsController : Controller
{
    private readonly ApplicationDbContext _db;

    public ReviewsController(ApplicationDbContext db)
    {
        _db = db;
    }

    // GET: /Reviews/Index
    public async Task<IActionResult> Index(string filter)
    {
        var reviewsQuery = _db.Reviews
            .Include(r => r.ApplicationUser)
            .Include(r => r.Content)
            .AsQueryable();  // Start with IQueryable to build the query dynamically

        // Apply filtering logic based on the selected filter
        switch (filter)
        {
            case "newest": default:
                reviewsQuery = reviewsQuery.OrderByDescending(r => r.Time);
                break;
            case "oldest":
                reviewsQuery = reviewsQuery.OrderBy(r => r.Time);
                break;
            // Add more cases for other filters if needed
        }

        var reviews = await reviewsQuery.ToListAsync();
        ViewBag.SelectedFilter = filter;  // Store the selected filter in ViewBag for use in the view
        return View(reviews);
    }
    public async Task<IActionResult> DeleteReview(int reviewId)
    {
        // Retrieve the review from the database
        var review = await _db.Reviews.FindAsync(reviewId);
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        // Check if the review exists
            
        if (review == null)
        {
            // Handle the case where the review doesn't exist, for example, show an error message or redirect
            return RedirectToAction("Index");
        }

        if (userId == review.ApplicationUserId)
        {
            // Remove the review from the database
            _db.Reviews.Remove(review);
            await _db.SaveChangesAsync();
        }
            
        return RedirectToAction("Index");
    }
        
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddReview([Bind("Id, Score, Text, ContentId")] Review review)
    {
        // Ensure ContentId is set to the correct value
        review.ContentId = review.Id;
        review.Id = 0;

        if (ModelState.IsValid)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Check if the user already has a review for the content
            var existingReview = await _db.Reviews
                .FirstOrDefaultAsync(r => r.ContentId == review.ContentId && r.ApplicationUserId == userId);

            if (existingReview != null)
            {
                // Update the existing review
                existingReview.Score = review.Score;
                existingReview.Text = review.Text;

                // Save changes to the database
                await _db.SaveChangesAsync();

            }else{
                // Add the new review
                review.ApplicationUserId = userId;
                _db.Add(review);
                await _db.SaveChangesAsync();
            }

            try
            {
                var content = await _db.Contents.FirstOrDefaultAsync(t => t.Id == review.ContentId);
                if (content != null)
                {
                    var rating = _db.Reviews
                        .Where(t => t.ContentId == content.Id)
                        .Average(t => t.Score); 
                    Console.WriteLine(rating);
                    //avrundet til 1 decimal, trenger ikke mere
                    content.Rating =  Math.Round(rating * 2, 1); // times two since we only rate from 0 to 5, to make it match imdb rating range.
                    await _db.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            
        }
        
        // If something went wrong, redirect to the same page with the ContentId
        return RedirectToAction("Index", new { id = review.ContentId });
    }
}