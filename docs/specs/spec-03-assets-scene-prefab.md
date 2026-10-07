---
rule_type: assets
priority: high
applies_to: assets
phase: any
---

# 资源、场景与 Prefab

## 体量基线

| 目录 | 体量 | 性质 |
| --- | --- | --- |
| `Assets/MMD4Mecanim/` | 448.6 MB | 第三方（只读），已排除入库 |
| `Assets/Low Poly FPS Pack/` | 296.2 MB | 第三方（只读），已排除入库 |
| `Assets/ThirdParty/` | 208.4 MB | 第三方美术资产归口（只读），已入库 |
| `Assets/Plugins/` | 12.2 MB | 第三方代码插件（只读，仅 `Roslyn`），已入库 |

`Assets/Resources/` 已于 **2026-10-06 清空并删除**（原 182.8 MB）。取证依据：全工程 `Resources.Load`／`LoadAsync`／`Unload` 为 **0 次**；该目录下被引用的 79 项资产全部由 `Assets/Scenes/TestScene.unity` 以 Inspector 引用方式使用。经 Unity MCP `AssetDatabase.MoveAsset` 迁移到 `Assets/ThirdParty/` 后，GUID 保留、`TestScene` 对 `Assets/Resources/` 的依赖归零。

## 规则

### 1. 不重建 `Resources`（该目录已移除）

本工程已无 `Assets/Resources/`，且当前没有任何 `Resources.Load` 调用。默认**不重建**该目录；新增资源一律走 Inspector 直接引用或 `ScriptableObject` 配置持有。

若某资源确需「按名字在运行时加载」，先提案说明：为什么 Inspector 引用不可行、预估体积、常驻内存代价，经确认后才建 `Assets/Resources/`，并**只在该目录放这一项**，同时在变更记录里写明理由与体积。

优先顺序：Inspector 直接引用 → `ScriptableObject` 配置持有 → `Resources` 按名加载（需先提案）。

### 2. 场景

- 新场景放 `Assets/Scenes/`，按用途命名（如 `TestScene`、`Level_01`）。
- 不把第三方 Demo 场景当开发场景。
- 场景内新增单例对象前，先确认场景里没有同类实例（否则 `SingleMonoBase` 会报错，见 spec-02）。

### 3. 不手写序列化 YAML

`.unity` / `.prefab` / `.asset` / `ProjectSettings/*.asset` 一律经 Editor 修改。需要自动化时走 Unity MCP（见 [spec-11-ai-collab-mcp.md](spec-11-ai-collab-mcp.md)），不走文本编辑。

### 4. `.meta` 纪律

- 新增资源必须同时提交 `.meta`。
- 删除资源必须同时删除 `.meta`，不留孤儿。
- 不手工编辑已有 `.meta` 的 `guid`：GUID 变更会切断所有引用，表现为「引用丢失」。
- 移动资源用 Editor 内拖拽或 `AssetDatabase.MoveAsset`，不要用文件系统的移动。

### 5. 命名与目录

资源文件名与用途一致，避免 `New Material 1`、`Copy of ...` 这类默认名。同类资源在同目录内成组放置（贴图、材质、Prefab 不混放）。

### 6. 导入设置变更需说明影响

改动纹理尺寸/压缩格式、音频 Load Type、模型 Read/Write Enabled、动画压缩、Mesh 优化等，必须在汇报里说明对**内存峰值**与**包体**的影响方向与量级。默认不动第三方资产的导入设置。

### 7. 管线资产

`Assets/Settings/` 下 URP 资产（`URP-Balanced`、`URP-HighFidelity`、`URP-Performant` 及各 Renderer、`UniversalRenderPipelineGlobalSettings`）是渲染基线。改动 Renderer 特性、后处理、阴影分辨率等会影响全工程画面与性能，视为「大型」改动，先出方案。

### 8. 新增大型资源前先确认

新增与 `Low Poly FPS Pack` / `MMD4Mecanim` / `Assets/ThirdParty/`（208.4 MB）同量级（≥200 MB）的资源前，先确认是否必需、能否复用现有素材。

## 变更记录

- 2026-10-06：初版。体量为实测值。
- 2026-10-06：**布局调整**。`Assets/Resources/` 清空删除，内容与 `Plugins/EffectCore`、`Plugins/YSA Toon` 一并归口到 `Assets/ThirdParty/`（208.4 MB）；规则 1 由「新增资源默认不进 Resources」升级为「不重建 Resources」。移动经 Unity MCP `AssetDatabase.MoveAsset`，符合规则 4。

## 相关分册

- [spec-05-camera-cinemachine.md](spec-05-camera-cinemachine.md) 相机与 Cinemachine
- [spec-06-ui-crosshair.md](spec-06-ui-crosshair.md) UI 与准星
