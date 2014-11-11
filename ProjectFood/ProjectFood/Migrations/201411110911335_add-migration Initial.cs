namespace ProjectFood.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addmigrationInitial : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Recipes", "Tags", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Recipes", "Tags");
        }
    }
}
