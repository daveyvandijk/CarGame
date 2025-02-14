using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualsHandler : MonoBehaviour
{
    [Header("Visual Settings")]
    [SerializeField] private GameObject[] tires;
    [SerializeField] private GameObject[] frontTireParents;
    [SerializeField] private float maxSteeringAngle = 30f;
    [SerializeField] private float tireRotSpeed = 3000f;

    private InputHandler inputHandler;
    private Rigidbody carRB;

    private void Awake()
    {
        inputHandler = GetComponent<InputHandler>();
        carRB = GetComponent<Rigidbody>();
    }

    public void UpdateVisuals()
    {
        UpdateTireRotation();
        UpdateSteeringAngle();
    }

    private void UpdateTireRotation()
    {
        float carSpeed = carRB.velocity.magnitude; 

        
        foreach (var tire in tires)
        {
            float rotationSpeed = carSpeed * tireRotSpeed * Time.deltaTime;
            tire.transform.Rotate(Vector3.right, rotationSpeed);
        }
    }

    private void UpdateSteeringAngle()
    {
        float steeringAngle = inputHandler.SteerInput * maxSteeringAngle;
        foreach (var frontTire in frontTireParents)
        {
            frontTire.transform.localRotation = Quaternion.Euler(0, steeringAngle, 0);
        }
    }

}
