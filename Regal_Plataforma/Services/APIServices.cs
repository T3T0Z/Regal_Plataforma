using System.Net.Http;
using System.Text;
using Google.Protobuf.WellKnownTypes;
using Newtonsoft.Json;
using Regal_Plataforma.Models.BDD;

public class ApiService : IAPIServices
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    private readonly string _company;
    private readonly string _user;
    private readonly string _password;
    private readonly REGAL_ASISTENCIAContext _context;

    public ApiService(HttpClient httpClient, IConfiguration configuration, REGAL_ASISTENCIAContext context)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _baseUrl = configuration["ApiSettings:BaseUrl"] ?? throw new ArgumentNullException("BaseUrl not configured");
        _company = configuration["ApiSettings:Company"] ?? throw new ArgumentNullException("Company not configured");
        _user = configuration["ApiSettings:Usuario"] ?? throw new ArgumentNullException("Username not configured");
        _password = configuration["ApiSettings:Contra"] ?? throw new ArgumentNullException("Password not configured");
        _context = context;
    }

    private async Task<string> GetTokenAsync()
    {
        var requestUrl = $"{_baseUrl}/seg_authWebServices/rest/loginUserService";

        var orderAuthorization = new
        {
            company = _company,
            user = _user,
            password = _password
        };

        var content = new StringContent(JsonConvert.SerializeObject(orderAuthorization), Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(requestUrl, content);
        var responseString = await response.Content.ReadAsStringAsync();

        // Deserializar JSON y extraer el valor de "sesion"
        dynamic jsonResponse = JsonConvert.DeserializeObject(responseString);
        string token = jsonResponse?.sesion;

        return token;
    }

    public async Task<string> GetOrderDetailsAsync(string orderId, string company, string claimNumber, string professionalId)
    {
        var token = await GetTokenAsync();
        var requestUrl = $"{_baseUrl}/cla_claimsManagementWebServices/rest/order/detail";

        var orderAuthorization = new
        {
            orderID = orderId,
            company = company,
            claimNumber = claimNumber,
            professionalID = professionalId
        };

        var content = new StringContent(JsonConvert.SerializeObject(orderAuthorization), Encoding.UTF8, "application/json");
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.PostAsync(requestUrl, content);
        var responseString = await response.Content.ReadAsStringAsync();

        return await response.Content.ReadAsStringAsync();
    }
}
