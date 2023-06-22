using Dotnet7API.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dotnet7API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _service;
        public CustomerController(ICustomerService service)
        {
            this._service = service;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var data = _service.GetallCustomers();
            if (data == null)
            {
                return NotFound();
            }

            return Ok(data);
        }
    }
}
