namespace PriceSignageSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTuom_QueueTbl : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemQueues", "Tuom", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ItemQueues", "Tuom");
        }
    }
}
