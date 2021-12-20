using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using WebMVC.Models;

namespace WebMVC.Controllers
{
    public class OrderController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;

        public OrderController(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        // GET: Order
        public ActionResult Index()
        {
            List<Order> orders = new List<Order>();
            string folderPath = Path.Combine(_hostingEnvironment.WebRootPath, "XML");
            foreach (string fileName in Directory.EnumerateFiles(folderPath, "*.xml"))
            {
                try
                {
                    orders.AddRange(Models.Upload.GetOrdersFromOneXml(folderPath, fileName));
                }
                catch
                { }
            }
            return View(orders);
        }

        // Upload
        [HttpGet]
        public ActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Upload(Upload model)
        {
            if (ModelState.IsValid)
            {
                // has selected a file
                if (model.XML_File != null)
                {
                    model.UploadMessage = model.CopyXMLFile(_hostingEnvironment);
                }
            }
            return View(model);
        }
    }
}
