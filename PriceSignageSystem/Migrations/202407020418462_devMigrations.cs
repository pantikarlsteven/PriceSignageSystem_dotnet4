namespace PriceSignageSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class devMigrations : DbMigration
    {
        public override void Up()
        {
            //AddColumn("dbo.STRPRCs", "PromoVal", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            //DropColumn("dbo.STRPRCs", "PromoVal");
        }
    }
}
