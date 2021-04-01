using KoLeadForm.Entities;
using KoLeadForm.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace KoLeadForm.Service
{
    public class SendTo
    {
        private string country;
        private string web;
        private string returnurl;
        private string uri;

        public void SendKoForms(List<FormDetail> forms, string utm_source, string utm_content, string utm_medium)
        {
            if (forms != null)
            {
                for (int i = 0; i < forms.Count; i++)
                {
                    using (WebClient webClient = new WebClient())
                    {
                        NameValueCollection nameValueCollection = new NameValueCollection();

                        nameValueCollection.Add("*", "*");
                        nameValueCollection.Add("*", "www.*.com");
                        nameValueCollection.Add("*", "");
                        nameValueCollection.Add("*", "Ücretsiz Deneyin");
                        nameValueCollection.Add("*", "");
                        nameValueCollection.Add("*", "");
                        nameValueCollection.Add("*", $"{utm_source}");
                        nameValueCollection.Add("*", $"{utm_medium}");
                        nameValueCollection.Add("*", "*");
                        nameValueCollection.Add("*", $"{utm_content}");
                        nameValueCollection.Add("*", "");
                        nameValueCollection.Add("*", "");
                        nameValueCollection.Add("*", "");
                        nameValueCollection.Add("*", "");
                        nameValueCollection.Add("*", "*");
                        nameValueCollection.Add("*", "");
                        nameValueCollection.Add("*", "");
                        nameValueCollection.Add("*", $"{forms[i].Fullname}");
                        nameValueCollection.Add("*", $"{forms[i].Email}");
                        nameValueCollection.Add("*", $"{forms[i].AreaCode}");
                        nameValueCollection.Add("*", $"{forms[i].PhoneNumber}");

                        webClient.UploadValuesAsync(new Uri("*"), nameValueCollection);
                        Thread.Sleep(1200);
                    }
                }
            }
        }

        public void SendGlobalForms(List<FormDetail> forms, string utm_source, string utm_content, FilterCountry filterCountry)
        {
            switch (filterCountry)
            {
                case FilterCountry.AZ:
                    country = "AZ";
                    web = "www.*.com";
                    returnurl = "*";
                    uri = "*";
                    break;
                case FilterCountry.EG:
                    country = "EG";
                    web = "www.alingliziah.com";
                    returnurl = "*";
                    uri = "*";
                    break;
                default:
                    break;
            }

            if (forms != null)
            {
                for (int i = 0; i < forms.Count; i++)
                {
                    using (WebClient webClient = new WebClient())
                    {
                        NameValueCollection nameValueCollection = new NameValueCollection();

                        nameValueCollection.Add("*", country);
                        nameValueCollection.Add("*", web);
                        nameValueCollection.Add("*", returnurl);
                        nameValueCollection.Add("*", "*");
                        nameValueCollection.Add("*", "");
                        nameValueCollection.Add("", $"{utm_source}");
                        nameValueCollection.Add("*", "");
                        nameValueCollection.Add("*", "");
                        nameValueCollection.Add("*", $"{utm_content}");
                        nameValueCollection.Add("*", "");
                        nameValueCollection.Add("*", "");
                        nameValueCollection.Add("*", $"{forms[i].Fullname}");
                        nameValueCollection.Add("*", $"{forms[i].Email}");
                        nameValueCollection.Add("*", $"{forms[i].AreaCode}");
                        nameValueCollection.Add("*", $"{forms[i].PhoneNumber}");

                        webClient.UploadValuesAsync(new Uri(uri), nameValueCollection);
                        Thread.Sleep(1200);
                    }
                }
            }
        }
    }
}

