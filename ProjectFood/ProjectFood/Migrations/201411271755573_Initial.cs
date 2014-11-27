namespace ProjectFood.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ShoppingLists", "User_ID", "dbo.Users");
            DropIndex("dbo.ShoppingLists", new[] { "User_ID" });
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
            
            DropColumn("dbo.ShoppingLists", "User_ID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ShoppingLists", "User_ID", c => c.Int());
            DropForeignKey("dbo.UserShoppingLists", "ShoppingListId", "dbo.ShoppingLists");
            DropForeignKey("dbo.UserShoppingLists", "UserId", "dbo.Users");
            DropIndex("dbo.UserShoppingLists", new[] { "ShoppingListId" });
            DropIndex("dbo.UserShoppingLists", new[] { "UserId" });
            DropTable("dbo.UserShoppingLists");
            CreateIndex("dbo.ShoppingLists", "User_ID");
            AddForeignKey("dbo.ShoppingLists", "User_ID", "dbo.Users", "ID");
        }
    }
}
