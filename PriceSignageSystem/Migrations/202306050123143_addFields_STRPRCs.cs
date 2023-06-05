namespace PriceSignageSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addFields_STRPRCs : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.STRPRCs", "TypeId", c => c.Int(nullable: false));
            AddColumn("dbo.STRPRCs", "SizeId", c => c.Int(nullable: false));
            AddColumn("dbo.STRPRCs", "CategoryId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.STRPRCs", "CategoryId");
            DropColumn("dbo.STRPRCs", "SizeId");
            DropColumn("dbo.STRPRCs", "TypeId");
        }
    }
}
