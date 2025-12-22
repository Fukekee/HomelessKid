# Input System 设置说明

## 问题说明

代码现在使用 `InputActionAsset` 来访问输入，而不是生成的包装类。这需要在Unity编辑器中手动设置引用。

## 设置步骤

### 方法1：在Inspector中设置（推荐）

1. 在场景中选择Player GameObject
2. 找到以下组件：
   - `PlayerMovementController`
   - `PlayerCameraController`
   - `InteractionManager`
3. 在Inspector中，将 `Assets/InputSystem_Actions.inputactions` 拖拽到这些组件的 "Input Actions" 字段中

### 方法2：自动加载（仅限Editor）

代码中已经包含了自动加载逻辑，在Unity编辑器中会自动查找 `InputSystem_Actions` 资产。如果找不到，请确保：
- 文件名为 `InputSystem_Actions.inputactions`
- 文件位于 `Assets/` 目录下（或其子目录）

## 运行时注意事项

如果需要在运行时动态加载InputActions，可以考虑：
1. 将InputActions文件放在Resources文件夹中
2. 使用 `Resources.Load<InputActionAsset>("InputSystem_Actions")`

## 验证设置

设置完成后，运行游戏应该不再有编译错误，并且输入应该正常工作。


