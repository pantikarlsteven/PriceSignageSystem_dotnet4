namespace PriceSignageSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSizeId_ItemQueueTbl : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemQueues", "SizeId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ItemQueues", "SizeId");
        }
    }
}
