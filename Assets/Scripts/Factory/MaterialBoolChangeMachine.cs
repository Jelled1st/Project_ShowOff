using System.Linq;
using UnityEngine;

namespace Factory
{
    public class MaterialBoolChangeMachine : Machine
    {
        [SerializeField] private string _shaderName = "Shader Graphs/shdr_potatoes";

        [Header("See the debug inspector of shader")] [SerializeField]
        private string _propertyToDisable = "_enableDirty";

        [SerializeField] private string _propertyToEnable = "_enableClean";


        protected override GameObject PreDelayAction(GameObject o)
        {
            o.SetActive(false);
            return o;
        }

        protected override GameObject PostDelayAction(GameObject o)
        {
            o.SetActive(true);
            foreach (var material in o.GetComponent<Renderer>().materials.Where(m => m.shader.name == _shaderName))
            {
                material.SetFloat(_propertyToDisable, 0f);
                material.SetFloat(_propertyToEnable, 1f);
            }

            return o;
        }
    }
}