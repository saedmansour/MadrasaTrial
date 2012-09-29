using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Data.Entity;
using Madrasa.Controllers;
using Madrasa.Filters;
using Madrasa.Models;

namespace Madrasa
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
          
        
        protected void Application_AcquireRequestState(object sender, EventArgs e)
        {
            var handler = Context.Handler as MvcHandler;
            if (handler == null) {
                return;
            }
            var routeData = handler.RequestContext.RouteData;
            var lang = Request.QueryString["lang"];

            if (lang == null)
            {
                lang = "ar";
            }

            CultureInfo ci = new CultureInfo(lang);
            System.Threading.Thread.CurrentThread.CurrentUICulture   = ci;
            //System.Threading.Thread.CurrentThread.CurrentCulture     = CultureInfo.CreateSpecificCulture(ci.Name);
         }


        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            
            //<author>Saed</author>
            filters.Add(new LogonAuthorize());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );
        }

        protected void Application_Start()
        {
            // Optimization: don't lookup other than razor views.
            // resource: http://encosia.com/a-harsh-reminder-about-the-importance-of-debug-false/
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine());

            //
            // TODO: change to "not delete tables".
            // TODO: delete in production.
            // This is to fix a bug in asp.net mvc/visual. It fixes that when a model changes, the tables are redone. Careful from this
            //in production, you might delete all your data.
            // http://stackoverflow.com/questions/3552000/entity-framework-code-only-error-the-model-backing-the-context-has-changed-sinc
            //
            //
            // On model change, reset databases.
            /*
            Database.SetInitializer<HomeworkDbContext>(new DropCreateDatabaseIfModelChanges<HomeworkDbContext>());
            Database.SetInitializer<SubjectDbContext>(new DropCreateDatabaseIfModelChanges<SubjectDbContext>());
            Database.SetInitializer<AdminDbContext>(new DropCreateDatabaseIfModelChanges<AdminDbContext>());
            Database.SetInitializer<HomeWorkQuestionDbContext>(new DropCreateDatabaseIfModelChanges<HomeWorkQuestionDbContext>());
            Database.SetInitializer<HomeworkSolvedDbContext>(new DropCreateDatabaseIfModelChanges<HomeworkSolvedDbContext>());
            Database.SetInitializer<MaterialDbContext>(new DropCreateDatabaseIfModelChanges<MaterialDbContext>());
            Database.SetInitializer<QuestionDbContext>(new DropCreateDatabaseIfModelChanges<QuestionDbContext>());
            Database.SetInitializer<RoleDbContext>(new DropCreateDatabaseIfModelChanges<RoleDbContext>());
            Database.SetInitializer<SchoolAdminDbContext>(new DropCreateDatabaseIfModelChanges<SchoolAdminDbContext>());
            Database.SetInitializer<SchoolDbContext>(new DropCreateDatabaseIfModelChanges<SchoolDbContext>());
            Database.SetInitializer<StudentDbContext>(new DropCreateDatabaseIfModelChanges<StudentDbContext>());
            Database.SetInitializer<TeacherDbContext>(new DropCreateDatabaseIfModelChanges<TeacherDbContext>());
            Database.SetInitializer<CourseDbContext>(new DropCreateDatabaseIfModelChanges<CourseDbContext>());
            Database.SetInitializer<CourseRegistrationDbContext>(new DropCreateDatabaseIfModelChanges<CourseRegistrationDbContext>());
            Database.SetInitializer<HomeworkStateDbContext>(new DropCreateDatabaseIfModelChanges<HomeworkStateDbContext>());
              */

            /*
            Database.SetInitializer<HomeworkDbContext>(new Devtalk.EF.CodeFirst.DontDropDbJustCreateTablesIfModelChanged<HomeworkDbContext>());
            Database.SetInitializer<SubjectDbContext>(new Devtalk.EF.CodeFirst.DontDropDbJustCreateTablesIfModelChanged<SubjectDbContext>());
            Database.SetInitializer<AdminDbContext>(new Devtalk.EF.CodeFirst.DontDropDbJustCreateTablesIfModelChanged<AdminDbContext>());
            Database.SetInitializer<HomeWorkQuestionDbContext>(new Devtalk.EF.CodeFirst.DontDropDbJustCreateTablesIfModelChanged<HomeWorkQuestionDbContext>());
            Database.SetInitializer<HomeworkSolvedDbContext>(new Devtalk.EF.CodeFirst.DontDropDbJustCreateTablesIfModelChanged<HomeworkSolvedDbContext>());
            Database.SetInitializer<MaterialDbContext>(new Devtalk.EF.CodeFirst.DontDropDbJustCreateTablesIfModelChanged<MaterialDbContext>());
            Database.SetInitializer<QuestionDbContext>(new Devtalk.EF.CodeFirst.DontDropDbJustCreateTablesIfModelChanged<QuestionDbContext>());
            Database.SetInitializer<RoleDbContext>(new Devtalk.EF.CodeFirst.DontDropDbJustCreateTablesIfModelChanged<RoleDbContext>());
            Database.SetInitializer<SchoolAdminDbContext>(new Devtalk.EF.CodeFirst.DontDropDbJustCreateTablesIfModelChanged<SchoolAdminDbContext>());
            Database.SetInitializer<SchoolDbContext>(new Devtalk.EF.CodeFirst.DontDropDbJustCreateTablesIfModelChanged<SchoolDbContext>());
            Database.SetInitializer<StudentDbContext>(new Devtalk.EF.CodeFirst.DontDropDbJustCreateTablesIfModelChanged<StudentDbContext>());
            Database.SetInitializer<TeacherDbContext>(new Devtalk.EF.CodeFirst.DontDropDbJustCreateTablesIfModelChanged<TeacherDbContext>());
            Database.SetInitializer<CourseDbContext>(new Devtalk.EF.CodeFirst.DontDropDbJustCreateTablesIfModelChanged<CourseDbContext>());
            Database.SetInitializer<CourseRegistrationDbContext>(new Devtalk.EF.CodeFirst.DontDropDbJustCreateTablesIfModelChanged<CourseRegistrationDbContext>());
            Database.SetInitializer<HomeworkStateDbContext>(new Devtalk.EF.CodeFirst.DontDropDbJustCreateTablesIfModelChanged<HomeworkStateDbContext>());
           */
            
            AreaRegistration.RegisterAllAreas();
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
        }
    }
}