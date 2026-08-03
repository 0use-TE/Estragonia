# DocFX 文档系统 — AI 工作提示词（Estragonia）

> 复制给其他 AI，用于维护本仓库 DocFX 文档。  
> 官方文档：https://dotnet.github.io/docfx/

---

## 一、给 AI 的总提示词（可直接复制）

```
你在维护 Estragonia 的 DocFX 文档时，必须遵循：

1. 版本切换：只用顶栏左侧 Version 下拉（不要在根 toc 放版本号链接）。根 toc.yml：Home | Docs（指向最新版）| API Reference。默认最新版 v1.0.0。
2. 语言切换：只用顶栏 navbar 的 Lang 下拉（English / 简体中文）。正文里不要写行内双语链接。
3. 双语结构：docs/<version>/（英文）与 docs/<version>/zh-CN/（中文）镜像；文件名一一对应。当前版本目录：docs/v1.0.0/。
4. GitHub Pages 项目站：globalMetadata._appBasePath 按实际 Pages 路径调整（本地预览可留空字符串）。
5. docfx.json template：["default", "modern", "docfx/template"]；顶栏开关由 docfx/template/public/main.js 注入 dk-switcher.js（versions 数组须与文件夹名一致，最新在前）。若需深度定制布局再加 layout/_master.tmpl。
6. introduction.md 用 redirect_url: getting-started.html。
7. 侧边栏 toc：每语言各一份 toc.yml，只放文档章节。
8. api/ 与 _site/ 不提交（已在 .gitignore）；改完执行 docfx docfx.json，要求 0 error。
9. 跨语言/跨版本同页切换由 dk-switcher.js 按 html 文件名映射（docPages 含全部章节 stem）。
10. 面向「用框架写业务」的提示词写在 docs/<ver>/ai-prompt.md（及 zh-CN 镜像）；本文件只负责 DocFX 站点维护规则。
11. 发新版本文档时：复制 docs/上一版 → docs/vX.Y.Z，更新 _master.tmpl 的 option、dk-switcher.js 的 versions（最新在前）、根 toc.yml 的 Docs 指向最新版，并同步 docPages。
12. 文档须标明：本仓库含 AI 辅助代码、不保证稳定性、维护者会审查；并保留对 Julien Lebosquain / 原版 Estragonia（MIT）的署名。
13. 每个功能章节尽量有「Minimal runnable example / 最小可运行示例」。
```

---

## 二、本仓库目录结构

```
仓库根/
├── docfx.json
├── toc.yml
├── index.md
├── DOCFX-AI-PROMPT.md
├── docfx/template/
│   ├── layout/_master.tmpl
│   └── public/dk-switcher.{js,css}
├── docs/v1.0.0/
│   ├── toc.yml
│   ├── getting-started.md
│   ├── hosting.md
│   ├── input-and-rendering.md
│   ├── ai-prompt.md
│   ├── release-notes.md
│   └── zh-CN/
├── api/                        # .gitignore — docfx metadata 生成
└── _site/                      # .gitignore — 站点输出
```

---

## 三、常用命令

需本机已安装 DocFX（`dotnet tool install -g docfx` 或见官网）。

```bash
docfx docfx.json
docfx serve _site --port 8080
```

仅重新生成 API：

```bash
docfx metadata docfx.json
```

---

## 四、新增文档页 checklist

1. 在 `docs/v1.0.0/` 写英文 `.md`
2. 在 `docs/v1.0.0/zh-CN/` 写同名中文 `.md`
3. 两边 `toc.yml` 各加一项
4. 在 `dk-switcher.js` 的 `docPages` Set 里加页面 stem（无扩展名）
5. `dotnet docfx docfx.json` 验证 0 error
