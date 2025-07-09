using AwlrAziz.Interfaces;

namespace AwlrAziz.Repositories
{
    public class UnitOfWorkRepository : IUnitOfWorkRepository
    {
        public UnitOfWorkRepository(
            IDeviceRepository deviceRepository
        )
        {
            Devices = deviceRepository;
        }

        public IDeviceRepository Devices { get; }
    }
}