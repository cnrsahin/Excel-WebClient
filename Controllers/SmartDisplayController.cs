using Grpc.Core;
using KoLeadForm.Entities;
using KoLeadForm.Models;
using KoLeadForm.Service;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace KoLeadForm.Controllers
{
    public class SmartDisplayController : Controller
    {
        static FormListViewModel model = new FormListViewModel { FormDetails = new List<FormDetail>() };

        private readonly IWebHostEnvironment _env;
        private readonly string _wwwroot;
        private const string folderName = "excels";

        public SmartDisplayController(IWebHostEnvironment env)
        {
            _env = env;
            _wwwroot = _env.WebRootPath;
        }

        [HttpGet]
        public IActionResult Index()
        {
            model.FormDetails.Clear();
            return View(model);
        }

        [HttpPost]
        public ActionResult Upload(IFormFile excelFile)
        {
            if (excelFile == null || excelFile.Length == 0)
                return RedirectToAction("Index");
            else
            {
                if (excelFile.FileName.EndsWith("csv"))
                {
                    var path = Path.Combine($"{_wwwroot}/{folderName}", excelFile.FileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        excelFile.CopyTo(stream);
                    }

                    string[] allLines = System.IO.File.ReadAllLines(path);

                    var query = from line in allLines.Skip(1)
                                let data = line.Split(',')
                                select new
                                {
                                    Name = data[2],
                                    Email = data[3],
                                    Phone = data[4],
                                };

                    foreach (var item in query)
                    {
                        FormDetail studentCandidate = new FormDetail
                        {
                            Fullname = item.Name,
                            Email = item.Email,
                            AreaCode = item.Phone.Substring(0, 3),
                            PhoneNumber = item.Phone.Substring(3)
                        };

                        model.FormDetails.Add(studentCandidate);
                    }

                    System.IO.File.Delete(path);

                    return View("Index", model);
                }
                else
                    return RedirectToAction("Index");
            }
        }

        public IActionResult Send()
        {
            SendTo sendTo = new SendTo();
            sendTo.SendKoForms(model.FormDetails, "google", "smartDisplay", "cpc");
            return RedirectToAction("Index");
        }
    }
}
