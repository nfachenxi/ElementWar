---
rule_type: ui
priority: medium
applies_to: scripts
phase: any
---

# UI 与准星

## 现状基线

| 项 | 现状 |
| --- | --- |
| UI 包 | `com.unity.ugui` 2.0.0（uGUI），未使用 UI Toolkit |
| 准星配置 | `Crosshair/CrosshairSettings`：`ScriptableObject`，资产在 `Assets/Settings/CrosshairSettings.asset`（2026-10-06 由 `Resources/` 迁入），菜单 **`ElementWar/Crosshair Settings`**，`fileName = "CrosshairSettings"` |
| 准星样式 | `CrosshairStyle` 枚举：`Cross` / `Dot` / `Circle` / `Chevron` |
| 准星显示 | `Crosshair/CrosshairUI`，静态入口 `CrosshairUI.Instance`，提供 `Show()` / `Hide()` |
| 调用方 | `PlayerController.EnterAim()` 调 `Show()`，`ExitAim()` 调 `Hide()` |

### `CrosshairSettings` 可配项

样式与外观：`style`、`color`、`lineLength`、`lineThickness`、`gapSize`、`showCenterDot`、`centerDotSize`、`outlineEnabled`、`outlineColor`、`outlineWidth`、`previewSpread`。

动态散度：`minSpread`、`maxSpread`、`spreadPerShot`、`recoverySpeed`。

## 规则

### 1. 准星行为改配置，不改常量

样式、颜色、粗细、散度曲线一律改 `CrosshairSettings` 资产（Inspector 或经 Unity MCP 改资产），不在代码里写死数值。代码里只保留「如何应用这些值」的逻辑。

新增可配项时，同步在 `CrosshairSettings` 加字段 + 加 `[Tooltip]` 中文说明，并在 `CrosshairUI.ApplyAllSettings()` 中接入；不要只加字段不生效。

### 2. 准星子物体程序化创建

`CrosshairUI` 在 `Awake` 经 `EnsureChildObjects()` / `EnsureLine()` / `EnsureChild()` 程序化创建 `RectTransform` + `Image`，不依赖场景中手工摆放的子物体。新增准星元素（如刻度、外圈）沿用该方式，保持「一个脚本 + 一个配置资产即可工作」。

不要改成「在场景里手摆一个 Prefab」——那会破坏当前零配置可用性。

### 3. 配置热更新路径

`OnValidate()` 负责在 Editor 改配置时即时刷新；运行时改动走 `ApplyAllSettings()` / `ApplyVisibility()`。两条路径都要覆盖新增项。

### 4. 每帧只做数值

`CrosshairUI.Update()` 只做散度插值（`_currentSpread` 向目标收敛）与显示状态同步，不做查找、不做 `GetComponent`、不做字符串操作。新增每帧逻辑前先确认能否合并进现有插值。

### 5. 可见性用状态而不是反复 `SetActive`

现有 `SetActive(RectTransform, bool)` 会先比较当前状态再赋值。新增子元素复用该方法，避免每帧无意义地开关对象触发 Canvas 重建。

### 6. UI 技术栈单一

现状是 uGUI + Canvas。不要在同一界面混用 UI Toolkit 与 uGUI，也不为单个界面引入新的 UI 框架。要迁移 UI 技术栈属于「大型」改动，先提案。

### 7. Canvas 与分辨率

- 新增 Canvas 统一设置 `CanvasScaler` 参考分辨率与匹配模式，保持与现有 UI 一致。
- 频繁变动的 UI 与静态 UI 分开到不同 Canvas/子 Canvas，减少重建范围。
- 世界空间 UI（如血条）优先用 `World Space` Canvas + 对象池，不在每帧 `Instantiate`。

### 8. 中文文案

玩家可见文案用中文，但在代码中集中管理（常量或配置字段），不散落硬编码在同一方法的多处。禁止表情符号与装饰性分隔符。

## 变更记录

- 2026-10-06：初版。菜单路径、样式枚举、可配项与程序化建子物体方式据实测量。

## 相关分册

- [spec-03-assets-scene-prefab.md](spec-03-assets-scene-prefab.md) 资源、场景与 Prefab
- [spec-05-camera-cinemachine.md](spec-05-camera-cinemachine.md) 相机与 Cinemachine
