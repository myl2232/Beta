local Lplus = require("Lplus");
local TBPanelBase = require("GUI.TBPanelBase");
local GameUtil = require("Main.GameUtil");
local LoginModule = require("Game.Login.LoginModule")
local UILogin = Lplus.Extend(TBPanelBase, "UILogin");
local def = UILogin.define;
local m_Instance = nil;
local m_obj = nil;
--类成员
def.field("userdata").mInputUser = nil;
def.field("userdata").mLoginBtn = nil;
def.field("userdata").mLogoutBtn = nil;
def.field("userdata").mUserList = nil;

-- 单例对象
def.static("=>", UILogin).Instance = function (self)
	if(m_Instance == nil) then
		m_Instance = UILogin();
    end
    print("Init AlphaWork-login!!!!");
	return m_Instance;
end

def.override().OnCreate = function (self)
    self.mInputUser = self:FindChild("canvas/InputField")
    self.mLoginBtn = self:FindChild("canvas/BtnLogin")
    self.mLogoutBtn = self:FindChild("canvas/BtnExit")
    self.mUserList = self:FindChild("canvas/UserViewList")
end

-- 预创建UI回调
def.override().DoCreate = function (self)
    self:CreateUGUIPanel(GameUtil.GetResPath(100001), 3,{});
     
end

-- 单击事件回调
def.method("userdata").onClickObj = function(self, obj)
    if(obj.name == self.mLoginBtn.name) then
        self:OnClickLogin(obj);
    elseif(obj.name == self.mLogoutBtn.name) then    
        self:OnClickLogout(obj);
    end
end

def.method("userdata").OnClickLogin = function ( self, obj )
    -- body
end

def.method("userdata").OnClickLogout = function ( self, obj )
    -- body
end

function UILogin.RegistObj( self, obj )
    -- body
    m_obj = obj;
	self:TouchUGUIGameObject(m_obj);
end

UILogin.Commit()
return UILogin
