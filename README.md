# COMP2160 Prac 04: Tower Defense

## Topics covered:
* Code Architecture – Entity Relationship Diagrams
* Collision
* Raycasting

## Discussion

## Today's Task
In this prac you will implement a tower defense game: 
https://wordsonplay.itch.io/comp2160-prac-5-tower-defense

The player places towers (blue cylinders) near the path to kill the yellow creeps.

The base framework already implements the path and creep prefab for you. In the Documentation folder there is a powerpoint file which contains an Entity Relationship diagram (ERD) for the code as it stands.

Notice that the creep prefab is made up of two components: CreepMove and CreepHealth. This functionality has been split up, since there is little interaction between the movement code for the creep and the health code (i.e. they are loosely coupled). By splitting it into two components, we make each one simpler to read, and easier to understand. Follow this development philosophy throughout this prac, and in your assignment.

## Step 1 – Add a creep spawner
* Take a look at the CreepMove class. Notice how it uses the path object to determine which waypoint the creep is currently moving towards. Here is a snippet from the Update class that demonstrates some of this:

```
    Vector3 waypoint = path.Waypoint(nextWaypoint);
    float distanceTravelled = speed * Time.deltaTime;
    float distanceToWaypoint = Vector3.Distance(waypoint, transform.position);

    if (distanceToWaypoint <= distanceTravelled)
    {
    transform.position = waypoint; // reached the waypoint, start heading to the next one
    NextWaypoint();
}
```

* Edit the ERD to add a CreepSpawner class. We want this class to spawn new instances of the creep prefab on the path, with a designer-specified time in between creeps. Add a node to the ERD to represent this.
* Implement the CreepSpawner, following the same pattern in Week 3 for creating the ObstacleSpawner.

The spawned Creeps need to have a way to find the path they are going to be navigating. The current method of dropping the path into the Creep's Path field in the inspector won't work, as this is scene-specific, and we are instantiating new Creeps as we go. The lecture introduced us to three different methods to handle this. I'll lay out some code snippets for each, and it's up to you to decide which one to implement and why.

### Set the path on the Spawner
Instead of setting the path on the Creeper, we can instead set this on the Spawner itself and pass it on when a new Creeper is created. Add the following code to your CreepSpawner class to allow allocation in the inspector:

```
[SerializeField] private Path path;
```

In your CreepMove script, create a property for setting the Path parameter:

```
public Path Path
{
    get
    {
        return Path;
    }
    set
    {
        path = value;
    }
}
```

Back in your CreepSpawner class, add the following code after instantiating the Creeper (as it fits your own naming of the newly made Creeper):

```
newCreep.Path = path;
timer = spawnTime;
```

### Use GameObject.Find()
Another way is to have the Creep find the Path when they are created. They can do so by searching the scene for a GameObject with the right name, then allocate this object to the Path parameter. To do this, place the following code in your CreepMove's Start() method (note: you'd need to remove the previous pattern to see how this works):

```
GameObject pathInScene = GameObject.Find("Path");
path = pathInScene.GetComponent<Path>();
```

### Use FindComponent<>()
A final approach is the same as the last, but looks for an object with the corresponding component (i.e, the Path class) instead. To try this, replace the code with:

```
Path = FindObjectOfType<Path>();
```

### Pick an approach!
Each of these approaches has pros and cons. Consider which one is best, thinking about scaleability, how error prone it is, and how it might help or hinder a level designer on your team. Your tutor will be asking you why you chose this method when marking. Before moving on, make sure you've documented your approach in your ERD.

### Checkpoint! Save, commit and push your work now.

## Step 2 – Create a tower prefab
We want to now create a tower object that "attacks" the creepers as they move along the path. Firstly, create your tower object:

* Use an empty game object as the parent object.
* Use a child cylinder object to represent the Tower's mesh. Similar to last week, position the child object so we can treat the tower's base as its origin point.
* Create a 2D circle (GameObject > 2D Object > Sprites > Circle) as a child of the tower to represent the area of effect. Rotate and position it so it sits at the base of the tower and give it an appropriate Trigger Collider (Note: DO NOT give it a Collider2D!). It should look something like this:

![An image of a tower object, with a sphere collider used to create the area of effect trigger.](images/Week4_Tower.png)

While inside the tower’s area of effect, the creep should lose health at a rate per second given by the tower’s strength. We want to make this a tuneable parameter of the tower so we can have towers of different strengths. Create a script for your Tower and add this paramater in.

