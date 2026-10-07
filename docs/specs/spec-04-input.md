---
rule_type: input
priority: high
applies_to: scripts
phase: any
---

# 输入系统（Input System 1.14.0）

## 现状基线

| 项 | 现状 |
| --- | --- |
| 包版本 | `com.unity.inputsystem` **1.14.0** |
| 生效模式 | `ProjectSettings/ProjectSettings.asset` 中 `activeInputHandler: 2` ＝ **仅新输入系统（Input System Package）** |
| 资产 | `Assets/Settings/InputSystem/MyInputSystem.inputactions` |
| 生成类 | `Assets/Settings/InputSystem/MyInputSystem.cs`（**生成物，不手改**） |
| 使用方 | `PlayerController` 持有一个 `MyInputSystem` 实例，在 `OnEnable` / `OnDisable` 启停 |

### 现有 Action

| Action Map | Action | 类型 |
| --- | --- | --- |
| `Player` | `Move` | Value (Vector2) |
| `Player` | `Look` | Value |
| `Player` | `Fire` | Button |
| `Player` | `IsSprint` | Button |
| `Player` | `IsAiming` | Button |
| `Player` | `IsJumping` | Button |
| `UI` | `Navigate` `Submit` `Cancel` `Point` `Click` `ScrollWheel` `MiddleClick` `RightClick` `TrackedDevicePosition` `TrackedDeviceOrientation` | 默认 UI 组 |

## 规则

### 1. 禁止使用旧版 Input API

`activeInputHandler: 2` 意味着 `UnityEngine.Input`（`Input.GetKey`、`Input.mousePosition`、`Input.GetAxis` 等）已**被禁用**，调用会抛 `InvalidOperationException`。

- 读输入一律用 `MyInputSystem` 生成的 Action 包装。
- 例外：`Cursor.lockState` / `Cursor.visible` 属于 `UnityEngine.Cursor`，不受此限制，可继续使用。

### 2. 改输入的唯一姿势

```
改 .inputactions（Editor 里改或经 Unity MCP 改）
  → 保存，Editor 自动重新生成 MyInputSystem.cs
  → 再写消费代码
```

不直接编辑 `MyInputSystem.cs`：它会被下一次重新生成整体覆盖，改动必然丢失。

### 3. Action 命名约定

| 类型 | 命名 | 现有示例 |
| --- | --- | --- |
| Button（纯按下状态） | `Is` + 动词/状态 | `IsSprint`、`IsAiming`、`IsJumping` |
| Button（瞬时触发） | 动词 | `Fire` |
| Value（数值） | 名词 | `Move`、`Look` |

新增 Input Action 前，先确认现有 Map 内没有语义重复的 Action。

### 4. 实例与启停成对

- 一个使用方持有一个 Action Map 实例（现状 `private MyInputSystem _input`），不每帧 `new`。
- `OnEnable` 调 `Enable()`、`OnDisable` 调 `Disable()`，成对出现。
- 多使用方共享同一 Action Map 时，用同一个 `InputActionAsset` 实例的多个 Map 引用，而不是各自 `new` 一份（多实例会导致回调重复触发）。

### 5. 按键绑定写在资产里

按键/手柄映射一律在 `.inputactions` 中配置，不在代码里硬编码 `KeyCode`。代码里只出现 Action 名。

### 6. 不引入第二套输入方案

现状是「代码持有生成类实例」这一种方式。不要同时引入 `PlayerInput` 组件式方案或自写 `InputAction` 解析，形成并行机制。要切换方案需先提案（涉及 `PlayerController` 与所有状态类的输入读取）。

### 7. 消费点集中在 `PlayerController`

输入统一由 `PlayerController.Update` 采样，写入 `moveInput` / `isSprint` / `isAiming` / `isJumping` / `isFire` 等公开字段，状态类只读这些字段，不各自去 `ReadValue`。

> `Look` Action 当前存在但代码未消费（视角由 Cinemachine 的轴驱动，见 [spec-05](spec-05-camera-cinemachine.md)）。删除前先确认没有计划接入。

## 变更记录

- 2026-10-06：初版。`activeInputHandler: 2`、Action 清单与消费点均据实测量。

## 相关分册

- [spec-05-camera-cinemachine.md](spec-05-camera-cinemachine.md) 相机与 Cinemachine
- [spec-07-naming-style.md](spec-07-naming-style.md) 命名与代码风格
