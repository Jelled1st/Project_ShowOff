using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using NaughtyAttributes;
using UnityTemplateProjects;

[SelectionBase]
public class FlatConveyorBelt : MonoBehaviour, IControllable
{
    public enum SpecialBeltType
    {
        SpeedUp,
        SpeedDown
    }

    // [ShowIf(nameof(_canRotate))]
    // [SerializeField]
    public const float RotateInterval = 0.35f;

    public static readonly Color GrayArrowColor = new Color32(60, 59, 59, 0);

    public static event Action ConveyorTurned = delegate { };
    public static event Action<SpecialBeltType, bool> SpecialConveyorHeld = delegate { };

    public static event Action BeltRotated = delegate { };
    public static event Action SpecialBeltPressed = delegate { };


    [Header("Conveyor settings")]
    [SerializeField]
    private float _speed = 1;

    [SerializeField]
    private bool _reverseShaderDirection;

    [SerializeField]
    private bool _runtimeUseInspectorSpeed;

    [SerializeField]
    private bool _isSpecialConveyor;

    [HideIf(nameof(_isSpecialConveyor))]
    [SerializeField]
    private bool _canRotate = true;

    [ShowIf(nameof(_isSpecialConveyor))]
    [SerializeField]
    private float _speedChangeTime = 0f;

    [Tooltip("Speed of belt when held")]
    [ShowIf(nameof(_isSpecialConveyor))]
    [SerializeField]
    private float _heldSpeed = 2f;

    [ShowIf(nameof(_isSpecialConveyor))]
    [SerializeField]
    private float _speedHoldTime = 3f;

    [ShowIf(nameof(_isSpecialConveyor))]
    [SerializeField]
    private float _colorChangeBeforeSpeedResetInterval = 5f;

    [ShowIf(nameof(_isSpecialConveyor))]
    [SerializeField]
    private SpecialBeltType _specialBeltType = SpecialBeltType.SpeedDown;

    public bool IsSpecialConveyor => _isSpecialConveyor;

    public SpecialBeltType BeltType => _specialBeltType;

    private readonly List<Material> _scrollingMaterials = new List<Material>();
    protected Rigidbody _rBody;
    protected Tween _rotateTween;
    private float _previousSpeed;
    private float _nonSerializedSpeed;
    private Color _specialBeltInitialColor;

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


    private void OnValidate()
    {
        if (_isSpecialConveyor)
        {
            switch (_specialBeltType)
            {
                case SpecialBeltType.SpeedUp:
                    if (_heldSpeed.Abs() <= _speed.Abs())
                    {
                        Debug.LogWarning("Speed-up belt's held speed is lower than initial speed!", this);
                    }

                    break;
                case SpecialBeltType.SpeedDown:
                    if (_heldSpeed.Abs() >= _speed.Abs())
                    {
                        Debug.LogWarning("Slow-down belt's held speed is higher than initial speed!", this);
                    }

                    break;
            }

            if (_heldSpeed > 0 && _speed < 0 || _heldSpeed < 0 && _speed > 0)
            {
                Debug.LogWarning("Special belt's held speed will reverse the belt's direction!");
            }
        }
    }

    protected virtual void Start()
    {
        if (!TryGetComponent(out _rBody))
        {
            _rBody = gameObject.AddComponent<Rigidbody>();
        }

        _rBody.useGravity = true;
        _rBody.isKinematic = true;

        _nonSerializedSpeed = _speed;

        LoadScrollingMaterials();

        if (_scrollingMaterials.Count > 0)
        {
            _specialBeltInitialColor =
                _scrollingMaterials.First().GetColor(ShaderConstants.ScrollingShaderArrowColorName);
        }
        else
        {
            Debug.LogWarning("No scrolling materials found!", this);
        }

        SetConveyorSpeed();
    }

    private void OnEnable()
    {
        Speed = _speed;
        SetConveyorSpeed();
    }

