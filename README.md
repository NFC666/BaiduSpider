# NewsSpider

一个基于 **Microsoft Playwright（C#）** 的多源新闻抓取工具集，使用 **系统 Edge 浏览器**模拟真实用户行为，支持抓取多个新闻网站的热搜列表及相关新闻正文内容，并保存为 JSON 文件。

---

## ✨ 功能特性

* ✅ 使用 **Playwright（Chromium）** 自动化浏览器
* ✅ 指定 **本机 Edge 浏览器（msedge.exe）**
* ✅ 支持多个新闻源：
  * 🔍 **百度热搜** - 抓取百度首页热搜榜及相关新闻
  * 📰 **澎湃新闻** - 支持关键词搜索和按栏目采集
  * 🎓 **教育部新闻** - 按新闻类型抓取政策解读、工作动态等
  * 👷 **中工网新闻** - 按新闻类型抓取全总、工会、评论、权益等
* ✅ 防反爬延时处理
* ✅ 抓取结果自动保存为 JSON
* ✅ 提供 **WPF 图形界面**（NewsSpider）统一管理

---

## 🧱 项目结构

```
NewsSpider/
│
├── BaiduSpider/              # 百度热搜爬虫（控制台应用）
│   ├── Program.cs
│   └── BaiduSpider.csproj
│
├── ThePaperSpider/           # 澎湃新闻爬虫（控制台应用）
│   ├── Program.cs
│   └── ThePaperSpider.csproj
│
├── MoeSpider/                # 教育部新闻爬虫（控制台应用）
│   ├── Program.cs
│   └── MoeSpider.csproj
│
├── WorkerCnSpider/           # 中工网新闻爬虫（控制台应用）
│   ├── Program.cs
│   └── WorkerCnSpider.csproj
│
├── Spider.Common/             # 公共库（核心服务与模型）
│   ├── Models/                # 数据模型
│   │   ├── Baidu/            # 百度相关模型
│   │   ├── ThePaper/        # 澎湃新闻相关模型
│   │   ├── Moe/             # 教育部相关模型
│   │   ├── WorkerCn/        # 中工网相关模型
│   │   └── SpiderSource.cs  # 爬虫源枚举
│   ├── Services/             # 核心服务
│   │   ├── PlaywrightService.cs      # Playwright 基础服务（抽象类）
│   │   ├── FileService.cs            # 文件保存服务
│   │   ├── Baidu/                    # 百度爬虫服务
│   │   ├── ThePaper/                 # 澎湃新闻爬虫服务
│   │   ├── Moe/                      # 教育部爬虫服务
│   │   └── WorkerCn/                 # 中工网爬虫服务
│   └── Helpers/              # 辅助工具
│
└── NewsSpider/               # WPF 图形界面应用
    ├── Views/                # 视图
    ├── ViewModels/           # 视图模型（MVVM）
    ├── Services/             # 应用服务
    ├── Models/               # 应用模型
    └── App.xaml.cs          # 应用入口
```

---

## 📦 核心组件说明

### Spider.Common - 公共库

#### `PlaywrightService`（抽象基类）

所有爬虫服务的基类，提供 Playwright 初始化和浏览器管理功能。

```csharp
public abstract class PlaywrightService
{
    public IBrowser Browser;
    public IPage Page;
    
    public async Task InitializeAsync()
    {
        // 初始化 Playwright 并使用系统 Edge 浏览器
    }
}
```

#### `FileService`

统一的数据保存服务，支持将抓取结果保存为 JSON 文件。

```csharp
public class FileService
{
    public async Task SaveAllContentToJson<T>(
        T newsContents, 
        SpiderSource spiderSource)
    {
        // 保存为 {SpiderSource}_{时间戳}.json
    }
}
```

#### 各爬虫服务

* **`BaiduSpiderService`** - 百度热搜爬虫服务
* **`PaperSpiderService`** - 澎湃新闻爬虫服务
* **`MoeService`** - 教育部新闻爬虫服务
* **`WorkCnService`** - 中工网新闻爬虫服务

---

## 🚀 使用方式

### 1️⃣ 环境准备

#### 安装 .NET SDK

确保已安装 **.NET 10.0** 或更高版本。

#### 安装 Playwright

```bash
dotnet add package Microsoft.Playwright
```

初始化浏览器依赖：

```bash
playwright install
```

#### 系统要求

* **Windows** 操作系统（依赖 Edge 浏览器）
* **Microsoft Edge**（Chromium 内核）
* Edge 默认路径：`C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe`

---

### 2️⃣ 控制台应用使用

#### BaiduSpider - 百度热搜爬虫

```bash
cd BaiduSpider
dotnet run
```

功能：
* 自动打开百度首页
* 抓取热搜榜列表
* 进入每个热搜详情页，提取相关新闻链接（最多 10 条）
* 逐条访问新闻页面，提取正文内容
* 保存为 JSON 文件到 `./Baidu` 目录

#### ThePaperSpider - 澎湃新闻爬虫

```bash
cd ThePaperSpider
dotnet run
```

