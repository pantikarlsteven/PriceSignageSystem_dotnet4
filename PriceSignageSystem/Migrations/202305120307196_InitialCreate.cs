namespace PriceSignageSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Sizes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.STRPRCs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        O3LOC = c.Decimal(nullable: false, precision: 18, scale: 2),
                        O3SKU = c.Decimal(nullable: false, precision: 18, scale: 2),
                        O3SCCD = c.String(),
                        O3IDSC = c.String(),
                        O3UPC = c.Decimal(nullable: false, precision: 18, scale: 2),
                        O3VNUM = c.Decimal(nullable: false, precision: 18, scale: 2),
                        O3TYPE = c.String(),
                        O3DEPT = c.Decimal(nullable: false, precision: 18, scale: 2),
                        O3SDPT = c.Decimal(nullable: false, precision: 18, scale: 2),
                        O3CLAS = c.Decimal(nullable: false, precision: 18, scale: 2),
                        O3SCLS = c.Decimal(nullable: false, precision: 18, scale: 2),
                        O3POS = c.Decimal(nullable: false, precision: 18, scale: 2),
                        O3POSU = c.Decimal(nullable: false, precision: 18, scale: 2),
                        O3REG = c.Decimal(nullable: false, precision: 18, scale: 2),
                        O3REGU = c.Decimal(nullable: false, precision: 18, scale: 2),
                        O3ORIG = c.Decimal(nullable: false, precision: 18, scale: 2),
                        O3ORGU = c.Decimal(nullable: false, precision: 18, scale: 2),
                        O3EVT = c.Decimal(nullable: false, precision: 18, scale: 2),
                        O3PMMX = c.Decimal(nullable: false, precision: 18, scale: 2),
                        O3PMTH = c.Decimal(nullable: false, precision: 18, scale: 2),
                        O3PDQT = c.Decimal(nullable: false, precision: 18, scale: 2),
                        O3PDPR = c.Decimal(nullable: false, precision: 18, scale: 2),
                        O3SDT = c.Decimal(nullable: false, precision: 18, scale: 2),
                        O3EDT = c.Decimal(nullable: false, precision: 18, scale: 2),
                        O3TRB3 = c.String(),
                        O3FGR = c.Decimal(nullable: false, precision: 18, scale: 2),
                        O3FNAM = c.String(),
                        O3SLUM = c.String(),
                        O3DIV = c.String(),
                        O3TUOM = c.String(),
                        O3DATE = c.Decimal(nullable: false, precision: 18, scale: 2),
                        O3CURD = c.Decimal(nullable: false, precision: 18, scale: 2),
                        O3USER = c.String(),
                        DateUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Types",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                        Password = c.String(),
                        IsActive = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Users");
            DropTable("dbo.Types");
            DropTable("dbo.STRPRCs");
            DropTable("dbo.Sizes");
            DropTable("dbo.Categories");
        }
    }
}
