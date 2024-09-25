using System.ComponentModel.DataAnnotations;

namespace YouWeMovie.Models;

public class Review
{
    public Review(){}
    public Review(float score, string text = ""){}
    
    //key
    public int Id { get; set; }
    
    
    //attributes
    [Range(0, 5)]
    public float Score { get; set; }

    [StringLength(5000)]
    public string? Text { get; set; } = string.Empty;

    public DateTime Time { get; set; } = DateTime.Now;

    
    //foreign keys
    public int ContentId { get; set; }
    public string ApplicationUserId { get; set; } = string.Empty;
    public ApplicationUser? ApplicationUser { get; set; }
    public Content? Content { get; set; }
}