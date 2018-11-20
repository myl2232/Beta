local Lplus = require "Lplus"
local TBPanelLoader = require "GUI.TBPanelLoader"
local TBGUIMan = Lplus.ForwardDeclare("TBGUIMan")
local GameUtil = require "Main.GameUtil"


--[[
	GUI 顶级面板。有全局名称；有 z 序；可 show, hide
	继承自 TBPanelLoader，由 TBPanelLoader 完成资源加载
]]
local TBPanelBase = Lplus.Extend(TBPanelLoader, "TBPanelBase")
local def = TBPanelBase.define

def.field("string").m_panelName = ""
def.field("number").m_depthLayer = 1
def.field("number").m_level =0
def.field("boolean").m_showBlur = true
def.field("boolean").m_showMask = false
def.field("boolean").m_maskRaycast = true
def.field("userdata").m_parantObj = nil
def.field("boolean").m_notProcessedPermanentUI = false;
--是否播放面板动画
def.field("boolean").m_IsPlayPanelAni = true
--补充逻辑缓存结果操作
def.field("dynamic").m_catcheShow =nil
--调用CreateUI传入的参数
def.field("dynamic").m_para = nil
--额外参数(主要用于界面跳转)
def.field("dynamic").m_extrapara = nil
--用来确保doCreate方法在DestroyPanel之前只会被调用一次
--解决创建ui调用CreateUGUIPanel前有内容需要预加载的情况
--flyfish17/07/12
def.field("boolean").m_doCreateOnce =false
local depthLayers = { { depth = -2000, lastPanel = nil }, { depth = 0, lastPanel = nil }, { depth = 10000, lastPanel = nil} ,{ depth = 30000, lastPanel = nil} , { depth = 60000, lastPanel = nil }  , { depth = 90000, lastPanel = nil } , { depth = 120000, lastPanel = nil } , { depth = 150000, lastPanel = nil }}

def.constructor().ctor = function (self)
	--self:SetGroupRoot(self)
end

def.override().OnCreate = function (self)

end

def.override().OnDestroy = function (self)

end

def.override("boolean").OnShow = function( self, s )
end

def.virtual().AfterCreate = function (self)
end

def.override().AfterDestroy = function (self)
end

def.virtual("boolean", "boolean").OnGUIChange = function (self, rolechange, userchange)
end
def.virtual().OnGUIDisConnect = function(self)
end

def.virtual("number","number").OnWorldReady = function(self,old_sceneid,cur_sceneid)
end

def.override("=>","boolean").IsShow = function (self)
	if not self.m_panel or self.m_panel == nil then return false end
	local isActive = self.m_panel.activeSelf
	return isActive
end

local function getPanelNameFromResName (resName)
	local i, j, cap = resName:lower():find("/([%w_]+)%.prefab%.u3dext$")
	if cap then
		return cap
	end
	local i, j, cap = resName:lower():find("([^/]*)$")
	return cap or "noname"
end

local function findControl(name,obj)
	local t = GameUtil.FindChild(obj.gameObject, name)
	if not t then
		print("can not find control with name:"..name)
	end
	return t
end

def.method("string","userdata","=>","userdata").FindControl = function(self,name,parent)
	if parent == nil then
		return findControl(name,self.m_panel)
	else
		return findControl(name,parent)
	end
end

def.method("string","=>","userdata").FindChild = function(self,name)
	return self:FindControl(name,nil)
end

def.method("userdata","=>","string")._GetObjNamePath = function(self,obj)
	if not obj or obj == nil then return "" end
	local name_path = obj.name
	local p = obj.parent
	while p ~= nil do
		if p.name == "UI Root(2D)" then
			break
		end
		name_path = p.name .. "/" .. name_path
		p = p.parent
	end
	if not self.m_panel or self.m_panel == nil then return "" end
	local panel_name = self.m_panel.name
	if name_path:sub(1,1+panel_name:len()) == panel_name.."/" then
		name_path = name_path:sub(2+panel_name:len())
	end
	return name_path
end

