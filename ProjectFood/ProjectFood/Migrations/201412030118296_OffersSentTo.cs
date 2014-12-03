namespace ProjectFood.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OffersSentTo : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Offers", "User_ID", "dbo.Users");
            DropIndex("dbo.Offers", new[] { "User_ID" });
            CreateTable(
                "dbo.OfferSentTo",
                c => new
                    {
                        UserId = c.Int(nullable: false),
                        OfferId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserId, t.OfferId })
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.Offers", t => t.OfferId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.OfferId);
            
            DropColumn("dbo.Offers", "User_ID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Offers", "User_ID", c => c.Int());
            DropForeignKey("dbo.OfferSentTo", "OfferId", "dbo.Offers");
            DropForeignKey("dbo.OfferSentTo", "UserId", "dbo.Users");
            DropIndex("dbo.OfferSentTo", new[] { "OfferId" });
            DropIndex("dbo.OfferSentTo", new[] { "UserId" });
            DropTable("dbo.OfferSentTo");
            CreateIndex("dbo.Offers", "User_ID");
            AddForeignKey("dbo.Offers", "User_ID", "dbo.Users", "ID");
        }
    }
}
