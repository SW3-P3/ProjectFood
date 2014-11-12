namespace ProjectFood.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Recipes", "AuthorName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Recipes", "AuthorName");
        }
    }
}
