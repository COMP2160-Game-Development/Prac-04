using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreepMove : MonoBehaviour
{
    [SerializeField] private float speed = 2; // metres per second
    [SerializeField] private Path path;
    private int nextWaypoint = 1;

    void Start()
    {
        transform.position = path.Waypoint(0); // start at waypoint 0
        Vector3 waypoint = path.Waypoint(1);
        Vector3 direction = waypoint - transform.position;
        transform.rotation = Quaternion.FromToRotation(Vector3.forward, direction); // rotate to face the next waypoint
    }

    void Update()
    {
        Vector3 waypoint = path.Waypoint(nextWaypoint);
        float distanceTravelled = speed * Time.deltaTime;
        float distanceToWaypoint = Vector3.Distance(waypoint, transform.position);

        if (distanceToWaypoint <= distanceTravelled)
        {
            transform.position = waypoint; // reached the waypoint, start heading to the next one
            NextWaypoint();
        }
        else 
        { 
            Vector3 direction = waypoint - transform.position;
            direction = direction.normalized;
            transform.Translate(direction * distanceTravelled, Space.World); // move towards waypoint

            transform.rotation = Quaternion.FromToRotation(Vector3.forward, direction); // rotate to face waypoint
        }
    }

    private void NextWaypoint()
    {
        nextWaypoint++;
        if (nextWaypoint == path.Length) // destroy self if we have reached the end of the path.
        {
            Destroy(gameObject);
        }
    }
}
