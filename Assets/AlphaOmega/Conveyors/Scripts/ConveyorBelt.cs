using System;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace AlphaOmega.Conveyors
{
    [RequireComponent(typeof(Rigidbody))]
    public abstract class ConveyorBelt : MonoBehaviour
    {
        public static event Action ConveyorTurned;

        private const string ScrollingShaderName = "Shader Graphs/shdr_textureScroll";
        private static readonly int ScrollingShaderSpeedFloat = Shader.PropertyToID("_scrollingSpeed");

        [Header("Conveyor settings")]
        [SerializeField]
        private float speed = 1;

        [SerializeField]
        private bool reverseShaderDirection;

        [Header("Rotation settings")]
        [SerializeField]
        private bool canRotate = true;

        [SerializeField]
        private Vector3 rotateAngle = new Vector3(0f, 90f, 0f);

        [SerializeField]
        private float rotateInterval = 0.35f;

        protected virtual Transform RotateTarget => transform;

        protected Rigidbody Rigidbody;
        private float? _preDisableSpeed;
        private Material _scrollingMaterial;
        protected Sequence RotateTween;

        public float Speed
        {
            get => speed;
            set
            {
                speed = value;
                SetConveyorSpeed();
            }
        }

        protected abstract void FixedUpdate();

        private void Awake()
        {
            Rigidbody = GetComponent<Rigidbody>();

            Rigidbody.useGravity = true;
            Rigidbody.isKinematic = true;

            LoadScrollingMaterials();

            SetConveyorSpeed();
        }

        private void OnEnable()
        {
            if (_preDisableSpeed.HasValue)
            {
                Speed = _preDisableSpeed.Value;
                _preDisableSpeed = null;
            }
        }

        private void OnDisable()
        {
            _preDisableSpeed = Speed;

            var almostZeroSpeedInCurrentDirection = 1e-7f;
            if (reverseShaderDirection)
                almostZeroSpeedInCurrentDirection *= -1f;

            Speed = almostZeroSpeedInCurrentDirection;
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
                        if (meshRenderer.materials[i].shader.name.Equals(ScrollingShaderName))
                        {
                            meshRenderer.materials[i] = new Material(meshRenderer.materials[i]);
                            _scrollingMaterial = meshRenderer.materials[i];
                            return;
                        }
                    }
                }
            }
        }

        private void SetConveyorSpeed()
        {
            var mul = reverseShaderDirection ? -1 : 1;

            _scrollingMaterial.SetFloat(ScrollingShaderSpeedFloat, Speed * mul);
        }

        public void TryTurn()
        {
            if (canRotate && RotateTween == null)
            {
                ConveyorTurned?.Invoke();
                Turn();
            }
        }

        protected virtual void Turn()
        {
            RotateTween = DOTween.Sequence()
                .Append(RotateTarget.DORotate(RotateTarget.rotation.eulerAngles + rotateAngle, rotateInterval))
                .OnComplete(delegate { RotateTween = null; });
        }


        private void OnMouseDown()
        {
            if (!enabled)
                return;

            TryTurn();
        }
    }
}