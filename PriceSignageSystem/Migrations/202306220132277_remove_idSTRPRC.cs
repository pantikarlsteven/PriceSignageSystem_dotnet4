namespace PriceSignageSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class remove_idSTRPRC : DbMigration
    {
        public override void Up()
        {
            AddPrimaryKey("dbo.STRPRCs", "O3SKU");
        }
        
        public override void Down()
        {
      
        }
    }
}
