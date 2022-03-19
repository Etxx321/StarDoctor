using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Compass : MonoBehaviour
{
    // Variables declerations
    private float m_lastMagneticHeading = 0.0f;
    public float compassSmooth = 0.5f;
    public float smoothing = 0.5f;
    Quaternion rotation;

    // Start is called before the first frame update
    void Start()
    {
        // Start location services and enable compass
        Input.location.Start();
        Input.compass.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        // Read the current magnetometer reading
        float currentMagneticHeading = (float)Math.Round(Input.compass.trueHeading, 2);
        // Using smoothing constant, check if the reading is changed enough (based on the user looking direction) from previous reading
        if (m_lastMagneticHeading < currentMagneticHeading - compassSmooth || m_lastMagneticHeading > currentMagneticHeading + compassSmooth)
        {
            // Update the rotation of the apps sphere based on the magnetic reading
            m_lastMagneticHeading = currentMagneticHeading;
            rotation = Quaternion.Euler(0, (-m_lastMagneticHeading), 0);
            // Due to the camera also changing the user prespective, based on the gyro and accelerometer, a 0.5 update constant is being used 
            rotation = Quaternion.Euler(0, ((float)0.5 * rotation.y), 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, smoothing);
        }
    }
}