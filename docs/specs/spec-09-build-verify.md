---
rule_type: verify
priority: critical
applies_to: all
phase: verify
---

# 构建、编译与验证

验证按代价从低到高，**能离线完成的不动 Editor**。命令统一用 PowerShell（`pwsh` 或 Windows PowerShell 均可）。

## 1 编译信号（首选，无需 Unity 运行）

工程根已有 Unity 生成的 `Assembly-CSharp.csproj`、`Assembly-CSharp-Editor.csproj`、`ElementWar.sln`（本机 dotnet SDK 9.0.315）。

```powershell
cd E:\Unity_Project\ElementWar
dotnet build Assembly-CSharp.csproj
```

判据：`0 个错误` 且无 `error CS`，退出码 0。实测约 2.3 秒。

**基线噪声（勿追）**：该命令会固定输出 **2 个 MSB3277 警告**，内容为 `System.Threading.Tasks.Extensions` 版本冲突（4.2.0.0 与 4.2.0.1），来源是 `Assets/Plugins/Roslyn/Microsoft.CodeAnalysis.dll` 与 Unity Facade 程序集。这是既有状态，与业务代码无关；判据只看错误数为 0。

注意：

- 这几个 `.csproj` / `.sln` 由 Unity 生成，且被 `.gitignore` 排除。**Unity 未重新生成前，新增的源文件不会出现在 csproj 里**，此时编译通过不代表新文件能编译——新增文件后先在 Editor 触发一次刷新（或跑一次批处理编译）。
- 该命令给出的是编译信号，**不验证**运行时行为、Inspector 绑定与资产正确性。

## 2 Editor 批处理编译（需要 Editor 侧确认时）

必须先**关闭 Editor**，否则工程被占用会失败：

```powershell
& "E:\unity\Unity 2023.2.20f1\Editor\Unity.exe" -batchmode -quit -nographics `
    -projectPath "E:\Unity_Project\ElementWar" `
    -logFile "<日志绝对路径>"
```

判据：日志中无 `error CS`，且进程退出码为 0。日志文件放在工程外的临时目录，不要留在工程根污染仓库。

## 3 自动化测试

Test Framework **1.3.9** 可用，工程当前**没有测试目录**。

新增测试前先确认放哪（约定：`Assets/Tests/EditMode/`、`Assets/Tests/PlayMode/`），并确认是否需要 asmdef 承载测试程序集——**新增 asmdef 属于架构变动**，先提案（见 [spec-01-architecture.md](spec-01-architecture.md)）。

```powershell
& "E:\unity\Unity 2023.2.20f1\Editor\Unity.exe" -batchmode -runTests `
    -projectPath "E:\Unity_Project\ElementWar" `
    -testPlatform EditMode `
    -testResults "<结果 xml 绝对路径>" `
    -logFile "<日志绝对路径>"
```

`-testPlatform` 取 `EditMode` 或 `PlayMode`。

## 4 实机验证（Unity MCP）

Unity 打开本工程并在 `Window → MCP for Unity` 启动服务后，MCP 工具可用于：

- 运行/停止 Play 模式，读 Console 日志
- 创建与修改场景、Prefab、资源
- 读取 Inspector 值、执行菜单命令
- 触发资源刷新（`AssetDatabase.Refresh`）

端点与参数见 [spec-11-ai-collab-mcp.md](spec-11-ai-collab-mcp.md)。

**MCP 不可用时如实声明未验证**，并列出需在 Editor 手动确认的项。

## 5 收尾汇报格式（强制）

每轮交付以如下内容结尾，缺项要显式说明：

1. **验证方式**：用了哪几层（1/2/3/4）
2. **验证结果**：具体命令、退出码、关键输出（错误/警告数）
3. **尚未验证项**：未做的层次 + 需人工确认的清单（如「Inspector 上 `aimLayerMask` 需重连」「准星新元素在 16:9 与 21:9 下的表现未看」）
4. **改动文件清单**：文件路径 + 一句话改动摘要
5. **建议提交信息**：一行 `type: 摘要`

## 6 常见验证陷阱

| 陷阱 | 说明 |
| --- | --- |
| 只看编译通过就宣称完成 | 编译不覆盖 Inspector 绑定、资产引用、运行时行为 |
| 新增文件后立刻 `dotnet build` | csproj 未刷新，新文件根本没参与编译 |
| Editor 开着跑批处理 | 工程被占用，日志报锁文件错误 |
| 把 `-logFile` 写在工程根 | 污染仓库；`.gitignore` 虽忽略 `*.log` 但仍不应产生 |
| 编辑模式改脚本后不刷新 | 需 `AssetDatabase.Refresh` 或回 Editor 触发编译 |
| 把 MSB3277 基线警告当成本次引入的问题 | 见第 1 节，判据只看错误数 |
| 用 Profiler 结论替代功能验证 | 性能与正确性是两个维度，都要给 |

## 变更记录

- 2026-10-06：初版。Unity 路径、dotnet 9.0.315、Test Framework 1.3.9 与「无测试目录」现状据实测量；实测 `dotnet build Assembly-CSharp.csproj` = 0 错误 / 2 个 MSB3277 基线警告，耗时约 2.3 秒。

## 相关分册

- [spec-08-problem-solving.md](spec-08-problem-solving.md) 问题排查纪律与决策流程
- [spec-11-ai-collab-mcp.md](spec-11-ai-collab-mcp.md) AI 协作与 MCP／代码图谱
