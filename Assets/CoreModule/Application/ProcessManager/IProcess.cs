#pragma warning disable IDE0130

namespace Texell.CoreModule.ProcessManager
{
    public interface IProcess
    {
        void OnStart();

        void OnUpdate();

        void OnExit();
    }
}