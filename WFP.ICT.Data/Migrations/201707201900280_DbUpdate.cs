namespace WFP.ICT.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DbUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PianoOrder", "PickUpNotes", c => c.String());
            AddColumn("dbo.PianoOrder", "DeliveryNotes", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PianoOrder", "DeliveryNotes");
            DropColumn("dbo.PianoOrder", "PickUpNotes");
        }
    }
}
