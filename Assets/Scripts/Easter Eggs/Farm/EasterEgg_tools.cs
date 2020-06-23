using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EasterEgg_tools : MonoBehaviour
{
    [SerializeField] private Rigidbody _tool1;
    [SerializeField] private Rigidbody _tool2;
    [SerializeField] private Rigidbody _tool3;
    [SerializeField] private GameObject _beam;
    [SerializeField] private int _playCount;
    private bool _isOpen = false;
    private bool _hasClicked = false;
    void Start()
    {
        _tool1.constraints = RigidbodyConstraints.FreezeAll;
        _tool2.constraints = RigidbodyConstraints.FreezeAll;
        _tool3.constraints = RigidbodyConstraints.FreezeAll;
    }
    private void Update()
    {
        if (!_hasClicked && _isOpen && _playCount < 1)
        {
            //disable frozen
            _tool1.constraints = RigidbodyConstraints.None;
            _tool2.constraints = RigidbodyConstraints.None;
            _tool3.constraints = RigidbodyConstraints.None;

            //disable gravity on tools
            _tool1.useGravity = false;
            _tool2.useGravity = false;
            _tool3.useGravity = false;

            //add force upwards
            _tool1.AddForce(0, Random.Range(100f, 150f), 0);
            _tool2.AddForce(0, Random.Range(100f, 150f), 0);
            _tool3.AddForce(0, Random.Range(100f, 150f), 0);

            //add angular velocity
            _tool1.AddRelativeTorque(Random.Range(-3f, 3f), Random.Range(-3f, 3f), Random.Range(-3f, 3f));
            _tool2.AddRelativeTorque(Random.Range(-3f, 3f), Random.Range(-3f, 3f), Random.Range(-3f, 3f));
            _tool3.AddRelativeTorque(Random.Range(-3f, 3f), Random.Range(-3f, 3f), Random.Range(-3f, 3f));
            _isOpen = false;
            _hasClicked = true;
        }
    }

    private void OnMouseDown()
    {
        //activate beam
        if (!_beam.activeSelf)
        {
            _beam.SetActive(true);
        }
        //start tools coroutine
        if (!_hasClicked && _playCount < 1)
        {
            StartCoroutine(beam());
        }
        _playCount--;
    }

    private IEnumerator beam()
    {
        yield return null;

        while(_beam.transform.localScale.x < 2.45f && !_isOpen)
        {
            _beam.GetComponent<Transform>().DOScale(new Vector3(2.5f, 5, 2.5f), 1);
            if(_beam.transform.localScale.x > 1.5f)
            {
                _isOpen = true;
                yield return new WaitForSeconds(4);
                _beam.GetComponent<Transform>().DOScale(new Vector3(0, 5, 0), 0.5f);
                yield break;
            }
            yield return null;
        }
    }
}
