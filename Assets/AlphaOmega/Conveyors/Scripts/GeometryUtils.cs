using UnityEngine;

namespace AlphaOmega.Conveyors
{
    public static class GeometryUtils
    {
        public static Vector3 TriangleHeightPoint(ref Vector3 linePointA, ref Vector3 linePointB,
            ref Vector3 heightPoint)
        {
            var lambda =
                (linePointA.x * linePointA.x + linePointB.x * heightPoint.x -
                 linePointA.x * (linePointB.x + heightPoint.x) + linePointA.y * linePointA.y -
                 linePointA.y * linePointB.y - linePointA.y * heightPoint.y +
                 linePointB.y * heightPoint.y +
                 linePointA.z * linePointA.z - linePointA.z * linePointB.z - linePointA.z * heightPoint.z +
                 linePointB.z * heightPoint.z) /
                (linePointA.x * linePointA.x - 2 * linePointA.x * linePointB.x + linePointB.x * linePointB.x +
                    linePointA.y * linePointA.y - 2 * linePointA.y * linePointB.y + linePointB.y * linePointB.y +
                    linePointA.z * linePointA.z -
                    2 * linePointA.z * linePointB.z + linePointB.z * linePointB.z);

            return new Vector3(linePointA.x + (linePointB.x - linePointA.x) * lambda,
                linePointA.y + (linePointB.y - linePointA.y) * lambda,
                linePointA.z + (linePointB.z - linePointA.z) * lambda);
        }
    }
}