using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public float MoveInput { get; private set; }
    public float SteerInput { get; private set; }
    
    public void CaptureInput()
    {
        MoveInput = Input.GetAxis("Vertical");
        SteerInput = Input.GetAxis("Horizontal");
    }
}
