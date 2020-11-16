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
using OfficeOpenXml;
using OfficeOpenXml.Table;
using System.IO;
using System.Data;

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

        //code prod= AB116034176497MsLInVgUONTu5CfOkOQaSpO6Hz7ZdPYtzGVZ
        //realmid = 123145798828544
        //sand box realm id= 4620816365037572030

        //refresh token prod = AB11612143903KxSniW7eHHQBiEfIK5SaMiJ4m3CYsWXMWoFN4
        //refresh token = AB11612007615vOw3rvJTs9xcTfxejaG7fzLRuQGcfva1Bycf7

        public static string clientid = "ABtbGg86yOB32TNPcsZSaDXVSm2wBlgV89AGXiNGMJ2ja8yVCR";
        public static string clientsecret = "iOFqEfvrOsmP7lCMmyCwlAHdHaHUWg4n1PNc6sXr";
        //PRODUCCTION
        //public static string clientid = "ABIaUtlQOuizSswJSbv5bZWKPXTuiF00BNvh1TCYWXLbgdnM6P";
        //public static string clientsecret = "Qha5DJeNEhxhd6uFV8LdabImmdYEYAOAoxniZxMO";


        //public static string redirectUrl = "https://developer.intuit.com/v2/OAuth2Playground/RedirectUrl";
        //public static string redirectUrl = "http://localhost:59649/api/QBIntegracion/Responses";
        public static string redirectUrl = "http://localhost:5000/api/QBIntegracion/Responses";
        //public static string redirectUrl = "http://admin.gotravelandtours.com/publicEliecer/api/QBIntegracion/Responses";

        public static string environment = "sandbox";
        //public static string environment = "";

        public static OAuth2Client auth2Client = new OAuth2Client(clientid, clientsecret, redirectUrl, environment);

        /*Este diccionario es para almacenar los token y solamente solicitarlos una vez*/
        public static Dictionary<string, string> dictionary = new Dictionary<string, string>();

        public static string QboBaseUrl = "https://sandbox-quickbooks.api.intuit.com/";
        //public static string QboBaseUrl = "https://quickbooks.api.intuit.com/";

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

            return Redirect(authorizeUrl);

        }


        [HttpGet]
        [Route("Responses")]
        public async System.Threading.Tasks.Task<ActionResult> ApiCallService(string realmId, string code)
        {
            var principal = User as ClaimsPrincipal;


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


        [HttpPost]
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
                    return Ok(new { token = "Error Cargando el Token" });
                }

                access_token = dictionary["accessToken"];
                realmId = dictionary["realmId"];
            }
            catch (Exception ex)
            {
                return Ok(new { token = "Error connectandose a QB" });
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
                    return Ok("Se creo la categoria");
                }
                else
                {
                    return Ok("No se creo la categoria");
                }


            }
            else
            {
                objItemFound.Name = new_nombre;
                objItemFound.TypeSpecified = true;
                // ObjItem.Sku = producto.SKU;
                objItemFound.Type = ItemTypeEnum.Category;
                objItemFound.SubItem = true;
                objItemFound.SubItemSpecified = true;


                DataService dataService = new DataService(serviceContext);
                Item ItemAdd = dataService.Update(objItemFound);
                if (ItemAdd != null && !string.IsNullOrEmpty(ItemAdd.Id))
                {
                    return Ok("Se actualizo la categoria");
                }
                else
                {
                    return Ok("No se actualizo la categoria");
                }
            }

            //return Ok("No se encontro la categoria");
        }




        [HttpPost]
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
                    return BadRequest(new { token = "Error Cargando el Token" });
                }





                access_token = dictionary["accessToken"];
                realmId = dictionary["realmId"];
            }
            catch (Exception ex)
            {


                return BadRequest(new { token = "Error connectandose a QB" });
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
                //ObjItem.Title = cliente.Nombre;
                ObjItem.PrimaryEmailAddr = new EmailAddress { Address = cliente.Correo };
                ObjItem.AlternatePhone = new TelephoneNumber { DeviceType = "LandLine", FreeFormNumber = cliente.Telefono };
                ObjItem.Mobile = new TelephoneNumber { DeviceType = "Mobile", FreeFormNumber = cliente.Telefono };
                ObjItem.PrimaryPhone = new TelephoneNumber { DeviceType = "Mobile", FreeFormNumber = cliente.Telefono };



                DataService dataService = new DataService(serviceContext);
                Customer customer = dataService.Add(ObjItem);
                if (customer != null && !string.IsNullOrEmpty(customer.Id))
                {
                    cliente.IdQB = int.Parse(customer.Id);
                    _context.Entry(cliente).State = EntityState.Modified;
                    _context.SaveChanges();

                    return Ok(new { token = "Se inserto el cliente" });

                }
                else
                {
                    Ok(new { token = "No se inserto el cliente" });

                }



            }
            catch (Exception ex)
            {
                return BadRequest(new { token = "Exception : " + ex.Message });
            }

            return Ok(new { token = "Se inserto el cliente" });

        }

        [HttpPost]
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
                    return Ok(new { token = "Error Cargando el Token" });
                }





                access_token = dictionary["accessToken"];
                realmId = dictionary["realmId"];
            }
            catch (Exception ex)
            {


                return Ok(new { token = "Error connectandose a QB" });
            }



            OAuth2RequestValidator oauthValidator = new OAuth2RequestValidator(access_token);
            // Create a ServiceContext with Auth tokens and realmId
            ServiceContext serviceContext = new ServiceContext(realmId, IntuitServicesType.QBO, oauthValidator);
            serviceContext.IppConfiguration.MinorVersion.Qbo = "23";
            serviceContext.IppConfiguration.BaseUrl.Qbo = QboBaseUrl;

            QueryService<Customer> querySvc = new QueryService<Customer>(serviceContext);
            string EXISTING_ITEM_QUERYBYNAME = string.Format("select * from Customer where id = '{0}' ", cliente.IdQB);
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

                        cliente.IdQB = int.Parse(UpdateEntity.Id);
                        _context.Entry(cliente).State = EntityState.Modified;
                        _context.SaveChanges();
                        //you can write Database code here
                        return Ok(new { token = "Se desactivo el cliente" });
                    }
                    else
                    {
                        return Ok(new { token = "No se desactivo el cliente" });
                    }
                }
                else
                {
                    return Ok(new { token = "No se desactivo el cliente pues ya estaba desactivado" });
                }
            }

            return Ok(new { token = "No se encontro el cliente " });




        }


        [HttpPost]
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
                    return BadRequest(new { token = "Error Cargando el Token" });
                }





                access_token = dictionary["accessToken"];
                realmId = dictionary["realmId"];
            }
            catch (Exception ex)
            {


                return BadRequest(new { token = "Error connectandose a QB" });
            }



            OAuth2RequestValidator oauthValidator = new OAuth2RequestValidator(access_token);
            // Create a ServiceContext with Auth tokens and realmId
            ServiceContext serviceContext = new ServiceContext(realmId, IntuitServicesType.QBO, oauthValidator);
            serviceContext.IppConfiguration.MinorVersion.Qbo = "23";
            serviceContext.IppConfiguration.BaseUrl.Qbo = QboBaseUrl;

            QueryService<Customer> querySvc = new QueryService<Customer>(serviceContext);
            string EXISTING_ITEM_QUERYBYNAME = string.Format("select * from Customer where ID= '{0}' ", cliente.IdQB);
            if (cliente.IdQB == 0)
            {
                 EXISTING_ITEM_QUERYBYNAME = string.Format("select * from Customer where DisplayName= '{0} ", cliente.Nombre.Trim());
            }
            Customer objItemFound = querySvc.ExecuteIdsQuery(EXISTING_ITEM_QUERYBYNAME).FirstOrDefault<Customer>();



            if (objItemFound != null)
            {
                Customer ObjItem = new Customer();
                objItemFound.Id = objItemFound.Id;
                objItemFound.DisplayName = cliente.Nombre;
                objItemFound.FamilyName = cliente.Nombre;
                objItemFound.GivenName = cliente.Nombre;
                objItemFound.ContactName = cliente.Nombre;
                objItemFound.Title = cliente.Nombre;
                objItemFound.PrimaryEmailAddr = new EmailAddress { Address = cliente.Correo };
                objItemFound.AlternatePhone = new TelephoneNumber { DeviceType = "LandLine", FreeFormNumber = cliente.Telefono };
                objItemFound.Mobile = new TelephoneNumber { DeviceType = "Mobile", FreeFormNumber = cliente.Telefono };
                objItemFound.PrimaryPhone = new TelephoneNumber { DeviceType = "Mobile", FreeFormNumber = cliente.Telefono };
                DataService dataService = new DataService(serviceContext);
                Customer UpdateEntity = dataService.Update<Customer>(objItemFound);
                if (UpdateEntity != null && !string.IsNullOrEmpty(UpdateEntity.Id))
                {

                    cliente.IdQB = int.Parse(UpdateEntity.Id);
                    _context.Entry(cliente).State = EntityState.Modified;
                    _context.SaveChanges();
                    //you can write Database code here
                    return Ok(new { token = "Se actualizo el cliente " });
                }
                else
                {
                    return BadRequest(new { token = "No se actualizo el cliente " });
                }


            }

            return Ok(new { token = "No se encontro el cliente " });




        }


        [HttpPost]
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
                    return BadRequest(new { token = "Error Cargando el Token" });
                }





                access_token = dictionary["accessToken"];
                realmId = dictionary["realmId"];
            }
            catch (Exception ex)
            {


                return BadRequest(new { token = "Error connectandose a QB" });
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

                List<Item> tiposProductos = querySvcI.ExecuteIdsQuery(string.Format("SELECT * from Item where Name = '{0}' and Type = '{1}' ", "Ground Transportation", ItemTypeEnum.Category)).ToList();


                Item tipoProd = new Item();
                tipoProd = tiposProductos.First();

                List<Item> proveedores = querySvcI.ExecuteIdsQuery(string.Format("SELECT * from Item where Name = '{0}' and Type = '{1}' ", proveedor.Nombre, ItemTypeEnum.Category)).ToList();

                Item prov = new Item();
                if (proveedores == null || proveedores.Count() == 0)
                {
                    prov = agregarCategoriaProveedor(tipoProd, proveedor, serviceContext);
                    Vendor ven = agregarVendorProveedor(proveedor, serviceContext);
                    if (ven == null) return Ok(new { token = "Error insertando el Vendor" });
                    if (prov == null) return Ok(new { token = "Error insertando el proveedor" });
                }
                else
                {

                    if (proveedores.Where(x => x.FullyQualifiedName.Contains("Ground Transportation")).ToList().Count > 0)
                    {
                        prov = proveedores.First(x => x.FullyQualifiedName.Contains("Ground Transportation"));
                        proveedor.IdQB = int.Parse(prov.Id);
                        _context.Entry(proveedor).State = EntityState.Modified;
                        _context.SaveChanges();
                    }
                    else

                    {
                        prov = agregarCategoriaProveedor(tipoProd, proveedor, serviceContext);
                        Vendor ven = agregarVendorProveedor(proveedor, serviceContext);
                        if (ven == null) return Ok(new { token = "Error insertando el Vendor" });
                        if (prov == null) return Ok(new { token = "Error insertando el proveedor" });
                    }

                }



                QueryService<Item> querySvc1 = new QueryService<Item>(serviceContext);
                string EXISTING_ITEM_QUERYBYNAME = string.Format("select * from Item where Sku = '{0}' ", producto.SKU);
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
                            producto.IdQB = int.Parse(UpdateEntity.Id);
                            _context.Entry(producto).State = EntityState.Modified;
                            _context.SaveChanges();

                            //you can write Database code here
                            return Ok(new { token = "Ya estaba en QB y se activo" });
                        }
                        else
                        {
                            return Ok(new { token = "Ya estaba en QB pero no se activo" });
                        }
                    }
                    else
                    {
                        return Ok(new { token = "Ya hay un producto en QB con ese nombre" });
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

                    return Ok(new { token = "Error obteniendo la cuenta Income" });
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

                    return Ok(new { token = "Error obteniendo la cuenta Expense" });
                }
                DataService dataService = new DataService(serviceContext);
                Item ItemAdd = dataService.Add(ObjItem);
                if (ItemAdd != null && !string.IsNullOrEmpty(ItemAdd.Id))
                {
                    producto.IdQB = int.Parse(ItemAdd.Id);
                    _context.Entry(producto).State = EntityState.Modified;
                    _context.SaveChanges();
                    //you can write Database code here

                }


                /*string output = JsonConvert.SerializeObject(companyInfo, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });*/


            }
            catch (Exception ex)
            {
                return Ok(new { token = "Error no determinado. El error es: " + ex.Message });
            }
            return Ok(new { token = "Se insertó correctamente el producto" });
        }


        [HttpPost]
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
                    return Ok(new { token = "Error Cargando el Token" });
                }





                access_token = dictionary["accessToken"];
                realmId = dictionary["realmId"];
            }
            catch (Exception ex)
            {


                return Ok(new { token = "Error connectandose a QB" });
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

                List<Item> tiposProductos = querySvcI.ExecuteIdsQuery(string.Format("SELECT * from Item where Name = '{0}' and Type = '{1}' ", "Ground Transportation", ItemTypeEnum.Category)).ToList();


                Item tipoProd = new Item();
                tipoProd = tiposProductos.First();

                List<Item> proveedores = querySvcI.ExecuteIdsQuery(string.Format("SELECT * from Item where Name = '{0}' and Type = '{1}' ", proveedor.Nombre, ItemTypeEnum.Category)).ToList();
                Item prov = new Item();
                if (proveedores == null || proveedores.Count() == 0)
                {
                    prov = agregarCategoriaProveedor(tipoProd, proveedor, serviceContext);
                    Vendor ven = agregarVendorProveedor(proveedor, serviceContext);
                    if (ven == null) return Ok(new { token = "Error insertando el Vendor" });
                    if (prov == null) return Ok(new { token = "Error insertando el proveedor" });
                }
                else
                {
                    if (proveedores.Where(x => x.FullyQualifiedName.Contains("Ground Transportation")).ToList().Count > 0)
                    {
                        prov = proveedores.First(x => x.FullyQualifiedName.Contains("Ground Transportation"));
                        proveedor.IdQB = int.Parse(prov.Id);
                        _context.Entry(proveedor).State = EntityState.Modified;
                        _context.SaveChanges();
                    }
                    else

                    {
                        prov = agregarCategoriaProveedor(tipoProd, proveedor, serviceContext);
                        Vendor ven = agregarVendorProveedor(proveedor, serviceContext);
                        if (ven == null) return Ok(new { token = "Error insertando el Vendor" });
                        if (prov == null) return Ok(new { token = "Error insertando el proveedor" });
                    }
                }

                if (producto.IdQB == null)
                {
                    producto.IdQB = 0;
                }

                QueryService<Item> querySvc1 = new QueryService<Item>(serviceContext);
                string EXISTING_ITEM_QUERYBYNAME = string.Format("select * from Item where Id = '{0}' ", producto.IdQB);
                Item objItemFound = querySvc1.ExecuteIdsQuery(EXISTING_ITEM_QUERYBYNAME).FirstOrDefault<Item>();


                if (objItemFound == null)
                {
                    EXISTING_ITEM_QUERYBYNAME = string.Format("select * from Item where sku = '{0}' ", producto.SKU);
                    objItemFound = querySvc1.ExecuteIdsQuery(EXISTING_ITEM_QUERYBYNAME).FirstOrDefault<Item>();
                }

                if (objItemFound != null)
                {
                    Item ObjItem = new Item();

                    objItemFound.Id = objItemFound.Id;
                    objItemFound.Name = producto.Nombre;
                    objItemFound.ParentRef = new ReferenceType { Value = prov.Id, type = ItemTypeEnum.Category.GetStringValue(), name = prov.Name };
                    objItemFound.TypeSpecified = true;
                    objItemFound.Sku = producto.SKU;
                    objItemFound.Type = ItemTypeEnum.Service;
                    objItemFound.SubItem = true;
                    objItemFound.SubItemSpecified = true;
                    objItemFound.Active = true;

                    DataService dataService1 = new DataService(serviceContext);
                    Item UpdateEntity = dataService1.Update<Item>(objItemFound);
                    if (UpdateEntity != null && !string.IsNullOrEmpty(UpdateEntity.Id))
                    {
                        producto.IdQB = int.Parse(UpdateEntity.Id);
                        _context.Entry(producto).State = EntityState.Modified;
                        _context.SaveChanges();


                        //you can write Database code here
                        return Ok(new { token = "Se actualizo el producto" });
                    }
                    else
                    {
                        return Ok(new { token = "No se actualizo el producto" });
                    }




                }



            }
            catch (Exception ex)
            {
                return Ok(new { token = "Error no determinado. El error es: " + ex.Message });
            }
            return Ok(new { token = "Se insertó correctamente el producto" });
        }

        [HttpPost]
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
                    return Ok(new { token = "Error Cargando el Token" });
                }





                access_token = dictionary["accessToken"];
                realmId = dictionary["realmId"];
            }
            catch (Exception ex)
            {


                return Ok(new { token = "Error connectandose a QB" });
            }



            OAuth2RequestValidator oauthValidator = new OAuth2RequestValidator(access_token);
            // Create a ServiceContext with Auth tokens and realmId
            ServiceContext serviceContext = new ServiceContext(realmId, IntuitServicesType.QBO, oauthValidator);
            serviceContext.IppConfiguration.MinorVersion.Qbo = "23";
            serviceContext.IppConfiguration.BaseUrl.Qbo = QboBaseUrl;

            QueryService<Item> querySvc = new QueryService<Item>(serviceContext);
            string EXISTING_ITEM_QUERYBYNAME = string.Format("select * from Item where Id = '{0}' ", traslado.IdQB);
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
                        return Ok(new { token = "Se desactivo el producto" });
                    }
                    else
                    {
                        return Ok(new { token = "No se desactivo el producto" });
                    }
                }
                else
                {
                    return Ok(new { token = "No se desactivo el producto pues ya estaba desactivado" });
                }
            }

            return Ok(new { token = "No se encontro el producto" });




        }



        [HttpPost]
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
                    return Ok(new { token = "Error Cargando el Token" });
                }





                access_token = dictionary["accessToken"];
                realmId = dictionary["realmId"];
            }
            catch (Exception ex)
            {


                return Ok(new { token = "Error connectandose a QB" });
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

                List<Item> tiposProductos = querySvcI.ExecuteIdsQuery(string.Format("SELECT * from Item where Name = '{0}' and Type = '{1}' ", "Activity", ItemTypeEnum.Category)).ToList();


                Item tipoProd = new Item();
                tipoProd = tiposProductos.First();

                List<Item> proveedores = querySvcI.ExecuteIdsQuery(string.Format("SELECT * from Item where Name = '{0}' and Type = '{1}' ", proveedor.Nombre, ItemTypeEnum.Category)).ToList();
                Item prov = new Item();
                if (proveedores == null || proveedores.Count() == 0)
                {
                    prov = agregarCategoriaProveedor(tipoProd, proveedor, serviceContext);
                    Vendor ven = agregarVendorProveedor(proveedor, serviceContext);
                    if (ven == null) return Ok(new { token = "Error insertando el Vendor" });
                    if (prov == null) return Ok(new { token = "Error insertando el proveedor" });
                }
                else
                {
                    if (proveedores.Where(x => x.FullyQualifiedName.Contains("Activity")).ToList().Count > 0)
                    {
                        prov = proveedores.First(x => x.FullyQualifiedName.Contains("Activity"));
                        proveedor.IdQB = int.Parse(prov.Id);
                        _context.Entry(proveedor).State = EntityState.Modified;
                        _context.SaveChanges();
                    }
                    else

                    {
                        prov = agregarCategoriaProveedor(tipoProd, proveedor, serviceContext);
                        Vendor ven = agregarVendorProveedor(proveedor, serviceContext);
                        if (ven == null) return Ok(new { token = "Error insertando el Vendor" });
                        if (prov == null) return Ok(new { token = "Error insertando el proveedor" });
                    }
                }


                QueryService<Item> querySvc1 = new QueryService<Item>(serviceContext);
                string EXISTING_ITEM_QUERYBYNAME = string.Format("select * from Item where Sku = '{0}' ", producto.SKU);
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
                            producto.IdQB = int.Parse(UpdateEntity.Id);
                            _context.Entry(producto).State = EntityState.Modified;
                            _context.SaveChanges();

                            //you can write Database code here
                            return Ok(new { token = "Ya estaba en QB y se activo" });
                        }
                        else
                        {
                            return Ok(new { token = "Ya estaba en QB pero no se activo" });
                        }
                    }
                    else
                    {
                        return Ok(new { token = "Ya hay un producto en QB con ese nombre" });
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

                    return Ok(new { token = "Error obteniendo la cuenta Income" });
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

                    return Ok(new { token = "Error obteniendo la cuenta Expense" });
                }
                DataService dataService = new DataService(serviceContext);
                Item ItemAdd = dataService.Add(ObjItem);
                if (ItemAdd != null && !string.IsNullOrEmpty(ItemAdd.Id))
                {
                    producto.IdQB = int.Parse(ItemAdd.Id);
                    _context.Entry(producto).State = EntityState.Modified;
                    _context.SaveChanges();
                    //you can write Database code here

                }





            }
            catch (Exception ex)
            {
                return Ok(new { token = "Error no determinado. El error es: " + ex.Message });
            }
            return Ok(new { token = "Se insertó correctamente el producto" });
        }



        [HttpPost]
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
                    return Ok(new { token = "Error Cargando el Token" });
                }





                access_token = dictionary["accessToken"];
                realmId = dictionary["realmId"];
            }
            catch (Exception ex)
            {


                return Ok(new { token = "Error connectandose a QB" });
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

                List<Item> tiposProductos = querySvcI.ExecuteIdsQuery(string.Format("SELECT * from Item where Name = '{0}' and Type = '{1}' ", "Activity", ItemTypeEnum.Category)).ToList();


                Item tipoProd = new Item();
                tipoProd = tiposProductos.First();

                List<Item> proveedores = querySvcI.ExecuteIdsQuery(string.Format("SELECT * from Item where Name = '{0}' and Type = '{1}' ", proveedor.Nombre, ItemTypeEnum.Category)).ToList();
                Item prov = new Item();
                if (proveedores == null || proveedores.Count() == 0)
                {
                    prov = agregarCategoriaProveedor(tipoProd, proveedor, serviceContext);
                    Vendor ven = agregarVendorProveedor(proveedor, serviceContext);
                    if (ven == null) return Ok(new { token = "Error insertando el Vendor" });
                    if (prov == null) return Ok(new { token = "Error insertando el proveedor" });
                }
                else
                {
                    if (proveedores.Where(x => x.FullyQualifiedName.Contains("Activity")).ToList().Count > 0)
                    {
                        prov = proveedores.First(x => x.FullyQualifiedName.Contains("Activity"));
                        proveedor.IdQB = int.Parse(prov.Id);
                        _context.Entry(proveedor).State = EntityState.Modified;
                        _context.SaveChanges();
                    }
                    else

                    {
                        prov = agregarCategoriaProveedor(tipoProd, proveedor, serviceContext);
                        Vendor ven = agregarVendorProveedor(proveedor, serviceContext);
                        if (ven == null) return Ok(new { token = "Error insertando el Vendor" });
                        if (prov == null) return Ok(new { token = "Error insertando el proveedor" });
                    }
                }

                if (producto.IdQB == null)
                {
                    producto.IdQB = 0;
                }

                QueryService<Item> querySvc1 = new QueryService<Item>(serviceContext);
                string EXISTING_ITEM_QUERYBYNAME = string.Format("select * from Item where Id = '{0}' ", producto.IdQB);
                Item objItemFound = querySvc1.ExecuteIdsQuery(EXISTING_ITEM_QUERYBYNAME).FirstOrDefault<Item>();

                if (objItemFound == null)
                {
                    EXISTING_ITEM_QUERYBYNAME = string.Format("select * from Item where sku = '{0}' ", producto.SKU);
                    objItemFound = querySvc1.ExecuteIdsQuery(EXISTING_ITEM_QUERYBYNAME).FirstOrDefault<Item>();
                }

                if (objItemFound != null)
                {
                    Item ObjItem = new Item();
                    objItemFound.Id = objItemFound.Id;
                    objItemFound.Name = producto.Nombre;
                    objItemFound.ParentRef = new ReferenceType { Value = prov.Id, type = ItemTypeEnum.Category.GetStringValue(), name = prov.Name };
                    objItemFound.TypeSpecified = true;
                    objItemFound.Sku = producto.SKU;
                    objItemFound.Type = ItemTypeEnum.Service;
                    objItemFound.SubItem = true;
                    objItemFound.SubItemSpecified = true;
                    objItemFound.Active = true;

                    DataService dataService1 = new DataService(serviceContext);
                    Item UpdateEntity = dataService1.Update<Item>(objItemFound);
                    if (UpdateEntity != null && !string.IsNullOrEmpty(UpdateEntity.Id))
                    {
                        producto.IdQB = int.Parse(UpdateEntity.Id);
                        _context.Entry(producto).State = EntityState.Modified;
                        _context.SaveChanges();

                        //you can write Database code here
                        return Ok(new { token = "Se actualizo el producto" });
                    }
                    else
                    {
                        return Ok(new { token = "No se actualizo el producto" });
                    }




                }



            }
            catch (Exception ex)
            {
                return Ok(new { token = "Error no determinado. El error es: " + ex.Message });
            }
            return Ok(new { token = "Se insertó correctamente el producto" });
        }


        [HttpPost]
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
                    return Ok(new { token = "Error Cargando el Token" });
                }





                access_token = dictionary["accessToken"];
                realmId = dictionary["realmId"];
            }
            catch (Exception ex)
            {


                return Ok(new { token = "Error connectandose a QB" });
            }



            OAuth2RequestValidator oauthValidator = new OAuth2RequestValidator(access_token);
            // Create a ServiceContext with Auth tokens and realmId
            ServiceContext serviceContext = new ServiceContext(realmId, IntuitServicesType.QBO, oauthValidator);
            serviceContext.IppConfiguration.MinorVersion.Qbo = "23";
            serviceContext.IppConfiguration.BaseUrl.Qbo = QboBaseUrl;

            QueryService<Item> querySvc = new QueryService<Item>(serviceContext);
            string EXISTING_ITEM_QUERYBYNAME = string.Format("select * from Item where Id = '{0}' ", producto.IdQB);
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
                        return Ok(new { token = "Se desactivo el producto" });
                    }
                    else
                    {
                        return Ok(new { token = "No se desactivo el producto" });
                    }
                }
                else
                {
                    return Ok(new { token = "No se desactivo el producto pues ya estaba desactivado" });
                }
            }

            return Ok(new { token = "No se encontro el producto" });




        }



        [HttpPost]
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
                    return Ok(new { token = "Error Cargando el Token" });
                }





                access_token = dictionary["accessToken"];
                realmId = dictionary["realmId"];
            }
            catch (Exception ex)
            {


                return Ok(new { token = "Error connectandose a QB" });
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

                List<Item> tiposProductos = querySvcI.ExecuteIdsQuery(string.Format("SELECT * from Item where Name = '{0}' and Type = '{1}' ", "Vehicle Rental", ItemTypeEnum.Category)).ToList();

                Item tipoProd = new Item();
                tipoProd = tiposProductos.First();

                List<Item> proveedores = querySvcI.ExecuteIdsQuery(string.Format("SELECT * from Item where Name = '{0}' and Type = '{1}' ", proveedor.Nombre, ItemTypeEnum.Category)).ToList();
                Item prov = new Item();
                if (proveedores == null || proveedores.Count() == 0)
                {
                    prov = agregarCategoriaProveedor(tipoProd, proveedor, serviceContext);
                    Vendor ven = agregarVendorProveedor(proveedor, serviceContext);
                    if (ven == null) return Ok(new { token = "Error insertando el Vendor" });
                    if (prov == null) return Ok(new { token = "Error insertando el proveedor" });
                }
                else
                {
                    if (proveedores.Where(x => x.FullyQualifiedName.Contains("Vehicle Rental")).ToList().Count > 0)
                    {
                        prov = proveedores.First(x => x.FullyQualifiedName.Contains("Vehicle Rental"));
                        proveedor.IdQB = int.Parse(prov.Id);
                        _context.Entry(proveedor).State = EntityState.Modified;
                        _context.SaveChanges();
                    }
                    else
                    {
                        prov = agregarCategoriaProveedor(tipoProd, proveedor, serviceContext);
                        Vendor ven = agregarVendorProveedor(proveedor, serviceContext);
                        if (ven == null) return Ok(new { token = "Error insertando el Vendor" });
                        if (prov == null) return Ok(new { token = "Error insertando el proveedor" });
                    }
                }

                QueryService<Item> querySvc1 = new QueryService<Item>(serviceContext);
                string EXISTING_ITEM_QUERYBYNAME = string.Format("select * from Item where Sku = '{0}' ", producto.SKU);
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
                            producto.IdQB = int.Parse(UpdateEntity.Id);
                            _context.Entry(producto).State = EntityState.Modified;
                            _context.SaveChanges();
                            //you can write Database code here
                            return Ok(new { token = "Ya estaba en QB y se activo" });
                        }
                        else
                        {
                            return Ok(new { token = "Ya estaba en QB pero no se activo" });
                        }
                    }
                    else
                    {
                        return Ok(new { token = "Ya hay un producto en QB con ese nombre" });
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

                    return Ok(new { token = "Error obteniendo la cuenta Income" });
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

                    return Ok(new { token = "Error obteniendo la cuenta Expense" });
                }
                DataService dataService = new DataService(serviceContext);
                Item ItemAdd = dataService.Add(ObjItem);
                if (ItemAdd != null && !string.IsNullOrEmpty(ItemAdd.Id))
                {
                    producto.IdQB = int.Parse(ItemAdd.Id);
                    _context.Entry(producto).State = EntityState.Modified;
                    _context.SaveChanges();
                    //you can write Database code here

                }





            }
            catch (Exception ex)
            {
                return Ok(new { token = "Error no determinado. El error es: " + ex.Message });
            }
            // return Ok(new { token = "Se insertó correctamente el producto" });
            return Ok(new { token = "Se insertó correctamente el producto" });
        }


        [HttpPost]
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
                    return Ok(new { token = "Error Cargando el Token" });
                }





                access_token = dictionary["accessToken"];
                realmId = dictionary["realmId"];
            }
            catch (Exception ex)
            {


                return Ok(new { token = "Error connectandose a QB" });
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

                List<Item> tiposProductos = querySvcI.ExecuteIdsQuery(string.Format("SELECT * from Item where Name = '{0}' and Type = '{1}' ", "Vehicle Rental", ItemTypeEnum.Category)).ToList();

                Item tipoProd = new Item();
                tipoProd = tiposProductos.First();

                List<Item> proveedores = querySvcI.ExecuteIdsQuery(string.Format("SELECT * from Item where Name = '{0}' and Type = '{1}' ", proveedor.Nombre, ItemTypeEnum.Category)).ToList();
                Item prov = new Item();
                if (proveedores == null || proveedores.Count() == 0)
                {
                    prov = agregarCategoriaProveedor(tipoProd, proveedor, serviceContext);
                    Vendor ven = agregarVendorProveedor(proveedor, serviceContext);
                    if (ven == null) return Ok(new { token = "Error insertando el Vendor" });
                    if (prov == null) return Ok(new { token = "Error insertando el proveedor" });
                }
                else
                {
                    if (proveedores.Where(x => x.FullyQualifiedName.Contains("Vehicle Rental")).ToList().Count > 0)
                    {
                        prov = proveedores.First(x => x.FullyQualifiedName.Contains("Vehicle Rental"));
                        proveedor.IdQB = int.Parse(prov.Id);
                        _context.Entry(proveedor).State = EntityState.Modified;
                        _context.SaveChanges();
                    }
                    else
                    {
                        prov = agregarCategoriaProveedor(tipoProd, proveedor, serviceContext);
                        Vendor ven = agregarVendorProveedor(proveedor, serviceContext);
                        if (ven == null) return Ok(new { token = "Error insertando el Vendor" });
                        if (prov == null) return Ok(new { token = "Error insertando el proveedor" });
                    }
                }

                if(producto.IdQB == null)
                {
                    producto.IdQB = 0;
                }

                QueryService<Item> querySvc1 = new QueryService<Item>(serviceContext);
                string EXISTING_ITEM_QUERYBYNAME = string.Format("select * from Item where Id = '{0}' ", producto.IdQB);
                Item objItemFound = querySvc1.ExecuteIdsQuery(EXISTING_ITEM_QUERYBYNAME).FirstOrDefault<Item>();

                if (objItemFound == null)
                {
                    EXISTING_ITEM_QUERYBYNAME = string.Format("select * from Item where sku = '{0}' ", producto.SKU);
                    objItemFound = querySvc1.ExecuteIdsQuery(EXISTING_ITEM_QUERYBYNAME).FirstOrDefault<Item>();
                }

                if (objItemFound != null)
                {
                    Item ObjItem = new Item();
                    objItemFound.Id = objItemFound.Id;
                    objItemFound.Name = producto.Nombre;
                    objItemFound.ParentRef = new ReferenceType { Value = prov.Id, type = ItemTypeEnum.Category.GetStringValue(), name = prov.Name };
                    objItemFound.TypeSpecified = true;
                    objItemFound.Sku = producto.SKU;
                    objItemFound.Type = ItemTypeEnum.Service;
                    objItemFound.SubItem = true;
                    objItemFound.SubItemSpecified = true;
                    objItemFound.Active = true;


                    DataService dataService1 = new DataService(serviceContext);
                    Item UpdateEntity = dataService1.Update<Item>(objItemFound);
                    if (UpdateEntity != null && !string.IsNullOrEmpty(UpdateEntity.Id))
                    {
                        producto.IdQB = int.Parse(UpdateEntity.Id);
                        _context.Entry(producto).State = EntityState.Modified;
                        _context.SaveChanges();
                        //you can write Database code here
                        return Ok(new { token = "Se actualizo el producto" });
                    }
                    else
                    {
                        return Ok(new { token = "No se actualizo el producto" });
                    }




                }



            }
            catch (Exception ex)
            {
                return Ok(new { token = "Error no determinado. El error es: " + ex.Message });
            }
            return Ok(new { token = "Se insertó correctamente el producto" });
        }


        [HttpPost]
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
                    return Ok(new { token = "Error Cargando el Token" });
                }





                access_token = dictionary["accessToken"];
                realmId = dictionary["realmId"];
            }
            catch (Exception ex)
            {


                return Ok(new { token = "Error connectandose a QB" });
            }



            OAuth2RequestValidator oauthValidator = new OAuth2RequestValidator(access_token);
            // Create a ServiceContext with Auth tokens and realmId
            ServiceContext serviceContext = new ServiceContext(realmId, IntuitServicesType.QBO, oauthValidator);
            serviceContext.IppConfiguration.MinorVersion.Qbo = "23";
            serviceContext.IppConfiguration.BaseUrl.Qbo = QboBaseUrl;

            QueryService<Item> querySvc = new QueryService<Item>(serviceContext);
            string EXISTING_ITEM_QUERYBYNAME = string.Format("select * from Item where Id = '{0}' ", producto.IdQB);
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
                        return Ok(new { token = "Se desactivo el producto" });
                    }
                    else
                    {
                        return Ok(new { token = "No se desactivo el producto" });
                    }
                }
                else
                {
                    return Ok(new { token = "No se desactivo el producto pues ya estaba desactivado" });
                }
            }

            return Ok(new { token = "No se encontro el producto" });




        }


        [HttpPost]
        [Route("addProductAlojamiento")]
        public async System.Threading.Tasks.Task<ActionResult> AddProductoAlojamiento([FromBody] Alojamiento producto)
        {
            var access_token = "";
            var realmId = "";
            producto = _context.Alojamientos.Include(x => x.Proveedor).First(x => x.ProductoId == producto.ProductoId);
            List<Habitacion> habitaciones = _context.Habitaciones.Where(x => x.ProductoId == producto.ProductoId).ToList();

            if (habitaciones == null || !habitaciones.Any())
            {
                return Ok(new { token = "No hay habitaciones para poner en el Hotel" });
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
                    return Ok(new { token = "Error Cargando el Token" });
                }





                access_token = dictionary["accessToken"];
                realmId = dictionary["realmId"];
            }
            catch (Exception ex)
            {


                return Ok(new { token = "Error connectandose a QB" });
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

                List<Item> tiposProductos = querySvcI.ExecuteIdsQuery(string.Format("SELECT * from Item where Name = '{0}' and Type = '{1}' ", "Accommodation", ItemTypeEnum.Category)).ToList();


                Item tipoProd = new Item();
                tipoProd = tiposProductos.First();

                List<Item> proveedores = querySvcI.ExecuteIdsQuery(string.Format("SELECT * from Item where Name = '{0}' and Type = '{1}' ", proveedor.Nombre, ItemTypeEnum.Category)).ToList();
                Item prov = new Item();
                if (proveedores == null || proveedores.Count() == 0)
                {
                    prov = agregarCategoriaProveedor(tipoProd, proveedor, serviceContext);
                    Vendor ven = agregarVendorProveedor(proveedor, serviceContext);
                    if (ven == null) return Ok(new { token = "Error insertando el Vendor" });
                    if (prov == null) return Ok(new { token = "Error insertando el proveedor" });
                }
                else
                {
                    if (proveedores.Where(x => x.FullyQualifiedName.Contains("Accommodation")).ToList().Count > 0)
                    {
                        prov = proveedores.First(x => x.FullyQualifiedName.Contains("Accommodation"));
                        proveedor.IdQB = int.Parse(prov.Id);
                        _context.Entry(proveedor).State = EntityState.Modified;
                        _context.SaveChanges();
                    }
                    else

                    {
                        prov = agregarCategoriaProveedor(tipoProd, proveedor, serviceContext);
                        Vendor ven = agregarVendorProveedor(proveedor, serviceContext);
                        if (ven == null) return Ok(new { token = "Error insertando el Vendor" });
                        if (prov == null) return Ok(new { token = "Error insertando el proveedor" });
                    }
                }


                List<Item> hoteles = querySvcI.ExecuteIdsQuery("SELECT * from Item ").Where(x => x.Name == producto.Nombre && x.Type == ItemTypeEnum.Category).ToList();
                Item hotel = new Item();
                if (hoteles == null || hoteles.Count() == 0)
                {
                    hotel = agregarCategoriaHotel(prov, producto, serviceContext);
                    if (hotel == null) return Ok(new { token = "Error insertando el hotel" });
                    producto.IdQB = int.Parse(hotel.Id);
                    _context.Entry(producto).State = EntityState.Modified;
                    _context.SaveChanges();
                }
                else
                {
                    hotel = hoteles.First();
                }

                foreach (Habitacion hab in habitaciones)
                {


                    QueryService<Item> querySvc1 = new QueryService<Item>(serviceContext);
                    string EXISTING_ITEM_QUERYBYNAME = string.Format("select * from Item where Sku = '{0}' ", hab.SKU);
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
                                hab.IdQB = int.Parse(objItemFound.Id);
                                _context.Entry(hab).State = EntityState.Modified;
                                _context.SaveChanges();
                                //you can write Database code here
                                Ok(new { token = "Ya estaba en QB y se activo" });
                                continue;
                            }
                            else
                            {
                                Ok(new { token = "Ya estaba en QB pero no se activo" });
                                continue;
                            }
                        }
                        else
                        {
                            hab.IdQB = int.Parse(objItemFound.Id);
                            _context.Entry(hab).State = EntityState.Modified;
                            _context.SaveChanges();
                            Ok(new { token = "Ya hay un producto en QB con ese sku" });
                            continue;
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

                        return Ok(new { token = "Error obteniendo la cuenta Income" });
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

                        return Ok(new { token = "Error obteniendo la cuenta Expense" });
                    }
                    DataService dataService = new DataService(serviceContext);
                    Item ItemAdd = dataService.Add(ObjItem);
                    if (ItemAdd != null && !string.IsNullOrEmpty(ItemAdd.Id))
                    {
                        hab.IdQB = int.Parse(ItemAdd.Id);
                        _context.Entry(hab).State = EntityState.Modified;
                        _context.SaveChanges();
                        //you can write Database code here

                    }

                }







            }
            catch (Exception ex)
            {
                return Ok(new { token = "Error no determinado. El error es: " + ex.Message });
            }
            return Ok(new { token = "Se insertó correctamente el producto" });
        }


        [HttpPost]
        [Route("updateProductAlojamiento")]
        public async System.Threading.Tasks.Task<ActionResult> UpdateProductoAlojamiento([FromBody] Alojamiento producto)
        {
            var access_token = "";
            var realmId = "";
            producto = _context.Alojamientos.Include(x => x.Proveedor).First(x => x.ProductoId == producto.ProductoId);
            List<Habitacion> habitaciones = _context.Habitaciones.Where(x => x.ProductoId == producto.ProductoId).ToList();

            if (habitaciones == null || !habitaciones.Any())
            {
                return Ok(new { token = "No hay habitaciones para poner en el Hotel" });
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
                    return Ok(new { token = "Error Cargando el Token" });
                }





                access_token = dictionary["accessToken"];
                realmId = dictionary["realmId"];
            }
            catch (Exception ex)
            {


                return Ok(new { token = "Error connectandose a QB" });
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

                List<Item> tiposProductos = querySvcI.ExecuteIdsQuery(string.Format("SELECT * from Item where Name = '{0}' and Type = '{1}' ", "Accommodation", ItemTypeEnum.Category)).ToList();


                Item tipoProd = new Item();
                tipoProd = tiposProductos.First();

                List<Item> proveedores = querySvcI.ExecuteIdsQuery(string.Format("SELECT * from Item where Name = '{0}' and Type = '{1}' ", proveedor.Nombre, ItemTypeEnum.Category)).ToList();
                Item prov = new Item();
                if (proveedores == null || proveedores.Count() == 0)
                {
                    prov = agregarCategoriaProveedor(tipoProd, proveedor, serviceContext);
                    Vendor ven = agregarVendorProveedor(proveedor, serviceContext);
                    if (ven == null) return Ok(new { token = "Error insertando el Vendor" });
                    if (prov == null) return Ok(new { token = "Error insertando el proveedor" });
                }
                else
                {

                    if (proveedores.Where(x => x.FullyQualifiedName.Contains("Accommodation")).ToList().Count > 0)
                    {
                        prov = proveedores.First(x => x.FullyQualifiedName.Contains("Accommodation"));
                        proveedor.IdQB = int.Parse(prov.Id);
                        _context.Entry(proveedor).State = EntityState.Modified;
                        _context.SaveChanges();
                    }
                    else

                    {
                        prov = agregarCategoriaProveedor(tipoProd, proveedor, serviceContext);
                        Vendor ven = agregarVendorProveedor(proveedor, serviceContext);
                        if (ven == null) return Ok(new { token = "Error insertando el Vendor" });
                        if (prov == null) return Ok(new { token = "Error insertando el proveedor" });
                    }
                }


                List<Item> hoteles = querySvcI.ExecuteIdsQuery("SELECT * from Item ").Where(x => x.Name == producto.Nombre && x.Type == ItemTypeEnum.Category).ToList();
                Item hotel = new Item();
                if (hoteles == null || hoteles.Count() == 0)
                {
                    hotel = agregarCategoriaHotel(prov, producto, serviceContext);
                    if (hotel == null) return Ok(new { token = "Error insertando el hotel" });
                    producto.IdQB = int.Parse(hotel.Id);
                    _context.Entry(producto).State = EntityState.Modified;
                    _context.SaveChanges();

                }
                else
                {
                    hotel = hoteles.First();
                }

                foreach (Habitacion hab in habitaciones)
                {

                    if (hab.IdQB == null)
                    {
                        hab.IdQB = 0;
                    }
                    QueryService<Item> querySvc1 = new QueryService<Item>(serviceContext);
                    string EXISTING_ITEM_QUERYBYNAME = string.Format("select * from Item where Id = '{0}' ", hab.IdQB);
                    Item objItemFound = querySvc1.ExecuteIdsQuery(EXISTING_ITEM_QUERYBYNAME).FirstOrDefault<Item>();

                    if (objItemFound == null)
                    {
                        EXISTING_ITEM_QUERYBYNAME = string.Format("select * from Item where sku = '{0}' ", hab.SKU);
                        objItemFound = querySvc1.ExecuteIdsQuery(EXISTING_ITEM_QUERYBYNAME).FirstOrDefault<Item>();
                    }

                    if (objItemFound != null)
                    {
                        Item ObjItem = new Item();
                        objItemFound.Id = objItemFound.Id;
                        objItemFound.Name = hab.Nombre;
                        objItemFound.ParentRef = new ReferenceType { Value = hotel.Id, type = ItemTypeEnum.Category.GetStringValue(), name = hotel.Name };
                        objItemFound.TypeSpecified = true;
                        objItemFound.Sku = hab.SKU;
                        objItemFound.Type = ItemTypeEnum.Service;
                        objItemFound.SubItem = true;
                        objItemFound.SubItemSpecified = true;
                        objItemFound.Active = true;
                        DataService dataService1 = new DataService(serviceContext);
                        Item UpdateEntity = dataService1.Update<Item>(objItemFound);



                    }
                    else
                    {
                        Item ObjItem = new Item();
                        //ObjItem.Id = objItemFound.Id;
                        ObjItem.Name = hab.Nombre;
                        ObjItem.ParentRef = new ReferenceType { Value = hotel.Id, type = ItemTypeEnum.Category.GetStringValue(), name = hotel.Name };
                        ObjItem.TypeSpecified = true;
                        ObjItem.Sku = hab.SKU;
                        ObjItem.Type = ItemTypeEnum.Service;
                        ObjItem.SubItem = true;
                        ObjItem.SubItemSpecified = true;
                        ObjItem.Active = true;



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

                            return Ok(new { token = "Error obteniendo la cuenta Income" });
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

                            return Ok(new { token = "Error obteniendo la cuenta Expense" });
                        }


                        DataService dataService1 = new DataService(serviceContext);
                        Item UpdateEntity = dataService1.Add<Item>(ObjItem);
                        if (UpdateEntity != null && !string.IsNullOrEmpty(UpdateEntity.Id))
                        {
                            hab.IdQB = int.Parse(UpdateEntity.Id);
                            _context.Entry(hab).State = EntityState.Modified;
                            _context.SaveChanges();
                            //you can write Database code here

                        }

                    }

                }


            }
            catch (Exception ex)
            {
                return Ok(new { token = "Error no determinado. El error es: " + ex.Message });
            }
            return Ok(new { token = "Se insertó correctamente el producto" });
        }

        [HttpPost]
        [Route("deleteProductAlojamiento")]
        public async System.Threading.Tasks.Task<ActionResult> DeleteProductAlojamiento([FromBody] Alojamiento producto)
        {
            var access_token = "";
            var realmId = "";
            producto = _context.Alojamientos.Find(producto.ProductoId);
            List<Habitacion> habitaciones = _context.Habitaciones.Where(x => x.ProductoId == producto.ProductoId).ToList();
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
                    return Ok(new { token = "Error Cargando el Token" });
                }





                access_token = dictionary["accessToken"];
                realmId = dictionary["realmId"];
            }
            catch (Exception ex)
            {


                return Ok(new { token = "Error connectandose a QB" });
            }

            OAuth2RequestValidator oauthValidator = new OAuth2RequestValidator(access_token);
            // Create a ServiceContext with Auth tokens and realmId
            ServiceContext serviceContext = new ServiceContext(realmId, IntuitServicesType.QBO, oauthValidator);
            serviceContext.IppConfiguration.MinorVersion.Qbo = "23";
            serviceContext.IppConfiguration.BaseUrl.Qbo = QboBaseUrl;


            QueryService<Item> querySvc1 = new QueryService<Item>(serviceContext);
            string EXISTING_ITEM_QUERYBYNAME1 = string.Format("select * from Item where Id = '{0}' ", producto.IdQB);
            Item objItemFound1 = querySvc1.ExecuteIdsQuery(EXISTING_ITEM_QUERYBYNAME1).FirstOrDefault<Item>();



            //If Item found on Quickbooks online
            if (objItemFound1 != null)
            {
                //if Item is active
                if (objItemFound1.Active == true)
                {
                    objItemFound1.Active = false;
                    DataService dataService = new DataService(serviceContext);
                    Item UpdateEntity = dataService.Update<Item>(objItemFound1);
                    if (UpdateEntity != null && !string.IsNullOrEmpty(UpdateEntity.Id))
                    {
                        //you can write Database code here
                        Ok(new { token = "Se desactivo el producto" });
                    }
                    else
                    {
                        Ok(new { token = "No se desactivo el producto" });
                    }
                }
                else
                {
                    Ok(new { token = "No se desactivo el producto pues ya estaba desactivado" });
                }
            }





            foreach (Habitacion hab in habitaciones)
            {


                QueryService<Item> querySvc = new QueryService<Item>(serviceContext);
                string EXISTING_ITEM_QUERYBYNAME = string.Format("select * from Item where Id = '{0}' ", hab.IdQB);
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
                            Ok("Se desactivo el hab");
                        }
                        else
                        {
                            Ok("No se desactivo el hab");
                        }
                    }
                    else
                    {
                        Ok(new { token = "No se desactivo el producto pues ya estaba desactivado" });
                    }
                }



            }


            return Ok(new { token = "Todo ok" });




        }



        [HttpPost]
        [Route("createEstimated")]
        public async System.Threading.Tasks.Task<ActionResult> CreateEstimated([FromBody] Orden orden)
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
                      .Include(d => d.Habitacion)
                      .Include(d => d.PlanAlimenticio)/*.ThenInclude(xv => xv.ListaCombinacionesDisponibles)
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
                    return Ok(new { token = "Error Cargando el Token" });
                }





                access_token = dictionary["accessToken"];
                realmId = dictionary["realmId"];
            }
            catch (Exception ex)
            {


                return Ok(new { token = "Error connectandose a QB" });
            }



            OAuth2RequestValidator oauthValidator = new OAuth2RequestValidator(access_token);
            // Create a ServiceContext with Auth tokens and realmId
            ServiceContext serviceContext = new ServiceContext(realmId, IntuitServicesType.QBO, oauthValidator);
            serviceContext.IppConfiguration.MinorVersion.Qbo = "23";
            serviceContext.IppConfiguration.BaseUrl.Qbo = QboBaseUrl;

            QueryService<Customer> querySvc = new QueryService<Customer>(serviceContext);
            string EXISTING_ITEM_QUERYBYNAME = string.Format("select * from Customer where id = '{0}' ", orden.Cliente.IdQB);
            Customer objCustomerFound;
            try
            {
                objCustomerFound = querySvc.ExecuteIdsQuery(EXISTING_ITEM_QUERYBYNAME).FirstOrDefault<Customer>();
            }
            catch (Exception ex)
            {

                // Create a QuickBooks QueryService using ServiceContext
                QueryService<Customer> querySvcCustomer = new QueryService<Customer>(serviceContext);



                Customer ObjItem = new Customer();
                ObjItem.DisplayName = orden.Cliente.Nombre;
                ObjItem.FamilyName = orden.Cliente.Nombre;
                ObjItem.GivenName = orden.Cliente.Nombre;
                ObjItem.ContactName = orden.Cliente.Nombre;
                ObjItem.Title = orden.Cliente.Nombre;
                ObjItem.PrimaryEmailAddr = new EmailAddress { Address = orden.Cliente.Correo };
                ObjItem.AlternatePhone = new TelephoneNumber { DeviceType = "LandLine", FreeFormNumber = orden.Cliente.Telefono };
                ObjItem.Mobile = new TelephoneNumber { DeviceType = "Mobile", FreeFormNumber = orden.Cliente.Telefono };
                ObjItem.PrimaryPhone = new TelephoneNumber { DeviceType = "Mobile", FreeFormNumber = orden.Cliente.Telefono };



                DataService dataService2 = new DataService(serviceContext);
                Customer customer = dataService2.Add(ObjItem);
                if (customer != null && !string.IsNullOrEmpty(customer.Id))
                {
                    objCustomerFound = customer;

                }
                else
                {
                    objCustomerFound = null;

                }
            }

            if(objCustomerFound ==  null && orden.Cliente.IdQB == 0)
            {
                // Create a QuickBooks QueryService using ServiceContext
                QueryService<Customer> querySvcCustomer = new QueryService<Customer>(serviceContext);



                Customer ObjItem = new Customer();
                ObjItem.DisplayName = orden.Cliente.Nombre;
                ObjItem.FamilyName = orden.Cliente.Nombre;
                ObjItem.GivenName = orden.Cliente.Nombre;
                ObjItem.ContactName = orden.Cliente.Nombre;
                ObjItem.Title = orden.Cliente.Nombre;
                ObjItem.PrimaryEmailAddr = new EmailAddress { Address = orden.Cliente.Correo };
                ObjItem.AlternatePhone = new TelephoneNumber { DeviceType = "LandLine", FreeFormNumber = orden.Cliente.Telefono };
                ObjItem.Mobile = new TelephoneNumber { DeviceType = "Mobile", FreeFormNumber = orden.Cliente.Telefono };
                ObjItem.PrimaryPhone = new TelephoneNumber { DeviceType = "Mobile", FreeFormNumber = orden.Cliente.Telefono };



                DataService dataService2 = new DataService(serviceContext);
                Customer customer = dataService2.Add(ObjItem);
                if (customer != null && !string.IsNullOrEmpty(customer.Id))
                {
                    objCustomerFound = customer;

                }
                else
                {
                    objCustomerFound = null;

                }
            }

            if (objCustomerFound == null)
            {
                return Ok(new { token = "Error creando el cliente" });
            }

            Estimate ObjEstimate = new Estimate();
            ObjEstimate.CustomerRef = new ReferenceType();
            ObjEstimate.CustomerRef.Value = objCustomerFound.Id; //Quickbooks online Customer Id
            ObjEstimate.DocNumber = orden.NumeroOrden;
            List<Line> LineList = new List<Line>();
            if (orden.ListaActividadOrden != null && orden.ListaActividadOrden.Any())
                foreach (var item in orden.ListaActividadOrden)
                {
                    decimal precio = item.PrecioOrden;
                    if (_context.PreciosOrdenModificados.Where(x => x.OrdenId == item.OrdenId && x.OrdenActividadId == item.OrdenActividadId).Count() > 0)
                        precio = _context.PreciosOrdenModificados.Where(x => x.OrdenId == item.OrdenId && x.OrdenActividadId == item.OrdenActividadId).Sum(x => x.Precio);
                    Line objLine = new Line();
                    objLine.DetailTypeSpecified = true;
                    objLine.DetailType = LineDetailTypeEnum.SalesItemLineDetail;
                    objLine.AmountSpecified = true;
                    objLine.Amount = precio;
                    var ca = item.CantAdulto == null ? "0" : item.CantAdulto.ToString();
                    var cm = (item.CantInfante ?? 0 + item.CantNino ?? 0).ToString();
                    objLine.Description = "Activity: " + item.Actividad.Nombre + ". " +
                                          "Date: " + item.FechaActividad.ToString("MM/dd/yyyy") + ". " +
                                          "Lugar: " + item.LugarActividad + ". " +
                                         "Adults: " + ca + ". " +
                                          "Childs:  " + cm + ". ";
                    SalesItemLineDetail salesItemLineDetail = new SalesItemLineDetail();
                    salesItemLineDetail.QtySpecified = true;
                    salesItemLineDetail.Qty = 1;
                    salesItemLineDetail.ItemRef = new ReferenceType();
                    salesItemLineDetail.AnyIntuitObject = precio ;
                    salesItemLineDetail.ItemElementName = ItemChoiceType.UnitPrice;
                    salesItemLineDetail.ServiceDate = item.FechaActividad;
                    salesItemLineDetail.ServiceDateSpecified = true;


                    QueryService<Item> querySvc1 = new QueryService<Item>(serviceContext);
                    string EXISTING_ITEM_QUERYBYNAMEITEMPROD = string.Format("select * from Item where Id = '{0}' ", item.Actividad.IdQB);
                    Item itemProduct = querySvc1.ExecuteIdsQuery(EXISTING_ITEM_QUERYBYNAMEITEMPROD).FirstOrDefault<Item>();
                    if (itemProduct == null)
                    {
                        return Ok(new { token = "El producto no exite en QB: " + item.Actividad.Nombre });
                    }
                    salesItemLineDetail.ItemRef.Value = itemProduct.Id; //Quickbooks online Item Id
                    objLine.AnyIntuitObject = salesItemLineDetail;
                    LineList.Add(objLine);


                }
            if (orden.ListaTrasladoOrden != null && orden.ListaTrasladoOrden.Any())
                foreach (var item in orden.ListaTrasladoOrden)
                {
                    decimal precio = item.PrecioOrden;
                    if (_context.PreciosOrdenModificados.Where(x => x.OrdenId == item.OrdenId && x.OrdenTrasladoId == item.OrdenTrasladoId).Count() > 0)
                        precio = _context.PreciosOrdenModificados.Where(x => x.OrdenId == item.OrdenId && x.OrdenTrasladoId == item.OrdenTrasladoId).Sum(x => x.Precio);
                    Line objLine = new Line();
                    objLine.DetailTypeSpecified = true;
                    objLine.DetailType = LineDetailTypeEnum.SalesItemLineDetail;
                    objLine.AmountSpecified = true;
                    objLine.Amount = precio;
                    var ca = item.CantAdulto == null ? "0" : item.CantAdulto.ToString();
                    var cm = (item.CantInfante ?? 0 + item.CantNino ?? 0).ToString();
                    objLine.Description = "Ground Transportation: " + item.PuntoOrigen.Nombre + " - " + item.PuntoDestino.Nombre +
                                          "Date: " + item.FechaRecogida.ToString("MM/dd/yyyy") + ". " +
                                          "Adults: " + ca + ". " +
                                          "Childs:  " + cm + ". " +
                                          "Capacity: " + item.Traslado.CapacidadTraslado;
                    SalesItemLineDetail salesItemLineDetail = new SalesItemLineDetail();
                    salesItemLineDetail.QtySpecified = true;
                    salesItemLineDetail.Qty = 1;
                    salesItemLineDetail.ItemRef = new ReferenceType();
                    salesItemLineDetail.AnyIntuitObject = precio;
                    salesItemLineDetail.ItemElementName = ItemChoiceType.UnitPrice;
                    salesItemLineDetail.ServiceDate = item.FechaRecogida;
                    salesItemLineDetail.ServiceDateSpecified = true;

                    QueryService<Item> querySvc1 = new QueryService<Item>(serviceContext);
                    string EXISTING_ITEM_QUERYBYNAMEITEMPROD = string.Format("select * from Item where Id = '{0}' ", item.Traslado.IdQB);
                    Item itemProduct = querySvc1.ExecuteIdsQuery(EXISTING_ITEM_QUERYBYNAMEITEMPROD).FirstOrDefault<Item>();
                    if (itemProduct == null)
                    {
                        return Ok(new { token = "El producto no exite en QB: " + item.Traslado.Nombre });
                    }
                    salesItemLineDetail.ItemRef.Value = itemProduct.Id; //Quickbooks online Item Id
                    objLine.AnyIntuitObject = salesItemLineDetail;
                    LineList.Add(objLine);


                }

            if (orden.ListaVehiculosOrden != null && orden.ListaVehiculosOrden.Any())
                foreach (var item in orden.ListaVehiculosOrden)
                {
                    decimal precio = item.PrecioOrden;
                    if (_context.PreciosOrdenModificados.Where(x => x.OrdenId == item.OrdenId && x.OrdenVehiculoId == item.OrdenVehiculoId).Count() > 0)
                        precio = _context.PreciosOrdenModificados.Where(x => x.OrdenId == item.OrdenId && x.OrdenVehiculoId == item.OrdenVehiculoId).Sum(x => x.Precio);
                    Line objLine = new Line();
                    objLine.DetailTypeSpecified = true;
                    objLine.DetailType = LineDetailTypeEnum.SalesItemLineDetail;
                    objLine.AmountSpecified = true;
                    objLine.Amount = precio;
                    objLine.Description = /*"Vehicle: " + item.Vehiculo.Nombre +*/
                                          "Date: " + item.FechaRecogida.ToString("MM/dd/yyyy") + " - " + item.FechaEntrega.ToString("MM/dd/yyyy");
                                          
                    SalesItemLineDetail salesItemLineDetail = new SalesItemLineDetail();
                    salesItemLineDetail.QtySpecified = true;
                    salesItemLineDetail.Qty = (item.FechaEntrega - item.FechaRecogida).Days;
                    salesItemLineDetail.ItemRef = new ReferenceType();
                    salesItemLineDetail.AnyIntuitObject = precio / salesItemLineDetail.Qty;
                    salesItemLineDetail.ItemElementName = ItemChoiceType.UnitPrice;
                    salesItemLineDetail.ServiceDate = item.FechaRecogida;
                    salesItemLineDetail.ServiceDateSpecified = true;

                    QueryService<Item> querySvc1 = new QueryService<Item>(serviceContext);
                    string EXISTING_ITEM_QUERYBYNAMEITEMPROD = string.Format("select * from Item where Id = '{0}' ", item.Vehiculo.IdQB);
                    Item itemProduct = querySvc1.ExecuteIdsQuery(EXISTING_ITEM_QUERYBYNAMEITEMPROD).FirstOrDefault<Item>();
                    if (itemProduct == null)
                    {
                        return Ok(new { token = "El producto no exite en QB: " + item.Vehiculo.Nombre });
                    }
                    salesItemLineDetail.ItemRef.Value = itemProduct.Id; //Quickbooks online Item Id
                    objLine.AnyIntuitObject = salesItemLineDetail;
                    LineList.Add(objLine);


                }

            if (orden.ListaAlojamientoOrden != null && orden.ListaAlojamientoOrden.Any())
                foreach (var item in orden.ListaAlojamientoOrden)
                {
                    decimal precio = item.PrecioOrden;
                    if (_context.PreciosOrdenModificados.Where(x => x.OrdenId == item.OrdenId && x.OrdenAlojamientoId == item.OrdenAlojamientoId).Count() > 0)
                        precio = _context.PreciosOrdenModificados.Where(x => x.OrdenId == item.OrdenId && x.OrdenAlojamientoId == item.OrdenAlojamientoId).Sum(x => x.Precio);
                    Line objLine = new Line();
                    objLine.DetailTypeSpecified = true;
                    objLine.DetailType = LineDetailTypeEnum.SalesItemLineDetail;
                    objLine.AmountSpecified = true;
                    objLine.Amount = precio;
                    var ca = item.CantAdulto == null ? "0" : item.CantAdulto.ToString();
                    var cm = (item.CantInfante ?? 0 + item.CantNino ?? 0).ToString();
                    objLine.Description = "Accommodation: " + item.Alojamiento.Nombre + ". " +
                                          "Date: " + item.FechaInicio.ToString("MM/dd/yyyy").Substring(0, 10) + " - " + item.FechaFin.ToString().Substring(0, 10) + ". "+
                                          "Booking Categories: " + item.PlanAlimenticio.Nombre + ". " +
                                          "Adults: " + ca  + ". " +
                                          "Childs:  " + cm;
                    SalesItemLineDetail salesItemLineDetail = new SalesItemLineDetail();
                    salesItemLineDetail.QtySpecified = true;
                    salesItemLineDetail.Qty = 1;
                    salesItemLineDetail.ItemRef = new ReferenceType();

                    QueryService<Item> querySvc1 = new QueryService<Item>(serviceContext);
                    string EXISTING_ITEM_QUERYBYNAMEITEMPROD = string.Format("select * from Item where Id = '{0}' ", item.Habitacion.IdQB);
                    Item itemProduct = querySvc1.ExecuteIdsQuery(EXISTING_ITEM_QUERYBYNAMEITEMPROD).FirstOrDefault<Item>();
                    if (itemProduct == null)
                    {
                        return Ok(new { token = "El producto no exite en QB: " + item.Alojamiento.Nombre });
                    }
                    salesItemLineDetail.AnyIntuitObject =precio;
                    salesItemLineDetail.ItemElementName = ItemChoiceType.UnitPrice;
                    salesItemLineDetail.ServiceDate = item.FechaInicio;
                    salesItemLineDetail.ServiceDateSpecified = true;
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
                return Ok(new { token = "Se creo el estimado" });
            }


            return Ok(new { token = "No se encontro el producto" });




        }


        [HttpPost]
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
                      .Include(d => d.Habitacion)
                      .Include(d => d.PlanAlimenticio)/*.ThenInclude(xv => xv.ListaCombinacionesDisponibles)
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
                    return Ok(new { token = "Error Cargando el Token" });
                }





                access_token = dictionary["accessToken"];
                realmId = dictionary["realmId"];
            }
            catch (Exception ex)
            {


                return Ok(new { token = "Error connectandose a QB" });
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
                string EXISTING_ITEM_QUERYBYNAME = string.Format("select * from Customer where Id = '{0}' ", orden.Cliente.IdQB);

                Customer objCustomerFound;
                try
                {
                    objCustomerFound = querySvc.ExecuteIdsQuery(EXISTING_ITEM_QUERYBYNAME).FirstOrDefault<Customer>();
                }
                catch (Exception ex)
                {

                    // Create a QuickBooks QueryService using ServiceContext
                    QueryService<Customer> querySvcCustomer = new QueryService<Customer>(serviceContext);



                    Customer ObjItem = new Customer();
                    ObjItem.DisplayName = orden.Cliente.Nombre;
                    ObjItem.FamilyName = orden.Cliente.Nombre;
                    ObjItem.GivenName = orden.Cliente.Nombre;
                    ObjItem.ContactName = orden.Cliente.Nombre;
                    ObjItem.Title = orden.Cliente.Nombre;
                    ObjItem.PrimaryEmailAddr = new EmailAddress { Address = orden.Cliente.Correo };
                    ObjItem.AlternatePhone = new TelephoneNumber { DeviceType = "LandLine", FreeFormNumber = orden.Cliente.Telefono };
                    ObjItem.Mobile = new TelephoneNumber { DeviceType = "Mobile", FreeFormNumber = orden.Cliente.Telefono };
                    ObjItem.PrimaryPhone = new TelephoneNumber { DeviceType = "Mobile", FreeFormNumber = orden.Cliente.Telefono };



                    DataService dataService2 = new DataService(serviceContext);
                    Customer customer = dataService2.Add(ObjItem);
                    if (customer != null && !string.IsNullOrEmpty(customer.Id))
                    {
                        objCustomerFound = customer;

                    }
                    else
                    {
                        objCustomerFound = null;

                    }
                }

                if (objCustomerFound == null)
                {
                    return Ok(new { token = "Error creando el cliente" });
                }

                Estimate ObjEstimate = new Estimate();
                objEstimateFound.Id = objEstimateFound.Id;
                objEstimateFound.CustomerRef = new ReferenceType();
                objEstimateFound.CustomerRef.Value = objCustomerFound.Id; //Quickbooks online Customer Id
                objEstimateFound.DocNumber = orden.NumeroOrden;
                List<Line> LineList = new List<Line>();
                if (orden.ListaActividadOrden != null && orden.ListaActividadOrden.Any())
                    foreach (var item in orden.ListaActividadOrden)
                    {
                        decimal precio = item.PrecioOrden;
                        if (_context.PreciosOrdenModificados.Where(x => x.OrdenId == item.OrdenId && x.OrdenActividadId == item.OrdenActividadId).Count() > 0)
                            precio = _context.PreciosOrdenModificados.Where(x => x.OrdenId == item.OrdenId && x.OrdenActividadId == item.OrdenActividadId).Sum(x => x.Precio);
                        Line objLine = new Line();
                        objLine.DetailTypeSpecified = true;
                        objLine.DetailType = LineDetailTypeEnum.SalesItemLineDetail;
                        objLine.AmountSpecified = true;
                        objLine.Amount = precio;
                        var ca = item.CantAdulto == null ? "0" : item.CantAdulto.ToString();
                        var cm = (item.CantInfante ?? 0 + item.CantNino ?? 0).ToString();
                        objLine.Description = "Activity: " + item.Actividad.Nombre + ". " +
                                              "Date: " + item.FechaActividad.ToString("MM/dd/yyyy") + ". " +
                                              "Lugar: " + item.LugarActividad + ". " +
                                             "Adults: " + ca + ". " +
                                              "Childs:  " + cm + ". ";
                        SalesItemLineDetail salesItemLineDetail = new SalesItemLineDetail();
                        salesItemLineDetail.QtySpecified = true;
                        salesItemLineDetail.Qty = 1;
                        salesItemLineDetail.ItemRef = new ReferenceType();
                        salesItemLineDetail.AnyIntuitObject = precio;
                        salesItemLineDetail.ItemElementName = ItemChoiceType.UnitPrice;
                        salesItemLineDetail.ServiceDate = item.FechaActividad;
                        salesItemLineDetail.ServiceDateSpecified = true;


                        QueryService<Item> querySvc1 = new QueryService<Item>(serviceContext);
                        string EXISTING_ITEM_QUERYBYNAMEITEMPROD = string.Format("select * from Item where Id = '{0}' ", item.Actividad.IdQB);

                        Item itemProduct = querySvc1.ExecuteIdsQuery(EXISTING_ITEM_QUERYBYNAMEITEMPROD).FirstOrDefault<Item>();
                        if (itemProduct == null)
                        {
                            return Ok(new { token = "El producto no exite en QB: " + item.Actividad.Nombre });
                        }
                        salesItemLineDetail.ItemRef.Value = itemProduct.Id; //Quickbooks online Item Id
                        objLine.AnyIntuitObject = salesItemLineDetail;
                        LineList.Add(objLine);


                    }
                if (orden.ListaTrasladoOrden != null && orden.ListaTrasladoOrden.Any())
                    foreach (var item in orden.ListaTrasladoOrden)
                    {
                        decimal precio = item.PrecioOrden;
                        if (_context.PreciosOrdenModificados.Where(x => x.OrdenId == item.OrdenId && x.OrdenTrasladoId == item.OrdenTrasladoId).Count() > 0)
                            precio = _context.PreciosOrdenModificados.Where(x => x.OrdenId == item.OrdenId && x.OrdenTrasladoId == item.OrdenTrasladoId).Sum(x => x.Precio);
                        Line objLine = new Line();
                        objLine.DetailTypeSpecified = true;
                        objLine.DetailType = LineDetailTypeEnum.SalesItemLineDetail;
                        objLine.AmountSpecified = true;
                        objLine.Amount = precio;
                        var ca = item.CantAdulto == null ? "0" : item.CantAdulto.ToString();
                        var cm = (item.CantInfante ?? 0 + item.CantNino ?? 0).ToString();
                        objLine.Description = "Ground Transportation: " + item.PuntoOrigen.Nombre + " - " + item.PuntoDestino.Nombre +
                                              "Date: " + item.FechaRecogida.ToString("MM/dd/yyyy") + ". " +
                                              "Adults: " + ca + ". " +
                                              "Childs:  " + cm + ". " +
                                              "Capacity: " + item.Traslado.CapacidadTraslado;
                        SalesItemLineDetail salesItemLineDetail = new SalesItemLineDetail();
                        salesItemLineDetail.QtySpecified = true;
                        salesItemLineDetail.Qty = 1;
                        salesItemLineDetail.ItemRef = new ReferenceType();
                        salesItemLineDetail.AnyIntuitObject = precio;
                        salesItemLineDetail.ItemElementName = ItemChoiceType.UnitPrice;
                        salesItemLineDetail.ServiceDate = item.FechaRecogida;
                        salesItemLineDetail.ServiceDateSpecified = true;

                        QueryService<Item> querySvc1 = new QueryService<Item>(serviceContext);
                        string EXISTING_ITEM_QUERYBYNAMEITEMPROD = string.Format("select * from Item where Id = '{0}' ", item.Traslado.IdQB);
                        Item itemProduct = querySvc1.ExecuteIdsQuery(EXISTING_ITEM_QUERYBYNAMEITEMPROD).FirstOrDefault<Item>();
                        if (itemProduct == null)
                        {
                            return Ok(new { token = "El producto no exite en QB: " + item.Traslado.Nombre });
                        }
                        salesItemLineDetail.ItemRef.Value = itemProduct.Id; //Quickbooks online Item Id
                        objLine.AnyIntuitObject = salesItemLineDetail;
                        LineList.Add(objLine);


                    }

                if (orden.ListaVehiculosOrden != null && orden.ListaVehiculosOrden.Any())
                    foreach (var item in orden.ListaVehiculosOrden)
                    {
                        decimal precio = item.PrecioOrden;
                        if (_context.PreciosOrdenModificados.Where(x => x.OrdenId == item.OrdenId && x.OrdenVehiculoId == item.OrdenVehiculoId).Count() > 0)
                            precio = _context.PreciosOrdenModificados.Where(x => x.OrdenId == item.OrdenId && x.OrdenVehiculoId == item.OrdenVehiculoId).Sum(x => x.Precio);
                        Line objLine = new Line();
                        objLine.DetailTypeSpecified = true;
                        objLine.DetailType = LineDetailTypeEnum.SalesItemLineDetail;
                        objLine.AmountSpecified = true;
                        objLine.Amount = precio;
                        objLine.Description = /*"Vehicle: " + item.Vehiculo.Nombre +*/
                                         "Date: " + item.FechaRecogida.ToString("MM/dd/yyyy") + " - " + item.FechaEntrega.ToString("MM/dd/yyyy");

                        SalesItemLineDetail salesItemLineDetail = new SalesItemLineDetail();
                        salesItemLineDetail.QtySpecified = true;
                        salesItemLineDetail.Qty = (item.FechaEntrega - item.FechaRecogida).Days;
                        salesItemLineDetail.ItemRef = new ReferenceType();
                        salesItemLineDetail.AnyIntuitObject = precio / salesItemLineDetail.Qty;
                        salesItemLineDetail.ItemElementName = ItemChoiceType.UnitPrice;
                        salesItemLineDetail.ServiceDate = item.FechaRecogida;
                        salesItemLineDetail.ServiceDateSpecified = true;

                        QueryService<Item> querySvc1 = new QueryService<Item>(serviceContext);
                        string EXISTING_ITEM_QUERYBYNAMEITEMPROD = string.Format("select * from Item where Id = '{0}' ", item.Vehiculo.IdQB);
                        Item itemProduct = querySvc1.ExecuteIdsQuery(EXISTING_ITEM_QUERYBYNAMEITEMPROD).FirstOrDefault<Item>();
                        if (itemProduct == null)
                        {
                            return Ok(new { token = "El producto no exite en QB: " + item.Vehiculo.Nombre });
                        }
                        salesItemLineDetail.ItemRef.Value = itemProduct.Id; //Quickbooks online Item Id
                        objLine.AnyIntuitObject = salesItemLineDetail;
                        LineList.Add(objLine);


                    }

                if (orden.ListaAlojamientoOrden != null && orden.ListaAlojamientoOrden.Any())
                    foreach (var item in orden.ListaAlojamientoOrden)
                    {
                        decimal precio = item.PrecioOrden;
                        if (_context.PreciosOrdenModificados.Where(x => x.OrdenId == item.OrdenId && x.OrdenAlojamientoId == item.OrdenAlojamientoId).Count() > 0)
                            precio = _context.PreciosOrdenModificados.Where(x => x.OrdenId == item.OrdenId && x.OrdenAlojamientoId == item.OrdenAlojamientoId).Sum(x => x.Precio);
                        Line objLine = new Line();
                        objLine.DetailTypeSpecified = true;
                        objLine.DetailType = LineDetailTypeEnum.SalesItemLineDetail;
                        objLine.AmountSpecified = true;
                        objLine.Amount = precio;
                        var ca = item.CantAdulto == null ? "0" : item.CantAdulto.ToString();
                        var cm = (item.CantInfante ?? 0 + item.CantNino ?? 0).ToString();
                        objLine.Description = "Accommodation: " + item.Alojamiento.Nombre + ". " +
                                              "Date: " + item.FechaInicio.ToString("MM/dd/yyyy").Substring(0, 10) + " - " + item.FechaFin.ToString().Substring(0, 10) + ". " +
                                              "Booking Categories: " + item.PlanAlimenticio.Nombre + ". " +
                                              "Adults: " + ca + ". " +
                                              "Childs:  " + cm;
                       
                        SalesItemLineDetail salesItemLineDetail = new SalesItemLineDetail();
                        salesItemLineDetail.QtySpecified = true;
                        salesItemLineDetail.Qty = 1;
                        salesItemLineDetail.ItemRef = new ReferenceType();

                        QueryService<Item> querySvc1 = new QueryService<Item>(serviceContext);
                        string EXISTING_ITEM_QUERYBYNAMEITEMPROD = string.Format("select * from Item where Id = '{0}' ", item.Habitacion.IdQB);
                        Item itemProduct = querySvc1.ExecuteIdsQuery(EXISTING_ITEM_QUERYBYNAMEITEMPROD).FirstOrDefault<Item>();
                        if (itemProduct == null)
                        {
                            return Ok(new { token = "El producto no exite en QB: " + item.Alojamiento.Nombre });
                        }
                        salesItemLineDetail.AnyIntuitObject = precio;
                        salesItemLineDetail.ItemElementName = ItemChoiceType.UnitPrice;
                        salesItemLineDetail.ServiceDate = item.FechaInicio;
                        salesItemLineDetail.ServiceDateSpecified = true;
                        salesItemLineDetail.ItemRef.Value = itemProduct.Id; //Quickbooks online Item Id
                        objLine.AnyIntuitObject = salesItemLineDetail;
                        LineList.Add(objLine);


                    }




                objEstimateFound.Line = LineList.ToArray();
                DataService dataService = new DataService(serviceContext);
                Estimate UpdateEntity = dataService.Update<Estimate>(objEstimateFound);
                if (UpdateEntity != null && !string.IsNullOrEmpty(UpdateEntity.Id))
                {

                    //you can write Database code here
                    return Ok(new { token = "Se creo el estimado" });
                }

            }




            return Ok(new { token = "No se encontro el producto" });




        }


        [HttpPost]
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
                      .Include(d => d.Habitacion)
                      .Include(d => d.PlanAlimenticio)/*.ThenInclude(xv => xv.ListaCombinacionesDisponibles)
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
                    return Ok(new { token = "Error Cargando el Token" });
                }





                access_token = dictionary["accessToken"];
                realmId = dictionary["realmId"];
            }
            catch (Exception ex)
            {


                return Ok(new { token = "Error connectandose a QB" });
            }



            OAuth2RequestValidator oauthValidator = new OAuth2RequestValidator(access_token);
            // Create a ServiceContext with Auth tokens and realmId
            ServiceContext serviceContext = new ServiceContext(realmId, IntuitServicesType.QBO, oauthValidator);
            serviceContext.IppConfiguration.MinorVersion.Qbo = "23";
            serviceContext.IppConfiguration.BaseUrl.Qbo = QboBaseUrl;

            QueryService<Customer> querySvc = new QueryService<Customer>(serviceContext);
            string EXISTING_ITEM_QUERYBYNAME = string.Format("select * from Customer where Id = '{0}' ", orden.Cliente.IdQB);

            Customer objCustomerFound;
            try
            {
                objCustomerFound = querySvc.ExecuteIdsQuery(EXISTING_ITEM_QUERYBYNAME).FirstOrDefault<Customer>();
            }
            catch (Exception ex)
            {

                // Create a QuickBooks QueryService using ServiceContext
                QueryService<Customer> querySvcCustomer = new QueryService<Customer>(serviceContext);



                Customer ObjItem = new Customer();
                ObjItem.DisplayName = orden.Cliente.Nombre;
                ObjItem.FamilyName = orden.Cliente.Nombre;
                ObjItem.GivenName = orden.Cliente.Nombre;
                ObjItem.ContactName = orden.Cliente.Nombre;
                ObjItem.Title = orden.Cliente.Nombre;
                ObjItem.PrimaryEmailAddr = new EmailAddress { Address = orden.Cliente.Correo };
                ObjItem.AlternatePhone = new TelephoneNumber { DeviceType = "LandLine", FreeFormNumber = orden.Cliente.Telefono };
                ObjItem.Mobile = new TelephoneNumber { DeviceType = "Mobile", FreeFormNumber = orden.Cliente.Telefono };
                ObjItem.PrimaryPhone = new TelephoneNumber { DeviceType = "Mobile", FreeFormNumber = orden.Cliente.Telefono };



                DataService dataService2 = new DataService(serviceContext);
                Customer customer = dataService2.Add(ObjItem);
                if (customer != null && !string.IsNullOrEmpty(customer.Id))
                {
                    objCustomerFound = customer;

                }
                else
                {
                    objCustomerFound = null;

                }
            }

            if (objCustomerFound == null)
            {
                return Ok(new { token = "Error creando el cliente" });
            }

            Invoice ObjInvoice = new Invoice();
            ObjInvoice.CustomerRef = new ReferenceType();
            ObjInvoice.CustomerRef.Value = objCustomerFound.Id; //Quickbooks online Customer Id
            ObjInvoice.DocNumber = orden.NumeroOrden;
            List<Line> LineList = new List<Line>();
            if (orden.ListaActividadOrden != null && orden.ListaActividadOrden.Any())
                foreach (var item in orden.ListaActividadOrden)
                {
                    decimal precio = item.PrecioOrden;
                    if (_context.PreciosOrdenModificados.Where(x => x.OrdenId == item.OrdenId && x.OrdenActividadId == item.OrdenActividadId).Count() > 0)
                        precio = _context.PreciosOrdenModificados.Where(x => x.OrdenId == item.OrdenId && x.OrdenActividadId == item.OrdenActividadId).Sum(x => x.Precio);
                    Line objLine = new Line();
                    objLine.DetailTypeSpecified = true;
                    objLine.DetailType = LineDetailTypeEnum.SalesItemLineDetail;
                    objLine.AmountSpecified = true;
                    objLine.Amount = precio;
                    var ca = item.CantAdulto == null ? "0" : item.CantAdulto.ToString();
                    var cm = (item.CantInfante ?? 0 + item.CantNino ?? 0).ToString();
                    objLine.Description = "Activity: " + item.Actividad.Nombre + ". " +
                                          "Date: " + item.FechaActividad.ToString("MM/dd/yyyy") + ". " +
                                          "Lugar: " + item.LugarActividad + ". " +
                                         "Adults: " + ca + ". " +
                                          "Childs:  " + cm + ". ";
                    SalesItemLineDetail salesItemLineDetail = new SalesItemLineDetail();
                    salesItemLineDetail.QtySpecified = true;
                    salesItemLineDetail.Qty = 1;
                    salesItemLineDetail.ItemRef = new ReferenceType();
                    salesItemLineDetail.AnyIntuitObject = precio;
                    salesItemLineDetail.ItemElementName = ItemChoiceType.UnitPrice;
                    salesItemLineDetail.ServiceDate = item.FechaActividad;
                    salesItemLineDetail.ServiceDateSpecified = true;


                    QueryService<Item> querySvc1 = new QueryService<Item>(serviceContext);
                    string EXISTING_ITEM_QUERYBYNAMEITEMPROD = string.Format("select * from Item where Id = '{0}' ", item.Actividad.IdQB);
                    Item itemProduct = querySvc1.ExecuteIdsQuery(EXISTING_ITEM_QUERYBYNAMEITEMPROD).FirstOrDefault<Item>();
                    if (itemProduct == null)
                    {
                        return Ok(new { token = "El producto no exite en QB: " + item.Actividad.Nombre });
                    }
                    salesItemLineDetail.ItemRef.Value = itemProduct.Id; //Quickbooks online Item Id
                    objLine.AnyIntuitObject = salesItemLineDetail;
                    LineList.Add(objLine);


                }
            if (orden.ListaTrasladoOrden != null && orden.ListaTrasladoOrden.Any())
                foreach (var item in orden.ListaTrasladoOrden)
                {
                    decimal precio = item.PrecioOrden;
                    if (_context.PreciosOrdenModificados.Where(x => x.OrdenId == item.OrdenId && x.OrdenTrasladoId == item.OrdenTrasladoId).Count() > 0)
                        precio = _context.PreciosOrdenModificados.Where(x => x.OrdenId == item.OrdenId && x.OrdenTrasladoId == item.OrdenTrasladoId).Sum(x => x.Precio);
                    Line objLine = new Line();
                    objLine.DetailTypeSpecified = true;
                    objLine.DetailType = LineDetailTypeEnum.SalesItemLineDetail;
                    objLine.AmountSpecified = true;
                    objLine.Amount = precio;
                    var ca = item.CantAdulto == null ? "0" : item.CantAdulto.ToString();
                    var cm = (item.CantInfante ?? 0 + item.CantNino ?? 0).ToString();
                    objLine.Description = "Ground Transportation: " + item.PuntoOrigen.Nombre + " - " + item.PuntoDestino.Nombre +
                                          "Date: " + item.FechaRecogida.ToString("MM/dd/yyyy") + ". " +
                                          "Adults: " + ca + ". " +
                                          "Childs:  " + cm + ". " +
                                          "Capacity: " + item.Traslado.CapacidadTraslado;
                    SalesItemLineDetail salesItemLineDetail = new SalesItemLineDetail();
                    salesItemLineDetail.QtySpecified = true;
                    salesItemLineDetail.Qty = 1;
                    salesItemLineDetail.ItemRef = new ReferenceType();
                    salesItemLineDetail.AnyIntuitObject = precio;
                    salesItemLineDetail.ItemElementName = ItemChoiceType.UnitPrice;
                    salesItemLineDetail.ServiceDate = item.FechaRecogida;
                    salesItemLineDetail.ServiceDateSpecified = true;

                    QueryService<Item> querySvc1 = new QueryService<Item>(serviceContext);
                    string EXISTING_ITEM_QUERYBYNAMEITEMPROD = string.Format("select * from Item where Id = '{0}' ", item.Traslado.IdQB);
                    Item itemProduct = querySvc1.ExecuteIdsQuery(EXISTING_ITEM_QUERYBYNAMEITEMPROD).FirstOrDefault<Item>();
                    if (itemProduct == null)
                    {
                        return Ok(new { token = "El producto no exite en QB: " + item.Traslado.Nombre });
                    }
                    salesItemLineDetail.ItemRef.Value = itemProduct.Id; //Quickbooks online Item Id
                    objLine.AnyIntuitObject = salesItemLineDetail;
                    LineList.Add(objLine);


                }

            if (orden.ListaVehiculosOrden != null && orden.ListaVehiculosOrden.Any())
                foreach (var item in orden.ListaVehiculosOrden)
                {
                    decimal precio = item.PrecioOrden;
                    if (_context.PreciosOrdenModificados.Where(x => x.OrdenId == item.OrdenId && x.OrdenVehiculoId == item.OrdenVehiculoId).Count() > 0)
                        precio = _context.PreciosOrdenModificados.Where(x => x.OrdenId == item.OrdenId && x.OrdenVehiculoId == item.OrdenVehiculoId).Sum(x => x.Precio);
                    Line objLine = new Line();
                    objLine.DetailTypeSpecified = true;
                    objLine.DetailType = LineDetailTypeEnum.SalesItemLineDetail;
                    objLine.AmountSpecified = true;
                    objLine.Amount = precio;
                    objLine.Description = /*"Vehicle: " + item.Vehiculo.Nombre +*/
                                         "Date: " + item.FechaRecogida.ToString("MM/dd/yyyy") + " - " + item.FechaEntrega.ToString("MM/dd/yyyy");

                    SalesItemLineDetail salesItemLineDetail = new SalesItemLineDetail();
                    salesItemLineDetail.QtySpecified = true;
                    salesItemLineDetail.Qty = salesItemLineDetail.Qty = (item.FechaEntrega - item.FechaRecogida).Days;
                    salesItemLineDetail.ItemRef = new ReferenceType();
                    salesItemLineDetail.AnyIntuitObject = precio / salesItemLineDetail.Qty;
                    salesItemLineDetail.ItemElementName = ItemChoiceType.UnitPrice;
                    salesItemLineDetail.ServiceDate = item.FechaRecogida;
                    salesItemLineDetail.ServiceDateSpecified = true;

                    QueryService<Item> querySvc1 = new QueryService<Item>(serviceContext);
                    string EXISTING_ITEM_QUERYBYNAMEITEMPROD = string.Format("select * from Item where Id = '{0}' ", item.Vehiculo.IdQB);
                    Item itemProduct = querySvc1.ExecuteIdsQuery(EXISTING_ITEM_QUERYBYNAMEITEMPROD).FirstOrDefault<Item>();
                    if (itemProduct == null)
                    {
                        return Ok(new { token = "El producto no exite en QB: " + item.Vehiculo.Nombre });
                    }
                    salesItemLineDetail.ItemRef.Value = itemProduct.Id; //Quickbooks online Item Id
                    objLine.AnyIntuitObject = salesItemLineDetail;
                    LineList.Add(objLine);


                }

            if (orden.ListaAlojamientoOrden != null && orden.ListaAlojamientoOrden.Any())
                foreach (var item in orden.ListaAlojamientoOrden)
                {
                    decimal precio = item.PrecioOrden;
                    if (_context.PreciosOrdenModificados.Where(x => x.OrdenId == item.OrdenId && x.OrdenAlojamientoId == item.OrdenAlojamientoId).Count() > 0)
                        precio = _context.PreciosOrdenModificados.Where(x => x.OrdenId == item.OrdenId && x.OrdenAlojamientoId == item.OrdenAlojamientoId).Sum(x => x.Precio);
                    Line objLine = new Line();
                    objLine.DetailTypeSpecified = true;
                    objLine.DetailType = LineDetailTypeEnum.SalesItemLineDetail;
                    objLine.AmountSpecified = true;
                    objLine.Amount = precio;
                    var ca = item.CantAdulto == null ? "0" : item.CantAdulto.ToString();
                    var cm = (item.CantInfante ?? 0 + item.CantNino ?? 0).ToString();
                    objLine.Description = "Accommodation: " + item.Alojamiento.Nombre + ". " +
                                          "Date: " + item.FechaInicio.ToString("MM/dd/yyyy").Substring(0, 10) + " - " + item.FechaFin.ToString().Substring(0, 10) + ". " +
                                          "Booking Categories: " + item.PlanAlimenticio.Nombre + ". " +
                                          "Adults: " + ca + ". " +
                                          "Childs:  " + cm;
                    SalesItemLineDetail salesItemLineDetail = new SalesItemLineDetail();
                    salesItemLineDetail.QtySpecified = true;
                    salesItemLineDetail.Qty = 1;
                    salesItemLineDetail.ItemRef = new ReferenceType();
                    
                    QueryService<Item> querySvc1 = new QueryService<Item>(serviceContext);
                    string EXISTING_ITEM_QUERYBYNAMEITEMPROD = string.Format("select * from Item where Id = '{0}' ", item.Habitacion.IdQB);
                    Item itemProduct = querySvc1.ExecuteIdsQuery(EXISTING_ITEM_QUERYBYNAMEITEMPROD).FirstOrDefault<Item>();
                    if (itemProduct == null)
                    {
                        return Ok(new { token = "El producto no exite en QB: " + item.Alojamiento.Nombre });
                    }
                    salesItemLineDetail.AnyIntuitObject = precio;
                    salesItemLineDetail.ItemElementName = ItemChoiceType.UnitPrice;
                    salesItemLineDetail.ServiceDate = item.FechaInicio;
                    salesItemLineDetail.ServiceDateSpecified = true;
                    salesItemLineDetail.ItemRef.Value = itemProduct.Id; //Quickbooks online Item Id
                    objLine.AnyIntuitObject = salesItemLineDetail;
                    LineList.Add(objLine);


                }




            ObjInvoice.Line = LineList.ToArray();
            DataService dataService = new DataService(serviceContext);
            Invoice InvoiceAdd = dataService.Add(ObjInvoice);
            if (InvoiceAdd != null && !string.IsNullOrEmpty(InvoiceAdd.Id))
            {
                orden.IdInvoiceQB = int.Parse(InvoiceAdd.Id);
                _context.Entry(orden).State = EntityState.Modified;
                _context.SaveChanges();
                //you can write Database code here
                return Ok(new { token = "Se creo el Invoice" });
            }


            return Ok(new { token = "No se encontro el Invoice" });




        }


        [HttpPost]
        [Route("updateInvoice")]
        public async System.Threading.Tasks.Task<ActionResult> UpdateInvoice([FromBody] Orden orden)
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
                      .Include(d => d.Habitacion)
                      .Include(d => d.PlanAlimenticio)/*.ThenInclude(xv => xv.ListaCombinacionesDisponibles)
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
                    return Ok(new { token = "Error Cargando el Token" });
                }





                access_token = dictionary["accessToken"];
                realmId = dictionary["realmId"];
            }
            catch (Exception ex)
            {


                return Ok(new { token = "Error connectandose a QB" });
            }



            OAuth2RequestValidator oauthValidator = new OAuth2RequestValidator(access_token);
            // Create a ServiceContext with Auth tokens and realmId
            ServiceContext serviceContext = new ServiceContext(realmId, IntuitServicesType.QBO, oauthValidator);
            serviceContext.IppConfiguration.MinorVersion.Qbo = "23";
            serviceContext.IppConfiguration.BaseUrl.Qbo = QboBaseUrl;

            string EXISTING_INVOICE_QUERYBYID = string.Format("select * from Invoice where id = '{0}'", orden.IdInvoiceQB);
            var queryService = new QueryService<Invoice>(serviceContext);
            Invoice objInvoiceFound = queryService.ExecuteIdsQuery(EXISTING_INVOICE_QUERYBYID).FirstOrDefault<Invoice>();
            //If Invoice found on Quickbooks online
            if (objInvoiceFound != null)
            {

                QueryService<Customer> querySvc = new QueryService<Customer>(serviceContext);
                string EXISTING_ITEM_QUERYBYNAME = string.Format("select * from Customer where Id = '{0}' ", orden.Cliente.IdQB);

                Customer objCustomerFound;
                try
                {
                    objCustomerFound = querySvc.ExecuteIdsQuery(EXISTING_ITEM_QUERYBYNAME).FirstOrDefault<Customer>();
                }
                catch (Exception ex)
                {

                    // Create a QuickBooks QueryService using ServiceContext
                    QueryService<Customer> querySvcCustomer = new QueryService<Customer>(serviceContext);



                    Customer ObjItem = new Customer();
                    ObjItem.DisplayName = orden.Cliente.Nombre;
                    ObjItem.FamilyName = orden.Cliente.Nombre;
                    ObjItem.GivenName = orden.Cliente.Nombre;
                    ObjItem.ContactName = orden.Cliente.Nombre;
                    ObjItem.Title = orden.Cliente.Nombre;
                    ObjItem.PrimaryEmailAddr = new EmailAddress { Address = orden.Cliente.Correo };
                    ObjItem.AlternatePhone = new TelephoneNumber { DeviceType = "LandLine", FreeFormNumber = orden.Cliente.Telefono };
                    ObjItem.Mobile = new TelephoneNumber { DeviceType = "Mobile", FreeFormNumber = orden.Cliente.Telefono };
                    ObjItem.PrimaryPhone = new TelephoneNumber { DeviceType = "Mobile", FreeFormNumber = orden.Cliente.Telefono };



                    DataService dataService2 = new DataService(serviceContext);
                    Customer customer = dataService2.Add(ObjItem);
                    if (customer != null && !string.IsNullOrEmpty(customer.Id))
                    {
                        objCustomerFound = customer;

                    }
                    else
                    {
                        objCustomerFound = null;

                    }
                }

                if (objCustomerFound == null)
                {
                    return Ok(new { token = "Error creando el cliente" });
                }

                Invoice ObjInvoice = new Invoice();
                objInvoiceFound.Id = objInvoiceFound.Id;
                objInvoiceFound.CustomerRef = new ReferenceType();
                objInvoiceFound.CustomerRef.Value = objCustomerFound.Id; //Quickbooks online Customer Id
                objInvoiceFound.DocNumber = orden.NumeroOrden;
                List<Line> LineList = new List<Line>();
                if (orden.ListaActividadOrden != null && orden.ListaActividadOrden.Any())
                    foreach (var item in orden.ListaActividadOrden)
                    {
                        decimal precio = item.PrecioOrden;
                        if (_context.PreciosOrdenModificados.Where(x => x.OrdenId == item.OrdenId && x.OrdenActividadId == item.OrdenActividadId).Count() > 0)
                            precio = _context.PreciosOrdenModificados.Where(x => x.OrdenId == item.OrdenId && x.OrdenActividadId == item.OrdenActividadId).Sum(x => x.Precio);
                        Line objLine = new Line();
                        objLine.DetailTypeSpecified = true;
                        objLine.DetailType = LineDetailTypeEnum.SalesItemLineDetail;
                        objLine.AmountSpecified = true;
                        objLine.Amount = precio;
                        var ca = item.CantAdulto == null ? "0" : item.CantAdulto.ToString();
                        var cm = (item.CantInfante ?? 0 + item.CantNino ?? 0).ToString();
                        objLine.Description = "Activity: " + item.Actividad.Nombre + ". " +
                                              "Date: " + item.FechaActividad.ToString("MM/dd/yyyy") + ". " +
                                              "Lugar: " + item.LugarActividad + ". " +
                                             "Adults: " + ca + ". " +
                                              "Childs:  " + cm + ". ";
                        SalesItemLineDetail salesItemLineDetail = new SalesItemLineDetail();
                        salesItemLineDetail.QtySpecified = true;
                        salesItemLineDetail.Qty = 1;
                        salesItemLineDetail.ItemRef = new ReferenceType();
                        salesItemLineDetail.AnyIntuitObject = precio;
                        salesItemLineDetail.ItemElementName = ItemChoiceType.UnitPrice;
                        salesItemLineDetail.ServiceDate = item.FechaActividad;
                        salesItemLineDetail.ServiceDateSpecified = true;


                        QueryService<Item> querySvc1 = new QueryService<Item>(serviceContext);
                        string EXISTING_ITEM_QUERYBYNAMEITEMPROD = string.Format("select * from Item where Id = '{0}' ", item.Actividad.IdQB);
                        Item itemProduct = querySvc1.ExecuteIdsQuery(EXISTING_ITEM_QUERYBYNAMEITEMPROD).FirstOrDefault<Item>();
                        if (itemProduct == null)
                        {
                            return Ok(new { token = "El producto no exite en QB: " + item.Actividad.Nombre });
                        }
                        salesItemLineDetail.ItemRef.Value = itemProduct.Id; //Quickbooks online Item Id
                        objLine.AnyIntuitObject = salesItemLineDetail;
                        LineList.Add(objLine);


                    }
                if (orden.ListaTrasladoOrden != null && orden.ListaTrasladoOrden.Any())
                    foreach (var item in orden.ListaTrasladoOrden)
                    {
                        decimal precio = item.PrecioOrden;
                        if (_context.PreciosOrdenModificados.Where(x => x.OrdenId == item.OrdenId && x.OrdenTrasladoId == item.OrdenTrasladoId).Count() > 0)
                            precio = _context.PreciosOrdenModificados.Where(x => x.OrdenId == item.OrdenId && x.OrdenTrasladoId == item.OrdenTrasladoId).Sum(x => x.Precio);
                        Line objLine = new Line();
                        objLine.DetailTypeSpecified = true;
                        objLine.DetailType = LineDetailTypeEnum.SalesItemLineDetail;
                        objLine.AmountSpecified = true;
                        objLine.Amount = precio;
                        var ca = item.CantAdulto == null ? "0" : item.CantAdulto.ToString();
                        var cm = (item.CantInfante ?? 0 + item.CantNino ?? 0).ToString();
                        objLine.Description = "Ground Transportation: " + item.PuntoOrigen.Nombre + " - " + item.PuntoDestino.Nombre +
                                              "Date: " + item.FechaRecogida.ToString("MM/dd/yyyy") + ". " +
                                              "Adults: " + ca + ". " +
                                              "Childs:  " + cm + ". " +
                                              "Capacity: " + item.Traslado.CapacidadTraslado;
                        SalesItemLineDetail salesItemLineDetail = new SalesItemLineDetail();
                        salesItemLineDetail.QtySpecified = true;
                        salesItemLineDetail.Qty = 1;
                        salesItemLineDetail.ItemRef = new ReferenceType();
                        salesItemLineDetail.AnyIntuitObject = precio;
                        salesItemLineDetail.ItemElementName = ItemChoiceType.UnitPrice;
                        salesItemLineDetail.ServiceDate = item.FechaRecogida;
                        salesItemLineDetail.ServiceDateSpecified = true;

                        QueryService<Item> querySvc1 = new QueryService<Item>(serviceContext);
                        string EXISTING_ITEM_QUERYBYNAMEITEMPROD = string.Format("select * from Item where Id = '{0}' ", item.Traslado.IdQB);
                        Item itemProduct = querySvc1.ExecuteIdsQuery(EXISTING_ITEM_QUERYBYNAMEITEMPROD).FirstOrDefault<Item>();
                        if (itemProduct == null)
                        {
                            return Ok(new { token = "El producto no exite en QB: " + item.Traslado.Nombre });
                        }
                        salesItemLineDetail.ItemRef.Value = itemProduct.Id; //Quickbooks online Item Id
                        objLine.AnyIntuitObject = salesItemLineDetail;
                        LineList.Add(objLine);


                    }

                if (orden.ListaVehiculosOrden != null && orden.ListaVehiculosOrden.Any())
                    foreach (var item in orden.ListaVehiculosOrden)
                    {
                        decimal precio = item.PrecioOrden;
                        if (_context.PreciosOrdenModificados.Where(x => x.OrdenId == item.OrdenId && x.OrdenVehiculoId == item.OrdenVehiculoId).Count() > 0)
                            precio = _context.PreciosOrdenModificados.Where(x => x.OrdenId == item.OrdenId && x.OrdenVehiculoId == item.OrdenVehiculoId).Sum(x => x.Precio);
                        Line objLine = new Line();
                        objLine.DetailTypeSpecified = true;
                        objLine.DetailType = LineDetailTypeEnum.SalesItemLineDetail;
                        objLine.AmountSpecified = true;
                        objLine.Amount = precio;
                        objLine.Description = /*"Vehicle: " + item.Vehiculo.Nombre +*/
                                          "Date: " + item.FechaRecogida.ToString("MM/dd/yyyy") + " - " + item.FechaEntrega.ToString("MM/dd/yyyy");

                        SalesItemLineDetail salesItemLineDetail = new SalesItemLineDetail();
                        salesItemLineDetail.QtySpecified = true;
                        salesItemLineDetail.Qty = (item.FechaEntrega - item.FechaRecogida).Days;
                        salesItemLineDetail.ItemRef = new ReferenceType();
                        salesItemLineDetail.AnyIntuitObject = precio / salesItemLineDetail.Qty;
                        salesItemLineDetail.ItemElementName = ItemChoiceType.UnitPrice;
                        salesItemLineDetail.ServiceDate = item.FechaRecogida;
                        salesItemLineDetail.ServiceDateSpecified = true;

                        QueryService<Item> querySvc1 = new QueryService<Item>(serviceContext);
                        string EXISTING_ITEM_QUERYBYNAMEITEMPROD = string.Format("select * from Item where Id = '{0}' ", item.Vehiculo.IdQB);
                        Item itemProduct = querySvc1.ExecuteIdsQuery(EXISTING_ITEM_QUERYBYNAMEITEMPROD).FirstOrDefault<Item>();
                        if (itemProduct == null)
                        {
                            return Ok(new { token = "El producto no exite en QB: " + item.Vehiculo.Nombre });
                        }
                        salesItemLineDetail.ItemRef.Value = itemProduct.Id; //Quickbooks online Item Id
                        objLine.AnyIntuitObject = salesItemLineDetail;
                        LineList.Add(objLine);


                    }

                if (orden.ListaAlojamientoOrden != null && orden.ListaAlojamientoOrden.Any())
                    foreach (var item in orden.ListaAlojamientoOrden)
                    {
                        decimal precio = item.PrecioOrden;
                        if (_context.PreciosOrdenModificados.Where(x => x.OrdenId == item.OrdenId && x.OrdenAlojamientoId == item.OrdenAlojamientoId).Count() > 0)
                            precio = _context.PreciosOrdenModificados.Where(x => x.OrdenId == item.OrdenId && x.OrdenAlojamientoId == item.OrdenAlojamientoId).Sum(x => x.Precio);
                        Line objLine = new Line();
                        objLine.DetailTypeSpecified = true;
                        objLine.DetailType = LineDetailTypeEnum.SalesItemLineDetail;
                        objLine.AmountSpecified = true;
                        objLine.Amount = precio;
                        var ca = item.CantAdulto == null ? "0" : item.CantAdulto.ToString();
                        var cm = (item.CantInfante ?? 0 + item.CantNino ?? 0).ToString();
                        objLine.Description = "Accommodation: " + item.Alojamiento.Nombre + ". " +
                                              "Date: " + item.FechaInicio.ToString("MM/dd/yyyy").Substring(0, 10) + " - " + item.FechaFin.ToString().Substring(0, 10) + ". " +
                                              "Booking Categories: " + item.PlanAlimenticio.Nombre + ". " +
                                              "Adults: " + ca + ". " +
                                              "Childs:  " + cm;
                        SalesItemLineDetail salesItemLineDetail = new SalesItemLineDetail();
                        salesItemLineDetail.QtySpecified = true;
                        salesItemLineDetail.Qty = 1;
                        salesItemLineDetail.ItemRef = new ReferenceType();

                        QueryService<Item> querySvc1 = new QueryService<Item>(serviceContext);
                        string EXISTING_ITEM_QUERYBYNAMEITEMPROD = string.Format("select * from Item where Id = '{0}' ", item.Habitacion.IdQB);
                        Item itemProduct = querySvc1.ExecuteIdsQuery(EXISTING_ITEM_QUERYBYNAMEITEMPROD).FirstOrDefault<Item>();
                        if (itemProduct == null)
                        {
                            return Ok(new { token = "El producto no exite en QB: " + item.Alojamiento.Nombre });
                        }
                        salesItemLineDetail.AnyIntuitObject = precio;
                        salesItemLineDetail.ItemElementName = ItemChoiceType.UnitPrice;
                        salesItemLineDetail.ServiceDate = item.FechaInicio;
                        salesItemLineDetail.ServiceDateSpecified = true;
                        salesItemLineDetail.ItemRef.Value = itemProduct.Id; //Quickbooks online Item Id
                        objLine.AnyIntuitObject = salesItemLineDetail;
                        LineList.Add(objLine);


                    }




                objInvoiceFound.Line = LineList.ToArray();
                DataService dataService = new DataService(serviceContext);
                Invoice UpdateEntity = dataService.Update<Invoice>(objInvoiceFound);
                if (UpdateEntity != null && !string.IsNullOrEmpty(UpdateEntity.Id))
                {

                    //you can write Database code here
                    return Ok(new { token = "Se actualizo el invoice" });
                }

            }




            return Ok(new { token = "No se encontro el producto" });




        }

        [HttpPost]
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
               .Include(d => d.Actividad).ThenInclude(xx => xx.Proveedor)/*.ThenInclude(l => l.ListaDistribuidoresProducto)
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
                      .Include(d => d.Habitacion)
                      .Include(d => d.PlanAlimenticio)/*.ThenInclude(xv => xv.ListaCombinacionesDisponibles)
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
                    return Ok(new { token = "Error Cargando el Token" });
                }





                access_token = dictionary["accessToken"];
                realmId = dictionary["realmId"];
            }
            catch (Exception ex)
            {


                return Ok(new { token = "Error connectandose a QB" });
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
                    ObjBill.DocNumber = orden.NumeroOrden;
                    QueryService<Vendor> querySvcI = new QueryService<Vendor>(serviceContext);

                    List<Vendor> proveedores = querySvcI.ExecuteIdsQuery("SELECT * from Vendor ").Where(x => x.CompanyName == item.Actividad.Proveedor.Nombre).ToList();
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
                    var ca = item.CantAdulto == null ? "0" : item.CantAdulto.ToString();
                    var cm = (item.CantInfante ?? 0 + item.CantNino ?? 0).ToString();
                    objLine.Description = "Activity: " + item.Actividad.Nombre + ". " +
                                          "Date: " + item.FechaActividad.ToString("MM/dd/yyyy") + ". " +
                                          "Lugar: " + item.LugarActividad + ". " +
                                         "Adults: " + ca + ". " +
                                          "Childs:  " + cm + ". ";
                   

                    AccountBasedExpenseLineDetail ItemLineDetail = new AccountBasedExpenseLineDetail();
                     ItemLineDetail.AccountRef = new ReferenceType();
                     ItemLineDetail.AccountRef.Value = "78"; //Quickbooks online Account Id
                                                             // We can give Account Name insted of Account Id, if we give Account Id and Account Name both then Account name will be ignore.
                                                             //ItemLineDetail.AccountRef.name = "Purchases"; //Quickbooks online Account Name*/
                    objLine.AnyIntuitObject = ItemLineDetail;

                    QueryService<Item> querySvc1 = new QueryService<Item>(serviceContext);
                    string EXISTING_ITEM_QUERYBYNAMEITEMPROD = string.Format("select * from Item where Id = '{0}' ", item.Actividad.IdQB);
                    Item itemProduct = querySvc1.ExecuteIdsQuery(EXISTING_ITEM_QUERYBYNAMEITEMPROD).FirstOrDefault<Item>();
                    if (itemProduct == null)
                    {
                        return Ok(new { token = "El producto no exite en QB: " + item.Actividad.Nombre });
                    }

                    LineList.Add(objLine);


                    ObjBill.Line = LineList.ToArray();
                    DataService dataService = new DataService(serviceContext);
                    Bill BillAdd = dataService.Add(ObjBill);
                    if (BillAdd != null && !string.IsNullOrEmpty(BillAdd.Id))
                    {
                        item.IdBillQB = int.Parse(BillAdd.Id);
                        _context.Entry(item).State = EntityState.Modified;
                        _context.SaveChanges();
                        //you can write Database code here
                        Ok("Se creo el bill");
                    }
                    else
                    {

                        return Ok(new { token = "No se encontro el producto" });
                    }
                }

            }
            if (orden.ListaTrasladoOrden != null && orden.ListaTrasladoOrden.Any())
            {
                foreach (var item in orden.ListaTrasladoOrden)
                {
                    Bill ObjBill = new Bill();
                    ObjBill.DocNumber = orden.NumeroOrden;
                    QueryService<Vendor> querySvcI = new QueryService<Vendor>(serviceContext);

                    List<Vendor> proveedores = querySvcI.ExecuteIdsQuery("SELECT * from Vendor ").Where(x => x.CompanyName == item.Traslado.Proveedor.Nombre).ToList();
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
                    objLine.DetailType = LineDetailTypeEnum.AccountBasedExpenseLineDetail;
                    objLine.AmountSpecified = true;
                    objLine.Amount = item.PrecioOrden - item.ValorSobreprecioAplicado - (item.ValorSobreprecioAplicado * orden.Cliente.Descuento / 100); ;
                    var ca = item.CantAdulto == null ? "0" : item.CantAdulto.ToString();
                    var cm = (item.CantInfante ?? 0 + item.CantNino ?? 0).ToString();
                    objLine.Description = "Ground Transportation: " + item.PuntoOrigen.Nombre + " - " + item.PuntoDestino.Nombre +
                                          "Date: " + item.FechaRecogida.ToString("MM/dd/yyyy") + ". " +
                                          "Adults: " + ca + ". " +
                                          "Childs:  " + cm + ". " +
                                          "Capacity: " + item.Traslado.CapacidadTraslado;
                   
                    AccountBasedExpenseLineDetail ItemLineDetail = new AccountBasedExpenseLineDetail();
                     ItemLineDetail.AccountRef = new ReferenceType();
                     ItemLineDetail.AccountRef.Value = "78"; //Quickbooks online Account Id
                                                             // We can give Account Name insted of Account Id, if we give Account Id and Account Name both then Account name will be ignore.
                                                             //ItemLineDetail.AccountRef.name = "Purchases"; //Quickbooks online Account Name*/
                    objLine.AnyIntuitObject = ItemLineDetail;

                    QueryService<Item> querySvc1 = new QueryService<Item>(serviceContext);
                    string EXISTING_ITEM_QUERYBYNAMEITEMPROD = string.Format("select * from Item where Id = '{0}' ", item.Traslado.IdQB);
                    Item itemProduct = querySvc1.ExecuteIdsQuery(EXISTING_ITEM_QUERYBYNAMEITEMPROD).FirstOrDefault<Item>();
                    if (itemProduct == null)
                    {
                        return Ok(new { token = "El producto no exite en QB: " + item.Traslado.Nombre });
                    }

                    LineList.Add(objLine);

                    ObjBill.Line = LineList.ToArray();
                    DataService dataService = new DataService(serviceContext);
                    Bill BillAdd = dataService.Add(ObjBill);
                    if (BillAdd != null && !string.IsNullOrEmpty(BillAdd.Id))
                    {
                        item.IdBillQB = int.Parse(BillAdd.Id);
                        _context.Entry(item).State = EntityState.Modified;
                        _context.SaveChanges();
                        //you can write Database code here
                        Ok("Se creo el bill");
                    }
                    else
                    {

                        return Ok(new { token = "No se encontro el producto" });
                    }
                }

            }

            if (orden.ListaVehiculosOrden != null && orden.ListaVehiculosOrden.Any())
            {
                foreach (var item in orden.ListaVehiculosOrden)
                {
                    Bill ObjBill = new Bill();
                    ObjBill.DocNumber = orden.NumeroOrden;
                    QueryService<Vendor> querySvcI = new QueryService<Vendor>(serviceContext);

                    List<Vendor> proveedores = querySvcI.ExecuteIdsQuery("SELECT * from Vendor ").Where(x => x.CompanyName == item.Vehiculo.Proveedor.Nombre).ToList();
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
                    objLine.DetailType = LineDetailTypeEnum.AccountBasedExpenseLineDetail;
                    objLine.AmountSpecified = true;
                    objLine.Amount = item.PrecioOrden - item.ValorSobreprecioAplicado - (item.ValorSobreprecioAplicado * orden.Cliente.Descuento / 100); ;
                    objLine.Description = /*"Vehicle: " + item.Vehiculo.Nombre +*/
                                          "Date: " + item.FechaRecogida.ToString("MM/dd/yyyy") + " - " + item.FechaEntrega.ToString("MM/dd/yyyy");

                    AccountBasedExpenseLineDetail ItemLineDetail = new AccountBasedExpenseLineDetail();
                    ItemLineDetail.AccountRef = new ReferenceType();
                    ItemLineDetail.AccountRef.Value = "78"; //Quickbooks online Account Id
                                                            // We can give Account Name insted of Account Id, if we give Account Id and Account Name both then Account name will be ignore.
                                                            //ItemLineDetail.AccountRef.name = "Purchases"; //Quickbooks online Account Name

                    QueryService<Item> querySvc1 = new QueryService<Item>(serviceContext);
                    string EXISTING_ITEM_QUERYBYNAMEITEMPROD = string.Format("select * from Item where Id = '{0}' ", item.Vehiculo.IdQB);
                    Item itemProduct = querySvc1.ExecuteIdsQuery(EXISTING_ITEM_QUERYBYNAMEITEMPROD).FirstOrDefault<Item>();
                    if (itemProduct == null)
                    {
                        return Ok(new { token = "El producto no exite en QB: " + item.Vehiculo.Nombre });
                    }

                    objLine.AnyIntuitObject = ItemLineDetail;
                    LineList.Add(objLine);

                    ObjBill.Line = LineList.ToArray();
                    DataService dataService = new DataService(serviceContext);
                    Bill BillAdd = dataService.Add(ObjBill);
                    if (BillAdd != null && !string.IsNullOrEmpty(BillAdd.Id))
                    {
                        item.IdBillQB = int.Parse(BillAdd.Id);
                        _context.Entry(item).State = EntityState.Modified;
                        _context.SaveChanges();
                        //you can write Database code here
                        Ok("Se creo el bill");
                    }
                    else
                    {

                        return Ok(new { token = "No se encontro el producto" });
                    }

                }

            }


            if (orden.ListaAlojamientoOrden != null && orden.ListaAlojamientoOrden.Any())
            {
                foreach (var item in orden.ListaAlojamientoOrden)
                {
                    Bill ObjBill = new Bill();
                    ObjBill.DocNumber = orden.NumeroOrden;
                    QueryService<Vendor> querySvcI = new QueryService<Vendor>(serviceContext);

                    List<Vendor> proveedores = querySvcI.ExecuteIdsQuery("SELECT * from Vendor ").Where(x => x.CompanyName == item.Alojamiento.Proveedor.Nombre).ToList();
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
                    objLine.DetailType = LineDetailTypeEnum.AccountBasedExpenseLineDetail;
                    objLine.AmountSpecified = true;
                    objLine.Amount = item.PrecioOrden - item.ValorSobreprecioAplicado - (item.ValorSobreprecioAplicado * orden.Cliente.Descuento / 100); ;
                    var ca = item.CantAdulto == null ? "0" : item.CantAdulto.ToString();
                    var cm = (item.CantInfante ?? 0 + item.CantNino ?? 0).ToString();
                    objLine.Description = "Accommodation: " + item.Alojamiento.Nombre + ". " +
                                          "Date: " + item.FechaInicio.ToString("MM/dd/yyyy") + " - " + item.FechaFin.ToString("MM/dd/yyyy") + ". " +
                                          "Booking Categories: " + item.PlanAlimenticio.Nombre + ". " +
                                          "Adults: " + ca + ". " +
                                          "Childs:  " + cm;
                    AccountBasedExpenseLineDetail ItemLineDetail = new AccountBasedExpenseLineDetail();
                    ItemLineDetail.AccountRef = new ReferenceType();
                    ItemLineDetail.AccountRef.Value = "78"; //Quickbooks online Account Id
                                                            // We can give Account Name insted of Account Id, if we give Account Id and Account Name both then Account name will be ignore.
                                                            //ItemLineDetail.AccountRef.name = "Purchases"; //Quickbooks online Account Name
                    QueryService<Item> querySvc1 = new QueryService<Item>(serviceContext);
                    string EXISTING_ITEM_QUERYBYNAMEITEMPROD = string.Format("select * from Item where Id = '{0}' ", item.Habitacion.IdQB);
                    Item itemProduct = querySvc1.ExecuteIdsQuery(EXISTING_ITEM_QUERYBYNAMEITEMPROD).FirstOrDefault<Item>();
                    if (itemProduct == null)
                    {
                        return Ok(new { token = "El producto no exite en QB: " + item.Alojamiento.Nombre });
                    }

                    objLine.AnyIntuitObject = ItemLineDetail;
                    LineList.Add(objLine);

                    ObjBill.Line = LineList.ToArray();
                    DataService dataService = new DataService(serviceContext);
                    Bill BillAdd = dataService.Add(ObjBill);
                    if (BillAdd != null && !string.IsNullOrEmpty(BillAdd.Id))
                    {
                        item.IdBillQB = int.Parse(BillAdd.Id);
                        _context.Entry(item).State = EntityState.Modified;
                        _context.SaveChanges();
                        //you can write Database code here
                        Ok("Se creo el bill");
                    }
                    else
                    {

                        return Ok(new { token = "No se encontro el producto" });
                    }

                }

            }





            return Ok(new { token = "Proceso correcto" });




        }



        [HttpPost]
        [Route("updateBill")]
        public async System.Threading.Tasks.Task<ActionResult> UpdateBill([FromBody] Orden orden)
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
               .Include(d => d.Actividad).ThenInclude(xx => xx.Proveedor)/*.ThenInclude(l => l.ListaDistribuidoresProducto)
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
                      .Include(d => d.Habitacion)
                      .Include(d => d.PlanAlimenticio)/*.ThenInclude(xv => xv.ListaCombinacionesDisponibles)
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
                    return Ok(new { token = "Error Cargando el Token" });
                }





                access_token = dictionary["accessToken"];
                realmId = dictionary["realmId"];
            }
            catch (Exception ex)
            {


                return Ok(new { token = "Error connectandose a QB" });
            }



            OAuth2RequestValidator oauthValidator = new OAuth2RequestValidator(access_token);
            // Create a ServiceContext with Auth tokens and realmId
            ServiceContext serviceContext = new ServiceContext(realmId, IntuitServicesType.QBO, oauthValidator);
            serviceContext.IppConfiguration.MinorVersion.Qbo = "23";
            serviceContext.IppConfiguration.BaseUrl.Qbo = QboBaseUrl;




            if (orden.ListaActividadOrden != null && orden.ListaActividadOrden.Any())
            {

                foreach (var item in orden.ListaActividadOrden)
                {

                    QueryService<Vendor> querySvcI = new QueryService<Vendor>(serviceContext);

                    List<Vendor> proveedores = querySvcI.ExecuteIdsQuery("SELECT * from Vendor ").Where(x => x.CompanyName == item.Actividad.Proveedor.Nombre).ToList();
                    Vendor VendorRef = null;
                    if (proveedores != null && proveedores.Any())
                    {

                        VendorRef = proveedores.First();
                    }
                    else
                    {
                        VendorRef = agregarVendorProveedor(item.Actividad.Proveedor, serviceContext);
                    }


                    string EXISTING_BILL_QUERYBYID = string.Format("select * from bill where id = '{0}'", item.IdBillQB);
                    var queryService = new QueryService<Bill>(serviceContext);
                    Bill objBillFound = queryService.ExecuteIdsQuery(EXISTING_BILL_QUERYBYID).FirstOrDefault<Bill>();
                    //If Bill found on Quickbooks online
                    if (objBillFound != null)
                    {
                        Bill ObjBill = new Bill();
                        ObjBill.DocNumber = orden.NumeroOrden;
                        objBillFound.Id = objBillFound.Id;
                        objBillFound.SyncToken = objBillFound.SyncToken;
                        objBillFound.VendorRef = new ReferenceType();
                        objBillFound.VendorRef = objBillFound.VendorRef;
                        List<Line> LineList = new List<Line>();
                        Line objLine = new Line();
                        objLine.DetailTypeSpecified = true;
                        objLine.DetailType = LineDetailTypeEnum.AccountBasedExpenseLineDetail;
                        objLine.AmountSpecified = true;
                        objLine.Amount = item.PrecioOrden - item.ValorSobreprecioAplicado - (item.ValorSobreprecioAplicado * orden.Cliente.Descuento / 100); ;
                        var ca = item.CantAdulto == null ? "0" : item.CantAdulto.ToString();
                        var cm = (item.CantInfante ?? 0 + item.CantNino ?? 0).ToString();
                        objLine.Description = "Activity: " + item.Actividad.Nombre + ". " +
                                              "Date: " + item.FechaActividad.ToString("MM/dd/yyyy") + ". " +
                                              "Lugar: " + item.LugarActividad + ". " +
                                             "Adults: " + ca + ". " +
                                              "Childs:  " + cm + ". ";
                       

                        AccountBasedExpenseLineDetail ItemLineDetail = new AccountBasedExpenseLineDetail();
                        ItemLineDetail.AccountRef = new ReferenceType();
                        ItemLineDetail.AccountRef.Value = "78"; //Quickbooks online Account Id
                                                                // We can give Account Name insted of Account Id, if we give Account Id and Account Name both then Account name will be ignore.
                                                                //ItemLineDetail.AccountRef.name = "Purchases"; //Quickbooks online Account Name
                        objLine.AnyIntuitObject = ItemLineDetail;
                        LineList.Add(objLine);
                        objBillFound.Line = LineList.ToArray();
                        DataService dataService = new DataService(serviceContext);
                        Bill UpdateEntity = dataService.Update<Bill>(objBillFound);
                        if (UpdateEntity != null && !string.IsNullOrEmpty(UpdateEntity.Id))
                        {
                            //you can write Database code here
                            Ok("Se actualizo");
                        }




                    }
                    else
                    {

                        return Ok(new { token = "No se encontro el producto" });
                    }
                }

            }
            if (orden.ListaTrasladoOrden != null && orden.ListaTrasladoOrden.Any())
            {
                foreach (var item in orden.ListaTrasladoOrden)
                {

                    QueryService<Vendor> querySvcI = new QueryService<Vendor>(serviceContext);

                    List<Vendor> proveedores = querySvcI.ExecuteIdsQuery("SELECT * from Vendor ").Where(x => x.CompanyName == item.Traslado.Proveedor.Nombre).ToList();
                    Vendor VendorRef = null;
                    if (proveedores != null && proveedores.Any())
                    {

                        VendorRef = proveedores.First();
                    }
                    else
                    {
                        VendorRef = agregarVendorProveedor(item.Traslado.Proveedor, serviceContext);
                    }
                    string EXISTING_BILL_QUERYBYID = string.Format("select * from bill where id = '{0}'", item.IdBillQB);
                    var queryService = new QueryService<Bill>(serviceContext);
                    Bill objBillFound = queryService.ExecuteIdsQuery(EXISTING_BILL_QUERYBYID).FirstOrDefault<Bill>();
                    //If Bill found on Quickbooks online
                    if (objBillFound != null)
                    {
                        Bill ObjBill = new Bill();
                        ObjBill.DocNumber = orden.NumeroOrden;
                        objBillFound.Id = objBillFound.Id;
                        objBillFound.SyncToken = objBillFound.SyncToken;
                        objBillFound.VendorRef = new ReferenceType();
                        objBillFound.VendorRef = objBillFound.VendorRef;
                        List<Line> LineList = new List<Line>();
                        Line objLine = new Line();
                        objLine.DetailTypeSpecified = true;
                        objLine.DetailType = LineDetailTypeEnum.AccountBasedExpenseLineDetail;
                        objLine.AmountSpecified = true;
                        objLine.Amount = item.PrecioOrden - item.ValorSobreprecioAplicado - (item.ValorSobreprecioAplicado * orden.Cliente.Descuento / 100); ;
                        var ca = item.CantAdulto == null ? "0" : item.CantAdulto.ToString();
                        var cm = (item.CantInfante ?? 0 + item.CantNino ?? 0).ToString();
                        objLine.Description = "Ground Transportation: " + item.PuntoOrigen.Nombre + " - " + item.PuntoDestino.Nombre +
                                              "Date: " + item.FechaRecogida.ToString("MM/dd/yyyy") + ". " +
                                              "Adults: " + ca + ". " +
                                              "Childs:  " + cm + ". " +
                                              "Capacity: " + item.Traslado.CapacidadTraslado;
                       
                        AccountBasedExpenseLineDetail ItemLineDetail = new AccountBasedExpenseLineDetail();
                        ItemLineDetail.AccountRef = new ReferenceType();
                        ItemLineDetail.AccountRef.Value = "78"; //Quickbooks online Account Id
                                                                // We can give Account Name insted of Account Id, if we give Account Id and Account Name both then Account name will be ignore.
                                                                //ItemLineDetail.AccountRef.name = "Purchases"; //Quickbooks online Account Name

                        objLine.AnyIntuitObject = ItemLineDetail;
                        LineList.Add(objLine);
                        objBillFound.Line = LineList.ToArray();
                        DataService dataService = new DataService(serviceContext);
                        Bill UpdateEntity = dataService.Update<Bill>(objBillFound);
                        if (UpdateEntity != null && !string.IsNullOrEmpty(UpdateEntity.Id))
                        {
                            //you can write Database code here
                            Ok("Se actualizo");
                        }




                    }
                    else
                    {

                        return Ok(new { token = "No se encontro el producto" });
                    }
                }

            }

            if (orden.ListaVehiculosOrden != null && orden.ListaVehiculosOrden.Any())
            {
                foreach (var item in orden.ListaVehiculosOrden)
                {

                    QueryService<Vendor> querySvcI = new QueryService<Vendor>(serviceContext);

                    List<Vendor> proveedores = querySvcI.ExecuteIdsQuery("SELECT * from Vendor ").Where(x => x.CompanyName == item.Vehiculo.Proveedor.Nombre).ToList();
                    Vendor VendorRef = null;
                    if (proveedores != null && proveedores.Any())
                    {

                        VendorRef = proveedores.First();
                    }
                    else
                    {
                        VendorRef = agregarVendorProveedor(item.Vehiculo.Proveedor, serviceContext);
                    }
                    string EXISTING_BILL_QUERYBYID = string.Format("select * from bill where id = '{0}'", item.IdBillQB);
                    var queryService = new QueryService<Bill>(serviceContext);
                    Bill objBillFound = queryService.ExecuteIdsQuery(EXISTING_BILL_QUERYBYID).FirstOrDefault<Bill>();
                    //If Bill found on Quickbooks online
                    if (objBillFound != null)
                    {
                        Bill ObjBill = new Bill();
                        objBillFound.Id = objBillFound.Id;
                        ObjBill.DocNumber = orden.NumeroOrden;
                        objBillFound.SyncToken = objBillFound.SyncToken;
                        objBillFound.VendorRef = new ReferenceType();
                        objBillFound.VendorRef = objBillFound.VendorRef;
                        List<Line> LineList = new List<Line>();
                        Line objLine = new Line();
                        objLine.DetailTypeSpecified = true;
                        objLine.DetailType = LineDetailTypeEnum.AccountBasedExpenseLineDetail;
                        objLine.AmountSpecified = true;
                        objLine.Amount = item.PrecioOrden - item.ValorSobreprecioAplicado - (item.ValorSobreprecioAplicado * orden.Cliente.Descuento / 100); ;
                        objLine.Description = /*"Vehicle: " + item.Vehiculo.Nombre +*/
                                          "Date: " + item.FechaRecogida.ToString("MM/dd/yyyy") + " - " + item.FechaEntrega.ToString("MM/dd/yyyy");

                       
                        
                        AccountBasedExpenseLineDetail ItemLineDetail = new AccountBasedExpenseLineDetail();
                        ItemLineDetail.AccountRef = new ReferenceType();
                        ItemLineDetail.AccountRef.Value = "78"; //Quickbooks online Account Id
                                                                // We can give Account Name insted of Account Id, if we give Account Id and Account Name both then Account name will be ignore.
                                                                //ItemLineDetail.AccountRef.name = "Purchases"; //Quickbooks online Account Name
                        objLine.AnyIntuitObject = ItemLineDetail;
                        LineList.Add(objLine);
                        objBillFound.Line = LineList.ToArray();
                        DataService dataService = new DataService(serviceContext);
                        Bill UpdateEntity = dataService.Update<Bill>(objBillFound);
                        if (UpdateEntity != null && !string.IsNullOrEmpty(UpdateEntity.Id))
                        {
                            //you can write Database code here
                            Ok("Se actualizo");
                        }




                    }
                    else
                    {

                        return Ok(new { token = "No se encontro el producto" });
                    }

                }

            }


            if (orden.ListaAlojamientoOrden != null && orden.ListaAlojamientoOrden.Any())
            {
                foreach (var item in orden.ListaAlojamientoOrden)
                {

                    QueryService<Vendor> querySvcI = new QueryService<Vendor>(serviceContext);

                    List<Vendor> proveedores = querySvcI.ExecuteIdsQuery("SELECT * from Vendor ").Where(x => x.CompanyName == item.Alojamiento.Proveedor.Nombre).ToList();
                    Vendor VendorRef = null;
                    if (proveedores != null && proveedores.Any())
                    {

                        VendorRef = proveedores.First();
                    }
                    else
                    {
                        VendorRef = agregarVendorProveedor(item.Alojamiento.Proveedor, serviceContext);
                    }
                    string EXISTING_BILL_QUERYBYID = string.Format("select * from bill where id = '{0}'", item.IdBillQB);
                    var queryService = new QueryService<Bill>(serviceContext);
                    Bill objBillFound = queryService.ExecuteIdsQuery(EXISTING_BILL_QUERYBYID).FirstOrDefault<Bill>();
                    //If Bill found on Quickbooks online
                    if (objBillFound != null)
                    {
                        Bill ObjBill = new Bill();
                        ObjBill.DocNumber = orden.NumeroOrden;
                        objBillFound.Id = objBillFound.Id;
                        objBillFound.SyncToken = objBillFound.SyncToken;
                        objBillFound.VendorRef = new ReferenceType();
                        objBillFound.VendorRef = objBillFound.VendorRef;
                        List<Line> LineList = new List<Line>();
                        Line objLine = new Line();
                        objLine.DetailTypeSpecified = true;
                        objLine.DetailType = LineDetailTypeEnum.AccountBasedExpenseLineDetail;
                        objLine.AmountSpecified = true;
                        objLine.Amount = item.PrecioOrden - item.ValorSobreprecioAplicado - (item.ValorSobreprecioAplicado * orden.Cliente.Descuento / 100);
                        var ca = item.CantAdulto == null ? "0" : item.CantAdulto.ToString();
                        var cm = (item.CantInfante ?? 0 + item.CantNino ?? 0).ToString();
                        objLine.Description = "Accommodation: " + item.Alojamiento.Nombre + ". " +
                                              "Date: " + item.FechaInicio.ToString("MM/dd/yyyy").Substring(0, 10) + " - " + item.FechaFin.ToString().Substring(0, 10) + ". " +
                                              "Booking Categories: " + item.PlanAlimenticio.Nombre + ". " +
                                              "Adults: " + ca + ". " +
                                              "Childs:  " + cm;
                        AccountBasedExpenseLineDetail ItemLineDetail = new AccountBasedExpenseLineDetail();
                        ItemLineDetail.AccountRef = new ReferenceType();
                        ItemLineDetail.AccountRef.Value = "78"; //Quickbooks online Account Id
                                                                // We can give Account Name insted of Account Id, if we give Account Id and Account Name both then Account name will be ignore.
                                                                //ItemLineDetail.AccountRef.name = "Purchases"; //Quickbooks online Account Name
                        objLine.AnyIntuitObject = ItemLineDetail;
                        LineList.Add(objLine);
                        objBillFound.Line = LineList.ToArray();
                        DataService dataService = new DataService(serviceContext);
                        Bill UpdateEntity = dataService.Update<Bill>(objBillFound);
                        if (UpdateEntity != null && !string.IsNullOrEmpty(UpdateEntity.Id))
                        {
                            //you can write Database code here
                            Ok("Se actualizo");
                        }




                    }
                    else
                    {

                        return Ok(new { token = "No se encontro el producto" });
                    }

                }

            }





            return Ok(new { token = "Proceso Correcto" });




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
            string EXISTING_QUERYBYID = "";
            if ( prov.IdQB != null && prov.IdQB > 0)
            {
                EXISTING_QUERYBYID = string.Format("SELECT * from Vendor where ID = '{0}'", prov.IdQB);
            }
            else
            {
                EXISTING_QUERYBYID = string.Format("SELECT * from Vendor where CompanyName = '{0}'", prov.Nombre);                
            }
            
           

            List<Vendor> proveedores = querySvcI.ExecuteIdsQuery(EXISTING_QUERYBYID).ToList<Vendor>();

            if (proveedores != null && proveedores.Any())
            {
                Vendor ven = proveedores.First();

                prov.IdQB = int.Parse(ven.Id);
                _context.Entry(prov).State = EntityState.Modified;
                _context.SaveChanges();
                return ven;

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
                prov.IdQB = int.Parse(VendorAdd.Id);
                _context.Entry(prov).State = EntityState.Modified;
                _context.SaveChanges();
                return VendorAdd;
            }
            return null;

        }


        [HttpGet]
        [Route("excelCliente")]
        public IActionResult ExportarExcelClientes()
        {
            string excelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var clientes = _context.Clientes.AsNoTracking().ToList();
            using (var libro = new ExcelPackage())
            {
                var worksheet = libro.Workbook.Worksheets.Add("Customer");



                /* worksheet.Cells["A1"].Value = "Nombre";
                 worksheet.Cells["B1"].Value = "Correo";
                 worksheet.Cells["C1"].Value = "Teléfono";
                 worksheet.Cells["D1"].Value = "Calle";
                 worksheet.Cells["A1"].Value = "Ciudad";
                 worksheet.Cells["A1"].Value = "ZIP";
                 worksheet.Cells["A1"].Value = "Pais";*/

                DataTable dataTable = new DataTable();

                dataTable.Columns.Add("Nombre", typeof(string));
                dataTable.Columns.Add("Correo", typeof(string));
                dataTable.Columns.Add("Telefono", typeof(string));
                dataTable.Columns.Add("Calle", typeof(string));
                dataTable.Columns.Add("Ciudad", typeof(string));
                dataTable.Columns.Add("ZIP", typeof(string));
                dataTable.Columns.Add("Pais", typeof(string));
                dataTable.Columns.Add("Estado", typeof(string));



                foreach (var item in clientes)
                {
                    DataRow fila = dataTable.NewRow();
                    fila["Nombre"] = item.Nombre;
                    fila["Correo"] = item.Correo;
                    fila["Telefono"] = item.Telefono;
                    fila["Calle"] = item.Calle;
                    fila["Ciudad"] = item.Ciudad;
                    fila["ZIP"] = item.ZipCode;
                    fila["Pais"] = item.Pais;
                    fila["Estado"] = item.Estado;




                    dataTable.Rows.Add(fila);
                }

                worksheet.Cells["A1"].LoadFromDataTable(dataTable, true);
                for (var col = 1; col < clientes.Count + 1; col++)
                {
                    worksheet.Column(col).AutoFit();
                }

                // Agregar formato de tabla
                var tabla = worksheet.Tables.Add(new ExcelAddressBase(fromRow: 1, fromCol: 1, toRow: clientes.Count + 1, toColumn: 8), "Customer");
                tabla.ShowHeader = true;
                tabla.TableStyle = TableStyles.Light6;
                tabla.ShowTotal = true;


                return File(libro.GetAsByteArray(), excelContentType, "Customer.xlsx");
            }

        }


        [HttpGet]
        [Route("excelVendor")]
        public IActionResult ExportarExcelVendor()
        {
            string excelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var proveedores = _context.Proveedores.AsNoTracking().ToList();
            using (var libro = new ExcelPackage())
            {
                var worksheet = libro.Workbook.Worksheets.Add("Vendor");




                DataTable dataTable = new DataTable();

                dataTable.Columns.Add("Nombre", typeof(string));
                dataTable.Columns.Add("Correo", typeof(string));
                dataTable.Columns.Add("Telefono", typeof(string));




                foreach (var item in proveedores)
                {
                    DataRow fila = dataTable.NewRow();
                    fila["Nombre"] = item.Nombre;
                    fila["Correo"] = item.Correo;
                    fila["Telefono"] = item.Telefono;





                    dataTable.Rows.Add(fila);
                }

                worksheet.Cells["A1"].LoadFromDataTable(dataTable, true);
                for (var col = 1; col < proveedores.Count + 1; col++)
                {
                    worksheet.Column(col).AutoFit();
                }

                // Agregar formato de tabla
                var tabla = worksheet.Tables.Add(new ExcelAddressBase(fromRow: 1, fromCol: 1, toRow: proveedores.Count + 1, toColumn: 3), "Vendor");
                tabla.ShowHeader = true;
                tabla.TableStyle = TableStyles.Light6;
                tabla.ShowTotal = true;


                return File(libro.GetAsByteArray(), excelContentType, "Vendors.xlsx");
            }

        }


        [HttpGet]
        [Route("excelProducto")]
        public IActionResult ExportarExcelProducto()
        {
            string excelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var productos = _context.Productos.Include(x=>x.Proveedor).AsNoTracking().ToList();


            using (var libro = new ExcelPackage())
            {
                var worksheet = libro.Workbook.Worksheets.Add("Productos");




                DataTable dataTable = new DataTable();

                dataTable.Columns.Add("Nombre_Producto", typeof(string));
                dataTable.Columns.Add("Descripcion", typeof(string));
                dataTable.Columns.Add("SKU", typeof(string));
                dataTable.Columns.Add("Type", typeof(string));
                dataTable.Columns.Add("Precio", typeof(string));
                dataTable.Columns.Add("Income", typeof(string));
                dataTable.Columns.Add("Expense", typeof(string));




                foreach (var item in productos)
                {
                    string prov = item.Proveedor.Nombre;

                    if (item.TipoProductoId == 1)
                    {
                        DataRow fila = dataTable.NewRow();
                        fila["Nombre_Producto"] = "Vehicle Rental:"+prov+":"+item.Nombre;
                        fila["Descripcion"] = item.Descripcion;
                        fila["SKU"] = item.SKU;
                        fila["Type"] = "Service";
                        fila["Precio"] = "0.00";
                        fila["Income"] = "Vehicle Rental";
                        fila["Expense"] = "Vehicle Rental Booking";
                        dataTable.Rows.Add(fila);
                    }

                    if (item.TipoProductoId == 2)
                    {
                        DataRow fila = dataTable.NewRow();
                        fila["Nombre_Producto"] = "Activity:" + prov + ":" + item.Nombre; 
                        fila["Descripcion"] = item.Descripcion;
                        fila["SKU"] = item.SKU;
                        fila["Type"] = "Service";
                        fila["Precio"] = "0.00";
                        fila["Income"] = "Activity";
                        fila["Expense"] = "Activity Booking";
                        dataTable.Rows.Add(fila);
                    }

                    if (item.TipoProductoId == 3)
                    {
                        DataRow fila = dataTable.NewRow();
                        fila["Nombre_Producto"] = "Ground Transportation:" + prov + ":" + item.Nombre;
                        fila["Descripcion"] = item.Descripcion;
                        fila["SKU"] = item.SKU;
                        fila["Type"] = "Service";
                        fila["Precio"] = "0.00";
                        fila["Income"] = "Ground Transportation";
                        fila["Expense"] = "Ground Transportation Booking";
                        dataTable.Rows.Add(fila);
                    }

                    if (item.TipoProductoId == 4)
                    {
                        DataRow fila = dataTable.NewRow();
                        fila["Nombre_Producto"] = item.Nombre;
                        fila["Descripcion"] = item.Descripcion;
                        fila["SKU"] = item.SKU;
                        fila["Type"] = "Service";
                        fila["Precio"] = "0.00";
                        fila["Income"] = "Service";
                        fila["Expense"] = "0.00";
                        dataTable.Rows.Add(fila);
                    }

                    if (item.TipoProductoId == 5)
                    {
                        var habitaciones = _context.Habitaciones.Where(x => x.ProductoId == item.ProductoId);

                        foreach (var hab in habitaciones)
                        {
                            DataRow fila = dataTable.NewRow();
                            fila["Nombre_Producto"] = "Accommodation:" + prov + ":" + item.Nombre+":"+ hab.Nombre;
                            fila["Descripcion"] = hab.Descripcion;
                            fila["SKU"] = hab.SKU;
                            fila["Type"] = "Service";
                            fila["Precio"] = "0.00";
                            fila["Income"] = "Accommodation";
                            fila["Expense"] = "Accommodation Booking";
                            dataTable.Rows.Add(fila);
                        }


                    }



                }

                worksheet.Cells["A1"].LoadFromDataTable(dataTable, true);
                for (var col = 1; col < productos.Count + 1; col++)
                {
                    worksheet.Column(col).AutoFit();
                }

                // Agregar formato de tabla
                var tabla = worksheet.Tables.Add(new ExcelAddressBase(fromRow: 1, fromCol: 1, toRow: productos.Count + 1, toColumn: 7), "Productos");
                tabla.ShowHeader = true;
                tabla.TableStyle = TableStyles.Light6;
                tabla.ShowTotal = true;


                return File(libro.GetAsByteArray(), excelContentType, "Productos.xlsx");
            }

        }



        [HttpGet]
        [Route("existeQb")]
        public async System.Threading.Tasks.Task<ActionResult> ExisteEnQB(string sku)
        {
            var realmId = "";
            var access_token = "";
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
                    return Ok(new { token = "Error Cargando el Token" });
                }



                access_token = dictionary["accessToken"];
                realmId = dictionary["realmId"];
            }
            catch (Exception ex)
            {


                return Ok(new { token = "Error connectandose a QB" });
            }



            OAuth2RequestValidator oauthValidator = new OAuth2RequestValidator(access_token);
            // Create a ServiceContext with Auth tokens and realmId
            ServiceContext serviceContext = new ServiceContext(realmId, IntuitServicesType.QBO, oauthValidator);
            serviceContext.IppConfiguration.MinorVersion.Qbo = "23";
            serviceContext.IppConfiguration.BaseUrl.Qbo = QboBaseUrl;

            // Create a QuickBooks QueryService using ServiceContext
            QueryService<Item> querySvc = new QueryService<Item>(serviceContext);

            string EXISTING_ITEM_QUERYBYNAMEITEMPROD = string.Format("select * from Item where Sku = '{0}' ", sku);
            Item itemProduct = querySvc.ExecuteIdsQuery(EXISTING_ITEM_QUERYBYNAMEITEMPROD).FirstOrDefault<Item>();
            if (itemProduct == null)
            {
                return Ok(new { idQb = 0 });
            }
            else
            {
                return Ok(new { idQb = itemProduct.Id });
            }





        }


        [HttpGet]
        [Route("existeClienteQb")]
        public async System.Threading.Tasks.Task<ActionResult> ExisteClienteEnQB(string sku)
        {
            var realmId = "";
            var access_token = "";
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
                    return Ok(new { token = "Error Cargando el Token" });
                }



                access_token = dictionary["accessToken"];
                realmId = dictionary["realmId"];
            }
            catch (Exception ex)
            {


                return Ok(new { token = "Error connectandose a QB" });
            }



            OAuth2RequestValidator oauthValidator = new OAuth2RequestValidator(access_token);
            // Create a ServiceContext with Auth tokens and realmId
            ServiceContext serviceContext = new ServiceContext(realmId, IntuitServicesType.QBO, oauthValidator);
            serviceContext.IppConfiguration.MinorVersion.Qbo = "23";
            serviceContext.IppConfiguration.BaseUrl.Qbo = QboBaseUrl;

            // Create a QuickBooks QueryService using ServiceContext
            QueryService<Customer> querySvc = new QueryService<Customer>(serviceContext);

            string EXISTING_ITEM_QUERYBYNAMEITEMPROD = string.Format("select * from Customer where DisplayName = '{0}' ", sku);
            Customer cust = querySvc.ExecuteIdsQuery(EXISTING_ITEM_QUERYBYNAMEITEMPROD).FirstOrDefault<Customer>();
            if (cust == null)
            {
                return Ok(new { idQb = 0 });
            }
            else
            {
                return Ok(new { idQb = cust.Id });
            }





        }

    }
}
