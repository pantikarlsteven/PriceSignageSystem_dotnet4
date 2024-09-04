namespace PriceSignageSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddColumnsForPrinting : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InventoryPrintingLogs", "ExpDateCER", c => c.String());
            AddColumn("dbo.InventoryPrintingLogs", "IsEdited", c => c.String());
            AddColumn("dbo.ItemQueues", "ExpDateCER", c => c.String());
            AddColumn("dbo.ItemQueues", "IsEdited", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ItemQueues", "IsEdited");
            DropColumn("dbo.ItemQueues", "ExpDateCER");
            DropColumn("dbo.InventoryPrintingLogs", "IsEdited");
            DropColumn("dbo.InventoryPrintingLogs", "ExpDateCER");
        }
    }
}
