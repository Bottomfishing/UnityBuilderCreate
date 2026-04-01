# WaveStarterUI 配置指南

## 问题：WaveStarterUI 显示不出来

### 原因分析
WaveStarterUI 显示不出来通常有以下原因：
1. **场景中没有 WaveStarterUI 实例**
2. **WaveStarterUI 不在 Canvas 下**
3. **UI 组件引用没有配置**

---

## 解决方案：在 Unity 场景中正确配置

### 第一步：创建 WaveStarterUI 实例

1. **打开游戏场景** (LevelScene.unity)

2. **找到 Canvas 对象**
   - 在 Hierarchy 中查找 Canvas
   - 如果没有，右键 → UI → Canvas 创建一个

3. **创建 WaveStarterUI 对象**
   - 方法 A：从 Prefabs 文件夹拖拽 `WaveStarterUI` 预制体到 Canvas 下
   - 方法 B：在 Canvas 下创建空对象，命名为 `WaveStarterUI`，挂载 `WaveStarterUI` 脚本

### 第二步：配置 UI 组件引用

选中 `WaveStarterUI` 对象，在 Inspector 中配置：

#### 必需的 UI 组件：
```
WaveStarterUI (脚本)
├── Start Button: 拖入开始按钮
├── Countdown Circle: 拖入倒计时圈 Image
├── Countdown Text: 拖入倒计时数字 Text
├── Bonus Text: 拖入奖励金币 Text
└── Wave Number Text: 拖入波次数 Text
```

#### 创建子对象结构：
```
WaveStarterUI
├── StartButton (Button)
│   └── Background (Image)
├── CountdownCircle (Image)
│   - Image Type: Filled
│   - Fill Method: Radial 360
│   - Fill Origin: Top
│   - Clockwise: 取消勾选
├── CountdownText (Text)
├── BonusText (Text)
└── WaveNumberText (Text)
```

### 第三步：配置 WaveStarterManager

1. **找到或创建 WaveStarterManager 对象**
   - 在 Hierarchy 中创建空对象，命名为 `WaveStarterManager`
   - 挂载 `WaveStarterManager` 脚本

2. **配置引用**
   ```
   WaveStarterManager (脚本)
   ├── Settings: 可以留空（会自动创建默认设置）
   ├── Wave Starter UI: 拖入场景中的 WaveStarterUI 对象
   └── Zombie Spawner: 拖入场景中的 ZombieSpawner 对象
   ```

### 第四步：配置 ZombieSpawner

1. **找到 ZombieSpawner 对象**

2. **配置引用**
   ```
   ZombieSpawner (脚本)
   └── Wave Starter Manager: 拖入场景中的 WaveStarterManager 对象
   ```

---

## 验证配置是否正确

### 运行游戏后查看 Console 日志

应该看到以下日志序列：

```
WaveStarterUI: Awake called
WaveStarterUI: Start button listener added
WaveStarterUI: Start called, hiding UI
WaveStarterManager: ZombieSpawner found: True
WaveStarterManager: WaveStarterUI found: True
WaveStarterManager: UI initialized successfully
ZombieSpawner: DelayedStartGame called
ZombieSpawner: WaveStarterManager found: True
ZombieSpawner: Calling WaveStarterManager.StartGame()
WaveStarterManager: StartGame called
WaveStarterManager: Preparing first wave 1
WaveStarterManager: Showing UI for wave 1
WaveStarterUI: ShowStartWave called, wave 1, waitTime 10
WaveStarterUI: Show called, setting active to true
WaveStarterUI: Active state is now: True
```

### 如果看到警告日志

- `WaveStarterUI: Start button is not assigned!` → 需要配置 Start Button 引用
- `WaveStarterUI: waveNumberText is not assigned!` → 需要配置 Wave Number Text 引用
- `WaveStarterManager: WaveStarterUI found: False` → 场景中没有 WaveStarterUI 实例

---

## 快速测试方法

### 方法 1：使用预制体
1. 找到 `Assets/Prefabs/WaveStarterUI.prefab`
2. 拖拽到 Canvas 下
3. 确保 WaveStarterManager 的 Wave Starter UI 字段引用了这个对象

### 方法 2：手动创建
如果预制体不可用，手动创建：

1. 在 Canvas 下创建空对象 `WaveStarterUI`
2. 添加 `WaveStarterUI` 脚本
3. 创建子对象：
   - `CountdownCircle` (UI → Image)
     - 设置 Image Type 为 Filled
     - 设置 Fill Method 为 Radial 360
   - `CountdownText` (UI → Text)
   - `BonusText` (UI → Text)
   - `WaveNumberText` (UI → Text)
4. 将这些子对象拖到脚本的对应字段

---

## 常见问题

### Q: UI 显示了但是点击没反应？
A: 检查：
- CountdownCircle 是否有 Button 组件
- EventSystem 是否存在（Canvas 创建时会自动创建）

### Q: UI 显示位置不对？
A: 调整 WaveStarterUI 的 RectTransform：
- Anchor: Center
- Position: (0, 0, 0)

### Q: 倒计时圈不显示？
A: 检查：
- CountdownCircle 的 Image 组件是否启用
- Source Image 是否设置
- Color 的 Alpha 是否为 255

---

## 完整的组件检查清单

- [ ] Canvas 存在
- [ ] WaveStarterUI 在 Canvas 下
- [ ] WaveStarterUI 脚本已挂载
- [ ] 所有 UI 组件引用已配置
- [ ] WaveStarterManager 对象存在
- [ ] WaveStarterManager 脚本已挂载
- [ ] Wave Starter UI 引用已配置
- [ ] Zombie Spawner 引用已配置
- [ ] ZombieSpawner 的 Wave Starter Manager 引用已配置
- [ ] EventSystem 存在

完成以上配置后，WaveStarterUI 应该能正常显示了！
