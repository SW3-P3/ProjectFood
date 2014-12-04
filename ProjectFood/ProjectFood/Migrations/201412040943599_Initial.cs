namespace ProjectFood.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Items",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Recipe_ID = c.Int(),
                        ShoppingList_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Recipes", t => t.Recipe_ID)
                .ForeignKey("dbo.ShoppingLists", t => t.ShoppingList_ID)
                .Index(t => t.Recipe_ID)
                .Index(t => t.ShoppingList_ID);
            
            CreateTable(
                "dbo.Offers",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Heading = c.String(),
                        Store = c.String(),
                        Begin = c.DateTime(nullable: false),
                        End = c.DateTime(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Unit = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Username = c.String(),
                        Name = c.String(),
                        LastSentNotification = c.DateTime(),
                        MaxSendEmailsEveryDays = c.Int(),
                        RelevantOffers_ID = c.Int(),
                        WatchList_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.ShoppingLists", t => t.RelevantOffers_ID)
                .ForeignKey("dbo.ShoppingLists", t => t.WatchList_ID)
                .Index(t => t.RelevantOffers_ID)
                .Index(t => t.WatchList_ID);
            
            CreateTable(
                "dbo.Prefs",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Value = c.String(),
                        Store = c.Boolean(nullable: false),
                        User_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Users", t => t.User_ID)
                .Index(t => t.User_ID);
            
            CreateTable(
                "dbo.Ratings",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Score = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Recipe_ID = c.Int(),
                        User_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Recipes", t => t.Recipe_ID)
                .ForeignKey("dbo.Users", t => t.User_ID)
                .Index(t => t.Recipe_ID)
                .Index(t => t.User_ID);
            
            CreateTable(
                "dbo.Recipes",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        OriginalAuthorName = c.String(),
                        AuthorName = c.String(),
                        Title = c.String(),
                        Minutes = c.Int(nullable: false),
                        Instructions = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.ShoppingLists",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Recipe_Ingredient",
                c => new
                    {
                        RecipeID = c.Int(nullable: false),
                        IngredientID = c.Int(nullable: false),
                        AmountPerPerson = c.Double(nullable: false),
                        Unit = c.String(),
                    })
                .PrimaryKey(t => new { t.RecipeID, t.IngredientID })
                .ForeignKey("dbo.Items", t => t.IngredientID, cascadeDelete: true)
                .ForeignKey("dbo.Recipes", t => t.RecipeID, cascadeDelete: true)
                .Index(t => t.RecipeID)
                .Index(t => t.IngredientID);
            
            CreateTable(
                "dbo.ShoppingList_Item",
                c => new
                    {
                        ShoppingListID = c.Int(nullable: false),
                        ItemID = c.Int(nullable: false),
                        Amount = c.Double(nullable: false),
                        Unit = c.String(),
                        Bought = c.Boolean(nullable: false),
                        selectedOffer_ID = c.Int(),
                    })
                .PrimaryKey(t => new { t.ShoppingListID, t.ItemID })
                .ForeignKey("dbo.Items", t => t.ItemID, cascadeDelete: true)
                .ForeignKey("dbo.Offers", t => t.selectedOffer_ID)
                .ForeignKey("dbo.ShoppingLists", t => t.ShoppingListID, cascadeDelete: true)
                .Index(t => t.ShoppingListID)
                .Index(t => t.ItemID)
                .Index(t => t.selectedOffer_ID);
            
            CreateTable(
                "dbo.OfferItems",
                c => new
                    {
                        Offer_ID = c.Int(nullable: false),
                        Item_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Offer_ID, t.Item_ID })
                .ForeignKey("dbo.Offers", t => t.Offer_ID, cascadeDelete: true)
                .ForeignKey("dbo.Items", t => t.Item_ID, cascadeDelete: true)
                .Index(t => t.Offer_ID)
                .Index(t => t.Item_ID);
            
            CreateTable(
                "dbo.OfferSentTo",
                c => new
                    {
                        UserId = c.Int(nullable: false),
                        OfferId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserId, t.OfferId })
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.Offers", t => t.OfferId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.OfferId);
            
            CreateTable(
                "dbo.UserShoppingLists",
                c => new
                    {
                        UserId = c.Int(nullable: false),
                        ShoppingListId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserId, t.ShoppingListId })
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.ShoppingLists", t => t.ShoppingListId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.ShoppingListId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ShoppingList_Item", "ShoppingListID", "dbo.ShoppingLists");
            DropForeignKey("dbo.ShoppingList_Item", "selectedOffer_ID", "dbo.Offers");
            DropForeignKey("dbo.ShoppingList_Item", "ItemID", "dbo.Items");
            DropForeignKey("dbo.Recipe_Ingredient", "RecipeID", "dbo.Recipes");
            DropForeignKey("dbo.Recipe_Ingredient", "IngredientID", "dbo.Items");
            DropForeignKey("dbo.Users", "WatchList_ID", "dbo.ShoppingLists");
            DropForeignKey("dbo.UserShoppingLists", "ShoppingListId", "dbo.ShoppingLists");
            DropForeignKey("dbo.UserShoppingLists", "UserId", "dbo.Users");
            DropForeignKey("dbo.OfferSentTo", "OfferId", "dbo.Offers");
            DropForeignKey("dbo.OfferSentTo", "UserId", "dbo.Users");
            DropForeignKey("dbo.Users", "RelevantOffers_ID", "dbo.ShoppingLists");
            DropForeignKey("dbo.Items", "ShoppingList_ID", "dbo.ShoppingLists");
            DropForeignKey("dbo.Ratings", "User_ID", "dbo.Users");
            DropForeignKey("dbo.Ratings", "Recipe_ID", "dbo.Recipes");
            DropForeignKey("dbo.Items", "Recipe_ID", "dbo.Recipes");
            DropForeignKey("dbo.Prefs", "User_ID", "dbo.Users");
            DropForeignKey("dbo.OfferItems", "Item_ID", "dbo.Items");
            DropForeignKey("dbo.OfferItems", "Offer_ID", "dbo.Offers");
            DropIndex("dbo.UserShoppingLists", new[] { "ShoppingListId" });
            DropIndex("dbo.UserShoppingLists", new[] { "UserId" });
            DropIndex("dbo.OfferSentTo", new[] { "OfferId" });
            DropIndex("dbo.OfferSentTo", new[] { "UserId" });
            DropIndex("dbo.OfferItems", new[] { "Item_ID" });
            DropIndex("dbo.OfferItems", new[] { "Offer_ID" });
            DropIndex("dbo.ShoppingList_Item", new[] { "selectedOffer_ID" });
            DropIndex("dbo.ShoppingList_Item", new[] { "ItemID" });
            DropIndex("dbo.ShoppingList_Item", new[] { "ShoppingListID" });
            DropIndex("dbo.Recipe_Ingredient", new[] { "IngredientID" });
            DropIndex("dbo.Recipe_Ingredient", new[] { "RecipeID" });
            DropIndex("dbo.Ratings", new[] { "User_ID" });
            DropIndex("dbo.Ratings", new[] { "Recipe_ID" });
            DropIndex("dbo.Prefs", new[] { "User_ID" });
            DropIndex("dbo.Users", new[] { "WatchList_ID" });
            DropIndex("dbo.Users", new[] { "RelevantOffers_ID" });
            DropIndex("dbo.Items", new[] { "ShoppingList_ID" });
            DropIndex("dbo.Items", new[] { "Recipe_ID" });
            DropTable("dbo.UserShoppingLists");
            DropTable("dbo.OfferSentTo");
            DropTable("dbo.OfferItems");
            DropTable("dbo.ShoppingList_Item");
            DropTable("dbo.Recipe_Ingredient");
            DropTable("dbo.ShoppingLists");
            DropTable("dbo.Recipes");
            DropTable("dbo.Ratings");
            DropTable("dbo.Prefs");
            DropTable("dbo.Users");
            DropTable("dbo.Offers");
            DropTable("dbo.Items");
        }
    }
}
