namespace PriceSignageSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_InventoryPrintingLogTbl : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.InventoryPrintingLogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        O3SKU = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PrintedBy = c.String(),
                        DateCreated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.InventoryPrintingLogs");
        }
    }
}
