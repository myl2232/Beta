local Lplus = require("Lplus");
local TBPanelBase = require("GUI.TBPanelBase");
local GameUtil = require("Main.GameUtil");
local LoginModule = require("Game.Login.LoginModule")
local UIMainMenu = Lplus.Extend(TBPanelBase, "UIMainMenu");
local def = UIMainMenu.define;
local m_Instance = nil;
local m_obj = nil;
--类成员
def.field("userdata").mBtnStart = nil;
def.field("userdata").mBtnSetting = nil;
def.field("userdata").mBtnAbout = nil;
def.field("userdata").mBtnQuit = nil;

-- 单例对象
def.static("=>", UIMainMenu).Instance = function (self)
	if(m_Instance == nil) then
		m_Instance = UIMainMenu();
	end
	return m_Instance;
end

-- 预创建UI回调
def.override().DoCreate = function (self)
	self:CreateUGUIPanel(GameUtil.GetResPath(100000), 3,{});
end

def.override().OnCreate = function (self)
    self.mBtnStart = self:FindChild("CenterScreen/Buttons/Start")
    self.mBtnSetting = self:FindChild("CenterScreen/Buttons/Setting")
    self.mBtnAbout = self:FindChild("CenterScreen/Buttons/About")
    self.mBtnQuit = self:FindChild("CenterScreen/Buttons/Quit")
end

-- 单击事件回调
def.method("userdata").onClickObj = function(self, obj)
    if(obj.name == self.mBtnStart.name) then
        self:OnClickStart(obj);
    elseif(obj.name == self.mBtnSetting.name) then    
		self:OnClickSetting(obj);
	elseif(obj.name == self.mBtnAbout.name) then    
		self:OnClickAbout(obj);
	elseif(obj.name == self.mBtnQuit.name) then    
        self:OnClickQuit(obj);
    end
end

def.method("userdata").OnClickStart = function ( self, obj )
	-- body
	self:GetComponent("UGUIFormExtend").ProcedureImpl()
	--AlphaWork.UGUIFormExtend
end

def.method("userdata").OnClickSetting = function ( self, obj )
	-- body
	AlphaWork.GameEntry.UI.OpenUIForm(UIFormId.SettingForm)
end

def.method("userdata").OnClickAbout = function ( self, obj )
	-- body
	AlphaWork.GameEntry.UI.OpenUIForm(UIFormId.AboutForm)
end

def.method("userdata").OnClickQuit = function ( self, obj )
	-- body
	--[[ AlphaWork.GameEntry.UI.OpenDialog(new DialogParams()
	{
		Mode = 2,
		Title = GameEntry.Localization.GetString("AskQuitGame.Title"),
		Message = GameEntry.Localization.GetString("AskQuitGame.Message"),
		OnClickConfirm = delegate (object userData) { UnityGameFramework.Runtime.GameEntry.Shutdown(ShutdownType.Quit); },
	}); ]]
end

function UIMainMenu.RegistObj( obj )
	-- body		
	m_obj = obj;

	UIMainMenu:TouchUGUIGameObject(m_obj);
end

 function UIMainMenu.SetUGUIGameObject()
	if m_obj == nil then return nil end
--[[ 	local tt = panelObj:GetComponent("UGUIFormExtend")
	print(type(tt)) ]]
	if uguiMsgHandler == nil then
		uguiMsgHandler = m_obj:AddComponent("UGUIMsgHandler")
	end
	self.m_uguiMsgHandler = uguiMsgHandler
	self.m_msgTable = {
		onClick 		= self:_onEvent("onClick"),--一个参数 name
		onClickObj 		= self:_onEvent("onClickObj"),
		onStrValueChanged  = self:_onEvent("onStrValueChanged"),--两个参数 name和字符串值
		onFloatValueChanged  = self:_onEvent("onFloatValueChanged"),--两个参数 name和数值
		onIntValueChanged  = self:_onEvent("onIntValueChanged"),--两个参数 name和数值
		onBoolValueChanged  = self:_onEvent("onBoolValueChanged"),--两个参数 name和boolean值
		onRectValueChanged  = self:_onEvent("onRectValueChanged"), --返回 name,x ,y 3个参数
		onEditEnd       = self:_onEvent("onEditEnd"),--两个参数 name和字符串值
		onScroll        = self:_onEvent("onScroll"),--一个参数 name
		onDrag          = self:_onEvent("onDrag"),--返回 name,x ,y,touch_x , touch_y 5个参数
		onBeginDrag     = self:_onEvent("onBeginDrag"),--5个参数 name,x ,y,touch_x , touch_y 5个参数
		onEndDrag       = self:_onEvent("onEndDrag"),--5个参数 name,x ,y,touch_x , touch_y 5个参数
		onEnter         = self:_onEvent("onEnter"),--一个参数 name
		onExit          = self:_onEvent("onExit"),--一个参数 name
		onDown          = self:_onEvent("onDown"),--一个参数 name
		onHold          = self:_onEvent("onHold"),--一个参数 name
		onUp            = self:_onEvent("onUp"),--一个参数 name
		onUpDetail		= self:_onEvent("onUpDetail"),--5个参数 name,x ,y,touch_x , touch_y 5个参数
		onDownDetail	= self:_onEvent("onDownDetail"),--5个参数 name,x ,y,touch_x , touch_y 5个参数
		onSelect        = self:_onEvent("onSelect"),--一个参数 name
		onMove          = self:_onEvent("onMove"),--一个参数 name
		onSubmit        = self:_onEvent("onSubmit"),--一个参数 name
		onCancel        = self:_onEvent("onCancel"),--一个参数 name
		onPlayTweenFinish = self:_onEvent("onPlayTweenFinish__"),--一个参数 name
		--onGuideClickObj	= self:GuideClickOpr(),--新手引导用点击事件接受
	}
	self.m_uguiMsgHandler:SetMsgTable(self.m_msgTable,self)
	self.m_uguiMsgHandler:Touch(panelObj)
end

UIMainMenu.Commit()
return UIMainMenu