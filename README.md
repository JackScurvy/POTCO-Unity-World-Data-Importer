# Pirates of the Caribbean Online - Unity Toolkit

> A Unity Editor toolkit for working with Panda3D - POTCO game assets and world data.

![Unity](https://img.shields.io/badge/Unity-6000.1.11f1-black?logo=unity)
![License](https://img.shields.io/badge/License-Educational-blue)
![Platform](https://img.shields.io/badge/Platform-Windows%20%7C%20Mac%20%7C%20Linux-lightgrey)

---
## ⚠️ Unity may take up to an hour to process all .egg files during the initial import, as it needs to cache all model data. Your patience is appreciated! ⚠️
---

## ✨ What is this?

This toolkit brings **Pirates of the Caribbean Online** into Unity, allowing you to import game worlds, export custom content, and build new experiences using authentic POTCO assets. This was created using the help of AI to expedite the process. So please excuse if the code is 🔥🗑️ but it works! 😎

The whole point of this is to bring out the creativity within our community, creating custom worlds and being able to share them or even potentially create little mini games using the POTCO assets within Unity. One thing that our community lacks from our sister community Toontown is the amount of user generated content. We need more of that!

## 🚀 Features

🗺️ **Import POTCO World Data** - Bring World Data files into Unity scenes  
🏴‍☠️ **Export to POTCO World Data** - Convert Unity scenes back to game-compatible files  
⛏️ **Generate Cave Systems** - Create interconnected caves                      
🎨 **Process EGG Files** - Import Panda3D models with materials and animations  
🎯 **ObjectList Detection** - Automatically classify and organize POTCO objects  

---

## 🛠️ Quick Start

### Requirements
- Unity 6000.1.11f1
- Universal Render Pipeline (URP)

Open the project in Unity 6000.1.11f1, then access tools via:
**Unity Menu Bar → POTCO → [Choose Tool]**

---

## 🎮 Tools Overview

### EGG File Importer
Automatically import Panda3D `.egg` files
- Geometry and animation processing
- Texture material support
- Bone hierarchy and skeletal data handling
- Vertex Colors Support (Really brings out the POTCO Style Graphics with them)

### World Data Importer
Import a `.py` World Data file from POTCO into Unity scenes
- Automatic object placement and hierarchy creation
- Select if to import Collisions & Holiday Props
- Choose if you want to apply the prop colors
- Importing Speed
- Coordinate system conversion

### World Data Exporter  
Export Unity scenes back to POTCO WorlData compatible format
- Objectlist filtering and type detection
- Coordinate system conversion between Unity and Panda3D formats
- Debugging tools for scene analysis

### Procedural Cave Generator
Create interconnected cave systems using cave pieces
- Connector-based assembly system
- Weighted randomization for varied layouts
- Built-in presets and custom configuration saving


---

## 📖 Basic Usage

### Import a World
1. Open `POTCO → World Data Importer`
2. Click "Select World .py File"
3. Choose your world file and click "Import"
4. Watch as your POTCO world appears in Unity!

### Generate a Cave
1. Open `POTCO → Procedural Cave Generator`  
2. Load the "Padres Del Fuego Theme" preset
3. Set cave length (try 10-15 pieces)
4. Click "Generate Cave" and watch it build!

### Export Your Scene
1. Add objects to your scene with POTCO components
2. Open `POTCO → World Data Exporter`
3. Configure export settings
4. Click "Export World Data" to create a `.py` file

All props must have a parent object to be able to export, either import a pre created scene and then drag and drop props into it after you're done select them all and attach them to the parent GameObject or parent all the props to 1 prop.

Imported WorldData will always spawn in 0 0 0

---

## 🎯 POTCO Object System

Objects in your scene can be automatically detected and classified:

- **Buildings** - Taverns, forts, houses
- **Props** - Barrels, crates, furniture, decorations  
- **Lighting** - Torches, lanterns, dynamic lights
- **Environment** - Trees, rocks, water features
- **Interactive** - Spawn points, connectors, collision zones

The system automatically assigns the correct object type, generates unique IDs, and prepares objects for export.

---

## 🖼️ Screenshots

### Cave Generation in Action
https://github.com/user-attachments/assets/b4187ae0-391a-410d-99f8-5706204fa792

### Custom Scenes
<img width="1607" height="1091" alt="image" src="https://github.com/user-attachments/assets/6a8616a4-4b24-401c-964a-fefe9b388d31" />


### POTCO Tortuga Tavern
<img width="3829" height="1890" alt="image" src="https://github.com/user-attachments/assets/e0e57ff1-6b74-4bf9-a862-c4084f12340f" />


### Tools Windows
<img width="3143" height="1356" alt="Unity_CTd6HhjjwX" src="https://github.com/user-attachments/assets/58e3240e-da14-47e3-8cd0-c2b454ea57b9" />

---

## ⚠️ Known Issues

- Some dragged objects to the scene may not be given the ObjectList script, in the exporter browser you can select all the objects in the scene and then click "Add POTCOTypeInfo to Selected Objects"
- Alpha transparency RGB's not working
- Large cave systems may experience occasional piece overlapping
- Complex EGG files with extensive bone data may require additional processing time

---

## 📚 Educational Use

This project is designed for **educational and research purposes**:

❌ **Original POTCO assets remain property of their owners**  

---

## 🤝 Contributing

Interested in improving the toolkit? 

1. Fork the repository
2. Create a feature branch
3. Make your improvements
4. Submit a pull request with a clear description

All contributions should maintain the educational focus and respect the original game's intellectual property.

---

## 🏴‍☠️ Set Sail!

Transform Unity into your personal POTCO playground environment and start creating today!

**Happy sailing, matey!** ⚓
