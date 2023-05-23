namespace PriceSignageSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_STRPRCtbl : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.STRPRCs", "O3MODL", c => c.String());
            AddColumn("dbo.STRPRCs", "O3LONG", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.STRPRCs", "O3LONG");
            DropColumn("dbo.STRPRCs", "O3MODL");
        }
    }
}
