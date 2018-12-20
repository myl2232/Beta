local Lplus = require("Lplus");
local TBPanelBase = require("GUI.TBPanelBase");
local GameUtil = require("Main.GameUtil");
local LoginModule = require("Game.Login.LoginModule")
local UILogin = Lplus.Extend(TBPanelBase, "UILogin");
local def = UILogin.define;
local m_Instance = nil;
local LoginModule = require("Game.Login.LoginModule").Instance();
--类成员
def.field("userdata").mInputUser = nil;
def.field("userdata").mLoginBtn = nil;
def.field("userdata").mLogoutBtn = nil;
def.field("userdata").mUserList = nil;
def.field("userdata").mUser = nil;

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
    self.mUser = self.mInputUser:GetComponent("InputField")
end

-- 预创建UI回调
def.override().DoCreate = function (self)
    --self:CreateUGUIPanel(GameUtil.GetResPath(100001), 3,{});
     
end

-- 创建完成回调
def.override().AfterCreate = function(self)
    Event.RegisterMemberEvent(ModuleId.Login, gmodule.notifyId.Login.CREATEPLAYER,self, UILogin.CreateUserInternal)
end

def.override().OnDestroy = function (self)
    print("destroy petEdit------------------")
    Event.UnregisterEvent(ModuleId.Login, gmodule.notifyId.Login.CREATEPLAYER, UILogin.CreateUserInternal)
    
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

    local text = self.mUser.text
    if text ~= "" then
        self:CreateUserImpl(text);
        self.m_panel:GetComponent("UGUIFormExtend"):ProcedureImpl()
    end
end

def.method("userdata").OnClickLogout = function ( self, obj )
    -- body
    --print("-------------------back login: "..typeof(self.m_panel).."..................")
    local arg = AlphaWork.GameToLoginEventArgs.New(self.m_panel);--Myl:调用方式有疑问
    AlphaWork.GameEntry.Event:Fire(this, arg);
end

def.method("userdata").OnClickUserList = function ( self, obj )
    -- body
    --print("--------------clicked userlist---------")
    --self.m_panel:GetComponent("UGUIFormExtend"):ProcedureImpl()
end

def.method("string").CreateUserImpl = function ( self, name )
    local player = AlphaWork.UPlayer().New();
    player.user = name;
    player.gamesetting = AlphaWork.GameEntry.Config.GameSetting.UID;

    local players = {}
    AlphaWork.GameEntry.DataBase:FetchPlayersByName(name);

    --[[ if players == nil then
        print("-------------------------------------has no player-------")
    elseif players[0] == nil then
        print("-----------------------------player 0 is nil-------------")
    else
        print("--------------------------player "..players[0].."is valide------")
    end ]]
    
--[[     if players == nil or players[0] == nil then
        AlphaWork.GameEntry.DataBase.AddPlayer(player);
    else
        players[0].gamesetting = player.gamesetting;
    end 
    AlphaWork.GameEntry.Config.GameSetting.CurrentUser = name;     ]]
end

def.method("table").CreateUserInternal = function ( self, params )
    -- body
    print("........createuser....."..#params)
    local players = params;
    if players == nil or players[0] == nil then
        AlphaWork.GameEntry.DataBase.AddPlayer(params[0]);
    else
        players[0].gamesetting = params[0].gamesetting;
    end 
    AlphaWork.GameEntry.Config.GameSetting.CurrentUser = name;    
end

function UILogin.RegistObj( obj )
    -- body       
    m_Instance:CreateUGUIPanel(GameUtil.GetResPath(100001), 1,{},obj);
    
end

UILogin.Commit()
return UILogin
