namespace WFP.ICT.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Piano", "ClientId", c => c.Guid());
            CreateIndex("dbo.Piano", "ClientId");
            AddForeignKey("dbo.Piano", "ClientId", "dbo.Client", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Piano", "ClientId", "dbo.Client");
            DropIndex("dbo.Piano", new[] { "ClientId" });
            DropColumn("dbo.Piano", "ClientId");
        }
    }
}
