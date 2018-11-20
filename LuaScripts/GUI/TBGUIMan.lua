--[[
	结构
	TBGUIMan: 管理层
		├ PanelBase <- PanelLoader
		└ GroupBase
]]--
local Lplus = require "Lplus"
local TBPanelBase = require "GUI.TBPanelBase"
local ConfigMgr = require("Main.ConfigMgr")
local GameUtil = require("Main.GameUtil")
local TBGUIMan = Lplus.Class("TBGUIMan")
local def = TBGUIMan.define

def.field("table").m_uguiPanelMap = function () return {} end	-- name => panel
def.field("table").m_uiLevelMap = nil
def.field("userdata").m_uiLight = nil
def.field("boolean").m_showLight =false
def.field("userdata").mWaitingPanel = nil
def.field("table").m_touchRemove = nil
local man = nil
def.static("=>", TBGUIMan).new = function ()
	man = TBGUIMan()
	man.m_uiLevelMap ={}
	return man
end

def.static("=>", TBGUIMan).Instance = function ()
	if man == nil then
		man = TBGUIMan ()
		man.m_uiLevelMap ={}
		man.m_touchRemove = {}
	end
	return man
end

--通用的打开界面接口，后面可以加入未开启提示相关内容
def.static("table","dynamic","=>","boolean").OpenUIWithReturn = function(funcType, para)
    local open,ret = CheckFuncOpen(funcType,true)
    --此处应该加入检测开启代码，未开启返回false此时原界面可能不需要关闭
	if open then
		TBGUIMan.OnCreateUI(funcType)
	end
    return open
end
--调用打开界面逻辑，并返回是否打开成功
_G.OpenUIWithReturn = function(funcType,para)
	return TBGUIMan.OpenUIWithReturn(funcType,para)
end
--用来检测界面是否开启
def.static("table","boolean","=>","boolean").CheckFuncOpen = function(funcType,isTips)
	local open = true
	return open
end
--检测界面是否开启   isTips 未开启的话，是否弹通用提示，可选参数
_G.CheckFuncOpen = function(funcType,isTips)
	if isTips == nil then
		isTips = false
	end

	return TBGUIMan.CheckFuncOpen(funcType,isTips)
end


def.static("userdata","string").SetUIUserData = function(go,data)
	local userdatacomponent = go:GetComponent(typeof(UIUserData))
	if(userdatacomponent == nil) then
		userdatacomponent = go:AddComponent(typeof(UIUserData))
	end
	userdatacomponent.userdata = data
end
def.static("userdata","=>","dynamic").GetUIUserData = function(go,data)
	local userdatacomponent = go:GetComponent(typeof(UIUserData))
	if(userdatacomponent ~= nil) then
		return  userdatacomponent.userdata
	end
	return nil
end
_G.SetUIUserData = function(go,data)
	TBGUIMan.SetUIUserData(go,data)
end
_G.GetUIUserData = function(go)
   return TBGUIMan.GetUIUserData(go)
end

--调用统一创建panel逻辑,参数列表第一个是类型标志，后面是模块使用参数
def.static("table").OnCreateUI = function(p1)
	local GUIHandlers = require"GUI.GUIHandlers"
	if p1[1] then
		local handler = GUIHandlers[p1[1]]
		if(handler) then
			handler:CreateUI(p1)
		else
			warn("没有找到 UIHandler :"..p1[1] )
		end
	else
		warn("创建UI界面参数错误")
	end
end

--调用统一销毁panel逻辑
def.static("table").OnDestroyUI =function(p1)
	local GUIHandlers =require"GUI.GUIHandlers"
	if p1[1] then
		local handler = GUIHandlers[p1[1]]
		if(handler) then
			handler:DestroyPanel()
		--else
			--warn("没有找到 UIHandler :"..p1[1] )
		end
	else
		warn("销毁UI界面参数错误")
	end
end

def.static("table").CloseUI = function(p1)
	local GUIHandlers =require"GUI.GUIHandlers"
	if p1[1] then
		local handler = GUIHandlers[p1[1]]
		if(handler) then
			handler:DestroyPanelRaw()
		end
	else
		warn("销毁UI界面参数错误")
	end
end

def.static("userdata").CreateMask = function(panel)
	local maskgo = GameObject.New()
	maskgo.name = "mask"
	maskgo.transform:SetParent(panel.transform)
	maskgo.transform:SetAsFirstSibling()
	maskgo.transform.localScale = UnityEngine.Vector3(1,1,1)
	maskgo.layer = panel.layer
	local maskimg = maskgo:AddComponent(typeof(Image))
	local btn = maskgo:AddComponent(typeof(Button))
	local rect = maskimg.rectTransform
	rect.anchoredPosition3D = UnityEngine.Vector3(0,0,0)
	rect.anchorMin = UnityEngine.Vector2(0,0)
	rect.anchorMax = UnityEngine.Vector2(1,1)
	rect.offsetMin = UnityEngine.Vector2(0,0)
	rect.offsetMax = UnityEngine.Vector2(0,0)
	GameUtil.AsyncLoadSprite("common", PanelMaskImgName,function(Sprite)
		if(Sprite == nil) then
		   error(string.format("load sprite %s err!"),PanelMaskImgName)
		   return
		end
		maskimg.sprite = Sprite
	end) 