We want to use one of the OnTrigger event handlers for this, similar to what we did in Week 3. Have a look at the documentation for [OnTriggerEnter](https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnTriggerEnter.html), [OnTriggerStay](https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnTriggerStay.html) and [OnTriggerExit](https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnTriggerExit.html). Determine which one to use.

Next, we need to consider which class this event handler should go on. Both objects have Trigger Colliders, so both will receive this event. For our purposes, we're going to place the event on the Tower itself. There are pros and cons to this, and you are encouraged to consider (and experiment) with another approach.

Firstly, represent the approach in the ERD, showing how the tower will need to access (and set) the health of the Creep based on their tunable damage amount.

If we look at the CreepHealth script, we'll see there's already a public method for taking damage:

```
public void TakeDamage(float damage)
{
    health -= damage;
    if (health <= 0)
    {
        Destroy(gameObject);
    }
}
```

We can use this method in our Tower script, but first we need to determine how the Tower is going to know it is colliding with a Creep. Again, there a few methods for handling this. In more complex situations, we may with to use [LayerMasks](https://docs.unity3d.com/ScriptReference/LayerMask.html). However, as we don't want Towers to collide with anything except for Creeps, we can instead simply place our Towers and Creeps on their own layers, as we did in COMP1150. Select Edit > Project Settings > Tags and Layers and fill in two of the User Layer slots (Preferable 6 and 7 to keep things clean). Name them appropriately for the Tower and for the Creep:

![An image of the Tags & Layers settings, with two new layers added: "Tower" and "Creep".](images/Week4_DefiningLayer.png)

Now, seelect "Physics" in the Project Settings menu and scroll down to the Layer Collision Matrix. We want to change it so that Creeps and Towers only collide with one another, so untick all other combinations for the two. It can be a bit confusing, but your result should look something like this:

![An image of the Tags & Layers settings, with two new layers added: "Tower" and "Creep".](images/Week4_CollisionMatrix.png)

Select your Tower object and your Creep prefab and set their Layers approporiately in the inspector.

In the Tower script's OnTrigger event (whichever one you implemented) call the CreepHealth's TakeDamage method. It should look something like this (allowing for your different choice of method and paramter names):

```
void OnTriggerStay(Collider collider)
{
    CreepHealth creep = collider.GetComponent<CreepHealth>();
    creep?.TakeDamage(strength * Time.deltaTime);
}
```

Is it working? If not, is there anything you have forgotten to do? Look back at the lecture and last week's practical and check everything that is necessary for trigger colliders to work has been implemented. Call your tutor over if you need some help. Once your tower is functional, create a prefab for it.

### Checkpoint! Save, commit and push your work now.

## Step 3 – Create a Tower spawner
We want to be able to spawn towers wherever the player left-clicks the mouse. There's a few things we need to do to get this working.

### Add the tower spawner to the ERD
What state does the tower need to keep track of? What events does it respond to? Don't worry if this isn't perfect - you can update this once you've figured it all out. It's a good idea to add something to help yourself plan.

### Mouse position and left-click inputs



* Add the tower spawner object to the ERD. What state does it keep track of? What events does it respond to?
The Input manager provides the Input.mousePosition property, which returns the mouse coordinates in screen space (i.e. pixel coordinates). We need to convert this into coordinates in the 3D world. We can do this using ray-casting.
* Check out the documentation for Camera.ScreenPointToRay. This method can turn a point in screen coordinates into a ray that starts at the camera position and points towards a particular screen position.
* Following the example code in lectures, use the Physics.Raycast method to fire a ray until it hits the Ground object in the scene (you will need to use a LayerMask to make sure you only hit the ground).
* The RaycastHit value contains a field point which is the point at which the ray hit the surface. Use this point to instantiate a new tower.

## To receive half marks for today, show your tutor:
TODO



 

### Prac Complete! Save, commit and push your work now.

## To receive full marks for today, show your tutor:




Step 4 (optional) – Create a tower variant
We’ve got one tower, now let’s add another tower that the player can spawn by right-clicking the mouse. For instance, consider what would be necessary to add a second tower type that slowed down the creeps while they were in its area of effect (without damaging them). 
How would you add this to your project? Which other objects would it need to interact with? How would you change the ERD to handle this?
If you’d like, you can consider a tower that effects the creeps in other ways, such as changing their size or colour. As with the slow tower, consider how you would add this to your project, as well as what additional classes you may need to write for the creep.
