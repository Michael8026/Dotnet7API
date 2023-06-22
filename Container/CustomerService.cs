using AutoMapper;
using Dotnet7API.Helper;
using Dotnet7API.Modal;
using Dotnet7API.Repos;
using Dotnet7API.Repos.Models;
using Dotnet7API.Service;
using Microsoft.EntityFrameworkCore;

namespace Dotnet7API.Container
{
    public class CustomerService : ICustomerService
    {
        private readonly LearndataContextb _context;
        private readonly IMapper _mapper;
        private readonly ILogger<CustomerService> _logger;


        public CustomerService(LearndataContextb context, IMapper mapper, ILogger<CustomerService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

      
        public async Task<List<CustomerModal>> GetAllCustomers()
        {
            List<CustomerModal> response = new List<CustomerModal>();

            var customer = await _context.TblCustomers.ToListAsync();

            if (customer != null)
            {
                response = _mapper.Map<List <TblCustomer>, List<CustomerModal>>(customer);
            }

            return response;
        }

        public async Task<CustomerModal> GetCustomersByCode(string code)
        {
            CustomerModal response = new CustomerModal();

            var customer = await _context.TblCustomers.FindAsync(code);

            if (customer != null)
            {
                response = _mapper.Map<TblCustomer, CustomerModal>(customer);
            }

            return response;
        }

        public async Task<APIResponse> CreateCustomer(CustomerModal customer)
        {
            APIResponse response = new APIResponse();

            try
            {
                _logger.LogInformation("Create Begins");
                TblCustomer newCustomer = _mapper.Map<CustomerModal, TblCustomer>(customer);
                await _context.TblCustomers.AddAsync(newCustomer);
                await _context.SaveChangesAsync();
                response.ResponseCode = 201;
                response.Result = newCustomer.Code;
            }
            catch (Exception ex)
            {
                response.ResponseCode = 400;
                response.ErrorMessage = ex.Message;
                _logger.LogError(ex.Message, ex);
            }

            return response;
        }
        public async Task<APIResponse> RemoveCustomer(string code)
        {
            APIResponse response = new APIResponse();

            try
            {
                var customer = await _context.TblCustomers.FindAsync(code);

                if (customer != null)
                {
                    _context.TblCustomers.Remove(customer);
                    await _context.SaveChangesAsync();
                    response.ResponseCode = 200;
                    response.Result = code;
                }
                else
                {
                    response.ResponseCode = 404;
                    response.ErrorMessage = "Customer not found";
                }
            }
            catch (Exception ex)
            {
                response.ResponseCode = 400;
                response.ErrorMessage = ex.Message;
            }

            return response;
        }
                    
        
        public async Task<APIResponse> UpdateCustomer(CustomerModal customer, string code)
        {
            APIResponse response = new APIResponse();

            try
            {
                var oldCustomer = await _context.TblCustomers.FindAsync(code);

                if (customer != null)
                {
                    oldCustomer.Name = customer.Name;
                    oldCustomer.Email = customer.Email;
                    oldCustomer.Phone = customer.Phone;
                    oldCustomer.IsActive = customer.IsActive;
                    oldCustomer.CreditLimit = customer.CreditLimit;
                    oldCustomer.Taxcode = customer.Taxcode;

                    await _context.SaveChangesAsync();
                    response.ResponseCode = 200;
                    response.Result = code;
                }
                else
                {
                    response.ResponseCode = 404;
                    response.ErrorMessage = "Customer not found";
                }
            }
            catch (Exception ex)
            {
                response.ResponseCode = 400;
                response.ErrorMessage = ex.Message;
            }

            return response;
        }

       
    }
}