def.method("string","=>","string").GetObjNamePath = function(self,name)
	return self:_GetObjNamePath(self:FindChild(name))
end

def.method("number").SetLayer = function(self,l)
	if self.m_panel and self.m_panel ~= nil then
		self.m_panel:SetLayer(l)
	end
end

def.method("=>","number").GetLayer = function(self)
	if self.m_panel and self.m_panel ~= nil then
		return self.m_panel.layer
	end
	return ClientDef_Layer.Invisible
end

def.method("string", "=>", "string").RegisterPanel = function (self, resName)
	if resName == nil then
		return ""
	end
	local panelName = getPanelNameFromResName(resName)
	self.m_panelName = panelName
	TBGUIMan.Instance():RegisterPanel(self, panelName)

	return panelName
end

def.method("string", "=>", "string").RegisterUGUIPanel = function (self, resName)
	local panelName = getPanelNameFromResName(resName)
	self.m_panelName = panelName
	local TBGUIMan = require "GUI.TBGUIMan"
	TBGUIMan.Instance():RegisterUGUIPanel(self, panelName)

	return panelName
end
--用于提供默认创建方法,接受参数
def.method("dynamic").CreateUI = function(self,para,extrapara)
	self.m_para = para
	if para and para[3] then
       self.m_extrapara = para[3]
	end

	
	if self.m_doCreateOnce then
		if self.m_panel ~= nil then
			-- TODO
			-- 临时方案: 统一用CreatePanel消息机制打开隐藏界面
			--TBGUIMan.Instance():ShowAllUI(false);
			self:Show(true);
		end
	else
		self.m_doCreateOnce = true
		self:DoCreate()
	end
end

def.virtual().DoCreate = function(self)

end
def.method("userdata").SetParentObj = function (self,parentObj)
	self.m_parantObj = parentObj
end
def.method("table", "number", "table").CreateUGUIPanel = function(self, resInfo, level, relatedRes)
	self.m_level = level
	self.loadResTable = {};
	self:CreatePanelInternal(resInfo,relatedRes)
end

def.method("table","table").CreatePanelInternal = function(self, resInfo, relatedRes)
	if self:IsResourceReady() then
		return
	end
	if self.m_disappearing then	--如果是正处于消失中的panel，直接销毁，重新加载
		--self:UnloadPanel()
	end
	self.m_catcheShow = nil
	--注册顶级 Panel
	local respath = resInfo["path"] or ""

	local panelName = self:RegisterUGUIPanel(respath)
	
	TBGUIMan.Instance():ShowWaiting(true)
	local TBGUIMan = require("GUI.TBGUIMan")
	TBGUIMan.Instance():AddUI(self, self.m_level)
	--实际资源加载由GF完成
	--[[ self:LoadPanel(resInfo, panelName, self.m_parantObj, relatedRes, function (bFinished, bSucceeded)
		if not bFinished or not bSucceeded then
			return
		end
		TBGUIMan.Instance():ShowWaiting(false)
		self:OnLoadPanel()
	end) ]]
end

def.field("number").m_tweenTimer = 0
def.field("number").m_tweenDuration = 0.3
def.method("boolean").PlayPanelTween = function ( self ,isOpen)
	--[[
	local UIPlayTween = self.m_panel:GetComponent(typeof(DG.Tweening.DOTweenAnimation))
	if UIPlayTween then
		UIPlayTween:DORestart()
		if not isOpen then self.m_disappearing = true end

		
		GameUtil.RemoveGlobalTimer(self.m_tweenTimer)
		self.m_tweenTimer = GameUtil.AddGlobalTimer(m_tweenDuration, 1, function ()
			if self:IsPanelNil() then return end
			self:onPlayTweenFinish(self.m_panel.name)
		end)
	end
	]]

	if self.m_panel then
		local panelTween = self.m_panel:GetComponent("PanelDOTweenAnimation")
		if panelTween then
			if isOpen then
				panelTween:PlayTween();
			else
				panelTween:PlayReverseTween();
				GameUtil.RemoveTimer(self.m_tweenTimer)
				self.m_tweenTimer = GameUtil.AddGlobalTimer(self.m_tweenDuration, 1, function ()
					if self:IsPanelNil() then return end
					self:onPlayTweenFinish(self.m_panel.name)
				end)
			end
		end
	end
