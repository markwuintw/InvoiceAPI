namespace InvoiceAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FinalUpdateDb : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.InvAccounts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        InitDate = c.DateTime(),
                        Guid = c.String(nullable: false),
                        CompName = c.String(nullable: false, maxLength: 50),
                        UniformNumbers = c.String(nullable: false, maxLength: 10),
                        Password = c.String(nullable: false),
                        Salt = c.String(nullable: false),
                        MngrEmail = c.String(nullable: false, maxLength: 200),
                        MngrName = c.String(nullable: false, maxLength: 50),
                        MngrPhoneNumber = c.String(maxLength: 200),
                        CompAddress = c.String(nullable: false),
                        CompPhoneNumber = c.String(nullable: false),
                        verif = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.InvClientInfoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        InitDate = c.DateTime(),
                        AccountId = c.Int(nullable: false),
                        UniformNumbers = c.String(nullable: false, maxLength: 10),
                        CompName = c.String(nullable: false, maxLength: 50),
                        CompAddress = c.String(nullable: false),
                        CompPhoneNumber = c.String(nullable: false),
                        ContactName = c.String(),
                        ContactPhone = c.String(),
                        ContactEmail = c.String(),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.InvAccounts", t => t.AccountId, cascadeDelete: true)
                .Index(t => t.AccountId);
            
            CreateTable(
                "dbo.InvLetters",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        InitDate = c.DateTime(),
                        AccountId = c.Int(nullable: false),
                        Year = c.Int(nullable: false),
                        Period = c.Int(nullable: false),
                        StartMonth = c.Int(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        Letter = c.String(nullable: false),
                        StartNum = c.String(nullable: false),
                        EndNum = c.String(nullable: false),
                        InvLetterStatus = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.InvAccounts", t => t.AccountId, cascadeDelete: true)
                .Index(t => t.AccountId);
            
            CreateTable(
                "dbo.InvTables",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        InitDate = c.DateTime(),
                        AccountId = c.Int(nullable: false),
                        Letter = c.String(nullable: false),
                        Num = c.String(nullable: false),
                        InvNum = c.String(nullable: false),
                        InvDate = c.DateTime(),
                        ClientId = c.Int(nullable: false),
                        Client = c.String(nullable: false),
                        ClientUniformNum = c.String(nullable: false),
                        ClientAdress = c.String(nullable: false),
                        ClientPhoneNumber = c.String(nullable: false),
                        Total = c.Int(nullable: false),
                        InvStatus = c.Int(nullable: false),
                        DropTime = c.DateTime(),
                        DropReason = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.InvAccounts", t => t.AccountId, cascadeDelete: true)
                .Index(t => t.AccountId);
            
            CreateTable(
                "dbo.InvItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        InitDate = c.DateTime(),
                        InvTableId = c.Int(nullable: false),
                        Item = c.String(nullable: false),
                        Count = c.Int(nullable: false),
                        Price = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.InvTables", t => t.InvTableId, cascadeDelete: true)
                .Index(t => t.InvTableId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.InvItems", "InvTableId", "dbo.InvTables");
            DropForeignKey("dbo.InvTables", "AccountId", "dbo.InvAccounts");
            DropForeignKey("dbo.InvLetters", "AccountId", "dbo.InvAccounts");
            DropForeignKey("dbo.InvClientInfoes", "AccountId", "dbo.InvAccounts");
            DropIndex("dbo.InvItems", new[] { "InvTableId" });
            DropIndex("dbo.InvTables", new[] { "AccountId" });
            DropIndex("dbo.InvLetters", new[] { "AccountId" });
            DropIndex("dbo.InvClientInfoes", new[] { "AccountId" });
            DropTable("dbo.InvItems");
            DropTable("dbo.InvTables");
            DropTable("dbo.InvLetters");
            DropTable("dbo.InvClientInfoes");
            DropTable("dbo.InvAccounts");
        }
    }
}
