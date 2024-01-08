using MessagePack;
using System.Buffers;
using TemplateBlazorWasmHostedNet8.Shared.Extensions;

namespace TemplateBlazorWasmHostedNet8.Api.Endpoints;

public static class MapDogtEndpointRouteBuilder
{
    public static IEndpointConventionBuilder MapDogEndpoints(this IEndpointRouteBuilder endpoints)
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        var accountGroup = endpoints.MapGroup("/Dog");
            //.AllowAnonymous();

        //accountGroup.MapPost("/Login", (
        //    HttpContext context,
        //    [FromServices] SignInManager<ApplicationUser> signInManager,
        //    [FromForm] string provider,
        //    [FromForm] string returnUrl) =>
        //{
        //    return 
        //});

        accountGroup.MapGet("/get", () =>
        {
            return Results.Ok("OK");
        })
        .RequireAuthorization() ;


        // Message Pack
        accountGroup.MapPost("/getbinary", async (
            [FromServices] MessagePackService messagePackService, 
            IHttpClientFactory httpClientFactory,
            [FromBody] DogDto dto) =>
        {
            //var json = new Root
            //{
            //    info =
            //    {
            //        page = 1,
            //        seed = "seed",
            //        version = "1",
            //        results = 1
            //    },
            //    //results =
            //    //[
            //    //    new Result
            //    //    {
            //    //        gender = "female",
            //    //        name = {
            //    //        title= "Mademoiselle",
            //    //        first= "Marlen",
            //    //        last= "Berger"
            //    //    },
            //    //        location = {
            //    //        street= {
            //    //        number= 2089,
            //    //        name= "Montée Saint-Barthélémy"
            //    //    },
            //    //    city= "Rümlang",
            //    //        state= "Zürich",
            //    //        country= "Switzerland",
            //    //        postcode= 2884,
            //    //        coordinates= {
            //    //        latitude= "-64.0018",
            //    //        longitude= "147.7674"
            //    //    },
            //    //    timezone= {
            //    //        offset= "+2:00",
            //    //        description= "Kaliningrad, South Africa"
            //    //        }
            //    //    },
            //    //        email = "marlen.berger@example.com",
            //    //        login = {
            //    //        uuid= "a00833bf-7c41-4433-bd8c-51fc67354168",
            //    //        username= "silvergoose185",
            //    //        password= "lemons",
            //    //        salt= "WthsnmUZ",
            //    //        md5= "1524cdf6076bacaa7381be0ea8bf61ab",
            //    //        sha1= "14305fbcefc885b75ac16d7be903641be7292ae8",
            //    //        sha256= "82de535dbd3a530ffacf16565d4bb7b3c0d769ddd21e4e16cc9fa343ed5934a1"
            //    //    },
            //    //        dob = {
            //    //        date= DateTime.Now,
            //    //        age= 23
            //    //    },
            //    //        registered = {
            //    //        date= DateTime.Now,
            //    //        age= 6
            //    //    },
            //    //        phone = "076 203 51 33",
            //    //        cell = "079 371 49 49",
            //    //        id = {
            //    //        name= "AVS",
            //    //        value= "756.2258.0153.14"
            //    //    },
            //    //        picture = {
            //    //        large= "https://randomuser.me/api/portraits/women/41.jpg",
            //    //        medium= "https://randomuser.me/api/portraits/med/women/41.jpg",
            //    //        thumbnail= "https://randomuser.me/api/portraits/thumb/women/41.jpg"
            //    //    },
            //    //        nat = "CH"
            //    //    }
            //    //]
            //};

            //        var json = new Location
            //        {
            //            street = {
            //                number = 2089,
            //    name = "Montée Saint-Barthélémy"
            //        },
            //            city = "Rümlang",
            //            state = "Zürich",
            //            country = "Switzerland",
            //            postcode = 2884,
            //            coordinates = {
            //                latitude = "-64.0018",
            //    longitude = "147.7674"
            //},
            //            timezone = {
            //                offset = "+2:00",
            //    description = "Kaliningrad, South Africa"
            //    }
            //        };

            var json = new Login
            {
                md5 = "asasasasas",
                password = "password",
                salt = "password",
                sha1 = "password",
                sha256 = "password",
                username = "username",
                uuid = "password"
            };

            // https://randomuser.me/api/
            var binary = messagePackService.Serialize(json);


            var toJson = messagePackService.Deserialize<Login>(binary.Data);
            var bytes = binary.Data.ToArray();
            //return bytes;


            using var httpclient = httpClientFactory.CreateClient();
            httpclient.BaseAddress = new Uri("https://random-word-api.herokuapp.com/all");
            var result = await httpclient.GetAsync("");
            if (!result.IsSuccessStatusCode) throw new Exception("Erro na solicitação de terceiro");

            var dados = await result.Content.ReadFromJsonAsync<List<string>>();

            dados = dados.Randomize().ToList();

            return new LoginDto(dados.First(), dados.Last());
        }).RequireAuthorization();

