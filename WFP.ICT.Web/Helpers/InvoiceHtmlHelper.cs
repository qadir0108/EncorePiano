using NSERReceipts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WFP.ICT.Data.Entities;

namespace WFP.ICT.Common
{
    public class InvoiceHtmlHelper
    {
         public static string GenerateInvoiceHtml(Order order , string _directoryPath)
        {
            StringBuilder html = new StringBuilder();

            string imgPath = ZingCoder.GenerateForInvoice(_directoryPath, order.OrderNumber);

            html.Append(@"<!DOCTYPE html><html><head><style>
                                    table {table-layout:fixed; font-family: arial,text-align:
                                           left; sans-serif; padding: 5px; border-collapse:collapse; width:100%; }
                                  table td{padding:3px;}
                                    table.bordered {border: 1px solid #dddddd;} 
                                    table.bordered td{border: 1px solid #dddddd;}

                                    table span.bold{font-weight:600;} 
                                    table td.right{ text-align:right;}
                                    table tr td.heading-cmp{font-weight:600; font-size:28px;}

                                    </style>
                                    </head>
                                    <body>");

            html.AppendFormat(@"<table><tr>
                                    <td colspan='3' class='heading-cmp'>Encore Piano & <br /> Organ Moving LLC</td>
                                    <td colspan='2'>Invoice # {0} <br />
                                    Pickup Date {1} <br />
                                    Delivery Date {2}<br />  </td>
                                    <td class='right'><img src='{3}' /></td>
                                    </tr>", order.OrderNumber,
                                order.PickupDate.Value.ToString("yyyy-MM-dd"),
                                order.DeliveryDate.Value.ToString("yyyy-MM-dd"),
                                imgPath);

            html.AppendFormat(@"<tr>
                                    <td colspan='2'>
                                   <span class='bold'> 15915 CANARY AVE </span> <br />
                                   <span class='bold'> LA MIRADA, CA 90638 </span><br />
                                    <span class='bold'>MOVEMYPIANO.COM </span> <br /> 
                                    </td>
                                    <td colspan='2'>
                                    <span class='bold'> PH: 1-888-336-2673 </span><br />
                                    <span class='bold'> FX: 714-670-1214</span> <br />
                                    <span class='bold'> CA 367167 </span><br />  
                                     </td>
                                     <td>
                                     </td>
                                     <td>
                                     </td>
                                    </tr></table>");

            html.AppendFormat(@"<table class='bordered'><tr>
                                    <td>Stop</td>
                                     <td>Time</td>
                                     <td>Grid</td>
                                     <td>Stop</td>
                                     <td>Time</td>
                                     <td>Grid</td>
                                    </tr>");


            html.AppendFormat(@" <tr>
                                    <td colspan='3'>
                                    <span class='bold'>Shipper</span> <br />
                                    {0} <br />
                                    {1} <br />
                                    {2} <br /> 
                                    </td>
                                    <td colspan='3'>
                                      <span class='bold'>Consignee</span> <br />
                                    {3} <br />
                                    {4} <br />
                                    {5} <br />  
                                    </td>
                                    </tr>",
                                order.PickupAddress.Name, order.PickupAddress.City, order.PickupAddress.State,
                                order.PickupAddress.Name, order.PickupAddress.City, order.PickupAddress.State);
            //Move Notes
            html.AppendFormat(@"<tr>
                                    <td colspan='3'>
                                    <span class='bold'>Move Notes</span> <br />
                                    {0}
                                    </td>
                                    <td colspan='3'>
                                     <span class='bold'>Move notes</span> <br />
                                    {1}
                                    </td>
                                    </tr>
                                    <tr>
                                    <td colspan='6'>
                                    <span class='bold'>UNITS : </span> <br />
                                    </td>
                                    </tr>
                                    </table>",
                                order.PickupAddress.Notes,
                                order.DeliveryAddress.Notes);
            //Piano Units
            foreach (var item in order.Pianos)
            {

                html.AppendFormat(@"<table style='float:left; width: 80%; ' class='bordered'>
                                    <tr>
                                    <td>
                                    <span class='bold'>SERIAL</span> : {0} &nbsp &nbsp <span class='bold'>TYPE</span> : {1}  &nbsp &nbsp <span class='bold'>MAKE</span> : {2} &nbsp &nbsp <span class='bold'>MODEL</span> : {3} &nbsp &nbsp <span class='bold'>SIZE</span> : {4} <br /> <br />
                                    <span class='bold'>BENCH</span> : {5} &nbsp &nbsp <span class='bold'>PLAYER</span> : {6} &nbsp &nbsp <span class='bold'>BOX</span> : {7} <br /> <br />
                                     <span class='bold'>MISCELLANEOUS</span> : {8} 
                                    </td>
                                    </tr>
                                    <tr>
                                    </tr>
                                    </table>", item.SerialNumber, item.PianoType == null ? "" : item.PianoType.Type, item.PianoMake == null ? "" : item.PianoMake.Name, item.Model == null ? "" : item.Model, item.PianoSize == null ? "" : PianoSizeConversion.GetFeetInches(item.PianoSize.Width),
                                item.IsBench ? "Yes" : "No", item.IsBoxed ? "Yes" : "No", item.IsPlayer ? "Yes" : "No",
                                item.Notes);
            }

            //Drivers 

            html.AppendFormat(@"<table style = 'width:20%;' class='bordered'>");
            var index = 1;
            if(order.Assignments.Count > 0)
            {
                var drivers = order.Assignments.FirstOrDefault().Drivers.ToList();
                drivers.ForEach(x =>
                {
                    html.AppendFormat(@"<tr>
                                     <td style= 'width:20%' >
                                     {0}
                                    </td>
                                    <td>
                                     {1}
                                    </td>
                                    </tr>
                                   ", index, x.Name);
                    index++;

                });
            }
            html.AppendFormat(@"</table>");


            html.AppendFormat(@"<table style='float:left; width: 50%; ' class='bordered'>
                                    <tr>
                                    <td>
                                    <span style='boder-bottom:1px solid #dddddd;' class='bold'>PAYMENTS</span> <br /> <br />
                                   <span class='bold'>COLLECTABLE</span> : {0}  <br /> <br />
                                   <span class='bold'>OFFICE STAFF</span> : {1}  <br /> <br />
                                   <span class='bold'>PAYMENT DETAIL</span> : {2}  <br /> <br />
                                    </td>
                                    </tr>
                                    </table>", order.CodAmount, order.OfficeStaff, order.OnlinePayment);

            html.AppendFormat(@"<table style = 'width:50%;' class='bordered'>
                                    <tr>
                                    <td colspan='2'>
                                      <span class='bold'>ADDITIONAL CHARGES</span>
                                    </td>
                                    </tr>");
            long SumAmount = 0;
            order.OrderCharges.ToList().ForEach(x =>
            {
                html.AppendFormat(@"<tr>
                                    <td>
                                     {0}
                                    </td>
                                    <td>
                                     {1}
                                    </td>
                                    </tr>
                                   ", x.PianoCharges.Details, x.Amount);
                SumAmount += x.Amount;
            });



            html.AppendFormat(@"    <tr>
                                    <td colspan='2'>
                                      <span class='bold'>DISCOUNT</span> <br />
                                    </td>
                                    </tr>
                                    <tr>
                                    <td colspan='2'>
                                      <span class='bold'>TOTAL</span> <br />
                                    {0}
                                    </td>
                                    </tr>

                                    </table>", SumAmount);


            html.AppendFormat(@"<table >
                                    <tr>
                                    <td colspan='4'>
                                    <br />
                                    The rates apply to the services performed under this cartage agreement are based on the following limits of laibiltiy  <br />    
                                    1.The shipper agreed or declared value of the shipment herunder is stated by the shipper to be not exceeding $0.60 per lb per article <br />
                                    2.The carrier is not responsible for the internal working parts of the merhandise covered by this agreement <br />
                                    3.The carrier is not responsible for damage to sidewalks, driveways, stairways or walls  <br />
                                    4.The carrier is not responsible for tile, linoleum, carpets, floors or floor covering <br />
                                    5.The shipper by signing this document hereby agrees to indemnify and hold carrier harmless as a result of any damage and indouminitive above <br />
                                    </td>
                                    </tr>
                                    <tr>
                                    <td colspan='2'>
                                     <br />
                                     <br />
                                     ___________________________________________ <br />
                                     Shipper or agent signature
                                    </td>
                                      <td colspan='2'>
                                     RECIEVED BY CONSIGNEE IN GOOD CONDITIONEXCEPT AS NOTED AND FURTHER CLAIMS HEREBY WAIVED<br />
                                    <br />
                                     ___________________________________________ <br />
                                     Consignee or agent signature
                                    </td>
                                    </tr>

                                    </table>");


                                 html.Append(@"</body></html>");

            return html.ToString();

        }

        public static string GenerateClientInvoiceHtml(IEnumerable<Order> Orders, Client client, List<Piano> Pianos, int invoiceNumber, out long totalAmount)
        {
            StringBuilder html = new StringBuilder();


            html.Append(@"<!DOCTYPE html><html><head><style>
                                    body{border:solid 1px #efefef;}
                                    table {color:#2C3E50;
                                    table-layout:fixed; font-family: arial,text-
                                    align:left; sans-serif; padding: 5px; border-
                                    collapse:collapse; width:100%; border-spacing:0px }
                                                                       
                                 	table td{padding:3px;}

                                    table.striped tr:nth-child(odd) {background-color: #f0f0f0 } 
                                    table.striped tr:nth-child(even) {background-color: #f7f7f7}
                                    table tr.header-row{background-color:#8cb1d6  !important; color :#ffffff;} 
                                    table.bordered {border: 1px solid #dddddd;}
                                    table.bordered td{border: 1px solid #dddddd;}
									table td.right{ text-align:right;}
                                    table span.bold{font-weight:600;} 
                                    table span.blue-head{font-weight:600;
                                    color:#5E738B;
                                    font-size:28px;
                                    } 
                                     table span.under{text-decoration:underline;}
                                     table span.bill{font-size:20px;}
                                    table span.heading-cmp{
                                    font-weight:600; 
                                    font-size:28px;}
                                    </style>
                                    </head>
                                    <body>");

            html.AppendFormat(@"<table><tr>
                                    <td colspan='3'><span class='heading-cmp'>{0} </span> <br /><span class='bold'>{1} <br />{2}  <br />{3} <br /><span/></td>
                                     <td colspan='1'>
                                    <span class='blue-head under'>Invoice</span> <br />
                                    <span class='bold'>Date : {4} <br />
                                    Invoice # {5} <br />
                                    Customer Id : {6}</span><br />  </td>
                                    </tr>   ", client.Name, client.Address.Address1, client.Address.City, client.Address.State, DateTime.Now.ToString("yyyy-MM-dd"), invoiceNumber,
                                    client.AccountCode);

            html.Append(@"</table>");

            html.AppendFormat(@"<table><tr>
                                    <td><span class='bold under bill'>Bill To</span><br />
                                    {0}<br />
                                    {1}<br />
                                    {2}<br />
                                    {3}<br />
                                    </td>
                                    </tr>", DateTime.Now.ToString("yyyy-MM-dd"), "2211", client.AccountCode,
                        client.Name, client.CompanyLogo);
            html.AppendFormat(@"</table>");
            html.AppendFormat(@"<table class='striped'><tr class='header-row'>
                                    <td style ='width:70%'><span class=bold'>Description</span><br />
                                   
                                    </td>
                                    <td style ='width:15%'><span class='bold'>Taxed</span><br />
                                   
                                    </td>
                                    <td style ='width:15%'><span class='bold'>Amount</span><br />
                                   
                                    </td>
                                    </tr>");

            totalAmount = 0;

            foreach (var item in Orders)
            {
                var amount = item.OrderCharges.Select(x => x.Amount).Sum();

                totalAmount += amount;

                html.AppendFormat(@" <tr>
                                    <td><span class='bold'>Units </span>{0}
                                      
                                        <span class='bold'>Order Number </span>{2} </ br>
                                        <span class='bold'>Date Ordered </span>{1}
                                    </td>
                                    <td><span class='bold'>{3}</span>
                                   
                                    </td>
                                    <td><span class='bold'>{4}</span>
                                   
                                    </td>
                                    </tr>   ", item.Pianos.Count, item.CreatedAt, item.OrderNumber,
                                                "No amount", "$ " + amount);
            }
            html.AppendFormat(@"</table>");

            html.AppendFormat(@"<table style='width: 30%; margin-left:70%;'><tr>
                                    <td><span class='bold'>Subtotal</span><br />
                                    </td>
                                    <td><span class='bold'>{0}</span><br />
                                    </td>
                                    </tr>
                                    <tr>
                                    <td><span class='bold'>Taxable</span><br />
                                    </td>
                                    <td><span class='bold'>{1}</span><br />
                                    </td>
                                    </tr>
                                    <tr>
                                    <td><span class='bold'>Tax Rate</span><br />
                                    </td>
                                    <td><span class='bold'>{2}</span><br />
                                    </td>
                                    </tr>
                                    <tr>
                                    <td><span class='bold'>Tax due</span><br />
                                    </td>
                                    <td><span class='bold'>{3}</span><br />
                                    </td>
                                    </tr>
                                    <tr>
                                    <td><span class='bold'>Other</span><br />
                                    </td>
                                    <td><span class='bold'>{4}</span><br />
                                    </td>
                                    </tr>
                                    <tr>
                                    <td style='border-top:double black;'><span class='bold'>Total</span><br />
                                    </td>
                                    <td style='border-top: double black'><span class='bold'>{5}</span><br />
                                    </td>
                                    </tr>
                                    <tr style='padding-top:10px;'>
                                    <td colspan='2'>Make all check payable to Encore Piano Limited<br />
                                    </td>
                                    </table>", totalAmount, 0, 0, 0, 0, totalAmount);

            html.AppendFormat(@"</table>");

            html.AppendFormat(@"<table style='width: 70%; margin-top:15px;' class='bordered striped'><tr class='header-row'>
                                    <td><span class='bold'>Other Notes</span><br />
                                    </tr>
                                    <tr>
                                    <td>1 - Total payment due in 30 days <br />
                                    </td></tr>
                                    <tr>
                                    <td>2 - Please include the invoice number on your check <br />
                                    </td></tr>");

            html.AppendFormat(@"</table>");



            html.AppendFormat(@"<table><tr>
                                    <td style='text-align:center;'><span class='bold'>Thank you for your business!</span><br />
                                    </td></tr>");

            html.AppendFormat(@"</table>");


            html.Append(@"</body></html>");

            return html.ToString();

        }

    }
}
