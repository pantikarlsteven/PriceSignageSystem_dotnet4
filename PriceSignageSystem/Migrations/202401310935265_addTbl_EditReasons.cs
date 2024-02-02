namespace PriceSignageSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addTbl_EditReasons : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EditReasons",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.EditReasons");
        }
    }
}
