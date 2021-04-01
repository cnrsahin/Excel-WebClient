using ExcelDataReader;
using Grpc.Core;
using KoLeadForm.Entities;
using KoLeadForm.Models;
using KoLeadForm.Service;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml.Core.ExcelPackage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace KoLeadForm.Controllers
{
    public class FacebookController : Controller
    {
        static FormListViewModel model = new FormListViewModel { FormDetails = new List<FormDetail>() };

        private readonly IWebHostEnvironment _env;
        private readonly string _wwwroot;
        private const string folderName = "excels";

        public FacebookController(IWebHostEnvironment env)
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
                if (excelFile.FileName.EndsWith("xlsx"))
                {
                    var path = Path.Combine($"{_wwwroot}/{folderName}", excelFile.FileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        excelFile.CopyTo(stream);
                    }

                    System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

                    using (var stream = System.IO.File.Open(path, FileMode.Open, FileAccess.Read))
                    {
                        using (var excelRead = ExcelReaderFactory.CreateReader(stream))
                        {
                            do
                            {
                                while (excelRead.Read())
                                {
                                    var studentCandidates = new FormDetail
                                    {
                                        Fullname = excelRead.GetString(12),
                                        Email = excelRead.GetString(13),
                                        AreaCode = excelRead.GetString(14).Substring(2, 3),
                                        PhoneNumber = excelRead.GetString(14).Substring(5)
                                    };

                                    model.FormDetails.Add(studentCandidates);
                                }
                            } while (excelRead.NextResult());
                        }
                    }

                    System.IO.File.Delete(path);
                    model.FormDetails.RemoveAt(0);

                    return View("Index", model);
                }
                else
                    return RedirectToAction("Index");
            }
        }

        public IActionResult Send()
        {
            SendTo sendTo = new SendTo();
            sendTo.SendKoForms(model.FormDetails, "facebook", "facebookLeadAds", string.Empty);
            return RedirectToAction("Index");
        }
    }
}
