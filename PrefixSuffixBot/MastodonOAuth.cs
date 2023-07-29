using PrefixSuffixBot.Helper;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace PrefixSuffixBot;
public class MastodonOAuth
{
    private string _localServerPath = $"http://localhost:45932/";
    private string _callbackPath = "";

    private DatabaseContext _db;
    private MastodonToken _token;
    private HttpClient _http;

    private string GenerateAuthorizeURI()
        => $"{_http.BaseAddress!.ToString()}oauth/authorize?response_type=code&client_id={_token.ClientID}&redirect_uri={_localServerPath}&scope=read write";

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

    public bool IsTokenAvailable() => _token.Token != "";

    public async Task GenerateToken()
    {
        Logging.Info("Make server for retrieve code", "OAUTH");
        var runServer = true;
        var listener = new HttpListener();
        listener.Prefixes.Add(_localServerPath);
        listener.Start();

        Logging.Info(
            $"Access OAuth from this url (this must be doing manually)\n\n{GenerateAuthorizeURI()}",
            "OAUTH");

        while (runServer)
        {
            var ctx = await listener.GetContextAsync();
            var req = ctx.Request;
            var res = ctx.Response;
            
            var uriString = req.Url!.ToString();
            if (uriString.Contains("?code="))
            {
                _callbackPath = uriString;
                runServer = false;
            }
            
            var data = Encoding.UTF8.GetBytes("You can back to the app.");
            res.ContentType = "text/html";
            res.ContentEncoding = Encoding.UTF8;
            res.ContentLength64 = data.LongLength;
            await res.OutputStream.WriteAsync(data, 0, data.Length);

            res.Close();
        }

        Logging.Info("Retrieving the real token.", "OAUTH");
        var privCode = _callbackPath.Split('?')[1].Split('=')[1];
        var resToken = await _http.PostAsJsonAsync("oauth/token", new
        {
            grant_type = "authorization_code",
            code = privCode,
            client_id = _token.ClientID,
            client_secret = _token.ClientSecret,
            redirect_uri = _localServerPath,
            scope = "read write"
        });
        var resContent = await resToken.Content.ReadAsStringAsync();
        var tempContent = JsonConvert.DeserializeObject<object>(resContent);
        var printContent = JsonConvert.SerializeObject(tempContent, Formatting.Indented);

        if (!resToken.IsSuccessStatusCode)
        {
            Logging.Error(new Exception($"Server giving {resToken.StatusCode} with output:\n{printContent}"));
            Environment.Exit(1);
        }
        var content = JsonConvert.DeserializeObject<MastodonTokenResponse>(resContent);
        if (content == null)
        {
            Logging.Error(new Exception($"Content not successfully deserialzed."));
            Environment.Exit(1);
        }

        _token.Token = content.AccessToken;
        _token.TokenType = content.TokenType;

        Logging.Info("Updating token to database", "OAUTH");
        var dbData = await _db.MastodonOAuth.Where(x => x.ClientID == _token.ClientID).FirstOrDefaultAsync();
        if (dbData == null)
        {
            Logging.Error(new Exception("ClientID not found!"));
            Environment.Exit(1);
        }

        dbData.Token = _token.Token;
        dbData.TokenType = _token.TokenType;
        _db.MastodonOAuth.Update(dbData);
        await _db.SaveChangesAsync();
        Logging.Info("Updating token to database done!", "OAUTH");
    }

    public async Task GenerateClientData()
    {
        // Get client id and secret
        Logging.Info("Start requesting client secret to server.", "OAUTH");
        var res = await _http.PostAsJsonAsync("api/v1/apps", new
        {
            client_name =  _token.ClientName,
            redirect_uris = _localServerPath,
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

    public async Task InitializeClientInfo()
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
    public string TokenType { get; set; } = string.Empty;
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

public class MastodonTokenResponse
{
    [JsonProperty("access_token")] public string AccessToken { get; set; } = string.Empty;
    [JsonProperty("created_at")] public long CreatedAt { get; set; }
    [JsonProperty("scope")] public string ScopeRaw { get; set; } = string.Empty;
    [JsonProperty("token_type")] public string TokenType { get; set; } = string.Empty;
}