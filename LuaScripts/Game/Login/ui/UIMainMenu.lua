local Lplus = require("Lplus");
local TBPanelBase = require("GUI.TBPanelBase");
local GameUtil = require("Main.GameUtil");
local LoginModule = require("Game.Login.LoginModule")
local UIMainMenu = Lplus.Extend(TBPanelBase, "UIMainMenu");
local def = UIMainMenu.define;
local m_Instance = nil;

--类成员
def.field("userdata").mBtnStart = nil;
def.field("userdata").mBtnSetting = nil;
def.field("userdata").mBtnAbout = nil;
def.field("userdata").mBtnQuit = nil;

-- 单例对象
def.static("=>", UIMainMenu).Instance = function (self)
	if(m_Instance == nil) then
		m_Instance = UIMainMenu();
		print("=====================intstance UIMainMenu ")
	end
	return m_Instance;
end

-- 预创建UI回调
def.override().DoCreate = function (self)
	--self:CreateUGUIPanel(GameUtil.GetResPath(100000), 3,{});
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
--[[ 	local extendCom = m_panel:GetComponent("UGUIFormExtend");
	print("------m_panel.UIExtend-----: "..type(extendCom)) ]]

	self.m_panel:GetComponent("UGUIFormExtend"):ProcedureImpl()
end

def.method("userdata").OnClickSetting = function ( self, obj )
	-- body	
	AlphaWork.UIFunctionTools.OpenUIFromId(UIFormId.SettingForm);
end

def.method("userdata").OnClickAbout = function ( self, obj )
	-- body		
	--AlphaWork.GameEntry.UI.OpenUIForm(AlphaWork.GameEntry.UI,UIFormId.AboutForm)
	AlphaWork.UIFunctionTools.OpenUIFromId(UIFormId.AboutForm);
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
	print("------------------Regist Panel----------------"..obj.name..".................")
	m_Instance:CreateUGUIPanel(GameUtil.GetResPath(100000), 1,{},obj);
end

UIMainMenu.Commit()
return UIMainMenu