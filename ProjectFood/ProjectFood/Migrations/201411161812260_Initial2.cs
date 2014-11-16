namespace ProjectFood.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Recipe_Ingredient", "AmountPerPerson", c => c.Double(nullable: false));
            DropColumn("dbo.Recipe_Ingredient", "Amount");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Recipe_Ingredient", "Amount", c => c.Double(nullable: false));
            DropColumn("dbo.Recipe_Ingredient", "AmountPerPerson");
        }
    }
}
