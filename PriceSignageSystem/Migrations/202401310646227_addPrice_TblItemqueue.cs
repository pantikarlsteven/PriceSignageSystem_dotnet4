namespace PriceSignageSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addPrice_TblItemqueue : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemQueues", "RegularPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.ItemQueues", "CurrentPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ItemQueues", "CurrentPrice");
            DropColumn("dbo.ItemQueues", "RegularPrice");
        }
    }
}
