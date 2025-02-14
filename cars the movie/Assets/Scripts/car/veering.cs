using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class veering : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private Rigidbody carRB;
    [SerializeField] private Transform[] rayPoints;
    [SerializeField] private LayerMask drivable;
    [SerializeField] private Transform accelerationpoint;
    [SerializeField] private GameObject[] tires = new GameObject[4];
    [SerializeField] private GameObject[] frontTireParents = new GameObject[2];

    [Header("Suspension Settings")] 
    [SerializeField] private float springStiffness;
    [SerializeField] private float damperStiffness;
    [SerializeField] private float restLength;
    [SerializeField] private float springTravel;
    [SerializeField] private float wheelRadius;

    [Header("Input")] 
    private float moveInput = 0;
    private float steerInput = 0;

    [Header("Car settings")] 
    [SerializeField] private float acceleration = 25f;
    [SerializeField] private float maxSpeed = 100f;
    [SerializeField] private float deceleration = 10f;
    [SerializeField] private float steerStrength = 15f;
    [SerializeField] private AnimationCurve turningCurve;
    [SerializeField] private float dragCoefficient = 1f;

    private Vector3 currentCarLocalVelocity = Vector3.zero;
    private float carVelocityRatio = 0;
    
    private int[] wheelsIsGrounded = new int[4];
    private bool isGrounded = false;

    [Header("visuals")] 
    [SerializeField] private float tireRotSpeed = 3000f;
    [SerializeField] private float maxSteeringAngle = 30f;
    private void Start()
    {
        carRB = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Suspension();
        GroundCheck();
        CalculateCarVelocity();
        Movement();
        Visuals();
    }
    
    private void Update()
    {
        GetPlayerInput();
    }

    private void Movement()
    {
        if (isGrounded)
        {
            Acceleration();
            Deceleration();
            Turn();
            SidewaysDrag();
        }
    }

    private void Visuals()
    {
        TireVisuals();
    }

    private void TireVisuals()
    {
        for (int i = 0; i < tires.Length; i++)
        {
            float rotationSpeed = currentCarLocalVelocity.z * tireRotSpeed * Time.fixedDeltaTime / wheelRadius;
            tires[i].transform.Rotate(Vector3.right, rotationSpeed);
        }

        // Voorwielen laten sturen
        float steeringAngle = steerInput * maxSteeringAngle;
        frontTireParents[0].transform.localRotation = Quaternion.Euler(0, steeringAngle, 0); // Linker voorband
        frontTireParents[1].transform.localRotation = Quaternion.Euler(0, steeringAngle, 0); // Rechter voorband
    }

    private void SetTirePosition(GameObject tire, Vector3 targetPosition)
    {
        tire.transform.position = targetPosition;
    }

    private void Acceleration()
    {
        carRB.AddForceAtPosition(acceleration * moveInput * transform.forward, accelerationpoint.position, ForceMode.Acceleration);
    }
    
    private void Deceleration()
    {
        if (Mathf.Abs(moveInput) > 0)
        {
            carRB.AddForceAtPosition(deceleration * moveInput * -transform.forward, accelerationpoint.position, ForceMode.Acceleration);
        }
        else
        {
            Vector3 currentVelocity = carRB.velocity;
            Vector3 brakingForce = -currentVelocity.normalized * deceleration * Time.fixedDeltaTime;
            carRB.AddForce(brakingForce, ForceMode.Acceleration);
        }
    }


    private void Suspension()
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
                float dampforce = damperStiffness * springVelocity;

                float springForce = springStiffness * springCompression;

                float netForce = springForce - dampforce;
                
                carRB.AddForceAtPosition(netForce * rayPoints[i].up, rayPoints[i].position);
                
                SetTirePosition(tires[i], hit.point + rayPoints[i].up * wheelRadius);
                
                Debug.DrawLine(rayPoints[i].position, hit.point, Color.red);
            }
            else
            {
                SetTirePosition(tires[i], rayPoints[i].position - rayPoints[i].up * maxDistance);
                wheelsIsGrounded[i] = 0;
                Debug.DrawLine(rayPoints[i].position, rayPoints[i].position + (wheelRadius + maxDistance) * -rayPoints[i].up, Color.green);
            }
        }
    }

    private void GroundCheck()
    {
        int tempGroundedwheels = 0;

        for (int i = 0; i < wheelsIsGrounded.Length; i++)
        {
            tempGroundedwheels += wheelsIsGrounded[i];
        }

        if (tempGroundedwheels > 1 )
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    private void GetPlayerInput()
    {
        moveInput = Input.GetAxis("Vertical");
        if (Mathf.Abs(moveInput) < 0.01f)
        {
            moveInput = 0;
        }
        steerInput = Input.GetAxis("Horizontal");
    }

    private void CalculateCarVelocity()
    {
        currentCarLocalVelocity = transform.InverseTransformDirection(carRB.velocity);
        carVelocityRatio = currentCarLocalVelocity.z / maxSpeed;
    }

    private void Turn()
    {
        carRB.AddTorque(steerStrength * steerInput * turningCurve.Evaluate(carVelocityRatio) * Mathf.Sign(carVelocityRatio) * transform.up, ForceMode.Acceleration);
    }

    private void SidewaysDrag()
    {
        float currentSidewaysSpeed = currentCarLocalVelocity.x;
        float dragMagnitude = -currentSidewaysSpeed * dragCoefficient;
        Vector3 dragForce = transform.right * dragMagnitude;
        carRB.AddForceAtPosition(dragForce, carRB.worldCenterOfMass, ForceMode.Acceleration);
    }
}
