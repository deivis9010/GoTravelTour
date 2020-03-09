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
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace GoTravelTour.QuickBooks
{
    [Route("api/[controller]")]
    [ApiController]
    public class QBIntegracionController : ControllerBase
    {
       
        public static string clientid = "ABtbGg86yOB32TNPcsZSaDXVSm2wBlgV89AGXiNGMJ2ja8yVCR";
        public static string clientsecret = "iOFqEfvrOsmP7lCMmyCwlAHdHaHUWg4n1PNc6sXr";
        public static string redirectUrl = "https://developer.intuit.com/v2/OAuth2Playground/RedirectUrl";
        public static string environment ="";

        public static OAuth2Client auth2Client = new OAuth2Client(clientid, clientsecret, redirectUrl, environment);


        public ActionResult InitiateAuth(string submitButton)
        {
            List<OidcScopes> scopes = new List<OidcScopes>();
            scopes.Add(OidcScopes.Accounting);
            string authorizeUrl = auth2Client.GetAuthorizationURL(scopes);
            return Redirect(authorizeUrl);
        }

        public ActionResult ApiCallService()
        {
            string realmId = "GOTravel";//Session["realmId"].ToString();
            var principal = User as ClaimsPrincipal;
            OAuth2RequestValidator oauthValidator = new OAuth2RequestValidator(principal.FindFirst("access_token").Value);
            // Create a ServiceContext with Auth tokens and realmId
            ServiceContext serviceContext = new ServiceContext(realmId, IntuitServicesType.QBO, oauthValidator);
            serviceContext.IppConfiguration.MinorVersion.Qbo = "23";

            // Create a QuickBooks QueryService using ServiceContext
            QueryService<CompanyInfo> querySvc = new QueryService<CompanyInfo>(serviceContext);
            CompanyInfo companyInfo = querySvc.ExecuteIdsQuery("SELECT * FROM CompanyInfo").FirstOrDefault();

            string output = JsonConvert.SerializeObject(companyInfo, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            return Ok( "ApiCallService" + " QBO API call Successful!! Response: " );
        }

       /* public async Task<ActionResult> IndexCallback()
        {
            string code = Request.QueryString["code"] ?? "none";
            string realmId = Request.QueryString["realmId"] ?? "none";
            await GetAuthTokensAsync(code, realmId);
        }
        */

        // GET: api/QBIntegracion
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/QBIntegracion/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/QBIntegracion
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/QBIntegracion/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}