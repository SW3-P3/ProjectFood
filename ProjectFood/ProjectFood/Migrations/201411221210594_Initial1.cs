namespace ProjectFood.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "WatchList_ID", c => c.Int());
            CreateIndex("dbo.Users", "WatchList_ID");
            AddForeignKey("dbo.Users", "WatchList_ID", "dbo.ShoppingLists", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Users", "WatchList_ID", "dbo.ShoppingLists");
            DropIndex("dbo.Users", new[] { "WatchList_ID" });
            DropColumn("dbo.Users", "WatchList_ID");
        }
    }
}
