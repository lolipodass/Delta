using System.Collections.Generic;
using UnityEngine;

public class PatrolGroup : MonoBehaviour
{
    [Tooltip("Path color in editor")]
    public Color pathColor = Color.yellow;

    [Tooltip("Connect last to first")]
    public bool loopPath = false;
    public bool ShowAlways = false;
    public Transform[] GetPatrolPoints()
    {
        List<Transform> patrolPoints = new List<Transform>();

        for (int i = 0; i < transform.childCount; i++)
        {

            patrolPoints.Add(transform.GetChild(i));
        }

        return patrolPoints.ToArray();
    }

    public Transform[] Points => GetPatrolPoints();

    void OnDrawGizmos()
    {
        if (ShowAlways)
        {
            OnDrawGizmosSelected();
        }
    }
    public void OnDrawGizmosSelected()
    {
        Gizmos.color = pathColor;

        int childCount = transform.childCount;

        if (childCount < 2)
        {
            return;
        }

        for (int i = 0; i < childCount - 1; i++)
        {
            Transform currentPoint = transform.GetChild(i);
            Transform nextPoint = transform.GetChild(i + 1);
            Gizmos.DrawLine(currentPoint.position, nextPoint.position);
        }

        if (loopPath)
        {
            Transform lastPoint = transform.GetChild(childCount - 1);
            Transform firstPoint = transform.GetChild(0);
            Gizmos.DrawLine(lastPoint.position, firstPoint.position);
        }

        Gizmos.color = Color.red;
        for (int i = 0; i < childCount; i++)
        {
            Gizmos.DrawSphere(transform.GetChild(i).position, 0.1f);
        }
    }
}
