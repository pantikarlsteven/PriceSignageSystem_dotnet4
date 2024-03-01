namespace PriceSignageSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTbl_CompleteSTRPRC : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CompleteSTRPRCs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IUPC = c.Decimal(nullable: false, precision: 18, scale: 2),
                        INUMBR = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.CompleteSTRPRCs");
        }
    }
}
