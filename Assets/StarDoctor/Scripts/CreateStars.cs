using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using WorldWideAstronomy;
using System.Threading;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreateStars : MonoBehaviour
{
    // Variables declerations
    public Transform spawnPos;
    public GameObject spawnee;
    public GameObject loadingScreen;
    public List<GameObject> starsObjectList;
    public Canvas canvas;
    public Text textspawnee;
    public Camera cam;
    TimeZoneInfo infos = TimeZoneInfo.Local;
    Vector3 vector3 = new Vector3(0, 0, 0);
    public float longitude = 0, latitude = 0, altitude = 0;
    string time = "";
    bool isUpdating, locationupdated = false, startedmakestar = false,
         debugflag = false, makestarisdone = false, getlocationiscomplete = false;
    int timezone, year, month, day, hour, minute, amountofstarscreated = 0;
    float x, y, z, radius;
    double aob = 0, zob = 0, second, phpa = 984.2, tc = 25,
        rh = 0.5, wl = 0.55, dut1 = 0, px = 0, rv = 0;
    // Start is called before the first frame update
    void Start()
    {
        //Get the time zone at the initialization of the app
        timezone = (int)infos.GetUtcOffset(System.DateTime.UtcNow.ToLocalTime()).Hours;
        Debug.LogFormat(timezone.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        // Update the time and date based on system available information
        time = System.DateTime.UtcNow.ToLocalTime().ToString("dd-MM-yyyy   HH:mm");
        year = (int)System.DateTime.Now.Year;
        month = (int)(System.DateTime.Now.Month);
        day = (int)(System.DateTime.Now.Day);
        hour = (int)(System.DateTime.Now.Hour);
        minute = (int)(System.DateTime.Now.Minute);
        second = (double)(System.DateTime.Now.Second);

        // Update the location reading, at every frame the 
        if (!isUpdating)
        {
            StartCoroutine(GetLocation());
            isUpdating = !isUpdating;
        }
        // Call the star creation coroutine in order to initialize stars
        if (!startedmakestar)
        {
            startedmakestar = !startedmakestar;
            StartCoroutine(Makestar());
        }
        /* Once star creation coroutine is done, disable loading screen and start
         * the star location update thread. This will update stars location based
         on new readings of time, location, and more */
        if (makestarisdone)
        {
            loadingScreen.SetActive(false);
            makestarisdone = !makestarisdone;
            Thread UpdateStarThread = new Thread(UpdateStarLocation);
            UpdateStarThread.Start();
        }

    }

    // Function used to constantly update the gps reading of the user's location
    IEnumerator GetLocation()
    {
        // Check if the app has premissions to read location from system
        if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.FineLocation))
        {
            //Ask system for premissions to read location data
            UnityEngine.Android.Permission.RequestUserPermission(UnityEngine.Android.Permission.FineLocation);
            UnityEngine.Android.Permission.RequestUserPermission(UnityEngine.Android.Permission.CoarseLocation);
        }

        // If the app did not receive location permissions, end the process, and put on screen the error message 
        if (!UnityEngine.Input.location.isEnabledByUser)
        {
            locationupdated = false;
            Debug.LogFormat("Android and Location not enabled");
            getlocationiscomplete = true;
            yield break;
        }

        // Start location reading and wait 3 seconds to get a reading
        UnityEngine.Input.location.Start(500f, 500f);
        int maxWait = 3;
        while (UnityEngine.Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSecondsRealtime(1);
            maxWait--;
        }

        // If reading did not occur in 3 seconds, end with error
        if (maxWait < 1)
        {
            Debug.LogFormat("Timed out");
            getlocationiscomplete = true;
            yield break;
        }

        // Connection has failed, end with error
        if (UnityEngine.Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.LogFormat("Unable to determine device location. Failed with status {0}", UnityEngine.Input.location.status);
            getlocationiscomplete = true;
            yield break;
        }

        // If all succeeded, update all relevant variables
        else
        {
            locationupdated = true;
            longitude = UnityEngine.Input.location.lastData.longitude;
            latitude = UnityEngine.Input.location.lastData.latitude;
            altitude = UnityEngine.Input.location.lastData.altitude;
        }

        //Set flag to enable the next reading, and stop location services.
        isUpdating = !isUpdating;
        UnityEngine.Input.location.Stop();
        getlocationiscomplete = true;
    }

    /* Warpper function that waits for location reading
     * and calls the star creation function */
    IEnumerator Makestar()
    {
        // Wait for location reading
        int maxWait = 300;
        while (getlocationiscomplete == false && maxWait > 0)
        {
            yield return new WaitForSecondsRealtime(1);
            maxWait--;
        }
        // If reading failed, put error on screen
        if (locationupdated == false)
        {
            Debug.Log("Makestar got timed out");
            SceneManager.LoadScene("NoPermission");
            yield break;
        }

        /* If reading succeeded, call the star creation function
         * and end the coroutine once its done */
        else
        {
            if (debugflag)
            {
                Debug.Log("Year= " + System.DateTime.Now.Year +
                   " Month= " + System.DateTime.Now.Month +
                " Day= " + System.DateTime.Now.Day +
                " Hour= " + System.DateTime.Now.Hour +
                " Minute= " + System.DateTime.Now.Minute +
                " Second= " + System.DateTime.Now.Second);
            }

            // Call the read csv file function, which initiates star objects
            ReadCSVfile();
            while (true)
            {
                if (makestarisdone)
                {
                    yield break;
                }
            }

        }
    }

    // Csv reading function that initializes game object based on the data base
    void ReadCSVfile()
    {
        Debug.Log("ReadCSVfile");
        GameObject star;
        Text text;
        double mag;
        float radius;
        TextAsset stardata = Resources.Load<TextAsset>("hygdata_v3");
        string[] data = stardata.text.Split(new char[] { '\n' });
        // Scan all lines of the csv data file
        for (int i = 2; i < data.Length - 1; i++)
        {
            string[] row = data[i].Split(new char[] { ',' });

            // Select star to initiate based on their magnitude value
            if (System.Convert.ToDouble(row[13]) < 6.5)
            {
                /* Create a new star game object, updating its internal
                 * variables to the information stored in the data file */
                star = new GameObject();
                star = Instantiate(spawnee, spawnPos.position, spawnPos.rotation);
                star.GetComponent<UpdateStar>().id = todouble(row[0]);
                star.GetComponent<UpdateStar>().hip = todouble(row[1]);
                star.GetComponent<UpdateStar>().hd = todouble(row[2]);
                star.GetComponent<UpdateStar>().hr = todouble(row[3]);
                star.GetComponent<UpdateStar>().gl = row[4];
                star.GetComponent<UpdateStar>().bf = row[5];
                star.GetComponent<UpdateStar>().proper = row[6];
                star.GetComponent<UpdateStar>().ra = todouble(row[7]);
                star.GetComponent<UpdateStar>().dec = todouble(row[8]);
                star.GetComponent<UpdateStar>().pmra = todouble(row[10]);
                star.GetComponent<UpdateStar>().pmdec = todouble(row[11]);
                star.GetComponent<UpdateStar>().mag = todouble(row[13]);
                star.GetComponent<UpdateStar>().absmag = todouble(row[14]);
                star.GetComponent<UpdateStar>().spect = row[15];
                star.GetComponent<UpdateStar>().cam = cam;
                mag = System.Convert.ToDouble(row[13]);
                radius = Mathf.Pow(100, -(float)mag / 10);
                star.transform.localScale = new Vector3(radius, radius, radius);

                /* Call the function thst calculates the positioning of the star,
                 * based on user data */
                CalculateStarPosition(System.Convert.ToDouble(row[7]), System.Convert.ToDouble(row[8]),
                    System.Convert.ToDouble(row[10]), System.Convert.ToDouble(row[11]), ref aob, ref zob);
                // Apply transformations from shperical to euclidean coordinates
                x = (float)(20 * System.Math.Sin(aob) * System.Math.Sin(zob));
                z = (float)(20 * System.Math.Cos(aob) * System.Math.Sin(zob));
                y = (float)(20 * System.Math.Cos(zob));
                vector3 = new Vector3(x, y, z);
                // Set the object's position to the calculated place in scene and add it to initiated stars list
                star.transform.position = vector3;
                starsObjectList.Add(star);
                amountofstarscreated++;
                // If the star has an official name, or its magnitude is high enough, initiate a text object for it
                if (row[6].Length > 1)
                {
                    star.GetComponent<UpdateStar>().type = 1;
                    text = Instantiate(textspawnee, new Vector3(0, 0, 0), Quaternion.identity);
                    text.transform.SetParent(canvas.transform);
                    star.GetComponent<UpdateStar>().text = text;
                    text.GetComponent<TextAboveObject>().target = star;
                    text.GetComponent<TextAboveObject>().cam = cam;
                    text.GetComponent<Text>().text = row[6];
                    // Offset for overlapping objects
                    if (row[6] == "Alcor")
                    {
                        text.GetComponent<TextAboveObject>().offsetval = 0.3f;
                    }
                }
                else if (row[5].Length > 1)
                {
                    star.GetComponent<UpdateStar>().type = 2;
                    text = Instantiate(textspawnee, new Vector3(0, 0, 0), Quaternion.identity);
                    text.transform.SetParent(canvas.transform);
                    star.GetComponent<UpdateStar>().text = text;
                    text.GetComponent<TextAboveObject>().target = star;
                    text.GetComponent<TextAboveObject>().cam = cam;
                    text.GetComponent<Text>().text = row[5];
                    text.GetComponent<Text>().color = Color.yellow;
                }
            }
        }
        Debug.Log("Total stars created: " + amountofstarscreated);
        makestarisdone = !makestarisdone;
    }

    double todouble(string instring)
    {
        if (instring == "" || instring == null)
        {
            return 0;
        }
        else
        {
            return System.Convert.ToDouble(instring);
        }
    }

    /* Thread function that runs in the background, going over the list of
     * initiated stars, and updating their location one by one */
    void UpdateStarLocation()
    {
        double curr_ra, curr_dec, curr_pmra, curr_pmdec;
        while (true)
        {
            foreach (GameObject CurrentStar in starsObjectList)
            {
                curr_ra = CurrentStar.GetComponent<UpdateStar>().ra;
                curr_dec = CurrentStar.GetComponent<UpdateStar>().dec;
                curr_pmra = CurrentStar.GetComponent<UpdateStar>().pmra;
                curr_pmdec = CurrentStar.GetComponent<UpdateStar>().pmdec;
                /* Call the function that calculates the positioning of the star,
                 * based on user data */
                CalculateStarPosition(curr_ra, curr_dec, curr_pmra, curr_pmdec, ref aob, ref zob);
                x = (float)(20 * System.Math.Sin(aob) * System.Math.Sin(zob));
                z = (float)(20 * System.Math.Cos(aob) * System.Math.Sin(zob));
                y = (float)(20 * System.Math.Cos(zob));
                vector3 = new Vector3(x, y, z);
                // Set the object's position to the calculated place
                CurrentStar.transform.position = vector3;
            }
        }
    }

    // Display on top of the scene a GUI containing information from the software
    void OnGUI()
    {
        GUILayout.Label("");
        GUILayout.Label("");
        GUILayout.Label("True heading: " + Input.compass.trueHeading);
        GUILayout.Label("Longitude: " + longitude);
        GUILayout.Label("Latitude: " + latitude);
        GUILayout.Label("Time: " + time);
        GUILayout.Label("Pitch: " + cam.GetComponent<Gyrocontrol>().pitch);
        GUILayout.Label("fieldOfView: " + cam.fieldOfView);
    }

    //Function that calculates the positioning of the star using the WWA library based on SOFA's C libraries
    public int CalculateStarPosition(double ra, double dec, double pmra,
                                     double pmdec, ref double aob, ref double zob)
    {
        // Variables declerations
        double phi = 0, elong = 0, hm, utc1 = 0, utc2 = 0, tai1 = 0, tai2 = 0,
             tt1 = 0, tt2 = 0, xp, yp, rc = 0, dc = 0, pr, pd,
             eo = 0, hob = 0, dob = 0, rob = 0;
        // Transform gps readings to radians
        elong = longitude * WWA.DD2R;
        phi = latitude * WWA.DD2R;
        hm = altitude;

        /* UTC date. */
        if (WWA.wwaDtf2d("UTC", timezone, year, month, day, hour, minute, second, ref utc1, ref utc2) != 0) return -1; //CHANGED THE CODE TO INCLUDE TIME ZONES!!!
        /* TT date. */
        if (WWA.wwaUtctai(utc1, utc2, ref tai1, ref tai2) != 0) return -1;
        if (WWA.wwaTaitt(tai1, tai2, ref tt1, ref tt2) != 0) return -1;
        /* EOPs: polar motion in radians, UT1-UTC in seconds */
        xp = 0.1908 * WWA.DAS2R;
        yp = 0.2606 * WWA.DAS2R;
        /* Star ICRS RA,Dec (radians) */
        rc = (60.0 * (60.0 * ra)) * WWA.DS2R;
        dc = (60.0 * (60.0 * dec)) * WWA.DAS2R;
        /* Annual proper motion: RA/Dec derivatives, epoch J2000.0 */
        pr = System.Math.Atan2(pmra * (1e-3) * WWA.DAS2R, System.Math.Cos(dc));
        pd = pmdec * (1e-3) * WWA.DAS2R;

        /* ICRS to observed */
        if (WWA.wwaAtco13(rc, dc, pr, pd, px, rv, utc1, utc2, dut1,
                         elong, phi, hm, xp, yp, phpa, tc, rh, wl,
                         ref aob, ref zob, ref hob, ref dob, ref rob, ref eo) != 0) return -1;
        return 0;
    }
}