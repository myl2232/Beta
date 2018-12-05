local Lplus = require("Lplus");
local TBPanelBase = require("GUI.TBPanelBase");
local GameUtil = require("Main.GameUtil");

local UIInGame = Lplus.Extend(TBPanelBase, "UIInGame");
local def = UIInGame.define;
local m_Instance = nil;

--类成员
def.field("userdata").mAttack1 = nil;
def.field("userdata").mAttack2 = nil;
def.field("userdata").mAddItem = nil;
def.field("userdata").mAI = nil;
def.field("userdata").mExit = nil;
def.field("userdata").mReplace = nil;
def.field("userdata").mTrackPad = nil;

-- 单例对象
def.static("=>", UIInGame).Instance = function (self)
	if(m_Instance == nil) then
		m_Instance = UIInGame();
		print("=====================intstance UIInGame ")
	end
	return m_Instance;
end

def.override().OnCreate = function (self)
    self.mAttack1 = self:FindChild("Attack1")
    self.mAttack2 = self:FindChild("Attack2")
    self.mAddItem = self:FindChild("AddBtn")
	self.mAI = self:FindChild("AIBtn")
	self.mExit = self:FindChild("btnExit")
	self.mReplace = self:FindChild("ReplaceBtn")
	self.mTrackPad = self:FindChild("TrackPad")
end

-- 预创建UI回调
def.override().DoCreate = function (self)
	--self:CreateUGUIPanel(GameUtil.GetResPath(100002), 3,{});
end

-- 单击事件回调
def.method("userdata").onClickObj = function(self, obj)
    if(obj.name == self.mAttack1.name) then
        self:OnAttack1(obj);
    elseif(obj.name == self.mAttack2.name) then    
        self:OnAttack2(obj);
	elseif(obj.name == self.mAI.name) then    
		self:OnKick1(obj);
	elseif(obj.name == self.mReplace.name) then    
		self:OnKick2(obj);
	elseif(obj.name == self.mAddItem.name) then    
		self:OnAdd(obj);
	elseif(obj.name == self.mExit.name) then    
        self:OnBackLogin(obj);
    end
end

def.method("userdata").OnAttack1 = function ( self, obj )
	AlphaWork.GameEntry.Event:Fire(self.m_panel, AlphaWork.UIAttack1EventArgs.New())
end

def.method("userdata").OnAttack2 = function ( self, obj )
	AlphaWork.GameEntry.Event:Fire(self.m_panel, AlphaWork.UIAttack2EventArgs.New())
end

def.method("userdata").OnKick1 = function ( self, obj )
	AlphaWork.GameEntry.Event:Fire(self.m_panel, AlphaWork.UIAlphaEventArgs.New())
end

def.method("userdata").OnKick2 = function ( self, obj )
	AlphaWork.GameEntry.Event:Fire(self.m_panel, AlphaWork.UIBetaEventArgs.New())
end

def.method("userdata").OnAdd = function ( self, obj )
	AlphaWork.GameEntry.Event:Fire(self.m_panel, AlphaWork.UIThetaEventArgs.New())
end

def.method("userdata").OnBackLogin = function ( self, obj )
	--print("-------------------back login: "..typeof(self.m_panel).."..................")
	local arg = AlphaWork.GameToLoginEventArgs.New(self.m_panel);
	AlphaWork.GameEntry.Event:Fire(self.m_panel, arg)	
end

function UIInGame.RegistObj( obj )
    -- body	
	print("------------------Regist Panel----------------"..obj.name..".................")
	m_Instance:CreateUGUIPanel(GameUtil.GetResPath(100002), 1,{},obj);

end

UIInGame.Commit()
return UIInGame