end

--暂时先这么处理 应该需要更全面逻辑
def.static("table").ToLogin = function(p1)
	if man~= nil then 
		for i =UILevel.NoManager ,UILevel.Max-1 do 
			man:DestroyUIAtLevel(i);
		end 
		-- man:RemoveLevelOneUI()
		
	end
	gmodule.moduleMgr:ResetAllModules();
	TBGUIMan.OpenUIWithReturn({EFuncType.Login},nil)
end

-- -1（不记录不管理） 0（只记录，不管理） 1（一级UI，两两互斥） 2（二级UI，会在一级UI变化时，全部销毁）
-- 3(流程界面，打开B界面成功会销毁A界面)
def.method(TBPanelBase, "number").AddUI = function(self, ui, level)
	if self.m_uiLevelMap[level] == nil then
		self.m_uiLevelMap[level] = {}
	end
	if level == 1 then
		local prevUI = self.m_uiLevelMap[1][#(self.m_uiLevelMap[1])]
		if prevUI ~= nil then
			prevUI:Show(false)
		end
		if self.m_uiLevelMap[2] ~= nil then
			local level2 =self.m_uiLevelMap[2]
			for i =#level2,1,-1 do
				level2[i]:DestroyPanel()
			end
			self.m_uiLevelMap[2] = {}
		end
	end
	if level == 3 then
		if self.m_uiLevelMap[1] then 
			local ui =nil
			local count =#self.m_uiLevelMap[1];
			for i=count,1,-1 do 
				
				ui = self.m_uiLevelMap[1][i]
				
				if ui~=nil then
					print("panelName"..ui.m_panelName);
					ui:DestroyPanel()
				end 
			end 
			self.m_uiLevelMap[1]=nil
		end 

		local prevUI = self.m_uiLevelMap[3][#(self.m_uiLevelMap[3])]
		if prevUI ~= nil then
			prevUI:DestroyPanel()
		end
	end
	table.insert(self.m_uiLevelMap[level], ui)
end

def.method(TBPanelBase, "number").RemoveUI = function(self, ui, level)
	if self.m_uiLevelMap[level] == nil then
		return
	end
	local curUI = nil
	local index = 0
	for i = #self.m_uiLevelMap[level], 1, -1 do
		if self.m_uiLevelMap[level][i] == ui then
			index = i
			curUI = ui
			break
		end
	end
	if curUI == nil then return end
	if level == 1 and index == #self.m_uiLevelMap[level] then

		for k,v in ipairs(self.m_uiLevelMap[level]) do 
			print(v.m_panelName);
		end


		local prevUI = self.m_uiLevelMap[1][index - 1]
		if prevUI ~= nil then
			prevUI:Show(true)
		end
		if self.m_uiLevelMap[2] ~= nil then
			for k, v in ipairs(self.m_uiLevelMap[2]) do
				v:DestroyPanel()
			end
			self.m_uiLevelMap[2] = {}
		end
	end
	table.remove(self.m_uiLevelMap[level], index)
end

--仅仅用于给活动UI关闭的时候提供接口清除所有1级UI
--其他人慎用
--慎用
--慎用
def.method().RemoveLevelOneUI = function(self)
	if self.m_uiLevelMap == nil or self.m_uiLevelMap[1] == nil or #self.m_uiLevelMap[1] <= 0 then return end
	local lsTable = self.m_uiLevelMap[1]
	self.m_uiLevelMap[1] = {}
	for k, v in ipairs(lsTable) do
		v:DestroyPanel()
	end
end
def.method("boolean").ShowAllUI = function(self, hide)
	for i = 0, 2 do
		local uis = self.m_uiLevelMap[i]
		if uis ~= nil then
			-- if i == 1 and #uis > 0 then
			-- 	uis[#uis]:Show(hide)
			-- else
				for k, v in ipairs(uis) do
					v:Show(hide)
				end
			-- end
		end
	end
end

def.method("number").DestroyUIAtLevel = function(self, level)
	if self.m_uiLevelMap[level] ~= nil then
		local curLevelUIList = self.m_uiLevelMap[level]
		local uiCount = #curLevelUIList
		for i = uiCount, 1, -1 do
			local ui = curLevelUIList[i]
			ui:DestroyPanel()
		end
	end
end

--返回到一级面板的某个被隐藏的界面
def.method("table").BackToPanel = function (self, panel)
	local uis = self.m_uiLevelMap[1]
	local findIndex = 0
	for i= 1,#uis do
		if uis[i] == panel then
			findIndex = i
			break
		end
	end 
	for i=#uis,findIndex+1,-1 do
		uis[i]:DestroyPanel()
	end
end
--获取最近一次打开的面板
def.method("=>","table").GetLastPanel = function (self)
	local uis = self.m_uiLevelMap[1]
	return uis[#uis]
end

--设置点击其他位置关闭
def.method("table").SetOutTouchDisappear = function(self,ui)
	table.insert(self.m_touchRemove,ui)
end
def.method("string").NotifyDisappear = function(self,panelname)
	for i=#self.m_touchRemove,1,-1 do
		if(self.m_touchRemove[i].m_panel.name ~= panelname) then
			self.m_touchRemove[i]:DestroyPanel()
		end	
	end
	self.m_touchRemove = {}
end
def.method().Init = function( self )
	--资源管理器初始化，为了加载图集先行--
	--ResourceManager.Initialize()
	self:PreloadAtlas()
	self:RegEvent()
	self:PreInitUI()
end
def.method().PreloadAtlas = function(self)
	-- GameUtil.RequestAtlas("temporary", function(atlas)
	-- end)
	-- GameUtil.RequestAtlas("tbskillatlas", function(atlas)
	-- end)
	-- GameUtil.RequestAtlas("common", function(atlas)
	-- end)
	-- GameUtil.AsyncLoadRes(GameUtil.GetResPath(100103), function(obj)
	-- end)
	-- GameUtil.AsyncLoadRes(GameUtil.GetResPath(100165), function(obj)
	-- end)
	-- GameUtil.AsyncLoadRes(GameUtil.GetResPath(100133), function(obj)
	-- end)
end

--注册响应事件
def.method().RegEvent = function()
	require("Main.module.Module")
--[[ 	Event.RegisterEvent(ModuleId.Common, gmodule.notifyId.Common.CREATE_PANEL, TBGUIMan.OnCreateUI);
	Event.RegisterEvent(ModuleId.Common, gmodule.notifyId.Common.DESTROY_PANEL, TBGUIMan.OnDestroyUI);
	Event.RegisterEvent(ModuleId.Common, gmodule.notifyId.Common.TO_LOGIN, TBGUIMan.ToLogin);
 ]]end

def.method().Release = function( self )
	self:_LeaveGameStage(true)
	--[[ Event.UnregisterEvent(ModuleId.Common, gmodule.notifyId.Common.CREATE_PANEL, TBGUIMan.OnCreateUI);
	Event.UnregisterEvent(ModuleId.Common, gmodule.notifyId.Common.DESTROY_PANEL, TBGUIMan.OnDestroyUI);
	Event.UnregisterEvent(ModuleId.Common, gmodule.notifyId.Common.TO_LOGIN, TBGUIMan.ToLogin);
 ]]end
--初始化UI 相关
def.method().PreInitUI = function()
--[[ 	require "Game.Common.PlotMgr".Instance():Init()
	require "Game.Trigger.EventTriggerMgr".Instance():Init()
	require "Game.Common.GuideMgr_new".Instance():Init() ]]
end
def.method(TBPanelBase, "string").RegisterUGUIPanel = function (self, panel, panelName)
	self.m_uguiPanelMap[panelName] = panel
end

def.method("string", "=>", TBPanelBase).FindPanelByName = function (self, panelName)
	return self.m_uguiPanelMap[panelName]
end

def.method("=>", "varlist").EachPanel = function (self)
	return pairs(self.m_uguiPanelMap)		--TODO 改为 m_uguiPanelMap
end

------------------------------
-- TBGame调用部分 --
------------------------------

def.method().EnterLoginStage = function(self)
	--self:InitWaiting()
	local event = EFuncType.Login
	TBGUIMan.OpenUIWithReturn({event},nil)
end

def.method().LeaveLoginStage = function(self)
	
end

def.method().EnterGameStage = function(self)
end
def.method("boolean").LeaveGameStage = function(self, changerole)
	if changerole then
		for k,v in pairs(self.m_uguiPanelMap) do
			local panel = v
			panel:OnGUIChange(changerole, false)
			if panel:IsDebugUI() then
			else
				panel:DestroyPanel()
			end
		end
	end
end

def.method("boolean").ShowLight = function (self,show)
	self.m_showLight = show
	print(  show )
	if self.m_uiLight~=nil then 
		
		self.m_uiLight:SetActive(show)
	end 
end

def.method().InitWaiting = function (self)
	if self.mWaitingPanel==nil then 
		GameUtil.AsyncLoadRes(GameUtil.GetResPath(100132), function(obj)
			if(obj == nil) then
				return
			end
			self.mWaitingPanel = UnityEngine.Object.Instantiate(obj)
			GameUtil.SetParent(self.mWaitingPanel,GUIRoot.GetUGUIRootObj(12))
			self:ShowWaiting(false)
		end)
	end 
end

def.method("boolean").ShowWaiting = function (self, show)
	if self.mWaitingPanel then
		self.mWaitingPanel:SetActive(show);
	end
end


def.method().ApplicationQuit = function(self)
end


TBGUIMan.Commit()
TBGUIMan.Instance()

return TBGUIMan
