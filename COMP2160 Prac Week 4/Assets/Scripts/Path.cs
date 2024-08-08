using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Path : MonoBehaviour
{
    private LineRenderer lineRenderer;

    public int Length
    {
        get
        {
            return transform.childCount;
        }
    }

    void Start()
    {
        Vector3[] waypoints = new Vector3[transform.childCount];

        for (int i = 0; i < waypoints.Length; i++)
        {
            waypoints[i] = transform.GetChild(i).position;
        }

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = waypoints.Length;
        lineRenderer.SetPositions(waypoints);
    }

    public Vector3 Waypoint(int i)
    {
        return transform.GetChild(i).position;
    }
 
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        Vector3 last = transform.GetChild(0).position;
        for (int i = 1; i < transform.childCount; i++)
        {
            Vector3 next = transform.GetChild(i).position;
            Gizmos.DrawLine(last, next);
            last = next;
        }
        
    }
}
