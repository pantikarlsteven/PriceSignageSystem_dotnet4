namespace PriceSignageSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveFIelds_Itemqueue : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.ItemQueues", "SizeId");
            DropColumn("dbo.ItemQueues", "CategoryId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ItemQueues", "CategoryId", c => c.Int(nullable: false));
            AddColumn("dbo.ItemQueues", "SizeId", c => c.Int(nullable: false));
        }
    }
}