end

def.method().OnLoadPanel = function (self)
	local panelObj = self.m_panel
	--local TBGUIMan = require("GUI.TBGUIMan")
	--TBGUIMan.Instance():AddUI(self, self.m_level)
	--默认挂在Panel上面的UIPlayTween就是关闭界面的PlayTween
	if(self.m_IsPlayPanelAni) then
		self:PlayPanelTween(true)
	end	
	-- show 之前的处理
	self:OnCreateInternal()
	-- show
	if not self.m_panel or self.m_panel == nil then
		return
	end
	print("TBPanelBase :OnLoadPanel ".. self.m_panelName)
	if self.m_catcheShow ~= nil then
		local catcheShow = self.m_catcheShow
		self.m_catcheShow = nil
		self.m_panel:SetActive(catcheShow)
		--print("********************* self.m_catcheShow",catcheShow,self.m_panel.name)
		self:OnShowInternal(catcheShow)
	else
		self:OnShowInternal(panelObj.activeSelf)
	end

	if self.m_notProcessedPermanentUI == false and self.m_level ~= UILevel.L1Popup then
		if self.m_para and self.m_para[1] ~= nil then
			Event.DispatchEvent(ModuleId.Common, gmodule.notifyId.Common.SHOW_PANEL, {self.m_para[1]});
		end
	end

	-- after create
	self:AfterCreate()
	local GameUtil = require("Main.GameUtil")
	GameUtil.AddGlobalTimer(0, 1, function()
		if(self.m_showMask) then
			TBGUIMan.CreateMask(self.m_panel)
		end
        Event.DispatchEvent(ModuleId.Common,gmodule.notifyId.Common.CREATE_PANEL_FINISH,self)
	end)
end

def.override().OnChangePanelObject = function (self)
	--self.m_groupObj = self.m_panel
end

def.method("table").ShowPanel = function (self,funcType)
	TBGUIMan.OpenUIWithReturn(funcType,nil)
end

def.method().BackToLastPanel = function(self)
	if self.m_RelateGameobjectMap then
		for k,v in pairs(self.m_RelateGameobjectMap) do 
			GameObject.Destroy(v);
		end
	end
	
	self:DestroyPanel();
end
--有时关闭界面需要立即关闭 不播放关闭动画 
def.method("boolean").EnablePanelAnimation = function(self,isenabled)
	self.m_IsPlayPanelAni = isenabled
end

