--[[
    **************************
        面板功能注册，用于统一打开面板 调用重写的基类方法DoCreate 17/07/11
        flyfish
    **************************
]]
local GUIHandlers = {
    [EFuncType.Login] = require("Game.Login.ui.UILogin").Instance(),
    [EFuncType.InGame] = require("Game.InGame.ui.UIInGame").Instance(),

}
return GUIHandlers
