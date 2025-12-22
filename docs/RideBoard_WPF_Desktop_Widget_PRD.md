# RideBoard 产品需求文档 (PRD)

> **版本**: 1.2
> **更新日期**: 2025-12-22
> **状态**: 已发布 (Released)

---

## 1. 项目概述

**RideBoard** 是一个运行在 Windows 桌面上的极简骑行数据挂件。它通过本地服务连接 Strava API，将关键的训练数据实时展示在桌面上，提供一种“低打扰、持续反馈”的数据消费体验。

---

## 2. 核心功能 (已实现)

### 2.1 数据展示面板
挂件包含两个主要视图，支持左右翻页切换，采用数字仪表盘风格设计：

**视图 1：日常数据 (Daily View)**
*   **LAST RIDE (最近一次)**:
    *   距离 (km)
    *   时长 (HHH:MM:SS)
    *   爬升 (m)
    *   平均功率 (W)
*   **THIS WEEK (本周)**:
    *   累计距离 (km)
    *   累计时长 (HHH:MM:SS)
    *   累计爬升 (m)

**视图 2：年度总结 (Yearly Summary)**
*   **年份范围**: 自动显示当前年份 (e.g., 2025年1月1日 - 12月31日)。
*   **核心指标**:
    *   Total Distance (年度总里程)
    *   Total Elevation (年度总爬升)
    *   Total Moving Time (年度总时长, 格式 HHH:MM:SS)

### 2.2 系统集成与交互
*   **软件化封装**:
    *   **单文件分发**: WPF 客户端与 Python 环境打包为独立的 `.exe`，无需用户配置环境。
    *   **隐形启动**: 后端 Python 数据服务静默运行，无终端弹窗。
    *   **系统托盘**: 动态生成的品牌图标 (橙色圆底白色'R')，提供右键菜单 (显示/退出)。
*   **设置管理**:
    *   独立设置窗口 (齿轮图标入口)。
    *   **开机自启**: 支持一键开启/关闭 Windows 开机自动运行。
    *   **Strava 登录**: 集成 OAuth 授权流程。
    *   **关于信息**: 包含版本号及 GitHub 仓库链接。
*   **刷新机制**:
    *   自动刷新：后台定时拉取。
    *   手动刷新：底部提供强制刷新按钮 (⟳)，支持即时数据同步 (绕过缓存)。
*   **UI 细节**:
    *   **高分屏适配**: 界面整体缩放 1.5x，字体清晰。
    *   **视觉风格**: 深色磨砂半透明背景 (Transparency ~80%)，橙色 (DigitalAmber) 高亮关键数据。
    *   **窗口行为**: 无边框窗口，支持拖拽，默认不置顶 (可作为桌面摆件)。

---

## 3. 技术架构

*   **前端 (Widget)**:
    *   框架: WPF / .NET 10.0
    *   模式: MVVM
    *   特性: P/Invoke 窗口控制, System.Windows.Forms 托盘集成, 动态图标生成 (GDI+)
*   **后端 (Server)**:
    *   语言: Python 3
    *   服务: `http.server`
    *   功能: Strava OAuth2, API 代理, 本地缓存 (JSON)
*   **构建与发布**:
    *   工具: `publish_release.bat`
    *   产物: 包含嵌入式 Python 运行时的独立文件夹。

---

## 4. 文件结构

```
rideboard/
├─ server/                # Python 数据服务
│  ├─ src/server.py       # 服务入口
│  ├─ src/strava_api.py   # Strava API 封装
│  └─ data/               # 缓存与 Token 存储
├─ widget/                # WPF 客户端
│  ├─ MainWindow.xaml     # 主界面 (Scale 1.5x)
│  ├─ SettingsWindow.xaml # 设置界面
│  └─ ViewModels/         # MVVM 逻辑
└─ publish_release.bat    # 一键发布脚本
```

---

## 5. 后续规划

*   支持更多运动类型过滤 (Run/Swim) 的配置项。
*   自定义主题颜色。
*   历史数据图表化展示。
