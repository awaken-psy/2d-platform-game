# 2D Platform Game

一个基于 Unity 2022.3 LTS 的 2D 平台游戏学习项目，包含完整的玩家控制、敌人系统、陷阱系统、多关卡、UI 框架和皮肤切换等功能。

## 项目简介

本项目作为 Unity 长期学习项目，逐步实现平台游戏的核心机制：

- **玩家控制**：移动、跳跃（二段跳、墙跳、土狼跳、缓冲跳）、墙滑
- **多种敌人**：Chicken、Plant、Rino、Snail、Trunk、Mushroom，各自拥有独立 AI 和动画
- **多种陷阱**：锯（沿路点移动）、下落平台、弹跳垫、箭、火焰、尖刺球等
- **检查点与重生系统**：检查点记录重生位置，死亡后自动重生
- **可收集水果**：支持随机外观
- **皮肤系统**：4 种角色皮肤（MaskDude、NinjaFrog、PinkMan、VirtualGuy），使用 Animator Override Controller 实现动态切换
- **多关卡**：3 个关卡 + 主菜单 + 结束画面
- **完整 UI**：主菜单、关卡选择、皮肤选择、难度选择、游戏内 HUD、暂停菜单、淡入淡出特效、制作人员名单
- **难度系统**：可调节游戏难度

## 环境要求

- Unity 2022.3.62f3 LTS
- VS Code + C# Dev Kit + Unity 插件

## 根目录结构

### 需要关注的目录和文件

| 路径 | 说明 |
|------|------|
| `Assets/` | 游戏资源，所有开发工作都在这里 |
| `Packages/` | Unity 包依赖配置（`manifest.json`），进 Git |
| `ProjectSettings/` | Unity 项目设置（物理、输入、层级等），进 Git |
| `.editorconfig` | 统一代码格式规范 |

### 自动生成、无需关注（不进 Git）

| 路径 | 说明 |
|------|------|
| `Library/` | Unity 编译缓存 |
| `Temp/` | Unity 临时文件 |
| `Logs/` | Unity 运行日志 |
| `obj/` | C# 编译中间文件 |
| `UserSettings/` | 个人编辑器偏好（窗口布局等） |
| `*.csproj` / `*.sln` | VS Code IntelliSense 用的项目文件 |

### Assets 目录结构

```
Assets/
├── Animations/           动画 Controller 和 Clip
│   ├── Checkpoint/
│   ├── Enemy/
│   ├── Fruit/
│   ├── MainMenu/
│   ├── Player/
│   └── Trap/
├── Editor/               编辑器扩展脚本
├── Graphics/             美术资源（Sprite）
│   ├── Background/
│   ├── Enemies/
│   ├── Items/
│   ├── Main Characters/
│   ├── Other/
│   ├── Terrain/
│   └── Traps/
├── Material/             物理材质和普通材质
├── Prefab/               预制体
│   ├── Enemy/
│   ├── Fruit/
│   ├── Trap/
│   ├── UI/
│   └── VFX/
├── Scenes/
│   ├── MainMenu.unity    主菜单场景
│   ├── Level_1.unity     第 1 关
│   ├── Level_2.unity     第 2 关
│   ├── Level_3.unity     第 3 关
│   └── TheEnd.unity      结束画面
├── Scripts/              所有 C# 脚本
│   ├── player/           玩家控制
│   ├── CheckPoint/       检查点系统
│   ├── Enemy/            敌人 AI
│   ├── Fruit/            可收集物品
│   ├── General/          通用工具（伤害触发、死亡区域等）
│   ├── Trap/             陷阱逻辑
│   ├── UI/               UI 管理
│   ├── GameManager.cs    全局管理单例
│   ├── SkinManager.cs    皮肤管理
│   └── DifficultyManager.cs  难度管理
├── TextMesh Pro/         TextMesh Pro 插件资源
└── Tile Palette/         瓷砖调色板
```

## 核心脚本

### 全局管理

| 脚本 | 职责 |
|------|------|
| `GameManager.cs` | 全局单例，管理玩家重生、水果计数、对象生成 |
| `SkinManager.cs` | 角色皮肤管理与切换 |
| `DifficultyManager.cs` | 游戏难度调节 |

### 玩家系统

| 脚本 | 职责 |
|------|------|
| `player/player.cs` | 玩家移动、跳跃、墙跳、击退、死亡 |
| `player/PlayerAnimationEvents.cs` | 动画帧事件回调 |

### 敌人系统

| 脚本 | 职责 |
|------|------|
| `Enemy/Enemy.cs` | 敌人基类，共享巡逻、检测、受伤逻辑 |
| `Enemy/Enemy_Chicken.cs` | 小鸡 — 检测到玩家后冲刺 |
| `Enemy/Enemy_Plant.cs` | 食人花 — 定点远程攻击 |
| `Enemy/Enemy_Rino.cs` | 犀牛 — 冲撞攻击 |
| `Enemy/Enemy_Snail.cs` / `Enemy_Snailbody.cs` | 蜗牛 — 受击后缩壳并滑行 |
| `Enemy/Enemy_Trunk.cs` | 树干怪 — 远程射击 |
| `Enemy/Enemy_Mushroom.cs` | 蘑菇怪 — 基础巡逻 |
| `Enemy/Enemy_Bullet.cs` | 敌人发射的子弹 |

### 陷阱系统

| 脚本 | 职责 |
|------|------|
| `Trap/Trap_Saw.cs` | 沿路点移动的锯形陷阱 |
| `Trap/Trap_SawWayPoint.cs` | 锯形陷阱路点配置 |
| `Trap/Trap_FallingPlatform.cs` | 震动后下落的平台 |
| `Trap/Trap_Trampoline.cs` | 弹跳垫 |
| `Trap/Trap_Arrow.cs` | 箭形陷阱 |
| `Trap/Trap_Fire.cs` | 火焰陷阱 |
| `Trap/Trap_FireButton.cs` | 火焰触发按钮 |
| `Trap/Trap_SpikedBall.cs` | 尖刺球陷阱 |

### 物品与区域

| 脚本 | 职责 |
|------|------|
| `Fruit/Fruit.cs` | 可收集水果，支持随机外观 |
| `General/DamageTrigger.cs` | 触发击退的伤害区域 |
| `General/DeadZone.cs` | 触发死亡的区域 |
| `General/DestroyMeEvent.cs` | 动画事件销毁特效对象 |
| `CheckPoint/CheckPoint.cs` | 更新重生点 |
| `CheckPoint/StartPoint.cs` / `EndPoint.cs` | 关卡起点与终点 |

### UI 系统

| 脚本 | 职责 |
|------|------|
| `UI/UI_MainMenu.cs` | 主菜单界面 |
| `UI/UI_LevelSelection.cs` / `UI_LevelButton.cs` | 关卡选择 |
| `UI/UI_SkinSelection.cs` | 角色皮肤选择界面 |
| `UI/UI_Difficulty.cs` / `UI_DifficultyButton.cs` | 难度选择 |
| `UI/UI_InGame.cs` | 游戏内 HUD 和暂停菜单 |
| `UI/UI_FadeEffect.cs` | 场景切换淡入淡出特效 |
| `UI/UI_Credits.cs` | 制作人员名单 |
| `UI/AnimatedBackground.cs` | 动态背景动画 |

## 许可

[MIT License](LICENSE)
