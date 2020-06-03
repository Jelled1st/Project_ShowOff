using System.Linq;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

namespace Factory
{
    public class MaterialBoolChangeMachine : Machine
    {
        [BoxGroup("Shader settings")]
        [SerializeField]
        private string _shaderName = "Shader Graphs/shdr_potatoes";


        [BoxGroup("Shader settings")]
        [Header("See the debug inspector of shader")]
        [SerializeField]
        private string _propertyToDisable = "_enableDirty";

        [BoxGroup("Shader settings")]
        [SerializeField]
        private string _propertyToEnable = "_enableClean";


        protected override GameObject PreDelayProcess(GameObject o)
        {
            o.SetActive(false);
            return o;
        }

        protected override GameObject PostDelayProcess(GameObject o)
        {
            o.SetActive(true);
            var renderer = o.GetComponent<Renderer>();

            if (renderer != null)
            {
                foreach (var material in renderer.materials.Where(m => m.shader.name == _shaderName))
                {
                    material.SetFloat(_propertyToDisable, 0f);
                    material.SetFloat(_propertyToEnable, 1f);
                }
            }

            return o;
        }
    }
}