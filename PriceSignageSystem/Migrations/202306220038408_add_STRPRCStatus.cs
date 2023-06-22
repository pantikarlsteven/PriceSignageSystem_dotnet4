namespace PriceSignageSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_STRPRCStatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.STRPRCs", "Status", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.STRPRCs", "Status");
        }
    }
}