    private void OnDisable()
    {
        var speed = 1e-7f;
        if (_reverseShaderDirection)
            speed *= -1f;

        Speed = speed;
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
                    if (meshRenderer.materials[i].shader.name.Equals(ShaderConstants.ScrollingShaderName))
                    {
                        meshRenderer.materials[i] = new Material(meshRenderer.materials[i]);
                        _scrollingMaterials.Add(meshRenderer.materials[i]);
                    }
                }
            }
        }
    }

    private void SetConveyorSpeed()
    {
        var mul = _reverseShaderDirection ? -1 : 1;

        _scrollingMaterials.ForEach(t => t.SetFloat(ShaderConstants.ScrollingShaderSpeedFloat, Speed * mul));
    }

    private void FixedUpdate()
    {
        if (enabled)
            FixedUpdateMovement();
    }

    protected virtual void FixedUpdateMovement()
    {
        var pos = _rBody.position;
        _rBody.position += _rBody.transform.right * -Speed * Time.fixedDeltaTime;
        _rBody.MovePosition(pos);
    }

    private void TurnInternal()
    {
        if (_rotateTween == null || !_rotateTween.IsPlaying())
        {
            ConveyorTurned();

            Turn();
        }
    }

    protected virtual void Turn()
    {
        _rotateTween = gameObject.transform.DORotate(
            gameObject.transform.rotation.eulerAngles + new Vector3(0, 90, 0),
            RotateInterval);
        _rotateTween.onComplete += delegate { _rotateTween = null; };
    }

    private bool _isChangingBeltSpeed = false;

    private void ChangeSpecialBeltSpeed()
    {
        if (!_isChangingBeltSpeed && Speed != _heldSpeed)
        {
            SpecialConveyorHeld(_specialBeltType, true);

            _isChangingBeltSpeed = true;
            DOTween.Sequence()
                .AppendCallback(() =>
                {
                    Speed = _heldSpeed;
                    SetConveyorSpeed();
                })
                .Join(_scrollingMaterials.First().DOColor(GrayArrowColor, ShaderConstants.ScrollingShaderArrowColorName,
                    _speedChangeTime))
                .AppendCallback(ResetSpecialBeltSpeed);
        }
    }

    private void ResetSpecialBeltSpeed()
    {
        DOTween.Sequence()
            .AppendInterval(_speedHoldTime - _colorChangeBeforeSpeedResetInterval)
            .AppendInterval(_colorChangeBeforeSpeedResetInterval)
            .Join(_scrollingMaterials.First()
                .DOColor(_specialBeltInitialColor,
                    ShaderConstants.ScrollingShaderArrowColorName,
                    _colorChangeBeforeSpeedResetInterval))
            .Append(DOTween.To(() => Speed, x => Speed = x, _speed, _speedChangeTime))
            .AppendCallback(() =>
            {
                SpecialConveyorHeld(_specialBeltType, false);
                SetConveyorSpeed();
                _isChangingBeltSpeed = false;
            });
    }

    protected virtual void OnCollisionStay(Collision other)
    {
        var beltPosition = transform.position;
        var objectPosition = other.transform.position;
        var transformRight = transform.right;

        var a = beltPosition - transformRight * 2f;
        a.y = objectPosition.y;

        var b = beltPosition + transformRight * 4f;
        b.y = objectPosition.y;

        var c = objectPosition;

        var triangleHeightPoint = GeometryUtils.TriangleHeightPoint(ref a, ref b, ref c);

        // Debug.DrawLine(a, b);
        // Debug.DrawLine(b, c);
        // Debug.DrawLine(a, c);
        // Debug.DrawLine(objectPosition, triangleHeightPoint);

        if ((triangleHeightPoint - other.transform.position).sqrMagnitude > 0.05f)
            other.rigidbody.MovePosition(objectPosition +
                                         (triangleHeightPoint - objectPosition).normalized * Time.fixedDeltaTime *
                                         0.5f);
    }

    public void PerformSpecialBelt()
    {
        if (_isSpecialConveyor || !_isChangingBeltSpeed)
        {
            _isChangingBeltSpeed = true;
            DOTween.Sequence()
                .AppendCallback(() =>
                {
                    Speed = _heldSpeed;
                    SetConveyorSpeed();
                })
                .Join(_scrollingMaterials.First().DOColor(GrayArrowColor, ShaderConstants.ScrollingShaderArrowColorName,
                    _speedChangeTime))
                .AppendInterval(_speedHoldTime - _colorChangeBeforeSpeedResetInterval)
                .AppendInterval(_colorChangeBeforeSpeedResetInterval)
                .Join(_scrollingMaterials.First()
                    .DOColor(_specialBeltInitialColor,
                        ShaderConstants.ScrollingShaderArrowColorName,
                        _colorChangeBeforeSpeedResetInterval))
                .Append(DOTween.To(() => Speed, x => Speed = x, _speed, _speedChangeTime))
                .AppendCallback(() =>
                {
                    SetConveyorSpeed();
                    _isChangingBeltSpeed = false;
                });
        }
    }

    private void PerformSpecialBeltInternal()
    {
        if (_isSpecialConveyor)
        {
            SpecialBeltPressed();
            ChangeSpecialBeltSpeed();
        }
    }

    public virtual void OnClick(Vector3 hitPoint)
    {
    }

    public virtual void OnPress(Vector3 hitPoint)
    {
        if (!enabled)
            return;

        if (_canRotate && !_isSpecialConveyor)
        {
            BeltRotated?.Invoke();
            TurnInternal();
        }

        PerformSpecialBeltInternal();
    }

    public virtual void OnHold(float holdTime, Vector3 hitPoint)
    {
    }

    public virtual void OnHoldRelease(float timeHeld)
    {
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