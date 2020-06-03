using UnityEngine;

public static class GeometryUtils
{
    public static Vector3 TriangleHeightPoint(Vector3 linePointA, Vector3 linePointB, Vector3 heightPoint)
    {
        var p1 = linePointA;
        var p2 = linePointB;
        var p3 = heightPoint;

        float lambda =
            (p1.x * p1.x + p2.x * p3.x - p1.x * (p2.x + p3.x) + p1.y * p1.y - p1.y * p2.y - p1.y * p3.y +
                p2.y * p3.y +
                p1.z * p1.z - p1.z * p2.z - p1.z * p3.z + p2.z * p3.z) /
            (p1.x * p1.x - 2 * p1.x * p2.x + p2.x * p2.x + p1.y * p1.y - 2 * p1.y * p2.y + p2.y * p2.y +
                p1.z * p1.z -
                2 * p1.z * p2.z + p2.z * p2.z);

        float PointCoordinate(float point1, float point2)
        {
            return point1 + (point2 - point1) * lambda;
        }

        return new Vector3(PointCoordinate(p1.x, p2.x), PointCoordinate(p1.y, p2.y), PointCoordinate(p1.z, p2.z));
    }
}