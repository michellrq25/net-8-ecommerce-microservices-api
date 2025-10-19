namespace PF.Sol.EC.Pedidos.Application.Http
{
    public interface IHttpClientService
    {
        Task<T?> GetAsync<T>(string url);
        Task<T?> PostAsync<T>(string url, object data);
    }
}
