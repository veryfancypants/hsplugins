This is a work-in-progress AI/HS2 port of the Timeline plugin from https://bitbucket.org/Joan6694/hsplugins, and a fork of the AIPE plugin of the same author.

KNOWN ISSUES:

* The UI may not look quite right, because I had to do some major changes to get it to show up properly.
* Not all standard interpolables may be working. 

NEW FEATURES:

* A "Blend shapes - everything" interpolable. Set up the entire blend shape (or load a preset) and apply it with one click. 
* 'Reset all' button in the advanced bones editor.

WORK IN PROGRESS:

* Global colliders. Lets all colliders in the scene interact with all dynamic bones in the same scene. Lets you do things like:

** let one character move another character's hair (the hand would actually push the hair instead of passing straight through), or handle their breasts

** implement a blanket that would take shape of character (or characters) under it, without being designated that character's clothes accessory

** put a collider into a chair in Unity and prevent a female character's skirt from clipping straight through that chair (or at least you could do that if AI/HS2 supported plane and box colliders; due to a quirk of its dynamic bone implementation, most dynamic bones are only affected by  capsule colliders, and working around it would be a nontrivial task.)

** put a collider into a sex toy in Unity and have that collider automatically do things to the character 

Basic implementation is in place, but it's a major change and there are probably 50 unintended side effects that I didn't consider, so YMMV. 
May cause performance issues if you try to load more than 3-5 characters in one scene.

* An "All bones" interpolable. The idea is to have _one_ interpolable that controls the entire character shape. Instead of manually handling 13 IK interpolables and possibly dozens of FK interpolables, you pose the character, add a keyframe with one mouseclick, and you're done. 
Unfortunately, it's easier said than done. So far, it's very raw, it's _kind_of_ working in pure-IK mode except there's no saving/loading, something is broken with FK bones, and don't even think of interpolating between states with different sets of active IK/FK bones. 

OTHER CHANGES:

* For now, in a break from the master version of AIPE, blend shapes are no longer linked. Instead of setting the head blend shape and moving on to other things, you need to set head blend shape, matching eyebrows blend shape, matching tears blend shape, etc.
* Loading a timeline single file will replace any existing rows with whatever is in the file (in the original version, existing rows were left untouched.) 

For questions/comments, I am (sometimes) reachable via Discord as hamster#2443.