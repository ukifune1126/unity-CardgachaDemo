# unity-CardgachaDemo
a tiny card gacha in unity. 超小型抽卡demo（unity）
做的很烂，仅展示用

## 功能
- 共六张卡，抽取三张卡，三张内必含一金币卡
- 鼠标悬停在卡片上，存在卡片放大和粒子掉落效果
- 点击卡片，被选卡片移到中央并翻转显示效果
- 点击石像按钮触发抽卡
  
## 结构
- `Assets/Scripts/` : 脚本（Card.cs, CardData.cs, CardDrawManager.cs）
- `Assets/Prefabs/` : 卡片 prefab、粒子 prefab
- `Assets/Scenes/sample.unity` : 主场景

- ## 说明
- 使用screen space-camera模式，修改会导致粒子掉落特效无法显示
