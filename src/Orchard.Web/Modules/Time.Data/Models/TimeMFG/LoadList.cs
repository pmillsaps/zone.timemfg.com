using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace Time.Data.EntityModels.TimeMFG
{
    [MetadataType(typeof(LoadListMetadata))]
    public partial class LoadList
    {
        public string AllDistributors
        {
            get
            {
                return String.Join(",", this.LoadListDistributors.Select(x => x.Name).ToArray());
            }
        }
    }

    public class LoadListMetadata
    {
        [DisplayName("Issued Date")]
        public DateTime DateIssued { get; set; }
        [DisplayName("Revised Date")]
        public DateTime DateRevised { get; set; }
        [DisplayName("Ship Date")]
        public string DateSchedShip { get; set; }
        [DisplayName("Trucking Company")]
        public string TruckingCompany { get; set; }
        [UIHint("MultiLineText")]
        [DisplayName("Special Instructions")]
        public string Comments { get; set; }
        [UIHint("ByteCheckBox")]
        [DisplayName("Cmp")]
        public byte Complete { get; set; }
        [UIHint("ByteCheckBox")]
        [DisplayName("Mk Rdy")]
        public byte MakeReady { get; set; }

    }
}
