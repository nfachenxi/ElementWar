---
rule_type: architecture
priority: high
applies_to: scripts
phase: any
---

# 程序集与目录分层

## 现状基线

| 项 | 现状 |
| --- | --- |
| 程序集 | **无 asmdef**，全部项目代码编入默认程序集 `Assembly-CSharp`（Editor 代码进 `Assembly-CSharp-Editor`） |
| 源码根 | `Assets/Scripts/` |
| 场景 | `Assets/Scenes/TestScene.unity`（当前唯一项目场景） |
| 管线资产 | `Assets/Settings/`（URP-Balanced / URP-HighFidelity 及 Renderer） |
| 第三方资源 | `Assets/ThirdParty/`（208.4 MB，只读，见 spec-03）；`Assets/Resources/` 已于 2026-10-06 移除 |

## 目录职责

| 目录 | 职责 |
| --- | --- |
| `Assets/Scripts/Base/` | 跨域基类：`StateBase`、`EnemyStateBase`、`PlayerStateBase`、`EnemyBase`、`SingleMonoBase<T>` |
| `Assets/Scripts/Utils/` | 通用工具：`StateMachine`、`IStateMachineOwner`、`MonoManager` |
| `Assets/Scripts/Player/` | 玩家域：`PlayerController`、`PlayerModel`、`PlayerWeapon`、`PlayerWeaponBullet` |
| `Assets/Scripts/Player/State/` | 玩家状态：`PlayerIdleState`、`PlayerMoveState`、`PlayerAimingState`、`PlayerHoverState` |
| `Assets/Scripts/Enemy/` | 敌人域：`ZombieEnemy` |
| `Assets/Scripts/Enemy/State/` | 敌人状态：`ZombieIdleState`、`ZombieMoveState`、`ZombieAttackState`、`ZombieDeadState` |
| `Assets/Scripts/Crosshair/` | 准星域：`CrosshairSettings`（ScriptableObject）、`CrosshairUI` |
| `Assets/Scenes/` | 项目场景，新增场景放这里 |
| `Assets/Settings/` | URP 管线资产、Input System（含生成物）、Volume Profile、`CrosshairSettings.asset` |
| `Assets/ThirdParty/` | 第三方美术资产归口（只读，208.4 MB）：`Animations`／`EffectCore`／`Effects`／`Materials`／`Models`／`Textures`／`YSA Toon` |
| `Assets/Plugins/` | 第三方代码插件（只读，12.2 MB，仅 `Roslyn`） |
| `Assets/Low Poly FPS Pack/`、`Assets/MMD4Mecanim/` | 第三方资产包（只读，不入库） |
| ~~`Assets/Resources/`~~ | **已于 2026-10-06 移除**（原 182.8 MB 常驻）；不再重建，见 [spec-03](spec-03-assets-scene-prefab.md) |

## 规则

### 1. 新脚本归位

业务脚本进对应域目录（`Player/`、`Enemy/`、`Crosshair/`，按系统名新增域目录）。**只有确实跨域复用**才进 `Base/` 或 `Utils/`。

判定标准：两个及以上域同时依赖同一逻辑，才算跨域复用。仅「以后可能会用到」不算。

### 2. 域内状态放 `State/`

状态类统一放 `<域>/State/`，继承该域的 `*StateBase`（`PlayerStateBase` / `EnemyStateBase`）。

### 3. 不擅自引入架构

不新增 asmdef、不引入第三方状态机／DI 框架／事件总线／资源框架——以现有自研框架为准。确有需要时先提案，说明收益、迁移成本与对编译时间的影响，等确认后再动。

> asmdef 会改变程序集边界与包引用关系，并影响 `dotnet build` 验证方式（见 spec-09），属于「大型」改动。

### 4. 不新增第二套并行机制

同一能力只保留一个入口。已存在的机制：

| 能力 | 唯一入口 |
| --- | --- |
| 状态机 | `Utils/StateMachine` + `IStateMachineOwner` |
| 单例 | `Base/SingleMonoBase<T>` |
| 帧驱动／协程调度 | `Utils/MonoManager` |
| 玩家全局入口 | `PlayerController.INSTANCE` |
| 输入 | `Assets/Settings/InputSystem/MyInputSystem`（生成类） |
| 准星配置 | `CrosshairSettings`（ScriptableObject） |

### 5. 场景边界

项目场景只在 `Assets/Scenes/`。第三方 Demo 场景（`Assets/Low Poly FPS Pack/Demo_Scenes/`，共 45 个）不作为开发场景，也不复制到 `Assets/Scenes/` 充当开发场景。

### 6. 命名空间与目录一致

命名空间 = 目录路径（`Base` / `Utils` / `Player` / `Player.State` / `Enemy` / `Enemy.State` / `Crosshair`）。详见 [spec-07-naming-style.md](spec-07-naming-style.md)。

## 变更记录

- 2026-10-06：初版。目录与程序集现状据实测量，未新增 asmdef。
- 2026-10-06：**布局调整**。`Assets/Resources/` 清空删除，`Assets/ThirdParty/` 成为第三方美术资产归口（`Plugins/EffectCore`、`Plugins/YSA Toon` 迁出，`Plugins/` 只留 `Roslyn`）；`CrosshairSettings.asset` 进入 `Assets/Settings/`，其 Inspector 引用随 GUID 保留。`Assets/Scripts/` 域分层未变动。

## 相关分册

- [spec-02-lifecycle-performance.md](spec-02-lifecycle-performance.md) 生命周期、帧驱动与性能
- [spec-07-naming-style.md](spec-07-naming-style.md) 命名与代码风格
