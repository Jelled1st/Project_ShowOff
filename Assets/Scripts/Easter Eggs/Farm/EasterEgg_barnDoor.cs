using UnityEngine;

public class EasterEgg_barnDoor : MonoBehaviour
{
    private Animator _anim;

    private void Start()
    {
        _anim = gameObject.GetComponent<Animator>();
    }

    private void OnMouseDown()
    {
        if (_anim.GetBool("isClosed"))
        {
            _anim.SetBool("isClosed", false);
        }
        else
        {
            _anim.SetBool("isClosed", true);
        }
    }
}