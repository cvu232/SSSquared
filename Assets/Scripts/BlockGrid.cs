using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockGrid : MonoBehaviour
{
    public int XGridMeshSize = 100; // unit size of gizmo display. useful
    public int YGridMeshSize = 50;
    public float pointDist = 5.0f;
    public float pointSize = 0.2f;

    public Vector3 GetNearestPointOnGrid(Vector3 position)
    {
        position -= transform.position;

        int xPoints = Mathf.RoundToInt(position.x / pointDist); // nearest x of a point
        int yPoints = Mathf.RoundToInt(position.y / pointDist); // nearest y ..

        Vector3 point = new Vector3(xPoints * pointDist, yPoints * pointDist, 0f); // point coord

        point += transform.position; // move point by 2d offset
        return point;
    }

    /*
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        // grid visual markers //
        for (float x = -XGridMeshSize; x < XGridMeshSize; x += pointDist/2.0f)
        {
            for (float y = -YGridMeshSize; y < YGridMeshSize; y += pointDist/2.0f)
            {
                var point = GetNearestPointOnGrid(new Vector3(x, y, 0f));
                Gizmos.DrawSphere(point, pointSize);
            }
        }
    }
    */
}
