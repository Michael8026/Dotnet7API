using Dotnet7API.Modal;
using Dotnet7API.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Dotnet7API.Controllers
{
    [Authorize]
    [EnableRateLimiting("fixed window")]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _service;
        public CustomerController(ICustomerService service)
        {
            _service = service;
        }

        [AllowAnonymous]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllCustomers()
        {
            var data = await _service.GetAllCustomers();

            if (data == null)
            {
                return NotFound();
            }

            return Ok(data);
        }

        [HttpGet("GetCustomerByCode")]
        public async Task<IActionResult> GetCustomerByCode(string code)
        {
            var data = await _service.GetCustomersByCode(code);

            if (data == null)
            {
                return NotFound();
            }

            return Ok(data);
        }

        [HttpPost("CreateCustomer")]
        public async Task<IActionResult> CreateCustomer(CustomerModal customer)
        {
            var data = await _service.CreateCustomer(customer);

            return Ok(data);
        }

        [HttpPut("UpdateCustomer")]
        public async Task<IActionResult> UpdateCustomer(CustomerModal customer, string code)
        {
            var data = await _service.UpdateCustomer(customer, code);

            return Ok(data);
        }

        [HttpDelete("RemoveCustomer")]
        public async Task<IActionResult> RemoveCustomer(string code)
        {
            var data = await _service.RemoveCustomer(code);

            return Ok(data);
        }

    }
}
