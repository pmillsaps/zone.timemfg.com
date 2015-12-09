using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using WIPReconMatcher.Model;

namespace WIPReconMatcher
{
    internal class Program
    {
        private const string SearchAccount = "11600-001-01";
        private const string suspectFile = @"C:\Users\PaulM\Desktop\WipRecon14Full_Suspect.csv";
        private const string outFile = @"C:\Users\PaulM\Desktop\WipRecon14Full_Matched.csv";

        private static List<PartHolder> UnMatchedParts { get; set; }

        private static List<string> UnMatchedPartsLog { get; set; }

        private static List<WipRecon> transactions { get; set; }

        //static string outFile { get; set; }
        //static string suspectFile { get; set; }

        private static void Main(string[] args)
        {
            string filename = @"C:\Users\PaulM\Desktop\WipRecon14Full.txt";
            //outFile = @"C:\Users\PaulM\Desktop\WipRecon14Full_Matched.csv";
            //suspectFile = @"C:\Users\PaulM\Desktop\WipRecon14Full_Matched.csv";
            transactions = new List<WipRecon>();
            UnMatchedParts = new List<PartHolder>();
            UnMatchedPartsLog = new List<string>();
            ProcessFile(filename);
        }

        private static void ProcessFile(string filename)
        {
            int entryNumber = 1;
            string currentAccount = String.Empty;
            transactions.Clear();
            TextFieldParser parser = new TextFieldParser(filename);
            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(",");
            while (!parser.EndOfData)
            {
                string[] fields = parser.ReadFields();
                if (fields.Count() >= 6 && fields[0] != "Account :" && fields[7] != "")  // Ship over the first row, which will not match
                {
                    WipRecon wip = new WipRecon();
                    wip.EntryNumber = entryNumber++;
                    wip.TranDate = DateTime.Parse(fields[0]);
                    wip.TranType = fields[1];
                    // wip.Posted = Boolean.Parse(fields[2]);
                    if (fields[2] == "Y") wip.Posted = true; else wip.Posted = false;
                    wip.CallNumber = fields[3];
                    wip.PartNumber = fields[4];
                    wip.Debit = Decimal.Parse(fields[5]);
                    wip.Credit = Decimal.Parse(fields[6]);
                    wip.Reference = fields[7];
                    var first = wip.Reference.IndexOf(":");
                    var end = wip.Reference.IndexOf(" ");
                    wip.Ref_Customer = wip.Reference.Substring(first + 1, end - first).Trim();
                    // wip.Ref_Customer = "";
                    first = wip.Reference.LastIndexOf(":");
                    wip.Ref_Reference = wip.Reference.Substring(first + 1).Trim();
                    wip.Account = currentAccount;

                    transactions.Add(wip);
                }
                else
                {
                    Console.WriteLine("{0} - {1} - {2} - {3} - {4} - {5} - {6} - {7}", fields[0], fields[1], fields[2], fields[3], fields[4], fields[5], fields[6], fields[7]);
                    if (fields[0].Contains("Account") && fields[1].Length > 10) currentAccount = fields[1].Substring(0, 12).Trim();
                }
            }

            Console.WriteLine("Lines Parsed: {0}", transactions.Count());

            //var tranlist = transactions.ToList();
            int trancount = transactions.Count(x => x.Searched != true && x.Account == SearchAccount);

            while (trancount > 0)
            {
                var item = transactions.First(x => x.Searched == false && x.Account == SearchAccount);
                if (item.EntryNumber % 1000 == 0)
                {
                    Console.WriteLine(item.EntryNumber);  
                } 
                string offset = String.Empty;
                var matches = transactions.Where(x => x.Ref_Customer == item.Ref_Customer && x.Ref_Reference == item.Ref_Reference).ToList();
                if (matches.Count() > 1)
                {
                    // Start matchin dollar amounts
                    var diff = matches.Sum(x => x.Debit) - matches.Sum(x => x.Credit);

                    if (diff == 0)
                    {
                        foreach (var match in matches)
                        {
                            if (match.Account != SearchAccount) match.Searched = true;
                            match.Matched = true;
                            if (match.EntryNumber != item.EntryNumber)
                                offset += String.Format("{0} {1} {2} {3} {4} |", match.Account, match.CallNumber, match.Reference, match.Debit, match.Credit);
                            if (match.Account == null) Console.Write("");
                        }
                    }
                    else
                    {
                        foreach (var match in matches)
                        {
                            match.Searched = true;
                        }
                    }
                }

                item.Searched = true;
                item.MatchedTransactions = offset;
                if (String.IsNullOrEmpty(offset) || offset == "" || offset == null)
                {
                    Console.Write(String.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12}{13}",
                    item.TranDate, item.TranType, item.Posted, item.CallNumber, item.PartNumber,
                    item.Debit, item.Credit, item.Reference, item.Ref_Customer, item.Ref_Reference,
                    item.Searched, item.Matched, item.MatchedTransactions, Environment.NewLine));
                }

