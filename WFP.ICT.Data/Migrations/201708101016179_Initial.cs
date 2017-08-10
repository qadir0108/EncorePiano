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
                        Address1 = c.String(),
                        City = c.String(),
                        State = c.String(),
                        PostCode = c.String(),
                        PhoneNumber = c.String(),
                        Notes = c.String(),
                        NumberTurns = c.Int(nullable: false),
                        NumberStairs = c.Int(nullable: false),
                        AlternateContact = c.String(),
                        AlternatePhone = c.String(),
                        Lat = c.String(),
                        Lng = c.String(),
                        AddressType = c.Int(nullable: false),
                        CustomerId = c.Guid(),
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
                        CustomerInvoiceId = c.Guid(),
                        CustomerPaymentId = c.Guid(),
                        CustomerType = c.Int(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        Addresses_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Address", t => t.Addresses_Id)
                .Index(t => t.Addresses_Id);
            
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
                        SentOn = c.DateTime(),
                        Amount = c.Long(nullable: false),
                        Status = c.Int(nullable: false),
                        Notes = c.String(),
                        GeneratedAt = c.DateTime(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        CustomerId = c.Guid(),
                        CreatedAt = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        Client_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Client", t => t.Client_Id)
                .Index(t => t.Client_Id);
            
            CreateTable(
                "dbo.PianoOrder",
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
                        PreferredPickupDateTime = c.DateTime(),
                        PickupDate = c.DateTime(),
                        DeliveryDate = c.DateTime(),
                        PickupAddressId = c.Guid(),
                        DeliveryAddressId = c.Guid(),
                        CustomerId = c.Guid(),
                        PianoOrderBillingId = c.Guid(),
                        PianoAssignmentId = c.Guid(),
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
                        ClientInvoice_Id = c.Guid(),
                        Client_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PianoOrderBilling", t => t.PianoOrderBillingId)
                .ForeignKey("dbo.Client", t => t.CustomerId)
                .ForeignKey("dbo.Address", t => t.DeliveryAddressId)
                .ForeignKey("dbo.Client", t => t.InvoiceBillingPartyId)
                .ForeignKey("dbo.Client", t => t.InvoiceClientId)
                .ForeignKey("dbo.Address", t => t.PickupAddressId)
                .ForeignKey("dbo.ClientInvoice", t => t.ClientInvoice_Id)
                .ForeignKey("dbo.Client", t => t.Client_Id)
                .Index(t => t.PickupAddressId)
                .Index(t => t.DeliveryAddressId)
                .Index(t => t.CustomerId)
                .Index(t => t.PianoOrderBillingId)
                .Index(t => t.InvoiceClientId)
                .Index(t => t.InvoiceBillingPartyId)
                .Index(t => t.ClientInvoice_Id)
                .Index(t => t.Client_Id);
            
            CreateTable(
                "dbo.PianoOrderBilling",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        PaymentOption = c.Int(nullable: false),
                        BillingType = c.Int(nullable: false),
                        Amount = c.Long(nullable: false),
                        PianoOrderId = c.Guid(),
                        CreatedAt = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PianoOrderCharges",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Amount = c.Long(nullable: false),
                        ServiceStatus = c.Int(nullable: false),
                        PianoChargesId = c.Guid(),
                        PianoOrderId = c.Guid(),
                        CreatedAt = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PianoCharges", t => t.PianoChargesId)
                .ForeignKey("dbo.PianoOrder", t => t.PianoOrderId)
                .Index(t => t.PianoChargesId)
                .Index(t => t.PianoOrderId);
            
            CreateTable(
                "dbo.PianoCharges",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ChargesCode = c.Int(nullable: false),
                        ChargesType = c.Int(nullable: false),
                        ChargesDetails = c.String(),
                        Amount = c.Long(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PianoAssignment",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        AssignmentNumber = c.String(),
                        WarehouseStartId = c.Guid(),
                        VehicleId = c.Guid(),
                        PianoOrderId = c.Guid(),
                        PianoPodId = c.Guid(),
                        PickupTicket = c.String(),
                        PickupTicketGenerationTime = c.DateTime(),
                        EstimatedTime = c.Long(nullable: false),
                        isStarted = c.Boolean(nullable: false),
                        StartTime = c.DateTime(),
                        MinutesAway = c.Int(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Vehicle", t => t.VehicleId)
                .ForeignKey("dbo.Warehouse", t => t.WarehouseStartId)
                .ForeignKey("dbo.PianoOrder", t => t.Id)
                .Index(t => t.Id)
                .Index(t => t.WarehouseStartId)
                .Index(t => t.VehicleId);
            
            CreateTable(
                "dbo.Driver",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Code = c.String(),
                        Name = c.String(),
                        Description = c.String(),
                        Password = c.String(),
                        DefaultVehicleID = c.Guid(),
                        FCMToken = c.String(),
                        CreatedAt = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PianoPOD",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ReceivedBy = c.String(),
                        Signature = c.String(),
                        ReceivingTime = c.DateTime(),
                        PodStatus = c.Int(nullable: false),
                        Notes = c.String(),
                        PodForm = c.String(),
                        SignedOn = c.DateTime(),
                        AcknowledgeTo = c.String(),
                        PianoAssignmentId = c.Guid(),
                        PianoId = c.Guid(),
                        CreatedAt = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Piano", t => t.PianoId)
                .ForeignKey("dbo.PianoAssignment", t => t.Id)
                .Index(t => t.Id)
                .Index(t => t.PianoId);
            
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
                        WareHouseStatus = c.Int(nullable: false),
                        IsLocated = c.Boolean(nullable: false),
                        PianoStatusId = c.Guid(),
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
                .ForeignKey("dbo.PianoOrder", t => t.OrderId)
                .ForeignKey("dbo.PianoFinish", t => t.PianoFinishId)
                .ForeignKey("dbo.PianoMake", t => t.PianoMakeId)
                .ForeignKey("dbo.PianoSize", t => t.PianoSizeId)
                .ForeignKey("dbo.PianoStatus", t => t.PianoStatusId)
                .ForeignKey("dbo.PianoType", t => t.PianoTypeId)
                .ForeignKey("dbo.Warehouse", t => t.WarehouseId)
                .ForeignKey("dbo.PianoQuote", t => t.PianoQuote_Id)
                .Index(t => t.PianoTypeId)
                .Index(t => t.PianoMakeId)
                .Index(t => t.PianoSizeId)
                .Index(t => t.PianoFinishId)
                .Index(t => t.PianoStatusId)
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
                "dbo.PianoStatus",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Status = c.Int(nullable: false),
                        StatusTime = c.DateTime(),
                        StatusBy = c.String(),
                        Comments = c.String(),
                        DrawingOfPiano = c.String(),
                        PianoId = c.Guid(),
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
                        PianoPodId = c.Guid(),
                        CreatedAt = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Piano", t => t.PianoId)
                .ForeignKey("dbo.PianoPOD", t => t.PianoPodId)
                .Index(t => t.PianoId)
                .Index(t => t.PianoPodId);
            
            CreateTable(
                "dbo.Warehouse",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        Code = c.String(),
                        AddressId = c.Guid(),
                        CreatedAt = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Address", t => t.AddressId)
                .Index(t => t.AddressId);
            
            CreateTable(
                "dbo.PianoAssignmentRoute",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Lat = c.String(),
                        Lng = c.String(),
                        Order = c.Int(nullable: false),
                        PianoAssignmentId = c.Guid(),
                        CreatedAt = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PianoAssignment", t => t.PianoAssignmentId)
                .Index(t => t.PianoAssignmentId);
            
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
                "dbo.ClientPayment",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        PaymentType = c.Int(nullable: false),
                        CheckNumber = c.String(),
                        PaymentDate = c.DateTime(nullable: false),
                        Amount = c.Long(nullable: false),
                        CustomerId = c.Guid(),
                        CreatedAt = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        Client_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Client", t => t.Client_Id)
                .Index(t => t.Client_Id);
            
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
                        CustomerId = c.Guid(),
                        PianoOrderBillingId = c.Guid(),
                        CreatedAt = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        PianoType_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PianoOrderBilling", t => t.PianoOrderBillingId)
                .ForeignKey("dbo.Client", t => t.CustomerId)
                .ForeignKey("dbo.Address", t => t.DeliveryAddressId)
                .ForeignKey("dbo.PianoType", t => t.PianoType_Id)
                .ForeignKey("dbo.Address", t => t.PickupAddressId)
                .Index(t => t.PickupAddressId)
                .Index(t => t.DeliveryAddressId)
                .Index(t => t.CustomerId)
                .Index(t => t.PianoOrderBillingId)
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
                        CustomerId = c.Guid(),
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
                .ForeignKey("dbo.Client", t => t.CustomerId)
                .Index(t => t.CustomerId)
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
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUsers", "CustomerId", "dbo.Client");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.PianoQuote", "PickupAddressId", "dbo.Address");
            DropForeignKey("dbo.PianoQuote", "PianoType_Id", "dbo.PianoType");
            DropForeignKey("dbo.Piano", "PianoQuote_Id", "dbo.PianoQuote");
            DropForeignKey("dbo.PianoQuote", "DeliveryAddressId", "dbo.Address");
            DropForeignKey("dbo.PianoQuote", "CustomerId", "dbo.Client");
            DropForeignKey("dbo.PianoQuote", "PianoOrderBillingId", "dbo.PianoOrderBilling");
            DropForeignKey("dbo.ClientPayment", "Client_Id", "dbo.Client");
            DropForeignKey("dbo.PianoOrder", "Client_Id", "dbo.Client");
            DropForeignKey("dbo.ClientInvoice", "Client_Id", "dbo.Client");
            DropForeignKey("dbo.PianoOrder", "ClientInvoice_Id", "dbo.ClientInvoice");
            DropForeignKey("dbo.PianoOrder", "PickupAddressId", "dbo.Address");
            DropForeignKey("dbo.PianoAssignment", "Id", "dbo.PianoOrder");
            DropForeignKey("dbo.PianoAssignment", "WarehouseStartId", "dbo.Warehouse");
            DropForeignKey("dbo.PianoAssignment", "VehicleId", "dbo.Vehicle");
            DropForeignKey("dbo.Vehicle", "VehicleTypeId", "dbo.VehicleType");
            DropForeignKey("dbo.PianoAssignmentRoute", "PianoAssignmentId", "dbo.PianoAssignment");
            DropForeignKey("dbo.PianoPOD", "Id", "dbo.PianoAssignment");
            DropForeignKey("dbo.PianoPOD", "PianoId", "dbo.Piano");
            DropForeignKey("dbo.Piano", "WarehouseId", "dbo.Warehouse");
            DropForeignKey("dbo.Warehouse", "AddressId", "dbo.Address");
            DropForeignKey("dbo.PianoPicture", "PianoPodId", "dbo.PianoPOD");
            DropForeignKey("dbo.PianoPicture", "PianoId", "dbo.Piano");
            DropForeignKey("dbo.Piano", "PianoTypeId", "dbo.PianoType");
            DropForeignKey("dbo.Piano", "PianoStatusId", "dbo.PianoStatus");
            DropForeignKey("dbo.Piano", "PianoSizeId", "dbo.PianoSize");
            DropForeignKey("dbo.PianoSize", "PianoTypeId", "dbo.PianoType");
            DropForeignKey("dbo.Piano", "PianoMakeId", "dbo.PianoMake");
            DropForeignKey("dbo.Piano", "PianoFinishId", "dbo.PianoFinish");
            DropForeignKey("dbo.Piano", "OrderId", "dbo.PianoOrder");
            DropForeignKey("dbo.Piano", "ClientId", "dbo.Client");
            DropForeignKey("dbo.DriverPianoAssignment", "PianoAssignment_Id", "dbo.PianoAssignment");
            DropForeignKey("dbo.DriverPianoAssignment", "Driver_Id", "dbo.Driver");
            DropForeignKey("dbo.PianoOrderCharges", "PianoOrderId", "dbo.PianoOrder");
            DropForeignKey("dbo.PianoOrderCharges", "PianoChargesId", "dbo.PianoCharges");
            DropForeignKey("dbo.PianoOrder", "InvoiceClientId", "dbo.Client");
            DropForeignKey("dbo.PianoOrder", "InvoiceBillingPartyId", "dbo.Client");
            DropForeignKey("dbo.PianoOrder", "DeliveryAddressId", "dbo.Address");
            DropForeignKey("dbo.PianoOrder", "CustomerId", "dbo.Client");
            DropForeignKey("dbo.PianoOrder", "PianoOrderBillingId", "dbo.PianoOrderBilling");
            DropForeignKey("dbo.ClientStore", "Client_Id", "dbo.Client");
            DropForeignKey("dbo.ClientStore", "AddressId", "dbo.Address");
            DropForeignKey("dbo.Client", "Addresses_Id", "dbo.Address");
            DropForeignKey("dbo.AspNetRoleClaims", "RoleID", "dbo.AspNetRoles");
            DropForeignKey("dbo.AspNetRoleClaims", "ClaimID", "dbo.AspNetClaims");
            DropIndex("dbo.DriverPianoAssignment", new[] { "PianoAssignment_Id" });
            DropIndex("dbo.DriverPianoAssignment", new[] { "Driver_Id" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUsers", new[] { "CustomerId" });
            DropIndex("dbo.PianoQuote", new[] { "PianoType_Id" });
            DropIndex("dbo.PianoQuote", new[] { "PianoOrderBillingId" });
            DropIndex("dbo.PianoQuote", new[] { "CustomerId" });
            DropIndex("dbo.PianoQuote", new[] { "DeliveryAddressId" });
            DropIndex("dbo.PianoQuote", new[] { "PickupAddressId" });
            DropIndex("dbo.ClientPayment", new[] { "Client_Id" });
            DropIndex("dbo.Vehicle", new[] { "VehicleTypeId" });
            DropIndex("dbo.PianoAssignmentRoute", new[] { "PianoAssignmentId" });
            DropIndex("dbo.Warehouse", new[] { "AddressId" });
            DropIndex("dbo.PianoPicture", new[] { "PianoPodId" });
            DropIndex("dbo.PianoPicture", new[] { "PianoId" });
            DropIndex("dbo.PianoSize", new[] { "PianoTypeId" });
            DropIndex("dbo.Piano", new[] { "PianoQuote_Id" });
            DropIndex("dbo.Piano", new[] { "OrderId" });
            DropIndex("dbo.Piano", new[] { "ClientId" });
            DropIndex("dbo.Piano", new[] { "WarehouseId" });
            DropIndex("dbo.Piano", new[] { "PianoStatusId" });
            DropIndex("dbo.Piano", new[] { "PianoFinishId" });
            DropIndex("dbo.Piano", new[] { "PianoSizeId" });
            DropIndex("dbo.Piano", new[] { "PianoMakeId" });
            DropIndex("dbo.Piano", new[] { "PianoTypeId" });
            DropIndex("dbo.PianoPOD", new[] { "PianoId" });
            DropIndex("dbo.PianoPOD", new[] { "Id" });
            DropIndex("dbo.PianoAssignment", new[] { "VehicleId" });
            DropIndex("dbo.PianoAssignment", new[] { "WarehouseStartId" });
            DropIndex("dbo.PianoAssignment", new[] { "Id" });
            DropIndex("dbo.PianoOrderCharges", new[] { "PianoOrderId" });
            DropIndex("dbo.PianoOrderCharges", new[] { "PianoChargesId" });
            DropIndex("dbo.PianoOrder", new[] { "Client_Id" });
            DropIndex("dbo.PianoOrder", new[] { "ClientInvoice_Id" });
            DropIndex("dbo.PianoOrder", new[] { "InvoiceBillingPartyId" });
            DropIndex("dbo.PianoOrder", new[] { "InvoiceClientId" });
            DropIndex("dbo.PianoOrder", new[] { "PianoOrderBillingId" });
            DropIndex("dbo.PianoOrder", new[] { "CustomerId" });
            DropIndex("dbo.PianoOrder", new[] { "DeliveryAddressId" });
            DropIndex("dbo.PianoOrder", new[] { "PickupAddressId" });
            DropIndex("dbo.ClientInvoice", new[] { "Client_Id" });
            DropIndex("dbo.ClientStore", new[] { "Client_Id" });
            DropIndex("dbo.ClientStore", new[] { "AddressId" });
            DropIndex("dbo.Client", new[] { "Addresses_Id" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.AspNetRoleClaims", new[] { "RoleID" });
            DropIndex("dbo.AspNetRoleClaims", new[] { "ClaimID" });
            DropTable("dbo.DriverPianoAssignment");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.PianoQuote");
            DropTable("dbo.DriverLogin");
            DropTable("dbo.Company");
            DropTable("dbo.ClientPayment");
            DropTable("dbo.VehicleType");
            DropTable("dbo.Vehicle");
            DropTable("dbo.PianoAssignmentRoute");
            DropTable("dbo.Warehouse");
            DropTable("dbo.PianoPicture");
            DropTable("dbo.PianoStatus");
            DropTable("dbo.PianoType");
            DropTable("dbo.PianoSize");
            DropTable("dbo.PianoMake");
            DropTable("dbo.PianoFinish");
            DropTable("dbo.Piano");
            DropTable("dbo.PianoPOD");
            DropTable("dbo.Driver");
            DropTable("dbo.PianoAssignment");
            DropTable("dbo.PianoCharges");
            DropTable("dbo.PianoOrderCharges");
            DropTable("dbo.PianoOrderBilling");
            DropTable("dbo.PianoOrder");
            DropTable("dbo.ClientInvoice");
            DropTable("dbo.ClientStore");
            DropTable("dbo.Client");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.AspNetRoleClaims");
            DropTable("dbo.AspNetClaims");
            DropTable("dbo.APIRequestLog");
            DropTable("dbo.Address");
        }
    }
}
