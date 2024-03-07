namespace PriceSignageSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addTbl_Audit_again : DbMigration
    {
        public override void Up()
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
        
        public override void Down()
        {
            DropTable("dbo.Audits");
        }
    }
}