                trancount = transactions.Count(x => x.Searched != true && x.Account == SearchAccount);
            }

            UnMatchedParts.Clear();

            foreach (var item in transactions.Where(x => x.Account == SearchAccount))
            {
                var partMatches = transactions.Where(x => x.Account == SearchAccount && x.PartNumber == item.PartNumber && x.Ref_Customer == item.Ref_Customer);
                if (partMatches.Sum(x => x.Debit) - partMatches.Sum(x => x.Credit) != 0)
                {
                    string match = String.Format("{0}|{1}", item.PartNumber, item.Ref_Customer);

                    if (!UnMatchedPartsLog.Contains(match))
                    {
                        Console.WriteLine(match);
                        UnMatchedPartsLog.Add(match);
                        UnMatchedParts.Add(new PartHolder
                        {
                            Part = item.PartNumber,
                            Customer = item.Ref_Customer,
                            Variance = partMatches.Sum(x => x.Debit) - partMatches.Sum(x => x.Credit)
                        });
                    }
                }
            }

            Console.WriteLine("Lines Parsed: {0}", transactions.Count());
            Console.WriteLine("Lines Searched: {0}", transactions.Count(x => x.Searched == true));
            Console.WriteLine("Lines Matched: {0}", transactions.Count(x => x.Matched == true));
            Console.WriteLine("Lines Un-Matched: {0}", transactions.Count(x => x.Matched != true));
            Console.WriteLine("Sum Matched: Debits: {0}  Credits: {1}", transactions.Where(x => x.Matched == true).Sum(x => x.Debit), transactions.Where(x => x.Matched == true).Sum(x => x.Credit));
            Console.WriteLine("{2} Totals: Debits: {0}  Credits: {1}", transactions.Where(x => x.Account == SearchAccount).Sum(x => x.Debit), transactions.Where(x => x.Account == SearchAccount).Sum(x => x.Credit), SearchAccount);

            var csv = new StringBuilder();
            // foreach (var item in transactions.Where(x => x.Account == SearchAccount && String.IsNullOrEmpty(x.MatchedTransactions)))
            foreach (var item in transactions.Where(x => x.Account == SearchAccount))
            {
                csv.AppendFormat("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13}{14}", item.Account,
                    item.TranDate, item.TranType, item.Posted, item.CallNumber, item.PartNumber,
                    item.Debit, item.Credit, item.Reference, item.Ref_Customer, item.Ref_Reference,
                    item.Searched, item.Matched, item.MatchedTransactions, Environment.NewLine);
            }
            WriteResults(outFile, csv);

            var suspectParts = new StringBuilder();
            foreach (var item in UnMatchedParts.OrderBy(x => x.Part))
            {
                suspectParts.AppendLine(String.Format("{0}, {1}, {2}", item.Part, item.Customer, item.Variance));
            }
            WriteResults(suspectFile, suspectParts);
        }

        private static void WriteResults(string outFile, StringBuilder csv)
        {
            File.WriteAllText(outFile, csv.ToString());
        }
    }
}