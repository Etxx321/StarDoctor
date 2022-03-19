using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICompass : MonoBehaviour
{
    public RawImage compassImage;
    public Transform player;
    public Transform sphere;

    // Update is called once per frame
    void Update()
    {
        //Update the UI compass, based on the player's looking direction
        compassImage.uvRect = new Rect((player.localEulerAngles.y - sphere.localEulerAngles.y) / 360f, 0f, 1f, 1f);
    }
}
