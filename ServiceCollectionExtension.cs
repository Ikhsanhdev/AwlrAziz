using AwlrAziz.Interfaces;
using AwlrAziz.Repositories;
// using AwlrAziz.Services;

namespace AwlrAziz
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