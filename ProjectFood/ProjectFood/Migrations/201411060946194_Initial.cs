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
                "dbo.ShoppingList_Item",
                c => new
                    {
                        ShoppingListID = c.Int(nullable: false),
                        ItemID = c.Int(nullable: false),
                        Amount = c.Double(nullable: false),
                        Unit = c.String(),
                    })
                .PrimaryKey(t => new { t.ShoppingListID, t.ItemID })
                .ForeignKey("dbo.Items", t => t.ItemID, cascadeDelete: true)
                .ForeignKey("dbo.ShoppingLists", t => t.ShoppingListID, cascadeDelete: true)
                .Index(t => t.ShoppingListID)
                .Index(t => t.ItemID);
            
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
                "dbo.Items_Offers",
                c => new
                    {
                        ItemID = c.Int(nullable: false),
                        OfferID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ItemID, t.OfferID })
                .ForeignKey("dbo.Items", t => t.ItemID, cascadeDelete: true)
                .ForeignKey("dbo.Offers", t => t.OfferID, cascadeDelete: true)
                .Index(t => t.ItemID)
                .Index(t => t.OfferID);
            
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
            DropForeignKey("dbo.ShoppingList_Item", "ItemID", "dbo.Items");
            DropForeignKey("dbo.ShoppingListItems", "Item_ID", "dbo.Items");
            DropForeignKey("dbo.ShoppingListItems", "ShoppingList_ID", "dbo.ShoppingLists");
            DropForeignKey("dbo.Items_Offers", "OfferID", "dbo.Offers");
            DropForeignKey("dbo.Items_Offers", "ItemID", "dbo.Items");
            DropIndex("dbo.GroupUsers", new[] { "User_ID" });
            DropIndex("dbo.GroupUsers", new[] { "Group_ID" });
            DropIndex("dbo.ShoppingListItems", new[] { "Item_ID" });
            DropIndex("dbo.ShoppingListItems", new[] { "ShoppingList_ID" });
            DropIndex("dbo.Items_Offers", new[] { "OfferID" });
            DropIndex("dbo.Items_Offers", new[] { "ItemID" });
            DropIndex("dbo.ShoppingList_Item", new[] { "ItemID" });
            DropIndex("dbo.ShoppingList_Item", new[] { "ShoppingListID" });
            DropIndex("dbo.ShoppingLists", new[] { "User_ID" });
            DropTable("dbo.GroupUsers");
            DropTable("dbo.ShoppingListItems");
            DropTable("dbo.Items_Offers");
            DropTable("dbo.Groups");
            DropTable("dbo.Users");
            DropTable("dbo.ShoppingList_Item");
            DropTable("dbo.ShoppingLists");
            DropTable("dbo.Offers");
            DropTable("dbo.Items");
        }
    }
}
