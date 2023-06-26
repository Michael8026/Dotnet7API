namespace Dotnet7API.Service
{
    public interface IRefreshHandler
    {
        Task<string> GenerateToken(string username);
    }
}
