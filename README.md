# 2D Platform Game

一个基于 Unity 2022.3 LTS 的 2D 平台游戏学习项目，包含完整的玩家控制、陷阱系统、检查点和可收集物品。

## 项目简介

本项目作为 Unity 长期学习项目，逐步实现平台游戏的核心机制：

- 玩家移动、跳跃（二段跳、墙跳、土狼跳、缓冲跳）、墙滑
- 多种陷阱（锯、下落平台、弹跳垫、箭）
- 检查点与重生系统
- 可收集水果（支持随机外观）

## 环境要求

- Unity 2022.3.62f1 LTS
- VS Code + C# Dev Kit + Unity 插件

## 根目录结构

### 需要关注的目录和文件

| 路径 | 说明 |
|------|------|
| `Assets/` | 游戏资源，所有开发工作都在这里 |
| `Packages/` | Unity 包依赖配置（`manifest.json`），进 Git |
| `ProjectSettings/` | Unity 项目设置（物理、输入、层级等），进 Git |
| `.vscode/` | VS Code 工作区配置（插件推荐、调试配置），进 Git |
| `.editorconfig` | 统一代码格式规范，进 Git |
| `.gitignore` | Git 忽略规则，进 Git |
| `README.md` | 项目说明文档，进 Git |
| `LICENSE` | MIT 开源协议，进 Git |
| `CLAUDE.md` | Claude Code 项目上下文说明，进 Git |

### 自动生成、无需关注

| 路径 | 说明 |
|------|------|
| `Library/` | Unity 编译缓存，自动生成，不进 Git |
| `Temp/` | Unity 临时文件，自动生成，不进 Git |
| `Logs/` | Unity 运行日志，自动生成，不进 Git |
| `obj/` | C# 编译中间文件，自动生成，不进 Git |
| `UserSettings/` | 个人编辑器偏好（窗口布局等），不进 Git |
| `*.csproj` / `*.sln` | VS Code IntelliSense 用的项目文件，Unity 自动生成，不进 Git |

### Assets 目录结构

```
Assets/
├── Animations/         动画 Controller 和 Clip
│   ├── Checkpoint/
│   ├── Fruit/
│   ├── Player/
│   └── Trap/
├── Graphics/           美术资源（Sprite）
│   ├── Background/
│   ├── Enemies/
│   ├── Items/
│   ├── Main Characters/
│   ├── Terrain/
│   └── Traps/
├── Material/           物理材质和普通材质
├── Prefab/             预制体
│   └── Fruit/
├── Scenes/
│   └── SampleScene.unity   主场景
├── Scripts/            所有 C# 脚本
│   ├── player/         玩家控制
│   ├── CheckPoint/     检查点系统
│   └── *.cs            陷阱、物品、管理器等
└── Tile Palette/       瓷砖调色板
```

## 核心脚本

| 脚本 | 职责 |
|------|------|
| `GameManager.cs` | 全局单例，管理重生、水果计数 |
| `player/player.cs` | 玩家移动、跳跃、击退、死亡 |
| `CheckPoint/CheckPoint.cs` | 更新重生点 |
| `Trap_Saw.cs` | 沿路点移动的锯形陷阱 |
| `Trap_FallingPlatform.cs` | 震动后下落的平台 |
| `Trap_Trampoline.cs` | 弹跳垫 |
| `Trap_Arrow.cs` | 箭形陷阱 |
| `Fruit.cs` | 可收集水果 |
| `DamageTrigger.cs` | 触发击退的伤害区域 |
| `DeadZone.cs` | 触发死亡的区域 |
