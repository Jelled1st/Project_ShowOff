using UnityEngine;

public class RigidbodyLocker : MonoBehaviour
{
    //Required to have a checkbox in inspector
    private void OnEnable()
    {
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!enabled)
            return;

        other.gameObject.TryGetComponent(out Rigidbody otherRigidbody);

        if (!otherRigidbody.Equals(null))
        {
            otherRigidbody.freezeRotation = true;
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (!enabled)
            return;

        other.gameObject.TryGetComponent(out Rigidbody otherRigidbody);

        if (!otherRigidbody.Equals(null))
        {
            otherRigidbody.freezeRotation = false;
        }
    }
}