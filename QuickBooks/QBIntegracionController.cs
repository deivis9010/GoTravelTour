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
using Microsoft.EntityFrameworkCore;

namespace GoTravelTour.QuickBooks
{
    [Route("api/[controller]")]
    [ApiController]
    public class QBIntegracionController : ControllerBase
    {

        private readonly GoTravelDBContext _context;

        public QBIntegracionController(GoTravelDBContext context)
        {
            _context = context;
        }


        public static string clientid = "ABtbGg86yOB32TNPcsZSaDXVSm2wBlgV89AGXiNGMJ2ja8yVCR";
        public static string clientsecret = "iOFqEfvrOsmP7lCMmyCwlAHdHaHUWg4n1PNc6sXr";
        //public static string redirectUrl = "https://developer.intuit.com/v2/OAuth2Playground/RedirectUrl";
        public static string redirectUrl = "http://localhost:59649/api/QBIntegracion/Responses";
       
        public static string environment ="sandbox";

        public static OAuth2Client auth2Client = new OAuth2Client(clientid, clientsecret, redirectUrl, environment);

        /*Este diccionario es para almacenar los token y solamente solicitarlos una vez*/
        public static Dictionary<string, string> dictionary = new Dictionary<string, string>();

        public static string QboBaseUrl = "https://sandbox-quickbooks.api.intuit.com/";

        private void CargarRefreshtoken()
        {
            if (dictionary.Count == 0 || !dictionary.ContainsKey("refreshToken"))
            {
                dictionary["refreshToken"] = _context.TokenQB.First().RefreshToken;
                dictionary["realmId"] = _context.TokenQB.First().RealmId;
            }
        }

