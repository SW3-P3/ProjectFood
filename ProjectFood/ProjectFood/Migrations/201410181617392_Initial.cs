namespace ProjectFood.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ShoppingLists",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Items",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                        Category = c.String(),
                        ShoppingList_ID = c.Int(),
                        ShoppingList_ID1 = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.ShoppingLists", t => t.ShoppingList_ID)
                .ForeignKey("dbo.ShoppingLists", t => t.ShoppingList_ID1)
                .Index(t => t.ShoppingList_ID)
                .Index(t => t.ShoppingList_ID1);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Items", "ShoppingList_ID1", "dbo.ShoppingLists");
            DropForeignKey("dbo.Items", "ShoppingList_ID", "dbo.ShoppingLists");
            DropIndex("dbo.Items", new[] { "ShoppingList_ID1" });
            DropIndex("dbo.Items", new[] { "ShoppingList_ID" });
            DropTable("dbo.Items");
            DropTable("dbo.ShoppingLists");
        }
    }
}
