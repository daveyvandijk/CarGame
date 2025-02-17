using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuspensionHandler : MonoBehaviour
{   
        [Header("Suspension Settings")]
    [SerializeField] private Transform[] rayPoints;
    [SerializeField] private LayerMask drivable;
    [SerializeField] private float springStiffness;
    [SerializeField] private float damperStiffness;
    [SerializeField] private float restLength;
    [SerializeField] private float springTravel;
    [SerializeField] private float wheelRadius;
    [SerializeField] private GameObject[] tires;

    private Rigidbody carRB;
    private int[] wheelsIsGrounded;

    private void Awake()
    {
        carRB = GetComponent<Rigidbody>();
        wheelsIsGrounded = new int[rayPoints.Length];
    }

    private void FixedUpdate()
    {
        HandleSuspension();
        ApplyAutoStabilization();
    }

    public void HandleSuspension()
    {
        for (int i = 0; i < rayPoints.Length; i++)
        {
            RaycastHit hit;
            float maxDistance = restLength + springTravel;

            if (Physics.Raycast(rayPoints[i].position, -rayPoints[i].up, out hit, maxDistance + wheelRadius, drivable))
            {
                wheelsIsGrounded[i] = 1;

                float currentSpringLength = hit.distance - wheelRadius;
                float springCompression = (restLength - currentSpringLength) / springTravel;

                float springVelocity = Vector3.Dot(carRB.GetPointVelocity(rayPoints[i].position), rayPoints[i].up);
                float dampForce = damperStiffness * springVelocity;

                float springForce = springStiffness * springCompression;
                float netForce = springForce - dampForce;

                carRB.AddForceAtPosition(netForce * rayPoints[i].up, rayPoints[i].position);
                tires[i].transform.position = hit.point + rayPoints[i].up * wheelRadius;
            }
            else
            {
                tires[i].transform.position = rayPoints[i].position - rayPoints[i].up * maxDistance;
                wheelsIsGrounded[i] = 0;
            }
        }
    }

    private void ApplyAutoStabilization()
    {
        if (carRB.velocity.magnitude < 0.1f)
        {
            carRB.velocity = Vector3.zero;
            carRB.angularVelocity = Vector3.zero;
        }
        else
        {
            Vector3 frictionForce = -carRB.velocity.normalized * 0.1f; 
            carRB.AddForce(frictionForce, ForceMode.VelocityChange);
        }
    }


    private void OnDrawGizmos()
    {
        if (rayPoints == null) return;
        Gizmos.color = Color.red;

        foreach (var rayPoint in rayPoints)
        {
            if (rayPoint != null)
            {
                Gizmos.DrawLine(rayPoint.position, rayPoint.position - rayPoint.up * (restLength + springTravel));
                Gizmos.DrawSphere(rayPoint.position - rayPoint.up * (restLength + springTravel), 0.1f);
            }
        }
    }
}
