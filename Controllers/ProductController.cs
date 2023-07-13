using Dotnet7API.Helper;
using Dotnet7API.Repos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace Dotnet7API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;
        private readonly LearndataContextb _context;
        public ProductController(IWebHostEnvironment environment, LearndataContextb context)
        {
            _environment = environment;
            _context = context;
        }

        //to upload single image
        [HttpPut("UploadImage")]
        public async Task<IActionResult> UploadImage(IFormFile formfile, string productcode)
        {
            APIResponse response = new APIResponse();
            try
            {
                var filePath = GetFilepath(productcode);

                if (!System.IO.Directory.Exists(filePath))
                {
                    System.IO.Directory.CreateDirectory(filePath);
                }

                var imagePath = filePath + "\\" + productcode + ".png";

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
                using (FileStream stream = System.IO.File.Create(imagePath))
                {
                    await formfile.CopyToAsync(stream);
                    response.ResponseCode = 200;
                    response.Result = "pass";
                }
            }
            catch (Exception ex)
            {
                response.ErrorMessage = ex.Message;
                
            }
            return Ok(response);
        }


        //to upload multiple image
        [HttpPut("MultipleUploadImage")]
        public async Task<IActionResult> MultipleUploadImage(IFormFileCollection filecollection, string productcode)
        {
            APIResponse response = new APIResponse();
            int passcount = 0; int errorcount = 0;
            try
            {
                var filePath = GetFilepath(productcode);

                if (!System.IO.Directory.Exists(filePath))
                {
                    System.IO.Directory.CreateDirectory(filePath);
                }

                foreach (var file in filecollection)
                {
                    var imagePath = filePath + "\\" + file.FileName;

                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                    using (FileStream stream = System.IO.File.Create(imagePath))
                    {
                        await file.CopyToAsync(stream);
                        passcount++;
                        
                    }
                }                
                
            }
            catch (Exception ex)
            {
                errorcount++;
                response.ErrorMessage = ex.Message;

            }

            response.ResponseCode = 200;

            response.Result = passcount + "Files Uploaded &" + errorcount + "Files Failed";

            return Ok(response);
        }



        //to upload multiple image to DB
        [HttpPut("DBMultipleUploadImage")]
        public async Task<IActionResult> DBMultipleUploadImage(IFormFileCollection filecollection, string productcode)
        {
            APIResponse response = new APIResponse();
            int passcount = 0; int errorcount = 0;
            try
            {
                
                foreach (var file in filecollection)
                {
                    using (MemoryStream stream = new MemoryStream())
                    {
                        await file.CopyToAsync(stream);
                        _context.TblProductImages.Add(new Repos.Models.TblProductImage()
                        {
                            Productcode = productcode,
                            Productimage = stream.ToArray()
                        });
                        await _context.SaveChangesAsync();
                        passcount++;
                    }
                   
                }

            }
            catch (Exception ex)
            {
                errorcount++;
                response.ErrorMessage = ex.Message;

            }

            response.ResponseCode = 200;

            response.Result = passcount + "Files Uploaded &" + errorcount + "Files Failed";

            return Ok(response);
        }

        //to get single image
        [HttpGet("GetImage")]
        public async Task<IActionResult> GetImage(string productcode)
        {
            string imageUrl = string.Empty;

            string hostUrl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";

            try
            {
                string filePath = GetFilepath(productcode);

                string imagePath = filePath + "\\" + productcode + ".png";

                if (System.IO.File.Exists(imagePath))
                {
                    imageUrl = hostUrl + "/Upload/Product/" + productcode + "/" + productcode + ".png";
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {

                
            }

            return Ok(imageUrl);
        }



        //to get multiple image
        [HttpGet("GetMultipleImage")]
        public async Task<IActionResult> GetMultipleImage(string productcode)
        {
            List<string> imagesUrl = new List<string>();

            string hostUrl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";

            try
            {
                string filePath = GetFilepath(productcode);

                if (System.IO.Directory.Exists(filePath))
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(filePath);
                    FileInfo[] fileInfos = directoryInfo.GetFiles();

                    foreach (FileInfo fileInfo in fileInfos)
                    {
                        string fileName = fileInfo.Name;
                        string imagePath = filePath + "\\" + fileName;

                        if (System.IO.File.Exists(imagePath))
                        {
                            string imageUrl = hostUrl + "/Upload/Product/" + productcode + "/" + fileName;
                            imagesUrl.Add(imageUrl);

                        }
                    }
                }
                
            }
            catch (Exception ex)
            {


            }

            return Ok(imagesUrl);
        }


        //to get multiple image from Database
        [HttpGet("GetDBMultipleImage")]
        public async Task<IActionResult> GetDBMultipleImage(string productcode)
        {
            List<string> imagesUrl = new List<string>();

            try
            {
                var productImage = _context.TblProductImages.Where(item => item.Productcode == productcode).ToList();

                if (productImage != null && productImage.Count > 0)
                {
                    productImage.ForEach(item =>
                    {
                        imagesUrl.Add(Convert.ToBase64String(item.Productimage));
                    });
                }
                else
                {
                    return NotFound();
                }


            }
            catch (Exception ex)
            {


            }

            return Ok(imagesUrl);
        }




        //to download image
        [HttpGet("Download")]
        public async Task<IActionResult> Download(string productcode)
        {

            try
            {
                string filePath = GetFilepath(productcode);

                string imagePath = filePath + "\\" + productcode + ".png";

                if (System.IO.File.Exists(imagePath))
                {
                    MemoryStream stream = new MemoryStream();
                    using (FileStream fileStream = new FileStream(imagePath, FileMode.Open))
                    {
                        await fileStream.CopyToAsync(stream);
                    }
                    stream.Position = 0;
                    return File(stream, "image/png", productcode + ".png");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return NotFound();

            }

            
        }


        //to download image from database
        [HttpGet("DbDownload")]
        public async Task<IActionResult> DbDownload(string productcode)
        {
            
            try
            {
                var productImage = await _context.TblProductImages.FirstOrDefaultAsync(item => item.Productcode == productcode);
                

                if (productImage != null)
                {
                    return File(productImage.Productimage, "image/png", productcode + ".png");
                }

                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return NotFound();

            }


        }



        //to delete single image
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(string productcode)
        {

            try
            {
                string filePath = GetFilepath(productcode);

                string imagePath = filePath + "\\" + productcode + ".png";

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);

                    return Ok("pass");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return NotFound();

            }


        }




        //to delete multiple image
        [HttpDelete("MultipleDelete")]
        public async Task<IActionResult> MultipleDelete(string productcode)
        {

            try
            {
                string filePath = GetFilepath(productcode);


                if (System.IO.Directory.Exists(filePath))
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(filePath);
                    FileInfo[] fileInfos = directoryInfo.GetFiles();

                    foreach (FileInfo fileInfo in fileInfos)
                    {
                        fileInfo.Delete();
                    }

                    return Ok("Pass");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return NotFound();

            }

        }






        [NonAction]
        private string GetFilepath(string productcode)
        {
            return _environment.WebRootPath + "\\Upload\\Product\\" + productcode;
        }
    }
}
