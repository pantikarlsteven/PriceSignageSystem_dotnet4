namespace PriceSignageSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDivField_invLogtbl : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InventoryPrintingLogs", "Divisor", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.InventoryPrintingLogs", "Divisor");
        }
    }
}
