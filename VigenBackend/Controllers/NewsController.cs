using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Vigen_Repository.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly string _newsApiKey;

        public NewsController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "VigenBackend/1.0");
            _newsApiKey = configuration["NewsApi:ApiKey"];
        }

        [HttpGet]
        public async Task<ActionResult<object>> GetNews()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_newsApiKey))
                {
                    return StatusCode(500, new { status = "error", message = "NewsAPI key is missing. Set it in appsettings.json or environment variables." });
                }

                // Calcular la fecha de hace 30 días
                var today = DateTime.Today;
                var halfMonthAgo = today.AddDays(-30);
                var formattedDate = halfMonthAgo.ToString("yyyy-MM-dd");

                // Construir la URL con parámetros codificados usando UriBuilder
                var uriBuilder = new UriBuilder("https://newsapi.org/v2/everything");
                var query = new List<string>
                {
                    $"q=Violencia%20de%20G%C3%A9nero", // Parámetro codificado manualmente
                    $"from={formattedDate}",
                    $"sortBy=publishedAt",
                    $"apiKey={_newsApiKey}"
                };
                uriBuilder.Query = string.Join("&", query);

                Console.WriteLine($"Request URL: {uriBuilder.Uri}");

                var response = await _httpClient.GetAsync(uriBuilder.Uri);

                if (!response.IsSuccessStatusCode)
                {
                    var errorResponse = await response.Content.ReadFromJsonAsync<NewsApiResponse>();
                    return StatusCode((int)response.StatusCode, new { status = "error", message = $"Error fetching news from NewsAPI: {response.ReasonPhrase}. Details: {errorResponse?.Message}" });
                }

                var newsData = await response.Content.ReadFromJsonAsync<NewsApiResponse>();

                if (newsData.Status != "ok")
                {
                    return BadRequest(new { status = "error", message = newsData.Message ?? "Error in NewsAPI response" });
                }

                var topNews = newsData.Articles.Take(3).ToArray();

                return Ok(new { status = "success", data = topNews });
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, new { status = "error", message = $"Internal server error: {ex.Message}" });
            }
            catch (System.Text.Json.JsonException ex)
            {
                return StatusCode(500, new { status = "error", message = $"Error deserializing NewsAPI response: {ex.Message}" });
            }
        }
    }

    public class NewsApiResponse
    {
        public string Status { get; set; }
        public int TotalResults { get; set; }
        public Article[] Articles { get; set; }
        public string Code { get; set; }
        public string Message { get; set; }
    }

    public class Article
    {
        public Source Source { get; set; }
        public string Author { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string UrlToImage { get; set; }
        public string PublishedAt { get; set; }
        public string Content { get; set; }
    }

    public class Source
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}