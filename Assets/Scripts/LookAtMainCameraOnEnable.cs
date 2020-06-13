using UnityEngine;

public class LookAtMainCameraOnEnable : MonoBehaviour
{
    private void OnEnable()
    {
        if (Camera.main != null)
        {
            transform.LookAt(transform.position + Camera.main.transform.forward);
        }
        else
        {
            Debug.LogError("Can't find the Main Camera!");
        }
    }
}