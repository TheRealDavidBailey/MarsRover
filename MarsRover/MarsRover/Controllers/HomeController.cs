using System;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Collections;
using System.Threading.Tasks;
using Newtonsoft.Json;
using MarsRover.Models;
using Microsoft.AspNetCore.Hosting;



namespace MarsRover.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHostingEnvironment _env;

        public HomeController(IHostingEnvironment env)
        {
            _env = env;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetMarsRoverDates()
        {
            List<MarsRoverDate> lstMarsRoverDate = new List<MarsRoverDate>();
            var path = _env.WebRootPath + "\\dates\\dates.txt";
            var fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            var sr = new StreamReader(fs, Encoding.UTF8);

            string date = String.Empty;

            while ((date = sr.ReadLine()) != null)
            {
                DateTime marsRoverDate;
                if (DateTime.TryParse(date, out marsRoverDate))
                {
                    MarsRoverDate marsRoverDateString = new MarsRoverDate();
                    marsRoverDateString.DateForAPI = marsRoverDate.ToString("yyyy-MM-dd");
                    lstMarsRoverDate.Add(marsRoverDateString);
                }
            }
            return Json(new { MarRoverResults = lstMarsRoverDate });
        }

        [HttpPost]
        public JsonResult DownloadImages(List<string> images)
        {
            //string result = string.Empty;
            List<string> result = new List<string>();

            try
            {
                string folderDate = images[0].Split("|")[0];
                string downloadedImageFolder = _env.WebRootPath + "\\dates\\Downloaded_Images\\" + folderDate;

                if (!Directory.Exists(downloadedImageFolder))
                {
                    Directory.CreateDirectory(downloadedImageFolder);
                }

                foreach (string imageUrl in images)
                {
                    var filenameFullPath = imageUrl.Split("|")[1];
                    var filename = Path.GetFileName(filenameFullPath);
                    var wc = new System.Net.WebClient();
                    wc.DownloadFileAsync(new Uri(filenameFullPath), downloadedImageFolder + "\\" + filename);
                    result.Add("/dates/Downloaded_Images/" + folderDate + "/" + filename);
                }
            }
            catch(Exception ex)
            {
                result.Add(ex.Message);
            }
  
            return Json(new { message = result });
        }
    }
}
