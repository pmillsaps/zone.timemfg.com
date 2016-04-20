using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Time.Data.EntityModels.TimeMFG;

namespace Time.Epicor.ViewModels
{
    public class EmailLoadListVM
    {
        public int LoadListId { get; set; }
        public LoadList LoadList { get; set; }
        [UIHint("MultiLineText")]
        public string Comments { get; set; }
        public List<string> selectedLines { get; set; }
    }
}