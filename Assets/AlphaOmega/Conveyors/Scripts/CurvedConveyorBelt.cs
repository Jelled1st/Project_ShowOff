using UnityEngine;

namespace AlphaOmega.Conveyors
{
    public class CurvedConveyorBelt : ConveyorBelt
    {
        private const float EyeCandySpeedMultiplier = 0.7f;

        protected override void FixedUpdate()
        {
            var rot = Rigidbody.rotation;
            Rigidbody.rotation *= Quaternion.Euler(0, Speed * EyeCandySpeedMultiplier, 0);
            Rigidbody.MoveRotation(rot);
        }
    }
}