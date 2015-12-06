using CRUDOperations.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CRUDOperations.Controllers
{
    public class HomeController : Controller
    {
        CRUDDbContext context = new CRUDDbContext();
        public static List<CRUDViewModel> usrList = new List<CRUDViewModel>();
        public static List<Course> courses = new List<Course>();



        [HttpGet]
        [AllowAnonymous]
        public ActionResult Index()
        {
            usrList.Clear();
            IList<User> users = context.Users.ToList();

            foreach (var user in users)
            {
                var countryId = context.UserCountries.Where(u => u.UserID == user.ID).Select(c => c.CountryID).FirstOrDefault();
                var country = context.Countries.Where(c => c.ID == countryId).Select(n => n.Name).FirstOrDefault();
                var description = context.UserDescriptions.Where(i => i.UserID == user.ID).Select(d => d.Description).FirstOrDefault();
                var courseIds = context.UserCourses.Where(u => u.UserID == user.ID).Select(x => x.CourseID).ToList();

                List<Course> courseNames = new List<Course>();
                foreach (var crsId in courseIds)
                {
                    courseNames.Add(new Course { Name = context.Courses.Where(x => x.ID == crsId).Select(x => x.Name).FirstOrDefault() });
                }

                usrList.Add(new CRUDViewModel
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Gender = user.Gender,
                    Country = country,
                    UserID = user.ID,
                    Description = description,
                    Courses = courseNames
                });
            }
            return View(usrList);
        }


        [HttpGet]
        [AllowAnonymous]
        public ActionResult Create()
        {
            var model = new CRUDViewModel();
            CreateCourseList(model);
            return View(model);
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CRUDViewModel model)
        {
            User user = new User
            {
                ID = Guid.NewGuid().ToString(),
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Gender = model.Gender
            };
            context.Users.Add(user);

            var countryId = context.Countries.Where(m => m.Name == model.Country).Select(m => m.ID).FirstOrDefault();
            UserCountry userCountry = new UserCountry
            {
                UserCountryID = Guid.NewGuid().ToString(),
                UserID = user.ID,
                CountryID = countryId
            };
            context.UserCountries.Add(userCountry);


            UserDescription userDescription = new UserDescription
            {
                UserID = user.ID,
                Description = model.Description
            };
            context.UserDescriptions.Add(userDescription);


            foreach (var course in model.Courses)
            {

                if (course.Checked == true)
                {
                    UserCourse userCourse = new UserCourse
                    {
                        UserCourseID = Guid.NewGuid().ToString(),
                        UserID = user.ID,
                        CourseID = course.ID,
                        Checked = true
                    };
                    context.UserCourses.Add(userCourse);
                }
            }

            await context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");

        }


        public string GetUserProperty(string id, System.Linq.Expressions.Expression<Func<User, string>> selector)
        {
            return context.Users.Where(x => x.ID == id).Select(selector).FirstOrDefault();
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Edit(string id, CRUDViewModel model)
        {
            model.FirstName = GetUserProperty(id, x => x.FirstName);
            model.LastName = GetUserProperty(id, x => x.LastName);
            model.Email = GetUserProperty(id, x => x.Email);
            model.Gender = GetUserProperty(id, x => x.Gender);
            model.Description = context.UserDescriptions.Where(x => x.UserID == id).Select(x => x.Description).FirstOrDefault();
            var countryId = context.UserCountries.Where(x => x.UserID == id).Select(x => x.CountryID).FirstOrDefault();
            model.Country = context.Countries.Where(x => x.ID == countryId).Select(x => x.Name).FirstOrDefault();
            model.UserID = id;

            CreateCourseList(model);
            return View(model);
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CRUDViewModel model)
        {
            User user = context.Users.Where(u => u.ID == model.UserID).FirstOrDefault();
            TryUpdateModel(user, "", new string[] { "FirstName", "LastName", "Email", "Gender" });

            UserDescription userDescription = context.UserDescriptions.Where(u => u.UserID == model.UserID).FirstOrDefault();
            TryUpdateModel(userDescription, "", new string[] { "Description" });


            UserCountry userCountry = context.UserCountries.Where(u => u.UserID == model.UserID).FirstOrDefault();
            var countryId = context.Countries.Where(n => n.Name == model.Country).Select(i => i.ID).FirstOrDefault();
            if (userCountry.CountryID != countryId)
            {
                if (userCountry != null)
                {
                    context.UserCountries.Remove(userCountry);
                    context.SaveChanges();
                }
                UserCountry uCountry = new UserCountry
                {
                    UserCountryID = Guid.NewGuid().ToString(),
                    UserID = user.ID,
                    CountryID = countryId
                };
                context.UserCountries.Add(uCountry);
            }


            var userCourses = context.UserCourses.Where(u => u.UserID == model.UserID).ToList();
            List<string> uCourseIds = new List<string>();
            foreach (var crId in userCourses)
            {
                uCourseIds.Add(crId.CourseID);
            }
            var newCourses = model.Courses.Where(x => x.Checked == true).ToList();
            List<string> nCourseIds = new List<string>();
            foreach (var crId in newCourses)
            {
                nCourseIds.Add(crId.ID);
            }
            if (!uCourseIds.SequenceEqual(nCourseIds))
            {
                foreach (var crId in userCourses)
                {
                    UserCourse userCourse = context.UserCourses.Where(x => x.UserID == model.UserID && x.CourseID == crId.CourseID).FirstOrDefault();
                    context.UserCourses.Remove(userCourse);
                    context.SaveChanges();
                }
                foreach (var course in model.Courses)
                {
                    if (course.Checked == true)
                    {
                        UserCourse userCourse = new UserCourse
                        {
                            UserCourseID = Guid.NewGuid().ToString(),
                            UserID = user.ID,
                            CourseID = course.ID,
                            Checked = true
                        };
                        context.UserCourses.Add(userCourse);
                    }
                }
            }


            context.SaveChanges();

            return RedirectToAction("Index", "Home");
        }


        [AllowAnonymous]
        public ActionResult Delete(string id)
        {
            var userCourses = context.UserCourses.Where(x => x.UserID == id).ToList();
            foreach(var crs in userCourses) { context.UserCourses.Remove(crs); }

            var userCountry = context.UserCountries.Where(x => x.UserID == id).FirstOrDefault();
            context.UserCountries.Remove(userCountry);

            var userDescription = context.UserDescriptions.Where(x => x.UserID == id).FirstOrDefault();
            context.UserDescriptions.Remove(userDescription);

            var user = context.Users.Where(x => x.ID == id).FirstOrDefault();
            context.Users.Remove(user);

            context.SaveChanges();
            return RedirectToAction("index", "Home");
        }



        public void CreateCourseList(CRUDViewModel model)
        {
            courses.Clear();
            List<Course> usrCourses = context.Courses.ToList();
            foreach (var crs in usrCourses)
            {
                courses.Add(new Course { Name = crs.Name, ID = crs.ID, Checked = crs.Checked });
            }
            model.Courses = courses;
        }

        public static IEnumerable<SelectListItem> GetCountries()
        {
            RegionInfo country = new RegionInfo(new CultureInfo("en-US", false).LCID);
            List<SelectListItem> countryNames = new List<SelectListItem>();
            string cult = CultureInfo.CurrentCulture.EnglishName;
            string count = cult.Substring(cult.IndexOf('(') + 1,
                             cult.LastIndexOf(')') - cult.IndexOf('(') - 1);
            //To get the Country Names from the CultureInfo installed in windows
            foreach (CultureInfo cul in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
            {
                country = new RegionInfo(new CultureInfo(cul.Name, false).LCID);
                countryNames.Add(new SelectListItem()
                {
                    Text = country.DisplayName,
                    Value = country.DisplayName,
                    Selected = count == country.EnglishName
                });
            }
            //Assigning all Country names to IEnumerable
            IEnumerable<SelectListItem> nameAdded =
                countryNames.GroupBy(x => x.Text).Select(
                    x => x.FirstOrDefault()).ToList<SelectListItem>()
                    .OrderBy(x => x.Text);
            return nameAdded;
        }


    }
}