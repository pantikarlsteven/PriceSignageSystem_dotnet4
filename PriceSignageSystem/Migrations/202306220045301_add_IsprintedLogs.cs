namespace PriceSignageSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_IsprintedLogs : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.STRPRCLogs", "IsPrinted", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.STRPRCLogs", "IsPrinted");
        }
    }
}
