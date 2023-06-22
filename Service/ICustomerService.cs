using Dotnet7API.Modal;
using Dotnet7API.Repos.Models;

namespace Dotnet7API.Service
{
    public interface ICustomerService
    {
        List<CustomerModal> GetallCustomers();
    }
}
