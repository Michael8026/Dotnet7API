using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office.CustomUI;
using Dotnet7API.Modal;
using Dotnet7API.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Data;

namespace Dotnet7API.Controllers
{
    //[Authorize]
    [EnableRateLimiting("fixed window")]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _service;
        private readonly IWebHostEnvironment _environment;

        public CustomerController(ICustomerService service, IWebHostEnvironment environment)
        {
            _service = service;
            _environment = environment;
        }

        //[AllowAnonymous]
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

        //to export the Customer list to Excel
        [HttpGet("ExportExcel")]
        public async Task<IActionResult> ExportExcel()
        {
            try
            {
                string filePath = GetFilepath();
                string excelPath = filePath + "\\customerinfo.xlsx";

                DataTable dt = new DataTable();
                dt.Columns.Add("Code", typeof(string));
                dt.Columns.Add("Name", typeof(string));
                dt.Columns.Add("Email", typeof(string));
                dt.Columns.Add("Phone", typeof(string));
                dt.Columns.Add("CreditLimit", typeof(string));

                var data = await _service.GetAllCustomers();

                if (data != null && data.Count > 0)
                {
                    data.ForEach(item =>
                    {
                        dt.Rows.Add(item.Code, item.Name, item.Email, item.Phone, item.CreditLimit);
                    });
                }

                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.AddWorksheet(dt, "Customer Info");

                    using (MemoryStream stream = new MemoryStream())
                    {
                        wb.SaveAs(stream);

                        if (System.IO.File.Exists(excelPath))
                        {
                            System.IO.File.Delete(excelPath);
                        }
                        wb.SaveAs(excelPath);

                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Customer.xlsx");
                        
                    }
                }
            }
            catch (Exception)
            {
                return NotFound();
            }
        }


        [NonAction]
        private string GetFilepath()
        {
            return _environment.WebRootPath + "\\Export";
        }

    }
}
