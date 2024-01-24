namespace PriceSignageSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addIsExempField_StrprcTbl : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.STRPRCs", "IsExemp", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.STRPRCs", "IsExemp");
        }
    }
}
