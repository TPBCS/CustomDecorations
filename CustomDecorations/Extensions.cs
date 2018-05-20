using UnityEngine;

namespace CustomDecorations
{
    public static class DecorationInfoExtension
    {
        public static void LoadMesh(this DecorationInfo info, Mesh mesh)
        {
            if (mesh == null) return;
            info.m_mesh = mesh;
        }

        public static Mesh GetMesh(this DecorationInfo info)
        {
            return info.m_mesh;
        }

        public static void LoadTexture(this DecorationInfo info, Texture2D texture)
        {
            if (texture == null) return;
            info?.m_renderMaterial?.SetTexture("_MainTex", texture);
        }

        public static Texture2D GetTexture(this DecorationInfo info)
        {
            return info.m_renderMaterial?.GetTexture("_MainTex") as Texture2D;
        }
    }

    public static class DecorationRendererExtension
    {
        public static void SetResolution(this DecorationRenderer renderer, int resolution)
        {
            renderer.m_renderDiffuseTexture = new RenderTexture(resolution, resolution, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear)
            {
                filterMode = FilterMode.Trilinear
            };
            renderer.m_renderXycaTexture = new RenderTexture(resolution, resolution, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear)
            {
                filterMode = FilterMode.Trilinear
            };
        }
    }    
}