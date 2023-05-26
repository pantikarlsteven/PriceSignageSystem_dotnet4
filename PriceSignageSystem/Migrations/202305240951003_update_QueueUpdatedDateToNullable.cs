namespace PriceSignageSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_QueueUpdatedDateToNullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ItemQueues", "DateUpdated", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ItemQueues", "DateUpdated", c => c.DateTime(nullable: false));
        }
    }
}
