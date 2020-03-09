using Intuit.Ipp.Core;
using Intuit.Ipp.Data;
using Intuit.Ipp.DataService;
using Intuit.Ipp.OAuth2PlatformClient;
using Intuit.Ipp.QueryFilter;
using Intuit.Ipp.Security;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GoTravelTour.QuickBooks
{
    public class QBServices
    {
              

        /*AGREGADO 29/02/20202*/
        // <summary>
        /// This routine creates an Invoice object
        /// </summary>
        private void CreateInvoice()
         {
              /// Step 1: Initialize OAuth2RequestValidator and ServiceContext
         /*     OAuth2RequestValidator ouathValidator = new OAuth2RequestValidator(
                  (string)Application["accessToken"]);
              ServiceContext serviceContext = new ServiceContext(
                  (string)Application["realmId"],
                  IntuitServicesType.QBO, ouathValidator);
              /// Step 2: Initialize an Invoice object
              Invoice invoice = new Invoice();
              invoice.Deposit = new Decimal(0.00);
              invoice.DepositSpecified = true;

              /// Step 3: Invoice is always created for a customer, so retrieve reference to a customer and set it in Invoice
              QueryService<Customer> querySvc =
                  new QueryService<Customer>(serviceContext);
              Customer customer = querySvc.ExecuteIdsQuery
                  ("SELECT * FROM Customer WHERE CompanyName like 'Amy%'").
                  FirstOrDefault();
              invoice.CustomerRef = new ReferenceType()
              {
                  Value = customer.Id
              };

              /// Step 4: Invoice is always created for an item so the retrieve reference to an item and create a Line item to the invoice
              QueryService<Item> querySvcItem =
                  new QueryService<Item>(serviceContext);
              Item item = querySvcItem.ExecuteIdsQuery(  "SELECT * FROM Item WHERE Name = 'Lighting'").FirstOrDefault();
              List<Line> lineList = new List<Line>();
              Line line = new Line();
              line.Description = "Description";
              line.Amount = new Decimal(100.00);
              line.AmountSpecified = true;
              lineList.Add(line);
              invoice.Line = lineList.ToArray();

              SalesItemLineDetail salesItemLineDeatil = new SalesItemLineDetail();
              salesItemLineDeatil.Qty = new Decimal(1.0);
              salesItemLineDeatil.ItemRef = new ReferenceType
              {
                  Value = item.Id
              };
              line.AnyIntuitObject = salesItemLineDeatil;

              line.DetailType = LineDetailTypeEnum.SalesItemLineDetail;
              line.DetailTypeSpecified = true;

              /// Step 5: Set other properties such as total amount, due date, email status, and transaction date
              invoice.DueDate = DateTime.UtcNow.Date;
              invoice.DueDateSpecified = true;

              invoice.TotalAmt = new Decimal(10.00);
              invoice.TotalAmtSpecified = true;

              invoice.EmailStatus = EmailStatusEnum.NotSet;
              invoice.EmailStatusSpecified = true;

              invoice.Balance = new Decimal(10.00);
              invoice.BalanceSpecified = true;

              invoice.TxnDate = DateTime.UtcNow.Date;
              invoice.TxnDateSpecified = true;
              invoice.TxnTaxDetail = new TxnTaxDetail()
              {
                  TotalTax = Convert.ToDecimal(10),
                  TotalTaxSpecified = true
              };

              ///Step 6: Initialize the service object and create Invoice
              DataService service = new DataService(serviceContext);
              Invoice addedInvoice = service.Add<Invoice>(invoice);*/
          }

          /// <summary>
          /// //////////-------------------------------------------------
          /// </summary>
        /*  //Initialize OAuth2RequestValidator and ServiceContext
          ServiceContext serviceContext = base.IntializeContext(realmId);
          DataService dataService = new DataService(serviceContext);

          //create income account
          Account newAccount = new Account
          {
              Name = "My " + type.ToString() + random.Next(),
              AccountType = AccountTypeEnum.Income,
              AccountSubType = AccountSubTypeEnum.SalesOfProductIncome.ToString(),
              AccountTypeSpecified = true,
              SubAccountSpecified = true
          };
          Account addedIncomeAccount = dataService.Add<Account>(newAccount);

          //create expense account
          newAccount = new Account{
     Name = "My "+type.ToString()+ random.Next(),
     AccountType = AccountTypeEnum.CostofGoodsSold,
     AccountSubType = AccountSubTypeEnum.SuppliesMaterialsCogs.ToString(),
     AccountTypeSpecified = true,
     SubAccountSpecified = true
  };
      Account addedIncomeAccount = dataService.Add<Account>(newAccount);

      //create asset account
      Account newAccount = new Account
      {
          Name = "My " + type.ToString() + random.Next(),
          AccountType = AccountTypeEnum.OtherAsset,
          AccountSubType = AccountSubTypeEnum.Inventory.ToString(),
          AccountTypeSpecified = true,
          SubAccountSpecified = true
      };
      Account addedIncomeAccount = dataService.Add<Account>(newAccount);

      //create inventory item using income, expense, asset account
      Item newItem = new Item
      {
          Type = ItemTypeEnum.Inventory,
          Name = "My Inventory 15" + Guid.NewGuid().ToString("N"),
          QtyOnHand = 10,
          InvStartDate = DateTime.Today,
          Description = "New Inventory with quantity 10",
          TrackQtyOnHand = true,
          TypeSpecified = true,
          QtyOnHandSpecified = true,
          TrackQtyOnHandSpecified = true,
          InvStartDateSpecified = true
      };
      newItem.IncomeAccountRef = new ReferenceType() { Value = incomeAccount.Id};
      newItem.ExpenseAccountRef = new ReferenceType() { Value = expenseAccount.Id};
      newItem.AssetAccountRef = new ReferenceType() { Value = assetAccount.Id};
      Item addedInventory = dataService.Add<Item>(newItem);

      //create invoice for the inventory item
      Invoice invoice = new Invoice();
      invoice.CustomerRef = new ReferenceType() { Value = customer.Id};

      List<Line> lineList = new List<Line>();
      Line line = new Line();
      line.Amount = new Decimal(100.00);
      line.AmountSpecified = true;
  lineList.Add(line);
  invoice.Line = lineList.ToArray();
  SalesItemLineDetail salesItemLineDetail = new SalesItemLineDetail();
      salesItemLineDetail.Qty = new Decimal(1.0);
      salesItemLineDetail.QtySpecified = true;
  salesItemLineDetail.ItemRef = new ReferenceType() { Value = addedInventory.Id};
      line.AnyIntuitObject = salesItemLineDetail;
  ine.DetailType = LineDetailTypeEnum.SalesItemLineDetail;
  line.DetailTypeSpecified = true;
  Invoice addedInvoice = dataService.Add<Invoice>(invoice);

      // Query inventory item - the quantity should be reduced
      QueryService<Item> querySvcItem = new QueryService<Item>(serviceContext);
      Item queryInventory = querySvcItem.ExecuteIdsQuery("SELECT * FROM Item WHERE Name = '" + addedInventory.Name + "'").FirstOrDefault();

      */


    }
}
