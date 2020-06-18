using UnityEngine;

public class Rotatesky : MonoBehaviour
{
    public float rotateSpeed = 1.2f;

    void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * rotateSpeed);
    }
}
