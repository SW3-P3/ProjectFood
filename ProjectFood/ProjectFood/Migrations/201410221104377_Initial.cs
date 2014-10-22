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
                "dbo.ShoppingList_Items",
                c => new
                    {
                        ShoppingListID = c.Int(nullable: false),
                        ItemID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ShoppingListID, t.ItemID })
                .ForeignKey("dbo.ShoppingLists", t => t.ShoppingListID, cascadeDelete: true)
                .ForeignKey("dbo.Items", t => t.ItemID, cascadeDelete: true)
                .Index(t => t.ShoppingListID)
                .Index(t => t.ItemID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ShoppingList_Items", "ItemID", "dbo.Items");
            DropForeignKey("dbo.ShoppingList_Items", "ShoppingListID", "dbo.ShoppingLists");
            DropForeignKey("dbo.Items_Offers", "OfferID", "dbo.Offers");
            DropForeignKey("dbo.Items_Offers", "ItemID", "dbo.Items");
            DropIndex("dbo.ShoppingList_Items", new[] { "ItemID" });
            DropIndex("dbo.ShoppingList_Items", new[] { "ShoppingListID" });
            DropIndex("dbo.Items_Offers", new[] { "OfferID" });
            DropIndex("dbo.Items_Offers", new[] { "ItemID" });
            DropTable("dbo.ShoppingList_Items");
            DropTable("dbo.Items_Offers");
            DropTable("dbo.ShoppingLists");
            DropTable("dbo.Offers");
            DropTable("dbo.Items");
        }
    }
}
