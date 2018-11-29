local Lplus = require("Lplus");
local TBPanelBase = require("GUI.TBPanelBase");
local GameUtil = require("Main.GameUtil");

local UIInGame = Lplus.Extend(TBPanelBase, "UIInGame");
local def = UIInGame.define;
local m_Instance = nil;

-- 单例对象
def.static("=>", UIInGame).Instance = function (self)
	if(m_Instance == nil) then
		m_Instance = UIInGame();
	end
	return m_Instance;
end

-- 预创建UI回调
def.override().DoCreate = function (self)
	--self:CreateUGUIPanel(GameUtil.GetResPath(100002), 3,{});
end

function UIInGame.RegistObj( self, obj )
    -- body
	m_Panel = obj;
	print("------------------Regist Panel----------------"..m_panel.name..".................")
	m_Instance:CreateUGUIPanel(GameUtil.GetResPath(100002), 1,{});

end

UIInGame.Commit()
return UIInGame