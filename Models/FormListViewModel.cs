using KoLeadForm.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KoLeadForm.Models
{
    public class FormListViewModel
    {
        public List<FormDetail> FormDetails { get; set; }
        public int MyProperty { get; set; }
        public int MyProperty2 { get; set; }
    }
}
