using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class test_foodCutting : MonoBehaviour
{
    [SerializeField] List<GameObject> _meshes;
    [SerializeField] GameObject _knife;
    GameObject _currentMesh;
    int _currentMeshIndex = -1;
    bool _cutting = false;

    // Start is called before the first frame update
    void Start()
    {
        LoadNextMeesh();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            _knife.transform.DORotate(new Vector3(0, 0, 0), 0.2f);
            _cutting = true;
        }
        else if(!DOTween.IsTweening(_knife.transform) && _cutting)
        {
            _cutting = false;
            LoadNextMeesh();
            _knife.transform.DORotate(new Vector3(-30, 0, 0), 0.2f);
        }
    }

    void LoadNextMeesh()
    {
        if(_currentMesh != null)
        {
            Destroy(_currentMesh);
        }
        _currentMesh = Instantiate(_meshes[++_currentMeshIndex]);
        _currentMesh.transform.SetParent(this.transform);

        if (_currentMeshIndex == 0)
        {
            _currentMesh.transform.localScale = new Vector3(3, 3, 3);
            _currentMesh.transform.localRotation = Quaternion.Euler(-90, 0, -90);
        }
        else
        {
            _currentMesh.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);
            _currentMesh.transform.localRotation = Quaternion.Euler(0, 90, 0);
        }
        _currentMesh.transform.localPosition = new Vector3(0, 0, 0);
    }
}
