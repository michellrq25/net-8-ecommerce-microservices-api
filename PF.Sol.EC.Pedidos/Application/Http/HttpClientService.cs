
using System.Net.Http;
using System.Text.Json;

namespace PF.Sol.EC.Pedidos.Application.Http
{
    public class HttpClientService : IHttpClientService
    {
        private readonly HttpClient httpClient;
        private readonly ILogger<HttpClientService> logger;

        public HttpClientService(HttpClient httpClient, ILogger<HttpClientService> logger)
        {
            this.httpClient = httpClient;
            this.logger = logger;
        }
        public async Task<T?> GetAsync<T>(string url)
        {
            try
            {
                logger.LogInformation($"Realizando petición GET. URL: {url}");
                var response = await httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode) 
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    logger.LogInformation($"Datos obtenidos exitosamente de URL: {url}");
                    return JsonSerializer.Deserialize<T>(jsonContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                    });
                }
                else
                {
                    logger.LogWarning($"La petición HTTP GET falló con el estado: {response.StatusCode}. URL: {url} ");
                    return default(T);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error realizando petición HTTP. URL: {url}");
                return default(T);
            }
        }

        public async Task<T?> PostAsync<T>(string url, object data)
        {
            try
            {
                logger.LogInformation($"Realizando petición POST. URL: {url}");
                var jsonContent = JsonSerializer.Serialize(data);
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync(url, content);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    logger.LogInformation($"Datos obtenidos exitosamente de URL: {url}");
                    return JsonSerializer.Deserialize<T>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
                else
                {
                    logger.LogWarning($"La petición HTTP POST falló con el estado: {response.StatusCode}. URL: {url} ");
                    return default(T);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error realizando petición HTTP. URL: {url}");
                return default(T);
            }
        }
    }
}
