using UnityEngine;

namespace UnityTemplateProjects
{
    public static class ShaderConstants
    {
        public const string PotatoShader = "Shader Graphs/shdr_potatoes";
        public const string ScrollingShaderName = "Shader Graphs/shdr_textureScroll";
        public static readonly int ScrollingShaderSpeedFloat = Shader.PropertyToID("_scrollingSpeed");
    }
}