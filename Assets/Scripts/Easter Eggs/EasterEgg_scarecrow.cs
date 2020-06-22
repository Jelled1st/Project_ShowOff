using UnityEngine;

public class EasterEgg_scarecrow : MonoBehaviour
{
    [SerializeField] private int PlayCount;
    private int _playCount;
    private Animator _anim;

    void Start()
    {
        _anim = gameObject.GetComponent<Animator>();
        _playCount = PlayCount;
    }

    private void OnMouseDown()
    {
        if(_playCount > 0)
        {
            _anim.SetTrigger("isPreAnim");
        }
        if(_playCount <= 0)
        {
            _anim.SetTrigger("isFinalAnim");
            _playCount = PlayCount +1;
        }
        _playCount--;
    }
}
