namespace PriceSignageSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_Queuetbl : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ItemQueues",
                c => new
                    {
                        O3SKU = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TypeId = c.Int(nullable: false),
                        SizeId = c.Int(nullable: false),
                        CategoryId = c.Int(nullable: false),
                        UserId = c.String(),
                        Status = c.String(),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.O3SKU);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ItemQueues");
        }
    }
}
