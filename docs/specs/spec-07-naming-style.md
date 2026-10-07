---
rule_type: naming
priority: high
applies_to: all
phase: any
---

# 命名与代码风格

## 规则

### 1. 命名空间 = 目录路径

`Assets/Scripts/Base/` → `namespace Base`；`Assets/Scripts/Player/State/` → `namespace Player.State`。新增目录即新增命名空间，不加多余层级。

现有命名空间：`Base`、`Utils`、`Player`、`Player.State`、`Enemy`、`Enemy.State`、`Crosshair`。

### 2. 文件名 = 公开类型名

一个文件一个公开类型，文件名与类型名完全一致。文件名不用拼音、不用缩写。

### 3. 标识符

| 类别 | 约定 | 现有示例 |
| --- | --- | --- |
| 类 / 结构 / 枚举 | PascalCase | `PlayerController`、`CrosshairStyle` |
| 接口 | `I` + PascalCase | `IStateMachineOwner` |
| 公开字段 / 属性 / 方法 | PascalCase | `aimLayerMask` 为字段例外见下 |
| 私有 / 受保护字段 | `_camelCase` | `_input`、`_impulseSource`、`_speedCache` |
| 受保护状态机字段 | PascalCase（现有约定） | `StateMachine`、`PlayerModel` |
| 常量 / `static readonly` | PascalCase | `PlayerMoveState._transitionSpeed` 为 `readonly` 实例字段 |
| 枚举成员 | PascalCase | `Idle`、`Move`、`Attack`、`Dead` |
| 方法 | 动词开头 | `SwitchState`、`EnterAim`、`ShakeCamera` |
| 布尔成员 | `Is` / `Has` / `Can` 前缀 | `IsBeControl()`、`IsHover()` |

注意现有约定：公开可序列化字段直接 PascalCase 且不加下划线（`moveInput`、`currentPlayerModel`、`bulletInterval`）。新增字段沿用所在文件的既有风格，不为了「统一」重排既有文件。

### 4. 中文注释，XML 文档注释

- 注释用中文。
- 类与公开方法写 XML 文档注释（`/// <summary>`），参数与泛型参数写 `<param>` / `<typeparam>`。参照 `Utils/StateMachine.cs`、`Base/EnemyBase.cs`。
- 复杂逻辑（状态流转、射线/命中判定、时序、动画哈希）写清「**为什么**这么做」，不写复述代码的「做了什么」。
- 禁止表情符号与装饰性分隔符（`=====`、`-----` 之类的注释横线）。
- 行内注释紧跟代码，用 `//` 后一个空格；现有写法如 `public static T INSTANCE; // 实例`。

### 5. 序列化字段

- 需要在 Inspector 调的字段加 `[Tooltip("中文说明")]`，说明用途与单位。
- 运行时赋值、不应暴露给 Inspector 的字段加 `[HideInInspector]`（现有 `moveInput`、`localMovement`、`rb`）。
- 字段上直接写简短行内注释与 `[Tooltip]` 并存是现有风格，允许。

**改名或改类型会丢失 Inspector 绑定**：这类改动前先说明影响面，改完提醒使用者在 Editor 里重新连接，并在汇报中列为「需实机确认项」。

### 6. 格式

- 缩进 4 空格，不用 Tab。
- 大括号 Allman 风格（左括号独占一行）。
- `if` / `for` 单语句也加大括号（现有代码有省略写法，新增代码不省略）。
- `#region` / `#endregion` 用于分段（现有 `PlayerController`、`CrosshairSettings` 使用）。新增沿用所在文件的既有分段风格，**不为整齐重排既有文件**。

### 7. 编码与换行

- 新建与修改的文件统一 **UTF-8 无 BOM**。
- 不为了统一编码而批量改写既有带 BOM 的文件——那会产生无意义的整文件 diff。既有文件在因功能改动被打开时，顺手改为无 BOM 是可接受的。
- 换行沿用文件现状，不做全文件换行符转换。

### 8. 既有拼写错误不擅自改名

工程内存在命名瑕疵，属于**既有公开接口**，改名会破坏所有引用，必须走提案：

| 现状 | 说明 |
| --- | --- |
| `StateBase.Destory()`（含 `StateMachine` 的调用点与各 `*StateBase` 覆盖） | 应为 `Destroy`。与 `MonoBehaviour.Destroy` 同名会引起误读，但改名属公共接口变更 |
| `EnemyBase.PlayerStateAnimation()` / `PlayerModel.PlayerStateAnimation()` | `Player` 应为 `Play`（触发播放） |

新增代码不要复制这类拼写；需要改名时先列出全部引用点、给出一次性改动方案。

### 9. `var` 的使用

仅当右侧类型在阅读时一眼可辨时使用；跨 API 调用、返回值含义不明时写显式类型。与所在文件现状保持一致。

## 变更记录

- 2026-10-06：初版。约定与拼写遗留项均据实测量（含 21 个源文件的 BOM/换行分布）。

## 相关分册

- [spec-01-architecture.md](spec-01-architecture.md) 程序集与目录分层
- [spec-04-input.md](spec-04-input.md) 输入系统
