using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextAboveObject : MonoBehaviour
{
    // The Object that this label should follow
    public GameObject target;
    public Camera cam;
    // Units in world space to offset
    Vector3 offset = Vector3.up;
    public float offsetval = -0.3f;
    Vector3 pos;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Set the text to be offset from the star object
        transform.position = cam.WorldToScreenPoint(target.transform.position + offsetval * offset);
    }
}
