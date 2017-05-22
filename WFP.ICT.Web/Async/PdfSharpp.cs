using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using System.Xml.XPath;
using MigraDoc.Rendering;
using PdfSharp.Pdf.Security;
using WFP.ICT.Web.Models;

namespace NSERReceipts
{
    public class PdfSharpp
    {
        Document document;
        Color TableBorder = Color.Parse("0x7C7C7C");
        Color TableBlue = Color.Parse("0xADD8E6");
        Color TableGray = Color.Parse("0xB2C2C7");
        private string _directoryPath;
        string imgBISP = @"encore_logo.png";
        string imageMessage = @"message.png";

        public string Generate(string ImagesPath, string directoryPath, List<OrderVm> orderVms)
        {
            imgBISP = ImagesPath + @"\\encore_logo.png";
            imageMessage = ImagesPath + @"\\message.png";
            _directoryPath = directoryPath;

            // Create a new MigraDoc document
            document = new Document();
            //document.Info.Title = title;
            //document.Info.Subject = title;
            document.Info.Author = "ICT Team";

            DefineStyles();

            Section section = this.document.AddSection();

            foreach (var orderVm in orderVms)
            {
                CreateTable(section, orderVm.PickupTicket, orderVm.OrderNumber, orderVm.PickupAddressString, orderVm.DeliveryAddressString);
            }

            // Footer
            //DateTime time = DateTime.Now;
            //string format = "ddd MMM d, yyyy HH:mm:ss";
            //string footer1 = "This is system generated report and doesn't need verfication.\t" + "Generated at :" + time.ToString(format);
            //Paragraph paragraph = section.Footers.Primary.AddParagraph();
            //paragraph.AddText(footer1);
            //paragraph.Format.Font.Name = "Times New Roman";
            //paragraph.Format.Font.Size = 9;
            //paragraph.Format.Font.Italic = true;
            //paragraph.Format.Alignment = ParagraphAlignment.Center;

            string filePath = _directoryPath + "\\tokens.pdf";

            PdfDocumentRenderer renderer = new PdfDocumentRenderer(true, PdfFontEmbedding.Always);
            renderer.Document = document;
            renderer.RenderDocument();
            renderer.PdfDocument.Save(filePath);

            return filePath;
        }
        void DefineStyles()
        {
            // Get the predefined style Normal.
            Style style = document.Styles["Normal"];
            // Because all styles are derived from Normal, the next line changes the
            // font of the whole document. Or, more exactly, it changes the font of
            // all styles and paragraphs that do not redefine the font.
            style.Font.Name = "Verdana";
            style = this.document.Styles[StyleNames.Header];
            style.ParagraphFormat.AddTabStop("16cm", TabAlignment.Right);
            style = this.document.Styles[StyleNames.Footer];
            style.ParagraphFormat.AddTabStop("8cm", TabAlignment.Center);
            // Create a new style called Table based on style Normal
            style = this.document.Styles.AddStyle("Table", "Normal");
            style.Font.Name = "Verdana";
            //style.Font.Name = "Times New Roman";
            style.Font.Size = 9;
            // Create a new style called Reference based on style Normal
            style = this.document.Styles.AddStyle("Reference", "Normal");
            style.ParagraphFormat.SpaceBefore = "5mm";
            style.ParagraphFormat.SpaceAfter = "5mm";
            style.ParagraphFormat.TabStops.AddTabStop("16cm", TabAlignment.Right);

            var hr = document.AddStyle("HorizontalRule", "Normal");
            var hrBorder = new Border();
            hrBorder.Width = "1pt";
            hrBorder.Color = Colors.DarkGray;
            hr.ParagraphFormat.Borders.Bottom = hrBorder;
            hr.ParagraphFormat.LineSpacing = 0;
            hr.ParagraphFormat.SpaceBefore = 15;
        }

