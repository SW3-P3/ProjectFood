namespace ProjectFood.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class recipe : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Recipes",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Titel = c.String(),
                        Minutes = c.Int(nullable: false),
                        Instructions = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Recipe_Ingredient",
                c => new
                    {
                        RecipeID = c.Int(nullable: false),
                        IngredientID = c.Int(nullable: false),
                        Amount = c.Double(nullable: false),
                        Unit = c.String(),
                    })
                .PrimaryKey(t => new { t.RecipeID, t.IngredientID })
                .ForeignKey("dbo.Items", t => t.IngredientID, cascadeDelete: true)
                .ForeignKey("dbo.Recipes", t => t.RecipeID, cascadeDelete: true)
                .Index(t => t.RecipeID)
                .Index(t => t.IngredientID);
            
            CreateTable(
                "dbo.RecipeItems",
                c => new
                    {
                        Recipe_ID = c.Int(nullable: false),
                        Item_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Recipe_ID, t.Item_ID })
                .ForeignKey("dbo.Recipes", t => t.Recipe_ID, cascadeDelete: true)
                .ForeignKey("dbo.Items", t => t.Item_ID, cascadeDelete: true)
                .Index(t => t.Recipe_ID)
                .Index(t => t.Item_ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Recipe_Ingredient", "RecipeID", "dbo.Recipes");
            DropForeignKey("dbo.Recipe_Ingredient", "IngredientID", "dbo.Items");
            DropForeignKey("dbo.RecipeItems", "Item_ID", "dbo.Items");
            DropForeignKey("dbo.RecipeItems", "Recipe_ID", "dbo.Recipes");
            DropIndex("dbo.RecipeItems", new[] { "Item_ID" });
            DropIndex("dbo.RecipeItems", new[] { "Recipe_ID" });
            DropIndex("dbo.Recipe_Ingredient", new[] { "IngredientID" });
            DropIndex("dbo.Recipe_Ingredient", new[] { "RecipeID" });
            DropTable("dbo.RecipeItems");
            DropTable("dbo.Recipe_Ingredient");
            DropTable("dbo.Recipes");
        }
    }
}
