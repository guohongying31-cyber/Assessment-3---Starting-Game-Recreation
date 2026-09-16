# 顺序开发计划

下表中的提交信息只是未来里程碑建议；只有实际完成对应改动后才使用。
没有预建全部功能分支，没有用空提交、预设日期或拆分已有成品来制造进度。

## 本轮：Feature-Setup

1. 建立 Main、Development 和 Feature-Setup，提交 Git 工作流基础。
2. 用指定 Unity 版本建立并导入 2D 空工程，建立清晰目录和空场景分组。
   提交建议：`chore: create Unity 6000.4.11f1 2D project foundation`。
3. 核对材料，加入主题说明、逐档验收、地图数值核对和 AI 协助记录。
   提交建议：`docs: map assessment requirements to Shan Hai development milestones`。
4. 重新导入工程，核对 Git 状态和所有待合并文件；通过后将 Feature-Setup
   合并 Development，保留该分支。Main 只保留最初的仓库基础提交。

空工程成功导入只证明工程基础有效，不代表音频、素材或场景评分档位已完成。

## 后续：每次从最新 Development 创建下一个分支

| 顺序／分支 | 小里程碑与建议提交信息 | 合并前的验收证据 |
|---|---|---|
| 1 / Feature-Audio | `audio: add five licensed music cues and provenance`；`audio: add six interaction effects and audition notes`；`feat: play intro then loop normal-state music` | 11 类音频齐全、许可核对、逐条试听；Intro 最多 3 秒后切换并循环；Unity 编译及 Play 通过 |
| 2 / Feature-Visual | `art: import student-drawn cultivator directional and death frames`；`art: add four student-drawn Shan Hai creature sprite sets`；`art: add pickups life icon and six base wall sprites`；`anim: configure cultivator and power pellet controllers`；`anim: configure ghost state cycles and scene showcase` | 帧数及方向齐全；场景可见所有素材；每只异兽全部 10 状态轮播；控制器命名和时长满足规范 |
| 3 / Feature-ManualLevel | `level: place and verify the top-left maze quadrant`；`level: mirror quadrants and fix center-row seams`；`level: frame the manual maze and place animated power pellets` | Scene View 中手动关卡存在；墙体连接、28×29 尺寸、4 枚强化收集物、无重复中线、全景相机 |
| 4 / Feature-Movement | `feat: tween cultivator clockwise around the first inner block`；`anim: synchronize direction changes with movement audio`；`fix: preserve tween speed across frame boundaries`（仅确有修复时） | 四段相同速度、即时转向；不同帧率整圈时间；移动音效；不使用禁用运动 API |
| 5 / Feature-LevelGenerator | `feat: instantiate a quadrant from the numeric level map`；`feat: infer wall rotations from neighboring tiles`；`feat: mirror generated quadrants and fit camera bounds`；`fix: handle generator edge cases from alternate maps`（按真实问题命名） | 默认图与手动图一致；不同尺寸合法图；角、T、出口和接缝；Stop 恢复手动关卡；没有 Rule Tiles |
| 最终 / 独立验证阶段 | `docs: record final play-mode and repository validation`；完整通过后 Development 合并 Main | 最低档到最高档逐项重验、实际远程分支、无缓存／私密文件、ZIP 解压复查 |

不要为了满足示例提交数量人为制造修复。若一个步骤尚未完成，可以真实提交其
已经完成的可解释部分，但不能标记功能完成或提前合并。之后继续在同一分支。

## 每次合并的固定门槛

1. 预告当前分支、改动、提交／合并信息和将要合并的方向。
2. 检查当前 `git status` 和确切差异文件清单。
3. 使用 Unity 6000.4.11f1 重新编译／导入；有游戏功能时实际 Play 测试。
4. 确认未混入 Library、Logs、UserSettings、构建输出和临时工具。
5. 工作树干净后，以保留分支结构的合并提交合入 Development，保留原分支。
6. 同步实际远程分支；创建下一功能前从最新 Development 出发。

## 本人创作与 AI 配合

当前 PDF 允许咨询 AI，但不允许整段复制生成的游戏代码。后续可让 AI 帮忙解释
tween 原理、逐条分析学生代码中的错误、核对动画和地图、建议测试用例及操作 Git。
游戏代码与最终美术由学生本人实现，实际利用的建议持续写入 AI-Assistance.md。
不要将本计划当作可以直接提交的完成证明。
