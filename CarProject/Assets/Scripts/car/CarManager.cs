using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CarManager : MonoBehaviour
{
    [SerializeField] private InputHandler inputHandler;
    [SerializeField] private SuspensionHandler suspensionHandler;
    [SerializeField] private MovementHandler movementHandler;
    [SerializeField] private VisualsHandler visualsHandler;

    private void Awake()
    {
        
        if (inputHandler == null) inputHandler = GetComponent<InputHandler>();
        if (suspensionHandler == null) suspensionHandler = GetComponent<SuspensionHandler>();
        if (movementHandler == null) movementHandler = GetComponent<MovementHandler>();
        if (visualsHandler == null) visualsHandler = GetComponent<VisualsHandler>();
    }

    private void FixedUpdate()
    {
        suspensionHandler.HandleSuspension();
    }

    private void Update()
    {
        inputHandler.CaptureInput();
        visualsHandler.UpdateVisuals();
    }
}
