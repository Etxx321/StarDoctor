using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gyrocontrol : MonoBehaviour
{

    private float _initialYAngle = 0f;
    private float _appliedGyroYAngle = 0f;
    private float _calibrationYAngle = 0f;
    private Transform _rawGyroRotation;
    private float _tempSmoothing;
    public float pitch;
    [SerializeField] private float _smoothing = 0f;

    // Initiate sensors at the beggining of the app
    private IEnumerator Start()
    {
        Input.gyro.enabled = true;
        Application.targetFrameRate = 60;
        _initialYAngle = transform.eulerAngles.y;
        _rawGyroRotation = new GameObject("GyroRaw").transform;
        _rawGyroRotation.position = transform.position;
        _rawGyroRotation.rotation = transform.rotation;

        // Wait until gyro is active, then calibrate to reset starting rotation.
        yield return new WaitForSeconds(1);

        StartCoroutine(CalibrateYAngle());
    }

    // Update is called once per frame
    private void Update()
    {
        //Update the gryo readings
        ApplyGyroRotation();

        // Find the phone's current pitch
        pitch = -Mathf.Rad2Deg * Mathf.Atan2(2 * _rawGyroRotation.rotation.x * _rawGyroRotation.rotation.w - 2 * _rawGyroRotation.rotation.y * _rawGyroRotation.rotation.z,
            1 - 2 * _rawGyroRotation.rotation.x * _rawGyroRotation.rotation.x - 2 * _rawGyroRotation.rotation.z * _rawGyroRotation.rotation.z);

        // Transform camera rotation based on movement
        transform.rotation = Quaternion.Slerp(transform.rotation, _rawGyroRotation.rotation, _smoothing);
    }

    // Function responsible for calibrating the Y angle based on sensor readings
    private IEnumerator CalibrateYAngle()
    {
        _tempSmoothing = _smoothing;
        _smoothing = 1;

        // Offsets the y angle in case it wasn't 0 at edit time
        _calibrationYAngle = _appliedGyroYAngle - _initialYAngle;
        yield return null;
        _smoothing = _tempSmoothing;
    }

    // Function responsible for applying rotation base on sensor readings 
    private void ApplyGyroRotation()
    {
        _rawGyroRotation.rotation = Input.gyro.attitude;
        // Swap "handedness" of quaternion from gyro
        _rawGyroRotation.Rotate(0f, 0f, 180f, Space.Self);
        // Rotate to make sense as a camera pointing out the back of your device
        _rawGyroRotation.Rotate(90f, 180f, 0f, Space.World);
        // Save the angle around y axis for use in calibration
        _appliedGyroYAngle = _rawGyroRotation.eulerAngles.y;
    }
}
