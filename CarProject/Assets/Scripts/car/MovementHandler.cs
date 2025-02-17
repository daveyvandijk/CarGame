using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementHandler : MonoBehaviour
{
    [SerializeField] private float acceleration = 25f;
    [SerializeField] private float maxSpeed = 100f;
    [SerializeField] private float deceleration = 10f;
    [SerializeField] private float steerStrength = 15f;
    [SerializeField] private AnimationCurve turningCurve;
    [SerializeField] private float dragCoefficient = 1f;
    [SerializeField] private Transform accelerationPoint;

    private Rigidbody carRB;
    private InputHandler inputHandler;
    private VisualsHandler visualsHandler;

    private void Awake()
    {
        carRB = GetComponent<Rigidbody>();
        inputHandler = GetComponent<InputHandler>();
        visualsHandler = GetComponent<VisualsHandler>();
    }

    private void FixedUpdate()
    {
        visualsHandler.UpdateVisuals();
        inputHandler.CaptureInput();
        ApplyMovement();
        ApplySteering();
        ApplySidewaysDrag();
    }

    private void ApplyMovement()
    {
        if (Mathf.Abs(inputHandler.MoveInput) > 0.01f)
        {
            carRB.AddForceAtPosition(acceleration * inputHandler.MoveInput * transform.forward, accelerationPoint.position, ForceMode.Acceleration);
        }
        else
        {
            Vector3 currentVelocity = carRB.velocity;
            Vector3 brakingForce = -currentVelocity.normalized * deceleration * Time.fixedDeltaTime;
            carRB.AddForce(brakingForce, ForceMode.Acceleration);
        }
    }

    private void ApplySteering()
    {
        float velocityRatio = carRB.velocity.magnitude / maxSpeed;
        carRB.AddTorque(steerStrength * inputHandler.SteerInput * turningCurve.Evaluate(velocityRatio) * transform.up, ForceMode.Acceleration);
    }

    private void ApplySidewaysDrag()
    {
        Vector3 localVelocity = transform.InverseTransformDirection(carRB.velocity);
        float dragMagnitude = -localVelocity.x * dragCoefficient;
        Vector3 dragForce = transform.right * dragMagnitude;
        carRB.AddForceAtPosition(dragForce, carRB.worldCenterOfMass, ForceMode.Acceleration);
    }
}