功能：
* **搜索模式**：根据关键词搜索新闻
* **采集模式**：按栏目采集新闻（时事、国际、财经、科技、暖文、智库）
* 支持分页和每页数量设置
* 保存为 JSON 文件到 `./ThePaper` 目录

#### MoeSpider - 教育部新闻爬虫

```bash
cd MoeSpider
dotnet run
```

功能：
* 交互式选择新闻类型：
  * 政策解读
  * 工作动态
* 自动抓取对应类型的新闻列表和正文
* 保存为 JSON 文件到 `./Moe` 目录

#### WorkerCnSpider - 中工网新闻爬虫

```bash
cd WorkerCnSpider
dotnet run
```

功能：
* 交互式选择新闻类型：
  * 全总
  * 工会
  * 评论
  * 权益
* 自动抓取对应类型的新闻列表和正文
* 保存为 JSON 文件到 `./WorkCn` 目录

---

### 3️⃣ WPF 图形界面使用

#### NewsSpider - 图形界面应用

```bash
cd NewsSpider
dotnet run
```

功能：
* 提供现代化的 WPF UI 界面（基于 WPF-UI）
* 统一管理多个爬虫
* 目前主要支持百度爬虫功能
* 使用 MVVM 架构模式

---

## 📦 数据模型

### 百度（Baidu）

```csharp
// 热搜条目
public class NewsCover
{
    public required string Title { get; set; }
    public required string Url { get; set; }
}

// 新闻内容
public class NewsContent
{
    public required string Url { get; set; }
    public required string Content { get; set; }
}

// 完整新闻
public class News
{
    public required NewsCover Item { get; set; }
    public required List<NewsContent> Contents { get; set; }
}
```

### 澎湃新闻（ThePaper）

```csharp
public class NewsCover
{
    public required string Title { get; set; }
    public required string Link { get; set; }
    // ... 其他属性
}

public class News
{
    public required NewsCover Cover { get; set; }
    public required string NewsContent { get; set; }
}
```

### 教育部（Moe）和 中工网（WorkerCn）

类似结构，包含 `NewsCover` 和 `News` 模型。

---

## 🛡 防反爬策略

项目中已内置基础防反爬措施：

* ⏱ **页面访问间隔延时**（`WaitForTimeoutAsync(800)`）
* 🌐 **使用真实浏览器**（Edge）
* 🧍 **非 Headless 模式**（可见浏览器窗口）
* 🔄 **顺序访问**，避免并发请求
* 📝 **模拟真实用户行为**

如需进一步加强，可扩展：

* 随机 User-Agent
* 随机延迟时间
* 页面行为模拟（滚动、点击、鼠标移动）
* 代理池支持

---

## 📄 数据存储

### 文件保存

* 使用 `FileService.SaveAllContentToJson` 统一保存
* 文件命名格式：`{SpiderSource}_{yyyyMMdd_HHmmss}.json`
* 保存位置：
  * 百度：`./Baidu/`
  * 澎湃新闻：`./ThePaper/`
  * 教育部：`./Moe/`
  * 中工网：`./WorkCn/`

### JSON 格式

所有数据以美观格式（缩进）保存，便于阅读和后续处理。

### 扩展支持

支持后续扩展为：
* CSV 格式
* 数据库存储（SQL Server、MySQL、PostgreSQL）
* Elasticsearch 索引
* 其他数据存储方案

---

## ⚠️ 注意事项

### 页面结构变化

* 各网站的页面结构 **可能随时变化**
* 若抓取失败，请优先检查：
  * CSS Selector 是否失效
  * 页面是否被重定向
  * 网站是否更新了反爬机制

### 网络稳定性

* `NetworkIdle` 在部分新闻站点可能不稳定
* 可调整为：
  ```csharp
  WaitUntil = WaitUntilState.DOMContentLoaded
  ```

### 性能考虑

* 爬虫采用顺序访问，避免对目标网站造成过大压力
* 建议在合理的时间间隔内运行
* 遵守网站的访问频率限制

---

## 🔧 技术栈

### 核心框架

* **.NET 10.0** - 开发框架
* **Microsoft Playwright** - 浏览器自动化
* **WPF** - Windows 桌面应用框架
* **WPF-UI** - 现代化 UI 组件库

### 依赖注入

* **Microsoft.Extensions.Hosting** - 依赖注入容器
* **CommunityToolkit.Mvvm** - MVVM 工具包

### 数据处理

* **System.Text.Json** - JSON 序列化
* **Newtonsoft.Json** - JSON 处理（部分服务使用）

---

## 📌 免责声明

本项目仅用于 **学习与技术研究**，请遵守目标网站的 **robots 协议** 和相关法律法规，禁止用于任何非法用途。

使用本工具时，请：

* ✅ 遵守目标网站的使用条款
* ✅ 尊重网站的访问频率限制
* ✅ 不要用于商业用途或大规模数据采集
* ✅ 不要对目标网站造成不必要的负担

---

## 📝 许可证

请查看 [LICENSE](LICENSE) 文件了解详情。

---

## 🤝 贡献

欢迎提交 Issue 和 Pull Request！

---

## 📧 联系方式

如有问题或建议，请通过 Issue 反馈。
