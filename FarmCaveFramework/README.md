To add a cave choice, use Content Patcher to modify the "farm_cave_choices" asset, which uses the following format:

# `farm_cave_choices` asset
A string to CaveChoice dictionary.

# CaveChoice
A model with these fields:

| Field name | Use |
| --- | --- |
| id | A string ID for the choice |
| choice | The text to display in the response for this choice |
| description | The text of the description Demetrius will say about the choice |
| object | A list of CaveObjects to put in the cave, similarly to mushroom boxes |
| resources | A list of CaveResources to put in the cave, similarly to fruit |
| resourceChance | The chance to spawn a resource each night. If the chance succeeds, it will try again over and over until it fails, generating a new resource each time. |
| animations | A list of CaveAnimations to display |
| periodics | A list of CavePeriodicEffects to display |
| ambientLight | A color tint for the ambient light |

# CaveObject
| Field Name | Use |
| --- | --- |
| id | The id of the BigCraftable-type item to spawn |
| X | The X coordinate |
| Y | The Y coordinate |

# CaveResource
| Field Name | Use |
| --- | --- |
| id | The id of the Object-type item to spawn randomly |
| weight | An integer determining how often an item spawned should be this item. Weight is relative to the weights of the other items, so more common items should have higher values compared to less common items. If all of them are set to 15, for example, they are all the same chance, so setting one to 16 will only make it slightly more common |
| min, max | The stack size of the item to spawn |

# CaveAnimation
I don't feel like documented all of this so here's the list of fields:
```
       int index;
       int X;
       int Y;
       string sourceFile;
       int sourceX;
       int sourceY;
       int width;
       int height;
       float interval;
       int length;
       int loops;
       bool flicker;
       bool flipped;
       float alphaFade;
       Color color = Color.White;
       int delay;
       float scale;
       float scaleChange;
       float rotation;
       float rotationChange;
       bool light;
       float lightRadius;
       float loopTIme;
       float range;
       float motionX;
       float motionY;
       bool bottom;
       bool right;
       bool randomX;
       bool randomY;
```

# CavePeriodicEffect
```
 List<CaveSound> randomSounds = new List<CaveSound>();
 List<CaveSound> repeatedSounds = new List<CaveSound>();
 List<CaveAnimation> animations = new List<CaveAnimation>();
 List<string> specials = new List<string>();
 float chance = 0.2f;
```

# CaveSound
```
        string id;
        float chance;
        int count;
        int pitch;
        int delayMult;
        int delayAdd;
```

