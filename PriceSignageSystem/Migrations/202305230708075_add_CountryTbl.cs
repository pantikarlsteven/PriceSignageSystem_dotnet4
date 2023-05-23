namespace PriceSignageSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_CountryTbl : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Countries",
                c => new
                    {
                        iatrb3 = c.String(nullable: false, maxLength: 128),
                        country_img = c.Binary(),
                    })
                .PrimaryKey(t => t.iatrb3);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Countries");
        }
    }
}
