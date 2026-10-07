---
rule_type: workflow
priority: medium
applies_to: all
phase: any
---

# Git 与版本工作流

## 仓库形态

单仓库单工程：`E:\Unity_Project\ElementWar`，远端 `git@github.com:nfachenxi/ElementWar.git`，默认分支 `master`。

> 与参考工程 `Neoforge2_chenxirfm`（四个独立子仓库 + worktree）不同：本工程是一个 Unity 工程，**不引入子仓库**。

## 规则

### 1. 必须入库的内容

| 路径 | 说明 |
| --- | --- |
| `Assets/`（除已排除的第三方大包） | 工程资源与源码 |
| `Packages/manifest.json`、`packages-lock.json` | 包依赖与锁定版本 |
| `ProjectSettings/` | 工程设置（版本、输入、渲染、物理等） |
| `AGENTS.md`、`CLAUDE.md` | 治理入口 |
| `docs/` | 规范正文与文档索引 |
| `.mcp.json` | 根级项目 MCP 清单 |
| `.dsh/@wingsky-1/dsh-mcp-manager/mcp.json` | dsh 项目级 MCP 清单 |
| `.gitignore` | 忽略策略本身 |

### 2. 不入库的内容

已在 `.gitignore` 中排除：`Library/`、`Temp/`、`obj/`、`Logs/`、`UserSettings/`、`.vs/`、`.idea/`、`.vscode/`、`*.csproj`、`*.sln`、`*.log`、`.codegraph/`、`.ekko-tmp/`，以及两个 296 MB / 448 MB 的第三方资源包（`Assets/Low Poly FPS Pack/`、`Assets/MMD4Mecanim/` 及其 `.meta`）。

> 这两个大包是**有意不入库**的本地资产，不是遗漏。克隆后需要自行放置；构建与打开工程前必须确认它们存在。

### 3. 不引入 worktree

本工程是 Unity 工程，一个副本对应一份体积可观的 `Library/`（缓存目录）。多 worktree ＝ 多份 Library，磁盘与打开时间成本高，且同一 Editor 进程约束下并行收益有限。

需要独立分支验证时，用 stash / 临时分支切换，不要用 worktree。

### 4. 分支与提交

- 直接在 `master` 上做小改动是现状，但**功能开发、重构、架构变动**应走独立分支。
- 分支命名：`feat/简述`、`fix/简述`、`refactor/简述`、`docs/简述`、`chore/简述`。
- 提交信息：`type: 摘要`（type 取 feat / fix / refactor / perf / docs / chore / test），摘要说清「改了什么行为」，不写「更新代码」这类无信息内容。
- 一次提交只做一件事；改 A 时顺手改 B 需在提交信息里说明，或拆成两次提交。

### 5. 提交前检查

```
[ ] 没提交 Library/、Temp/、Logs/、UserSettings/、*.csproj、*.sln
[ ] 新增资源带了对应 .meta；删除资源没有留下孤儿 .meta
[ ] 没有手工改过 .unity / .prefab / .asset / ProjectSettings 文本
[ ] 编译信号通过（见 spec-09）
[ ] 改动清单与提交信息一致
```

### 6. 大文件

- 不提交 > 100 MB 的单文件（GitHub 硬限制）。新增大资产前先确认放置方式。
- 不把 `Resources/`（当前 182.8 MB）里的内容再复制一份到别处。
- 二进制资产改动无法 diff 审阅，因此资产改动必须在提交信息里写清「改了什么、为什么」。

### 7. 与 AI 协作相关的提交

`.mcp.json`、`.dsh/.../mcp.json`、`AGENTS.md`、`CLAUDE.md`、`docs/` 的改动属于治理配置，应单独提交（`chore:` 或 `docs:`），不与功能代码混在同一次提交里。

## 变更记录

- 2026-10-06：初版。远端、分支与 .gitignore 策略据实测量；明确不引入 worktree 与子仓库。

## 相关分册

- [spec-00-core.md](spec-00-core.md) 核心铁律
- [spec-09-build-verify.md](spec-09-build-verify.md) 构建、编译与验证
