using FileApiConsume.DTO;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace FileApiConsume.Controllers
{
    public class FileController : Controller
    {
        HttpClient client;
        public FileController()
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
            client = new HttpClient(clientHandler);
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult UploadFile(FileDto f)
        {
            string url = "https://localhost:7278/api/File/fileupload";

            using (var form = new MultipartFormDataContent())
            {

                form.Add(new StringContent(f.ImgName ?? ""), "ImgName");

                if (f.ImgPath != null && f.ImgPath.Length > 0)
                {
                    var stream = f.ImgPath.OpenReadStream();
                    form.Add(new StreamContent(stream), "ImgPath", f.ImgPath.FileName);
                }

                // Add Isdeleted (default false from model)
                //form.Add(new StringContent(f.Isdeleted.ToString().ToLower()), "Isdeleted");

                var response = client.PostAsync(url, form).Result;

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
            }

            return RedirectToAction("Index");
        }
        public IActionResult ViewFiles()
        
        {
            string url = "https://localhost:7278/api/File/FetchFile"; 
            List<ViewFileDto> files = new List<ViewFileDto>();

            var response = client.GetAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {
                var jsonData = response.Content.ReadAsStringAsync().Result;
                files = JsonConvert.DeserializeObject<List<ViewFileDto>>(jsonData);
            }

            return View(files);
        }
        [HttpPost]
        public IActionResult SoftDeleteImages([FromBody] List<int> ids)
        {
            if (ids == null || ids.Count == 0)
                return BadRequest("No IDs provided.");

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri("https://localhost:7278/api/Files/DeleteFiles"),
                Content = new StringContent(JsonConvert.SerializeObject(ids), Encoding.UTF8, "application/json")
            };

            var response = client.SendAsync(request).Result;

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = response.Content.ReadAsStringAsync().Result; 
                return StatusCode((int)response.StatusCode, errorContent);
            }

            return Ok(new { message = "Images deleted successfully" });
        }

    }
}
