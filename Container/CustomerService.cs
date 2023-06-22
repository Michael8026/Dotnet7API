using AutoMapper;
using Dotnet7API.Modal;
using Dotnet7API.Repos;
using Dotnet7API.Repos.Models;
using Dotnet7API.Service;

namespace Dotnet7API.Container
{
    public class CustomerService : ICustomerService
    {
        private readonly LearndataContextb _context;
        private readonly IMapper _mapper;


        public CustomerService(LearndataContextb context, IMapper mapper)
        {
            this._context = context;
            this._mapper = mapper;
        }


        public List<CustomerModal> GetallCustomers()
        {
            List<CustomerModal> response = new List<CustomerModal>();

            var customer =  _context.TblCustomers.ToList();

            if (customer != null)
            {
                response = _mapper.Map<List <TblCustomer>, List<CustomerModal>>(customer);
            }

            return response;
        }
    }
}
