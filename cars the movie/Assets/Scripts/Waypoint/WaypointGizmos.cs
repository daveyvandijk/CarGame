using System;
using UnityEngine;

[ExecuteInEditMode, ExecuteAlways]
public class WaypointGizmos : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        if (transform.childCount == 0) return;
        
        Gizmos.color = Color.cyan;
        
       
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform waypoint = transform.GetChild(i);

            
            Gizmos.DrawSphere(waypoint.position, 0.5f);
            
            int currentWaypointIndex = i < transform.childCount - 1 ? i + 1 : 0;
            Transform nextWaypoint = transform.GetChild(currentWaypointIndex);
            
            Gizmos.DrawLine(waypoint.position, nextWaypoint.position);
            
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.white;
            UnityEditor.Handles.Label(waypoint.position + Vector3.up * 1f, $"Waypoint {i + 1}", style);
        }
    }
}
