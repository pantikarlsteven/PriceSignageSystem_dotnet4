namespace PriceSignageSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addEmpIdField_Users : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "EmployeeId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "EmployeeId");
        }
    }
}
