using IoT.Simulator.Settings.DPS;

using System.Threading.Tasks;

namespace IoT.Simulator.Services
{
    interface IProvisioningService
    {
        Task<string> ProvisionDevice();
    }
}
