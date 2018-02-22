using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WFP.ICT.Web.Models
{
    public class NewPaymentVm
    {
        public Guid? Id { get; set; }

        [Required(ErrorMessage = "Client is required")]
        public Guid? ClientId { get; set; }

        public string OrderNumber { get; set; }
        public string InvoiceNumber { get; set; }

        [Required(ErrorMessage = "Please select payment type")]
        public int PaymentType { get; set; }

        // Check
        public string CheckNumber { get; set; }
        public string CheckAmount { get; set; }
        public string CheckDate { get; set; }
        public int CheckStatus { get; set; }
        public string CheckClearanceDate { get; set; }

        // Credit Card
        public string CardHolderName { get; set; }
        public string CardNumber { get; set; }
        public string CardExpiryDate { get; set; }
        public int CardCVC { get; set; }
        public string TransactionNumber { get; set; }
        public string TransactionDate { get; set; }
        public bool IsPaymentRecurring { get; set; }

        public NewPaymentVm()
        {
        }
    }

}