        private void ActualizarRefreshtoken(string token, string realmId)
        {


            if (_context.TokenQB.Count()>0)
            {
                TokenQB tok = _context.TokenQB.First();
                tok.RefreshToken = token;
                tok.RealmId = realmId;

                _context.Entry(tok).State = EntityState.Modified;

                try
                {
                    _context.SaveChangesAsync();
                    dictionary["refreshToken"] = tok.RefreshToken;
                    dictionary["realmId"] = realmId;
                    return;
                }
                catch (DbUpdateConcurrencyException)
                {


                }
            }
            else
            {
                TokenQB tok = new TokenQB();
                tok.RefreshToken = token;
                tok.RealmId = realmId;
                _context.TokenQB.Add(tok);
                _context.SaveChangesAsync();
                dictionary["refreshToken"] = tok.RefreshToken;
                dictionary["realmId"] = realmId;
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
             response = client.Execute(request);
            //auth2Client = new OAuth2Client(clientid, clientsecret, redirectUrl, environment);*/
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

            ActualizarRefreshtoken(refresh_token,realmId);

            return Redirect("http://gotravelandtours.com");
           
           
        }

        [HttpGet]
        [Route("createCustomer")]
        public async System.Threading.Tasks.Task<ActionResult> CreateCustomer([FromBody] Cliente cliente)
        {
            var access_token = "";
            var realmId = "";

            try
            {
                CargarRefreshtoken();
                TokenResponse tokenResp = await auth2Client.RefreshTokenAsync(dictionary["refreshToken"]);

                if (tokenResp.AccessToken != null && tokenResp.RefreshToken != null)
                {
                    dictionary["accessToken"] = tokenResp.AccessToken;
                    dictionary["refreshToken"] = tokenResp.RefreshToken;
                    ActualizarRefreshtoken(tokenResp.RefreshToken, dictionary["realmId"]);

                }
                else
                {
                    throw new Exception();
                }





                access_token = dictionary["accessToken"];
                realmId = dictionary["realmId"];
            }
            catch (Exception ex)
            {


                return InitiateAuth("Connect");
            }



            OAuth2RequestValidator oauthValidator = new OAuth2RequestValidator(access_token);
            // Create a ServiceContext with Auth tokens and realmId
            ServiceContext serviceContext = new ServiceContext(realmId, IntuitServicesType.QBO, oauthValidator);
            serviceContext.IppConfiguration.MinorVersion.Qbo = "23";
            serviceContext.IppConfiguration.BaseUrl.Qbo = QboBaseUrl;

            // Create a QuickBooks QueryService using ServiceContext
            QueryService<Customer> querySvc = new QueryService<Customer>(serviceContext);

            try
            {
               // CompanyInfo companyInfo = querySvc.ExecuteIdsQuery("SELECT * FROM CompanyInfo").FirstOrDefault();
                Bill b = new Bill();
                Invoice inv = new Invoice(); //Factura
                Item it = new Item();// Esto son los productos
                Estimate est = new Estimate();
                Customer c = new Customer();



                Customer ObjItem = new Customer();
                ObjItem.DisplayName = cliente.Nombre;
                ObjItem.FamilyName = cliente.Nombre;
                ObjItem.GivenName = cliente.Nombre;
                ObjItem.ContactName= cliente.Nombre;
                ObjItem.Title = cliente.Nombre;
                ObjItem.PrimaryEmailAddr = new EmailAddress { Address = cliente.Correo };
                ObjItem.AlternatePhone = new TelephoneNumber { DeviceType="LandLine", FreeFormNumber= cliente.Telefono } ;
                ObjItem.Mobile= new TelephoneNumber { DeviceType = "Mobile", FreeFormNumber = cliente.Telefono };


                              
                DataService dataService = new DataService(serviceContext);
                Customer customer = dataService.Add(ObjItem);
                if (customer != null && !string.IsNullOrEmpty(customer.Id))
                {
                    return Ok("Se inserto el cliente");

                }
                else
                {
                    return Ok("No se insertó el cliente");

                }



            }
            catch (Exception ex)
            {
                return Ok("Exception : " + ex.Message);
            }
            
        } 

        [HttpGet]
        [Route("addProductTraslado")]
        public async System.Threading.Tasks.Task<ActionResult> AddProducto([FromBody] Traslado producto)
        {
            var access_token = "";
            var realmId = "";
         
            try
            {
                CargarRefreshtoken();
                TokenResponse tokenResp = await auth2Client.RefreshTokenAsync(dictionary["refreshToken"]);

                if (tokenResp.AccessToken != null && tokenResp.RefreshToken != null)
                {
                    dictionary["accessToken"] = tokenResp.AccessToken;
                    dictionary["refreshToken"] = tokenResp.RefreshToken;
                    ActualizarRefreshtoken(tokenResp.RefreshToken, dictionary["realmId"] );

                }
                else
                {
                    throw new Exception();
                }





                 access_token = dictionary["accessToken"];
                 realmId = dictionary["realmId"];
            }
            catch( Exception ex)
            {


                return InitiateAuth("Connect");
            }
           
           

            OAuth2RequestValidator oauthValidator = new OAuth2RequestValidator(access_token);
            // Create a ServiceContext with Auth tokens and realmId
            ServiceContext serviceContext = new ServiceContext(realmId, IntuitServicesType.QBO, oauthValidator);
            serviceContext.IppConfiguration.MinorVersion.Qbo = "23";
            serviceContext.IppConfiguration.BaseUrl.Qbo = QboBaseUrl;

            // Create a QuickBooks QueryService using ServiceContext
            QueryService<CompanyInfo> querySvc = new QueryService<CompanyInfo>(serviceContext);

            try
            {
                CompanyInfo companyInfo = querySvc.ExecuteIdsQuery("SELECT * FROM CompanyInfo").FirstOrDefault();
                Bill b = new Bill();
                Invoice inv = new Invoice(); //Factura
                Item it = new Item();// Esto son los productos
                Estimate est = new Estimate();
                Customer c = new Customer();

                // Create a QuickBooks QueryService using ServiceContext
                QueryService<Item> querySvcI = new QueryService<Item>(serviceContext);
                List<Item> categ = querySvcI.ExecuteIdsQuery("SELECT * from Item ").Where(x=> x.Name == "Activity" && x.Type == ItemTypeEnum.Category).ToList();
                Item a = new Item();
                a = categ.First();
                /*foreach(var itt in categ)
                {
                    if(itt.Name == "Activity")
                    a = itt;
                }*/

                Item ObjItem = new Item();
                ObjItem.Name = producto.Nombre;
                ObjItem.ParentRef = new ReferenceType { Value= a.Id, type=ItemTypeEnum.Category.GetStringValue(),name=a.Name };
                ObjItem.TypeSpecified = true;
                ObjItem.Sku = producto.SKU;
                ObjItem.Type = ItemTypeEnum.Service;
                ObjItem.SubItem = true;
                ObjItem.SubItemSpecified = true;


                /*   ObjItem.TrackQtyOnHand = false;
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
                   ObjItem.PurchaseCost = 50;*/
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


                    return Ok("Account of Type OtherCurrentAsset Does not found in QBO, We must have at least one Account which is Type of OtherCurrentAsset for Refrence");
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

                    return Ok("Account of Type Income Does not found in QBO, We must have at least one Account Name as 'Sales of Product Income' which is Type of Income for Refrence");
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

                    return Ok("Account of Type CostofGoodsSold Does not found in QBO, We must have at least one Account Name as 'Cost of Goods Sold' which is Type of CostofGoodsSold for Refrence");
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
            catch(Exception ex)
            {
                return Ok("Account of Type Income Does not found in QBO, We must have at least one Account Name as 'Sales of Product Income' which is Type of Income for Refrence");
            }
            return Ok("Account of Type Income Does not found in QBO, We must have at least one Account Name as 'Sales of Product Income' which is Type of Income for Refrence");
        }





        [HttpGet]
        [Route("createCategory")]
        public async System.Threading.Tasks.Task<ActionResult> CreateCategory(/*[FromBody] string nombre/*, [FromBody] string tipoProducto*/)
        {
            var access_token = "";
            var realmId = "";

            try
            {
                CargarRefreshtoken();
                TokenResponse tokenResp = await auth2Client.RefreshTokenAsync(dictionary["refreshToken"]);

                if (tokenResp.AccessToken != null && tokenResp.RefreshToken != null)
                {
                    dictionary["accessToken"] = tokenResp.AccessToken;
                    dictionary["refreshToken"] = tokenResp.RefreshToken;
                    ActualizarRefreshtoken(tokenResp.RefreshToken, dictionary["realmId"]);

                }
                else
                {
                    throw new Exception();
                }





                access_token = dictionary["accessToken"];
                realmId = dictionary["realmId"];
            }
            catch (Exception ex)
            {


                return InitiateAuth("Connect");
            }



            OAuth2RequestValidator oauthValidator = new OAuth2RequestValidator(access_token);
            // Create a ServiceContext with Auth tokens and realmId
            ServiceContext serviceContext = new ServiceContext(realmId, IntuitServicesType.QBO, oauthValidator);
            serviceContext.IppConfiguration.MinorVersion.Qbo = "23";
            serviceContext.IppConfiguration.BaseUrl.Qbo = QboBaseUrl;
            Item ObjCategoryItem = new Item();
            ObjCategoryItem.Name = "Activity";
            ObjCategoryItem.TypeSpecified = true;
            ObjCategoryItem.Type = ItemTypeEnum.Category;
            DataService dataService = new DataService(serviceContext);
            Item CategoryItemAdd = dataService.Add(ObjCategoryItem);
            if (CategoryItemAdd != null && !string.IsNullOrEmpty(CategoryItemAdd.Id))
            {
                return Ok("Se creo la categoria");
            }
            else
            {
                return Ok("No se creo la categoria");
            }
        }
            
        
      


    }
}
