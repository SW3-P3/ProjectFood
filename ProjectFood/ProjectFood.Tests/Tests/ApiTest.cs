using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using ProjectFood.Models.Api;
using RestSharp;

namespace ProjectFood.Tests.Tests
{
    [TestFixture]
    public class ApiTest
    {
        [TestFixtureSetUp]
        public void Init()
        {
            try
            {
                Apikey = File.ReadAllText(@"C:\apikey.secret");
                Secret = File.ReadAllText(@"C:\secret.secret");
            }
            catch (IOException ex)
            {
                Console.WriteLine("File not found. " + ex);
            }

            var client = new RestClient("https://api.etilbudsavis.dk");

            var sessionRequest = new RestRequest("v2/sessions", Method.POST);
            sessionRequest.AddParameter("api_key", Apikey);
            SessionObjData = client.Execute<Session>(sessionRequest).Data;
            SessionObjData.Signature = Session.Sha256(Secret + SessionObjData.Token);

            const string latitude = "57.021503";
            const string longitude = "9.952489";
            const string radius = "10000";

            var storeRequest = new RestRequest("v2/stores", Method.GET);
            storeRequest.AddParameter("r_lat", latitude);
            storeRequest.AddParameter("r_lng", longitude);
            storeRequest.AddParameter("r_radius", radius);
            storeRequest.AddHeader("X-Token", SessionObjData.Token);
            storeRequest.AddHeader("X-Signature", SessionObjData.Signature);

            StoresList = client.Execute<List<Models.Api.Store>>(storeRequest).Data;

            var offersRequest = new RestRequest("v2/offers", Method.GET);
            offersRequest.AddParameter("r_lat", latitude);
            offersRequest.AddParameter("r_lng", longitude);
            offersRequest.AddParameter("r_radius", radius);
            offersRequest.AddHeader("X-Token", SessionObjData.Token);
            offersRequest.AddHeader("X-Signature", SessionObjData.Signature);
            offersRequest.AddParameter("limit", "100");

            ApiOfferList = client.Execute<List<ApiOffer>>(offersRequest).Data;


        }

        [Test]
        public void LoadApikeyTest()
        {
            Assert.AreEqual("00i0nm90z49", Apikey.Substring(0, 11));
        }

        [Test]
        public void LoadSecretTest()
        {
            Assert.AreEqual("00i0nm90z47", Secret.Substring(0, 11));
        }

        [Test]
        public void ApiGetTokenTest()
        {
            Assert.IsTrue(SessionObjData.Token.Length.Equals(16));
        }

        [Test]
        public void ApiExpiresAfterNow()
        {
            Assert.IsTrue(DateTime.Parse(SessionObjData.Expires).CompareTo(DateTime.Now) > 0);
        }

        [Test]
        public void ApiAnyStoreExist()
        {
            Assert.IsTrue(StoresList.Count > 0);
        }

        [Test]
        public void ApiAnyOffersExist()
        {
            Assert.IsTrue(ApiOfferList.Count > 0);
        }

        public static string Apikey { get; set; }
        public static string Secret { get; set; }
        public static Session SessionObjData { get; set; }
        public static List<Models.Api.Store> StoresList { get; set; }
        public static List<ApiOffer> ApiOfferList { get; set; }


    }
}
