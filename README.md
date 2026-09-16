# 山海灵途 | Shan Hai Spirit Trail

31263 / 32004 Introduction to Game Development, Assessment 3.

一个以青衣修仙者和山海异兽为主题的 2D PacStudent 重制项目。
以灵气收集、破厄丹和封印表现原作交互，保留作业要求的关卡与运动规则。

## 当前阶段

目标为 **100% HD**，按各评分档位顺序推进。已建立并验证 Unity 工程基础，
下一阶段是 `Feature-Audio` 的音频准备与开场播放。
尚未完成音频、视觉素材、动画、手动关卡、移动或程序化关卡生成；当前内容不能作为已完成作业提交。

- Unity：**6000.4.11f1**（用户已确认）。
- 远程仓库：[Assessment-3---Starting-Game-Recreation](https://github.com/guohongying31-cyber/Assessment-3---Starting-Game-Recreation)。
- 开发分支：`Development`；每次只开展一个功能分支。
- 主分支：`Main`，最终验收之前不接收开发合并。

## 文档入口

- [评分要求与验收](Documentation/Assessment-Checklist.md)
- [主题及素材设计建议](Documentation/Theme-Brief.md)
- [真实开发里程碑](Documentation/Development-Plan.md)
- [AI 协助记录](Documentation/AI-Assistance.md)
- [环境与验证记录](Documentation/Validation.md)

美术和游戏代码的制作遵循 PDF 的本人创作要求；
AI 的计划、解释及检查结果如实记录，不替代学生本人完成的素材和代码。

## 打开工程

在 Unity Hub 中添加本目录，以 6000.4.11f1 打开。
场景路径：`Assets/Scenes/RecreatedLevel.unity`。
此场景目前只有正交相机和 `Systems`、`Level01_Manual`、`Characters`、
`AssetShowcase` 四个空分组；没有已完成的手动关卡，按 Play 只会看到空背景。

## Git 与提交

每个可验收的小里程碑单独提交。所有功能分支均从最新 `Development` 创建，
测试通过后合并回去并保留分支。最终检查通过后才合并到 `Main`，再按
`studentNumber_Assess3.zip` 打包，保留 `.git`、`.gitignore`，排除 `Library`。

`.gitignore` 采用作业指定的
[GitHub Unity 模板](https://github.com/github/gitignore/blob/main/Unity.gitignore)，
末尾另加本项目临时文件和本地参考资料的忽略规则。
