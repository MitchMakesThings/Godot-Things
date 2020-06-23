Spell 'Splosion Game
====================

Project Layout
--------------
* assets
    * Contains all of the art assets
* screenshots
    * Holds some debug / explanatory screenshots to help explain how the destruction system is working. See below.
* src
    * character
        * Contains wizard scene for the player character, and the script to control them.
        * Of particular note in [wizard.gd](src/character/wizard.gd) is the *weapon_projectile* export variable. This points to the weapon the character should use, from the weapons folder.
    * destruction
        * The majority of the scripts from this experiment. This will be better explained in the youtube tutorial. \
        TLDR: Add the Destructible scene as a child of a Sprite you want to be able to destroy (ie, your terrain). Add parent_material to the Sprite. When you want to destroy something call the "destroy" method of the "destructibles" group. \
        ie, get_tree().call_group("destructibles", "destroy", global_position, explosion_radius) - see [The Green Slime Weapon](src/weapons/Slime-Green.gd) as an example.
    * levels
        * Contains my example level. Nothing exciting here.
    * scripts
        * A catch-all folder for ad-hoc scripts.
        * Currently only contains a camera script to let the player right-click-and-drag to pan the camera around the scene.
    * weapons
        * Houses the code for the projectiles the wizard can launch.
        * Slime-Green gives an example of a simple exploding projectile.
        * Explosion is a simple scene to display an explosion animation.


Destruction System
------------------
The destruction system is broadly broken into two separate components - the collision destruction, and the visual destruction.

### Collision Destruction
This is the more complex of the two, but sets the stage for how the Visual Destruction system works. \
The collision part of the destruction system handles integration with the Godot physics systems. This is further broken into two different mechanisms - image based, and geometry based.

#### Image-based collisions
When the collision system first intitialises (in _ready) it copies all information from its parent to populate the Destruction system Viewport. This gives us the starting shape for our destruction mask, and its what allows us to power the Visual Destruction system.
We also use this starting Viewport image to generate our intial collision polygons (from the *rebuild_collisions_from_image* method) - this uses the built-in [Bitmap.opaque_to_polygons method](https://docs.godotengine.org/pt_BR/latest/classes/class_bitmap.html#class-bitmap-method-opaque-to-polygons)

We use the image-based collision generation when the system intialises because it gives us image-accurate collision shapes. However, regenerating the entire maps worth of collision polygons from our Viewport every time a destruction event occurs is too slow for real-time performance. \
Although the *opaque_to_polygons* method allows to specify a subset of an image to generate collisions for, this isn't as straightforward as you might think. When you trigger some destruction you'd need to generate the new collision shapes for that area of the image, and then replace the existing collision shapes for that part of the map with your newly-generated collision. You'd then probably want to merge the old and new collision polygons together.

This leads us to the second part of collision destruction.

#### Geometry-based collisions
After our initial collisions have been created from the level images, we want a fast way to destroy small parts of our map. This allows us to recalculate the collision shape of the map in real-time. The Geometry-based collisions logic is handled via the *rebuild_collisions_from_geometry* method.

This makes extensive use of the [Geometry class](https://docs.godotengine.org/pt_BR/latest/classes/class_geometry.html#class-geometry-method-clip-polygons-2d) that's built in to Godot.

We simply create a rough circle and 'clip' that against the existing collision shapes. This gives us back the parts of the existing collisions shapes **except** our circle. The existing collision shapes are then updated accordingly. If the destruction generates an 'island' (ie, we destroy the only bridge connecting two areas, causing the single collision shape to be broken in two) we will create a new collision shape to represent it.




Visual Destruction
------------------
The visual destruction system is a little simpler than the collision system.
It's primarily driven by the [parent_material shader](src/destruction/parent_material.shader) \
This shader should be assigned to a Sprite representing the terrain of the level. The shader takes an image as a parameter (*destruction_mask*) and multiplies it against the alpha of the terrain sprite. This allows the destruction mask to determine which bits of our terrain to show.

The shader parameter is populated from our [Destructible script](src/destruction/Destructible.gd) - specifically the *republish_sprite* method. \
Our [Destructible scene](src/destruction/Destructible.tscn) has a special viewport which is not visible. This is rendered onto a Sprite via a ViewportTexture and the sprite data is then used to populate the destruction mask that is passed to the shader.

The viewport is configured to only render when we call it from code. It also never clears what has been drawn to it, so anything displayed there will become a permanent part of the destroyed terrain passed into the parent_material shader. \
We populate the viewport using the [Destructible Circle node](src/destruction/Circle.gd). This implements custom drawing so that explosions of different sizes can be pushed into the Viewport, and from there to the parent_material shader, where it is subtracted from the Sprite showing the terrain.