# StarDoctor
StarDoctor is a project we made over the course our bachelor's degree at the Technion Institute of Technology. Through the course of our project, we implemented an app that allows the user to see the current position of thousands of stars outside of our solar system. To achieve this, we used 3D modeling tools, the Unity development environment for 3D applications, and the IAU SOFA’s astronomy code libraries. In addition, our app uses the HYG (Hipparcos, Yale, Gliese) database, that provides data crucial to calculate the position of stars relative to the user’s location. Using the database, and the data from the device’s sensors, the app finds the current position of the observed objects, using SOFA’s library functions that converts between celestial coordinated systems. After processing the data, the app visualizes everything in the simulated 3D environment. The orientation of the device in space is computed using the device’s magnetometer, gyroscope, and accelerometer. According to the observed field of view, relevant parts of the simulated sky map are shown on screen. 