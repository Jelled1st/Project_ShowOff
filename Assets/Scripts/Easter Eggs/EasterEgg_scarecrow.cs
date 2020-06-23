using UnityEngine;

public class EasterEgg_scarecrow : MonoBehaviour
{
    [SerializeField] private int PlayCount;
    private int _playCount;
    private Animator _anim;
    private bool _isPlaying = false;

    void Start()
    {
        _anim = gameObject.GetComponent<Animator>();
        _playCount = PlayCount;
    }
    private void OnMouseDown()
    {
        //play animation 1 if no other animation is playing
        if(_playCount > 0 && _isPlaying == false)
        {
            _anim.SetTrigger("isPreAnim");
            print("playing pre");
        }
        //play animation 2 if no other animation is playing
        if(_playCount <= 0 && _isPlaying == false)
        {
            _anim.SetTrigger("isFinalAnim");
            print("playing final");
            _playCount = PlayCount +1;
        }
        //don't allow counting _playCount while animation is playing
        if (!_isPlaying)
        {
            _playCount--;
        }
        print(_playCount);
    }

    private void animationListener(string message)
    {
        //set _isPlaying to true at the start of animation and false at the end
        if(message == "start")
        {
            _isPlaying = true;
        }
        if(message == "end")
        {
            _isPlaying = false;
        }
    }
}
