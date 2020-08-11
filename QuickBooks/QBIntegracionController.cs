﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Intuit.Ipp.Core;
using Intuit.Ipp.Data;
using Intuit.Ipp.OAuth2PlatformClient;
using Intuit.Ipp.QueryFilter;
using Intuit.Ipp.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using QuickBooks.Helper;

using RestSharp;

using Intuit.Ipp.DataService;
using GoTravelTour.Models;

namespace GoTravelTour.QuickBooks
{
    [Route("api/[controller]")]
    [ApiController]
    public class QBIntegracionController : ControllerBase
    {
       
        public static string clientid = "ABtbGg86yOB32TNPcsZSaDXVSm2wBlgV89AGXiNGMJ2ja8yVCR";
        public static string clientsecret = "iOFqEfvrOsmP7lCMmyCwlAHdHaHUWg4n1PNc6sXr";
        //public static string redirectUrl = "https://developer.intuit.com/v2/OAuth2Playground/RedirectUrl";
        public static string redirectUrl = "http://localhost:59649/api/QBIntegracion/Responses";
       
        public static string environment ="sandbox";

        public static OAuth2Client auth2Client = new OAuth2Client(clientid, clientsecret, redirectUrl, environment);

        /*Este diccionario es para almacenar los token y solamente solicitarlos una vez*/
        public static Dictionary<string, string> dictionary = new Dictionary<string, string>();
        

        private void agregarunRefreshtoken()
        {
            if (dictionary.Count == 0 || string.IsNullOrEmpty(dictionary["refreshToken"]))
            {
                dictionary["refreshToken"] = "AB11605875643sh3RdXUWzb5CWR1kmnN9ylsUFm2sBglmmCFzZ";
            }
        }

        [HttpGet]
        [Route("Connect")]
        public ActionResult InitiateAuth(string submitButton)
        {
            //return Ok("entramos");
            List<OidcScopes> scopes = new List<OidcScopes>();
            scopes.Add(OidcScopes.Accounting);
            string authorizeUrl = auth2Client.GetAuthorizationURL(scopes);

            return Redirect(authorizeUrl);
            //return Redirect("http://localhost:59649/api/QBIntegracion/Responses?code=AB11591303021AJsCwlhzsEKJUzT8YBRUnp8iYa4XSxVJGUJbK&state=e21c508b4b82468f538739ed076ab51c7efcb31bcf07c063fcd4412f1250a15c&realmId=4620816365037572030" );
        }        
      

        [HttpGet]
        [Route("Responses")]
        public async System.Threading.Tasks.Task<ActionResult> ApiCallService(string realmId, string code)
        {
            var principal = User as ClaimsPrincipal;

           /*var client = new RestClient("https://appcenter.intuit.com/app/connect/oauth2/v1/tokens/bearer");
            var request = new RestRequest(Method.POST);
            request.AddParameter("application/x-www-form-urlencoded", "grant_type=authorization_code&client_id=" + clientid + "&client_secret=" + clientsecret + "&code=" + code + "&redirect_uri=" + redirectUrl, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

             client = new RestClient("https://rest.tsheets.com/api/v1/grant");
             request = new RestRequest(Method.POST);
            request.AddParameter("application/x-www-form-urlencoded", "grant_type=authorization_code&client_id="+ clientid + "&client_secret="+ clientsecret + "&code="+ code + "&redirect_uri="+ redirectUrl, ParameterType.RequestBody);
             response = client.Execute(request);*/
            //auth2Client = new OAuth2Client(clientid, clientsecret, redirectUrl, environment);
            var tokenResponse = await auth2Client.GetBearerTokenAsync(code);

            var access_token = tokenResponse.AccessToken;
            var access_token_expires_at = tokenResponse.AccessTokenExpiresIn;

            var refresh_token = tokenResponse.RefreshToken;
            var refresh_token_expires_at = tokenResponse.RefreshTokenExpiresIn;

            if (!dictionary.ContainsKey("accessToken"))
                dictionary.Add("accessToken", access_token);
            else
                dictionary["accessToken"] = access_token;

            if (!dictionary.ContainsKey("refreshToken"))
                dictionary.Add("refreshToken", refresh_token);
            else
                dictionary["refreshToken"] = refresh_token;

            if (!dictionary.ContainsKey("realmId"))            
                dictionary.Add("realmId", realmId);
            else
                dictionary["realmId"] = realmId;



            return Ok( "ApiCallService" + " QBO API call Successful!! Response: " );
           
        }

