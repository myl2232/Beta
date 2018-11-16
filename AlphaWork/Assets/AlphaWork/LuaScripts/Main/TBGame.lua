local Lplus = require "Lplus"
local TBGUIMan = require "GUI.TBGUIMan"
local TBGame = Lplus.Class("TBGame")
local def = TBGame.define

local instance = nil

def.static("=>", TBGame).Instance = function ()
    if instance==nil then 
        instance = TBGame()
    end
	return instance
end
def.field(TBGUIMan).m_guiMan = nil
def.method().Start = function (self)
    --初始化服务器对接逻辑模块  就是消息处理--
    --TBNet.Instance():Start()
    print("************************** TBGame  Start ")
    require("Main.Module.Module")
    require"GUI.GUIHandlers"
    --UpdateBeat:Add(self.Update, self) --消除处理的Update监听--
    gmodule.gameStart()
    if not self.m_guiMan then
		self.m_guiMan = TBGUIMan.Instance()
		self.m_guiMan:Init()
	end
    self:EnterStartStage()    
end 
--进入启动预加载流程
def.method().EnterStartStage = function(self)
	require "Game.Start.StartLoading".Start();
end
--离开启动预加载流程
def.method().LeaveStartStage = function(self)
    self:EnterLoginStage();
end
--进入启动预加载流程
def.method().EnterLoginStage = function(self)
	self.m_guiMan:EnterLoginStage()
end
--离开启动预加载流程
def.method().LeaveLoginStage = function(self)
    self.m_guiMan:LeaveLoginStage()
end
--进入大厅模块
def.method().EnterHallStage = function(self)
	self.m_guiMan:EnterGameStage()
end

def.method().LeaveHallStage = function(self)
    self.m_guiMan:LeaveGameStage(changerole)
end
--进入游戏对战
def.method().EnterBattleStage = function(self)
	self.m_guiMan:EnterGameStage()
end

def.method("boolean").LeaveBattleStage = function(self, changerole)
    self.m_guiMan:LeaveGameStage(changerole)
end

def.method().ApplicationQuit = function (self)
    print("************************** TBGame  ApplicationQuit ")
    self.m_guiMan:ApplicationQuit()
    SaveToFile()
    if fileLog then 
        CloseFileLog()
    end 
end 

def.method().Update = function(self)
    TBNet.Instance():OnUpdate();
end

TBGame.Commit()
TBGame.Instance()
return TBGame