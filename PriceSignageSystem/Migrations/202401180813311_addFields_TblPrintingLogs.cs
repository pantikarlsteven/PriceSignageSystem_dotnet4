namespace PriceSignageSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addFields_TblPrintingLogs : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InventoryPrintingLogs", "RegularPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.InventoryPrintingLogs", "CurrentPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.InventoryPrintingLogs", "Remarks", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.InventoryPrintingLogs", "Remarks");
            DropColumn("dbo.InventoryPrintingLogs", "CurrentPrice");
            DropColumn("dbo.InventoryPrintingLogs", "RegularPrice");
        }
    }
}
