using AutoMapper;
using Dotnet7API.Modal;
using Dotnet7API.Repos.Models;

namespace Dotnet7API.Helper
{
    public class AutoMapperHandler : Profile
    {
        public AutoMapperHandler()
        {
            CreateMap<TblCustomer, CustomerModal>().ForMember(item => item.StatusName, opt
                => opt.MapFrom(item => (item.IsActive != null && item.IsActive.Value) ? "Active" : "In Active")).ReverseMap();
        }
    }
}
