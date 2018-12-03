local Lplus = require("Lplus");
local TBPanelBase = require("GUI.TBPanelBase");
local GameUtil = require("Main.GameUtil");
local LoginModule = require("Game.Login.LoginModule")
local UILogin = Lplus.Extend(TBPanelBase, "UILogin");
local def = UILogin.define;
local m_Instance = nil;
--类成员
def.field("userdata").mInputUser = nil;
def.field("userdata").mLoginBtn = nil;
def.field("userdata").mLogoutBtn = nil;
def.field("userdata").mUserList = nil;
def.field("userdata").mUserText = nil;

-- 单例对象
def.static("=>", UILogin).Instance = function (self)
	if(m_Instance == nil) then
        m_Instance = UILogin();
        print("=====================intstance UILogin ")
    end
	return m_Instance;
end

def.override().OnCreate = function (self)
    self.mInputUser = self:FindChild("Canvas/InputField")
    self.mLoginBtn = self:FindChild("Canvas/BtnLogin")
    self.mLogoutBtn = self:FindChild("Canvas/BtnExit")
    self.mUserList = self:FindChild("Canvas/UserViewList")
    --self.mUserText = self:FindChild("Canvas/InputField/Text")    
end

-- 预创建UI回调
def.override().DoCreate = function (self)
    --self:CreateUGUIPanel(GameUtil.GetResPath(100001), 3,{});
     
end

-- 单击事件回调
def.method("userdata").onClickObj = function(self, obj)
    if(obj.name == self.mLoginBtn.name) then
        self:OnClickLogin(obj);
    elseif(obj.name == self.mLogoutBtn.name) then    
        self:OnClickLogout(obj);
    elseif(obj.name == self.mUserList.name) then    
        self:OnClickUserList(obj);
    end
end

def.method("userdata").OnClickLogin = function ( self, obj )
    -- body

    local text = self.mInputUser:GetComponent("InputField").text

    if text ~= "" then
        self:CreateUserImpl(text);
        self.m_panel:GetComponent("UGUIFormExtend"):ProcedureImpl()
    end
end

def.method("userdata").OnClickLogout = function ( self, obj )
    -- body
    local arg = AlphaWork.GameToLoginEventArgs.New();--Myl:调用方式有疑问
    AlphaWork.GameEntry.Event:Fire(this, arg);
end

def.method("userdata").OnClickUserList = function ( self, obj )
    -- body
    print("--------------clicked userlist---------")
    --self.m_panel:GetComponent("UGUIFormExtend"):ProcedureImpl()
end

def.method("string").CreateUserImpl = function ( self, name )
    local player = UPlayer().New();
    player.user = name;
    player.gamesetting = GameEntry.Config.GameSetting.UID;

    local players = {};
    players = AlphaWork.GameEntry.DataBase.GetPlayerByName(name);
    --[[ AlphaWork.GameEntry.DataBase.DataDevice.GetDataByKey<AlphaWork.UPlayer>(name,out players);]]
    if players == nil or players[0] == nil then
        AlphaWork.GameEntry.DataBase.AddPlayer(player);
    else
        players[0].gamesetting = player.gamesetting;
    end 
    AlphaWork.GameEntry.Config.GameSetting.CurrentUser = name;    
end

function UILogin.RegistObj( obj )
    -- body   
    print("------------------Regist Panel----------------"..obj.name..".................")
    m_Instance:CreateUGUIPanel(GameUtil.GetResPath(100001), 1,{},obj);
    
end

UILogin.Commit()
return UILogin
