using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace CityInfo.API.Controllers
{


    [Route("api/v{version:apiVersion}/files")]
    [ApiController]
    public class FilesController : ControllerBase
    { //indicates the format of a file, document, or collection of bytes
        private readonly  FileExtensionContentTypeProvider _fileExtensionContentTypeProvider; //determines MIME type of files

        public FilesController(FileExtensionContentTypeProvider fileExtensionContentTypeProvider)
        {
            _fileExtensionContentTypeProvider= fileExtensionContentTypeProvider ?? throw
                 new System.ArgumentException(nameof(fileExtensionContentTypeProvider));
        }

        [HttpGet("{fileId}")]
        [ApiVersion(0.1, Deprecated = true)]
        public ActionResult GetFile(string fileId)
        {
            var pathToFile = "dotnet_core_tutorial.pdf";
            if (!System.IO.File.Exists(pathToFile))
            {
                return NotFound();
            }
            if (!_fileExtensionContentTypeProvider.TryGetContentType(pathToFile, out var contentType))
            { 
                contentType = "application/octet-stream";
            }
            var bytes = System.IO.File.ReadAllBytes(pathToFile);
            // return File(bytes, "text/plain", Path.GetFileName(pathToFile));
            return File(bytes, contentType, Path.GetFileName(pathToFile));
        }

        //uploading file
        [HttpPost]
        public async Task<ActionResult>CreateFile(IFormFile file)        //iFormFile specifies that file is uploading or we sent it by http request
        {
            //validate the input. Put the limit on the filsize to avoid the large uploads 
            //only accept the .pdf files( check content type)

            if(file.Length==0 || file.Length>20971520 || file.ContentType != "application/pdf")
            {
                return BadRequest("no file or invalid file is inputted");
            }

            //create the file path. Avoid using file.Filename, as an attacker can provide a 
            //malicious one including full path or relative path

            var path = Path.Combine(Directory.GetCurrentDirectory(), $"uploaded_file_{Guid.NewGuid()}.pdf");

            using (var stream=new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Ok("your file has been uploaded succesfully");

        }

    }
}