        [HttpGet]
        [Route("addProduct")]
        public async  void AddProducto([FromBody] Traslado producto)
        {

            try
            {
                agregarunRefreshtoken();
                TokenResponse tokenResp = await auth2Client.RefreshTokenAsync(dictionary["refreshToken"]);

                if (tokenResp.AccessToken != null && tokenResp.RefreshToken != null)
                {
                    dictionary["accessToken"] = tokenResp.AccessToken;
                    dictionary["refreshToken"] = tokenResp.RefreshToken;

                }
                else
                {
                    throw new Exception();
                }
            }
            catch( Exception ex)
            {
                InitiateAuth("Connect");
                AddProducto(producto);
            }
           
            var access_token = dictionary["accessToken"];
            var realmId = dictionary["realmId"];

            OAuth2RequestValidator oauthValidator = new OAuth2RequestValidator(access_token);
            // Create a ServiceContext with Auth tokens and realmId
            ServiceContext serviceContext = new ServiceContext(realmId, IntuitServicesType.QBO, oauthValidator);
            serviceContext.IppConfiguration.MinorVersion.Qbo = "23";
            serviceContext.IppConfiguration.BaseUrl.Qbo = "https://sandbox-quickbooks.api.intuit.com/";

            // Create a QuickBooks QueryService using ServiceContext
            QueryService<CompanyInfo> querySvc = new QueryService<CompanyInfo>(serviceContext);

            try
            {
                CompanyInfo companyInfo = querySvc.ExecuteIdsQuery("SELECT * FROM CompanyInfo").FirstOrDefault();
                Bill b = new Bill();
                Invoice inv = new Invoice(); //Factura
                Item it = new Item();// Esto son los productos




                Item ObjItem = new Item();
                ObjItem.Name = "Vision Keyboard";
                ObjItem.TypeSpecified = true;
                ObjItem.Type = ItemTypeEnum.Service;
                ObjItem.TrackQtyOnHand = false;
                ObjItem.TrackQtyOnHandSpecified = false;
                ObjItem.QtyOnHandSpecified = false;
                ObjItem.QtyOnHand = 10;
                ObjItem.InvStartDateSpecified = true;
                ObjItem.InvStartDate = DateTime.Now;
                ObjItem.Description = "This Keyboard is made by vision infotech";
                ObjItem.UnitPriceSpecified = true;
                ObjItem.UnitPrice = 100;
                ObjItem.PurchaseDesc = "This Keyboard is purchase from Vision";
                ObjItem.PurchaseCostSpecified = true;
                ObjItem.PurchaseCost = 50;
                // Create a QuickBooks QueryService using ServiceContext for getting list of all accounts from Quickbooks
                QueryService<Account> querySvcAc = new QueryService<Account>(serviceContext);
                var AccountList = querySvcAc.ExecuteIdsQuery("SELECT * FROM Account").ToList();

                //Get Account of type "OtherCurrentAsset" and named "Inventory Asset" for Asset Account Reference
                var AssetAccountRef = AccountList.Where(x => x.AccountType == AccountTypeEnum.Income && x.Name == "Ventas del Sitio").FirstOrDefault();



                if (AssetAccountRef != null)
                {
                    ObjItem.AssetAccountRef = new ReferenceType();
                    ObjItem.AssetAccountRef.Value = AssetAccountRef.Id;
                }
                else
                {


                     Ok("Account of Type OtherCurrentAsset Does not found in QBO, We must have at least one Account which is Type of OtherCurrentAsset for Refrence");
                }
                //Get Account of type "Income" and named "Sales of Product Income" for Income Account Reference
                var IncomeAccountRef = AccountList.Where(x => x.AccountType == AccountTypeEnum.Income && x.Name == "Sales of Product Income").FirstOrDefault();
                if (IncomeAccountRef != null)
                {
                    ObjItem.IncomeAccountRef = new ReferenceType();
                    ObjItem.IncomeAccountRef.Value = IncomeAccountRef.Id;
                }
                else
                {

                     Ok("Account of Type Income Does not found in QBO, We must have at least one Account Name as 'Sales of Product Income' which is Type of Income for Refrence");
                }
                //Get Account of type "CostofGoodsSold" and named "Cost of Goods Sold" for Expense Account Reference
                var ExpenseAccountRef = AccountList.Where(x => x.AccountType == AccountTypeEnum.CostofGoodsSold && x.Name == "Cost of Goods Sold").FirstOrDefault();
                if (ExpenseAccountRef != null)
                {
                    ObjItem.ExpenseAccountRef = new ReferenceType();
                    ObjItem.ExpenseAccountRef.Value = ExpenseAccountRef.Id;
                }
                else
                {

                     Ok("Account of Type CostofGoodsSold Does not found in QBO, We must have at least one Account Name as 'Cost of Goods Sold' which is Type of CostofGoodsSold for Refrence");
                }
                DataService dataService = new DataService(serviceContext);
                Item ItemAdd = dataService.Add(ObjItem);
                if (ItemAdd != null && !string.IsNullOrEmpty(ItemAdd.Id))
                {
                    //you can write Database code here

                }


                string output = JsonConvert.SerializeObject(companyInfo, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });


            }
            catch
            {
                 Ok("Account of Type Income Does not found in QBO, We must have at least one Account Name as 'Sales of Product Income' which is Type of Income for Refrence");
            }
             Ok("Account of Type Income Does not found in QBO, We must have at least one Account Name as 'Sales of Product Income' which is Type of Income for Refrence");
        }





    }
}
