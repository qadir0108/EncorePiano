namespace WFP.ICT.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatedconsignments : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PianoAssignment", "DriverId", "dbo.Driver");
            DropIndex("dbo.PianoAssignment", new[] { "DriverId" });
            CreateTable(
                "dbo.DriverPianoAssignment",
                c => new
                    {
                        Driver_Id = c.Guid(nullable: false),
                        PianoAssignment_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.Driver_Id, t.PianoAssignment_Id })
                .ForeignKey("dbo.Driver", t => t.Driver_Id, cascadeDelete: true)
                .ForeignKey("dbo.PianoAssignment", t => t.PianoAssignment_Id, cascadeDelete: true)
                .Index(t => t.Driver_Id)
                .Index(t => t.PianoAssignment_Id);
            
            DropColumn("dbo.PianoAssignment", "DriverId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PianoAssignment", "DriverId", c => c.Guid());
            DropForeignKey("dbo.DriverPianoAssignment", "PianoAssignment_Id", "dbo.PianoAssignment");
            DropForeignKey("dbo.DriverPianoAssignment", "Driver_Id", "dbo.Driver");
            DropIndex("dbo.DriverPianoAssignment", new[] { "PianoAssignment_Id" });
            DropIndex("dbo.DriverPianoAssignment", new[] { "Driver_Id" });
            DropTable("dbo.DriverPianoAssignment");
            CreateIndex("dbo.PianoAssignment", "DriverId");
            AddForeignKey("dbo.PianoAssignment", "DriverId", "dbo.Driver", "Id");
        }
    }
}
