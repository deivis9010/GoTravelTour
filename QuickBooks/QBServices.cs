using Intuit.Ipp.Core;
using Intuit.Ipp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.QuickBooks
{
    public class QBServices
    {
        public void AddInvoice()
        {
           // Intuit.Ipp.Security.OAuthRequestValidator reqValidator = new Intuit.Ipp.Security.OAuthRequestValidator(accessToken, accessTokenSecret, consumerKey, consumerKeySecret);
           /* ServiceContext context = new ServiceContext(realmId, IntuitServicesType.QBO, reqValidator);

            Invoice added = Helper.Add(qboContextoAuth, CreateInvoice(qboContextoAuth));*/

        }

        internal Invoice CreateInvoice(ServiceContext context)
        {
            Customer customer = null;//FindorAdd(context, new Customer());
            /* TaxCode taxCode = FindorAdd(context, new TaxCode());
            Account account = FindOrADDAccount(context, AccountTypeEnum.AccountsReceivable, AccountClassificationEnum.Liability);*/

            Invoice invoice = new Invoice();
            invoice.Deposit = new Decimal(0.00);
            invoice.DepositSpecified = true;
            invoice.AllowIPNPayment = false;
            invoice.AllowIPNPaymentSpecified = true;

            invoice.CustomerRef = new ReferenceType()
            {
                name = customer.DisplayName,
                Value = customer.Id
            };
            invoice.DueDate = DateTime.UtcNow.Date;
            invoice.DueDateSpecified = true;
            invoice.GlobalTaxCalculation = GlobalTaxCalculationEnum.TaxExcluded;
            invoice.GlobalTaxCalculationSpecified = true;
            invoice.TotalAmt = new Decimal(0.00);
            invoice.TotalAmtSpecified = true;
            invoice.ApplyTaxAfterDiscount = false;
            invoice.ApplyTaxAfterDiscountSpecified = true;

            invoice.PrintStatus = Intuit.Ipp.Data.PrintStatusEnum.NotSet;
            invoice.PrintStatusSpecified = true;
            invoice.EmailStatus = EmailStatusEnum.NotSet;
            invoice.EmailStatusSpecified = true;
            invoice.ARAccountRef = new ReferenceType()
            {
                type = Enum.GetName(typeof(objectNameEnumType), objectNameEnumType.Account),
                name = "Account Receivable",
                Value = "QB:37"
            };
            invoice.Balance = new Decimal(0.00);
            invoice.BalanceSpecified = true;

            List<Line> lineList = new List<Line>();
            Line line = new Line();
            line.Description = "Description";
            line.Amount = new Decimal(100.00);
            line.AmountSpecified = true;

            line.DetailType = LineDetailTypeEnum.DescriptionOnly;
            line.DetailTypeSpecified = true;

            lineList.Add(line);
            invoice.Line = lineList.ToArray();
            Intuit.Ipp.Data.TxnTaxDetail txnTaxDetail = new TxnTaxDetail();
            txnTaxDetail.DefaultTaxCodeRef = new ReferenceType()
            {
                //Value = taxCode.Id,
                type = Enum.GetName(typeof(objectNameEnumType), objectNameEnumType.Customer),
                //name = taxCode.Name
            };

            txnTaxDetail.TotalTax = new Decimal(0.00);
            txnTaxDetail.TotalTaxSpecified = true;


            return invoice;
        }
    }
}
