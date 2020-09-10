using System;
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
using GoTravelTour.Utiles;

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

        public static string environment = "sandbox";

        public static OAuth2Client auth2Client = new OAuth2Client(clientid, clientsecret, redirectUrl, environment);

        /*Este diccionario es para almacenar los token y solamente solicitarlos una vez*/
        public static Dictionary<string, string> dictionary = new Dictionary<string, string>();

        public static string QboBaseUrl = "https://sandbox-quickbooks.api.intuit.com/";

        /// <summary>
        /// Carga desde la base de el utltimo token
        /// </summary>
        private void CargarRefreshtoken()
        {
            if (dictionary.Count == 0 || !dictionary.ContainsKey("refreshToken"))
            {
                dictionary["refreshToken"] = _context.TokenQB.First().RefreshToken;
                dictionary["realmId"] = _context.TokenQB.First().RealmId;
            }
        }

        /// <summary>
        /// actualiza el token almacenado en base de datos
        /// </summary>
        /// <param name="token"></param>
        /// <param name="realmId"></param>
        private void ActualizarRefreshtoken(string token, string realmId)
        {


            if (_context.TokenQB.Count() > 0)
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
            Utiles.Utiles u = new Utiles.Utiles(_context);
            u.CrearExcel();
            return Ok();
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

            ActualizarRefreshtoken(refresh_token, realmId);

            return Redirect("http://gotravelandtours.com");


        }


        [HttpGet]
        [Route("updateCategory")]        
        public async System.Threading.Tasks.Task<ActionResult> updateCategory(string old_nombre, string new_nombre)
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
            QueryService<Item> querySvc = new QueryService<Item>(serviceContext);
            string EXISTING_ITEM_QUERYBYNAME = string.Format("select * from Item where Name = '{0}' and Type = 'Category' ", old_nombre);
            Item objItemFound = querySvc.ExecuteIdsQuery(EXISTING_ITEM_QUERYBYNAME).FirstOrDefault<Item>();


            if (objItemFound != null)
            {
                
                Item ObjItem = new Item();
                ObjItem.Name = new_nombre;               
                ObjItem.TypeSpecified = true;
                // ObjItem.Sku = producto.SKU;
                ObjItem.Type = ItemTypeEnum.Category;
                ObjItem.SubItem = true;
                ObjItem.SubItemSpecified = true;
               

                DataService dataService = new DataService(serviceContext);
                Item ItemAdd = dataService.Add(ObjItem);
                if (ItemAdd != null && !string.IsNullOrEmpty(ItemAdd.Id))
                {
                    return Ok("Se actualizo la categoria");
                }               
                else
                {
                    return Ok("No se actualizo la categoria");
                }


            }

            return Ok("No se encontro la categoria");
        }




        [HttpGet]
        [Route("createCustomer")]
        public async System.Threading.Tasks.Task<ActionResult> CreateCustomer([FromBody] Cliente cliente)
        {
            var access_token = "";
            var realmId = "";
            cliente = _context.Clientes.Find(cliente.ClienteId);
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

                Customer ObjItem = new Customer();
                ObjItem.DisplayName = cliente.Nombre;
                ObjItem.FamilyName = cliente.Nombre;
                ObjItem.GivenName = cliente.Nombre;
                ObjItem.ContactName = cliente.Nombre;
                ObjItem.Title = cliente.Nombre;
                ObjItem.PrimaryEmailAddr = new EmailAddress { Address = cliente.Correo };
                ObjItem.AlternatePhone = new TelephoneNumber { DeviceType = "LandLine", FreeFormNumber = cliente.Telefono };
                ObjItem.Mobile = new TelephoneNumber { DeviceType = "Mobile", FreeFormNumber = cliente.Telefono };
                ObjItem.PrimaryPhone = new TelephoneNumber { DeviceType = "Mobile", FreeFormNumber = cliente.Telefono };



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
        [Route("deleteCustomer")]
        public async System.Threading.Tasks.Task<ActionResult> DeleteCustomer([FromBody] Cliente cliente)
        {
            var access_token = "";
            var realmId = "";
            cliente = _context.Clientes.Find(cliente.ClienteId);
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

            QueryService<Customer> querySvc = new QueryService<Customer>(serviceContext);
            string EXISTING_ITEM_QUERYBYNAME = string.Format("select * from Customer where Name = '{0}' ", cliente.Nombre);
            Customer objItemFound = querySvc.ExecuteIdsQuery(EXISTING_ITEM_QUERYBYNAME).FirstOrDefault<Customer>();



            //If Item found on Quickbooks online
            if (objItemFound != null)
            {
                //if Item is active
                if (objItemFound.Active == true)
                {
                    objItemFound.Active = false;
                    DataService dataService = new DataService(serviceContext);
                    Customer UpdateEntity = dataService.Update<Customer>(objItemFound);
                    if (UpdateEntity != null && !string.IsNullOrEmpty(UpdateEntity.Id))
                    {
                        //you can write Database code here
                        return Ok("Se desactivo el cliente");
                    }
                    else
                    {
                        return Ok("No se desactivo el cliente");
                    }
                }
                else
                {
                    return Ok("No se desactivo el cliente pues ya estaba desactivado");
                }
            }

            return Ok("No se encontro el cliente ");




        }


        [HttpGet]
        [Route("updateCustomer")]
        public async System.Threading.Tasks.Task<ActionResult> UpdateCustomer([FromBody] Cliente cliente)
        {
            var access_token = "";
            var realmId = "";
            cliente = _context.Clientes.Find(cliente.ClienteId);
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

            QueryService<Customer> querySvc = new QueryService<Customer>(serviceContext);
            string EXISTING_ITEM_QUERYBYNAME = string.Format("select * from Customer where Name = '{0}' ", cliente.Nombre);
            Customer objItemFound = querySvc.ExecuteIdsQuery(EXISTING_ITEM_QUERYBYNAME).FirstOrDefault<Customer>();


            if (objItemFound != null)
            {
                Customer ObjItem = new Customer();
                ObjItem.Id = objItemFound.Id;
                ObjItem.DisplayName = cliente.Nombre;
                ObjItem.FamilyName = cliente.Nombre;
                ObjItem.GivenName = cliente.Nombre;
                ObjItem.ContactName = cliente.Nombre;
                ObjItem.Title = cliente.Nombre;
                ObjItem.PrimaryEmailAddr = new EmailAddress { Address = cliente.Correo };
                ObjItem.AlternatePhone = new TelephoneNumber { DeviceType = "LandLine", FreeFormNumber = cliente.Telefono };
                ObjItem.Mobile = new TelephoneNumber { DeviceType = "Mobile", FreeFormNumber = cliente.Telefono };
                ObjItem.PrimaryPhone = new TelephoneNumber { DeviceType = "Mobile", FreeFormNumber = cliente.Telefono };
                DataService dataService = new DataService(serviceContext);
                Customer UpdateEntity = dataService.Update<Customer>(ObjItem);
                if (UpdateEntity != null && !string.IsNullOrEmpty(UpdateEntity.Id))
                {
                    //you can write Database code here
                    return Ok("Se actualizo el cliente");
                }
                else
                {
                    return Ok("No se actualizo el cliente");
                }


            }

            return Ok("No se encontro el cliente ");




        }


        [HttpGet]
        [Route("addProductTraslado")]
        public async System.Threading.Tasks.Task<ActionResult> AddProductoTraslado([FromBody] Traslado producto)
        {
            var access_token = "";
            var realmId = "";
            producto = _context.Traslados.Include(x => x.Proveedor).First(x => x.ProductoId == producto.ProductoId);
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
           // QueryService<CompanyInfo> querySvc = new QueryService<CompanyInfo>(serviceContext);

            try
            {
               

                Proveedor proveedor = new Proveedor();
                proveedor = producto.Proveedor;


                // Create a QuickBooks QueryService using ServiceContext
                QueryService<Item> querySvcI = new QueryService<Item>(serviceContext);
                List<Item> tiposProductos = querySvcI.ExecuteIdsQuery("SELECT * from Item ").Where(x => x.Name == "Ground Transportation" && x.Type == ItemTypeEnum.Category).ToList();

                Item tipoProd = new Item();
                tipoProd = tiposProductos.First();

                List<Item> proveedores = querySvcI.ExecuteIdsQuery("SELECT * from Item ").Where(x => x.Name == proveedor.Nombre && x.Type == ItemTypeEnum.Category).ToList();
                Item prov = new Item();
                if (proveedores == null || proveedores.Count() == 0)
                {
                    prov = agregarCategoriaProveedor(tipoProd, proveedor, serviceContext);
                    Vendor ven = agregarVendorProveedor(proveedor, serviceContext);
                    if (ven == null) return Ok("Error insertando el Vendor");
                    if (prov == null) return Ok("Error insertando el proveedor");
                }
                else
                {
                    prov = proveedores.First();
                }



                QueryService<Item> querySvc1 = new QueryService<Item>(serviceContext);
                string EXISTING_ITEM_QUERYBYNAME = string.Format("select * from Item where Name = '{0}' ", producto.Nombre);
                Item objItemFound = querySvc1.ExecuteIdsQuery(EXISTING_ITEM_QUERYBYNAME).FirstOrDefault<Item>();


                if (objItemFound != null)
                {
                    if (objItemFound.Active == false)
                    {
                        objItemFound.Active = true;
                        DataService dataService1 = new DataService(serviceContext);
                        Item UpdateEntity = dataService1.Update<Item>(objItemFound);
                        if (UpdateEntity != null && !string.IsNullOrEmpty(UpdateEntity.Id))
                        {
                            //you can write Database code here
                            return Ok("Ya estaba en QB y se activo");
                        }
                        else
                        {
                            return Ok("Ya estaba en QB pero no se activo");
                        }
                    }
                    else
                    {
                        return Ok("Ya hay un producto en QB con ese nombre");
                    }
                   


                }


                Item ObjItem = new Item();
                ObjItem.Name = producto.Nombre;
                ObjItem.ParentRef = new ReferenceType { Value = prov.Id, type = ItemTypeEnum.Category.GetStringValue(), name = prov.Name };
                ObjItem.TypeSpecified = true;
                ObjItem.Sku = producto.SKU;
                ObjItem.Type = ItemTypeEnum.Service;
                ObjItem.SubItem = true;
                ObjItem.SubItemSpecified = true;



                // Create a QuickBooks QueryService using ServiceContext for getting list of all accounts from Quickbooks
                QueryService<Account> querySvcAc = new QueryService<Account>(serviceContext);
                var AccountList = querySvcAc.ExecuteIdsQuery("SELECT * FROM Account").ToList();

                //Get Account of type "OtherCurrentAsset" and named "Inventory Asset" for Asset Account Reference
                /*var AssetAccountRef = AccountList.Where(x => x.AccountType == AccountTypeEnum.Income && x.Name == "Ground Transportation Booking").FirstOrDefault();



                if (AssetAccountRef != null)
                {
                    ObjItem.AssetAccountRef = new ReferenceType();
                    ObjItem.AssetAccountRef.Value = AssetAccountRef.Id;
                }
                else
                {


                    return Ok("Error obteniendo la cuenta Asset");
                }*/
                //Get Account of type "Income" and named "Sales of Product Income" for Income Account Reference
                var IncomeAccountRef = AccountList.Where(x => x.AccountType == AccountTypeEnum.Income && x.Name == "Ground Transportation").FirstOrDefault();
                if (IncomeAccountRef != null)
                {
                    ObjItem.IncomeAccountRef = new ReferenceType();
                    ObjItem.IncomeAccountRef.Value = IncomeAccountRef.Id;
                }
                else
                {

                    return Ok("Error obteniendo la cuenta Income");
                }
                //Get Account of type "CostofGoodsSold" and named "Cost of Goods Sold" for Expense Account Reference
                var ExpenseAccountRef = AccountList.Where(x => x.AccountType == AccountTypeEnum.CostofGoodsSold && x.Name == "Ground Transportation Booking").FirstOrDefault();
                if (ExpenseAccountRef != null)
                {
                    ObjItem.ExpenseAccountRef = new ReferenceType();
                    ObjItem.ExpenseAccountRef.Value = ExpenseAccountRef.Id;
                }
                else
                {

                    return Ok("Error obteniendo la cuenta Expense");
                }
                DataService dataService = new DataService(serviceContext);
                Item ItemAdd = dataService.Add(ObjItem);
                if (ItemAdd != null && !string.IsNullOrEmpty(ItemAdd.Id))
                {
                    //you can write Database code here

                }


                /*string output = JsonConvert.SerializeObject(companyInfo, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });*/


            }
            catch (Exception ex)
            {
                return Ok("Error no determinado. El error es: " + ex.Message);
            }
            return Ok("Se insertó correctamente el producto");
        }


        [HttpGet]
        [Route("updateProductTraslado")]
        public async System.Threading.Tasks.Task<ActionResult> UpdateProductoTraslado([FromBody] Traslado producto_new)
        {
            var access_token = "";
            var realmId = "";
            Traslado producto = _context.Traslados.Include(x => x.Proveedor).First(x => x.ProductoId == producto_new.ProductoId);
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
            // QueryService<CompanyInfo> querySvc = new QueryService<CompanyInfo>(serviceContext);

            try
            {


                Proveedor proveedor = new Proveedor();
                proveedor = producto.Proveedor;


                // Create a QuickBooks QueryService using ServiceContext
                QueryService<Item> querySvcI = new QueryService<Item>(serviceContext);
                List<Item> tiposProductos = querySvcI.ExecuteIdsQuery("SELECT * from Item ").Where(x => x.Name == "Ground Transportation" && x.Type == ItemTypeEnum.Category).ToList();

                Item tipoProd = new Item();
                tipoProd = tiposProductos.First();

                List<Item> proveedores = querySvcI.ExecuteIdsQuery("SELECT * from Item ").Where(x => x.Name == proveedor.Nombre && x.Type == ItemTypeEnum.Category).ToList();
                Item prov = new Item();
                if (proveedores == null || proveedores.Count() == 0)
                {
                    prov = agregarCategoriaProveedor(tipoProd, proveedor, serviceContext);
                    Vendor ven = agregarVendorProveedor(proveedor, serviceContext);
                    if (ven == null) return Ok("Error insertando el Vendor");
                    if (prov == null) return Ok("Error insertando el proveedor");
                }
                else
                {
                    prov = proveedores.First();
                }



                QueryService<Item> querySvc1 = new QueryService<Item>(serviceContext);
                string EXISTING_ITEM_QUERYBYNAME = string.Format("select * from Item where Name = '{0}' ", producto.Nombre);
                Item objItemFound = querySvc1.ExecuteIdsQuery(EXISTING_ITEM_QUERYBYNAME).FirstOrDefault<Item>();


                if (objItemFound != null)
                {
                    Item ObjItem = new Item();
                    ObjItem.Id = objItemFound.Id;
                    ObjItem.Name = producto.Nombre;
                    ObjItem.ParentRef = new ReferenceType { Value = prov.Id, type = ItemTypeEnum.Category.GetStringValue(), name = prov.Name };
                    ObjItem.TypeSpecified = true;
                    ObjItem.Sku = producto.SKU;
                    ObjItem.Type = ItemTypeEnum.Service;
                    ObjItem.SubItem = true;
                    ObjItem.SubItemSpecified = true;

                   
                        DataService dataService1 = new DataService(serviceContext);
                        Item UpdateEntity = dataService1.Update<Item>(objItemFound);
                        if (UpdateEntity != null && !string.IsNullOrEmpty(UpdateEntity.Id))
                        {
                            //you can write Database code here
                            return Ok("Se actualizo el producto");
                        }
                        else
                        {
                            return Ok("No se actualizo el producto");
                        }
                  



                }



            }
            catch (Exception ex)
            {
                return Ok("Error no determinado. El error es: " + ex.Message);
            }
            return Ok("Se insertó correctamente el producto");
        }

        [HttpGet]
        [Route("deleteProductTraslado")]
        public async System.Threading.Tasks.Task<ActionResult> DeleteProductTraslado([FromBody] Traslado traslado)
        {
            var access_token = "";
            var realmId = "";
            traslado = _context.Traslados.Find(traslado.ProductoId);
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

            QueryService<Item> querySvc = new QueryService<Item>(serviceContext);
            string EXISTING_ITEM_QUERYBYNAME = string.Format("select * from Item where Name = '{0}' ", traslado.Nombre);
            Item objItemFound = querySvc.ExecuteIdsQuery(EXISTING_ITEM_QUERYBYNAME).FirstOrDefault<Item>();



            //If Item found on Quickbooks online
            if (objItemFound != null)
            {
                //if Item is active
                if (objItemFound.Active == true)
                {
                    objItemFound.Active = false;
                    DataService dataService = new DataService(serviceContext);
                    Item UpdateEntity = dataService.Update<Item>(objItemFound);
                    if (UpdateEntity != null && !string.IsNullOrEmpty(UpdateEntity.Id))
                    {
                        //you can write Database code here
                        return Ok("Se desactivo el producto");
                    }
                    else
                    {
                        return Ok("No se desactivo el producto");
                    }
                }
                else
                {
                    return Ok("No se desactivo el producto pues ya estaba desactivado");
                }
            }

            return Ok("No se encontro el producto ");




        }



        [HttpGet]
        [Route("addProductActividad")]
        public async System.Threading.Tasks.Task<ActionResult> AddProductoActividad([FromBody] Actividad producto)
        {
            var access_token = "";
            var realmId = "";
            producto = _context.Actividadess.Include(x => x.Proveedor).First(x => x.ProductoId == producto.ProductoId);
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
            //QueryService<CompanyInfo> querySvc = new QueryService<CompanyInfo>(serviceContext);

            try
            {
                

                Proveedor proveedor = new Proveedor();
                proveedor = producto.Proveedor;


                // Create a QuickBooks QueryService using ServiceContext
                QueryService<Item> querySvcI = new QueryService<Item>(serviceContext);
                List<Item> tiposProductos = querySvcI.ExecuteIdsQuery("SELECT * from Item ").Where(x => x.Name == "Activity" && x.Type == ItemTypeEnum.Category).ToList();

                Item tipoProd = new Item();
                tipoProd = tiposProductos.First();

                List<Item> proveedores = querySvcI.ExecuteIdsQuery("SELECT * from Item ").Where(x => x.Name == proveedor.Nombre && x.Type == ItemTypeEnum.Category).ToList();
                Item prov = new Item();
                if (proveedores == null || proveedores.Count() == 0)
                {
                    prov = agregarCategoriaProveedor(tipoProd, proveedor, serviceContext);
                    Vendor ven = agregarVendorProveedor(proveedor, serviceContext);
                    if (ven == null) return Ok("Error insertando el Vendor");
                    if (prov == null) return Ok("Error insertando el proveedor");
                }
                else
                {
                    prov = proveedores.First();
                }

                
                QueryService<Item> querySvc1 = new QueryService<Item>(serviceContext);
                string EXISTING_ITEM_QUERYBYNAME = string.Format("select * from Item where Name = '{0}' ", producto.Nombre);
                Item objItemFound = querySvc1.ExecuteIdsQuery(EXISTING_ITEM_QUERYBYNAME).FirstOrDefault<Item>();


                if (objItemFound != null)
                {
                    if (objItemFound.Active == false)
                    {
                        objItemFound.Active = true;
                        DataService dataService1 = new DataService(serviceContext);
                        Item UpdateEntity = dataService1.Update<Item>(objItemFound);
                        if (UpdateEntity != null && !string.IsNullOrEmpty(UpdateEntity.Id))
                        {
                            //you can write Database code here
                            return Ok("Ya estaba en QB y se activo");
                        }
                        else
                        {
                            return Ok("Ya estaba en QB pero no se activo");
                        }
                    }
                    else
                    {
                        return Ok("Ya hay un producto en QB con ese nombre");
                    }
                   


                }

                Item ObjItem = new Item();
                ObjItem.Name = producto.Nombre;
                ObjItem.ParentRef = new ReferenceType { Value = prov.Id, type = ItemTypeEnum.Category.GetStringValue(), name = prov.Name };
                ObjItem.TypeSpecified = true;
                ObjItem.Sku = producto.SKU;
                ObjItem.Type = ItemTypeEnum.Service;
                ObjItem.SubItem = true;
                ObjItem.SubItemSpecified = true;



                // Create a QuickBooks QueryService using ServiceContext for getting list of all accounts from Quickbooks
                QueryService<Account> querySvcAc = new QueryService<Account>(serviceContext);
                var AccountList = querySvcAc.ExecuteIdsQuery("SELECT * FROM Account").ToList();

                //Get Account of type "OtherCurrentAsset" and named "Inventory Asset" for Asset Account Reference
                /*var AssetAccountRef = AccountList.Where(x => x.AccountType == AccountTypeEnum.Income && x.Name == "Ventas del Sitio").FirstOrDefault();



                if (AssetAccountRef != null)
                {
                    ObjItem.AssetAccountRef = new ReferenceType();
                    ObjItem.AssetAccountRef.Value = AssetAccountRef.Id;
                }
                else
                {


                    return Ok("Error obteniendo la cuenta Asset");
                }*/
                //Get Account of type "Income" and named "Sales of Product Income" for Income Account Reference
                var IncomeAccountRef = AccountList.Where(x => x.AccountType == AccountTypeEnum.Income && x.Name == "Activity").FirstOrDefault();
                if (IncomeAccountRef != null)
                {
                    ObjItem.IncomeAccountRef = new ReferenceType();
                    ObjItem.IncomeAccountRef.Value = IncomeAccountRef.Id;
                }
                else
                {

                    return Ok("Error obteniendo la cuenta Income");
                }
                //Get Account of type "CostofGoodsSold" and named "Cost of Goods Sold" for Expense Account Reference
                var ExpenseAccountRef = AccountList.Where(x => x.AccountType == AccountTypeEnum.CostofGoodsSold && x.Name == "Activity Booking").FirstOrDefault();
                if (ExpenseAccountRef != null)
                {
                    ObjItem.ExpenseAccountRef = new ReferenceType();
                    ObjItem.ExpenseAccountRef.Value = ExpenseAccountRef.Id;
                }
                else
                {

                    return Ok("Error obteniendo la cuenta Expense");
                }
                DataService dataService = new DataService(serviceContext);
                Item ItemAdd = dataService.Add(ObjItem);
                if (ItemAdd != null && !string.IsNullOrEmpty(ItemAdd.Id))
                {
                    //you can write Database code here

                }


                


            }
            catch (Exception ex)
            {
                return Ok("Error no determinado. El error es: " + ex.Message);
            }
            return Ok("Se insertó correctamente el producto");
        }



        [HttpGet]
        [Route("updateProductActividad")]
        public async System.Threading.Tasks.Task<ActionResult> UpdateProductoActividad([FromBody] Actividad producto_new)
        {
            var access_token = "";
            var realmId = "";
            Actividad producto = _context.Actividadess.Include(x => x.Proveedor).First(x => x.ProductoId == producto_new.ProductoId);

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
            // QueryService<CompanyInfo> querySvc = new QueryService<CompanyInfo>(serviceContext);

            try
            {


                Proveedor proveedor = new Proveedor();
                proveedor = producto.Proveedor;


                // Create a QuickBooks QueryService using ServiceContext
                QueryService<Item> querySvcI = new QueryService<Item>(serviceContext);
                List<Item> tiposProductos = querySvcI.ExecuteIdsQuery("SELECT * from Item ").Where(x => x.Name == "Ground Transportation" && x.Type == ItemTypeEnum.Category).ToList();

                Item tipoProd = new Item();
                tipoProd = tiposProductos.First();

                List<Item> proveedores = querySvcI.ExecuteIdsQuery("SELECT * from Item ").Where(x => x.Name == proveedor.Nombre && x.Type == ItemTypeEnum.Category).ToList();
                Item prov = new Item();
                if (proveedores == null || proveedores.Count() == 0)
                {
                    prov = agregarCategoriaProveedor(tipoProd, proveedor, serviceContext);
                    Vendor ven = agregarVendorProveedor(proveedor, serviceContext);
                    if (ven == null) return Ok("Error insertando el Vendor");
                    if (prov == null) return Ok("Error insertando el proveedor");
                }
                else
                {
                    prov = proveedores.First();
                }



                QueryService<Item> querySvc1 = new QueryService<Item>(serviceContext);
                string EXISTING_ITEM_QUERYBYNAME = string.Format("select * from Item where Name = '{0}' ", producto.Nombre);
                Item objItemFound = querySvc1.ExecuteIdsQuery(EXISTING_ITEM_QUERYBYNAME).FirstOrDefault<Item>();


                if (objItemFound != null)
                {
                    Item ObjItem = new Item();
                    ObjItem.Id = objItemFound.Id;
                    ObjItem.Name = producto.Nombre;
                    ObjItem.ParentRef = new ReferenceType { Value = prov.Id, type = ItemTypeEnum.Category.GetStringValue(), name = prov.Name };
                    ObjItem.TypeSpecified = true;
                    ObjItem.Sku = producto.SKU;
                    ObjItem.Type = ItemTypeEnum.Service;
                    ObjItem.SubItem = true;
                    ObjItem.SubItemSpecified = true;


                    DataService dataService1 = new DataService(serviceContext);
                    Item UpdateEntity = dataService1.Update<Item>(objItemFound);
                    if (UpdateEntity != null && !string.IsNullOrEmpty(UpdateEntity.Id))
                    {
                        //you can write Database code here
                        return Ok("Se actualizo el producto");
                    }
                    else
                    {
                        return Ok("No se actualizo el producto");
                    }




                }



            }
            catch (Exception ex)
            {
                return Ok("Error no determinado. El error es: " + ex.Message);
            }
            return Ok("Se insertó correctamente el producto");
        }


        [HttpGet]
        [Route("deleteProductActividad")]
        public async System.Threading.Tasks.Task<ActionResult> DeleteProductActividad([FromBody] Actividad producto)
        {
            var access_token = "";
            var realmId = "";
            producto = _context.Actividadess.Find(producto.ProductoId);
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

            QueryService<Item> querySvc = new QueryService<Item>(serviceContext);
            string EXISTING_ITEM_QUERYBYNAME = string.Format("select * from Item where Name = '{0}' ", producto.Nombre);
            Item objItemFound = querySvc.ExecuteIdsQuery(EXISTING_ITEM_QUERYBYNAME).FirstOrDefault<Item>();



            //If Item found on Quickbooks online
            if (objItemFound != null)
            {
                //if Item is active
                if (objItemFound.Active == true)
                {
                    objItemFound.Active = false;
                    DataService dataService = new DataService(serviceContext);
                    Item UpdateEntity = dataService.Update<Item>(objItemFound);
                    if (UpdateEntity != null && !string.IsNullOrEmpty(UpdateEntity.Id))
                    {
                        //you can write Database code here
                        return Ok("Se desactivo el producto");
                    }
                    else
                    {
                        return Ok("No se desactivo el producto");
                    }
                }
                else
                {
                    return Ok("No se desactivo el producto pues ya estaba desactivado");
                }
            }

            return Ok("No se encontro el producto ");




        }



        [HttpGet]
        [Route("addProductVehiculo")]
        public async System.Threading.Tasks.Task<ActionResult> AddProductoVehiculo([FromBody] Vehiculo producto)
        {
            var access_token = "";
            var realmId = "";
            producto = _context.Vehiculos.Include(x => x.Proveedor).First(x => x.ProductoId == producto.ProductoId);
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
            //QueryService<CompanyInfo> querySvc = new QueryService<CompanyInfo>(serviceContext);

            try
            {
                /*CompanyInfo companyInfo = querySvc.ExecuteIdsQuery("SELECT * FROM CompanyInfo").FirstOrDefault();
                Bill b = new Bill();
                Invoice inv = new Invoice(); //Factura
                Item it = new Item();// Esto son los productos
                Estimate est = new Estimate();
                Customer c = new Customer();*/

                Proveedor proveedor = new Proveedor();
                proveedor = producto.Proveedor;


                // Create a QuickBooks QueryService using ServiceContext
                QueryService<Item> querySvcI = new QueryService<Item>(serviceContext);

                List<Item> tiposProductos = querySvcI.ExecuteIdsQuery("SELECT * from Item ").Where(x => x.Name == "Vehicle Rental" && x.Type == ItemTypeEnum.Category).ToList();

                Item tipoProd = new Item();
                tipoProd = tiposProductos.First();

                List<Item> proveedores = querySvcI.ExecuteIdsQuery("SELECT * from Item ").Where(x => x.Name == proveedor.Nombre && x.Type == ItemTypeEnum.Category).ToList();
                Item prov = new Item();
                if (proveedores == null || proveedores.Count() == 0)
                {
                    prov = agregarCategoriaProveedor(tipoProd, proveedor, serviceContext);
                   Vendor ven= agregarVendorProveedor( proveedor, serviceContext);
                    if (ven == null) return Ok("Error insertando el Vendor");
                    if (prov == null) return Ok("Error insertando el proveedor");
                }
                else
                {
                    prov = proveedores.First();
                }

                QueryService<Item> querySvc1 = new QueryService<Item>(serviceContext);
                string EXISTING_ITEM_QUERYBYNAME = string.Format("select * from Item where Name = '{0}' ", producto.Nombre);
                Item objItemFound = querySvc1.ExecuteIdsQuery(EXISTING_ITEM_QUERYBYNAME).FirstOrDefault<Item>();


                if (objItemFound != null)
                {
                    if (objItemFound.Active == false)
                    {
                        objItemFound.Active = true;
                        DataService dataService1 = new DataService(serviceContext);
                        Item UpdateEntity = dataService1.Update<Item>(objItemFound);
                        if (UpdateEntity != null && !string.IsNullOrEmpty(UpdateEntity.Id))
                        {
                            //you can write Database code here
                            return Ok("Ya estaba en QB y se activo");
                        }
                        else
                        {
                            return Ok("Ya estaba en QB pero no se activo");
                        }
                    }
                    else
                    {
                        return Ok("Ya hay un producto en QB con ese nombre");
                    }



                }
                Item ObjItem = new Item();
                ObjItem.Name = producto.Nombre;
                ObjItem.ParentRef = new ReferenceType { Value = prov.Id, type = ItemTypeEnum.Category.GetStringValue(), name = prov.Name };
                ObjItem.TypeSpecified = true;
                ObjItem.Sku = producto.SKU;
                ObjItem.Type = ItemTypeEnum.Service;
                ObjItem.SubItem = true;
                ObjItem.SubItemSpecified = true;



                // Create a QuickBooks QueryService using ServiceContext for getting list of all accounts from Quickbooks
                QueryService<Account> querySvcAc = new QueryService<Account>(serviceContext);
                var AccountList = querySvcAc.ExecuteIdsQuery("SELECT * FROM Account").ToList();

                //Get Account of type "OtherCurrentAsset" and named "Inventory Asset" for Asset Account Reference
                /*  var AssetAccountRef = AccountList.Where(x => x.AccountType == AccountTypeEnum.Income && x.Name == "Ventas del Sitio").FirstOrDefault();



                  if (AssetAccountRef != null)
                  {
                      ObjItem.AssetAccountRef = new ReferenceType();
                      ObjItem.AssetAccountRef.Value = AssetAccountRef.Id;
                  }
                  else
                  {


                      return Ok("Error obteniendo la cuenta Asset");
                  }*/
                //Get Account of type "Income" and named "Sales of Product Income" for Income Account Reference
                var IncomeAccountRef = AccountList.Where(x => x.AccountType == AccountTypeEnum.Income && x.Name == "Vehicle Rental").FirstOrDefault();
                if (IncomeAccountRef != null)
                {
                    ObjItem.IncomeAccountRef = new ReferenceType();
                    ObjItem.IncomeAccountRef.Value = IncomeAccountRef.Id;
                }
                else
                {

                    return Ok("Error obteniendo la cuenta Income");
                }
                //Get Account of type "CostofGoodsSold" and named "Cost of Goods Sold" for Expense Account Reference
                var ExpenseAccountRef = AccountList.Where(x => x.AccountType == AccountTypeEnum.CostofGoodsSold && x.Name == "Vehicle Rental Booking").FirstOrDefault();
                if (ExpenseAccountRef != null)
                {
                    ObjItem.ExpenseAccountRef = new ReferenceType();
                    ObjItem.ExpenseAccountRef.Value = ExpenseAccountRef.Id;
                }
                else
                {

                    return Ok("Error obteniendo la cuenta Expense");
                }
                DataService dataService = new DataService(serviceContext);
                Item ItemAdd = dataService.Add(ObjItem);
                if (ItemAdd != null && !string.IsNullOrEmpty(ItemAdd.Id))
                {
                    //you can write Database code here

                }


                


            }
            catch (Exception ex)
            {
                return Ok("Error no determinado. El error es: " + ex.Message);
            }
            return Ok("Se insertó correctamente el producto");
        }


        [HttpGet]
        [Route("updateProductVehiculo")]
        public async System.Threading.Tasks.Task<ActionResult> UpdateProductoVehiculo([FromBody] Vehiculo producto_new)
        {
            var access_token = "";
            var realmId = "";
            Vehiculo producto = _context.Vehiculos.Include(x => x.Proveedor).First(x => x.ProductoId == producto_new.ProductoId);

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
            // QueryService<CompanyInfo> querySvc = new QueryService<CompanyInfo>(serviceContext);

            try
            {


                Proveedor proveedor = new Proveedor();
                proveedor = producto.Proveedor;


                // Create a QuickBooks QueryService using ServiceContext
                QueryService<Item> querySvcI = new QueryService<Item>(serviceContext);
                List<Item> tiposProductos = querySvcI.ExecuteIdsQuery("SELECT * from Item ").Where(x => x.Name == "Ground Transportation" && x.Type == ItemTypeEnum.Category).ToList();

                Item tipoProd = new Item();
                tipoProd = tiposProductos.First();

                List<Item> proveedores = querySvcI.ExecuteIdsQuery("SELECT * from Item ").Where(x => x.Name == proveedor.Nombre && x.Type == ItemTypeEnum.Category).ToList();
                Item prov = new Item();
                if (proveedores == null || proveedores.Count() == 0)
                {
                    prov = agregarCategoriaProveedor(tipoProd, proveedor, serviceContext);
                    Vendor ven = agregarVendorProveedor(proveedor, serviceContext);
                    if (ven == null) return Ok("Error insertando el Vendor");
                    if (prov == null) return Ok("Error insertando el proveedor");
                }
                else
                {
                    prov = proveedores.First();
                }



                QueryService<Item> querySvc1 = new QueryService<Item>(serviceContext);
                string EXISTING_ITEM_QUERYBYNAME = string.Format("select * from Item where Name = '{0}' ", producto.Nombre);
                Item objItemFound = querySvc1.ExecuteIdsQuery(EXISTING_ITEM_QUERYBYNAME).FirstOrDefault<Item>();


                if (objItemFound != null)
                {
                    Item ObjItem = new Item();
                    ObjItem.Id = objItemFound.Id;
                    ObjItem.Name = producto.Nombre;
                    ObjItem.ParentRef = new ReferenceType { Value = prov.Id, type = ItemTypeEnum.Category.GetStringValue(), name = prov.Name };
                    ObjItem.TypeSpecified = true;
                    ObjItem.Sku = producto.SKU;
                    ObjItem.Type = ItemTypeEnum.Service;
                    ObjItem.SubItem = true;
                    ObjItem.SubItemSpecified = true;


                    DataService dataService1 = new DataService(serviceContext);
                    Item UpdateEntity = dataService1.Update<Item>(objItemFound);
                    if (UpdateEntity != null && !string.IsNullOrEmpty(UpdateEntity.Id))
                    {
                        //you can write Database code here
                        return Ok("Se actualizo el producto");
                    }
                    else
                    {
                        return Ok("No se actualizo el producto");
                    }




                }



            }
            catch (Exception ex)
            {
                return Ok("Error no determinado. El error es: " + ex.Message);
            }
            return Ok("Se insertó correctamente el producto");
        }


        [HttpGet]
        [Route("deleteProductVehiculo")]
        public async System.Threading.Tasks.Task<ActionResult> DeleteProductVehiculo([FromBody] Vehiculo producto)
        {
            var access_token = "";
            var realmId = "";
            producto = _context.Vehiculos.Find(producto.ProductoId);
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

            QueryService<Item> querySvc = new QueryService<Item>(serviceContext);
            string EXISTING_ITEM_QUERYBYNAME = string.Format("select * from Item where Name = '{0}' ", producto.Nombre);
            Item objItemFound = querySvc.ExecuteIdsQuery(EXISTING_ITEM_QUERYBYNAME).FirstOrDefault<Item>();



            //If Item found on Quickbooks online
            if (objItemFound != null)
            {
                //if Item is active
                if (objItemFound.Active == true)
                {
                    objItemFound.Active = false;
                    DataService dataService = new DataService(serviceContext);
                    Item UpdateEntity = dataService.Update<Item>(objItemFound);
                    if (UpdateEntity != null && !string.IsNullOrEmpty(UpdateEntity.Id))
                    {
                        //you can write Database code here
                        return Ok("Se desactivo el producto");
                    }
                    else
                    {
                        return Ok("No se desactivo el producto");
                    }
                }
                else
                {
                    return Ok("No se desactivo el producto pues ya estaba desactivado");
                }
            }

            return Ok("No se encontro el producto ");




        }


        [HttpGet]
        [Route("addProductAlojamiento")]
        public async System.Threading.Tasks.Task<ActionResult> AddProductoAlojamiento([FromBody] Alojamiento producto)
        {
            var access_token = "";
            var realmId = "";
            producto = _context.Alojamientos.Include(x => x.Proveedor).First(x => x.ProductoId == producto.ProductoId);
            List<Habitacion> habitaciones = _context.Habitaciones.Where(x => x.ProductoId == producto.ProductoId).ToList();

            if (habitaciones == null || habitaciones.Any())
            {
                return Ok("No hay habitacione para poner en el Hotel");
            }

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

            

            try
            {
                

                Proveedor proveedor = new Proveedor();
                proveedor = producto.Proveedor;


                // Create a QuickBooks QueryService using ServiceContext
                QueryService<Item> querySvcI = new QueryService<Item>(serviceContext);
                List<Item> tiposProductos = querySvcI.ExecuteIdsQuery("SELECT * from Item ").Where(x => x.Name == "Accommodation" && x.Type == ItemTypeEnum.Category).ToList();

                Item tipoProd = new Item();
                tipoProd = tiposProductos.First();

                List<Item> proveedores = querySvcI.ExecuteIdsQuery("SELECT * from Item ").Where(x => x.Name == proveedor.Nombre && x.Type == ItemTypeEnum.Category).ToList();
                Item prov = new Item();
                if (proveedores == null || proveedores.Count() == 0)
                {
                    prov = agregarCategoriaProveedor(tipoProd, proveedor, serviceContext);
                    Vendor ven = agregarVendorProveedor(proveedor, serviceContext);
                    if (ven == null) return Ok("Error insertando el Vendor");
                    if (prov == null) return Ok("Error insertando el proveedor");
                }
                else
                {
                    prov = proveedores.First();
                }


                List<Item> hoteles = querySvcI.ExecuteIdsQuery("SELECT * from Item ").Where(x => x.Name == producto.Nombre && x.Type == ItemTypeEnum.Category).ToList();
                Item hotel = new Item();
                if (hoteles == null || hoteles.Count() > 0)
                {
                    hotel = agregarCategoriaHotel(prov, producto, serviceContext);
                    if (hotel == null) return Ok("Error insertando el proveedor");
                }
                else
                {
                    hotel = proveedores.First();
                }

                foreach (Habitacion hab in habitaciones)
                {


                    QueryService<Item> querySvc1 = new QueryService<Item>(serviceContext);
                    string EXISTING_ITEM_QUERYBYNAME = string.Format("select * from Item where Name = '{0}' ", hab.Nombre);
                    Item objItemFound = querySvc1.ExecuteIdsQuery(EXISTING_ITEM_QUERYBYNAME).FirstOrDefault<Item>();


                    if (objItemFound != null)
                    {
                        if (objItemFound.Active == false)
                        {
                            objItemFound.Active = true;
                            DataService dataService1 = new DataService(serviceContext);
                            Item UpdateEntity = dataService1.Update<Item>(objItemFound);
                            if (UpdateEntity != null && !string.IsNullOrEmpty(UpdateEntity.Id))
                            {
                                //you can write Database code here
                                return Ok("Ya estaba en QB y se activo");
                            }
                            else
                            {
                                return Ok("Ya estaba en QB pero no se activo");
                            }
                        }
                        else
                        {
                            return Ok("Ya hay un producto en QB con ese nombre");
                        }



                    }



                    Item ObjItem = new Item();
                    ObjItem.Name = hab.Nombre;
                    ObjItem.ParentRef = new ReferenceType { Value = hotel.Id, type = ItemTypeEnum.Category.GetStringValue(), name = hotel.Name };
                    ObjItem.TypeSpecified = true;
                    ObjItem.Sku = hab.SKU;
                    ObjItem.Type = ItemTypeEnum.Service;
                    ObjItem.SubItem = true;
                    ObjItem.SubItemSpecified = true;



                    // Create a QuickBooks QueryService using ServiceContext for getting list of all accounts from Quickbooks
                    QueryService<Account> querySvcAc = new QueryService<Account>(serviceContext);
                    var AccountList = querySvcAc.ExecuteIdsQuery("SELECT * FROM Account").ToList();

                    //Get Account of type "OtherCurrentAsset" and named "Inventory Asset" for Asset Account Reference
                    /*  var AssetAccountRef = AccountList.Where(x => x.AccountType == AccountTypeEnum.Income && x.Name == "Ventas del Sitio").FirstOrDefault();



                      if (AssetAccountRef != null)
                      {
                          ObjItem.AssetAccountRef = new ReferenceType();
                          ObjItem.AssetAccountRef.Value = AssetAccountRef.Id;
                      }
                      else
                      {


                          return Ok("Error obteniendo la cuenta Asset");
                      }*/
                    //Get Account of type "Income" and named "Sales of Product Income" for Income Account Reference
                    var IncomeAccountRef = AccountList.Where(x => x.AccountType == AccountTypeEnum.Income && x.Name == "Accommodation").FirstOrDefault();
                    if (IncomeAccountRef != null)
                    {
                        ObjItem.IncomeAccountRef = new ReferenceType();
                        ObjItem.IncomeAccountRef.Value = IncomeAccountRef.Id;
                    }
                    else
                    {

                        return Ok("Error obteniendo la cuenta Income");
                    }
                    //Get Account of type "CostofGoodsSold" and named "Cost of Goods Sold" for Expense Account Reference
                    var ExpenseAccountRef = AccountList.Where(x => x.AccountType == AccountTypeEnum.CostofGoodsSold && x.Name == "Accommodation Booking").FirstOrDefault();
                    if (ExpenseAccountRef != null)
                    {
                        ObjItem.ExpenseAccountRef = new ReferenceType();
                        ObjItem.ExpenseAccountRef.Value = ExpenseAccountRef.Id;
                    }
                    else
                    {

                        return Ok("Error obteniendo la cuenta Expense");
                    }
                    DataService dataService = new DataService(serviceContext);
                    Item ItemAdd = dataService.Add(ObjItem);
                    if (ItemAdd != null && !string.IsNullOrEmpty(ItemAdd.Id))
                    {
                        //you can write Database code here

                    }

                }




               


            }
            catch (Exception ex)
            {
                return Ok("Error no determinado. El error es: " + ex.Message);
            }
            return Ok("Se insertó correctamente el producto");
        }


        [HttpGet]
        [Route("updateProductAlojamiento")]
        public async System.Threading.Tasks.Task<ActionResult> UpdateProductoAlojamiento([FromBody] Alojamiento producto)
        {
            var access_token = "";
            var realmId = "";
            producto = _context.Alojamientos.Include(x => x.Proveedor).First(x => x.ProductoId == producto.ProductoId);
            List<Habitacion> habitaciones = _context.Habitaciones.Where(x => x.ProductoId == producto.ProductoId).ToList();

            if (habitaciones == null || habitaciones.Any())
            {
                return Ok("No hay habitacione para poner en el Hotel");
            }

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



            try
            {


                Proveedor proveedor = new Proveedor();
                proveedor = producto.Proveedor;


                // Create a QuickBooks QueryService using ServiceContext
                QueryService<Item> querySvcI = new QueryService<Item>(serviceContext);
                List<Item> tiposProductos = querySvcI.ExecuteIdsQuery("SELECT * from Item ").Where(x => x.Name == "Accommodation" && x.Type == ItemTypeEnum.Category).ToList();

                Item tipoProd = new Item();
                tipoProd = tiposProductos.First();

                List<Item> proveedores = querySvcI.ExecuteIdsQuery("SELECT * from Item ").Where(x => x.Name == proveedor.Nombre && x.Type == ItemTypeEnum.Category).ToList();
                Item prov = new Item();
                if (proveedores == null || proveedores.Count() == 0)
                {
                    prov = agregarCategoriaProveedor(tipoProd, proveedor, serviceContext);
                    Vendor ven = agregarVendorProveedor(proveedor, serviceContext);
                    if (ven == null) return Ok("Error insertando el Vendor");
                    if (prov == null) return Ok("Error insertando el proveedor");
                }
                else
                {
                    prov = proveedores.First();
                }


                List<Item> hoteles = querySvcI.ExecuteIdsQuery("SELECT * from Item ").Where(x => x.Name == producto.Nombre && x.Type == ItemTypeEnum.Category).ToList();
                Item hotel = new Item();
                if (hoteles == null || hoteles.Count() > 0)
                {
                    hotel = agregarCategoriaHotel(prov, producto, serviceContext);
                    if (hotel == null) return Ok("Error insertando el proveedor");
                }
                else
                {
                    hotel = proveedores.First();
                }

                foreach (Habitacion hab in habitaciones)
                {


                    QueryService<Item> querySvc1 = new QueryService<Item>(serviceContext);
                    string EXISTING_ITEM_QUERYBYNAME = string.Format("select * from Item where Name = '{0}' ", hab.Nombre);
                    Item objItemFound = querySvc1.ExecuteIdsQuery(EXISTING_ITEM_QUERYBYNAME).FirstOrDefault<Item>();


                    if (objItemFound != null)
                    {
                        Item ObjItem = new Item();
                        ObjItem.Id = objItemFound.Id;
                        ObjItem.Name = hab.Nombre;
                        ObjItem.ParentRef = new ReferenceType { Value = hotel.Id, type = ItemTypeEnum.Category.GetStringValue(), name = hotel.Name };
                        ObjItem.TypeSpecified = true;
                        ObjItem.Sku = hab.SKU;
                        ObjItem.Type = ItemTypeEnum.Service;
                        ObjItem.SubItem = true;
                        ObjItem.SubItemSpecified = true;
                        DataService dataService1 = new DataService(serviceContext);
                        Item UpdateEntity = dataService1.Update<Item>(objItemFound);
                        

                    }

                }


            }
            catch (Exception ex)
            {
                return Ok("Error no determinado. El error es: " + ex.Message);
            }
            return Ok("Se insertó correctamente el producto");
        }

        [HttpGet]
        [Route("deleteProductAlojamiento")]
        public async System.Threading.Tasks.Task<ActionResult> DeleteProductAlojamiento([FromBody] Alojamiento producto)
        {
            var access_token = "";
            var realmId = "";
            producto = _context.Alojamientos.Find(producto.ProductoId);
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

            QueryService<Item> querySvc = new QueryService<Item>(serviceContext);
            string EXISTING_ITEM_QUERYBYNAME = string.Format("select * from Item where Name = '{0}' ", producto.Nombre);
            Item objItemFound = querySvc.ExecuteIdsQuery(EXISTING_ITEM_QUERYBYNAME).FirstOrDefault<Item>();



            //If Item found on Quickbooks online
            if (objItemFound != null)
            {
                //if Item is active
                if (objItemFound.Active == true)
                {
                    objItemFound.Active = false;
                    DataService dataService = new DataService(serviceContext);
                    Item UpdateEntity = dataService.Update<Item>(objItemFound);
                    if (UpdateEntity != null && !string.IsNullOrEmpty(UpdateEntity.Id))
                    {
                        //you can write Database code here
                        return Ok("Se desactivo el producto");
                    }
                    else
                    {
                        return Ok("No se desactivo el producto");
                    }
                }
                else
                {
                    return Ok("No se desactivo el producto pues ya estaba desactivado");
                }
            }

            return Ok("No se encontro el producto ");




        }



        [HttpGet]
        [Route("createEstimated")]
        public async System.Threading.Tasks.Task<ActionResult> CreateEstimated([FromBody] Orden orden)
        {
            var access_token = "";
            var realmId = "";
            orden = _context.Orden.Include(x=>x.ListaActividadOrden)
                .Include(x => x.ListaAlojamientoOrden)
                .Include(x => x.ListaTrasladoOrden)
                .Include(x => x.ListaVehiculosOrden)
                .Include(x => x.Cliente)
                .FirstOrDefault(x=>x.OrdenId== orden.OrdenId);


           
                if (orden.ListaActividadOrden != null && orden.ListaActividadOrden.Any())
                {
                    orden.ListaActividadOrden.ForEach(x => x = _context.OrdenActividad.Include(ex => ex.PrecioActividad)/*.ThenInclude(t => t.Temporada)*/
                   .Include(d => d.Actividad)/*.ThenInclude(l => l.ListaDistribuidoresProducto)
                   .ThenInclude(l => l.Distribuidor)
                   .Include(d => d.LugarActividad)
                   .Include(d => d.LugarRecogida)
                   .Include(d => d.LugarRetorno)
                    .Include(d => d.Sobreprecio)
                     .Include(d => d.Voucher)*/
                   .First(r => r.OrdenActividadId == x.OrdenActividadId));
                    foreach (var item in orden.ListaActividadOrden)
                    {
                        if (item.PrecioActividad != null && item.PrecioActividad.Temporada != null)
                            item.PrecioActividad.Temporada.ListaRestricciones = _context.Restricciones.Where(x => x.Temporada.TemporadaId == item.PrecioActividad.Temporada.TemporadaId).ToList();

                    }
                }


                if (orden.ListaAlojamientoOrden != null && orden.ListaAlojamientoOrden.Any())
                {
                orden.ListaAlojamientoOrden.ForEach(x => x = _context.OrdenAlojamiento.Include(ex => ex.ListaPrecioAlojamientos)
                    /* .Include(d => d.Sobreprecio)*/
                      .Include(d => d.Habitacion)/*.ThenInclude(xv => xv.ListaCombinacionesDisponibles)
                      .Include(d => d.TipoHabitacion)
                     .Include(d => d.ModificadorAplicado.ListaReglas)
                     .Include(d => d.Voucher)*/
                    .Include(d => d.Alojamiento)/*.ThenInclude(l => l.ListaDistribuidoresProducto)
                    .ThenInclude(l => l.Distribuidor)*/.First(r => r.OrdenAlojamientoId == x.OrdenAlojamientoId));
                    foreach (var item in orden.ListaAlojamientoOrden)
                    {
                        if (item.ListaPrecioAlojamientos != null)
                            foreach (var pra in item.ListaPrecioAlojamientos)
                            {
                                var ordenAloPrecio = _context.OrdenAlojamientoPrecioAlojamiento.Include(x => x.PrecioAlojamiento).ThenInclude(x => x.Temporada).Include(x => x.OrdenAlojamiento).Single(x => x.OrdenAlojamientoPrecioAlojamientoId == pra.OrdenAlojamientoPrecioAlojamientoId);

                                if (ordenAloPrecio.PrecioAlojamiento != null && ordenAloPrecio.PrecioAlojamiento.Temporada != null)
                                    pra.PrecioAlojamiento.Temporada.ListaRestricciones = _context.Restricciones.Where(x => x.Temporada.TemporadaId == ordenAloPrecio.PrecioAlojamiento.Temporada.TemporadaId).ToList();
                            }


                    }
                }


                if (orden.ListaVehiculosOrden != null && orden.ListaVehiculosOrden.Any())
                {
                orden.ListaVehiculosOrden.ForEach(x => x = _context.OrdenVehiculo.Include(ex => ex.ListaPreciosRentaAutos)
                    /* .Include(d => d.Sobreprecio)
                     .Include(d => d.Voucher)
                     .Include(d => d.LugarEntrega)
                     .Include(d => d.LugarRecogida)*/
                    .Include(v => v.Vehiculo)/*.ThenInclude(l => l.ListaDistribuidoresProducto)
                    .ThenInclude(l => l.Distribuidor)*/.First(r => r.OrdenVehiculoId == x.OrdenVehiculoId));
                    foreach (var item in orden.ListaVehiculosOrden)
                    {
                        if (item.ListaPreciosRentaAutos != null)
                            foreach (var pra in item.ListaPreciosRentaAutos)
                            {
                                var ordenVehiculoPrecio = _context.OrdenVehiculoPrecioRentaAuto.Include(x => x.PrecioRentaAutos).ThenInclude(x => x.Temporada).Include(x => x.OrdenVehiculo).Single(x => x.OrdenVehiculoPrecioRentaAutoId == pra.OrdenVehiculoPrecioRentaAutoId);

                                if (ordenVehiculoPrecio.PrecioRentaAutos != null && ordenVehiculoPrecio.PrecioRentaAutos.Temporada != null)
                                    pra.PrecioRentaAutos.Temporada.ListaRestricciones = _context.Restricciones.Where(x => x.Temporada.TemporadaId == ordenVehiculoPrecio.PrecioRentaAutos.Temporada.TemporadaId).ToList();
                            }


                    }
                }


                if (orden.ListaTrasladoOrden != null && orden.ListaTrasladoOrden.Any())
                {
                    orden.ListaTrasladoOrden.ForEach(x => x = _context.OrdenTraslado.Include(ex => ex.PrecioTraslado)/*.ThenInclude(t => t.Temporada)*/
                    /*.Include(d => d.PuntoDestino)
                    .Include(d => d.PuntoOrigen)
                    .Include(d => d.Sobreprecio)
                    .Include(d => d.Voucher)*/
                    .Include(d => d.Traslado)/*.ThenInclude(l => l.ListaDistribuidoresProducto)
                    .ThenInclude(l => l.Distribuidor)*/.First(r => r.OrdenTrasladoId == x.OrdenTrasladoId));
                }

            


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

            QueryService<Customer> querySvc = new QueryService<Customer>(serviceContext);
            string EXISTING_ITEM_QUERYBYNAME = string.Format("select * from Customer where Name = '{0}' ", orden.Cliente.Nombre);
            Customer objCustomerFound = querySvc.ExecuteIdsQuery(EXISTING_ITEM_QUERYBYNAME).FirstOrDefault<Customer>();

            Estimate ObjEstimate = new Estimate();
            ObjEstimate.CustomerRef = new ReferenceType();
            ObjEstimate.CustomerRef.Value = objCustomerFound.Id; //Quickbooks online Customer Id
            
            List<Line> LineList = new List<Line>();
            if(orden.ListaActividadOrden != null && orden.ListaActividadOrden.Any())
                foreach (var item in orden.ListaActividadOrden)
                {
                    Line objLine = new Line();
                    objLine.DetailTypeSpecified = true;
                    objLine.DetailType = LineDetailTypeEnum.SalesItemLineDetail;
                    objLine.AmountSpecified = true;
                    objLine.Amount = item.PrecioOrden;
                    objLine.Description = "";//Aqui pudiera ir una descripcion de lo q va en la linea
                    SalesItemLineDetail salesItemLineDetail = new SalesItemLineDetail();
                    salesItemLineDetail.QtySpecified = true;
                    salesItemLineDetail.Qty = 1;
                    salesItemLineDetail.ItemRef = new ReferenceType();

                    QueryService<Item> querySvc1 = new QueryService<Item>(serviceContext);
                    string EXISTING_ITEM_QUERYBYNAMEITEMPROD = string.Format("select * from Item where Name = '{0}' ", item.Actividad.Nombre);
                    Item itemProduct = querySvc1.ExecuteIdsQuery(EXISTING_ITEM_QUERYBYNAMEITEMPROD).FirstOrDefault<Item>();
                    salesItemLineDetail.ItemRef.Value = itemProduct.Id; //Quickbooks online Item Id
                    objLine.AnyIntuitObject = salesItemLineDetail;
                    LineList.Add(objLine);


                }
            if (orden.ListaTrasladoOrden != null && orden.ListaTrasladoOrden.Any())
                foreach (var item in orden.ListaTrasladoOrden)
                {
                    Line objLine = new Line();
                    objLine.DetailTypeSpecified = true;
                    objLine.DetailType = LineDetailTypeEnum.SalesItemLineDetail;
                    objLine.AmountSpecified = true;
                    objLine.Amount = item.PrecioOrden;
                    objLine.Description = "";//Aqui pudiera ir una descripcion de lo q va en la linea
                    SalesItemLineDetail salesItemLineDetail = new SalesItemLineDetail();
                    salesItemLineDetail.QtySpecified = true;
                    salesItemLineDetail.Qty = 1;
                    salesItemLineDetail.ItemRef = new ReferenceType();

                    QueryService<Item> querySvc1 = new QueryService<Item>(serviceContext);
                    string EXISTING_ITEM_QUERYBYNAMEITEMPROD = string.Format("select * from Item where Name = '{0}' ", item.Traslado.Nombre);
                    Item itemProduct = querySvc1.ExecuteIdsQuery(EXISTING_ITEM_QUERYBYNAMEITEMPROD).FirstOrDefault<Item>();
                    salesItemLineDetail.ItemRef.Value = itemProduct.Id; //Quickbooks online Item Id
                    objLine.AnyIntuitObject = salesItemLineDetail;
                    LineList.Add(objLine);


                }

            if (orden.ListaVehiculosOrden != null && orden.ListaVehiculosOrden.Any())
                foreach (var item in orden.ListaVehiculosOrden)
                {
                    Line objLine = new Line();
                    objLine.DetailTypeSpecified = true;
                    objLine.DetailType = LineDetailTypeEnum.SalesItemLineDetail;
                    objLine.AmountSpecified = true;
                    objLine.Amount = item.PrecioOrden;
                    objLine.Description = "";//Aqui pudiera ir una descripcion de lo q va en la linea
                    SalesItemLineDetail salesItemLineDetail = new SalesItemLineDetail();
                    salesItemLineDetail.QtySpecified = true;
                    salesItemLineDetail.Qty = 1;
                    salesItemLineDetail.ItemRef = new ReferenceType();

                    QueryService<Item> querySvc1 = new QueryService<Item>(serviceContext);
                    string EXISTING_ITEM_QUERYBYNAMEITEMPROD = string.Format("select * from Item where Name = '{0}' ", item.Vehiculo.Nombre);
                    Item itemProduct = querySvc1.ExecuteIdsQuery(EXISTING_ITEM_QUERYBYNAMEITEMPROD).FirstOrDefault<Item>();
                    salesItemLineDetail.ItemRef.Value = itemProduct.Id; //Quickbooks online Item Id
                    objLine.AnyIntuitObject = salesItemLineDetail;
                    LineList.Add(objLine);


                }

            if (orden.ListaAlojamientoOrden != null && orden.ListaAlojamientoOrden.Any())
                foreach (var item in orden.ListaAlojamientoOrden)
                {
                    Line objLine = new Line();
                    objLine.DetailTypeSpecified = true;
                    objLine.DetailType = LineDetailTypeEnum.SalesItemLineDetail;
                    objLine.AmountSpecified = true;
                    objLine.Amount = item.PrecioOrden;
                    objLine.Description = "";//Aqui pudiera ir una descripcion de lo q va en la linea
                    SalesItemLineDetail salesItemLineDetail = new SalesItemLineDetail();
                    salesItemLineDetail.QtySpecified = true;
                    salesItemLineDetail.Qty = 1;
                    salesItemLineDetail.ItemRef = new ReferenceType();

                    QueryService<Item> querySvc1 = new QueryService<Item>(serviceContext);
                    string EXISTING_ITEM_QUERYBYNAMEITEMPROD = string.Format("select * from Item where Name = '{0}' ", item.Habitacion.Nombre);
                    Item itemProduct = querySvc1.ExecuteIdsQuery(EXISTING_ITEM_QUERYBYNAMEITEMPROD).FirstOrDefault<Item>();
                    salesItemLineDetail.ItemRef.Value = itemProduct.Id; //Quickbooks online Item Id
                    objLine.AnyIntuitObject = salesItemLineDetail;
                    LineList.Add(objLine);


                }




            ObjEstimate.Line = LineList.ToArray();
            DataService dataService = new DataService(serviceContext);
            Estimate EstimateAdd = dataService.Add(ObjEstimate);
            if (EstimateAdd != null && !string.IsNullOrEmpty(EstimateAdd.Id))
            {
                orden.IdEstimadoQB = int.Parse(EstimateAdd.Id);
                _context.Entry(orden).State = EntityState.Modified;
                _context.SaveChanges();
                //you can write Database code here
                return Ok("Se creo el estimado");
            }


            return Ok("No se encontro el producto ");




        }


        [HttpGet]
        [Route("updateEstimated")]
        public async System.Threading.Tasks.Task<ActionResult> UpdateEstimated([FromBody] Orden orden)
        {
            var access_token = "";
            var realmId = "";
            orden = _context.Orden.Include(x => x.ListaActividadOrden)
                .Include(x => x.ListaAlojamientoOrden)
                .Include(x => x.ListaTrasladoOrden)
                .Include(x => x.ListaVehiculosOrden)
                .Include(x => x.Cliente)
                .FirstOrDefault(x => x.OrdenId == orden.OrdenId);



            if (orden.ListaActividadOrden != null && orden.ListaActividadOrden.Any())
            {
                orden.ListaActividadOrden.ForEach(x => x = _context.OrdenActividad.Include(ex => ex.PrecioActividad)/*.ThenInclude(t => t.Temporada)*/
               .Include(d => d.Actividad)/*.ThenInclude(l => l.ListaDistribuidoresProducto)
                   .ThenInclude(l => l.Distribuidor)
                   .Include(d => d.LugarActividad)
                   .Include(d => d.LugarRecogida)
                   .Include(d => d.LugarRetorno)
                    .Include(d => d.Sobreprecio)
                     .Include(d => d.Voucher)*/
               .First(r => r.OrdenActividadId == x.OrdenActividadId));
                foreach (var item in orden.ListaActividadOrden)
                {
                    if (item.PrecioActividad != null && item.PrecioActividad.Temporada != null)
                        item.PrecioActividad.Temporada.ListaRestricciones = _context.Restricciones.Where(x => x.Temporada.TemporadaId == item.PrecioActividad.Temporada.TemporadaId).ToList();

                }
            }


            if (orden.ListaAlojamientoOrden != null && orden.ListaAlojamientoOrden.Any())
            {
                orden.ListaAlojamientoOrden.ForEach(x => x = _context.OrdenAlojamiento.Include(ex => ex.ListaPrecioAlojamientos)
                      /* .Include(d => d.Sobreprecio)*/
                      .Include(d => d.Habitacion)/*.ThenInclude(xv => xv.ListaCombinacionesDisponibles)
                      .Include(d => d.TipoHabitacion)
                     .Include(d => d.ModificadorAplicado.ListaReglas)
                     .Include(d => d.Voucher)*/
                    .Include(d => d.Alojamiento)/*.ThenInclude(l => l.ListaDistribuidoresProducto)
                    .ThenInclude(l => l.Distribuidor)*/.First(r => r.OrdenAlojamientoId == x.OrdenAlojamientoId));
                foreach (var item in orden.ListaAlojamientoOrden)
                {
                    if (item.ListaPrecioAlojamientos != null)
                        foreach (var pra in item.ListaPrecioAlojamientos)
                        {
                            var ordenAloPrecio = _context.OrdenAlojamientoPrecioAlojamiento.Include(x => x.PrecioAlojamiento).ThenInclude(x => x.Temporada).Include(x => x.OrdenAlojamiento).Single(x => x.OrdenAlojamientoPrecioAlojamientoId == pra.OrdenAlojamientoPrecioAlojamientoId);

                            if (ordenAloPrecio.PrecioAlojamiento != null && ordenAloPrecio.PrecioAlojamiento.Temporada != null)
                                pra.PrecioAlojamiento.Temporada.ListaRestricciones = _context.Restricciones.Where(x => x.Temporada.TemporadaId == ordenAloPrecio.PrecioAlojamiento.Temporada.TemporadaId).ToList();
                        }


                }
            }


            if (orden.ListaVehiculosOrden != null && orden.ListaVehiculosOrden.Any())
            {
                orden.ListaVehiculosOrden.ForEach(x => x = _context.OrdenVehiculo.Include(ex => ex.ListaPreciosRentaAutos)
                    /* .Include(d => d.Sobreprecio)
                     .Include(d => d.Voucher)
                     .Include(d => d.LugarEntrega)
                     .Include(d => d.LugarRecogida)*/
                    .Include(v => v.Vehiculo)/*.ThenInclude(l => l.ListaDistribuidoresProducto)
                    .ThenInclude(l => l.Distribuidor)*/.First(r => r.OrdenVehiculoId == x.OrdenVehiculoId));
                foreach (var item in orden.ListaVehiculosOrden)
                {
                    if (item.ListaPreciosRentaAutos != null)
                        foreach (var pra in item.ListaPreciosRentaAutos)
                        {
                            var ordenVehiculoPrecio = _context.OrdenVehiculoPrecioRentaAuto.Include(x => x.PrecioRentaAutos).ThenInclude(x => x.Temporada).Include(x => x.OrdenVehiculo).Single(x => x.OrdenVehiculoPrecioRentaAutoId == pra.OrdenVehiculoPrecioRentaAutoId);

                            if (ordenVehiculoPrecio.PrecioRentaAutos != null && ordenVehiculoPrecio.PrecioRentaAutos.Temporada != null)
                                pra.PrecioRentaAutos.Temporada.ListaRestricciones = _context.Restricciones.Where(x => x.Temporada.TemporadaId == ordenVehiculoPrecio.PrecioRentaAutos.Temporada.TemporadaId).ToList();
                        }


                }
            }


            if (orden.ListaTrasladoOrden != null && orden.ListaTrasladoOrden.Any())
            {
                orden.ListaTrasladoOrden.ForEach(x => x = _context.OrdenTraslado.Include(ex => ex.PrecioTraslado)/*.ThenInclude(t => t.Temporada)*/
                                                                                                                 /*.Include(d => d.PuntoDestino)
                                                                                                                 .Include(d => d.PuntoOrigen)
                                                                                                                 .Include(d => d.Sobreprecio)
                                                                                                                 .Include(d => d.Voucher)*/
                .Include(d => d.Traslado)/*.ThenInclude(l => l.ListaDistribuidoresProducto)
                    .ThenInclude(l => l.Distribuidor)*/.First(r => r.OrdenTrasladoId == x.OrdenTrasladoId));
            }




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

            string EXISTING_INVOICE_QUERYBYID = string.Format("select * from Estimate where id = '{0}'", orden.IdEstimadoQB);
            var queryService = new QueryService<Estimate>(serviceContext);
            Estimate objEstimateFound = queryService.ExecuteIdsQuery(EXISTING_INVOICE_QUERYBYID).FirstOrDefault<Estimate>();
            //If Invoice found on Quickbooks online
            if (objEstimateFound != null)
            {

                QueryService<Customer> querySvc = new QueryService<Customer>(serviceContext);
                string EXISTING_ITEM_QUERYBYNAME = string.Format("select * from Customer where Name = '{0}' ", orden.Cliente.Nombre);
                Customer objCustomerFound = querySvc.ExecuteIdsQuery(EXISTING_ITEM_QUERYBYNAME).FirstOrDefault<Customer>();

                Estimate ObjEstimate = new Estimate();
                ObjEstimate.Id = objEstimateFound.Id;
                ObjEstimate.CustomerRef = new ReferenceType();
                ObjEstimate.CustomerRef.Value = objCustomerFound.Id; //Quickbooks online Customer Id

                List<Line> LineList = new List<Line>();
                if (orden.ListaActividadOrden != null && orden.ListaActividadOrden.Any())
                    foreach (var item in orden.ListaActividadOrden)
                    {
                        Line objLine = new Line();
                        objLine.DetailTypeSpecified = true;
                        objLine.DetailType = LineDetailTypeEnum.SalesItemLineDetail;
                        objLine.AmountSpecified = true;
                        objLine.Amount = item.PrecioOrden;
                        objLine.Description = "";//Aqui pudiera ir una descripcion de lo q va en la linea
                        SalesItemLineDetail salesItemLineDetail = new SalesItemLineDetail();
                        salesItemLineDetail.QtySpecified = true;
                        salesItemLineDetail.Qty = 1;
                        salesItemLineDetail.ItemRef = new ReferenceType();

                        QueryService<Item> querySvc1 = new QueryService<Item>(serviceContext);
                        string EXISTING_ITEM_QUERYBYNAMEITEMPROD = string.Format("select * from Item where Name = '{0}' ", item.Actividad.Nombre);
                        Item itemProduct = querySvc1.ExecuteIdsQuery(EXISTING_ITEM_QUERYBYNAMEITEMPROD).FirstOrDefault<Item>();
                        salesItemLineDetail.ItemRef.Value = itemProduct.Id; //Quickbooks online Item Id
                        objLine.AnyIntuitObject = salesItemLineDetail;
                        LineList.Add(objLine);


                    }
                if (orden.ListaTrasladoOrden != null && orden.ListaTrasladoOrden.Any())
                    foreach (var item in orden.ListaTrasladoOrden)
                    {
                        Line objLine = new Line();
                        objLine.DetailTypeSpecified = true;
                        objLine.DetailType = LineDetailTypeEnum.SalesItemLineDetail;
                        objLine.AmountSpecified = true;
                        objLine.Amount = item.PrecioOrden;
                        objLine.Description = "";//Aqui pudiera ir una descripcion de lo q va en la linea
                        SalesItemLineDetail salesItemLineDetail = new SalesItemLineDetail();
                        salesItemLineDetail.QtySpecified = true;
                        salesItemLineDetail.Qty = 1;
                        salesItemLineDetail.ItemRef = new ReferenceType();

                        QueryService<Item> querySvc1 = new QueryService<Item>(serviceContext);
                        string EXISTING_ITEM_QUERYBYNAMEITEMPROD = string.Format("select * from Item where Name = '{0}' ", item.Traslado.Nombre);
                        Item itemProduct = querySvc1.ExecuteIdsQuery(EXISTING_ITEM_QUERYBYNAMEITEMPROD).FirstOrDefault<Item>();
                        salesItemLineDetail.ItemRef.Value = itemProduct.Id; //Quickbooks online Item Id
                        objLine.AnyIntuitObject = salesItemLineDetail;
                        LineList.Add(objLine);


                    }

                if (orden.ListaVehiculosOrden != null && orden.ListaVehiculosOrden.Any())
                    foreach (var item in orden.ListaVehiculosOrden)
                    {
                        Line objLine = new Line();
                        objLine.DetailTypeSpecified = true;
                        objLine.DetailType = LineDetailTypeEnum.SalesItemLineDetail;
                        objLine.AmountSpecified = true;
                        objLine.Amount = item.PrecioOrden;
                        objLine.Description = "";//Aqui pudiera ir una descripcion de lo q va en la linea
                        SalesItemLineDetail salesItemLineDetail = new SalesItemLineDetail();
                        salesItemLineDetail.QtySpecified = true;
                        salesItemLineDetail.Qty = 1;
                        salesItemLineDetail.ItemRef = new ReferenceType();

                        QueryService<Item> querySvc1 = new QueryService<Item>(serviceContext);
                        string EXISTING_ITEM_QUERYBYNAMEITEMPROD = string.Format("select * from Item where Name = '{0}' ", item.Vehiculo.Nombre);
                        Item itemProduct = querySvc1.ExecuteIdsQuery(EXISTING_ITEM_QUERYBYNAMEITEMPROD).FirstOrDefault<Item>();
                        salesItemLineDetail.ItemRef.Value = itemProduct.Id; //Quickbooks online Item Id
                        objLine.AnyIntuitObject = salesItemLineDetail;
                        LineList.Add(objLine);


                    }

                if (orden.ListaAlojamientoOrden != null && orden.ListaAlojamientoOrden.Any())
                    foreach (var item in orden.ListaAlojamientoOrden)
                    {
                        Line objLine = new Line();
                        objLine.DetailTypeSpecified = true;
                        objLine.DetailType = LineDetailTypeEnum.SalesItemLineDetail;
                        objLine.AmountSpecified = true;
                        objLine.Amount = item.PrecioOrden;
                        objLine.Description = "";//Aqui pudiera ir una descripcion de lo q va en la linea
                        SalesItemLineDetail salesItemLineDetail = new SalesItemLineDetail();
                        salesItemLineDetail.QtySpecified = true;
                        salesItemLineDetail.Qty = 1;
                        salesItemLineDetail.ItemRef = new ReferenceType();

                        QueryService<Item> querySvc1 = new QueryService<Item>(serviceContext);
                        string EXISTING_ITEM_QUERYBYNAMEITEMPROD = string.Format("select * from Item where Name = '{0}' ", item.Habitacion.Nombre);
                        Item itemProduct = querySvc1.ExecuteIdsQuery(EXISTING_ITEM_QUERYBYNAMEITEMPROD).FirstOrDefault<Item>();
                        salesItemLineDetail.ItemRef.Value = itemProduct.Id; //Quickbooks online Item Id
                        objLine.AnyIntuitObject = salesItemLineDetail;
                        LineList.Add(objLine);


                    }




                ObjEstimate.Line = LineList.ToArray();
                DataService dataService = new DataService(serviceContext);
                Estimate UpdateEntity = dataService.Update<Estimate>(ObjEstimate);
                if (UpdateEntity != null && !string.IsNullOrEmpty(UpdateEntity.Id))
                {
                   
                    //you can write Database code here
                    return Ok("Se creo el estimado");
                }

            }

            


            return Ok("No se encontro el producto ");




        }


        [HttpGet]
        [Route("createInvoice")]
        public async System.Threading.Tasks.Task<ActionResult> CreateInvoice([FromBody] Orden orden)
        {
            var access_token = "";
            var realmId = "";
            orden = _context.Orden.Include(x => x.ListaActividadOrden)
                .Include(x => x.ListaAlojamientoOrden)
                .Include(x => x.ListaTrasladoOrden)
                .Include(x => x.ListaVehiculosOrden)
                .Include(x => x.Cliente)
                .FirstOrDefault(x => x.OrdenId == orden.OrdenId);



            if (orden.ListaActividadOrden != null && orden.ListaActividadOrden.Any())
            {
                orden.ListaActividadOrden.ForEach(x => x = _context.OrdenActividad.Include(ex => ex.PrecioActividad)/*.ThenInclude(t => t.Temporada)*/
               .Include(d => d.Actividad)/*.ThenInclude(l => l.ListaDistribuidoresProducto)
                   .ThenInclude(l => l.Distribuidor)
                   .Include(d => d.LugarActividad)
                   .Include(d => d.LugarRecogida)
                   .Include(d => d.LugarRetorno)
                    .Include(d => d.Sobreprecio)
                     .Include(d => d.Voucher)*/
               .First(r => r.OrdenActividadId == x.OrdenActividadId));
                foreach (var item in orden.ListaActividadOrden)
                {
                    if (item.PrecioActividad != null && item.PrecioActividad.Temporada != null)
                        item.PrecioActividad.Temporada.ListaRestricciones = _context.Restricciones.Where(x => x.Temporada.TemporadaId == item.PrecioActividad.Temporada.TemporadaId).ToList();

                }
            }


            if (orden.ListaAlojamientoOrden != null && orden.ListaAlojamientoOrden.Any())
            {
                orden.ListaAlojamientoOrden.ForEach(x => x = _context.OrdenAlojamiento.Include(ex => ex.ListaPrecioAlojamientos)
                      /* .Include(d => d.Sobreprecio)*/
                      .Include(d => d.Habitacion)/*.ThenInclude(xv => xv.ListaCombinacionesDisponibles)
                      .Include(d => d.TipoHabitacion)
                     .Include(d => d.ModificadorAplicado.ListaReglas)
                     .Include(d => d.Voucher)*/
                    .Include(d => d.Alojamiento)/*.ThenInclude(l => l.ListaDistribuidoresProducto)
                    .ThenInclude(l => l.Distribuidor)*/.First(r => r.OrdenAlojamientoId == x.OrdenAlojamientoId));
                foreach (var item in orden.ListaAlojamientoOrden)
                {
                    if (item.ListaPrecioAlojamientos != null)
                        foreach (var pra in item.ListaPrecioAlojamientos)
                        {
                            var ordenAloPrecio = _context.OrdenAlojamientoPrecioAlojamiento.Include(x => x.PrecioAlojamiento).ThenInclude(x => x.Temporada).Include(x => x.OrdenAlojamiento).Single(x => x.OrdenAlojamientoPrecioAlojamientoId == pra.OrdenAlojamientoPrecioAlojamientoId);

                            if (ordenAloPrecio.PrecioAlojamiento != null && ordenAloPrecio.PrecioAlojamiento.Temporada != null)
                                pra.PrecioAlojamiento.Temporada.ListaRestricciones = _context.Restricciones.Where(x => x.Temporada.TemporadaId == ordenAloPrecio.PrecioAlojamiento.Temporada.TemporadaId).ToList();
                        }


                }
            }


            if (orden.ListaVehiculosOrden != null && orden.ListaVehiculosOrden.Any())
            {
                orden.ListaVehiculosOrden.ForEach(x => x = _context.OrdenVehiculo.Include(ex => ex.ListaPreciosRentaAutos)
                    /* .Include(d => d.Sobreprecio)
                     .Include(d => d.Voucher)
                     .Include(d => d.LugarEntrega)
                     .Include(d => d.LugarRecogida)*/
                    .Include(v => v.Vehiculo)/*.ThenInclude(l => l.ListaDistribuidoresProducto)
                    .ThenInclude(l => l.Distribuidor)*/.First(r => r.OrdenVehiculoId == x.OrdenVehiculoId));
                foreach (var item in orden.ListaVehiculosOrden)
                {
                    if (item.ListaPreciosRentaAutos != null)
                        foreach (var pra in item.ListaPreciosRentaAutos)
                        {
                            var ordenVehiculoPrecio = _context.OrdenVehiculoPrecioRentaAuto.Include(x => x.PrecioRentaAutos).ThenInclude(x => x.Temporada).Include(x => x.OrdenVehiculo).Single(x => x.OrdenVehiculoPrecioRentaAutoId == pra.OrdenVehiculoPrecioRentaAutoId);

                            if (ordenVehiculoPrecio.PrecioRentaAutos != null && ordenVehiculoPrecio.PrecioRentaAutos.Temporada != null)
                                pra.PrecioRentaAutos.Temporada.ListaRestricciones = _context.Restricciones.Where(x => x.Temporada.TemporadaId == ordenVehiculoPrecio.PrecioRentaAutos.Temporada.TemporadaId).ToList();
                        }


                }
            }


            if (orden.ListaTrasladoOrden != null && orden.ListaTrasladoOrden.Any())
            {
                orden.ListaTrasladoOrden.ForEach(x => x = _context.OrdenTraslado.Include(ex => ex.PrecioTraslado)/*.ThenInclude(t => t.Temporada)*/
                                                                                                                 /*.Include(d => d.PuntoDestino)
                                                                                                                 .Include(d => d.PuntoOrigen)
                                                                                                                 .Include(d => d.Sobreprecio)
                                                                                                                 .Include(d => d.Voucher)*/
                .Include(d => d.Traslado)/*.ThenInclude(l => l.ListaDistribuidoresProducto)
                    .ThenInclude(l => l.Distribuidor)*/.First(r => r.OrdenTrasladoId == x.OrdenTrasladoId));
            }




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

            QueryService<Customer> querySvc = new QueryService<Customer>(serviceContext);
            string EXISTING_ITEM_QUERYBYNAME = string.Format("select * from Customer where Name = '{0}' ", orden.Cliente.Nombre);
            Customer objCustomerFound = querySvc.ExecuteIdsQuery(EXISTING_ITEM_QUERYBYNAME).FirstOrDefault<Customer>();

            Invoice ObjInvoice = new Invoice();
            ObjInvoice.CustomerRef = new ReferenceType();
            ObjInvoice.CustomerRef.Value = objCustomerFound.Id; //Quickbooks online Customer Id
            List<Line> LineList = new List<Line>();
            if (orden.ListaActividadOrden != null && orden.ListaActividadOrden.Any())
                foreach (var item in orden.ListaActividadOrden)
                {
                    Line objLine = new Line();
                    objLine.DetailTypeSpecified = true;
                    objLine.DetailType = LineDetailTypeEnum.SalesItemLineDetail;
                    objLine.AmountSpecified = true;
                    objLine.Amount = item.PrecioOrden;
                    objLine.Description = "";//Aqui pudiera ir una descripcion de lo q va en la linea
                    SalesItemLineDetail salesItemLineDetail = new SalesItemLineDetail();
                    salesItemLineDetail.QtySpecified = true;
                    salesItemLineDetail.Qty = 1;
                    salesItemLineDetail.ItemRef = new ReferenceType();

                    QueryService<Item> querySvc1 = new QueryService<Item>(serviceContext);
                    string EXISTING_ITEM_QUERYBYNAMEITEMPROD = string.Format("select * from Item where Name = '{0}' ", item.Actividad.Nombre);
                    Item itemProduct = querySvc1.ExecuteIdsQuery(EXISTING_ITEM_QUERYBYNAMEITEMPROD).FirstOrDefault<Item>();
                    salesItemLineDetail.ItemRef.Value = itemProduct.Id; //Quickbooks online Item Id
                    objLine.AnyIntuitObject = salesItemLineDetail;
                    LineList.Add(objLine);


                }
            if (orden.ListaTrasladoOrden != null && orden.ListaTrasladoOrden.Any())
                foreach (var item in orden.ListaTrasladoOrden)
                {
                    Line objLine = new Line();
                    objLine.DetailTypeSpecified = true;
                    objLine.DetailType = LineDetailTypeEnum.SalesItemLineDetail;
                    objLine.AmountSpecified = true;
                    objLine.Amount = item.PrecioOrden;
                    objLine.Description = "";//Aqui pudiera ir una descripcion de lo q va en la linea
                    SalesItemLineDetail salesItemLineDetail = new SalesItemLineDetail();
                    salesItemLineDetail.QtySpecified = true;
                    salesItemLineDetail.Qty = 1;
                    salesItemLineDetail.ItemRef = new ReferenceType();

                    QueryService<Item> querySvc1 = new QueryService<Item>(serviceContext);
                    string EXISTING_ITEM_QUERYBYNAMEITEMPROD = string.Format("select * from Item where Name = '{0}' ", item.Traslado.Nombre);
                    Item itemProduct = querySvc1.ExecuteIdsQuery(EXISTING_ITEM_QUERYBYNAMEITEMPROD).FirstOrDefault<Item>();
                    salesItemLineDetail.ItemRef.Value = itemProduct.Id; //Quickbooks online Item Id
                    objLine.AnyIntuitObject = salesItemLineDetail;
                    LineList.Add(objLine);


                }

            if (orden.ListaVehiculosOrden != null && orden.ListaVehiculosOrden.Any())
                foreach (var item in orden.ListaVehiculosOrden)
                {                                   
                    Line objLine = new Line();
                    objLine.DetailTypeSpecified = true;
                    objLine.DetailType = LineDetailTypeEnum.SalesItemLineDetail;
                    objLine.AmountSpecified = true;
                    objLine.Amount = item.PrecioOrden;
                    objLine.Description = "";//Aqui pudiera ir una descripcion de lo q va en la linea
                    SalesItemLineDetail salesItemLineDetail = new SalesItemLineDetail();
                    salesItemLineDetail.QtySpecified = true;
                    salesItemLineDetail.Qty = 1;
                    salesItemLineDetail.ItemRef = new ReferenceType();

                    QueryService<Item> querySvc1 = new QueryService<Item>(serviceContext);
                    string EXISTING_ITEM_QUERYBYNAMEITEMPROD = string.Format("select * from Item where Name = '{0}' ", item.Vehiculo.Nombre);
                    Item itemProduct = querySvc1.ExecuteIdsQuery(EXISTING_ITEM_QUERYBYNAMEITEMPROD).FirstOrDefault<Item>();
                    salesItemLineDetail.ItemRef.Value = itemProduct.Id; //Quickbooks online Item Id
                    objLine.AnyIntuitObject = salesItemLineDetail;
                    LineList.Add(objLine);


                }

            if (orden.ListaAlojamientoOrden != null && orden.ListaAlojamientoOrden.Any())
                foreach (var item in orden.ListaAlojamientoOrden)
                {
                    Line objLine = new Line();
                    objLine.DetailTypeSpecified = true;
                    objLine.DetailType = LineDetailTypeEnum.SalesItemLineDetail;
                    objLine.AmountSpecified = true;
                    objLine.Amount = item.PrecioOrden;
                    objLine.Description = "";//Aqui pudiera ir una descripcion de lo q va en la linea
                    SalesItemLineDetail salesItemLineDetail = new SalesItemLineDetail();
                    salesItemLineDetail.QtySpecified = true;
                    salesItemLineDetail.Qty = 1;
                    salesItemLineDetail.ItemRef = new ReferenceType();

                    QueryService<Item> querySvc1 = new QueryService<Item>(serviceContext);
                    string EXISTING_ITEM_QUERYBYNAMEITEMPROD = string.Format("select * from Item where Name = '{0}' ", item.Habitacion.Nombre);
                    Item itemProduct = querySvc1.ExecuteIdsQuery(EXISTING_ITEM_QUERYBYNAMEITEMPROD).FirstOrDefault<Item>();
                    salesItemLineDetail.ItemRef.Value = itemProduct.Id; //Quickbooks online Item Id
                    objLine.AnyIntuitObject = salesItemLineDetail;
                    LineList.Add(objLine);


                }




            ObjInvoice.Line = LineList.ToArray();
            DataService dataService = new DataService(serviceContext);
            Invoice InvoiceAdd = dataService.Add(ObjInvoice);
            if (InvoiceAdd != null && !string.IsNullOrEmpty(InvoiceAdd.Id))
            {
                //you can write Database code here
                return Ok("Se creo el InvoiceAdd");
            }


            return Ok("No se encontro el InvoiceAdd ");




        }


        [HttpGet]
        [Route("createBill")]
        public async System.Threading.Tasks.Task<ActionResult> CreateBill([FromBody] Orden orden)
        {
            var access_token = "";
            var realmId = "";
            orden = _context.Orden.Include(x => x.ListaActividadOrden)
                .Include(x => x.ListaAlojamientoOrden)
                .Include(x => x.ListaTrasladoOrden)
                .Include(x => x.ListaVehiculosOrden)
                .Include(x => x.Cliente)
                .FirstOrDefault(x => x.OrdenId == orden.OrdenId);



            if (orden.ListaActividadOrden != null && orden.ListaActividadOrden.Any())
            {
                orden.ListaActividadOrden.ForEach(x => x = _context.OrdenActividad.Include(ex => ex.PrecioActividad)/*.ThenInclude(t => t.Temporada)*/
               .Include(d => d.Actividad).ThenInclude(xx=>xx.Proveedor)/*.ThenInclude(l => l.ListaDistribuidoresProducto)
                   .ThenInclude(l => l.Distribuidor)
                   .Include(d => d.LugarActividad)
                   .Include(d => d.LugarRecogida)
                   .Include(d => d.LugarRetorno)
                    .Include(d => d.Sobreprecio)
                     .Include(d => d.Voucher)*/
               .First(r => r.OrdenActividadId == x.OrdenActividadId));
                foreach (var item in orden.ListaActividadOrden)
                {
                    if (item.PrecioActividad != null && item.PrecioActividad.Temporada != null)
                        item.PrecioActividad.Temporada.ListaRestricciones = _context.Restricciones.Where(x => x.Temporada.TemporadaId == item.PrecioActividad.Temporada.TemporadaId).ToList();

                }
            }


            if (orden.ListaAlojamientoOrden != null && orden.ListaAlojamientoOrden.Any())
            {
                orden.ListaAlojamientoOrden.ForEach(x => x = _context.OrdenAlojamiento.Include(ex => ex.ListaPrecioAlojamientos)
                      /* .Include(d => d.Sobreprecio)*/
                      .Include(d => d.Habitacion)/*.ThenInclude(xv => xv.ListaCombinacionesDisponibles)
                      .Include(d => d.TipoHabitacion)
                     .Include(d => d.ModificadorAplicado.ListaReglas)
                     .Include(d => d.Voucher)*/
                    .Include(d => d.Alojamiento).ThenInclude(xx => xx.Proveedor)/*.ThenInclude(l => l.ListaDistribuidoresProducto)
                    .ThenInclude(l => l.Distribuidor)*/.First(r => r.OrdenAlojamientoId == x.OrdenAlojamientoId));
                foreach (var item in orden.ListaAlojamientoOrden)
                {
                    if (item.ListaPrecioAlojamientos != null)
                        foreach (var pra in item.ListaPrecioAlojamientos)
                        {
                            var ordenAloPrecio = _context.OrdenAlojamientoPrecioAlojamiento.Include(x => x.PrecioAlojamiento).ThenInclude(x => x.Temporada).Include(x => x.OrdenAlojamiento).Single(x => x.OrdenAlojamientoPrecioAlojamientoId == pra.OrdenAlojamientoPrecioAlojamientoId);

                            if (ordenAloPrecio.PrecioAlojamiento != null && ordenAloPrecio.PrecioAlojamiento.Temporada != null)
                                pra.PrecioAlojamiento.Temporada.ListaRestricciones = _context.Restricciones.Where(x => x.Temporada.TemporadaId == ordenAloPrecio.PrecioAlojamiento.Temporada.TemporadaId).ToList();
                        }


                }
            }


            if (orden.ListaVehiculosOrden != null && orden.ListaVehiculosOrden.Any())
            {
                orden.ListaVehiculosOrden.ForEach(x => x = _context.OrdenVehiculo.Include(ex => ex.ListaPreciosRentaAutos)
                    /* .Include(d => d.Sobreprecio)
                     .Include(d => d.Voucher)
                     .Include(d => d.LugarEntrega)
                     .Include(d => d.LugarRecogida)*/
                    .Include(v => v.Vehiculo).ThenInclude(xx => xx.Proveedor)/*.ThenInclude(l => l.ListaDistribuidoresProducto)
                    .ThenInclude(l => l.Distribuidor)*/.First(r => r.OrdenVehiculoId == x.OrdenVehiculoId));
                foreach (var item in orden.ListaVehiculosOrden)
                {
                    if (item.ListaPreciosRentaAutos != null)
                        foreach (var pra in item.ListaPreciosRentaAutos)
                        {
                            var ordenVehiculoPrecio = _context.OrdenVehiculoPrecioRentaAuto.Include(x => x.PrecioRentaAutos).ThenInclude(x => x.Temporada).Include(x => x.OrdenVehiculo).Single(x => x.OrdenVehiculoPrecioRentaAutoId == pra.OrdenVehiculoPrecioRentaAutoId);

                            if (ordenVehiculoPrecio.PrecioRentaAutos != null && ordenVehiculoPrecio.PrecioRentaAutos.Temporada != null)
                                pra.PrecioRentaAutos.Temporada.ListaRestricciones = _context.Restricciones.Where(x => x.Temporada.TemporadaId == ordenVehiculoPrecio.PrecioRentaAutos.Temporada.TemporadaId).ToList();
                        }


                }
            }


            if (orden.ListaTrasladoOrden != null && orden.ListaTrasladoOrden.Any())
            {
                orden.ListaTrasladoOrden.ForEach(x => x = _context.OrdenTraslado.Include(ex => ex.PrecioTraslado)/*.ThenInclude(t => t.Temporada)*/
                                                                                                                 /*.Include(d => d.PuntoDestino)
                                                                                                                 .Include(d => d.PuntoOrigen)
                                                                                                                 .Include(d => d.Sobreprecio)
                                                                                                                 .Include(d => d.Voucher)*/
                .Include(d => d.Traslado).ThenInclude(xx => xx.Proveedor)/*.ThenInclude(l => l.ListaDistribuidoresProducto)
                    .ThenInclude(l => l.Distribuidor)*/.First(r => r.OrdenTrasladoId == x.OrdenTrasladoId));
            }




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

           

            List<Line> LineList = new List<Line>();
            if (orden.ListaActividadOrden != null && orden.ListaActividadOrden.Any())
            {
               
                foreach (var item in orden.ListaActividadOrden)
                {
                    Bill ObjBill = new Bill();
                    QueryService<Vendor> querySvcI = new QueryService<Vendor>(serviceContext);

                    List<Vendor> proveedores = querySvcI.ExecuteIdsQuery("SELECT * from Vendor ").Where(x => x.ContactName == item.Actividad.Proveedor.Nombre).ToList();
                    Vendor VendorRef = null;
                    if (proveedores != null && proveedores.Any())
                    {

                        VendorRef = proveedores.First();
                    }
                    else
                    {
                        VendorRef = agregarVendorProveedor(item.Actividad.Proveedor, serviceContext);
                    }
                    ObjBill.VendorRef = new ReferenceType();
                    ObjBill.VendorRef.Value = VendorRef.Id;//Quickbooks online Vendor Id

                    Line objLine = new Line();
                    objLine.DetailTypeSpecified = true;
                    objLine.DetailType = LineDetailTypeEnum.AccountBasedExpenseLineDetail;
                    objLine.AmountSpecified = true;
                    objLine.Amount = item.PrecioOrden - item.ValorSobreprecioAplicado - (item.ValorSobreprecioAplicado * orden.Cliente.Descuento / 100);
                    objLine.Description = "";//Aqui pudiera ir una descripcion de lo q va en la linea
                    AccountBasedExpenseLineDetail ItemLineDetail = new AccountBasedExpenseLineDetail();
                    ItemLineDetail.AccountRef = new ReferenceType();
                    ItemLineDetail.AccountRef.Value = "78"; //Quickbooks online Account Id
                                                            // We can give Account Name insted of Account Id, if we give Account Id and Account Name both then Account name will be ignore.
                                                            //ItemLineDetail.AccountRef.name = "Purchases"; //Quickbooks online Account Name
                    objLine.AnyIntuitObject = ItemLineDetail;
                    LineList.Add(objLine);


                    ObjBill.Line = LineList.ToArray();
                    DataService dataService = new DataService(serviceContext);
                    Bill BillAdd = dataService.Add(ObjBill);
                    if (BillAdd != null && !string.IsNullOrEmpty(BillAdd.Id))
                    {

                        //you can write Database code here
                        Ok("Se creo el bill");
                    }
                    else
                    {

                        return Ok("No se encontro el producto ");
                    }
                }
               
            }
            if (orden.ListaTrasladoOrden != null && orden.ListaTrasladoOrden.Any())
            {
                foreach (var item in orden.ListaTrasladoOrden)
                {
                    Bill ObjBill = new Bill();
                    QueryService<Vendor> querySvcI = new QueryService<Vendor>(serviceContext);

                    List<Vendor> proveedores = querySvcI.ExecuteIdsQuery("SELECT * from Vendor ").Where(x => x.ContactName == item.Traslado.Proveedor.Nombre).ToList();
                    Vendor VendorRef = null;
                    if (proveedores != null && proveedores.Any())
                    {

                        VendorRef = proveedores.First();
                    }
                    else
                    {
                        VendorRef = agregarVendorProveedor(item.Traslado.Proveedor, serviceContext);
                    }
                    ObjBill.VendorRef = new ReferenceType();
                    ObjBill.VendorRef.Value = VendorRef.Id;//Quickbooks online Vendor Id
                    Line objLine = new Line();
                    objLine.DetailTypeSpecified = true;
                    objLine.DetailType = LineDetailTypeEnum.SalesItemLineDetail;
                    objLine.AmountSpecified = true;
                    objLine.Amount = item.PrecioOrden;
                    objLine.Description = "";//Aqui pudiera ir una descripcion de lo q va en la linea
                    SalesItemLineDetail salesItemLineDetail = new SalesItemLineDetail();
                    salesItemLineDetail.QtySpecified = true;
                    salesItemLineDetail.Qty = 1;
                    salesItemLineDetail.ItemRef = new ReferenceType();

                    QueryService<Item> querySvc1 = new QueryService<Item>(serviceContext);
                    string EXISTING_ITEM_QUERYBYNAMEITEMPROD = string.Format("select * from Item where Name = '{0}' ", item.Traslado.Nombre);
                    Item itemProduct = querySvc1.ExecuteIdsQuery(EXISTING_ITEM_QUERYBYNAMEITEMPROD).FirstOrDefault<Item>();
                    salesItemLineDetail.ItemRef.Value = itemProduct.Id; //Quickbooks online Item Id
                    objLine.AnyIntuitObject = salesItemLineDetail;
                    LineList.Add(objLine);

                    ObjBill.Line = LineList.ToArray();
                    DataService dataService = new DataService(serviceContext);
                    Bill BillAdd = dataService.Add(ObjBill);
                    if (BillAdd != null && !string.IsNullOrEmpty(BillAdd.Id))
                    {

                        //you can write Database code here
                        Ok("Se creo el bill");
                    }
                    else
                    {

                        return Ok("No se encontro el producto ");
                    }
                }
               
            }

            if (orden.ListaVehiculosOrden != null && orden.ListaVehiculosOrden.Any())
            {
                foreach (var item in orden.ListaVehiculosOrden)
                {
                    Bill ObjBill = new Bill();
                    QueryService<Vendor> querySvcI = new QueryService<Vendor>(serviceContext);

                    List<Vendor> proveedores = querySvcI.ExecuteIdsQuery("SELECT * from Vendor ").Where(x => x.ContactName == item.Vehiculo.Proveedor.Nombre).ToList();
                    Vendor VendorRef = null;
                    if (proveedores != null && proveedores.Any())
                    {

                        VendorRef = proveedores.First();
                    }
                    else
                    {
                        VendorRef = agregarVendorProveedor(item.Vehiculo.Proveedor, serviceContext);
                    }
                    ObjBill.VendorRef = new ReferenceType();
                    ObjBill.VendorRef.Value = VendorRef.Id;//Quickbooks online Vendor Id

                    Line objLine = new Line();
                    objLine.DetailTypeSpecified = true;
                    objLine.DetailType = LineDetailTypeEnum.SalesItemLineDetail;
                    objLine.AmountSpecified = true;
                    objLine.Amount = item.PrecioOrden;
                    objLine.Description = "";//Aqui pudiera ir una descripcion de lo q va en la linea
                    SalesItemLineDetail salesItemLineDetail = new SalesItemLineDetail();
                    salesItemLineDetail.QtySpecified = true;
                    salesItemLineDetail.Qty = 1;
                    salesItemLineDetail.ItemRef = new ReferenceType();

                    QueryService<Item> querySvc1 = new QueryService<Item>(serviceContext);
                    string EXISTING_ITEM_QUERYBYNAMEITEMPROD = string.Format("select * from Item where Name = '{0}' ", item.Vehiculo.Nombre);
                    Item itemProduct = querySvc1.ExecuteIdsQuery(EXISTING_ITEM_QUERYBYNAMEITEMPROD).FirstOrDefault<Item>();
                    salesItemLineDetail.ItemRef.Value = itemProduct.Id; //Quickbooks online Item Id
                    objLine.AnyIntuitObject = salesItemLineDetail;
                    LineList.Add(objLine);

                    ObjBill.Line = LineList.ToArray();
                    DataService dataService = new DataService(serviceContext);
                    Bill BillAdd = dataService.Add(ObjBill);
                    if (BillAdd != null && !string.IsNullOrEmpty(BillAdd.Id))
                    {

                        //you can write Database code here
                        Ok("Se creo el bill");
                    }
                    else
                    {

                        return Ok("No se encontro el producto ");
                    }

                }
               
            }
               

            if (orden.ListaAlojamientoOrden != null && orden.ListaAlojamientoOrden.Any())
            {
                foreach (var item in orden.ListaAlojamientoOrden)
                {
                    Bill ObjBill = new Bill();
                    QueryService<Vendor> querySvcI = new QueryService<Vendor>(serviceContext);

                    List<Vendor> proveedores = querySvcI.ExecuteIdsQuery("SELECT * from Vendor ").Where(x => x.ContactName == item.Alojamiento.Proveedor.Nombre).ToList();
                    Vendor VendorRef = null;
                    if (proveedores != null && proveedores.Any())
                    {

                        VendorRef = proveedores.First();
                    }
                    else
                    {
                        VendorRef = agregarVendorProveedor(item.Alojamiento.Proveedor, serviceContext);
                    }
                    ObjBill.VendorRef = new ReferenceType();
                    ObjBill.VendorRef.Value = VendorRef.Id;//Quickbooks online Vendor Id

                    Line objLine = new Line();
                    objLine.DetailTypeSpecified = true;
                    objLine.DetailType = LineDetailTypeEnum.SalesItemLineDetail;
                    objLine.AmountSpecified = true;
                    objLine.Amount = item.PrecioOrden;
                    objLine.Description = "";//Aqui pudiera ir una descripcion de lo q va en la linea
                    SalesItemLineDetail salesItemLineDetail = new SalesItemLineDetail();
                    salesItemLineDetail.QtySpecified = true;
                    salesItemLineDetail.Qty = 1;
                    salesItemLineDetail.ItemRef = new ReferenceType();

                    QueryService<Item> querySvc1 = new QueryService<Item>(serviceContext);
                    string EXISTING_ITEM_QUERYBYNAMEITEMPROD = string.Format("select * from Item where Name = '{0}' ", item.Habitacion.Nombre);
                    Item itemProduct = querySvc1.ExecuteIdsQuery(EXISTING_ITEM_QUERYBYNAMEITEMPROD).FirstOrDefault<Item>();
                    salesItemLineDetail.ItemRef.Value = itemProduct.Id; //Quickbooks online Item Id
                    objLine.AnyIntuitObject = salesItemLineDetail;
                    LineList.Add(objLine);

                    ObjBill.Line = LineList.ToArray();
                    DataService dataService = new DataService(serviceContext);
                    Bill BillAdd = dataService.Add(ObjBill);
                    if (BillAdd != null && !string.IsNullOrEmpty(BillAdd.Id))
                    {

                        //you can write Database code here
                        Ok("Se creo el bill");
                    }
                    else
                    {

                        return Ok("No se encontro el producto ");
                    }

                }
                
            }
                
           
            


            return Ok("No se encontro el producto ");




        }



        private Item agregarCategoriaHotel(Item categoriaTipoProd, Alojamiento prov, ServiceContext serviceContext)
        {

            Item ObjItem = new Item();
            ObjItem.Name = prov.Nombre;
            ObjItem.ParentRef = new ReferenceType { Value = categoriaTipoProd.Id, type = ItemTypeEnum.Category.GetStringValue(), name = categoriaTipoProd.Name };
            ObjItem.TypeSpecified = true;
            // ObjItem.Sku = producto.SKU;
            ObjItem.Type = ItemTypeEnum.Category;
            ObjItem.SubItem = true;
            ObjItem.SubItemSpecified = true;




            DataService dataService = new DataService(serviceContext);
            Item ItemAdd = dataService.Add(ObjItem);
            if (ItemAdd != null && !string.IsNullOrEmpty(ItemAdd.Id))
            {
                return ItemAdd;
            }
            return null;

        }
        private Item agregarCategoriaProveedor(Item categoriaTipoProd, Proveedor prov, ServiceContext serviceContext)
        {

            Item ObjItem = new Item();
            ObjItem.Name = prov.Nombre;
            ObjItem.ParentRef = new ReferenceType { Value = categoriaTipoProd.Id, type = ItemTypeEnum.Category.GetStringValue(), name = categoriaTipoProd.Name };
            ObjItem.TypeSpecified = true;
            // ObjItem.Sku = producto.SKU;
            ObjItem.Type = ItemTypeEnum.Category;
            ObjItem.SubItem = true;
            ObjItem.SubItemSpecified = true;




            DataService dataService = new DataService(serviceContext);
            Item ItemAdd = dataService.Add(ObjItem);
            if (ItemAdd != null && !string.IsNullOrEmpty(ItemAdd.Id))
            {
                return ItemAdd;
            }
            return null;

        }


        private Vendor agregarVendorProveedor(Proveedor prov, ServiceContext serviceContext)
        {

            QueryService<Vendor> querySvcI = new QueryService<Vendor>(serviceContext);

         

            List<Vendor> proveedores = querySvcI.ExecuteIdsQuery("SELECT * from Vendor ").Where(x => x.ContactName == prov.Nombre ).ToList();

            if(proveedores != null && proveedores.Any())
            {
                return proveedores.First();

            }

            Vendor ObjVendor = new Vendor();
            ObjVendor.GivenName = prov.Nombre;
            ObjVendor.FamilyName = prov.Nombre;
            ObjVendor.ContactName = prov.Nombre; 
            ObjVendor.CompanyName = prov.Nombre;
            EmailAddress ObjEmail = new EmailAddress();
            ObjEmail.Address = prov.Correo; 
            ObjVendor.PrimaryEmailAddr = ObjEmail;
           /* PhysicalAddress ObjAddress = new PhysicalAddress();
            ObjAddress.PostalCode = "11379";
            ObjAddress.Country = "USA";
            ObjAddress.Line1 = "51 Front Dr";
            ObjAddress.City = "New York";
            ObjVendor.BillAddr = ObjAddress;*/
            TelephoneNumber ObjTelephoneNumber = new TelephoneNumber();
            ObjTelephoneNumber.FreeFormNumber = prov.Telefono;
            ObjVendor.PrimaryPhone = ObjTelephoneNumber;
            DataService dataService = new DataService(serviceContext);
            Vendor VendorAdd = dataService.Add(ObjVendor);
            if (VendorAdd != null && !string.IsNullOrEmpty(VendorAdd.Id))
            {
                return VendorAdd;
            }
            return null;

        }






    }
}
