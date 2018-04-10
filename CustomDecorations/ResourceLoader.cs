using UnityEngine;

namespace CustomDecorations
{
    class ResourceLoader : MonoBehaviour
    {
        private bool loaded;

        private DecorationType type;

        private CustomDecorationsManager Instance => CustomDecorationsManager.instance;

        private Mesh[] MeshList => type == DecorationType.Cliff ? Instance.CliffMeshes : type == DecorationType.Fertile ? Instance.FertileMeshes : Instance.GrassMeshes;

        private Texture2D[] TextureList => type == DecorationType.Cliff ? Instance.CliffTextures : type == DecorationType.Fertile ? Instance.FertileTextures : Instance.GrassTextures;

        private DecorationInfo[] DecorationTarget => type == DecorationType.Cliff ? Instance.CliffDecorations : type == DecorationType.Fertile ? Instance.FertileDecorations : Instance.GrassDecorations;

        public ResourceLoader(DecorationType decoType)
        {
            type = decoType;
            Instance.Prepare(type);
        }

        private void Load(DecorationType type)
        {
            for (int i = 0; i < DecorationTarget.Length; i++)
            {
                DecorationTarget[i].m_mesh = MeshList[i];
                DecorationTarget[i].m_renderMaterial.SetTexture("_MainTex", TextureList[i]);
            }
        }

        private void Update()
        {
            while (!loaded)
            {
                if (MeshList != null && TextureList != null)
                {
                    Load(type);

                    var done = false;

                    for (int i = 0; i < DecorationTarget.Length; i++)
                    {
                        if (DecorationTarget[i].m_mesh == MeshList[i]) done = true;
                        else done = false;
                    }

                    loaded = done;
                }
            }
            switch (type)
            {
                case DecorationType.Grass:
                    Instance.GrassMeshes = null;
                    Instance.GrassTextures = null;
                    break;

                case DecorationType.Fertile:
                    Instance.FertileMeshes = null;
                    Instance.FertileTextures = null;
                    break;
                default:
                    Instance.CliffMeshes = null;
                    Instance.CliffTextures = null;
                    break;
            }
            Destroy(this);
        }
    }
}
