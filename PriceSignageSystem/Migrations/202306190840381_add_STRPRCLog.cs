namespace PriceSignageSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_STRPRCLog : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.STRPRCLogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        O3SKU = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ColumnName = c.String(),
                        FromValue = c.String(),
                        ToValue = c.String(),
                        DateUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.STRPRCLogs");
        }
    }
}
