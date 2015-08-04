using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Time.Data.EntityModels.Epicor;

namespace Time.Epicor.ViewModels
{
    public class IMViewModel
    {
        public List<impartbin> ImPartBin { get; set; }
        public List<imjoboper> ImJobOper { get; set; }
    }
}