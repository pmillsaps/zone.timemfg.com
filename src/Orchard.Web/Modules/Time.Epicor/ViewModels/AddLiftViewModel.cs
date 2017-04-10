using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Time.Data.EntityModels.Epicor;
//using VersaliftDataServices.EntityModels.Epicor;

namespace Time.Epicor.ViewModels
{
    public class AddLiftViewModel
    {
        public int LoadListId { get; set; }
        public string Customer { get; set; }
        public string Search { get; set; }
        public List<SelectListItem> Customers { get; set; }
        public List<v_JobInformation> Jobs { get; set; }

        //[Required]
        [DisplayName("Job Number")]
        public string JobNumber { get; set; }
    }
}