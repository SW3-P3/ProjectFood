namespace ProjectFood.Migrations
{
    using ProjectFood.Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<ProjectFood.Models.ShoppingListDBContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ProjectFood.Models.ShoppingListDBContext context)
        {
            context.ShoppingLists.AddOrUpdate(i => i.Title,
                new ShoppingList
                {
                    Title = "Aftensmad",
                    Items = new List<Item>(),
                },
                 new ShoppingList
                {
                    Title = "Morgenmad",
                    Items = new List<Item>(),
                }
                );
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
}
