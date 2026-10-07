# 元素战争（ElementWar）工程规范

> **本文件是本工程唯一的治理入口**，由 dsh / Claude Code / Codex 等所有 AI 协作工具统一加载（工程根到当前目录逐层）。
> 规范正文按主题拆分在 `docs/specs/`，`CLAUDE.md` 只是 Claude 侧的薄入口。
> 改动行为就同步改本文件或对应分册；子目录若需专门规则，放就近的局部 `AGENTS.md`，不要全塞在这里。

## 一、工程概览

- **定位**：Unity 独立游戏项目「元素战争」，第一人称射击（FPS）。现有系统：玩家（移动/瞄准/跳跃/悬停）、武器与子弹、准星（可配置样式）、敌人（僵尸）AI 状态机。
- **引擎与管线**：Unity **2023.2.20f1**（`E:\unity\Unity 2023.2.20f1\Editor\Unity.exe`）+ **URP 16.0.6**（管线资产在 `Assets/Settings/`：URP-Balanced / URP-HighFidelity / URP-Performant 及其 Renderer）。
- **关键包**：Cinemachine **3.1.7**（命名空间已由 `Cinemachine` 改为 **`Unity.Cinemachine`**）、Input System **1.14.0**（`activeInputHandler: 2`，仅新输入系统）、Test Framework 1.3.9、Timeline、VisualScripting、Animation Rigging、ProBuilder、AI Navigation、MCP for Unity **10.2.0**。
- **程序集**：**无 asmdef**，全部项目代码编入默认程序集 `Assembly-CSharp`（源码在 `Assets/Scripts/`）。
- **项目场景**：`Assets/Scenes/TestScene.unity`（当前唯一）。
- **仓库**：单仓库单工程，远端 `git@github.com:nfachenxi/ElementWar.git`，默认分支 `master`。

## 二、规范结构

详细规范按主题拆分到 `docs/specs/`，本文件只保留入口级速览：

| 分册 | 说明 |
| --- | --- |
| [spec-00-core.md](docs/specs/spec-00-core.md) | 核心铁律与编码纪律（禁止臆造、先分析再确认、第三方只读、任务分级、汇报与检索预算） |
| [spec-01-architecture.md](docs/specs/spec-01-architecture.md) | 程序集与目录分层、域归位、不引入第二套机制 |
| [spec-02-lifecycle-performance.md](docs/specs/spec-02-lifecycle-performance.md) | 生命周期、帧驱动、射线与 GC、对象池、性能举证 |
| [spec-03-assets-scene-prefab.md](docs/specs/spec-03-assets-scene-prefab.md) | 资源体量红线、场景与 Prefab 纪律、`.meta`、导入设置 |
| [spec-04-input.md](docs/specs/spec-04-input.md) | Input System（`.inputactions` 为准，生成类不手改，禁用旧 API） |
| [spec-05-camera-cinemachine.md](docs/specs/spec-05-camera-cinemachine.md) | 相机与 Cinemachine 3.x（命名空间、弃用类、Priority 切换、震屏入口） |
| [spec-06-ui-crosshair.md](docs/specs/spec-06-ui-crosshair.md) | UI 与准星（uGUI、ScriptableObject 配置、Canvas 性能） |
| [spec-07-naming-style.md](docs/specs/spec-07-naming-style.md) | 命名、代码风格、注释与格式、既有拼写遗留项 |
| [spec-08-problem-solving.md](docs/specs/spec-08-problem-solving.md) | 问题排查纪律与决策流程（查证顺序、典型场景、禁止清单） |
| [spec-09-build-verify.md](docs/specs/spec-09-build-verify.md) | 构建、编译与验证四层手段 + 收尾汇报格式 |
| [spec-10-git-workflow.md](docs/specs/spec-10-git-workflow.md) | Git 与版本工作流（入库/忽略边界、分支与提交、不引入 worktree） |
| [spec-11-ai-collab-mcp.md](docs/specs/spec-11-ai-collab-mcp.md) | AI 协作与 MCP／代码图谱（三宿主配置、工具路由、索引维护、降级声明） |

## 三、核心铁律速览

完整表述与判断标准见 [spec-00](docs/specs/spec-00-core.md)。

