# Gorilla Homes Mod Project
Unity project version 2019.3.15

![image](GHAssets/HouseDescriptor.png)

- Player Model Name : Name your model
- Author : Put your name here
- Lefhand : left hand bone
- Righthand : right hand bone
- Torso : the toro/hips of the rig
- Body : The mesh of the model
- Custom Color : Enables custom colors on your playermodel
- Game Mode Textures : Enables Game Mode Textures on your playermodel

Create an Empty GameObject and reset the transforms, attach the PlayermodelDescriptor to it.
Import your model with the rig from the .blend file, and attach it to the GameObject.
Apply your materials to the model
Assign the specified bones to the PlayermodelDescriptor

Specific bones highlighted in red (naming of the bones don't matter)

I'm using the PlayerModelMod_FBX_Example.fbx as an example (In the Assets folder)

![image](https://user-images.githubusercontent.com/65086429/172035609-9c94028e-437c-41ed-ac81-cb9d654a99d3.png)
![image](https://user-images.githubusercontent.com/65086429/172035618-70e3a86e-f8ec-4e92-a4c6-36894f6a1c13.png)


To test the playermodel in the editor,
Enable the OfflineRig_GorillaPlayer
Press play in the editor.
If the Playermodel was setup correctly, the playermodel should be aligned to the OfflineRig_GorillaPlayer with green shaped on the arms

![image](https://user-images.githubusercontent.com/65086429/172035710-3565cc07-5845-4917-bed4-df27cb0dccfa.png)

Press play again to exit.

Next step is to export the .gtmodel file

Go the Menu item "Assets"
Then "Create Player Model"

The Console will print the name of your player model.
The playermodel is exported to the PlayerModelOutput Folder

Drap the .gtmodel file into the PlayerAssets folder in the PlayerModels Mod folder

D O N E
