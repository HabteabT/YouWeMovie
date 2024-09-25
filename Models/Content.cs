using System.Runtime.InteropServices.JavaScript;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Server.HttpSys;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace YouWeMovie.Models;

public class Content // This class will purely be used to store data from extern api. 
{
    public Content(){}
    
    public Content(string title, double rating, string synopsis, string producer, DateTime releaseDate, bool isSeries,
        string length, string genre, string picture)
    {
        Title = title;
        RatingImdb = rating;
        Synopsis = synopsis;
        Director = producer;
        ReleaseDate = releaseDate;
        IsSeries = isSeries;
        Length = length;
        Picture = picture;
        Genre = genre;
    }
        
    //autoincrement key
    public int Id { get; set; }
    
    //attributes
    public string? Title { get; set; }


    [JsonPropertyName("Poster")]
    public string? Picture { get; set; }
    
    
    [JsonPropertyName("Runtime")]
    public string? Length { get; set; }
    public string? Genre { get; set; }
    
    
    [JsonPropertyName("Plot")]
    public string? Synopsis { get; set; }
    
    public string? Director { get; set; }
    
    [JsonPropertyName("Released")]
    
    [JsonConverter(typeof(StringToDateTimeConverter))]
    public DateTime? ReleaseDate { get; set; }
    
    
    [JsonPropertyName("Type")]
    [JsonConverter(typeof(StringToBoolConverter))]
    public bool? IsSeries { get; set; }
    
    
    //rating just from IMDB:
    [JsonPropertyName("imdbRating")]
    [JsonConverter(typeof(StringToDoubleConverter))]
    public double? RatingImdb { get; set; }
    
    //Our rating
    public double? Rating { get; set; }
     

    // this is the list of all reviews
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    
    // this is the many to many connection to let users have personal (watched) list.
    public ICollection<ApplicationUser> ApplicationUsers { get; set; } = new List<ApplicationUser>();

}

// make a converter for the json parsing to convert string into double values.
public class StringToDoubleConverter : JsonConverter<double>
{
    public override double Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // if the type stored in the json string is already a double, that value will be returned
        if (reader.TokenType == JsonTokenType.Number)
            return reader.GetDouble();
        
        // if the type received is of anything other than string or number, a negative value will be returned (our program can't have negative rating)
        if (reader.TokenType != JsonTokenType.String)
            return -1.0;

        // if the type is of string, and correct format the string will be converted to double
        var stringValue = reader.GetString();
        return double.TryParse(stringValue, out var value) ? value : -1.0;
    }

    
    public override void Write(Utf8JsonWriter writer, double value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}




// A converter for the json parsing, to convert string to the correct datetime
public class StringToDateTimeConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // if the json was not read correctly (invalid json) we should stop the mapping of values
        if (reader.TokenType != JsonTokenType.String)
            return DateTime.MinValue;

        // converts the string to DateTime, returns 1/1/0001 12:00:00 AM if datetime was not correctly converted
        return DateTime.TryParseExact(reader.GetString(), "dd MMM yyyy",
            System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var date)
            ? date
            : DateTime.MinValue;
    }

    // converts the datetime back to the same format
    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString("dd MMM yyyy", System.Globalization.CultureInfo.InvariantCulture));
    }
}


public class StringToBoolConverter : JsonConverter<bool>
{
    // checks if the retreived json data, is a movie or series
    // returns true if it is a series.
    public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
            return false;

        return reader.GetString() == "series";
    }

    //also converts a true and false value to the same json attributes

    public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value ? "series" : "movie");
    }
}