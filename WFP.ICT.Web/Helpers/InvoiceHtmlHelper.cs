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
         public static string GenerateInvoiceHtml(PianoOrder order , string _directoryPath)
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
            order.PianoAssignment.Drivers.ToList().ForEach(x =>
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
                                   ", x.PianoCharges.ChargesDetails, x.Amount);
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


    }
}
