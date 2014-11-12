namespace ProjectFood.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Recipes", "AuthorID", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Recipes", "AuthorID");
        }
    }
}
