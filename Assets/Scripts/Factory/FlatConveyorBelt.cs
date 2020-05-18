using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class FlatConveyorBelt : MonoBehaviour, IControllable
{
    [Header("Conveyor settings")] [SerializeField]
    private float _speed = 1;

    [SerializeField] private bool _reverseShaderDirection;
    [SerializeField] private bool _runtimeUseInspectorSpeed;


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
            if (_speed != _nonSerializedSpeed)
                _nonSerializedSpeed = _speed;

            if (_runtimeUseInspectorSpeed)
                return _speed;

            return _nonSerializedSpeed;
        }

        set
        {
            if (_runtimeUseInspectorSpeed)
                Debug.LogWarning("Belt speed is using inspector values. Setting the speed value won't have effect!");

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

        LoadScrollingMaterials();
        SetConveyorSpeed(Speed);
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
        SetConveyorSpeed(Speed);
    }

    private void SetConveyorSpeed(float speed)
    {
        if (_reverseShaderDirection)
            speed *= -1;

        if (_scrollingMaterials.Count == 0)
            return;

        if (_previousSpeed == speed)
            return;

        _scrollingMaterials.ForEach(t => t.SetFloat(ScrollingSpeedShader, speed));

        _previousSpeed = speed;
    }

    // Update is called once per frame
    protected virtual void FixedUpdate()
    {
        Vector3 pos = _rBody.position;
        _rBody.position += _rBody.transform.right * -_speed * Time.fixedDeltaTime;
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

    public virtual void OnClick(Vector3 hitPoint)
    {
    }

    public virtual void OnPress(Vector3 hitPoint)
    {
        Turn();
    }

    public void OnHold(float holdTime, Vector3 hitPoint)
    {
    }

    public void OnHoldRelease(float timeHeld)
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