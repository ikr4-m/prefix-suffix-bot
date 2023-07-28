using PrefixSuffixBot.Helper;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace PrefixSuffixBot;
public class MastodonOAuth
{
    private int _localPort = 45932;

    private DatabaseContext _db;
    private MastodonToken _token;
    private HttpClient _http;


    public MastodonOAuth(string uri, DatabaseContext db)
    {
        _token = new MastodonToken();
        _db = db;

        // Create HTTP Client
        _http = new HttpClient();
        _http.BaseAddress = new Uri(uri);
        _http.DefaultRequestHeaders.Add("User-Agent", "PrefixSuffixBot/C# .NET/7.0");

        // Pregenerate token
        _token.ClientName = "PrefixSuffixBot";
    }

    public async Task GenerateToken()
    {
        // Get client id and secret
        Logging.Info("Start requesting client secret to server.", "OAUTH");
        var res = await _http.PostAsJsonAsync("api/v1/apps", new
        {
            client_name =  _token.ClientName,
            redirect_uris = $"http://localhost:{_localPort}/",
            scope = "read write"
        });
        var resContent = await res.Content.ReadAsStringAsync();
        var tempContent = JsonConvert.DeserializeObject<object>(resContent);
        var printContent = JsonConvert.SerializeObject(tempContent, Formatting.Indented);

        if (!res.IsSuccessStatusCode)
        {
            Logging.Error(new Exception($"Server giving {res.StatusCode} with output:\n{printContent}"));
            Environment.Exit(1);
        }
        var content = JsonConvert.DeserializeObject<MastodonAppsResponse>(resContent);
        if (content == null)
        {
            Logging.Error(new Exception($"Content not successfully deserialzed."));
            Environment.Exit(1);
        }

        Logging.Info("Found the client secret! Importing to database...", "OAUTH");
        await _db.MastodonOAuth.AddAsync(new Helper.MastodonOAuth()
        {
            ID = content.ID,
            ClientID = content.ClientID,
            ClientName = _token.ClientName,
            ClientSecret = content.ClientSecret,
            Uri = _http.BaseAddress!.ToString()
        });
        await _db.SaveChangesAsync();
        Logging.Info("Importing to database done!", "OAUTH");
    }

    public async Task InitializeToken()
    {
        var data = await _db.MastodonOAuth.FirstOrDefaultAsync();
        if (data == null)
        {
            Logging.Error(new Exception("OAuth data not found!"));
            Environment.Exit(1);
        }

        _token = data;
    }
}

public class MastodonToken
{
    public string Token { get; set; } = string.Empty;
    public string ClientID { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
}

public class MastodonAppsResponse
{
    [JsonProperty("id")] public string ID { get; set; } = string.Empty;
    [JsonProperty("name")] public string Name { get; set; } = string.Empty;
    [JsonProperty("redirect_uri")] public string RedirectURI { get; set; } = string.Empty;
    [JsonProperty("client_id")] public string ClientID { get; set; } = string.Empty;
    [JsonProperty("client_secret")] public string ClientSecret { get; set; } = string.Empty;
}