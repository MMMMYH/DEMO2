# Unity I Wanna 游戏项目

## 🎯 项目概述
基于Unity 2022.3 LTS开发的经典I Wanna风格平台跳跃游戏

## 📁 项目结构
```
Assets/
├── Scripts/
│   ├── Player/
│   │   ├── PlayerController.cs
│   │   ├── PlayerMovement.cs
│   │   └── PlayerAnimator.cs
│   ├── GameManagement/
│   │   ├── GameManager.cs
│   │   ├── LevelManager.cs
│   │   └── UIManager.cs
│   ├── Level/
│   │   ├── LevelData.cs
│   │   ├── TileManager.cs
│   │   └── Hazard.cs
│   └── Editor/
│       ├── LevelEditor.cs
│       └── TileMapEditor.cs
├── Prefabs/
│   ├── Player.prefab
│   ├── Tiles/
│   └── UI/
├── Sprites/
│   ├── Player/
│   ├── Tiles/
│   └── UI/
├── Scenes/
│   ├── MainMenu.unity
│   ├── Game.unity
│   └── LevelEditor.unity
└── Materials/
    └── Sprites/
```

## 🛠️ 核心功能
- 精确的2D物理控制
- 瓦片地图系统
- 关卡编辑器
- 死亡重生机制
- UI管理系统

## 🎮 技术特性
- Unity 2D Physics
- Tilemap System
- Custom Editor Tools
- ScriptableObject数据管理
- Event System架构
