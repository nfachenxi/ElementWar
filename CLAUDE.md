---
description: 元素战争（ElementWar）Unity 工程级开发规范入口，Claude 侧薄入口，正文以 AGENTS.md 与 docs/specs/ 为准
---

回答用户问题时，必须使用中文回复。

# 项目概览

Unity 2023.2.20f1 + URP 16.0.6 的第一人称射击项目「元素战争」。无 asmdef，全部项目代码编入默认程序集 `Assembly-CSharp`，源码在 `Assets/Scripts/`。

关键版本：Cinemachine **3.1.7**（命名空间 `Unity.Cinemachine`）、Input System **1.14.0**（`activeInputHandler: 2`，仅新输入系统）、Test Framework 1.3.9、MCP for Unity 10.2.0。

# 治理入口

**工程最终治理规则以 [`AGENTS.md`](AGENTS.md) 为准**（所有 AI 协作工具的唯一入口）。本文件只是 Claude 侧的薄入口，不重复维护正文。

详细规范位于 `docs/specs/`：

- [spec-00-core.md](docs/specs/spec-00-core.md) — 核心铁律与编码纪律
- [spec-01-architecture.md](docs/specs/spec-01-architecture.md) — 程序集与目录分层
- [spec-02-lifecycle-performance.md](docs/specs/spec-02-lifecycle-performance.md) — 生命周期、帧驱动与性能
- [spec-03-assets-scene-prefab.md](docs/specs/spec-03-assets-scene-prefab.md) — 资源、场景与 Prefab
- [spec-04-input.md](docs/specs/spec-04-input.md) — 输入系统
- [spec-05-camera-cinemachine.md](docs/specs/spec-05-camera-cinemachine.md) — 相机与 Cinemachine 3.x
- [spec-06-ui-crosshair.md](docs/specs/spec-06-ui-crosshair.md) — UI 与准星
- [spec-07-naming-style.md](docs/specs/spec-07-naming-style.md) — 命名与代码风格
- [spec-08-problem-solving.md](docs/specs/spec-08-problem-solving.md) — 问题排查纪律与决策流程
- [spec-09-build-verify.md](docs/specs/spec-09-build-verify.md) — 构建、编译与验证
- [spec-10-git-workflow.md](docs/specs/spec-10-git-workflow.md) — Git 与版本工作流
- [spec-11-ai-collab-mcp.md](docs/specs/spec-11-ai-collab-mcp.md) — AI 协作与 MCP／代码图谱

# 核心铁律（速览）

1. **禁止臆造**：不确定 API 先查 `Library/PackageCache/` 实际安装的包源码，查不到就抛问题等确认。
2. **先分析 → 再确认 → 再实施**：未经确认不批量修改、不整体重构、不扩展范围。
3. **第三方只读**：`Assets/Low Poly FPS Pack/`、`Assets/MMD4Mecanim/`、`Assets/Plugins/` 不改其源码与资源。
4. **生成文件不手改**：`MyInputSystem.cs` 由 `.inputactions` 生成。
5. **不手写序列化 YAML**：`.unity`／`.prefab`／`.asset`／`ProjectSettings` 经 Editor 或 Unity MCP 改。
6. **`.meta` 同增同删**，不手工改既有 GUID。
7. **不引入第二套机制**：状态机 `Utils/StateMachine`、单例 `Base/SingleMonoBase<T>`、帧驱动 `Utils/MonoManager`、玩家入口 `PlayerController.INSTANCE`。
8. **不擅自引入架构**：不新增 asmdef、不引第三方框架。
9. **Resources 慎用**：`Assets/Resources/` 已 182.8 MB，新增默认不进。
10. **性能结论要举证**：热点改动必须有 Profiler 依据。
11. **最小改动**：优先局部修改与复用。
12. **未授权不散写文档**：治理文档只写 `docs/`。

# 关键路径

- 源码：`Assets/Scripts/`（`Base/`、`Utils/`、`Player/`、`Enemy/`、`Crosshair/`）
- 场景：`Assets/Scenes/TestScene.unity`
- 管线与输入：`Assets/Settings/`（URP 资产、`InputSystem/MyInputSystem.inputactions`）
- 版本敏感 API 参考：`Library/PackageCache/`（只读）
- 规范正文：`docs/specs/`

# Claude 侧工作约定

- 需实时确认疑问和需求时，使用 `AskUserQuestion`。
- 编译验证统一走 `dotnet build Assembly-CSharp.csproj`（无需 Unity）；需要 Editor 侧确认时先用批处理编译（需关闭 Editor）。规则见 [spec-09](docs/specs/spec-09-build-verify.md)。
- 大型改造先给出**文件级计划**并等待批准，再进入实施。
- 代码查询优先使用 CodeGraph / CBM 图谱工具，图谱无答案时再 grep。
- 收尾统一汇报：验证方式 → 验证结果 → 尚未验证项 → 改动文件清单。
- 不要点 Unity 里 `Window → MCP for Unity` 的 Configure 按钮（会写用户级全局配置，与工程级清单重复）。
