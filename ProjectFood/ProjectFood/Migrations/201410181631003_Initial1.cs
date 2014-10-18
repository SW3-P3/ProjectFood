namespace ProjectFood.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial1 : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.Items");
            AlterColumn("dbo.Items", "ID", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.Items", "ID");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.Items");
            AlterColumn("dbo.Items", "ID", c => c.String(nullable: false, maxLength: 128));
            AddPrimaryKey("dbo.Items", "ID");
        }
    }
}
