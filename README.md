# Pirates of the Caribbean Online - Unity Toolkit

> A Unity Editor toolkit for working with Panda3D - POTCO game assets, world data, and level creation.
---
<p align="center">
Got any questions?
  </p>
<p align="center">
  <a href="https://discord.gg/Dr6YR8HkPJ" target="_blank">
    <img alt="Join our Discord"
      src="https://img.shields.io/badge/Join%20our%20Discord-5865F2?style=for-the-badge&logo=discord&logoColor=white">
  </a>
</p>


---
## ⚠️ Unity may take up to an hour to process all .egg files during the initial import, as it needs to cache all model data. Your patience is appreciated! ⚠️
---

## ✨ What is this?

This toolkit brings **Pirates of the Caribbean Online** into Unity, allowing you to import game worlds, export custom content, build new experiences, and create levels using authentic POTCO assets. This was created using the help of AI to expedite the process. So please excuse if the code is 🔥🗑️ but it works! 😎

The whole point of this is to bring out the creativity within our community, creating custom worlds and being able to share them or even potentially create little mini games using the POTCO assets within Unity. One thing that our community lacks from our sister community Toontown is the amount of user generated content. We need more of that!

## 🚀 Features

### Core Tools
🗺️ **World Data Importer** - Import POTCO World Data files into Unity scenes  
🏴‍☠️ **World Data Exporter** - Export Unity scenes back to game-compatible files  
⛏️ **Procedural Cave Generator** - Create interconnected cave systems with presets  
🎨 **Advanced EGG File Importer** - Import Panda3D models with multi-texture support  

### Level Creation & Management
🏗️ **Level Editor (Prop Browser)** - Visual asset browser and placement tool  
🎯 **ObjectList Detection** - Automatic POTCO object classification and organization  
🖼️ **Custom Thumbnails** - Auto-generated preview images for all assets  
📂 **Smart Categorization** - Folder-based and ObjectList-based organization  

---

## 🛠️ Quick Start

### Requirements
- Unity 6000.1.11f1
- Universal Render Pipeline (URP)

Open the project in Unity 6000.1.11f1, then access tools via:
**Unity Menu Bar → POTCO → [Choose Tool]**

---

## 🎮 Tools Overview

### 🎨 EGG File Importer Manager
Advanced Panda3D `.egg` file processing with comprehensive filtering options:
- **Multi-Texture Support** - Specialized overlay system for tiling ground textures (Very Broken)
- **Advanced Filtering** - Skip footprints, LOD control, skeletal data filtering
- **Folder Exclusions** - Default exclusions for gui, effects, sea, sky, textureCards
- **Manual Import Control** - Disable auto-import to prevent Unity startup delays
- **Startup Import Prompt** - Modal dialog for selective batch importing

### 🗺️ World Data Importer
Import `.py` World Data files from POTCO into Unity scenes:
- Automatic object placement and hierarchy creation
- ObjectList data integration for proper object classification
- Collision and Holiday Props import options
- Prop color application toggle
- Coordinate system conversion (Panda3D ↔ Unity)
- Import speed optimization

### 🏴‍☠️ World Data Exporter  
Export Unity scenes back to POTCO-compatible format:
- Advanced ObjectList filtering and type detection
- Coordinate system conversion between Unity and Panda3D formats

### ⛏️ Procedural Cave Generator
Create interconnected cave systems using connector-based cave pieces:
- Connector-based assembly system (`cave_connector_*` transform markers)
- Weighted randomization for varied layouts
- Built-in theme presets (Padres Del Fuego Theme)
- Custom configuration saving/loading (JSON format)

### 🏗️ Level Editor (Prop Browser)
Visual asset management and level creation tool:
- **Thumbnail Grid View** - Visual browsing of all POTCO assets
- **Smart Categorization** - Multiple organization methods:
  - Folder-based categorization
  - ObjectList-based categorization (Buildings, Props, Lighting, etc.)
  - Uncategorized item handling
- **Quick Placement** - Drag-and-drop asset placement in scenes
- **Search and Filtering** - Find assets quickly across categories
---

## 📖 Basic Usage

### Import a World
1. Open `POTCO → World Data Importer`
2. Click "Select World .py File"
3. Choose your world file and click "Import"
4. Watch as your POTCO world appears in Unity!

### Use the Level Editor
1. Open `POTCO → Level Editor`
2. Browse assets using the thumbnail grid
3. Switch between categorization modes (Folder/ObjectList)
4. Drag assets directly into your scene
5. Assets are automatically categorized and ready for export
6. Select "🎯Surface" for any placed models either from importing or dragging out to contain collisions to be able to easily drag and drop props on top seemlessly will cause the world importer to take longer


### Generate a Cave
1. Open `POTCO → Procedural Cave Generator`  
2. Load the "Padres Del Fuego Theme" preset
3. Set cave length (try 10-15 pieces)
4. Click "Generate Cave" and watch it build!

### Import EGG Files
1. Open `POTCO → EGG Importer Manager`
2. Configure filtering options (skip footprints, LOD settings, etc.)
3. Choose folders to exclude (gui, effects recommended)
4. Use "Import All EGG Files" or selective import
5. View statistics in the Statistics tab

### Export Your Scene
1. Add objects to your scene (use Level Editor for easy placement)
2. Open `POTCO → World Data Exporter`
3. Configure export settings and filters
4. Click "Export World Data" to create a `.py` file

**Important Notes:**
- All props must have a parent object to export properly
- Imported WorldData always spawns at (0,0,0)
- Use "Add ObjectListInfo to Selected Objects" for manual object classification

---

## 🖼️ Screenshots

### Level Editor in Action
https://github.com/user-attachments/assets/9ebcb461-3981-4f06-8db6-6236e9a3b744

### Cave Generation in Action

https://github.com/user-attachments/assets/39e63113-1cf6-457f-9515-f33dab15f0b0

### World Importer in Action
https://github.com/user-attachments/assets/b4187ae0-391a-410d-99f8-5706204fa792

### Custom Scenes
<img width="1607" height="1091" alt="image" src="https://github.com/user-attachments/assets/6a8616a4-4b24-401c-964a-fefe9b388d31" />

### POTCO Tortuga Tavern
<img width="3829" height="1890" alt="image" src="https://github.com/user-attachments/assets/e0e57ff1-6b74-4bf9-a862-c4084f12340f" />

### Tools Windows
<img width="3143" height="1356" alt="Unity_CTd6HhjjwX" src="https://github.com/user-attachments/assets/58e3240e-da14-47e3-8cd0-c2b454ea57b9" />

---

## ⚠️ Known Issues

- Some manually dragged objects may not receive ObjectListInfo components automatically - use "Add ObjectListInfo to Selected Objects" in the exporter
- Alpha transparency is not working
- Large cave systems may occasionally experience piece overlapping
- Complex EGG files with extensive bone data may require additional processing time



---

## 📚 Educational Use

This project is designed for **educational and research purposes**:

❌ **Original POTCO assets remain property of their owners**  

---

---

## 🏴‍☠️ Set Sail!

Transform Unity into your personal POTCO playground and level creation environment. With the new Level Editor, asset browser, and enhanced import systems, creating custom POTCO content has never been easier!

**Create, explore, and share your pirate adventures!** ⚓

---

*Built with passion for the POTCO community. Fair winds and following seas!*
