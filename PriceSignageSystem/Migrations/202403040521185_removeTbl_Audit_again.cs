namespace PriceSignageSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removeTbl_Audit_again : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.Audits");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Audits",
                c => new
                    {
                        Sku = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsAudited = c.String(),
                        AuditedBy = c.String(),
                        DateAudited = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Sku);
            
        }
    }
}