        return accountGroup;
    }
}

[MessagePackObject]
public record Coordinates
{
    [Key(0)]
    public string latitude { get; set; }

    [Key(1)]
    public string longitude { get; set; }
}

[MessagePackObject]
public record Dob
{
    [Key(0)]
    public DateTime date { get; set; }

    [Key(1)]
    public int age { get; set; }
}

[MessagePackObject]
public record Id
{
    [Key(0)]
    public string name { get; set; }

    [Key(1)]
    public string value { get; set; }
}

[MessagePackObject]
public record Info
{
    [Key(0)]
    public string seed { get; set; }

    [Key(1)]
    public int results { get; set; }

    [Key(2)]
    public int page { get; set; }

    [Key(3)]
    public string version { get; set; }
}

[MessagePackObject]
public record Location
{
    [Key(0)]
    public Street street { get; set; }

    [Key(1)]
    public string city { get; set; }

    [Key(2)]
    public string state { get; set; }

    [Key(3)]
    public string country { get; set; }

    [Key(4)]
    public int postcode { get; set; }

    [Key(5)]
    public Coordinates coordinates { get; set; }

    [Key(6)]
    public Timezone timezone { get; set; }
}

[MessagePackObject]
public record Login
{
    [Key(0)]
    public string uuid { get; set; }

    [Key(1)]
    public string username { get; set; }

    [Key(2)]
    public string password { get; set; }

    [Key(3)]
    public string salt { get; set; }

    [Key(4)]
    public string md5 { get; set; }

    [Key(5)]
    public string sha1 { get; set; }

    [Key(6)]
    public string sha256 { get; set; }
}

[MessagePackObject]
public record Name
{
    [Key(0)]
    public string title { get; set; }

    [Key(1)]
    public string first { get; set; }

    [Key(2)]
    public string last { get; set; }
}

[MessagePackObject]
public record Picture
{
    [Key(0)]
    public string large { get; set; }

    [Key(1)]
    public string medium { get; set; }

    [Key(2)]
    public string thumbnail { get; set; }
}

[MessagePackObject]
public record Registered
{
    [Key(0)]
    public DateTime date { get; set; }

    [Key(1)]
    public int age { get; set; }
}


[MessagePackObject]
public record Result
{
    [Key(0)]
    public string gender { get; set; }

    [Key(1)]
    public Name name { get; set; }

    [Key(2)]
    public Location location { get; set; }

    [Key(3)]
    public string email { get; set; }

    [Key(4)]
    public Login login { get; set; }

    [Key(5)]
    public Dob dob { get; set; }

    [Key(6)]
    public Registered registered { get; set; }

    [Key(7)]
    public string phone { get; set; }

    [Key(8)]
    public string cell { get; set; }

    [Key(9)]
    public Id id { get; set; }

    [Key(10)]
    public Picture picture { get; set; }

    [Key(11)]
    public string nat { get; set; }
}

[MessagePackObject]
public record Root
{

    [Key(0)]
    public List<Result> results { get; set; }

    [Key(1)]
    public Info info { get; set; }
}

[MessagePackObject]
public record Street
{
    [Key(0)]
    public int number { get; set; }

    [Key(1)]
    public string name { get; set; }
}

[MessagePackObject]
public record Timezone
{
    [Key(0)]
    public string offset { get; set; }

    [Key(1)]
    public string description { get; set; }
}
