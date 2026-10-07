---
rule_type: tooling
priority: high
applies_to: agents
phase: any
---

# AI 协作与 MCP／代码图谱

## 三宿主 MCP 清单（同源）

同一份 server 清单写入三个宿主各自的工程级入口：

| 宿主 | 工程级配置文件 | 使用方式 |
| --- | --- | --- |
| dsh | `.dsh/@wingsky-1/dsh-mcp-manager/mcp.json` | 在本工程目录启动 dsh，自动读取 |
| Claude Code | `.mcp.json`（工程根） | 在工程目录启动 Claude Code，首次会询问是否信任该工程 MCP |
| Codex CLI | `~/.codex/elementwar.config.toml` | 在工程目录执行 `codex --profile elementwar` |

> Codex **没有**工程级配置，只有用户级 `~/.codex/config.toml`，因此用 profile 文件隔离；删除该文件即可完全回退，不影响其他 Codex 会话。

## Server 清单

| 名称 | 传输 | 端点／命令 | 依赖 |
| --- | --- | --- | --- |
| `unity` | HTTP | `http://127.0.0.1:8080/mcp` | Unity 打开本工程并启动 MCP 服务 |
| `codegraph` | stdio | codegraph 自带 `node.exe` + `serve --mcp` | 工程内 `.codegraph/` 索引 |
| `codebase-memory-mcp` | stdio | `C:/Users/NFA/.local/bin/codebase-memory-mcp.exe` | CBM 已建索引 |

### 为什么 codegraph 不直接写 `codegraph`

Windows 上 `codegraph` 只有 `codegraph.cmd` 外壳（位于 `%LOCALAPPDATA%\codegraph\current\bin`）。MCP 客户端以 stdio 方式拉起进程时不经 shell，直接写 `codegraph` 会 ENOENT。因此统一写成：

```
C:/Users/NFA/AppData/Local/codegraph/current/node.exe
  --liftoff-only --disable-warning=ExperimentalWarning
  C:/Users/NFA/AppData/Local/codegraph/current/lib/dist/bin/codegraph.js serve --mcp
```

### Unity MCP 端点规则

`com.coplaydev.unity-mcp`（MCP for Unity，当前 **10.2.0**）的 HTTP 端点为 **Editor 的 HTTP Base URL + `/mcp`**，默认 `http://127.0.0.1:8080`。若在 `Window → MCP for Unity → Advanced` 里改过基址，必须同步改三个配置文件里的 `unity.url`。

不要点该窗口里的 **Configure** 按钮：它会把 `unityMCP` 写进宿主**用户级**全局配置，与本工程的工程级清单重复，并可能指向错误端口。要改配置时改本工程这三个文件。

## 工具使用规范

### 1. 查代码先看图谱

回答「这个类在哪」「谁调用了它」「改这里会影响什么」时：

1. codegraph：`codegraph_explore`（一次给出相关符号源码 + 调用路径）、`node`、`callers`、`callees`、`impact`
2. CBM：`search_graph`、`query_graph`、`trace_path`、`get_architecture`、`get_code_snippet`
3. 图谱答不上时再 grep / 直接读文件

图谱不替代精确核对：涉及版本敏感 API 的结论，仍要落到 `Library/PackageCache/` 实际源码上确认。

### 2. 索引维护

| 动作 | 命令 |
| --- | --- |
| 首次建 codegraph 索引 | `codegraph init E:\Unity_Project\ElementWar` |
| 增量同步 | `codegraph sync` |
| 全量重建 | `codegraph index` |
| 查看状态 | `codegraph status` |
| CBM 建／更新索引 | CBM 工具 `index_repository`（`repo-path` 传工程根） |
| CBM 查看是否已索引 | `list_projects` / `index_status` |

索引目录 `.codegraph/` 为本地产物，已在 `.gitignore` 排除。改动大量文件后同步一次即可，不需要每次任务都重建。

### 3. 实机操作走 Unity MCP

场景、Prefab、资产、Play 模式、Console 日志的操作一律走 Unity MCP，不手改资产文本（见 [spec-00-core.md](spec-00-core.md) 铁律 5）。

### 4. 降级与如实声明

- Unity 未打开 / MCP 连不上：**如实声明「实机未验证」**，改用编译信号（见 [spec-09-build-verify.md](spec-09-build-verify.md)），并列出需人工确认的清单。
- 某个 server 连不上不影响其他 server（例如 blockbench 这类本机未启动的服务不应影响 unity 与图谱）。配置失败要逐 server 定位，不因一个失败停用全部。
- 不编造 MCP 工具返回结果。

### 5. 治理入口单一

`AGENTS.md` 是全工程唯一治理入口，`CLAUDE.md` 只是 Claude 侧的薄入口，正文一律以 `docs/specs/` 为准。所有 AI 工具（dsh / Claude Code / Codex / 其他）都以本目录的规范为准，不在各自配置里另写一套规则。

## 变更记录

- 2026-10-06：初版。三宿主配置落地；确认 Unity MCP 10.2.0 的端点构成，修正 codegraph 在 Windows 上的 stdio 启动方式。

## 相关分册

- [spec-08-problem-solving.md](spec-08-problem-solving.md) 问题排查纪律与决策流程
- [spec-09-build-verify.md](spec-09-build-verify.md) 构建、编译与验证
