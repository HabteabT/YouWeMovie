using Microsoft.AspNetCore.Mvc;
using YouWeMovie.Data;
using Microsoft.EntityFrameworkCore;
using YouWeMovie.Models;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace YouWeMovie.Controllers;

public class ContentController : Controller
{
    //arver alt fra ApplicationDbContext slik at vi kan gjøre queries
    private readonly ApplicationDbContext _db;
    // View Engine lar rendre viewet i HTML og sende det til klienten
    private readonly ICompositeViewEngine _TheviewEngine;
    // Holder midlertidig data
    private readonly ITempDataProvider _temporaryDataProvider;
    

    //Lager en konstruktør for ContentController
    public ContentController(ApplicationDbContext db, ICompositeViewEngine theviewEngine, ITempDataProvider temporaryDataProvider)
    {
        // Vi henter instanser av følgende:
        _db = db;
        _TheviewEngine = theviewEngine;
        _temporaryDataProvider = temporaryDataProvider;
    }

    
    public async Task<IActionResult> Index(bool IsSeries = false)
    {
        //vår liste som skal bli bestående av enten filmer eller serier
        List<Content> contents;
        // setter verdien til IsSeries til viewbaggen.
        ViewBag.IsSeries = IsSeries;
        if (!IsSeries)
        {
            //Hvis det ikke er en serie setter vi IsSeries til false, henter da en liste med filmer
            contents = await _db.Contents.Where(c => c.IsSeries == false).ToListAsync();
        }
        else
        {
            //Hvis ikke setter vi IsSeries til true, henter da en liste med serier
            contents = await _db.Contents.Where(c => c.IsSeries == true).ToListAsync();
        }
        //returnerer index viewen og sender inn listen med contents til brukeren
        return View(contents);
    }

    [HttpPost]
    // en task som returnerer en JsonResult,
    public async Task<JsonResult> GetContent([FromBody] FilterRequest request)
    {
        // En LINQ query blir sendt til contents fra databasen der IsSeries er lik verdien til IsSeries i den nåverende variabelen i listen
        // Dette sørger for at vi enten bare gjøre queries for filmer eller serier avhengig av hvilken siden vi har trykket inn på
        IQueryable<Content> query = _db.Contents.Where(content => content.IsSeries == request.IsSeries);

        // Hvis ikke den skal vis alle filmene/seriene, vil den vise filmer/series fra valgt sjanger
        if (request.Genre != "All")
        {
            query = query.Where(content => content.Genre.Contains(request.Genre));
        }

        // queries etter hvilket filter d\en trykke på
        switch (request.FilterType)
        {
            //For høyeste filmer/serier, alstå sortert fra høyeste til laveste
            case "Highest":
                query = query.OrderByDescending(c => c.RatingImdb);
                break;
            //For laveste filmer/serier, altså sortert etter eldste til yngste
            case "oldest":
                query = query.OrderBy(c => c.ReleaseDate);
                break;
            //for nyeste filmer/serier, altså sortert fra laveste til høyeste
            case "newest":
                query = query.OrderByDescending(c => c.ReleaseDate);
                break;
            //Setter order by decending som standaren
            default:
                query = query.OrderByDescending(c => c.RatingImdb);
                break;
        }
        //eksekverer queries og gir tilbake en filtret liste av content fra databasen som list<Content>
        var content = await query.ToListAsync();
        //Sender Hteml stringen til partialtostring funksjonen, dette returnerer HTML som en string
        var TheHtmlString = await PartialToString("_ContentInfoPartial", content);
        return Json(new { success = true, html = TheHtmlString });
    }
    
    //Denne funksjonen konverterer partial viewn om til en string
    public async Task<string> PartialToString(string name, object model)
    {
        //Denne variabelen holder all informasjon som vi vil passere til viewen
        var AllData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
        //Dataen starter med ingen data  som betyr EmptyModelMetadataProvider og en ny ModelStateDictionary, dette betyr at AllData ikke har noe metedata til å begynne med
        {
            //lar Model få tilgang til model data
            Model = model
        };
        //Henter outputten til viewn, 
        using var WriteToString = new StringWriter();
        //Vi bruker FindView til å finne viewt med et spesifikt navn, 
        var output = _TheviewEngine.FindView(ControllerContext, name, false);
        //Dette er konteksten til viewet som skal rendres.
        var ContextForRender = new ViewContext(ControllerContext, output.View, AllData, new TempDataDictionary(HttpContext, _temporaryDataProvider), WriteToString, new HtmlHelperOptions()
        ); //den henter kontexten fra controllerenm output.view refererer til viewn som ble funnet av viewEngine, 
        // Rendrer viewen, await brukes slik at viewn kan bli ferdig med å rendre
        await output.View.RenderAsync(ContextForRender);
        // returnerer HTML som blir generert fra viewn, som en string
        return WriteToString.ToString();
    }
    
}

//De forskjellige filterene, etter filter type, sjanger og IsSeries
public class FilterRequest
{
    public string FilterType { get; set; }
    public string Genre { get; set; }
    public bool IsSeries { get; set; }
}