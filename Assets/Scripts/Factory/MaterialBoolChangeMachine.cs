using System.Linq;
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


        protected override GameObject PreDelayProcess(GameObject inputGameObject)
        {
            inputGameObject.SetActive(true);
            var inputRenderer = inputGameObject.GetComponent<Renderer>();

            if (inputRenderer != null)
            {
                foreach (var material in inputRenderer.materials.Where(m => m.shader.name == _shaderName))
                {
                    material.SetFloat(_propertyToDisable, 0f);
                    material.SetFloat(_propertyToEnable, 1f);
                }
            }

            return inputGameObject;
        }

        protected override GameObject PostDelayProcess(GameObject outputGameObject)
        {
            return outputGameObject;
        }
    }
}