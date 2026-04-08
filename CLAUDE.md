# CLAUDE.md — 2D Platform Game

## 用户背景
- Unity 新手，以此项目作为长期学习项目
- 使用 Claude Code + Coplay MCP 辅助开发
- 编辑器：VS Code（C# Dev Kit + Unity 插件）
- Unity 版本：2022.3.62f3 LTS

---

## 项目概览

一个 2D 平台游戏，包含玩家控制、陷阱系统、检查点、可收集水果等核心功能。

**场景**：`Assets/Scenes/SampleScene.unity`（目前唯一场景）

---

## 项目架构

### 核心系统

| 系统 | 脚本 | 说明 |
|------|------|------|
| 全局管理 | `GameManager.cs` | 单例，管理玩家重生、水果计数、对象生成 |
| 玩家控制 | `player/player.cs` | 移动、跳跃、墙跳、击退、死亡 |
| 动画事件 | `player/PlayerAnimationEvents.cs` | 动画帧事件回调 |
| 检查点 | `CheckPoint/CheckPoint.cs` | 更新重生点 |
| 起点/终点 | `CheckPoint/StartPoint.cs` / `EndPoint.cs` | 关卡流程 |
| 陷阱 | `Trap_Saw.cs` / `Trap_FallingPlatform.cs` / `Trap_Trampoline.cs` / `Trap_Arrow.cs` | 各类陷阱逻辑 |
| 物品 | `Fruit.cs` | 可收集水果，支持随机外观 |
| 伤害 | `DamageTrigger.cs` | 触发玩家击退 |
| 死亡区域 | `DeadZone.cs` | 触发玩家死亡 |
| VFX 辅助 | `DestroyMeEvent.cs` | 动画事件销毁特效对象 |

### 目录结构

```
Assets/
├── Animations/       # 动画 Controller 和 Clip
│   ├── Checkpoint/
│   ├── Fruit/
│   ├── Player/
│   └── Trap/
├── Graphics/         # 美术资源（Sprite）
│   ├── Background/
│   ├── Enemies/
│   ├── Items/
│   ├── Main Characters/
│   ├── Terrain/
│   └── Traps/
├── Material/         # 物理材质（PhysicsMaterial2D）和普通材质
├── Prefab/           # 预制体
│   ├── Fruit/
│   └── *.prefab
├── Scenes/
│   └── SampleScene.unity
├── Scripts/
│   ├── player/
│   ├── CheckPoint/
│   └── *.cs
└── Tile Palette/     # 瓷砖调色板
```

---

## 代码规范

### 现有代码风格（保持一致）
- 类名：`PascalCase`（例外：`player` 类目前是小写，新代码用 PascalCase）
- 私有字段：`camelCase`，加 `[SerializeField]` 暴露到 Inspector
- 用 `[Header("...")]` 在 Inspector 中分组字段
- 逻辑块用 `#region` / `#endregion` 组织
- 协程命名：`XxxRoutine` 或 `XxxCoroutine`

### 输入系统
- 目前使用**旧版输入系统**（`Input.GetAxisRaw`、`Input.GetKeyDown`）
- 尚未迁移到 Unity Input System（New Input System）

### 动画驱动方式
- 通过 `Animator.SetFloat` / `SetBool` / `SetTrigger` 驱动
- 玩家动画参数：`xVelocity`、`yVelocity`、`isGrounded`、`isWallDetected`、`isKnocked`

---

## 关键设计决策

- **GameManager 单例**：全局唯一，通过 `GameManager.instance` 访问
- **玩家翻转**：用 `transform.Rotate(0, 180, 0)` 实现，不用 `localScale.x = -1`
- **碰撞检测**：用 `Physics2D.Raycast` 检测地面和墙壁，不用 `OnCollisionEnter2D`
- **重生流程**：玩家死亡时 Destroy 自身 → GameManager 协程延迟后 Instantiate 新玩家

---

## MCP 工具说明

已配置 Coplay MCP，可直接操作 Unity Editor：
- 读取 Console 错误：`check_compile_errors` / `get_unity_logs`
- 控制 Play Mode：`play_game` / `stop_game`
- 操作场景对象、动画、材质等

---

## 待学习 / 待实现的功能（学习路线）

- [ ] 迁移到 New Input System
- [ ] ScriptableObject 管理角色数据
- [ ] UI（HUD 显示水果数量、血量）
- [ ] 多场景管理 + 场景切换
- [ ] Audio Manager（音效、BGM）
- [ ] 存档系统
