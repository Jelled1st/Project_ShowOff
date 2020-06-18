using System.Collections.Generic;
using System.Linq;
using Factory;
using UnityEngine;
using UnityTemplateProjects;

[RequireComponent(typeof(SubstitutingMachine))]
public class PackerCustomAnimations : MonoBehaviour
{
    private FlatConveyorBelt _packerBelt;
    private List<Material> _packerMaterials;
    private List<float> _shaderInitialSpeeds;

    private void Awake()
    {
        _packerBelt = GetComponentInChildren<FlatConveyorBelt>();

        _packerMaterials =
            GetComponent<Renderer>()
                .materials
                .Where(m => m.shader.name.Equals(ShaderConstants.ScrollingShaderName))
                .ToList();

        _shaderInitialSpeeds =
            _packerMaterials
                .Select(m => m.GetFloat(ShaderConstants.ScrollingShaderSpeedFloat))
                .ToList();
    }

    private void SetPackerShaderSpeeds(float speed)
    {
        _packerMaterials.ForEach(m => m.SetFloat(ShaderConstants.ScrollingShaderSpeedFloat, speed));
    }

    private void ResetPackerShaderSpeeds()
    {
        for (var i = 0; i < _packerMaterials.Count; i++)
        {
            _packerMaterials[i].SetFloat(ShaderConstants.ScrollingShaderSpeedFloat, _shaderInitialSpeeds[i]);
        }
    }

    private void OnEnable()
    {
        Machine.MachineBroke += OnMachineBroke;
        Machine.MachineRepaired += OnMachineRepaired;
    }

    private void OnDisable()
    {
        Machine.MachineBroke -= OnMachineBroke;
        Machine.MachineRepaired -= OnMachineRepaired;
    }

    private void OnMachineRepaired(Machine obj)
    {
        if (obj.gameObject == gameObject)
        {
            ResetPackerShaderSpeeds();
            _packerBelt.enabled = true;
        }
    }

    private void OnMachineBroke(Machine obj)
    {
        if (obj.gameObject == gameObject)
        {
            SetPackerShaderSpeeds(0f);
            _packerBelt.enabled = false;
        }
    }
}