        public Table CreateTable(Section section, string code, string orderNumber, string pickupAddress, string deliveryAddress)
        {
            Table outer = section.AddTable();
            outer.Format.LeftIndent = 0.1;                 
            var c = outer.AddColumn("18cm");
            var r = outer.AddRow();
            outer.Borders.Visible = true;
            outer.Borders.Width = 2;

            Table table1 = new Table(); // section.AddTable();
            //table1.Style = "Table";
            //table1.Borders.Color = TableBorder;

            //table1.Borders.Visible = true;
            //table1.Borders.Width = 2;
            table1.Borders.Left.Width = 0;
            table1.Borders.Right.Width = 0;
            table1.Borders.Top.Width = 0;
            table1.Borders.Bottom.Width = 0;

            // Before you can add a row, you must define the columns
            Column column = table1.AddColumn("5cm");
            column.Format.Alignment = ParagraphAlignment.Left;
            column = table1.AddColumn("7cm");
            column.Format.Alignment = ParagraphAlignment.Center;
            column = table1.AddColumn("5cm");
            column.Format.Alignment = ParagraphAlignment.Right;

            // Create the header of the table
            Row row = table1.AddRow();
            row.Borders.Visible = false;
            row.HeadingFormat = true;
            //row.Format.Alignment = ParagraphAlignment.Left;
            row.VerticalAlignment = VerticalAlignment.Center;
            row.Format.Font.Bold = true;
            //row.Shading.Color = TableBlue;
            row.Cells[0].Format.Alignment = ParagraphAlignment.Center;
            row.Cells[0].VerticalAlignment = VerticalAlignment.Center;
            row.Cells[0].AddParagraph().AddImage(imgBISP);

            //Paragraph paragraph = row.Cells[0].AddParagraph();
            //paragraph.AddText("3630446517241");
            //paragraph.Format.Font.Name = "Comic Sans MS";
            //paragraph.Format.Font.Size = 12;
            //paragraph.Format.Font.Italic = true;
            //paragraph.Format.Font.Bold = true;
            
            var filePath = _directoryPath+ "\\" + code + ".png";
            row.Cells[1].AddParagraph().AddImage(filePath);
            row.Cells[1].Format.Alignment = ParagraphAlignment.Center;
            row.Cells[1].VerticalAlignment = VerticalAlignment.Center;

            //row.Cells[2].AddParagraph().AddImage(imgNSER);
            //row.Cells[2].Format.Alignment = ParagraphAlignment.Center;
            //row.Cells[2].VerticalAlignment = VerticalAlignment.Center;
            //table1.SetEdge(0, 0, 3, 1, Edge.Box, BorderStyle.Single, 0.75, TableBorder);

            Paragraph paragraph;
            paragraph = row.Cells[2].AddParagraph();
            paragraph.AddText("Order Number: " + orderNumber);
            paragraph.Format.Font.Name = "Times New Roman";//"Comic Sans MS";
            paragraph.Format.Font.Size = 11;
            //paragraph.Format.Font.Italic = true;
            //paragraph.Format.Font.Bold = true;

            paragraph = row.Cells[2].AddParagraph();
            pickupAddress = pickupAddress.Replace("<br />", "\n");
            paragraph.AddText("Pickup Address: " + pickupAddress);
            paragraph.Format.Font.Name = "Times New Roman";//"Comic Sans MS";
            paragraph.Format.Font.Size = 11;
            //paragraph.Format.Font.Italic = true;
            //paragraph.Format.Font.Bold = true;

            paragraph = row.Cells[2].AddParagraph();
            deliveryAddress = deliveryAddress.Replace("<br />", "\n");
            paragraph.AddText("Delivery Address: " + deliveryAddress);
            paragraph.Format.Font.Name = "Times New Roman";//"Comic Sans MS";
            paragraph.Format.Font.Size = 11;
            //paragraph.Format.Font.Italic = true;
            //paragraph.Format.Font.Bold = true;

            row.Cells[2].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[2].VerticalAlignment = VerticalAlignment.Center;
            table1.SetEdge(0, 0, 3, 1, Edge.Box, BorderStyle.Single, 0.75, TableBorder);

            Row row2 = table1.AddRow();
            row.Borders.Visible = false;
            row.HeadingFormat = true;
            row2.Format.Alignment = ParagraphAlignment.Left;
            row2.VerticalAlignment = VerticalAlignment.Center;
            row2.Format.Font.Bold = true;
            //row2.Shading.Color = TableBlue;
            //row2.Cells[0].AddParagraph().AddImage(imageMessage);
            row2.Cells[0].MergeRight = 2;
            table1.SetEdge(0, 0, 3, 1, Edge.Box, BorderStyle.Single, 0.75, TableBorder);

            section.AddParagraph("");
            section.AddParagraph("");

            Cell cellC = r.Cells[0];
            cellC.Elements.Add(table1);

            return table1;
        }
    }
}