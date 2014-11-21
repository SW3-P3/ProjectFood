namespace ProjectFood.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial3 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Ratings",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Score = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Recipe_ID = c.Int(),
                        User_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Recipes", t => t.Recipe_ID)
                .ForeignKey("dbo.Users", t => t.User_ID)
                .Index(t => t.Recipe_ID)
                .Index(t => t.User_ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Ratings", "User_ID", "dbo.Users");
            DropForeignKey("dbo.Ratings", "Recipe_ID", "dbo.Recipes");
            DropIndex("dbo.Ratings", new[] { "User_ID" });
            DropIndex("dbo.Ratings", new[] { "Recipe_ID" });
            DropTable("dbo.Ratings");
        }
    }
}
