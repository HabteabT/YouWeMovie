using System.Text.Json;
using YouWeMovie.Models;
using Microsoft.AspNetCore.Identity;

namespace YouWeMovie.Data;

public class ApplicationDbInitializer
{
    public static void Initialize(ApplicationDbContext db)
    {
        // Delete the database before we initialize it. This is common to do during development.
        // Delete the database before we initialize it. This is common to do during development.
        db.Database.EnsureDeleted();
        // Recreate the database and tables according to our models
        db.Database.EnsureCreated();
        // Add test data to simplify debugging and testing
        

        db.SaveChanges();
    }
    
    //just so its easier to test
    public static async Task Initialize(ApplicationDbContext db, UserManager<ApplicationUser> um,
        RoleManager<IdentityRole> rm, IHttpClientFactory hcf)
    {
        await db.Database.EnsureDeletedAsync();
        await db.Database.EnsureCreatedAsync();

        var adminRole = new IdentityRole("Admin");
        rm.CreateAsync(adminRole).Wait();

        var admin = new ApplicationUser
        {
            UserName = "tester@hotmail.com", Email = "tester@hotmail.com", Age = 20, NickName = "Admin",
            EmailConfirmed = true
        };
        um.CreateAsync(admin, "Password1.").Wait();
        um.AddToRoleAsync(admin, "Admin").Wait();

        var user = new ApplicationUser
            { UserName = "user@uia.no", Email = "user@uia.no", Age = 20, NickName = "User", EmailConfirmed = true };
        um.CreateAsync(user, "Password1.").Wait();
        
        // client to make http requests
        var httpClient = hcf.CreateClient();

        var movieList = new List<string>
        {
            "The Shawshank Redemption" ,
            "The Godfather" ,
            "Forrest Gump" ,
            "Inception" ,
            "The Matrix" ,
            "Pulp Fiction" ,
            "Gladiator" ,
            "Titanic" ,
            "The Shining" ,
            "Jurassic Park" ,
            "The Dark Knight" ,
            "The Lion King" ,
            "Fight Club" ,
            "Avatar" ,
            "The Lord of the Rings: The Fellowship of the Ring" ,
            "Goodfellas" ,
            "The Silence of the Lambs" ,
            "Saving Private Ryan" ,
            "Coco" ,
            "The Departed" ,
            "Whiplash" ,
            "Mad Max: Fury Road" ,
            "La La Land" ,
            "Django Unchained" ,
            "The Prestige" ,
            "Interstellar" ,
            "The Green Mile" ,
            "The Great Gatsby" ,
            "The Social Network" ,
            "The Wolf of Wall Street" ,
            "A Beautiful Mind" ,
            "Braveheart" ,
            "Up" ,
            "The Incredibles" ,
            "Finding Nemo" ,
            "The Truman Show" ,
            "Gone with the Wind" ,
            "Casablanca" ,
            "Schindler's List" ,
            "Se7en" ,
            "The Usual Suspects" ,
            "Psycho" ,
            "Rear Window" ,
            "It's a Wonderful Life" ,
            "Singin' in the Rain" ,
            "Some Like It Hot" ,
            "Amélie" ,
            "Eternal Sunshine of the Spotless Mind" ,
            "The Grand Budapest Hotel" ,
            "Moonlight",    
            "Game of Thrones",
            "Breaking Bad",
            "The Crown",
            "Stranger Things",
            "The Office",
            "Friends",
            "Sherlock",
            "The Mandalorian",
            "The Witcher",
            "Westworld",
            "The Sopranos",
            "The Wire",
            "Black Mirror",
            "Doctor Who",
            "The Walking Dead",
            "Narcos",
            "Better Call Saul",
            "Fargo",
            "The Handmaid's Tale",
            "The Big Bang Theory",
            "Brooklyn Nine-Nine",
            "Parks and Recreation",
            "Dexter",
            "House",
            "Mad Men",
            "Twin Peaks",
            "Lost",
            "The X-Files",
            "Peaky Blinders",
            "The Boys",
            "Sex Education",
            "Money Heist",
            "Ozark",
            "Succession",
            "The Umbrella Academy",
            "Vikings",
            "True Detective",
            "Homeland",
            "The Marvelous Mrs. Maisel",
            "This Is Us",
            "Cobra Kai",
            "How I Met Your Mother",
            "The Simpsons",
            "Rick and Morty",
            "Arrow",
            "The Flash",
            "Star Trek: Discovery",
            "Grey's Anatomy",
            "Euphoria",
            "Bridgerton"
        };
        
        foreach (var requestUrl in movieList.Select(movie => "https://www.omdbapi.com/?apikey=52bbe196&plot=full&t=" + movie.Replace(" ", "+")))
        {

            var responseBody = await GetInfoFromApi(httpClient, requestUrl);

            Content? content;
            try
            {
                content = JsonSerializer.Deserialize<Content>(responseBody);
                if (content != null)
                    await db.AddAsync(content);
                else
                    Console.WriteLine("content is null");
            }
            catch
            {
                break;
            }
        }
        await db.SaveChangesAsync();
    }

    
    private static async Task<string> GetInfoFromApi(HttpClient client, string requestUrl)
    {
        // the json received will be a part of the response body, it will be stored in this variable
        string? responseBody;

        // http get request
        var httpRequest = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(requestUrl) // URL: https://www.omdbapi.com/?apikey=APIKEY&plot=full&t= + movie name
        };
        
        // the response from the website
        var httpResponse = await client.SendAsync(httpRequest);
        // makes sure the response is ok (code 200)
        if (httpResponse.IsSuccessStatusCode)
            responseBody = await httpResponse.Content.ReadAsStringAsync();
        else
            return "";

        return responseBody;
    }
    
}
