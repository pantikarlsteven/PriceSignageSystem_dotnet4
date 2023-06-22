namespace PriceSignageSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_IsPrintedSTRPRC : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.STRPRCs", "IsPrinted", c => c.Boolean(nullable: false));
            DropColumn("dbo.STRPRCs", "Status");
        }
        
        public override void Down()
        {
            AddColumn("dbo.STRPRCs", "Status", c => c.String());
            DropColumn("dbo.STRPRCs", "IsPrinted");
        }
    }
}
