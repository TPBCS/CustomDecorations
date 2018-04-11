using UnityEngine;

namespace CustomDecorations
{
    public class CliffLoader : MonoBehaviour
    {
        private bool loaded;

        private DecorationType type = DecorationType.Cliff;

        private CustomDecorationsManager Instance => CustomDecorationsManager.instance;

        private Mesh[] MeshList => Instance.CliffMeshes;

        private Texture2D[] TextureList => Instance.CliffTextures;

        private DecorationInfo[] DecorationTarget => Instance.CliffDecorations;

        public void Awake()
        {
            Instance.Prepare(type);
        }

        private void Load(DecorationType type)
        {
            for (int i = 0; i < DecorationTarget.Length; i++)
            {
                try
                {
                    DecorationTarget[i].m_mesh = MeshList[i];
                    DecorationTarget[i].m_renderMaterial.SetTexture("_MainTex", TextureList[i]);
                }
                catch (System.Exception x)
                {
                    Debug.LogError($"{x.Message} - {x.StackTrace}");
                }                
            }
        }

        private void Update()
        {
            while (!loaded)
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
            Instance.CliffMeshes = null;
            Instance.CliffTextures = null;
            Destroy(this);
        }
    }

    public class FertileLoader : MonoBehaviour
    {
        private bool loaded;

        private DecorationType type = DecorationType.Fertile;

        private CustomDecorationsManager Instance => CustomDecorationsManager.instance;

        private Mesh[] MeshList => Instance.FertileMeshes;

        private Texture2D[] TextureList => Instance.FertileTextures;

        private DecorationInfo[] DecorationTarget => Instance.FertileDecorations;

        public void Awake()
        {
            Instance.Prepare(type);
        }

        private void Load(DecorationType type)
        {
            for (int i = 0; i < DecorationTarget.Length; i++)
            {
                try
                {
                    DecorationTarget[i].m_mesh = MeshList[i];
                    DecorationTarget[i].m_renderMaterial.SetTexture("_MainTex", TextureList[i]);
                }
                catch (System.Exception x)
                {
                    Debug.LogError($"{x.Message} - {x.StackTrace}");
                }
            }
        }

        private void Update()
        {
            while (!loaded)
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
            Instance.FertileMeshes = null;
            Instance.FertileTextures = null;
            Destroy(this);
        }
    }

    public class GrassLoader : MonoBehaviour
    {
        private bool loaded;

        private DecorationType type = DecorationType.Grass;

        private CustomDecorationsManager Instance => CustomDecorationsManager.instance;

        private Mesh[] MeshList => Instance.GrassMeshes;

        private Texture2D[] TextureList => Instance.GrassTextures;

        private DecorationInfo[] DecorationTarget => Instance.GrassDecorations;

        public void Awake()
        {
            Instance.Prepare(type);
        }

        private void Load(DecorationType type)
        {
            for (int i = 0; i < DecorationTarget.Length; i++)
            {
                try
                {
                    DecorationTarget[i].m_mesh = MeshList[i];
                    DecorationTarget[i].m_renderMaterial.SetTexture("_MainTex", TextureList[i]);
                }
                catch (System.Exception x)
                {
                    Debug.LogError($"{x.Message} - {x.StackTrace}");
                }
            }
        }

        private void Update()
        {
            while (!loaded)
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
            Instance.GrassMeshes = null;
            Instance.GrassTextures = null;
            Destroy(this);
        }
    }
}
