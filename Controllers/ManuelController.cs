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
    public class ManuelController : Controller
    {
        static FormListViewModel model = new FormListViewModel { FormDetails = new List<FormDetail>() };

        private readonly IWebHostEnvironment _env;
        private readonly string _wwwroot;
        private const string folderName = "excels";

        private static string utm_source;
        private static string utm_content;
        private static string utm_medium;

        public ManuelController(IWebHostEnvironment env)
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
        public ActionResult Upload(IFormFile excelFile, string source, string content, string medium, int name
            ,int email, int phone, int skipLine)
        {
            utm_source = source;
            utm_content = content;
            utm_medium = medium;

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

                    var query = from line in allLines.Skip(skipLine)
                                let data = line.Split(',')
                                select new
                                {
                                    Name = data[name],
                                    Email = data[email],
                                    Phone = data[phone],
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
            sendTo.SendKoForms(model.FormDetails, utm_source, utm_content, utm_medium);
            return RedirectToAction("Index");
        }
    }
}
