namespace PriceSignageSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removeFields_TblQueues : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.ItemQueues", "RegularPrice");
            DropColumn("dbo.ItemQueues", "CurrentPrice");
            DropColumn("dbo.ItemQueues", "Remarks");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ItemQueues", "Remarks", c => c.String());
            AddColumn("dbo.ItemQueues", "CurrentPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.ItemQueues", "RegularPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
