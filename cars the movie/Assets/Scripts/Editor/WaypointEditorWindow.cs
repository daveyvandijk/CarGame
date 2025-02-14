using System;
using UnityEngine;
using UnityEditor;

public class WaypointEditorWindow : EditorWindow
{
    private GameObject waypointParent;
    private GameObject waypointPrefab;
    private GameObject[] waypoints;

    [MenuItem("Tools/Waypoints Editor")]
    public static void ShowWindow()
    {
        GetWindow<WaypointEditorWindow>("Waypoint Editor");
    }

    private void OnGUI()
    {
        GUILayout.Label("Waypoint Editor", EditorStyles.boldLabel);
        waypointParent = (GameObject)EditorGUILayout.ObjectField("Waypoint Parent", waypointParent, typeof(GameObject), true);
        waypointPrefab = (GameObject)EditorGUILayout.ObjectField("Waypoint Prefab", waypointPrefab, typeof(GameObject), false);

        if (GUILayout.Button("Add Waypoint"))
        {
            CreateWaypoint();
        }

        if (waypointParent != null)
        {
            waypoints = new GameObject[waypointParent.transform.childCount];
            for (int i = 0; i < waypointParent.transform.childCount; i++)
            {
                waypoints[i] = waypointParent.transform.GetChild(i).gameObject;
            }

            foreach (GameObject waypoint in waypoints)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(waypoint.name);
                if (GUILayout.Button("Remove"))
                {
                    RemoveWaypoint(waypoint);
                }
                EditorGUILayout.EndHorizontal();
            }
        }
    }

    private void CreateWaypoint()
    {
        if (waypointParent == null)
        {
            Debug.LogWarning("Please assign a parent GameObject for the waypoints.");
            return;
        }

        GameObject newWaypoint = new GameObject("Waypoint");
        Vector3 newPosition = Vector3.zero;

        int childCount = waypointParent.transform.childCount;
        if (childCount > 0)
        {
            Transform lastWaypoint = waypointParent.transform.GetChild(childCount - 1);
            newPosition = lastWaypoint.position + new Vector3(0f, 0f, 0f);
        }

        newWaypoint.transform.position = newPosition;
        newWaypoint.transform.parent = waypointParent.transform;

        BoxCollider collider = newWaypoint.AddComponent<BoxCollider>();
        collider.isTrigger = true;
        collider.size = new Vector3(30, 10, 1);
        collider.center = new Vector3(0, 5, 0);

        newWaypoint.tag = "Waypoint";

        if (waypointPrefab != null)
        {
            AddVisualMarkers(newWaypoint, collider);
        }

        Selection.activeGameObject = newWaypoint;
    }

    private void AddVisualMarkers(GameObject waypoint, BoxCollider collider)
    {
        WaypointMarkerManager markerManager = waypoint.AddComponent<WaypointMarkerManager>();
        markerManager.InitializeMarkers(waypointPrefab, collider);
    }


    private void RemoveWaypoint(GameObject waypoint)
    {
        if (waypoint != null)
        {
            DestroyImmediate(waypoint);
        }
    }
}
