using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.Practices.Unity;
using SurveyDomain;
using SurveyDomain.Repositories;
using SurveyInterfaces;
using SurveyInterfaces.Repositories;

namespace SurveyWeb
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public class UnityDependencyResolver : IDependencyResolver
        {
            readonly IUnityContainer _container;
            public UnityDependencyResolver(IUnityContainer container)
            {
                _container = container;
            }

            public object GetService(Type serviceType)
            {
                try
                {
                    return _container.Resolve(serviceType);
                }
                catch
                {
                    return null;
                }
            }

            public IEnumerable<object> GetServices(Type serviceType)
            {
                try
                {
                    return _container.ResolveAll(serviceType);
                }
                catch
                {
                    return new List<object>();
                }
            }
        }


        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
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

        public class HttpContextLifetimeManager<T> : LifetimeManager, IDisposable
        {
            public override object GetValue()
            {
                return HttpContext.Current.Items[typeof(T).AssemblyQualifiedName];
            }
            public override void RemoveValue()
            {
                HttpContext.Current.Items.Remove(typeof(T).AssemblyQualifiedName);
            }
            public override void SetValue(object newValue)
            {
                HttpContext.Current.Items[typeof(T).AssemblyQualifiedName] = newValue;
            }
            public void Dispose()
            {
                RemoveValue();
            }
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            Database.SetInitializer<DBSurveyRepository>(null);
//            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<DBSurveyRepository>());
//            Database.SetInitializer(new DropCreateDatabaseAlways<DBSurveyRepository>());  //  DropCreateDatabaseIfModelChanges

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            var containter =
            RegisterRepositories().RegisterType<ISurveyService, SurveyService>()
                                                 .RegisterType<IRespondentService, RespondentService>()
                                                 .RegisterType<IUserService, UserService>()
                                                 .RegisterType<IUnitOfWork, DBSurveyRepository>()
                                                 .RegisterType<IExportService, ExportService>()
                                                 .RegisterType<IUniverService, UniverService>();

            DependencyResolver.SetResolver(new UnityDependencyResolver(containter));
        }

        private static IUnityContainer RegisterRepositories()
        {
            return new UnityContainer()
                .RegisterType<IRespondentRepository, RespondentRepository>()
                .RegisterType<ITagRepository, TagRepository>()
                .RegisterType<ISurveyQuestionRepository, SurveyQuestionRepository>()
                .RegisterType<ISubQuestionRepository, SubQuestionRepository>()
                .RegisterType<ISurveyProjectRepository, SurveyProjectRepository>()
                .RegisterType<IInterviewAnswerRepository, InterviewAnswerRepository>()
                .RegisterType<IInterviewRepository, InterviewRepository>()
                .RegisterType<ITagValueRepository, TagValueRepository>()
                .RegisterType<ISurveyInvitationRepository, SurveyInvitationRepository>()
                .RegisterType<IAnswerVariantRepository, AnswerVariantRepository>()
                .RegisterType<ITagValueLabelRepository, TagValueLabelRepository>()
                .RegisterType<ITicketRepository, TicketRepository>()
                .RegisterType<ICourseRepository, CourseRepository>()
                .RegisterType<IStudentRepository, StudentRepository>()
                .RegisterType<IStudentGroupRepository, StudentGroupRepository>();
        }
    }

    public static class RegisterTypeExts
    {
        public static IUnityContainer RegisterType<T1,T2>(this IUnityContainer containter) where T2 : T1
        {
            return containter
                .RegisterType<T1, T2>(new MvcApplication.HttpContextLifetimeManager<T2>());
        }
    }
}