# Networked-Boids
3D networked boids with adjustable settings created using Unity Engine, Photon &amp; C++

###Info & Features

 - Data-coupled model, boids of each flocks are drawn to boids of other flocks and will account for separation
 - Flock of 100 boids (Threshold for network stability)
 - Target Seeking, to flock leader position (star)
 - Collision avoidance, boids will not move away from objects if they collide
 - Dead reckoning

**Debugging**

 - Adjustable latency settings
 - Adjustable flock settings
 - Trail renderer to see boid positions (Disabled by default)
 - Object following path
	 
###To run

``*``**INTERNET CONNECTION REQUIRED**

 1. Launch instance 1 of "Networked_Boids.exe"
		-Wait for and click button labelled "Create Room"
 2. Launch instance 2 of "Networked_Boids.exe"
		-Wait for and click button labelled "Join Room: network_boids_room1"
 3. Everything should be working and you should be able to see 2 flock systems with each client controlling 1

###CONTROLS

Input Key    |  Function
-------- | ---
	W, A, S, D  | Movement
	Cam 		| Change Elevation
	R			| Randomize colors
	T			| Toggle Trail renderer
	Mouse		| Selecting buttons / sliders
	
###Built with Unity 5
Assets from the Unity Asset Store Used

 - Photon Unity Networking Free 1.51
 - Fantasy Skybox Free 1.2
 - iTween
