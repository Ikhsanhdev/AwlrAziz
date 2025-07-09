using AwlrAziz.Interfaces;
using AwlrAziz.Jobs;
using AwlrAziz.Repositories;
// using AwlrAziz.Services;

namespace WebTelemetry
{
    public static class ServiceCollectionExtension
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            #region ========== [ Register Unit Of Works ] ==========
            services.AddScoped<IUnitOfWorkRepository, UnitOfWorkRepository>();
            #endregion

            #region ========== [ Register Services ] ==========
            
            #endregion

            #region ========== [ Register Repositories ] ==========
            services.AddScoped<IDeviceRepository, DeviceRepository>();
            #endregion

            // #region ========== [ Register Jobs ] ==========
            // services.AddScoped<IExistingReadingJob, ExistingReadingJob>();
            // #endregion
        }
    }
}