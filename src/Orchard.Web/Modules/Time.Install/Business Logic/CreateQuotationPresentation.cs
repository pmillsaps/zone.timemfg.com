using Novacode;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Time.Data.EntityModels.Install;
using Time.Install.Models;

namespace Time.Install.Business_Logic
{
    public class CreateQuotationPresentation
    {
        
        public static MemoryStream CreateDocument(int? userId, int liftFamilyId, string listOfOptions, VSWQuotesEntities db)
        {
            // Declaring all the variables needed and retrieving the data to create the document            
            QuotationPresentationModel model = new QuotationPresentationModel();// Loading the chassis specs and install details
            model.LiftDescription = db.LiftFamilies.Where(x => x.Id == liftFamilyId ).Select(x => x.Description).SingleOrDefault();
            model.ChassisSpecs = db.ChassisSpecsForWordDocs.FirstOrDefault(x => x.LiftFamilyId == liftFamilyId);
            model.InstallDetails = db.InstallDetailsForWordDocs.ToList();
            if (userId != null)//Loading the user
            {
                model.QuoteUser = db.QuoteDeptUsersForWordDocs.FirstOrDefault(x => x.Id == userId);
            }
            string[] titles = listOfOptions.Split(',');// Loading the options titles and descriptions
            model.TitlesAndDesc.Titles = titles;
            model.TitlesAndDesc.TitleDesc = new string[titles.Length];
            var ttlsDesc = db.OptionTitleDescpForWordDocs.Where(x => x.LiftFamilyId == liftFamilyId).AsQueryable();
            foreach (var item in ttlsDesc)
            {
                for (int i = 0; i < model.TitlesAndDesc.Titles.Length; i++)
                {
                    if (model.TitlesAndDesc.Titles[i] == item.OptionTitlesForWordDoc.OptionTitle)
                    {
                        model.TitlesAndDesc.TitleDesc[i] =  item.Description.Trim();
                        break;
                    }
                }
            }

            MemoryStream stream = new MemoryStream();
            // Create a document.
            using (DocX document = DocX.Create(stream))
            {
                System.Drawing.FontFamily fontF = new System.Drawing.FontFamily("Arial");
                double fontSize = 10;
                DateTime todayDate1 = DateTime.Now;
                string todayDate2 = todayDate1.ToString("MMMM dd, yyyy");
                string url = HttpContext.Current.Server.MapPath("~/Modules/Time.Install/Content/images/");


                // Setting the margins. 
                document.MarginLeft = 25F;
                document.MarginRight = 25F;
                document.MarginTop = 25F;

                // Adding a footer with the page number and date
                document.AddFooters();
                document.DifferentFirstPage = false;
                document.DifferentOddAndEvenPages = false;
                var footerP = document.Footers.odd.Paragraphs.First();
                footerP.Alignment = Alignment.left;
                footerP.Append("Page ");
                footerP.AppendPageNumber(PageNumberFormat.normal);
                footerP.Append(" of ");
                footerP.AppendPageCount(PageNumberFormat.normal);
                footerP.Append("\t\t\t\t\t\t\t\t\t\t\t" + todayDate1.ToShortDateString());

                // Insert TIME and VSW logos.
                Paragraph logos = document.InsertParagraph();
                Novacode.Image image1 = document.AddImage(url + "TimeLogoSmall.jpg");
                Novacode.Image image2 = document.AddImage(url + "VSWLogoSmall.jpg");
                Picture pic1 = image1.CreatePicture();
                Picture pic2 = image2.CreatePicture();
                logos.AppendPicture(pic1).Append("\t\t\t\t\t\t\t    ").AppendPicture(pic2).AppendLine();

                // Insert today's date and company address
                Paragraph dateAddress = document.InsertParagraph();
                dateAddress.Append("\t\t\t\t\t\t\t\t\t\t\t ").Append("7601 Imperial Drive").Font(new System.Drawing.FontFamily("Arial")).FontSize(10);
                dateAddress.AppendLine("DATE:  " + todayDate2).Bold().Font(new System.Drawing.FontFamily("Arial")).FontSize(10).Append("\t\t\t\t\t\t\t          ").Append("P.O. Box 20368 - Waco, Texas 76702-0368").Font(new System.Drawing.FontFamily("Arial")).FontSize(10);
                dateAddress.AppendLine("\t\t\t\t\t\t\t\t\t\t       ").Append("Fax Number: (254) 339-2651").Font(new System.Drawing.FontFamily("Arial")).FontSize(10);
                dateAddress.AppendLine("TO:").Bold().Font(new System.Drawing.FontFamily("Arial")).FontSize(10);
                dateAddress.AppendLine();
                dateAddress.AppendLine();
                dateAddress.AppendLine();
                dateAddress.AppendLine();
                dateAddress.AppendLine();
                dateAddress.AppendLine();

                // Inserting the Subject line and a horizontal rule
                Paragraph subject = document.InsertParagraph();
                subject.Append("SUBJECT: Quote # -").Bold().Font(new System.Drawing.FontFamily("Arial")).FontSize(10);
                subject.AppendLine("__________________________________________________________________________________________").Bold();
                subject.AppendLine();

                // Inserting lift description
                Paragraph liftDscrptn = document.InsertParagraph();
                liftDscrptn.Append(model.LiftDescription).Font(new System.Drawing.FontFamily("Arial")).FontSize(10);
                liftDscrptn.AppendLine();

                // Inserting the Aerial Lift Specifications header
                Paragraph header1 = document.InsertParagraph();
                header1.Alignment = Alignment.center;
                header1.Append("AERIAL LIFT SPECIFICATIONS").Bold().UnderlineStyle(UnderlineStyle.singleLine).Font(new System.Drawing.FontFamily("Arial")).FontSize(10).AppendLine();

                // Inserting the options titles and descriptions
                Paragraph liftOptions = document.InsertParagraph();
                for (int i = 0; i < model.TitlesAndDesc.Titles.Length; i++)
                {
                    liftOptions.Append(model.TitlesAndDesc.Titles[i] + " - ").Bold().Font(new System.Drawing.FontFamily("Arial")).FontSize(10).Append(model.TitlesAndDesc.TitleDesc[i]).Font(new System.Drawing.FontFamily("Arial")).FontSize(10);
                    liftOptions.AppendLine();
                    liftOptions.AppendLine();
                }

                // Inserting the Chassis Specifications header
                Paragraph header2 = document.InsertParagraph();
                header2.Alignment = Alignment.center;
                header2.AppendLine();
                header2.AppendLine("CHASSIS SPECIFICATIONS").Bold().UnderlineStyle(UnderlineStyle.singleLine).Font(new System.Drawing.FontFamily("Arial")).FontSize(10);
                header2.AppendLine();
                //header2.AppendLine("3").Script(Script.superscript);
                var in3 = "in\x00B3";
                // Chassis Specifications bulleted list
                var chassisBulletedList = document.AddList("Minimum Chassis Specifications:", 0, ListItemType.Bulleted);
                document.AddListItem(chassisBulletedList, "Cab to axle dimension………………………………………………………….….….." + model.ChassisSpecs.CbToAxDimension + "in.");
                document.AddListItem(chassisBulletedList, "Frame section modulus…………………………………………………………..…." + model.ChassisSpecs.FrmSctnModulus + in3);
                document.AddListItem(chassisBulletedList, "Frame resisting bending moment…………………………………………." + model.ChassisSpecs.FrmRsistngBndngMmnt + "in-lbs.");
                document.AddListItem(chassisBulletedList, "Front GAWR………………………………………………………………...……..." + model.ChassisSpecs.FrontGAWR + "lbs.");
                document.AddListItem(chassisBulletedList, "Rear GAWR…………………………………………………………………….…" + model.ChassisSpecs.RearGAWR + "lbs.");
                document.AddListItem(chassisBulletedList, "GVWR……………………………………………………………………………..." + model.ChassisSpecs.GVWR + "lbs.");
                document.AddListItem(chassisBulletedList, "Approx. curb weight for stability………………………………….………...…...." + model.ChassisSpecs.AprxCrbWghtStblty + "lbs.");
                document.InsertList(chassisBulletedList, fontF, fontSize);

                // Inserting the Body Specifications header
                Paragraph header3 = document.InsertParagraph();
                header3.Alignment = Alignment.center;
                header3.AppendLine();
                header3.Append("BODY SPECIFICATIONS").Bold().UnderlineStyle(UnderlineStyle.singleLine).Font(new System.Drawing.FontFamily("Arial")).FontSize(10);
                header3.AppendLine();
                header3.AppendLine();
                header3.AppendLine();
                header3.AppendLine();
                header3.AppendLine();
                header3.AppendLine();

                // Inserting the Installation Details header
                Paragraph header4 = document.InsertParagraph();
                header4.Alignment = Alignment.center;
                header4.Append("INSTALLATION DETAILS").Bold().UnderlineStyle(UnderlineStyle.singleLine).Font(new System.Drawing.FontFamily("Arial")).FontSize(10);
                header4.AppendLine();
                // Installation Details bulleted list
                var bulletedList1 = document.AddList(model.InstallDetails[0].DetailLine, 0, ListItemType.Bulleted);
                for (int i = 1; i < model.InstallDetails.Count; i++)
                {
                    document.AddListItem(bulletedList1, model.InstallDetails[i].DetailLine);
                }
                document.InsertList(bulletedList1, fontF, fontSize);

                // Inserting a page break
                document.InsertSectionPageBreak();
                // Inserting the Price Summary header
                Paragraph header5 = document.InsertParagraph();
                header5.Alignment = Alignment.center;
                header5.AppendLine();
                header5.AppendLine("PRICE SUMMARY").Bold().UnderlineStyle(UnderlineStyle.singleLine).Font(new System.Drawing.FontFamily("Arial")).FontSize(10);
                // Price Summary lines
                Paragraph pricePrgh = document.InsertParagraph();
                pricePrgh.AppendLine("Aerial, Body, Accessories and Installation:").Bold().Font(new System.Drawing.FontFamily("Arial")).FontSize(10).Append("\t\t\t\t\t\t$").Bold().Font(new System.Drawing.FontFamily("Arial")).FontSize(10);
                pricePrgh.AppendLine();
                pricePrgh.AppendLine("Chassis (year, make, model):").Bold().Font(new System.Drawing.FontFamily("Arial")).FontSize(10).Append("\t\t\t\t\t\t\t\t$").Bold().Font(new System.Drawing.FontFamily("Arial")).FontSize(10);
                pricePrgh.AppendLine();
                pricePrgh.AppendLine("SUBTOTAL:").Bold().Font(new System.Drawing.FontFamily("Arial")).FontSize(10).Append("\t\t\t\t\t\t\t\t\t\t$").Bold().Font(new System.Drawing.FontFamily("Arial")).FontSize(10);
                pricePrgh.AppendLine();
                pricePrgh.AppendLine();
                pricePrgh.AppendLine("NET PRICE FOB WACO, TEXAS:").Bold().Font(new System.Drawing.FontFamily("Arial")).FontSize(10).Append("\t\t\t\t\t\t\t$").Bold().Font(new System.Drawing.FontFamily("Arial")).FontSize(10);

                // Inserting the Options header
                Paragraph header6 = document.InsertParagraph();
                header6.Alignment = Alignment.center;
                header6.AppendLine();
                header6.AppendLine("OPTIONS").Bold().UnderlineStyle(UnderlineStyle.singleLine).Font(new System.Drawing.FontFamily("Arial")).FontSize(10);
                // Options lines
                Paragraph optionsPrgh = document.InsertParagraph();
                optionsPrgh.AppendLine("Option 1:").Bold().Font(new System.Drawing.FontFamily("Arial")).FontSize(10).Append("\tDelivery to customer location:").Font(new System.Drawing.FontFamily("Arial")).FontSize(10).Append("\t\t\t").Append("ADD TO NET PRICE:").Bold().UnderlineStyle(UnderlineStyle.singleLine).Font(new System.Drawing.FontFamily("Arial")).FontSize(10).Append("\t$").Bold().Font(new System.Drawing.FontFamily("Arial")).FontSize(10);
                optionsPrgh.AppendLine();
                optionsPrgh.AppendLine("__________________________________________________________________________________________").Bold();

                // Inserting the Terms header
                Paragraph header7 = document.InsertParagraph();
                header7.Alignment = Alignment.center;
                header7.AppendLine("TERMS:").Bold().UnderlineStyle(UnderlineStyle.singleLine).Font(new System.Drawing.FontFamily("Arial")).FontSize(10);
                header7.AppendLine();
                // Terms bulleted list
                var termsBulletedList = document.AddList("Your Terms This Order:                                                             Net 30 Days", 0, ListItemType.Numbered);
                document.AddListItem(termsBulletedList, "Days From Receipt of Order To Shipping:                                Approximately 120 – 150 Days.");
                document.AddListItem(termsBulletedList, "This Quotation Valid For:                                                          30 Days");
                document.AddListItem(termsBulletedList, "This quotation does not include any applicable sales tax, title, license or state inspection.");
                document.AddListItem(termsBulletedList, "Quoted chassis pricing is subject to change without notice from dealer.");
                document.InsertList(termsBulletedList, fontF, fontSize);

                // Inserting thank you line
                Paragraph thankPrgh = document.InsertParagraph();
                thankPrgh.AppendLine("__________________________________________________________________________________________").Bold();
                thankPrgh.AppendLine();
                thankPrgh.AppendLine();
                thankPrgh.AppendLine();
                thankPrgh.AppendLine();
                thankPrgh.AppendLine("Thank you for considering ").Font(new System.Drawing.FontFamily("Arial")).FontSize(10).Append("TIME Manufacturing ").Bold().Italic().Font(new System.Drawing.FontFamily("Arial")).FontSize(10).Append("to meet your utility equipment needs.  We look forward to earning your business.").Font(new System.Drawing.FontFamily("Arial")).FontSize(10);
                thankPrgh.AppendLine();
                thankPrgh.AppendLine("Sincerely,");
                thankPrgh.AppendLine();

                // Names and phone numbers
                Paragraph namesPrgh = document.InsertParagraph();
                if (model.QuoteUser != null)
                {
                    namesPrgh.Append(model.QuoteUser.Name).Font(new System.Drawing.FontFamily("Arial")).FontSize(10);
                    namesPrgh.AppendLine(model.QuoteUser.PositionTitle).Font(new System.Drawing.FontFamily("Arial")).FontSize(10);
                    namesPrgh.AppendLine("Phone: " + model.QuoteUser.Phone).Font(new System.Drawing.FontFamily("Arial")).FontSize(10);
                }
                else
                {
                    namesPrgh.Append("Your name").Font(new System.Drawing.FontFamily("Arial")).FontSize(10);
                    namesPrgh.AppendLine("Your position").Font(new System.Drawing.FontFamily("Arial")).FontSize(10);
                    namesPrgh.AppendLine("Your phone: (254) XXX-XXXX").Font(new System.Drawing.FontFamily("Arial")).FontSize(10);
                }
                namesPrgh.AppendLine();

                // Save this document.
                document.Save();
            }// Release this document from memory.
            return stream;
        }
    }
}