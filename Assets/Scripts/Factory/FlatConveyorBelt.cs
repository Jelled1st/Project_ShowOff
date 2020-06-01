using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using NaughtyAttributes;

[SelectionBase]
public class FlatConveyorBelt : MonoBehaviour, IControllable
{
    [Header("Conveyor settings")] [SerializeField]
    private float _speed = 1;

    [SerializeField] private bool _reverseShaderDirection;
    [SerializeField] private bool _runtimeUseInspectorSpeed;
    [SerializeField] private bool _isSpecialConveyor;

    [ShowIf(nameof(_isSpecialConveyor))] [SerializeField]
    private bool _isTurnedOn = true;

    [ShowIf(nameof(_isSpecialConveyor))] [SerializeField]
    private float _speedChangeTime = 0.5f;

    [Tooltip("Speed of belt when held")] [ShowIf(nameof(_isSpecialConveyor))] [SerializeField]
    private float _heldSpeed = 2f;


    private static readonly int ScrollingSpeedShader = Shader.PropertyToID("_scrollingSpeed");

    private readonly List<Material> _scrollingMaterials = new List<Material>();
    protected Rigidbody _rBody;
    private Tween _rotateTween;
    private float _previousSpeed;
    private float _nonSerializedSpeed;

    public float Speed
    {
        get
        {
            if (_runtimeUseInspectorSpeed)
                return _speed;

            return _nonSerializedSpeed;
        }

        set
        {
            if (_runtimeUseInspectorSpeed)
                Debug.LogWarning("Belt Speed is using inspector values. Setting the Speed value won't have effect!");

            _nonSerializedSpeed = value;
        }
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        if (!TryGetComponent<Rigidbody>(out _rBody))
        {
            _rBody = this.gameObject.AddComponent<Rigidbody>();
        }

        _rBody.useGravity = true;
        _rBody.isKinematic = true;

        _nonSerializedSpeed = _speed;

        LoadScrollingMaterials();
        SetConveyorSpeed();
    }

    private void LoadScrollingMaterials()
    {
        var renderers = GetComponents<MeshRenderer>().ToList();
        renderers.AddRange(GetComponentsInChildren<MeshRenderer>());

        if (renderers.Count != 0)
        {
            foreach (var meshRenderer in renderers)
            {
                for (var i = 0; i < meshRenderer.materials.Length; i++)
                {
                    if (meshRenderer.materials[i].shader.name.Equals("Shader Graphs/shdr_textureScroll"))
                    {
                        meshRenderer.materials[i] = new Material(meshRenderer.materials[i]);
                        _scrollingMaterials.Add(meshRenderer.materials[i]);
                    }
                }
            }
        }
    }

    private void Update()
    {
        // SetConveyorSpeed();
    }

    private void SetConveyorSpeed()
    {
        if (_scrollingMaterials.Count == 0 || _previousSpeed == Speed)
            return;

        var mul = 1;
        if (_reverseShaderDirection)
            mul *= -1;

        _scrollingMaterials.ForEach(t => t.SetFloat(ScrollingSpeedShader, Speed * mul));

        _previousSpeed = Speed;
    }

    // Update is called once per frame
    protected virtual void FixedUpdate()
    {
        if (_isSpecialConveyor)
            if (!_isTurnedOn)
                return;

        Vector3 pos = _rBody.position;
        _rBody.position += _rBody.transform.right * -Speed * Time.fixedDeltaTime;
        _rBody.MovePosition(pos);
    }

    public virtual void Turn()
    {
        if (_rotateTween == null || !_rotateTween.IsPlaying())
        {
            _rotateTween = this.gameObject.transform.DORotate(
                this.gameObject.transform.rotation.eulerAngles + new Vector3(0, 90, 0),
                0.2f);
        }
    }

    private void ChangeSpecialBeltSpeed()
    {
        if (Speed != _heldSpeed)
            DOTween.Sequence().Append(DOTween.To(() => Speed, x => Speed = x, _heldSpeed, _speedChangeTime))
                .AppendCallback(SetConveyorSpeed);
    }

    private void ResetSpecialBeltSpeed()
    {
        DOTween.Sequence().Append(DOTween.To(() => Speed, x => Speed = x, _speed, _speedChangeTime))
            .AppendCallback(SetConveyorSpeed);
    }

    public virtual void OnClick(Vector3 hitPoint)
    {
    }

    public virtual void OnPress(Vector3 hitPoint)
    {
        if (!_isSpecialConveyor)
            Turn();
    }

    public virtual void OnHold(float holdTime, Vector3 hitPoint)
    {
        if (_isSpecialConveyor)
            ChangeSpecialBeltSpeed();
    }

    public virtual void OnHoldRelease(float timeHeld)
    {
        if (_isSpecialConveyor)
            ResetSpecialBeltSpeed();
    }

    public void OnSwipe(Vector3 direction, Vector3 lastPosition)
    {
    }

    public void OnDrag(Vector3 position)
    {
    }

    public void OnDragDrop(Vector3 position, IControllable droppedOn, ControllerHitInfo hitInfo)
    {
    }

    public void OnDragDropFailed(Vector3 position)
    {
    }

    public void OnDrop(IControllable dropped, ControllerHitInfo hitInfo)
    {
    }

    public GameObject GetDragCopy()
    {
        // GameObject copy = Instantiate(this.gameObject);
        // Destroy(copy.GetComponent<FlatConveyorBelt>());
        // Destroy(copy.GetComponent<BoxCollider>());
        // return copy;
        return null;
    }
}