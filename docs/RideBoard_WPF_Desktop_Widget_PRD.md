# RideBoard 产品需求文档 (PRD)

> **版本**: 1.1
> **更新日期**: 2025-12-22
> **状态**: 已实现 (Implemented)

---

## 1. 项目概述

**RideBoard** 是一个运行在 Windows 桌面上的极简骑行数据挂件。它通过本地服务连接 Strava API，将关键的训练数据实时展示在桌面上，提供一种“低打扰、持续反馈”的数据消费体验。

---

## 2. 核心功能 (已实现)

### 2.1 数据展示面板
挂件包含两个主要视图，支持左右翻页切换：

**视图 1：日常数据 (Daily View)**
*   **TODAY (今日)**: 骑行距离 (km)、移动时间、爬升 (m)。
*   **LAST RIDE (最近一次)**: 距离、平均功率 (W)、平均心率 (bpm)。
*   **THIS WEEK (本周)**: 累计距离、累计时间。

**视图 2：年度总结 (Yearly Summary)**
*   **年份范围**: 自动显示当前年份 (e.g., 2025年1月1日 - 12月31日)。
*   **核心指标**:
    *   Total Distance (年度总里程)
    *   Total Elevation (年度总爬升)
    *   Total Moving Time (年度总时长)

### 2.2 系统集成与交互
*   **软件化封装**:
    *   **隐形启动**: 后端 Python 数据服务静默运行，无终端弹窗。
    *   **系统托盘**: 支持最小化到任务栏托盘，提供右键菜单 (显示/退出)。
*   **设置管理**:
    *   独立设置窗口。
    *   **开机自启**: 支持一键开启/关闭 Windows 开机自动运行。
    *   **Strava 登录**: 集成 OAuth 授权流程。
*   **刷新机制**:
    *   自动刷新：每 45 秒更新 UI，每 10 分钟拉取 API。
    *   手动刷新：底部提供强制刷新按钮 (⟳)，支持即时数据同步。
*   **UI 细节**:
    *   底部显示最后更新时间。
    *   半透明磨砂深色背景，圆角设计。
    *   置顶显示，支持拖拽。

---

## 3. 技术架构

*   **前端 (Widget)**:
    *   框架: WPF / .NET 10.0
    *   模式: MVVM
    *   特性: P/Invoke 窗口控制, System.Windows.Forms 托盘集成
*   **后端 (Server)**:
    *   语言: Python 3
    *   服务: `http.server`
    *   功能: Strava OAuth2, API 代理, 本地缓存 (JSON)

---

## 4. 文件结构

```
rideboard/
├─ server/                # Python 数据服务
│  ├─ src/server.py       # 服务入口
│  ├─ src/strava_api.py   # Strava API 封装
│  └─ data/               # 缓存与 Token 存储
├─ widget/                # WPF 客户端
│  ├─ MainWindow.xaml     # 主界面
│  ├─ SettingsWindow.xaml # 设置界面
│  └─ ViewModels/         # MVVM 逻辑
└─ start_widget.bat       # 一键启动脚本 (自动环境检查)
```

---

## 5. 后续规划

*   支持更多运动类型过滤 (Run/Swim) 的配置项。
*   自定义主题颜色。
*   历史数据图表化展示。
