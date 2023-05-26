namespace PriceSignageSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_ItemQueueTbl : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.ItemQueues");
            AddColumn("dbo.ItemQueues", "Id", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.ItemQueues", "Id");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.ItemQueues");
            DropColumn("dbo.ItemQueues", "Id");
            AddPrimaryKey("dbo.ItemQueues", "O3SKU");
        }
    }
}
