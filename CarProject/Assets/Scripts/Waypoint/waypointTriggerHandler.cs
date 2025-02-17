using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class waypointTriggerHandler : MonoBehaviour
{
    private int currentWaypointIndex = 0;
    private int currentLap = 1;
    public int totalLaps = 3;
    private List<Transform> waypoints;
    public Transform waypointPartent;

    private void Start()
    {
        if (waypointPartent == null)
        {
            waypointPartent = GameObject.Find("Waypoints")?.transform;

            if (waypointPartent == null)
            {
                Debug.LogError("er is geen naam met Waypoints gevonden ");
                return;
            }
        }
        
        
        waypoints = new List<Transform>();
        foreach (Transform child in waypointPartent)
        {
            waypoints.Add(child);
        }

        if (waypoints.Count < 2)
        {
            Debug.LogError("Er zijn niet genoeg waypoints om te racen");
        }
    }

    private void OnTriggerEnter (Collider other)
    {
        if (other.transform == waypoints[currentWaypointIndex])
        {
            Debug.Log($"Waypoint {currentWaypointIndex + 1} bereikt.");
            currentWaypointIndex++;
            
            if (currentWaypointIndex >= waypoints.Count)
            {
                if (currentLap >= totalLaps)
                {
                    Debug.Log("Race Gewonnen");
                }

                else
                {
                    Debug.Log($"Ronde {currentLap} voltooid Volgende ronden start!");
                    currentLap++;
                    currentWaypointIndex = 0;
                }
            }
        }
        else if (waypoints.Contains(other.transform))
        {
            Debug.Log("Wrong waypoint! Follow the correct path.");
        }
    }
}
