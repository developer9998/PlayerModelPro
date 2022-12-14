using ComputerInterface.Interfaces;
using Zenject;

namespace PlayerModelPro.Scripts.ComputerInterface
{
    internal class MainInstaller : Installer
    {
        public override void InstallBindings()
        {
            // Bind your mod entry like this
            Container.Bind<IComputerModEntry>().To<PlayerModelEntry>().AsSingle();
        }
    }
}
