using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIPReconMatcher.Model
{
    public class WipRecon
    {
        public int EntryNumber { get; set; }
        public DateTime TranDate { get; set; }
        public string TranType { get; set; }
        public bool Posted { get; set; }
        public string CallNumber { get; set; }
        public string PartNumber { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public string Reference { get; set; }
        public string Ref_Customer { get; set; }
        public string Ref_Reference { get; set; }
        public bool Searched { get; set; }
        public bool Matched { get; set; }
        public string Account { get; set; }
        public string MatchedTransactions { get; set; }
        public bool PartMatched { get; set; }
    }
}