1. **禁止臆造** — 不确定 Unity／包 API 时先查 `Library/PackageCache/` 里实际安装的包源码；查不到就抛出问题等确认。
2. **先分析 → 再确认 → 再实施** — 需求不明或影响面大时先出方案；未经确认不批量修改、不整体重构、不扩展范围。
3. **第三方只读** — `Assets/Low Poly FPS Pack/`、`Assets/MMD4Mecanim/`、`Assets/Plugins/` 不改其源码与资源；需要定制就复制到工程自己的目录再改。
4. **生成文件不手改** — `MyInputSystem.cs` 由 `.inputactions` 生成，改输入只改 `.inputactions`。
5. **不手写序列化 YAML** — `.unity`／`.prefab`／`.asset`／`ProjectSettings` 一律经 Editor（或 Unity MCP）修改。
6. **`.meta` 同增同删** — 不留孤儿 meta，不手工改既有 GUID。
7. **不引入第二套机制** — 状态机用 `Utils/StateMachine`，单例用 `Base/SingleMonoBase<T>`，帧驱动用 `Utils/MonoManager`，玩家入口用 `PlayerController.INSTANCE`。
8. **不擅自引入架构** — 不新增 asmdef、不引第三方状态机／DI／事件／资源框架（含 UI Toolkit 与 uGUI 混用）。
9. **Resources 慎用** — `Assets/Resources/` 已 182.8 MB 且打包后常驻；新增资源默认不进 Resources。
10. **性能结论要举证** — 热点路径改动必须有 Profiler 依据或量化推理，不靠感觉宣称「变快了」。
11. **最小改动** — 优先局部修改与复用现有结构；不为「更完整」堆抽象层。
12. **未授权不散写文档** — 不新建说明／设计／总结类 `.md`／`.txt`；治理文档只写 `docs/`，且需授权。

## 四、目录分层

| 目录 | 职责 |
| --- | --- |
| `Assets/Scripts/Base/` | 跨域基类：`StateBase`、`EnemyStateBase`、`PlayerStateBase`、`EnemyBase`、`SingleMonoBase<T>` |
| `Assets/Scripts/Utils/` | 通用工具：`StateMachine`（自研状态机）、`MonoManager`（帧驱动） |
| `Assets/Scripts/Player/` | 玩家域：`PlayerController`、`PlayerModel`、`PlayerWeapon`、`PlayerWeaponBullet`，状态放 `Player/State/` |
| `Assets/Scripts/Enemy/` | 敌人域：`ZombieEnemy`，状态放 `Enemy/State/` |
| `Assets/Scripts/Crosshair/` | 准星域：`CrosshairSettings`（ScriptableObject）、`CrosshairUI` |
| `Assets/Scenes/` | 项目场景 |
| `Assets/Settings/` | URP 管线资产、Input System（含生成物）、Volume Profile |
| `Assets/Resources/` | 运行时按名加载资源（182.8 MB，慎增） |
| `Assets/Plugins/` | 第三方插件（只读，37.7 MB） |
| `Assets/Low Poly FPS Pack/` | 第三方资产（只读，296.2 MB，不入库） |
| `Assets/MMD4Mecanim/` | 第三方资产（只读，448.6 MB，不入库） |
| `docs/specs/` | 规范正文 |
| `Library/` `Temp/` `obj/` `Logs/` `UserSettings/` | 引擎生成目录，只读参考、不写入、不入库 |

## 五、复用清单（优先使用现有能力）

- **状态机**：`Utils/StateMachine.cs` + `IStateMachineOwner`；域状态继承 `Base/*StateBase`，放 `<域>/State/`。
- **单例与帧驱动**：单例继承 `Base/SingleMonoBase`（`Awake` 写 `INSTANCE`，`OnDestroy` 置空）；需要 Update 调度走 `Utils/MonoManager.INSTANCE.AddUpdateAction` / `RemoveUpdateAction`（**成对调用**）。
- **玩家入口**：`PlayerController.INSTANCE`（状态基类已按此取用）。
- **输入**：`Assets/Settings/InputSystem/MyInputSystem`（生成类）+ `MyInputSystem.inputactions`。现有 `Player`map：`Move` / `Look` / `Fire` / `IsSprint` / `IsAiming` / `IsJumping`。输入统一在 `PlayerController.Update` 采样成字段，状态类只读字段。
- **相机**：Cinemachine 已接入 `PlayerController`（`freeLookCamera` / `aimingCamera`，`CinemachineImpulseSource` 震屏入口 `ShakeCamera()`）。当前为 CM 3.x：命名空间 `Unity.Cinemachine`；`CinemachineFreeLook` 属 CM3 的 Deprecated 保留类，可用但报 CS0618，**新代码不要再新增**，需要新相机用 `CinemachineCamera` + `OrbitalFollow` / `RotationComposer`。
- **准星**：`CrosshairSettings`（ScriptableObject，菜单 `ElementWar/Crosshair Settings`，Cross/Dot/Circle/Chevron 四种样式）+ `CrosshairUI.Instance.Show()/Hide()`；子物体由 `EnsureChildObjects()` 程序化创建。
- **武器与子弹**：`PlayerWeapon`（开火/后坐/射线）、`PlayerWeaponBullet`（弹道，`Destroy(gameObject, lifeTime)`，**尚未池化**）。
- **动画**：`PlayerModel.OnAnimatorMove` 驱动位移；`PlayerStateAnimation()` 播动画；Animation Rigging（`TwoBoneIKConstraint`、`MultiAimConstraint`）做手部/身体瞄准约束。

