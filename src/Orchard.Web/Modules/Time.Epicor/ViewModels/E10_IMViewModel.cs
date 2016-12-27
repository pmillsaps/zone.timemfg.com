using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Time.Data.EntityModels.Production;

namespace Time.Epicor.ViewModels
{
    public class E10_IMViewModel
    {
        public List<IMPartBin> ImPartBin { get; set; }
        public List<IMJobOper> ImJobOper { get; set; }
    }
}