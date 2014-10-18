using System.Collections.Generic;
using Microsoft.Owin;
using Owin;
using ProjectFood.Models;

[assembly: OwinStartupAttribute(typeof(ProjectFood.Startup))]
namespace ProjectFood
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            /* Add a single item to the shoppingList this is temp */
            //Item item = new Item("Torsk");
            //Global.GlobalShoppingList.Items.Add(item);
            /* END */

            ConfigureAuth(app);
        }
    }
}
