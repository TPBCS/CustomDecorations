using ICities;
namespace CustomDecorations
{
    public class LoadingExtension : LoadingExtensionBase
    {
        public override void OnLevelLoaded(LoadMode mode)
        {            
            base.OnLevelLoaded(mode);
            var loaded = false;
            while (!loaded)
            {
                if (LoadingManager.instance.m_loadingComplete)
                {
                    loaded = true;
                    CustomDecorationsManager.instance.OnLevelLoaded();
                }
            }
        }

        public override void OnLevelUnloading()
        {
            base.OnLevelUnloading();
            CustomDecorationsManager.instance.OnLevelUnloading();
        }
    }
}
