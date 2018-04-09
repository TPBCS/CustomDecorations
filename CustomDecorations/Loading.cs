using ICities;
namespace CustomDecorations
{
    public class LoadingExtension : LoadingExtensionBase
    {
        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);
            CustomDecorationsManager.instance.OnLevelLoaded();
        }

        public override void OnLevelUnloading()
        {
            base.OnLevelUnloading();
            CustomDecorationsManager.instance.OnLevelUnloading();
        }
    }
}
