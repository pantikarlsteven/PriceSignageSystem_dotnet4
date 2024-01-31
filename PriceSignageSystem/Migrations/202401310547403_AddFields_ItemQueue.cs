namespace PriceSignageSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFields_ItemQueue : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemQueues", "ItemDesc", c => c.String());
            AddColumn("dbo.ItemQueues", "Brand", c => c.String());
            AddColumn("dbo.ItemQueues", "Model", c => c.String());
            AddColumn("dbo.ItemQueues", "Divisor", c => c.String());
            AddColumn("dbo.ItemQueues", "Remarks", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ItemQueues", "Remarks");
            DropColumn("dbo.ItemQueues", "Divisor");
            DropColumn("dbo.ItemQueues", "Model");
            DropColumn("dbo.ItemQueues", "Brand");
            DropColumn("dbo.ItemQueues", "ItemDesc");
        }
    }
}
