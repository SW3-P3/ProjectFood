using System.Linq;
using System.Security.Principal;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using ProjectFood.Controllers;
using ProjectFood.Models;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ProjectFood.Tests.Tests
{
    [TestFixture]
    public class TestOfferController
    {
        #region setup

        [SetUp]
        public void initialize()
        {
            _mockdata = new TestProjectFoodContext();
            _controller = new OfferController(_mockdata);
            _user = DemoGetMethods.GetDemoUser(1);
            _mockdata.Users.Add(_user);
            var controllerContext = new Mock<ControllerContext>();
            _principal = new Moq.Mock<IPrincipal>();
            _principal.Setup(x => x.Identity.IsAuthenticated).Returns(true);
            _principal.SetupGet(x => x.Identity.Name).Returns(_user.Name);
            controllerContext.SetupGet(x => x.HttpContext.User).Returns(_principal.Object);
            _controller.ControllerContext = controllerContext.Object;

            _mockdata.Offers.Add(DemoGetMethods.GetDemoOffer("Leverpostej", 1, 10.00M));
            _mockdata.Offers.Add(DemoGetMethods.GetDemoOffer("Bacon", 2, 11.00M));
            _mockdata.Offers.Add(DemoGetMethods.GetDemoOffer("Ost", 3, 13.00M));

            var list =  _mockdata.ShoppingLists.Add(DemoGetMethods.GetDemoShoppingListWithItem(3, 1));
            _mockdata.Users.First().ShoppingLists.Add(list);

            _mockdata.Items.Add(DemoGetMethods.GetDemoItem(4, "Ost"));

        }

        #endregion

        private User _user = new User();
        private Moq.Mock<IPrincipal> _principal = new Moq.Mock<IPrincipal>();
        private OfferController _controller = new OfferController();
        private TestProjectFoodContext _mockdata = new TestProjectFoodContext();



        [Test]
        public void IndexView_UserLoggedIn_ShouldReturnViewResult()
        {
            var result = _controller.Index(_user.ShoppingLists.First().ID);

            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestCase(1, 1)]
        [TestCase(2, 1)]
        [TestCase(3, 1)]
        public void AddOfferToShoppingList_Offers_ShouldAddOffer(int offerId, int shoppingListId)
        {
            Assert.AreEqual(_user.ShoppingLists.First().Items.Count(), 3);

            _controller.AddOfferToShoppingList(offerId, shoppingListId);

            Assert.AreEqual(_user.ShoppingLists.First().Items.Count(), 4);
        }

        [TestCase("Ost")]
        [TestCase("Bacon")]
        [TestCase("Leverpostej")]
        public void GetOfferForItem_DifferentStrings_ShouldFindOffers(string id)
        {
            var result = _controller.GetOffersForItem(id);
            
            Assert.AreEqual(result.Count(), 1 );
        }

        [Test]
        public void GetofferForItem_AnItem_ShouldFind()
        {
            var result = _controller.GetOffersForItem(_mockdata.Items.First(x=> x.Name == "Ost"));

            Assert.AreEqual(result.Count(), 1);

        }

    }
}
