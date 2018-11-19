local Lplus = require("Lplus");
local TBPanelBase = require("GUI.TBPanelBase");
local GameUtil = require("Main.GameUtil");

local UILogin = Lplus.Extend(TBPanelBase, "UILogin");
local def = UILogin.define;
local m_Instance = nil;

-- 单例对象
def.static("=>", UILogin).Instance = function (self)
	if(m_Instance == nil) then
		m_Instance = UILogin();
	end
	return m_Instance;
end

-- 预创建UI回调
def.override().DoCreate = function (self)
	self:CreateUGUIPanel(GameUtil.GetResPath(100001), 3,{});
end

