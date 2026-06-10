一、玩家核心系统
脚本	功能
PlayerController.cs	玩家移动（走/跑/潜行）、跳跃、重力、动画驱动、上车检测（F键触发）
MouseCamera.cs	第三人称鼠标相机：跟随玩家位置、鼠标控制视角旋转和俯仰
PlayerInteraction.cs	射线检测告示牌交互（E键查看详情弹窗），查看时锁定移动和相机
关联：MouseCamera.player 引用玩家 Transform；PlayerInteraction 通过 MouseCamera.canControlCamera 静态变量控制相机锁定
二、车辆系统
脚本	功能
CarControl.cs	车辆驾驶（移动/转向）、G键下车、下车时重生玩家并恢复相机
关联：
PlayerController.EnterCar() → 传引用给 CarControl → 调用 CarControl.StartControl()
CarControl.ExitCar() → 实例化玩家 → 找到 MouseCamera 重新绑定新玩家
三、背包/物品系统
脚本	功能
ItemData.cs	物品数据定义（SO）：名称、图标、描述、堆叠上限、是否可丢弃
InventorySlot.cs	单个背包格子 UI 组件：显示图标、数量、选中高亮
HotbarSystem.cs	快捷栏控制器：1-9/滚轮切换、Q丢弃、AddItem 添加物品（单例）
HotbarShowHide.cs	快捷栏显隐动画：滚轮触发显示，定时自动隐藏
InventoryManager.cs	物品栏管理器（单例+DontDestroyOnLoad），27格背包数据管理
ItemHintSystem.cs	选中物品时显示提示文字，5秒自动消失（单例）
ItemDescManager.cs	H键查看物品详情面板（单例）
PickupItem.cs	场景中可拾取物品：靠近后E键拾取到快捷栏
关联链：
PickupItem.Pick() → HotbarSystem.Instance.AddItem() → InventorySlot.UpdateSlot()
                                                     → HotbarShowHide.ShowHotbarOnPickup()
ItemHintSystem ← 监听 → HotbarSystem.selectedIndex
ItemDescManager ← 读取 → HotbarSystem.hotbarSlots[current]
四、任务/对话系统
脚本	功能
QuestDataSO.cs	任务数据（SO）：名称、提示、NPC标签、对话CSV、完成状态
QuestSystem.cs	任务管理器（单例）：P键开始、O键查看、链式任务推进
QuestPanel.cs	任务面板UI：淡入→停留→淡出显示任务名和提示
UI1.cs	NPC对话面板：F开启、E下一句/加速、ESC关闭、完成后推进任务
TimelineAutoDisableAfter7s.cs	Timeline动画+黑屏打字对话，P键关闭，自动锁定玩家
关联链：
QuestSystem.StartQuest() → QuestPanel.Show() + 激活 UI1.dialogueUI
UI1.CompleteDialogue() → QuestSystem.NotifyQuestComplete() → 自动开启下一个任务
UI1 ← 读取 → QuestDataSO.dialogueData（CSV）
五、场景/环境交互
脚本	功能
Billboard.cs	告示牌数据载体：持有 SignData，显示交互提示
SignData.cs	告示牌数据结构：图片+描述文字
DoorTransform.cs	双向传送门（E键），射线检测，传送前禁用 CharacterController
SkyboxTransition.cs	K键切换天空盒
PhotoCameraSystem.cs	独立拍照相机：自由移动/旋转、鼠标拍照、照片存入快捷栏
PlayerCameraInput.cs	F键使用拍照相机（需背包中选中相机道具）
SettingsManager.cs	设置面板：音量、全屏、画质、分辨率
DontDestroy.cs	DontDestroyOnLoad 跨场景保留标记
menu.cs	主菜单：加载 "mainscene" 场景
关联：
PlayerInteraction ← 射线检测 → Billboard ← 持有 → SignData
PlayerCameraInput ← 检查 → HotbarSystem.selectedIndex == cameraItem
PhotoCameraSystem.OpenCamera() → 禁用 PlayerController + MouseCamera
PhotoCameraSystem.TakePhoto() → HotbarSystem.AddItem(photoItem)
整体系统关联图
PlayerController ──上车F──→ CarControl ──下车G──→ 重生玩家 → MouseCamera
      │                         │
      ├─ CharacterController    ├─ transform.Translate（移动）
      ├─ Animator               └─ transform.Rotate（转向）
      └─ MouseCamera.player

PickupItem ──拾取E──→ HotbarSystem ──选中──→ PlayerCameraInput ──使用F──→ PhotoCameraSystem
                            │
                            ├─→ ItemHintSystem（显示提示）
                            ├─→ ItemDescManager（H键详情）
                            └─→ InventoryManager（数据同步）

QuestSystem ──开始任务──→ QuestPanel（UI显示）
      │                   UI1（NPC对话）
      └── NotifyQuestComplete ←── UI1.CompleteDialogue()
