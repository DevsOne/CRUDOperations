namespace CRUDOperations.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CRUD : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Country",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.UserCountry",
                c => new
                    {
                        UserCountryID = c.String(nullable: false, maxLength: 128),
                        UserID = c.String(nullable: false, maxLength: 128),
                        CountryID = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.UserCountryID)
                .ForeignKey("dbo.Country", t => t.CountryID, cascadeDelete: true)
                .ForeignKey("dbo.User", t => t.UserID, cascadeDelete: true)
                .Index(t => t.UserID)
                .Index(t => t.CountryID);
            
            CreateTable(
                "dbo.User",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 128),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Gender = c.String(),
                        Email = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.UserCourse",
                c => new
                    {
                        UserCourseID = c.String(nullable: false, maxLength: 128),
                        UserID = c.String(nullable: false, maxLength: 128),
                        CourseID = c.String(nullable: false, maxLength: 128),
                        Checked = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.UserCourseID)
                .ForeignKey("dbo.Course", t => t.CourseID, cascadeDelete: true)
                .ForeignKey("dbo.User", t => t.UserID, cascadeDelete: true)
                .Index(t => t.UserID)
                .Index(t => t.CourseID);
            
            CreateTable(
                "dbo.Course",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                        Checked = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.UserDescription",
                c => new
                    {
                        UserID = c.String(nullable: false, maxLength: 128),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.UserID)
                .ForeignKey("dbo.User", t => t.UserID)
                .Index(t => t.UserID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserDescription", "UserID", "dbo.User");
            DropForeignKey("dbo.UserCourse", "UserID", "dbo.User");
            DropForeignKey("dbo.UserCourse", "CourseID", "dbo.Course");
            DropForeignKey("dbo.UserCountry", "UserID", "dbo.User");
            DropForeignKey("dbo.UserCountry", "CountryID", "dbo.Country");
            DropIndex("dbo.UserDescription", new[] { "UserID" });
            DropIndex("dbo.UserCourse", new[] { "CourseID" });
            DropIndex("dbo.UserCourse", new[] { "UserID" });
            DropIndex("dbo.UserCountry", new[] { "CountryID" });
            DropIndex("dbo.UserCountry", new[] { "UserID" });
            DropTable("dbo.UserDescription");
            DropTable("dbo.Course");
            DropTable("dbo.UserCourse");
            DropTable("dbo.User");
            DropTable("dbo.UserCountry");
            DropTable("dbo.Country");
        }
    }
}
