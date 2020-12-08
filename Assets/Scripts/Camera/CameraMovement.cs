using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// - Goatbandit

public class CameraMovement : MonoBehaviour
{
    public GameObject desiredCameraTarget; // Select what you'd like to focus the camera on

    public int speed = 5; // Select your desired movement speed for automatic turning
    
    void Update()
    {
        transform.LookAt (desiredCameraTarget.transform); // Focus on the desired camera target
        transform.Translate (Vector3.right * speed * Time.deltaTime); // Begin circular movement multiplied by the desired speed
    }
}

// - Goatbandit
