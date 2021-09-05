This is a work-in-progress AI/HS2 port of the Timeline plugin from https://bitbucket.org/Joan6694/hsplugins, and a fork of the AIPE plugin of the same author.

KNOWN ISSUES:

* Something is going wrong with loading materials for the Timeline UI. Unity fails to load materials from Joan's asset bundle (see Timeline.cs lines 529, 595, 722), I had to substitute a second bundle with my own mats. 
Can't tell if those materials are just missing from the old bundle, or there's some sort of version mismatch (AI uses Unity 2018.4, the old bundle is made with Unity 5.3). May be a shader version issue. 
* Even with substituted materials, the UI does not look quite right (e.g. curves aren't drawn in the keyframe window).
* Not all standard interpolables may be working; in particular, tears interpolation is missing

NEW FEATURES:

* An "All bones" interpolable. The idea is to have _one_ interpolable that controls the entire character shape. Instead of manually handling 13 IK interpolables and possibly dozens of FK interpolables, you pose the character, add a keyframe with one mouseclick, and you're done. 
Unfortunately, it's easier said than done. So far, it's _kind_of_ working in pure-IK mode except there's no saving/loading, something is broken with FK bones, and don't even think of interpolating between states with different sets of active IK/FK bones. 
* A "Blend shapes - everything" interpolable. Same idea: set up the entire blend shape (or load a preset) and apply it with one click. 
** Note: for now, in a break from the master version of AIPE, blend shapes are no longer linked. Instead of setting the head blend shape and moving on to other things, you need to set head blend shape, matching eyebrows blend shape, matching tears blend shape, etc.
* [EXPERIMENTAL] Global colliders. Lets all colliders in the scene interact with all dynamic bones in the same scene. Lets you do things like:
- let one character move another character's hair (the hand would actually push the hair instead of passing straight through)
- implement a blanket that would take shape of character (or characters) under it, without being designated that character's clothes accessory
- put a collider into a chair in Unity and prevent a female character's skirt from clipping straight through that chair
- put a collider into a sex toy in Unity and have that collider automatically do things to character's genitals 
Basic implementation is in place, but it's a major change and there are probably 50 unintended side effects that I didn't consider, so YMMV.