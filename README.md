下面是一份**可以直接放到你项目根目录的 `README.md`**，内容是按你现在这套 **Playwright + Edge + 百度热搜抓取** 的真实代码来写的，不虚、不夸，偏工程说明型。

---

# BaiduSpider

一个基于 **Microsoft Playwright（C#）** 的百度热搜新闻抓取工具，
使用 **系统 Edge 浏览器**，模拟真实用户行为，抓取热搜列表及相关新闻正文内容，并保存为 JSON 文件。

---

## ✨ 功能特性

* ✅ 使用 **Playwright（Chromium）**
* ✅ 指定 **本机 Edge 浏览器（msedge.exe）**
* ✅ 抓取百度首页 **热搜榜**
* ✅ 进入热搜详情页，提取相关新闻链接
* ✅ 逐条访问新闻页面，获取正文文本
* ✅ 防反爬延时处理
* ✅ 抓取结果自动保存为 JSON

---

## 🧱 项目结构

```
BaiduSpider
│
├── Models
│   ├── NewsItem.cs        // 热搜条目（标题 + URL）
│   └── NewsContent.cs     // 新闻内容集合
│
├── Services
│   ├── PlaywrightService.cs  // 核心爬虫逻辑
│   └── FileService.cs        // 文件保存（JSON）
│
├── Program.cs
└── README.md
```

---

## 📦 核心模型说明

### `NewsItem`

```csharp
public class NewsItem
{
    public required string Url { get; set; }
    public required string Title { get; set; }
}
```

表示百度热搜中的一条新闻入口。

---

### `NewsContent`

```csharp
public class NewsContent
{
    public required NewsItem Item { get; set; }
    public required List<string> Content { get; set; }
}
```

* `Item`：对应的热搜条目
* `Content`：抓取到的新闻正文文本列表

---

## 🚀 使用方式

### 1️⃣ 安装 Playwright

```bash
dotnet add package Microsoft.Playwright
```

初始化浏览器依赖：

```bash
playwright install
```

---

### 2️⃣ 初始化 PlaywrightService

```csharp
var playwrightService = new PlaywrightService();
await playwrightService.InitializeAsync();
```

> 默认使用系统 Edge：
>
> ```
> C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe
> ```

---

### 3️⃣ 打开百度首页

```csharp
await playwrightService.GotoBaiduHomePage();
```

---

### 4️⃣ 获取热搜列表

```csharp
var newsItems = await playwrightService.GetNewsItems();
```

---

### 5️⃣ 抓取新闻正文内容

```csharp
var contents = await playwrightService.GetNewsContent(newsItems);
```

抓取过程中会：

* 进入热搜详情页
* 提取最多前 10 条相关新闻链接
* 逐条访问并提取 `document.body.innerText`
* 自动保存为 JSON

---

## 🛡 防反爬策略

项目中已内置基础防反爬措施：

* ⏱ 页面访问间隔延时（`WaitForTimeoutAsync(800)`）
* 🌐 使用真实浏览器（Edge）
* 🧍 非 Headless 模式
* 🔄 顺序访问，避免并发

如需进一步加强，可扩展：

* 随机 User-Agent
* 随机延迟
* 页面行为模拟（滚动、点击）

---

## ⚠️ 注意事项

* 百度页面结构 **可能随时变化**

  * `li.hotsearch-item`
  * `a.title-content`
  * `div[class^='title-wrapper_']`
* 若抓取失败，请优先检查：

  * CSS Selector 是否失效
  * 页面是否被重定向
* `NetworkIdle` 在部分新闻站点可能不稳定，可调整为：

  ```csharp
  WaitUntil = WaitUntilState.DOMContentLoaded
  ```

---

## 📄 数据存储

* 使用 `FileService.SaveAllContentToJson`
* 默认保存为 JSON
* 支持后续扩展为：

  * CSV
  * 数据库
  * Elasticsearch

---

## 🔧 运行环境

* .NET 6+
* Windows（依赖 Edge）
* Microsoft Edge（Chromium 内核）

---

## 📌 免责声明

本项目仅用于 **学习与技术研究**，
请遵守目标网站的 **robots 协议** 和相关法律法规，
禁止用于任何非法用途。

---

如果你愿意，我可以再帮你：

* 📈 加一个 **爬取进度条**
* 🔄 改成 **多 Page 并发爬**
* 🧠 自动去重 / 相似新闻合并
* 📊 输出摘要 / 关键词

这套代码已经是**工程级爬虫的雏形**了。
