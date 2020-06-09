using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityTemplateProjects;

[RequireComponent(typeof(ObjectSpawner))]
public class ObjectSpawnerPostProcess : MonoBehaviour
{
    private void OnEnable()
    {
        GetComponent<ObjectSpawner>().ObjectSpawned += PostProcessObject;
    }

    private void OnDisable()
    {
        GetComponent<ObjectSpawner>().ObjectSpawned -= PostProcessObject;
    }

    private void PostProcessObject(GameObject item)
    {
        var potatoMaterial = item.GetComponent<Renderer>().materials
            .First(m => m.shader.name == ShaderConstants.PotatoShader);

        potatoMaterial?.SetFloat("_enableDirty", 0f);
        potatoMaterial?.SetFloat("_enableClean", 0f);
        potatoMaterial?.SetFloat("_enablePeeled", 1f);
    }
}