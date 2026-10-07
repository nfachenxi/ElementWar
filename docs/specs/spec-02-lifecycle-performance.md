---
rule_type: performance
priority: high
applies_to: scripts
phase: any
---

# 生命周期、帧驱动与性能

## 生命周期规则

### 1. 引用在 `Awake` 缓存

`GetComponent`、`Camera.main`、`Find*` 一类查找只在 `Awake` / `Start` 做一次并缓存到字段。禁止在 `Update` 里重复查找。

现有写法参照 `PlayerWeaponBullet.Awake()` 缓存 `rb`、`PlayerController.Awake()` 构造 `_input`。

### 2. 订阅与退订成对

`OnEnable` 订阅 / `OnDisable` 退订；启动的协程在 `OnDisable` 或 `OnDestroy` 停止。现有参照：`PlayerController` 的 `_input.Enable()` / `_input.Disable()`。

### 3. 单例生命周期

单例继承 `Base/SingleMonoBase<T>`，由基类在 `Awake` 写入 `INSTANCE`、在 `OnDestroy` 置空。子类覆盖这两个方法时必须调用 `base`。

同一场景内出现两个同类单例时，基类会 `Debug.LogError`，这是刻意的约束，不要为了「消除报错」绕开它——应当检查场景里是否错误地放了重复对象。

### 4. 帧驱动统一走 `MonoManager`

非 MonoBehaviour 的对象需要逐帧回调时，用 `MonoManager.INSTANCE.AddUpdateAction(...)` / `RemoveUpdateAction(...)`，不在各处另建分发器。**成对调用**，对象销毁前必须退订，否则会残留委托导致空引用与内存泄漏。

### 5. 物理与帧

- 物理相关（`Rigidbody` 速度、射线命中后的物理响应）放 `FixedUpdate`。
- `Update` 内的增量乘 `Time.deltaTime`；`FixedUpdate` 内乘 `Time.fixedDeltaTime`。
- 动画驱动的位移走 `OnAnimatorMove`（现有参照 `PlayerModel.OnAnimatorMove`），不要在 `Update` 里再叠加一遍位移。

## 性能规则

### 6. 射线与命中检测

- 控制检测频率，不每帧对全场景重复扫。
- 用 `LayerMask` 在检测阶段过滤，而不是检测完再事后过滤。工程内已有 `aimLayerMask`（默认 `~0`，即全层）——需要精度时必须显式收窄层，不要依赖默认值。
- 长距离检测（现状 `maxRayDistance = 1000f`）优先配合层掩码，否则会打到地形与装饰物。

### 7. 每帧避免

字符串拼接与 `ToString`、装箱、LINQ、闭包分配、`new WaitForSeconds`（改用缓存实例或 `WaitForSecondsRealtime` 缓存）。

动画参数哈希用 `Animator.StringToHash` 并声明为 `static readonly`，现有参照 `PlayerMoveState.MoveBlend`、`PlayerAimingState.AimingXHash`。

### 8. 高频生成销毁对象优先对象池

子弹（`PlayerWeaponBullet`）、命中特效、僵尸属于高频路径。当前子弹实现是 `Instantiate` + `Destroy(gameObject, lifeTime)`，**尚未池化**；在热点路径改动并声称「变快」之前，必须先有 Profiler 依据。

判定标准：同一对象类型在一次交战中出现 ≥ 每秒数次 的创建/销毁，即应评估池化。

### 9. 热点路径改动必须给依据

性能类结论必须附 Profiler 数据或明确的量化推理（调用次数 × 单次开销）。不允许凭感觉宣称「变快了」「省了很多 GC」。

### 10. 数值缓存

需要平滑量时用固定长度环形缓存而不是每帧累加。现有参照 `PlayerModel._speedCache`（容量 3）与 `UpdateAverageCacheSpeed`。

## 变更记录

- 2026-10-06：初版。含实测的未池化项（子弹）与现有帧驱动方式（`MonoManager` 单委托）。

## 相关分册

- [spec-01-architecture.md](spec-01-architecture.md) 程序集与目录分层
- [spec-09-build-verify.md](spec-09-build-verify.md) 构建、编译与验证
