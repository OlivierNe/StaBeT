[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(StageBeheersTool.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(StageBeheersTool.App_Start.NinjectWebCommon), "Stop")]

namespace StageBeheersTool.App_Start
{
    using System;
    using System.Web;

    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using Ninject;
    using Ninject.Web.Common;
    using StageBeheersTool.Models.Domain;
    using StageBeheersTool.Models.DAL;
    using StageBeheersTool.Models.Services;
    using Microsoft.AspNet.Identity;

    public static class NinjectWebCommon
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start()
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }

        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }

        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

                RegisterServices(kernel);
                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            kernel.Bind<IBedrijfRepository>().To<BedrijfRepository>().InRequestScope();
            kernel.Bind<IStageopdrachtRepository>().To<StageopdrachtRepository>().InRequestScope();
            kernel.Bind<ISpecialisatieRepository>().To<SpecialisatieRepository>().InRequestScope();
            kernel.Bind<IKeuzepakketRepository>().To<KeuzepakketRepository>().InRequestScope();
            kernel.Bind<IStudentRepository>().To<StudentRepository>().InRequestScope();
            kernel.Bind<IBegeleiderRepository>().To<BegeleiderRepository>().InRequestScope();
            kernel.Bind<IUserService>().To<UserService>().InRequestScope();
            kernel.Bind<IContactpersoonRepository>().To<ContactpersoonRepository>().InRequestScope();
            kernel.Bind<IImageService>().To<ImageService>().InRequestScope();
            kernel.Bind<IAcademiejaarRepository>().To<AcademiejaarRepository>().InRequestScope();
            kernel.Bind<ISpreadsheetService>().To<SpreadsheetService>().InRequestScope();
            kernel.Bind<StageToolDbContext>().ToSelf().InRequestScope();
        }
    }
}
