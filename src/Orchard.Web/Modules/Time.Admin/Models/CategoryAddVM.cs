using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Time.Admin.EntityModels.Versalift;

namespace Time.Admin.Models
{
    public class CategoryAddVM
    {
        public int ModelId { get; set; }
        public int CategoryId { get; set; }
        public SelectList Categories { get; set; }
        public VersaliftLiftModel Lift { get; set; }
    }
}