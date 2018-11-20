local Lplus = require("Lplus");
local TBPanelBase = require("GUI.TBPanelBase");
local GameUtil = require("Main.GameUtil");

local UIMainMenu = Lplus.Extend(TBPanelBase, "UIMainMenu");
local def = UIMainMenu.define;
local m_Instance = nil;

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

UIMainMenu.Commit()
return UIMainMenu