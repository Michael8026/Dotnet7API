using Dotnet7API.Helper;
using Dotnet7API.Modal;
using Dotnet7API.Repos.Models;

namespace Dotnet7API.Service
{
    public interface ICustomerService
    {
        Task<List<CustomerModal>> GetAllCustomers();
        Task<CustomerModal> GetCustomersByCode(string code);
        Task<APIResponse> RemoveCustomer(string code);
        Task<APIResponse> CreateCustomer(CustomerModal customer);
        Task<APIResponse> UpdateCustomer(CustomerModal customer, string code);
    }
}
