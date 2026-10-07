---
rule_type: camera
priority: high
applies_to: scripts
phase: any
---

# 相机与 Cinemachine 3.x

## 现状基线

| 项 | 现状 |
| --- | --- |
| 包版本 | `com.unity.cinemachine` **3.1.7** |
| 命名空间 | **`Unity.Cinemachine`**（CM 3.x 已从 `Cinemachine` 改名） |
| 现有相机 | `PlayerController.freeLookCamera`、`PlayerController.aimingCamera`，类型均为 `CinemachineFreeLook` |
| 震屏 | `PlayerController._impulseSource`（`CinemachineImpulseSource`，挂在瞄准相机上） |
| 主相机获取 | `PlayerController.Start` 里 `Camera.main` 缓存到 `mainCamera` 与 `_cameraTransform` |

## 规则

### 1. 命名空间与版本

新代码 `using Unity.Cinemachine;`。**不要**写 `using Cinemachine;`（CM 2.x 命名空间）。

### 2. `CinemachineFreeLook` 属弃用保留类

`CinemachineFreeLook` 在 CM 3.x 标记为 Deprecated，编译会产生 `CS0618` 警告。现有成员（`freeLookCamera`、`aimingCamera`）**维持现状可用**，但：

- 新代码不再新增 `CinemachineFreeLook` 字段。
- 需要新相机时用 **`CinemachineCamera` + `OrbitalFollow` / `RotationComposer`** 组合。
- 把现有 FreeLook 迁移到 `CinemachineCamera` 属于「大型」改动（涉及轴同步、瞄准切换、震屏绑定），先出方案再动。

判断 CM 3.x API 是否可用，一律以 `Library/PackageCache/com.unity.cinemachine@3.1.7/` 源码为准，不查网上 CM 2.x 示例。

### 3. 相机切换沿用 Priority 模式

现状用 `Priority` 高低切换：生效相机 `Priority = 100`，非生效 `0`。

```
进入瞄准：freeLookCamera.Priority = 0;   aimingCamera.Priority = 100;
退出瞄准：aimingCamera.Priority   = 0;   freeLookCamera.Priority = 100;
```

新增相机/视角切换沿用该模式，不要引入第二套切换机制（如直接 `SetActive` 相机对象造成 Brain 抖动）。

### 4. 视角角度同步

`CinemachineFreeLook` 的 `m_XAxis.Value` / `m_YAxis.Value` 是轴状态。瞄准与自由相机切换时必须互相同步，否则切换瞬间视角跳变。现有实现见 `PlayerController.EnterAim()` / `ExitAim()`——新视角切换必须复刻同样的同步步骤。

### 5. 震屏统一入口

震屏走 `PlayerController.ShakeCamera()`（内部 `_impulseSource.GenerateImpulse()`），不在其他脚本里各自抓 `CinemachineImpulseSource`。新增震源（如受击方向抖动）先提案，说明是复用同一 Source 还是新增 Impulse Listener。

### 6. 不新增第二套跟随逻辑

相机跟随、朝向、瞄准偏移一律由 Cinemachine 承载。禁止在脚本里手写 `transform.position = Vector3.Lerp(...)` 形式的相机跟随。

### 7. Brain 与主相机

场景中 `CinemachineBrain` 挂在主相机上，`Camera.main` 的引用在 `Start` 缓存。代码里不逐帧访问 `Camera.main`（它有查找开销，且缓存更稳定）。

若场景中找不到主相机，`PlayerController` 会 `Debug.LogError`——这是刻意的失败提示，不要改成静默兜底。

## 变更记录

- 2026-10-06：初版。CM 3.1.7 的 `CinemachineFreeLook` 弃用现状与 Priority 切换模式据实测量。

## 相关分册

- [spec-04-input.md](spec-04-input.md) 输入系统
- [spec-06-ui-crosshair.md](spec-06-ui-crosshair.md) UI 与准星
