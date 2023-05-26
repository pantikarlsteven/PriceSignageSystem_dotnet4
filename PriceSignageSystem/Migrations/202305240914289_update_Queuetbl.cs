namespace PriceSignageSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_Queuetbl : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemQueues", "UserName", c => c.String());
            DropColumn("dbo.ItemQueues", "UserId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ItemQueues", "UserId", c => c.String());
            DropColumn("dbo.ItemQueues", "UserName");
        }
    }
}
