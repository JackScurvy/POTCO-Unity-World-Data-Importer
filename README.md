# POTCO Unity World Data Importer
Imports Pirates of the Caribbean Online's World Data into Unity

WorldData & Phase files were taken from an available Pirates Online Rewritten repo on github.

All Models were converted using  Panda3D's bam2egg then imported using the Blender egg importer tool and exported into FBX. There may be some issues on rotations and sizes from exporting into fbx.
All models LODS were removed using Panda3D's egg-trans tool, collisions have also been stripped away to help with visuals within Unity.

Click on the top menu labled "POTCO" then select "World Data Importer"
Click on "Select World .py File" then select which file to import.

Importing the biggest Island which is Padres Del Fuego takes around 30 seconds for me, mostly anything else is instant.

# ***Issues***
Some objects are imported at a small noticible inclination, have not been able to pin point on why
may be due to parent objects who have child objects within it.

Alpha Textures do not work
Textures for some models are broken such as islands

Possible issues with FBX Size & Rotation

Its not perfect at importing

Props with bones aren't working
________________________________________________________________________

# ***To Maybe Add*** 
World Data Exporter

# Images

 Mansion Interior
<img width="2756" height="1190" alt="image" src="https://github.com/user-attachments/assets/0bc48425-a4c8-4776-bc17-4fcc4a55d750" />

Another Bilgewater concept by the Original Pirates Online Team, console tells you when a model fails to import.
<img width="2763" height="1644" alt="image" src="https://github.com/user-attachments/assets/adaddf6f-5ec0-4484-8188-2a0e98b6a7d8" />

Kingshead with some broken textures
<img width="2768" height="1191" alt="image" src="https://github.com/user-attachments/assets/85458c8d-6f7d-48af-b757-96ed46af1cf0" />

<img width="2761" height="1129" alt="image" src="https://github.com/user-attachments/assets/0fb7e76d-fa1f-48d0-bb10-f96840525b09" />