## 六、本地事实来源优先级

1. 本工程当前源码：`Assets/Scripts/`、`Assets/Settings/`、`ProjectSettings/`
2. 实际安装的包源码：`Library/PackageCache/<pkg>/`（只读，版本敏感 API 的唯一权威）
3. 包自带文档：`Library/PackageCache/<pkg>/Documentation~`
4. 代码图谱：codegraph `explore` / CBM `search_graph`、`trace_path`
5. 以上都无答案 → **停下提问**，不用模型记忆替代

**找不到可靠本地依据时必须暂停并询问用户，不得直接凭模型知识开发。**

## 七、AI 协作与 MCP（三宿主入口）

| 宿主 | 工程级配置 | 使用方式 |
| --- | --- | --- |
| dsh | `.dsh/@wingsky-1/dsh-mcp-manager/mcp.json` | 在本工程目录启动 dsh |
| Claude Code | `.mcp.json` | 在本工程目录启动 Claude Code |
| Codex CLI | `~/.codex/elementwar.config.toml` | `codex --profile elementwar` |

三个 server：`unity`（HTTP `http://127.0.0.1:8080/mcp`，需 Unity 打开并启动 MCP 服务）、`codegraph`（stdio 代码图谱）、`codebase-memory-mcp`（stdio 代码库记忆）。

注意：**不要点 Unity 里 `Window → MCP for Unity` 的 Configure 按钮**——它会写宿主的用户级全局配置，与本工程清单重复。详见 [spec-11](docs/specs/spec-11-ai-collab-mcp.md)。

## 八、验证速查

```powershell
# 1) 编译信号（首选，无需 Unity）
dotnet build Assembly-CSharp.csproj

# 2) Editor 批处理编译（需先关闭 Editor）
& "E:\unity\Unity 2023.2.20f1\Editor\Unity.exe" -batchmode -quit -nographics `
    -projectPath "E:\Unity_Project\ElementWar" -logFile "<日志绝对路径>"
```

判据：无 `error CS` 且退出码 0。实机验证走 Unity MCP；MCP 不可用时如实声明未验证。收尾汇报格式见 [spec-09](docs/specs/spec-09-build-verify.md)。

## 九、变更记录

- 2026-09-27：初版 AGENTS.md，依据工程实测（版本/管线/包/目录/脚本分层/场景分布）生成。
- 2026-09-28：Input System 1.7.0→1.14.0、Cinemachine 2.10.4→3.1.7。起因：CM 3.1.7 依赖 Input System 1.8+ 的 `InputAction.activeValueType`。注意 Unity 2023.2 上 Input System 只能用 ≤1.14.0（1.14.1 起用了本版本不存在的 `BuildTarget.VisionOS`）。CM3 命名空间为 `Unity.Cinemachine`，`PlayerController` 已同步 `using`。
- 2026-10-06：**改版为分册治理结构**（参考 `Neoforge2_chenxirfm`）。本文件瘦身为唯一入口，正文拆入 `docs/specs/` 十二册；新增 `CLAUDE.md` 薄入口与 `docs/README.md`；落地三宿主工程级 MCP 清单（`.mcp.json`、`.dsh/.../mcp.json`、`~/.codex/elementwar.config.toml`）；建立 codegraph 与 CBM 索引；`.gitignore` 增加 `.codegraph/` 与 `.ekko-tmp/` 并写明必须入库项。
