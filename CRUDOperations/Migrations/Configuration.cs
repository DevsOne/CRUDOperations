namespace CRUDOperations.Migrations
{
    using Controllers;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<CRUDOperations.Models.CRUDDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(CRUDOperations.Models.CRUDDbContext context)
        {
            int num = 0;
            foreach (var country in HomeController.GetCountries())
            {
                num++;
                context.Countries.AddOrUpdate(

                new Models.Country { Name = country.Text, ID = num.ToString() });
            }

            context.Courses.AddOrUpdate(new Models.Course { Name = "Course 1", ID = "1", Checked = false });
            context.Courses.AddOrUpdate(new Models.Course { Name = "Course 2", ID = "2", Checked = false });
            context.Courses.AddOrUpdate(new Models.Course { Name = "Course 3", ID = "3", Checked = false });
            context.Courses.AddOrUpdate(new Models.Course { Name = "Course 4", ID = "4", Checked = false });
            context.Courses.AddOrUpdate(new Models.Course { Name = "Course 5", ID = "5", Checked = false });

        }
    }
}
