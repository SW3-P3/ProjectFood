namespace ProjectFood.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial1 : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Items_Offers", newName: "OfferItems");
            RenameColumn(table: "dbo.OfferItems", name: "ItemID", newName: "Item_ID");
            RenameColumn(table: "dbo.OfferItems", name: "OfferID", newName: "Offer_ID");
            RenameIndex(table: "dbo.OfferItems", name: "IX_OfferID", newName: "IX_Offer_ID");
            RenameIndex(table: "dbo.OfferItems", name: "IX_ItemID", newName: "IX_Item_ID");
            DropPrimaryKey("dbo.OfferItems");
            AddPrimaryKey("dbo.OfferItems", new[] { "Offer_ID", "Item_ID" });
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.OfferItems");
            AddPrimaryKey("dbo.OfferItems", new[] { "ItemID", "OfferID" });
            RenameIndex(table: "dbo.OfferItems", name: "IX_Item_ID", newName: "IX_ItemID");
            RenameIndex(table: "dbo.OfferItems", name: "IX_Offer_ID", newName: "IX_OfferID");
            RenameColumn(table: "dbo.OfferItems", name: "Offer_ID", newName: "OfferID");
            RenameColumn(table: "dbo.OfferItems", name: "Item_ID", newName: "ItemID");
            RenameTable(name: "dbo.OfferItems", newName: "Items_Offers");
        }
    }
}
