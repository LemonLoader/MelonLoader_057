using ModHelper;

namespace MonkiiLoader
{
    internal class MuseDashModWrapper : MonkiiMod
    {
        internal IMod modInstance;
        public override void OnInitializeMonkii() => modInstance.DoPatching();
    }
}