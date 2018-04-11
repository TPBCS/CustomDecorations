using UnityEngine;

namespace CustomDecorations
{
    public class ResourceLoader : MonoBehaviour
    {
        internal bool loaded;

        internal DecorationType type;

        internal CustomDecorationsManager Instance => CustomDecorationsManager.instance;

        internal Mesh[] MeshList => type == DecorationType.Cliff ? Instance.CliffMeshes : type == DecorationType.Fertile ? Instance.FertileMeshes : Instance.GrassMeshes;

        internal Texture2D[] TextureList => type == DecorationType.Cliff ? Instance.CliffTextures : type == DecorationType.Fertile ? Instance.FertileTextures : Instance.GrassTextures;

        internal DecorationInfo[] DecorationTarget => type == DecorationType.Cliff ? Instance.CliffDecorations : type == DecorationType.Fertile ? Instance.FertileDecorations : Instance.GrassDecorations;

        internal void Awake()
        {
            Instance.Prepare(type);
        }

        internal void Load()
        {
            for (int i = 0; i < DecorationTarget.Length; i++)
            {
                try
                {
                    DecorationTarget[i].m_mesh = MeshList[i];
                    DecorationTarget[i].m_renderMaterial.SetTexture("_MainTex", TextureList[i]);
                }
                catch (System.Exception)
                {

                }
            }
        }

        internal void Update()
        {
            while (!loaded)
            {
                Load();

                var done = false;

                for (int i = 0; i < DecorationTarget.Length; i++)
                {
                    if (DecorationTarget[i].m_mesh == MeshList[i]) done = true;
                    else done = false;
                }

                loaded = done;
            }
            Destroy(this);
        }
    }

    public class CliffLoader : ResourceLoader
    {
        internal new DecorationType type = DecorationType.Cliff;
    }

    public class FertileLoader : ResourceLoader
    {
        internal new DecorationType type = DecorationType.Fertile;
    }

    public class GrassLoader : ResourceLoader
    {
        internal new DecorationType type = DecorationType.Grass;
    }
}
