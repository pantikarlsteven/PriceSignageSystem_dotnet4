namespace PriceSignageSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFields_invLogtbl : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InventoryPrintingLogs", "ItemDesc", c => c.String());
            AddColumn("dbo.InventoryPrintingLogs", "Brand", c => c.String());
            AddColumn("dbo.InventoryPrintingLogs", "Model", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.InventoryPrintingLogs", "Model");
            DropColumn("dbo.InventoryPrintingLogs", "Brand");
            DropColumn("dbo.InventoryPrintingLogs", "ItemDesc");
        }
    }
}
