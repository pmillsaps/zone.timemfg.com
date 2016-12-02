using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Time.Data.EntityModels.Install;
using Time.Install.Models;

namespace Time.Install.Business_Logic
{
    public class ParsingQuoteComments
    {
        // This class takes the comments from a lift quote and parses and 
        // returns a list of the Part Numbers (Options)   
        public static List<AerialOption> GetAerialOptions(string comment, int liftFamilyId, VSWQuotesEntities dbQ)
        {
            string[] quoteComment = comment.Split(new String[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            var aerialOptions = new List<AerialOption>();
            string line = "";
            int counter = 0;
            bool endOfAssMatl = false;
            string partNum = "";

            for (int i = 0; i < quoteComment.Length; i++)
            {
                line = quoteComment[i];
                if (line.Contains("Assemblies:"))
                {
                    endOfAssMatl = true;
                    counter = 1;
                }
                if (line.Contains("PAGE 1 SELECTIONS:"))
                {
                    endOfAssMatl = false;
                    break;
                }
                if (endOfAssMatl && counter == 3)
                {
                    AerialOption aQ = new AerialOption();

                    aQ.QtyPartAndDescription = line.Trim();
                    aQ.Hours = 0;
                    if (!String.IsNullOrEmpty(aQ.QtyPartAndDescription))
                    {
                        if ((!aQ.QtyPartAndDescription.Contains("Materials:")))
                        {
                            if (aQ.QtyPartAndDescription.Contains("<"))
                            {
                                // Replacing the angular brackets in the "<Error> Part Not Found" description to prevent an exception in MVC
                                aQ.QtyPartAndDescription = aQ.QtyPartAndDescription.Replace("<", "");
                                aQ.QtyPartAndDescription = aQ.QtyPartAndDescription.Replace(">", "!");
                            }
                            // Extracting the part number from the string to assign the labor hours
                            //int index = aQ.QtyPartAndDescription.IndexOf('-') - 3;
                            int index = aQ.QtyPartAndDescription.IndexOf('.') + 8;
                            if (index > 0)
                            {
                                partNum = aQ.QtyPartAndDescription.Substring(index, 17);
                                //partNum = aQ.QtyPartAndDescription.Substring(index, 11);
                                partNum = RemoveWhitespace(partNum);
                                // Assigns the labor hours to the Time options
                                var timeHours = dbQ.TimeOptions.Where(x => x.LiftFamilyId == liftFamilyId);
                                foreach (var item in timeHours)
                                {
                                    if (partNum == item.Option)
                                    {
                                        aQ.Hours = item.InstallHours;
                                        break;
                                    }
                                }
                            }
                            aerialOptions.Add(aQ);
                        }
                    }
                    counter = 2;
                }
                counter++;
                line = "";
            }
            return aerialOptions;
        }

        // Removing white spaces on the Part Number while processing the Line Comments from Epicor
        private static string RemoveWhitespace(string input)
        {
            return new string(input.ToCharArray()
                .Where(c => !Char.IsWhiteSpace(c))
                .ToArray());
        }
    }
}