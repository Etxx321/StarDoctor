using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class UpdateStar : MonoBehaviour
{
    // Variables declerations
    public string gl, bf, proper, spect;
    public double ra, dec, pmra, pmdec, mag, absmag, id, hip, hd, hr;
    public int type = 0;
    public Text text;
    public Camera cam;
    // Start is called before the first frame update
    void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {
        if (text != null)
        {
            // Once the star is out of the viewing area, disable the text attached to it
            if (cam.WorldToScreenPoint(transform.position).z < 0)
            {
                text.enabled = false;
            }
            else
            {
                // Based on the type of star, enable and disable text based on the amount of zoom determined by user gesture
                switch (type)
                {
                    case 1:
                        if (cam.fieldOfView < 70)
                        {
                            text.enabled = true;
                        }
                        else
                        {
                            text.enabled = false;
                        }
                        break;
                    case 2:
                        if (cam.fieldOfView < 30)
                        {
                            text.enabled = true;
                        }
                        else
                        {
                            text.enabled = false;
                        }
                        break;
                    default:
                        text.enabled = false;
                        break;
                }
            }
        }
    }

}
