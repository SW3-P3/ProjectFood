using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using ProjectFood.Models.Api;
using RestSharp;
using RestSharp.Deserializers;

namespace ProjectFood.Controllers
{
    public class OfferController : Controller
    {
        // GET: Offer
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ImportOffers()
        {
            string apikey, secret;
            /* Write Apikey and Secret to global variables */
            try
            {
                apikey = System.IO.File.ReadAllText(@"C:\apikey.secret");
                secret = System.IO.File.ReadAllText(@"C:\secret.secret");
            }
            catch (IOException ex)
            {
                Console.WriteLine("File not found. " + ex);
                throw;
            }

            /* Create a RestClient*/
            var client = new RestClient("https://api.etilbudsavis.dk");

            /* Initiate the first request to get a session */
            var SessionRequest = new RestRequest("v2/sessions", Method.POST);
            SessionRequest.AddParameter("api_key", apikey);

            /* Map the response to the class "Session" */
            IRestResponse<Session> response2 = client.Execute<Session>(SessionRequest);

            /* Save the response in an object */
            var sessionobj = response2.Data;

            /* Load aditional infomation into the object */
            sessionobj.Secret = secret;
            sessionobj.Apikey = apikey;
            sessionobj.Signature = Models.Api.Session.Sha256(sessionobj.Secret + sessionobj.Token);
            
            /* Copy the sessionobj to a globalSession */
            Global.Session = sessionobj;

            /* Catalog request */
            var catalogsRequest = new RestRequest("v2/catalogs", Method.GET);
            catalogsRequest.AddParameter("r_lat", Global.Latitude);
            catalogsRequest.AddParameter("r_lng", Global.Longitude);
            catalogsRequest.AddParameter("r_radius", Global.Radius);
            catalogsRequest.AddHeader("X-Token", Global.Session.Token);
            catalogsRequest.AddHeader("X-Signature", Global.Session.Signature);

            var catalogsResult = client.Execute<List<Catalog>>(catalogsRequest).Data;
            Console.WriteLine("\n" + catalogsResult.First().images.view);

            /* Offers request (Every offer from the catalog found above */
            var offersRequest = new RestRequest("v2/offers", Method.GET);
            offersRequest.AddParameter("r_lat", Global.Latitude);
            offersRequest.AddParameter("r_lng", Global.Longitude);
            offersRequest.AddParameter("r_radius", Global.Radius);
            offersRequest.AddHeader("X-Token", Global.Session.Token);
            offersRequest.AddHeader("X-Signature", Global.Session.Signature);
            offersRequest.AddParameter("catalog_ids", string.Join(",", catalogsResult.Select(x => x.id)));
            offersRequest.AddParameter("limit", "50");

            return RedirectToAction("Index");
        }
    }
}