namespace ProjectFood.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovedUnused : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.RecipeItems", "Recipe_ID", "dbo.Recipes");
            DropForeignKey("dbo.RecipeItems", "Item_ID", "dbo.Items");
            DropForeignKey("dbo.GroupUsers", "Group_ID", "dbo.Groups");
            DropForeignKey("dbo.GroupUsers", "User_ID", "dbo.Users");
            DropForeignKey("dbo.ShoppingListItems", "ShoppingList_ID", "dbo.ShoppingLists");
            DropForeignKey("dbo.ShoppingListItems", "Item_ID", "dbo.Items");
            DropIndex("dbo.RecipeItems", new[] { "Recipe_ID" });
            DropIndex("dbo.RecipeItems", new[] { "Item_ID" });
            DropIndex("dbo.GroupUsers", new[] { "Group_ID" });
            DropIndex("dbo.GroupUsers", new[] { "User_ID" });
            DropIndex("dbo.ShoppingListItems", new[] { "ShoppingList_ID" });
            DropIndex("dbo.ShoppingListItems", new[] { "Item_ID" });
            AddColumn("dbo.Items", "Recipe_ID", c => c.Int());
            AddColumn("dbo.Items", "ShoppingList_ID", c => c.Int());
            CreateIndex("dbo.Items", "Recipe_ID");
            CreateIndex("dbo.Items", "ShoppingList_ID");
            AddForeignKey("dbo.Items", "Recipe_ID", "dbo.Recipes", "ID");
            AddForeignKey("dbo.Items", "ShoppingList_ID", "dbo.ShoppingLists", "ID");
            DropColumn("dbo.Items", "Category");
            DropColumn("dbo.Recipes", "Tags");
            DropColumn("dbo.Offers", "Store_ID");
            DropColumn("dbo.Offers", "Image_URL");
            DropTable("dbo.Groups");
            DropTable("dbo.RecipeItems");
            DropTable("dbo.GroupUsers");
            DropTable("dbo.ShoppingListItems");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ShoppingListItems",
                c => new
                    {
                        ShoppingList_ID = c.Int(nullable: false),
                        Item_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ShoppingList_ID, t.Item_ID });
            
            CreateTable(
                "dbo.GroupUsers",
                c => new
                    {
                        Group_ID = c.Int(nullable: false),
                        User_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Group_ID, t.User_ID });
            
            CreateTable(
                "dbo.RecipeItems",
                c => new
                    {
                        Recipe_ID = c.Int(nullable: false),
                        Item_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Recipe_ID, t.Item_ID });
            
            CreateTable(
                "dbo.Groups",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            AddColumn("dbo.Offers", "Image_URL", c => c.String());
            AddColumn("dbo.Offers", "Store_ID", c => c.String());
            AddColumn("dbo.Recipes", "Tags", c => c.String());
            AddColumn("dbo.Items", "Category", c => c.String());
            DropForeignKey("dbo.Items", "ShoppingList_ID", "dbo.ShoppingLists");
            DropForeignKey("dbo.Items", "Recipe_ID", "dbo.Recipes");
            DropIndex("dbo.Items", new[] { "ShoppingList_ID" });
            DropIndex("dbo.Items", new[] { "Recipe_ID" });
            DropColumn("dbo.Items", "ShoppingList_ID");
            DropColumn("dbo.Items", "Recipe_ID");
            CreateIndex("dbo.ShoppingListItems", "Item_ID");
            CreateIndex("dbo.ShoppingListItems", "ShoppingList_ID");
            CreateIndex("dbo.GroupUsers", "User_ID");
            CreateIndex("dbo.GroupUsers", "Group_ID");
            CreateIndex("dbo.RecipeItems", "Item_ID");
            CreateIndex("dbo.RecipeItems", "Recipe_ID");
            AddForeignKey("dbo.ShoppingListItems", "Item_ID", "dbo.Items", "ID", cascadeDelete: true);
            AddForeignKey("dbo.ShoppingListItems", "ShoppingList_ID", "dbo.ShoppingLists", "ID", cascadeDelete: true);
            AddForeignKey("dbo.GroupUsers", "User_ID", "dbo.Users", "ID", cascadeDelete: true);
            AddForeignKey("dbo.GroupUsers", "Group_ID", "dbo.Groups", "ID", cascadeDelete: true);
            AddForeignKey("dbo.RecipeItems", "Item_ID", "dbo.Items", "ID", cascadeDelete: true);
            AddForeignKey("dbo.RecipeItems", "Recipe_ID", "dbo.Recipes", "ID", cascadeDelete: true);
        }
    }
}
