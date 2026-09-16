# 验证记录：工程准备阶段

日期：2026-09-16（Australia/Sydney）。这些是实际执行的检查，不是最终作业验收。

## 已执行

| 检查 | 结果与范围 |
|---|---|
| 指定编辑器 | Unity 6000.4.11f1，revision b0a1d6caadd2；用户已确认 |
| 初次受限环境启动 | 卡在许可证客户端连接，未计为通过；已停止该次启动 |
| 正常用户环境重新建工程 | Unity 日志报告成功退出，return code 0 |
| 临时 Editor 配置 | 2D 模式、文本序列化、正交相机和四个空分组保存成功；Unity 日志输出 `ASSESSMENT_FOUNDATION_OK` |
| 移除配置工具后再次导入 | Unity 进程退出码 0；没有 C# 编译错误、编译失败或异常记录 |
| 工程设置 | Visible Meta Files；1920×1080；启动场景列表包含 RecreatedLevel |
| 依赖 | 移除默认生成的 Multiplayer Center，只保留 Unity 内置模块 |
| 资源静态检查 | 25 个唯一 GUID；场景正好有相机加四个空分组；没有游戏或工具 C#／DLL；所有本地文档链接可解析 |
| CSV 数值区 | 15×14，与 PDF 第 11 页数组逐元素一致；与 CSV 字符区逐元素一致 |
| 参考镜像计算 | 29×28，中线不重复；218 个普通收集物、4 个强化收集物；见 Reference/Map-Validation.json |
| 远程仓库检查 | origin 已设置到用户提供的 GitHub 仓库；首次读取没有现有分支 |

本地原始日志存于被忽略的 `tmp`：`unity-create.log`、`unity-create-retry.log`、
`unity-foundation.log`、`unity-verify-foundation.log`。日志未放入提交。
个别成功运行日志中可见启动许可证重连消息和退出时的 Curl callback aborted；
据此没有声称“所有日志完全无警告”。通过判断使用进程退出、导入完成和编译结果。

## 当前实现边界

- 场景只有相机与空分组，没有手动迷宫、角色、Sprite、Animator 或音频。
- 没有提交任何游戏 C# 脚本；临时 Editor 配置脚本已删除。
- 未执行游戏 Play 验收、玩家构建、帧率移动测试或程序生成测试。
- 地图校验结果是参考数据，不能替代手动布局或 LevelGenerator 的评分证据。
- 未创建最终 ZIP，也没有将 Development 合并到 Main。

## 本轮 Git 收尾

基础工程与设计文档分别作为真实里程碑提交。本文件记录截至 Feature-Setup
合并前的验证结果；实际合并和远程同步以 Git 提交图与远程分支为准。
后续每个功能分支需重新记录对应的 Unity 编译和实际运行结果。
