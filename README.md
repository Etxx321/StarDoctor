# StarDoctor
StarDoctor is a project we made over the course our bachelor's degree at the Technion Institute of Technology. Both creators of this project are electrical engineering students.

The field of real-time sky map viewing is rich in projects executed by a variety of entities such as Google, NASA and more. The purpose of sky mapping is to present to the user the locations of bright objects in space, relative to their current time and location. There are many ways to implement a sky map, ranging from a static 2D map to a full interactive 3D VR environment.

Through the course of our project, we implemented an app that allows the user to see the current position of thousands of stars outside of our solar system. To achieve this, we used 3D modeling tools, the Unity development environment for 3D applications, and the IAU SOFA’s astronomy code libraries. In addition, our app uses the HYG (Hipparcos, Yale, Gliese) database, that provides data crucial to calculate the position of stars relative to the user’s location. Using the database, and the data from the device’s sensors, the app finds the current position of the observed objects, using SOFA’s library functions that converts between celestial coordinated systems. After processing the data, the app visualizes everything in the simulated 3D environment. The orientation of the device in space is computed using the device’s magnetometer, gyroscope, and accelerometer. According to the observed field of view, relevant parts of the simulated sky map are shown on screen. 

As the user holds their device while looking at the screen, they can observe stars presented as white circles, varied in size according to the star’s brightness, on a space-like background. These white circles represent actual stars that are present is space, in the direction the device is pointing to. The app opens on a default zoom level that can be changed using a pinching gesture on the device’s screen. Zooming in or out changes the amount of data presented to the user. Zoom out to see no names of stars at all or zoom in to see observe more objects’ names. On the top of the screen the user is presented with a compass, and in the upper left corner other data is shown, ¬including orientation, date, time, and GPS location. 

Through achieving the project’s goals, we have leveraged our implementation with respect to other open-source projects, by implementing the code in a beginner friendly environment, with high level code documentation, making the field more accessible for future projects to following us.

This project is done for academic uses only.
All rights to software done by IAU SOFA, and the creators of WWA are reserved to them.
To learn more, please check out the WWA github page:
https://github.com/abrudana/wwa
Also, please see SOFA's website:
https://www.iausofa.org
