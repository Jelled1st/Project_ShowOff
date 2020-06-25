using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CuttableFoodWithDropOff : CuttableFood
{
    [SerializeField]
    private List<CuttableFallOffOptions> _fallOffOptionForState;

    private List<GameObject> _fallOffPieces = new List<GameObject>();

    [SerializeField]
    private GameObject _dishMesh;

    private new void Awake()
    {
        base.Awake();
    }

    // Start is called before the first frame update
    private new void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    private new void Update()
    {
        base.Update();
    }

    public override bool Cut()
    {
        if (base.Cut())
        {
            var current = _currentState.transform;
            var fallOffIndex = _currentStateIndex - 1;
            if (_fallOffOptionForState.Count < _currentStateIndex) fallOffIndex = _fallOffOptionForState.Count - 1;
            var fallOff = Instantiate(_fallOffOptionForState[fallOffIndex].GetFallOffPiece(),
                cuttingBoard.GetCutPosition(), Quaternion.identity);
            fallOff.gameObject.transform.SetParent(gameObject.transform);
            fallOff.gameObject.transform.rotation = current.rotation;
            fallOff.gameObject.transform.position -=
                new Vector3(_fallOffOptionForState[fallOffIndex].GetFallOffPieceLength(), 0, 0);
            fallOff.gameObject.transform.localScale = current.localScale;
            var rb = fallOff.AddComponent<Rigidbody>();
            for (var i = 0; i < _fallOffPieces.Count; ++i)
            {
                _fallOffPieces[i].gameObject.transform
                    .DOMove(
                        _fallOffPieces[i].transform.position -
                        new Vector3(_fallOffOptionForState[fallOffIndex].GetFallOffPieceLength() / 2, 0, 0), 0.1f);
            }

            fallOff.AddComponent<BoxCollider>();
            _fallOffPieces.Add(fallOff);
            return true;
        }
        else
        {
            return false;
        }
    }

    public override GameObject GetDishMesh()
    {
        if (ReadyForDish())
        {
            var copy = Instantiate(_dishMesh);
            Destroy(copy.GetComponent<CuttableFood>());
            var renderers = copy.GetComponentsInChildren<Renderer>();
            for (var i = 0; i < renderers.Length; ++i)
            {
                renderers[i].enabled = true;
            }

            var colliders = copy.GetComponentsInChildren<Collider>();
            var rbs = copy.GetComponentsInChildren<Rigidbody>();
            for (var i = 0; i < rbs.Length; ++i)
            {
                Destroy(rbs[i]);
            }

            for (var i = 0; i < colliders.Length; ++i)
            {
                Destroy(colliders[i]);
            }

            return copy;
        }

        return base.GetDishMesh();
    }

    public override GameObject GetDragCopy()
    {
        var copy = base.GetDragCopy();
        if (copy == null) return null;
        return copy;
    }
}