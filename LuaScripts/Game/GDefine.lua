--给通用逻辑定义
_G.client_define_=true
_G.warn = function ( str )
    Debugger.LogWarning(str.."\n"..debug.traceback())
end 
_G.error = function ( str )
    Debugger.LogError(str.."\n"..debug.traceback())
end  
--[[
    Unity导出模块方便使用
]]
--[[ _G.Image =UnityEngine.UI.Image
_G.RawImage =UnityEngine.UI.RawImage
_G.Text =UnityEngine.UI.Text
_G.InputField =UnityEngine.UI.InputField
_G.Button =UnityEngine.UI.Button
_G.Toggle =UnityEngine.UI.Toggle
_G.ToggleGroup =UnityEngine.UI.ToggleGroup
_G.Slider =UnityEngine.UI.Slider
_G.Scrollbar =UnityEngine.UI.Scrollbar
_G.Selectable =UnityEngine.UI.Selectable
_G.Dropdown =UnityEngine.UI.Dropdown
_G.ScrollRect =UnityEngine.UI.ScrollRect  ]]

_G.Canvas =UnityEngine.Canvas
_G.Input =UnityEngine.Input
_G.KeyCode =UnityEngine.KeyCode
_G.Screen =UnityEngine.Screen

_G.GameObject =UnityEngine.GameObject
_G.Quaternion = UnityEngine.Quaternion
--[[
    ugui导出接口结束
]]

--myl:窗口id（同cs）
_G.UIFormId =
{
    Undefined = 0,
    DialogForm = 1,
    MenuForm = 100,
    SettingForm = 101,
    AboutForm = 102,
    LoginForm = 2,
    MainForm = 3,
}

_G.ScreenResolution = Screen.width / Screen.height

_G.UILevel = 
{
    NoManager = 0, --自己管理销毁逻辑
    HideSameLevel =1, --打开新界面隐藏同级旧界面并销毁之前打开Popup界面
    L1Popup =2,       -- 弹出界面 对应一级界面被销毁时此界面也会被销毁
    DestroySameAndL1 =3,--打开此类型界面会销毁一级界面和同级界面
    Max =4,
}

_G.TBAtlas=
{  
    temporary    = "temporary",
    skillatlas = "tbskillatlas",
    common       = "common",
    item  = "item",
    fabao = "fabaoatlas",
    match = "battlematch",
    battle = "battleatlas",
    head = "headatlas",
    sticker = "sticker",
}

_G.TextColor=
{
    Green = UnityEngine.Color(254 / 255, 253 / 255, 226 / 255);
    Red = UnityEngine.Color(214 / 255, 67 / 255, 56 / 255);
    Brown = UnityEngine.Color(50 / 255, 17 / 255, 13 / 255);
    Yellow = UnityEngine.Color(252 / 255, 214 / 255, 131 / 255);
}

_G.TextOutline=
{
    OutlineWithGreen = UnityEngine.Color(115 / 255, 82 / 255, 66 / 255);
    OutlineWithRed = UnityEngine.Color(84 / 255, 8 / 255, 7 / 255);
}

_G.EntityStateType =
{
    Idle =1,
    Attack =2,
    BeAttacked =3,
    Dead =4,
    MoveBack =5,
    Born = 6,
    Show = 7,
}
