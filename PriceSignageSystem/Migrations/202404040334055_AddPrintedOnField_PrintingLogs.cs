namespace PriceSignageSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPrintedOnField_PrintingLogs : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InventoryPrintingLogs", "PrintedOn", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.InventoryPrintingLogs", "PrintedOn");
        }
    }
}
