using AwlrAziz.Repositories;

namespace AwlrAziz.Interfaces
{
    public interface IUnitOfWorkRepository
    {
        IDeviceRepository Devices { get; }
    }
}