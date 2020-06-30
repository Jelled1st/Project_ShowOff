using UnityEngine;

namespace AlphaOmega.Conveyors
{
    [SelectionBase]
    public class FlatConveyorBelt : ConveyorBelt
    {
        [Header("Attract objects to the middle")]
        [SerializeField]
        private bool enableAttracting;

        [SerializeField]
        private float attractionForce = 0.5f;

        [Tooltip("[Adjust carefully!] Distance to the middle line of the belt when to stop applying attracting force")]
        [SerializeField]
        private float attractionDisableDistance = 0.02f;

        protected override void FixedUpdate()
        {
            var pos = Rigidbody.position;

            Rigidbody.position += RotateTarget.right * -Speed * Time.fixedDeltaTime;
            Rigidbody.MovePosition(pos);
        }

        protected virtual void OnCollisionStay(Collision other)
        {
            if (enableAttracting)
            {
                var beltPosition = RotateTarget.position;
                var objectPosition = other.transform.position;
                var transformRight = RotateTarget.right;

                var a = beltPosition - transformRight;
                a.y = objectPosition.y;

                var b = beltPosition + transformRight;
                b.y = objectPosition.y;

                var c = objectPosition;


                var triangleHeightPoint = GeometryUtils.TriangleHeightPoint(ref a, ref b, ref c);


                Debug.DrawLine(a, b);
                Debug.DrawLine(b, c);
                Debug.DrawLine(a, c);
                Debug.DrawLine(objectPosition, triangleHeightPoint);

                if ((triangleHeightPoint - other.transform.position).sqrMagnitude > attractionDisableDistance)
                {
                    other.rigidbody.MovePosition(objectPosition +
                                                 (triangleHeightPoint - objectPosition).normalized *
                                                 Time.fixedDeltaTime *
                                                 attractionForce);
                }
            }
        }
    }
}