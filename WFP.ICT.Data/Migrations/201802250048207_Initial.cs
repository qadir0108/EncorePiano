namespace WFP.ICT.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Address",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        PhoneNumber = c.String(),
                        AlternateContact = c.String(),
                        AlternatePhone = c.String(),
                        Address1 = c.String(),
                        City = c.String(),
                        PostCode = c.String(),
                        State = c.String(),
                        NumberStairs = c.Int(nullable: false),
                        NumberTurns = c.Int(nullable: false),
                        Notes = c.String(),
                        Lat = c.String(),
                        Lng = c.String(),
                        AddressType = c.Int(nullable: false),
                        ClientId = c.Guid(),
                        WarehouseId = c.Guid(),
                        CreatedAt = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.APIRequestLog",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        RequestType = c.Int(nullable: false),
                        Request = c.String(),
                        CreatedAt = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AssignmentRoute",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Lat = c.String(),
                        Lng = c.String(),
                        Order = c.Int(nullable: false),
                        AssignmentId = c.Guid(),
                        CreatedAt = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Assignment", t => t.AssignmentId)
                .Index(t => t.AssignmentId);
            
            CreateTable(
                "dbo.Assignment",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        AssignmentNumber = c.String(),
                        VehicleId = c.Guid(),
                        PickupTicket = c.String(),
                        PickupTicketGenerationTime = c.DateTime(),
                        StartTime = c.DateTime(),
                        EstimatedTime = c.DateTime(),
                        MinutesAway = c.Int(nullable: false),
                        OrderId = c.Guid(),
                        LegId = c.Guid(),
                        CreatedAt = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Order", t => t.OrderId)
                .ForeignKey("dbo.Leg", t => t.LegId)
                .ForeignKey("dbo.Vehicle", t => t.VehicleId)
                .Index(t => t.VehicleId)
                .Index(t => t.OrderId)
                .Index(t => t.LegId);
            
            CreateTable(
                "dbo.Driver",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Code = c.String(),
                        Name = c.String(),
                        Description = c.String(),
                        Password = c.String(),
                        FCMToken = c.String(),
                        WarehouseId = c.Guid(),
                        CreatedAt = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Leg",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        LegNumber = c.Int(nullable: false),
                        LegType = c.Int(nullable: false),
                        FromLocationId = c.Guid(),
                        ToLocationId = c.Guid(),
                        DriverId = c.Guid(),
                        LegDate = c.DateTime(),
                        OrderId = c.Guid(),
                        CreatedAt = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Driver", t => t.DriverId)
                .ForeignKey("dbo.Location", t => t.FromLocationId)
                .ForeignKey("dbo.Order", t => t.OrderId)
                .ForeignKey("dbo.Location", t => t.ToLocationId)
                .Index(t => t.FromLocationId)
                .Index(t => t.ToLocationId)
                .Index(t => t.DriverId)
                .Index(t => t.OrderId);
            
            CreateTable(
                "dbo.Location",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        ClientId = c.Guid(),
                        AddressId = c.Guid(),
                        CreatedAt = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Address", t => t.AddressId)
                .Index(t => t.AddressId);
            
            CreateTable(
                "dbo.Order",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        OrderNumber = c.String(),
                        OrderType = c.Int(nullable: false),
                        CallerFirstName = c.String(),
                        CallerLastName = c.String(),
                        CallerPhoneNumber = c.String(),
                        CallerEmail = c.String(),
                        CallerAlternatePhone = c.String(),
                        PaymentOption = c.Int(nullable: false),
                        SalesOrderNumber = c.String(),
                        PickupDate = c.DateTime(),
                        DeliveryDate = c.DateTime(),
                        PickupAddressId = c.Guid(),
                        DeliveryAddressId = c.Guid(),
                        ClientId = c.Guid(),
                        OrderBillingId = c.Guid(),
                        DeliveryForm = c.String(),
                        CodAmount = c.Double(nullable: false),
                        OfficeStaff = c.String(),
                        OnlinePayment = c.String(),
                        PickUpNotes = c.String(),
                        DeliveryNotes = c.String(),
                        CarriedBy = c.String(),
                        BillToDifferent = c.Boolean(nullable: false),
                        InvoiceClientId = c.Guid(),
                        InvoiceBillingPartyId = c.Guid(),
                        CreatedAt = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        Client_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.OrderBilling", t => t.OrderBillingId)
                .ForeignKey("dbo.Client", t => t.Client_Id)
                .ForeignKey("dbo.Client", t => t.ClientId)
                .ForeignKey("dbo.Address", t => t.DeliveryAddressId)
                .ForeignKey("dbo.Client", t => t.InvoiceBillingPartyId)
                .ForeignKey("dbo.Client", t => t.InvoiceClientId)
                .ForeignKey("dbo.Address", t => t.PickupAddressId)
                .Index(t => t.PickupAddressId)
                .Index(t => t.DeliveryAddressId)
                .Index(t => t.ClientId)
                .Index(t => t.OrderBillingId)
                .Index(t => t.InvoiceClientId)
                .Index(t => t.InvoiceBillingPartyId)
                .Index(t => t.Client_Id);
            
            CreateTable(
                "dbo.OrderBilling",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        PaymentOption = c.Int(nullable: false),
                        BillingType = c.Int(nullable: false),
                        Amount = c.Long(nullable: false),
                        OrderId = c.Guid(),
                        CreatedAt = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Client",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        AccountCode = c.String(),
                        CompanyLogo = c.String(),
                        Name = c.String(),
                        PhoneNumber = c.String(),
                        EmailAddress = c.String(),
                        Comment = c.String(),
                        ClientInvoiceId = c.Guid(),
                        ClientPaymentId = c.Guid(),
                        ClientType = c.Int(nullable: false),
                        AddressId = c.Guid(),
                        CreatedAt = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Address", t => t.AddressId)
                .Index(t => t.AddressId);
            
            CreateTable(
                "dbo.ClientStore",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        Code = c.String(),
                        AddressId = c.Guid(),
                        CreatedAt = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        Client_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Address", t => t.AddressId)
                .ForeignKey("dbo.Client", t => t.Client_Id)
                .Index(t => t.AddressId)
                .Index(t => t.Client_Id);
            
            CreateTable(
                "dbo.ClientInvoice",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        InvoiceNumber = c.String(),
                        SentOn = c.DateTime(),
                        Amount = c.Long(nullable: false),
                        Status = c.Int(nullable: false),
                        Notes = c.String(),
                        GeneratedAt = c.DateTime(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        ClientId = c.Guid(),
                        CreatedAt = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Client", t => t.ClientId)
                .Index(t => t.ClientId);
            
            CreateTable(
                "dbo.ClientPayment",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        PaymentType = c.Int(nullable: false),
                        CheckNumber = c.String(),
                        PaymentDate = c.DateTime(nullable: false),
                        Amount = c.Long(nullable: false),
                        TransactionNumber = c.String(),
                        ClientId = c.Guid(),
                        ClientInvoiceId = c.Guid(),
                        OrderId = c.Guid(),
                        CreatedAt = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Client", t => t.ClientId)
                .Index(t => t.ClientId);
            
            CreateTable(
                "dbo.OrderCharges",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Amount = c.Long(nullable: false),
                        ServiceStatus = c.Int(nullable: false),
                        PianoChargesId = c.Guid(),
                        OrderId = c.Guid(),
                        CreatedAt = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Order", t => t.OrderId)
                .ForeignKey("dbo.PianoCharges", t => t.PianoChargesId)
                .Index(t => t.PianoChargesId)
                .Index(t => t.OrderId);
            
            CreateTable(
                "dbo.PianoCharges",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Code = c.String(),
                        Details = c.String(),
                        Amount = c.Long(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Piano",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        PianoCategoryType = c.Int(nullable: false),
                        SerialNumber = c.String(),
                        Model = c.String(),
                        IsBench = c.Boolean(nullable: false),
                        IsBoxed = c.Boolean(nullable: false),
                        IsPlayer = c.Boolean(nullable: false),
                        Notes = c.String(),
                        ReceivedDate = c.DateTime(),
                        ShippedDate = c.DateTime(),
                        PianoTypeId = c.Guid(),
                        PianoMakeId = c.Guid(),
                        PianoSizeId = c.Guid(),
                        PianoFinishId = c.Guid(),
                        DrawingOfPiano = c.String(),
                        WareHouseStatus = c.Int(nullable: false),
                        IsLocated = c.Boolean(nullable: false),
                        IsLocatedInThirdParty = c.Boolean(nullable: false),
                        WarehouseId = c.Guid(),
                        ClientId = c.Guid(),
                        OrderId = c.Guid(),
                        CreatedAt = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        PianoQuote_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Client", t => t.ClientId)
                .ForeignKey("dbo.Order", t => t.OrderId)
                .ForeignKey("dbo.PianoFinish", t => t.PianoFinishId)
                .ForeignKey("dbo.PianoMake", t => t.PianoMakeId)
                .ForeignKey("dbo.PianoSize", t => t.PianoSizeId)
                .ForeignKey("dbo.PianoType", t => t.PianoTypeId)
                .ForeignKey("dbo.Warehouse", t => t.WarehouseId)
                .ForeignKey("dbo.PianoQuote", t => t.PianoQuote_Id)
                .Index(t => t.PianoTypeId)
                .Index(t => t.PianoMakeId)
                .Index(t => t.PianoSizeId)
                .Index(t => t.PianoFinishId)
                .Index(t => t.WarehouseId)
                .Index(t => t.ClientId)
                .Index(t => t.OrderId)
                .Index(t => t.PianoQuote_Id);
            
            CreateTable(
                "dbo.PianoFinish",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Code = c.String(),
                        Name = c.String(),
                        CreatedAt = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PianoMake",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Code = c.String(),
                        Name = c.String(),
                        CreatedAt = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PianoSize",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Width = c.Double(nullable: false),
                        PianoTypeId = c.Guid(),
                        CreatedAt = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PianoType", t => t.PianoTypeId)
                .Index(t => t.PianoTypeId);
            
            CreateTable(
                "dbo.PianoType",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Code = c.String(),
                        Type = c.String(),
                        CreatedAt = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PianoPicture",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        PictureType = c.Int(nullable: false),
                        Picture = c.String(),
                        PianoId = c.Guid(),
                        ProofId = c.Guid(),
                        CreatedAt = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Piano", t => t.PianoId)
                .ForeignKey("dbo.Proof", t => t.ProofId)
                .Index(t => t.PianoId)
                .Index(t => t.ProofId);
            
            CreateTable(
                "dbo.Proof",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ProofType = c.Int(nullable: false),
                        IsMainUnitLoaded = c.Boolean(nullable: false),
                        LoadTime = c.DateTime(),
                        PickerName = c.String(),
                        PickerSignature = c.String(),
                        AdditionalBench1Status = c.Int(nullable: false),
                        AdditionalBench2Status = c.Int(nullable: false),
                        AdditionalCasterCupsStatus = c.Int(nullable: false),
                        AdditionalCoverStatus = c.Int(nullable: false),
                        AdditionalLampStatus = c.Int(nullable: false),
                        AdditionalOwnersManualStatus = c.Int(nullable: false),
                        AdditionalMisc1Status = c.Int(nullable: false),
                        AdditionalMisc2Status = c.Int(nullable: false),
                        AdditionalMisc3Status = c.Int(nullable: false),
                        ReceivingTime = c.DateTime(),
                        ReceivedBy = c.String(),
                        Signature = c.String(),
                        Notes = c.String(),
                        Bench1UnloadStatus = c.Boolean(nullable: false),
                        Bench2UnloadStatus = c.Boolean(nullable: false),
                        CasterCupsUnloadStatus = c.Boolean(nullable: false),
                        CoverUnloadStatus = c.Boolean(nullable: false),
                        LampUnloadStatus = c.Boolean(nullable: false),
                        OwnersManualUnloadStatus = c.Boolean(nullable: false),
                        Misc1UnloadStatus = c.Boolean(nullable: false),
                        Misc2UnloadStatus = c.Boolean(nullable: false),
                        Misc3UnloadStatus = c.Boolean(nullable: false),
                        PodForm = c.String(),
                        AcknowledgeTo = c.String(),
                        PianoId = c.Guid(),
                        AssignmentId = c.Guid(),
                        CreatedAt = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Assignment", t => t.AssignmentId)
                .ForeignKey("dbo.Piano", t => t.PianoId)
                .Index(t => t.PianoId)
                .Index(t => t.AssignmentId);
            
            CreateTable(
                "dbo.PianoStatus",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Status = c.Int(nullable: false),
                        StatusTime = c.DateTime(),
                        StatusBy = c.String(),
                        Comments = c.String(),
                        PianoId = c.Guid(),
                        CreatedAt = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Piano", t => t.PianoId)
                .Index(t => t.PianoId);
            
            CreateTable(
                "dbo.Warehouse",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Code = c.String(),
                        Name = c.String(),
                        AlternateContact = c.String(),
                        PhoneNumber = c.String(),
                        AlternatePhone = c.String(),
                        Address1 = c.String(),
                        City = c.String(),
                        PostCode = c.String(),
                        State = c.String(),
                        Notes = c.String(),
                        Lat = c.String(),
                        Lng = c.String(),
                        CreatedAt = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TripStatus",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Status = c.Int(nullable: false),
                        StatusTime = c.DateTime(),
                        StatusBy = c.String(),
                        Comments = c.String(),
                        AssignmentId = c.Guid(),
                        CreatedAt = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Assignment", t => t.AssignmentId)
                .Index(t => t.AssignmentId);
            
            CreateTable(
                "dbo.Vehicle",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Code = c.String(),
                        Name = c.String(),
                        Description = c.String(),
                        Active = c.Boolean(nullable: false),
                        VehicleTypeId = c.Guid(),
                        CreatedAt = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.VehicleType", t => t.VehicleTypeId)
                .Index(t => t.VehicleTypeId);
            
            CreateTable(
                "dbo.VehicleType",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Code = c.String(),
                        Name = c.String(),
                        CreatedAt = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ClientPaymentCard",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        CardDetails = c.String(),
                        ClientId = c.Guid(),
                        CreatedAt = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetClaims",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                        CreatedAt = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetRoleClaims",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ClaimID = c.Guid(nullable: false),
                        RoleID = c.String(maxLength: 128),
                        CreatedAt = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetClaims", t => t.ClaimID, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleID)
                .Index(t => t.ClaimID)
                .Index(t => t.RoleID);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                        Description = c.String(),
                        IsEditable = c.Boolean(),
                        IsDeletable = c.Boolean(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.Company",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        Details = c.String(),
                        WebSite = c.String(),
                        ActiveDiretoryDomain = c.String(),
                        ActiveDiretoryUserName = c.String(),
                        ActiveDiretoryPassword = c.String(),
                        Logo = c.String(),
                        Theme = c.Int(nullable: false),
                        EmailServer = c.String(),
                        EmailUserName = c.String(),
                        EmailPassword = c.String(),
                        CreatedAt = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DriverLogin",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        DriverId = c.Guid(nullable: false),
                        Token = c.String(),
                        ExpiryTime = c.DateTime(),
                        CreatedAt = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PianoQuote",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        QuoteNumber = c.String(),
                        FirstName = c.String(),
                        LastName = c.String(),
                        PhoneNumber = c.String(),
                        EmailAddress = c.String(),
                        IsStairs = c.Boolean(nullable: false),
                        Notes = c.String(),
                        PickupAddressId = c.Guid(),
                        DeliveryAddressId = c.Guid(),
                        ClientId = c.Guid(),
                        OrderBillingId = c.Guid(),
                        CreatedAt = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        PianoType_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.OrderBilling", t => t.OrderBillingId)
                .ForeignKey("dbo.Client", t => t.ClientId)
                .ForeignKey("dbo.Address", t => t.DeliveryAddressId)
                .ForeignKey("dbo.PianoType", t => t.PianoType_Id)
                .ForeignKey("dbo.Address", t => t.PickupAddressId)
                .Index(t => t.PickupAddressId)
                .Index(t => t.DeliveryAddressId)
                .Index(t => t.ClientId)
                .Index(t => t.OrderBillingId)
                .Index(t => t.PianoType_Id);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        FirstName = c.String(),
                        LastName = c.String(),
                        EmailSignature = c.String(),
                        LastLogin = c.DateTime(),
                        Status = c.Int(nullable: false),
                        UserType = c.Int(nullable: false),
                        ClientId = c.Guid(),
                        CreatedAt = c.DateTime(nullable: false),
                        CreatedByID = c.String(),
                        APIKey = c.String(),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Client", t => t.ClientId)
                .Index(t => t.ClientId)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.DriverAssignment",
                c => new
                    {
                        Driver_Id = c.Guid(nullable: false),
                        Assignment_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.Driver_Id, t.Assignment_Id })
                .ForeignKey("dbo.Driver", t => t.Driver_Id, cascadeDelete: true)
                .ForeignKey("dbo.Assignment", t => t.Assignment_Id, cascadeDelete: true)
                .Index(t => t.Driver_Id)
                .Index(t => t.Assignment_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUsers", "ClientId", "dbo.Client");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.PianoQuote", "PickupAddressId", "dbo.Address");
            DropForeignKey("dbo.PianoQuote", "PianoType_Id", "dbo.PianoType");
            DropForeignKey("dbo.Piano", "PianoQuote_Id", "dbo.PianoQuote");
            DropForeignKey("dbo.PianoQuote", "DeliveryAddressId", "dbo.Address");
            DropForeignKey("dbo.PianoQuote", "ClientId", "dbo.Client");
            DropForeignKey("dbo.PianoQuote", "OrderBillingId", "dbo.OrderBilling");
            DropForeignKey("dbo.AspNetRoleClaims", "RoleID", "dbo.AspNetRoles");
            DropForeignKey("dbo.AspNetRoleClaims", "ClaimID", "dbo.AspNetClaims");
            DropForeignKey("dbo.Assignment", "VehicleId", "dbo.Vehicle");
            DropForeignKey("dbo.Vehicle", "VehicleTypeId", "dbo.VehicleType");
            DropForeignKey("dbo.TripStatus", "AssignmentId", "dbo.Assignment");
            DropForeignKey("dbo.AssignmentRoute", "AssignmentId", "dbo.Assignment");
            DropForeignKey("dbo.Assignment", "LegId", "dbo.Leg");
            DropForeignKey("dbo.Leg", "ToLocationId", "dbo.Location");
            DropForeignKey("dbo.Order", "PickupAddressId", "dbo.Address");
            DropForeignKey("dbo.Piano", "WarehouseId", "dbo.Warehouse");
            DropForeignKey("dbo.PianoStatus", "PianoId", "dbo.Piano");
            DropForeignKey("dbo.PianoPicture", "ProofId", "dbo.Proof");
            DropForeignKey("dbo.Proof", "PianoId", "dbo.Piano");
            DropForeignKey("dbo.Proof", "AssignmentId", "dbo.Assignment");
            DropForeignKey("dbo.PianoPicture", "PianoId", "dbo.Piano");
            DropForeignKey("dbo.Piano", "PianoTypeId", "dbo.PianoType");
            DropForeignKey("dbo.Piano", "PianoSizeId", "dbo.PianoSize");
            DropForeignKey("dbo.PianoSize", "PianoTypeId", "dbo.PianoType");
            DropForeignKey("dbo.Piano", "PianoMakeId", "dbo.PianoMake");
            DropForeignKey("dbo.Piano", "PianoFinishId", "dbo.PianoFinish");
            DropForeignKey("dbo.Piano", "OrderId", "dbo.Order");
            DropForeignKey("dbo.Piano", "ClientId", "dbo.Client");
            DropForeignKey("dbo.OrderCharges", "PianoChargesId", "dbo.PianoCharges");
            DropForeignKey("dbo.OrderCharges", "OrderId", "dbo.Order");
            DropForeignKey("dbo.Leg", "OrderId", "dbo.Order");
            DropForeignKey("dbo.Order", "InvoiceClientId", "dbo.Client");
            DropForeignKey("dbo.Order", "InvoiceBillingPartyId", "dbo.Client");
            DropForeignKey("dbo.Order", "DeliveryAddressId", "dbo.Address");
            DropForeignKey("dbo.Order", "ClientId", "dbo.Client");
            DropForeignKey("dbo.ClientPayment", "ClientId", "dbo.Client");
            DropForeignKey("dbo.Order", "Client_Id", "dbo.Client");
            DropForeignKey("dbo.ClientInvoice", "ClientId", "dbo.Client");
            DropForeignKey("dbo.ClientStore", "Client_Id", "dbo.Client");
            DropForeignKey("dbo.ClientStore", "AddressId", "dbo.Address");
            DropForeignKey("dbo.Client", "AddressId", "dbo.Address");
            DropForeignKey("dbo.Order", "OrderBillingId", "dbo.OrderBilling");
            DropForeignKey("dbo.Assignment", "OrderId", "dbo.Order");
            DropForeignKey("dbo.Leg", "FromLocationId", "dbo.Location");
            DropForeignKey("dbo.Location", "AddressId", "dbo.Address");
            DropForeignKey("dbo.Leg", "DriverId", "dbo.Driver");
            DropForeignKey("dbo.DriverAssignment", "Assignment_Id", "dbo.Assignment");
            DropForeignKey("dbo.DriverAssignment", "Driver_Id", "dbo.Driver");
            DropIndex("dbo.DriverAssignment", new[] { "Assignment_Id" });
            DropIndex("dbo.DriverAssignment", new[] { "Driver_Id" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUsers", new[] { "ClientId" });
            DropIndex("dbo.PianoQuote", new[] { "PianoType_Id" });
            DropIndex("dbo.PianoQuote", new[] { "OrderBillingId" });
            DropIndex("dbo.PianoQuote", new[] { "ClientId" });
            DropIndex("dbo.PianoQuote", new[] { "DeliveryAddressId" });
            DropIndex("dbo.PianoQuote", new[] { "PickupAddressId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.AspNetRoleClaims", new[] { "RoleID" });
            DropIndex("dbo.AspNetRoleClaims", new[] { "ClaimID" });
            DropIndex("dbo.Vehicle", new[] { "VehicleTypeId" });
            DropIndex("dbo.TripStatus", new[] { "AssignmentId" });
            DropIndex("dbo.PianoStatus", new[] { "PianoId" });
            DropIndex("dbo.Proof", new[] { "AssignmentId" });
            DropIndex("dbo.Proof", new[] { "PianoId" });
            DropIndex("dbo.PianoPicture", new[] { "ProofId" });
            DropIndex("dbo.PianoPicture", new[] { "PianoId" });
            DropIndex("dbo.PianoSize", new[] { "PianoTypeId" });
            DropIndex("dbo.Piano", new[] { "PianoQuote_Id" });
            DropIndex("dbo.Piano", new[] { "OrderId" });
            DropIndex("dbo.Piano", new[] { "ClientId" });
            DropIndex("dbo.Piano", new[] { "WarehouseId" });
            DropIndex("dbo.Piano", new[] { "PianoFinishId" });
            DropIndex("dbo.Piano", new[] { "PianoSizeId" });
            DropIndex("dbo.Piano", new[] { "PianoMakeId" });
            DropIndex("dbo.Piano", new[] { "PianoTypeId" });
            DropIndex("dbo.OrderCharges", new[] { "OrderId" });
            DropIndex("dbo.OrderCharges", new[] { "PianoChargesId" });
            DropIndex("dbo.ClientPayment", new[] { "ClientId" });
            DropIndex("dbo.ClientInvoice", new[] { "ClientId" });
            DropIndex("dbo.ClientStore", new[] { "Client_Id" });
            DropIndex("dbo.ClientStore", new[] { "AddressId" });
            DropIndex("dbo.Client", new[] { "AddressId" });
            DropIndex("dbo.Order", new[] { "Client_Id" });
            DropIndex("dbo.Order", new[] { "InvoiceBillingPartyId" });
            DropIndex("dbo.Order", new[] { "InvoiceClientId" });
            DropIndex("dbo.Order", new[] { "OrderBillingId" });
            DropIndex("dbo.Order", new[] { "ClientId" });
            DropIndex("dbo.Order", new[] { "DeliveryAddressId" });
            DropIndex("dbo.Order", new[] { "PickupAddressId" });
            DropIndex("dbo.Location", new[] { "AddressId" });
            DropIndex("dbo.Leg", new[] { "OrderId" });
            DropIndex("dbo.Leg", new[] { "DriverId" });
            DropIndex("dbo.Leg", new[] { "ToLocationId" });
            DropIndex("dbo.Leg", new[] { "FromLocationId" });
            DropIndex("dbo.Assignment", new[] { "LegId" });
            DropIndex("dbo.Assignment", new[] { "OrderId" });
            DropIndex("dbo.Assignment", new[] { "VehicleId" });
            DropIndex("dbo.AssignmentRoute", new[] { "AssignmentId" });
            DropTable("dbo.DriverAssignment");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.PianoQuote");
            DropTable("dbo.DriverLogin");
            DropTable("dbo.Company");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.AspNetRoleClaims");
            DropTable("dbo.AspNetClaims");
            DropTable("dbo.ClientPaymentCard");
            DropTable("dbo.VehicleType");
            DropTable("dbo.Vehicle");
            DropTable("dbo.TripStatus");
            DropTable("dbo.Warehouse");
            DropTable("dbo.PianoStatus");
            DropTable("dbo.Proof");
            DropTable("dbo.PianoPicture");
            DropTable("dbo.PianoType");
            DropTable("dbo.PianoSize");
            DropTable("dbo.PianoMake");
            DropTable("dbo.PianoFinish");
            DropTable("dbo.Piano");
            DropTable("dbo.PianoCharges");
            DropTable("dbo.OrderCharges");
            DropTable("dbo.ClientPayment");
            DropTable("dbo.ClientInvoice");
            DropTable("dbo.ClientStore");
            DropTable("dbo.Client");
            DropTable("dbo.OrderBilling");
            DropTable("dbo.Order");
            DropTable("dbo.Location");
            DropTable("dbo.Leg");
            DropTable("dbo.Driver");
            DropTable("dbo.Assignment");
            DropTable("dbo.AssignmentRoute");
            DropTable("dbo.APIRequestLog");
            DropTable("dbo.Address");
        }
    }
}
