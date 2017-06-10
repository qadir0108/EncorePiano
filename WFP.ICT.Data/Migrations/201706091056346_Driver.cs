namespace WFP.ICT.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Driver : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Driver", "FCMToken", c => c.String());
            AddColumn("dbo.DriverLogin", "DriverId", c => c.Guid(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DriverLogin", "DriverId");
            DropColumn("dbo.Driver", "FCMToken");
        }
    }
}