def.method().DestroyPanel = function(self)
	self.m_para = nil
	self.m_extrapara = nil
	self.m_catcheShow = nil
	self.m_doCreateOnce = false
	self.m_parantObj = nil
	--关闭子panel
	if self.m_subPanels then
		while #self.m_subPanels > 0 do
			local subPanel = table.remove(self.m_subPanels,#self.m_subPanels)
			--if subPanel.m_panel then
				subPanel:DestroyPanel()
			--end
		end
	end

	--TBGUIMan.Instance():RemoveUI(self, self.m_level)
	--TBGUIMan.Instance():RemoveUI(self, self.m_level)

	--TODO ugui 关闭效果
	local firstDestroy = self.m_createRequested
	self.m_createRequested = false
	if not self.m_panel or self.m_panel == nil then return end

	--默认挂在Panel上面的UIPlayTween就是关闭界面的PlayTween

	local UIPlayTween = self.m_panel:GetComponent("PanelDOTweenAnimation")
	if UIPlayTween and self.m_IsPlayPanelAni then
		self:PlayPanelTween(false)
	else
		self:DestroyPanelRaw()
	end
	--销毁界面时 保证本界面OnShowfalse 在前一个界面OnShowtrue 前调用 add by howie
	TBGUIMan.Instance():RemoveUI(self, self.m_level)
	self.m_IsPlayPanelAni = true
end

def.method().DestroyPanelRaw = function (self)
	if self:IsShow() then
		self:OnShowInternal(false)
	end
	self:OnDestroyInternal()
	Event.DispatchEvent(ModuleId.Common,gmodule.notifyId.Common.DESTROY_PANEL_FINISH,self)
	
	self:UnloadPanel()

	self:AfterDestroyInternal()
	Event.DispatchEvent(ModuleId.Common,gmodule.notifyId.Common.AFTER_DESTROY_PANEL,self)
end

def.method("boolean").Show = function(self,s)

	if self.m_panel and self.m_panel ~= nil then
		self.m_catcheShow=nil
		--控制子panel
		if self.m_subPanels then
			for i,v in ipairs(self.m_subPanels) do
				v:Show(s)
			end
		end
	
		self.m_panel:SetActive(s)
		self:OnShowInternal(s)
	else
		self.m_catcheShow =s
	end
	
	if s then
		if(self.m_IsPlayPanelAni) then
			self:PlayPanelTween(true)
		end	
		if self.m_notProcessedPermanentUI == false and self.m_level ~= UILevel.L1Popup then
			if self.m_para and self.m_para[1] ~= nil then
				Event.DispatchEvent(ModuleId.Common, gmodule.notifyId.Common.SHOW_PANEL, {self.m_para[1]});
			end
		end
	end
end

def.field("table").loadResTable = nil;
def.method("number", "function").AsyncLoadRes = function(self, resInfo, callback)
	GameUtil.AsyncLoadRes(GameUtil.GetResPath(resInfo), function(obj)
		if self:IsPanelNil() then
            return
		end
		
		if(obj == nil) then
			print("加载出錯！")
			return
		end
		
		table.insert(self.loadResTable,obj);
		callback(obj);
	end)            
end

def.method()._BringTopReal = function(self)
	if not self.m_panel or self.m_panel == nil then return end

	local depthLayer = depthLayers[self.m_depthLayer]
	if depthLayer.lastPanel ~= self.m_panel then
		depthLayer.depth = self.m_panel:BringUIPanelTopDepth(depthLayer.depth)
		depthLayer.lastPanel = self.m_panel
	end
end

def.virtual("boolean","=>","boolean")._canBringTop = function(self, pressState)
	return pressState
end

def.virtual("string").onPlayTweenFinish = function(self,id)
	if self.m_panel then
		--if id == self.m_panel.name and self.m_disappearing then
		if id == self.m_panel.name then
			self:DestroyPanelRaw()
		end
	end
end

def.virtual("=>", "boolean").IsDebugUI = function (self)
	return false
end
--判断panel是否是nil
def.method("=>","boolean").IsPanelNil = function ( self )
	return GameUtil.IsNil(self.m_panel);
end

def.method().SetOutTouchDisappear = function(self)
	local TBGUIMan = require "GUI.TBGUIMan"
	TBGUIMan.Instance():SetOutTouchDisappear(self)
end

def.virtual("string","string").OnOutTouchDisappear = function  (self,touchObjName,panelName)

end

--一级界面设置模糊蒙版
def.method("boolean").ShowBlur = function  (self,showBlur)
	self.m_showBlur = showBlur
end
--二级界面设置底板
def.method("boolean").ShowMask = function  (self,showMask)
	self.m_showMask = showMask
end

def.method("boolean").SetMaskRaycast = function ( self,enable )
	self.m_maskRaycast = enable
end
def.method("boolean").ShowUILight = function (self,show)
	TBGUIMan.Instance():ShowLight(show)
end
--[[
	统一管理子panel的生命周期，如果注册为子panel后，主panel被销毁时，其也跟着被销毁
	用来处理标签页
]]
def.field("table").m_subPanels = nil
def.method(TBPanelBase).RegisterSuPanel = function  (self,panel)
	self.m_subPanels = self.m_subPanels or {}
	panel.m_depthLayer = self.m_depthLayer
	table.insert(self.m_subPanels,panel)
end

TBPanelBase.Commit()
return TBPanelBase