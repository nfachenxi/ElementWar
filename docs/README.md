# 文档索引

本目录是 ElementWar 工程的治理文档区，白名单纳入版本管理。规范正文一律以 `docs/specs/` 为准；`AGENTS.md` 是唯一入口，`CLAUDE.md` 只是 Claude 侧薄入口。

## docs/specs/ 规范分册

| 分册 | 主题 | 优先级 |
| --- | --- | --- |
| [spec-00-core.md](specs/spec-00-core.md) | 核心铁律与编码纪律 | critical |
| [spec-01-architecture.md](specs/spec-01-architecture.md) | 程序集与目录分层 | high |
| [spec-02-lifecycle-performance.md](specs/spec-02-lifecycle-performance.md) | 生命周期、帧驱动与性能 | high |
| [spec-03-assets-scene-prefab.md](specs/spec-03-assets-scene-prefab.md) | 资源、场景与 Prefab | high |
| [spec-04-input.md](specs/spec-04-input.md) | 输入系统 | high |
| [spec-05-camera-cinemachine.md](specs/spec-05-camera-cinemachine.md) | 相机与 Cinemachine 3.x | high |
| [spec-06-ui-crosshair.md](specs/spec-06-ui-crosshair.md) | UI 与准星 | medium |
| [spec-07-naming-style.md](specs/spec-07-naming-style.md) | 命名与代码风格 | high |
| [spec-08-problem-solving.md](specs/spec-08-problem-solving.md) | 问题排查纪律与决策流程 | critical |
| [spec-09-build-verify.md](specs/spec-09-build-verify.md) | 构建、编译与验证 | critical |
| [spec-10-git-workflow.md](specs/spec-10-git-workflow.md) | Git 与版本工作流 | medium |
| [spec-11-ai-collab-mcp.md](specs/spec-11-ai-collab-mcp.md) | AI 协作与 MCP／代码图谱 | high |

每册开头有 frontmatter（`rule_type` / `priority` / `applies_to` / `phase`），末尾有「变更记录」与「相关分册」。改动行为时同步改对应分册，并在其变更记录里追加一行。

## 阅读顺序建议

- **新会话 / 新工具接入**：先读 `AGENTS.md` 的「核心铁律速览」与「复用清单」，再按需展开分册。
- **要动代码**：spec-00 → spec-01 → 对应域分册（04/05/06）→ spec-07。
- **要交付**：spec-08 → spec-09。
- **要配工具**：spec-11。

## 尚未设立的目录

按需再建，当前不预置空目录：

- `docs/plans/`：跨多步的实施计划
- `docs/归档/`：已完成计划的归档
- `docs/指南/`：面向使用者的操作说明

## 维护约定

- 未获授权不新增 `docs/` 以外的 `.md`／`.txt` 文件。
- 分册之间不复制正文，用相对链接互相引用，避免两处规则不一致。
- 若某分册内容被证伪（版本升级、实测与描述不符），就地修正并在变更记录写明「因何改动」，不静默删除。
