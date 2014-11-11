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
                        Category = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Recipes",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Tags = c.String(),
                        Minutes = c.Int(nullable: false),
                        Instructions = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Offers",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Heading = c.String(),
                        Store_ID = c.String(),
                        Store = c.String(),
                        Begin = c.DateTime(nullable: false),
                        End = c.DateTime(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Unit = c.String(),
                        Image_URL = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.ShoppingLists",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        User_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Users", t => t.User_ID)
                .Index(t => t.User_ID);
            
            CreateTable(
                "dbo.Recipe_Ingredient",
                c => new
                    {
                        RecipeID = c.Int(nullable: false),
                        IngredientID = c.Int(nullable: false),
                        Amount = c.Double(nullable: false),
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
                "dbo.Users",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Username = c.String(),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Groups",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.RecipeItems",
                c => new
                    {
                        Recipe_ID = c.Int(nullable: false),
                        Item_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Recipe_ID, t.Item_ID })
                .ForeignKey("dbo.Recipes", t => t.Recipe_ID, cascadeDelete: true)
                .ForeignKey("dbo.Items", t => t.Item_ID, cascadeDelete: true)
                .Index(t => t.Recipe_ID)
                .Index(t => t.Item_ID);
            
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
                "dbo.ShoppingListItems",
                c => new
                    {
                        ShoppingList_ID = c.Int(nullable: false),
                        Item_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ShoppingList_ID, t.Item_ID })
                .ForeignKey("dbo.ShoppingLists", t => t.ShoppingList_ID, cascadeDelete: true)
                .ForeignKey("dbo.Items", t => t.Item_ID, cascadeDelete: true)
                .Index(t => t.ShoppingList_ID)
                .Index(t => t.Item_ID);
            
            CreateTable(
                "dbo.GroupUsers",
                c => new
                    {
                        Group_ID = c.Int(nullable: false),
                        User_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Group_ID, t.User_ID })
                .ForeignKey("dbo.Groups", t => t.Group_ID, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.User_ID, cascadeDelete: true)
                .Index(t => t.Group_ID)
                .Index(t => t.User_ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ShoppingLists", "User_ID", "dbo.Users");
            DropForeignKey("dbo.GroupUsers", "User_ID", "dbo.Users");
            DropForeignKey("dbo.GroupUsers", "Group_ID", "dbo.Groups");
            DropForeignKey("dbo.ShoppingList_Item", "ShoppingListID", "dbo.ShoppingLists");
            DropForeignKey("dbo.ShoppingList_Item", "selectedOffer_ID", "dbo.Offers");
            DropForeignKey("dbo.ShoppingList_Item", "ItemID", "dbo.Items");
            DropForeignKey("dbo.Recipe_Ingredient", "RecipeID", "dbo.Recipes");
            DropForeignKey("dbo.Recipe_Ingredient", "IngredientID", "dbo.Items");
            DropForeignKey("dbo.ShoppingListItems", "Item_ID", "dbo.Items");
            DropForeignKey("dbo.ShoppingListItems", "ShoppingList_ID", "dbo.ShoppingLists");
            DropForeignKey("dbo.OfferItems", "Item_ID", "dbo.Items");
            DropForeignKey("dbo.OfferItems", "Offer_ID", "dbo.Offers");
            DropForeignKey("dbo.RecipeItems", "Item_ID", "dbo.Items");
            DropForeignKey("dbo.RecipeItems", "Recipe_ID", "dbo.Recipes");
            DropIndex("dbo.GroupUsers", new[] { "User_ID" });
            DropIndex("dbo.GroupUsers", new[] { "Group_ID" });
            DropIndex("dbo.ShoppingListItems", new[] { "Item_ID" });
            DropIndex("dbo.ShoppingListItems", new[] { "ShoppingList_ID" });
            DropIndex("dbo.OfferItems", new[] { "Item_ID" });
            DropIndex("dbo.OfferItems", new[] { "Offer_ID" });
            DropIndex("dbo.RecipeItems", new[] { "Item_ID" });
            DropIndex("dbo.RecipeItems", new[] { "Recipe_ID" });
            DropIndex("dbo.ShoppingList_Item", new[] { "selectedOffer_ID" });
            DropIndex("dbo.ShoppingList_Item", new[] { "ItemID" });
            DropIndex("dbo.ShoppingList_Item", new[] { "ShoppingListID" });
            DropIndex("dbo.Recipe_Ingredient", new[] { "IngredientID" });
            DropIndex("dbo.Recipe_Ingredient", new[] { "RecipeID" });
            DropIndex("dbo.ShoppingLists", new[] { "User_ID" });
            DropTable("dbo.GroupUsers");
            DropTable("dbo.ShoppingListItems");
            DropTable("dbo.OfferItems");
            DropTable("dbo.RecipeItems");
            DropTable("dbo.Groups");
            DropTable("dbo.Users");
            DropTable("dbo.ShoppingList_Item");
            DropTable("dbo.Recipe_Ingredient");
            DropTable("dbo.ShoppingLists");
            DropTable("dbo.Offers");
            DropTable("dbo.Recipes");
            DropTable("dbo.Items");
        }
    }
}
