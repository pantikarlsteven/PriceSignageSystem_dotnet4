namespace PriceSignageSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removeIsExempField_StrprcTbl : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.STRPRCs", "IsExemp");
        }
        
        public override void Down()
        {
            AddColumn("dbo.STRPRCs", "IsExemp", c => c.String());
        }
    }
}
