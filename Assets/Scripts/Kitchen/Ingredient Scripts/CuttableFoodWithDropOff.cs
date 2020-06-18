using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CuttableFoodWithDropOff : CuttableFood
{
    [SerializeField] List<CuttableFallOffOptions> _fallOffOptionForState;
    List<GameObject> _fallOffPieces = new List<GameObject>();

    new void Awake()
    {
        base.Awake();
    }

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
    }

    public override bool Cut()
    {
        if(base.Cut())
        {
            Transform current = _currentState.transform;
            int fallOffIndex = _currentStateIndex - 1;
            if (_fallOffOptionForState.Count < _currentStateIndex) fallOffIndex = _fallOffOptionForState.Count-1;
            GameObject fallOff = GameObject.Instantiate(_fallOffOptionForState[fallOffIndex].GetFallOffPiece(), cuttingBoard.GetCutPosition(), Quaternion.identity);
            fallOff.gameObject.transform.SetParent(this.gameObject.transform);
            fallOff.gameObject.transform.rotation = current.rotation;
            fallOff.gameObject.transform.position -= new Vector3(_fallOffOptionForState[fallOffIndex].GetFallOffPieceLength(), 0, 0);
            Rigidbody rb = fallOff.AddComponent<Rigidbody>();
            for(int i = 0; i < _fallOffPieces.Count; ++i)
            {
                _fallOffPieces[i].gameObject.transform.DOMove(_fallOffPieces[i].transform.position - new Vector3(_fallOffOptionForState[fallOffIndex].GetFallOffPieceLength()/2, 0, 0), 0.1f);
            }
            fallOff.AddComponent<BoxCollider>();
            _fallOffPieces.Add(fallOff);
            return true;
        }
        else return false;
    }

    public override GameObject GetDragCopy()
    {
        GameObject copy = base.GetDragCopy();
        if (copy == null) return null;
        return copy;
    }
}
