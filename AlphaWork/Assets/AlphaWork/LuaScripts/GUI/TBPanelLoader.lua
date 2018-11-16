local Lplus = require "Lplus"
local GameUtil = require("Main.GameUtil")

--[[
	GUI 面板。有加载资源的功能，但不负责相关的 OnGroupXXX 事件
	另负责 msgHandler 相关 UI 消息处理
]]
local TBPanelLoader = Lplus.Class( "Game.GUI.TBPanelLoader")
do
	local def = TBPanelLoader.define

	--已加载当前面板--
	def.field("userdata").m_panel = nil
	--当前是否应该被创建出。CreatePanel 后变为 true，DestroyPanel 后变为 false
	def.field("boolean").m_createRequested = false
	def.field("boolean").m_isLoading = false
	def.field("boolean").m_disappearing = false
	def.field("boolean").m_HideOnDestroy = false
	def.field("boolean").m_SyncLoad = false
	def.field("boolean").m_DelayCreate = false
	def.field("userdata").m_panelHide = nil
	def.field("table").m_msgTable = nil
	def.field("userdata").m_uguiMsgHandler = nil
	def.field("number").mWaitLoadCount = 0
	def.field("table").m_RelateGameobjectMap = nil;
	def.field("table").m_RelateGameobjects = nil;
	local s_handlers = {}

	--资源是否已加载
	--可以用这个来判断是否该执行核心逻辑之类的
	def.method("=>", "boolean").IsResourceReady = function (self)
		return self.m_createRequested and self.m_panel ~= nil and not self.m_disappearing
	end
	--[[
		param parentObj: 新面板以此对象为父，非 nil 表示是子面板；nil 表示使用默认
		param onCreateFinish:
			function onCreateFinish (bFinished, bSucceeded)
				bFinished 为 false 表示跳过了加载 (同时 bSucceeded 无意义)
	]]
	def.method("table", "string", "userdata", "table","function").LoadPanel = function(self, resInfo, panelName, parentObj, RelatedRes,onCreateFinish)
		
		self.m_createRequested = true
		if self.m_isLoading then
			onCreateFinish(false, false)
			return
		end  --资源加载中，不用重复创建
		self.m_isLoading = true
		self.mWaitLoadCount = #RelatedRes

		local function onResourceLoaded(obj, bFromPanelHide)
			if not bFromPanelHide and self.m_HideOnDestroy then
				self.m_panelHide = self.m_panel
			end
			self:LoadPanelFromGameObjectInternal(obj, panelName, parentObj, true, bFromPanelHide, function(bFinished, bSucceeded)
				onCreateFinish(bFinished, bSucceeded)
			end)
		end

		if self.m_panelHide then
			self.m_isLoading = false
			self.m_panelHide:SetActive(true)
			onResourceLoaded(self.m_panelHide, true)
			return
		end
		
		GameUtil.AsyncLoadRes(resInfo, function(obj)
			if not self.m_createRequested then
				self.m_isLoading = false
				onCreateFinish(false, false)
				return
			end

			if not obj then
				self.m_isLoading = false
				onCreateFinish(true, false)
				return
			end
			
			if self.mWaitLoadCount <= 0 then
				onResourceLoaded(obj, false)
			else
				self:CreateRelatedGameObject(RelatedRes,onResourceLoaded,obj)
			end
		end)
	end

	def.method("table","function","userdata").CreateRelatedGameObject = function(self, RelatedRes, resourceLoad,objview)
		self.m_RelateGameobjectMap = {}
		self.m_RelateGameobjects = {}
		for i = 1, #RelatedRes do
			local id = i;
			GameUtil.AsyncLoadRes(GameUtil.GetResPath(RelatedRes[i]), function(obj,id)
				if(obj == nil) then
					self.m_isLoading = false
					onCreateFinish(true, false)
					return
				end

				local go =GameUtil.Instantiate(obj)
				self.m_RelateGameobjectMap[RelatedRes[i]] = go;
				table.insert(self.m_RelateGameobjects,go);
		
				self.mWaitLoadCount = self.mWaitLoadCount - 1;
				
				if self.mWaitLoadCount <= 0 then
					resourceLoad(objview, false)
				end
			end);
		end
	end
	
	--[[
		param onCreateFinish:
			function onCreateFinish (bFinished, bSucceeded)
				bFinished 为 false 表示跳过了加载 (同时 bSucceeded 无意义)
	]]
	def.method("userdata", "string", "userdata", "function").LoadPanelFromGameObject = function(self, go, panelName, parentObj, onCreateFinish)
		self:LoadPanelFromGameObjectInternal(go, panelName, parentObj, false, false, onCreateFinish)
	end

	--[[
		param cb:
			function cb (bFinished, bSucceeded)
				bFinished 为 false 表示跳过了加载
	]]
	def.method("userdata", "string", "userdata", "boolean", "boolean", "function").LoadPanelFromGameObjectInternal = function(self, go, panelName, parentObj, inner_create, no_instantiate, cb)
		local delay_create = self.m_DelayCreate and inner_create and not no_instantiate
		local panel
		--已经实例化过
		if no_instantiate then
			panel = go
			--未实例化
		else
			if delay_create then
				go:SetActive(false)	--否则 Instantiate 会立即触发音效
			end
			local rootObj = parentObj or self:GetUIRoot(go)
			--panel = Object.InstantiateWithParent(go,rootObj:GetComponent("Transform"),"GameObject")
			panel = GameUtil.InstantiateWithParent(go, rootObj)
			panel:SetActive(false)
			panel.transform.localRotation = go.transform.localRotation
			if delay_create then
				panel:SetActive(false)
			end
		end
		panel.name = panelName
		--检测合法性，是否已经要被删除
		local function CheckPanelValid(self, panel)
			if panel == nil then
				self.m_isLoading = false
				return false
			end
			if not self.m_createRequested then
				Object.Destroy(panel)
				self.m_isLoading = false
				return false
			end
			return true
		end

		local function afterDelay_1()
			local function afterDelay_2()
				if delay_create then
					if not CheckPanelValid(self, panel) then
						cb(false, false)
						return
					end
					panel:SetActive(true)
				end

				self.m_isLoading = false
				self:SetPanelObject(panel)	--self.m_panel = panel

				if not self.m_panel or panel == nil then
					cb(false, false)
					return
				end
				--self:OnBeforeAddMsgInternal()
				--最后附加 msgHandler
				panel:SetActive(true)
				self:TouchUGUIGameObject(panel)
				cb(true, true)
			end

			if delay_create then
				if not CheckPanelValid(self, panel) then
					cb(false, false)
					return
				end
			end

			if delay_create then
				GameUtil.AddGlobalTimer(0, 1, afterDelay_2)
			else
				afterDelay_2()
			end
		end

		if delay_create then
			GameUtil.AddGlobalTimer(0, 1, afterDelay_1)
		else
			afterDelay_1()
		end
	end

	local l_uguiRoot = {}
	def.method("userdata","=>", "userdata").GetUIRoot = function (self,obj)
		if not l_uguiRoot[obj.layer] then
			l_uguiRoot[obj.layer] = GUIRoot.GetUGUIRootObj(obj.layer)
		end
		return l_uguiRoot[obj.layer]
	end
	--实例创建成功后调用
	def.method("userdata").SetPanelObject = function (self, panelObject)
		self.m_panel = panelObject
		--self:OnChangePanelObject()
	end

	def.method().UnloadPanel = function(self)
		if self.m_HideOnDestroy then
			self.m_panelHide = self.m_panel
			self.m_panelHide:SetActive(false)
			--UnityEventListener 本会随 Panel 销毁自动消失，此处用 SetActive(false) 代替 Destory，因此需手动删除 UnityEventListener
			-- self.m_panelHide:RemoveUnityEventListenersInChildren()
		else
			--清除 msgHandler
			if self.m_panel then
				if self.m_uguiMsgHandler and self.m_uguiMsgHandler ~= nil then
					self.m_uguiMsgHandler:UnTouch(self.m_panel)
				end
				self.m_uguiMsgHandler = nil
			end
			if self.m_panel and self.m_panel ~= nil then
				UnityEngine.Object.Destroy(self.m_panel)
			end

			if self.m_RelateGameobjectMap then
				self.m_RelateGameobjectMap = {}
			end
		end
		self:SetPanelObject(nil)	--self.m_panel = nil
		self.m_disappearing = false
	end

	def.method("boolean").SetHideOnDestroy = function (self, bHideOnDestroy)
		self.m_HideOnDestroy = bHideOnDestroy
		if not bHideOnDestroy and self.m_panelHide then
			if self.m_panelHide ~= self.m_panel then
				Object.Destroy(self.m_panelHide)
			end
			self.m_panelHide = nil
		end
	end

	--通知更换新的panel
	def.virtual().OnChangePanelObject = function (self)
	end

	----------------------------------------
	-- msgHandler 相关
	----------------------------------------

	def.virtual("userdata").TouchUGUIGameObject = function(self, panelObj)
		if panelObj == nil then return nil end

		local uguiMsgHandler = panelObj:GetComponent(typeof(UGUIMsgHandler))
		if uguiMsgHandler == nil then
			uguiMsgHandler = panelObj:AddComponent(typeof(UGUIMsgHandler))
		end
		self.m_uguiMsgHandler = uguiMsgHandler
		self.m_msgTable = {
			onClick 		= self:_onEvent("onClick"),--一个参数 name
			onClickObj 		= self:_onEvent("onClickObj"),
			onStrValueChanged  = self:_onEvent("onStrValueChanged"),--两个参数 name和字符串值
			onFloatValueChanged  = self:_onEvent("onFloatValueChanged"),--两个参数 name和数值
			onIntValueChanged  = self:_onEvent("onIntValueChanged"),--两个参数 name和数值
			onBoolValueChanged  = self:_onEvent("onBoolValueChanged"),--两个参数 name和boolean值
			onRectValueChanged  = self:_onEvent("onRectValueChanged"), --返回 name,x ,y 3个参数
			onEditEnd       = self:_onEvent("onEditEnd"),--两个参数 name和字符串值
			onScroll        = self:_onEvent("onScroll"),--一个参数 name
			onDrag          = self:_onEvent("onDrag"),--返回 name,x ,y,touch_x , touch_y 5个参数
			onBeginDrag     = self:_onEvent("onBeginDrag"),--5个参数 name,x ,y,touch_x , touch_y 5个参数
			onEndDrag       = self:_onEvent("onEndDrag"),--5个参数 name,x ,y,touch_x , touch_y 5个参数
			onEnter         = self:_onEvent("onEnter"),--一个参数 name
			onExit          = self:_onEvent("onExit"),--一个参数 name
			onDown          = self:_onEvent("onDown"),--一个参数 name
			onHold          = self:_onEvent("onHold"),--一个参数 name
			onUp            = self:_onEvent("onUp"),--一个参数 name
			onUpDetail		= self:_onEvent("onUpDetail"),--5个参数 name,x ,y,touch_x , touch_y 5个参数
			onDownDetail	= self:_onEvent("onDownDetail"),--5个参数 name,x ,y,touch_x , touch_y 5个参数
			onSelect        = self:_onEvent("onSelect"),--一个参数 name
			onMove          = self:_onEvent("onMove"),--一个参数 name
			onSubmit        = self:_onEvent("onSubmit"),--一个参数 name
			onCancel        = self:_onEvent("onCancel"),--一个参数 name
			onPlayTweenFinish = self:_onEvent("onPlayTweenFinish__"),--一个参数 name
			--onGuideClickObj	= self:GuideClickOpr(),--新手引导用点击事件接受
		}
		self.m_uguiMsgHandler:SetMsgTable(self.m_msgTable,self)
		self.m_uguiMsgHandler:Touch(panelObj)
	end

	-- UI事件重新绑定(UI中有动态添加的元素时, 需要重新绑定UI事件, 否则新元素无法响应UI事件) by lxh
	def.method().ReTouch = function (self)
		if self.m_panel == nil then
			return;
		end
		self.m_uguiMsgHandler:Touch(self.m_panel)
	end

	def.virtual("string","=>","function")._onEvent = function(self, eventName)

		local ECGUIMan = require "Game.GUI.TBGUIMan"
		local func = self:tryget(eventName)
		local beenHooked = false
		local f = function(self,id,param1,param2,param3,param4)
			if eventName == "onClick" or eventName == "onDoubleClick" or eventName == "onClickObj" or (eventName == "onPress" and param1 == true) then
				local TouchName = id
				if eventName == "onClickObj" then
					TouchName = id.name
				end
				if self.m_panel and self.m_panel ~= nil then
					self:OnOutTouchDisappear(TouchName,self.m_panel.name)
					local TBGUIMan = require "Game.GUI.TBGUIMan"
					TBGUIMan.Instance():NotifyDisappear(self.m_panel.name)
					Event.DispatchEvent(ModuleId.Common,gmodule.notifyId.Common.CLICK_BTN,{self.m_panel.name,TouchName})
				end
				Event.DispatchEvent(ModuleId.Common, gmodule.notifyId.Common.CLOSE_TIP, {});
			end
			
			local handlers = s_handlers[eventName]
			if handlers then
				for _,handler in ipairs(handlers) do
					if handler and handler(self,id,param1,param2,param3)  then
						beenHooked = true
						break
					end
				end
			end

			local innerFunc =function(call,innerSelf)
				if call then
					if param1 ~= nil and param2 ~= nil and param3 ~= nil and param4 ~= nil then
						call(innerSelf,id,param1,param2,param3,param4)
					elseif param1 ~= nil and param2 ~= nil and param3 ~= nil  then
						call(innerSelf,id,param1,param2,param3)
					elseif param1  ~= nil and param2  ~= nil then
						call(innerSelf,id,param1,param2)
					elseif param1  ~= nil then
						call(innerSelf,id,param1)
					else
						call(innerSelf,id)
					end
				end
			end
			innerFunc(func,self)
		
		end
		if not beenHooked and not func then
			return nil
		end
		return f
	end
	--[[
		增加特定GameObject的事件给特定handler的特定成员方法处理
	]]
	def.method("table","userdata","string", "string").AddEventLuaHandler = function(self, handler, obj, eventName, funcName)
		self:AddEventLuaHandlerWithPara(handler,obj,eventName,funcName,nil)
	end
	def.method("table","userdata","string", "string","dynamic").AddEventLuaHandlerWithPara = function(self, handler, obj, eventName, funcName,extraPara)
		if  self.m_uguiMsgHandler == nil or handler ==nil then
			return
		end
		local func = handler[funcName]
		if func==nil or obj == nil  then
			return
		end
		local refFuncName = string.format("%s%d ",funcName,obj:GetInstanceID()) --改用InstanceID--

		self.m_uguiMsgHandler:AddLuaEventHandler(obj, eventName, refFuncName)
		self.m_msgTable[refFuncName] = function(self,eventName,param0,param1,param2,param3)
			if eventName == "onClick" or eventName == "onDoubleClick" or eventName == "onClickObj" or (eventName == "onPress" and param1 == true)then
				self:OnOutTouchDisappear(obj.name,self.m_panel.name)
				local TBGUIMan = require "Game.GUI.TBGUIMan"
				TBGUIMan.Instance():NotifyDisappear(self.m_panel.name)
				Event.DispatchEvent(ModuleId.Common,gmodule.notifyId.Common.CLICK_BTN,{self.m_panel.name,obj.name})
				Event.DispatchEvent(ModuleId.Common, gmodule.notifyId.Common.CLOSE_TIP, {});
			end
			local params ={}
			if param0~=nil then 
				table.insert( params, param0)
			end
			if param1~=nil then 
				table.insert( params, param1)
			end
			if param2~=nil then 
				table.insert( params, param2)
			end
			if param3~=nil then 
				table.insert( params, param3)
			end
			if extraPara ~=nil then 
				table.insert( params, extraPara)
			end
			-- if param1 ~= nil and param2 ~= nil and param3 ~= nil  then
			-- 	func(handler,param0,param1,param2,param3)
			-- elseif param1  ~= nil and param2  ~= nil then
			-- 	func(handler,param0,param1,param2)
			-- elseif param1  ~= nil then
			-- 	func(handler,param0,param1)
			-- else
			if #params>0 then
				func(handler,unpack(params))
			else
				func(handler)
			end
		end
	end

	def.static("string","function").AddEventHook = function(eventName,func)
		local funcs = s_handlers[eventName]
		if not funcs then
			s_handlers[eventName] = {}
			funcs = s_handlers[eventName]
		end
		funcs[#funcs+1] =func
	end

	def.static("string","function").RemoveEventHook = function(eventName,func)
		local funcs = s_handlers[eventName]
		if not funcs then return end
		for i,handler in ipairs(funcs) do
			if handler == func then
				funcs[i] = false
				return
			end
		end
	end
	def.method().DoUILang = function(self)
		local uilocalization = self.m_panel:GetComponent("UILocalization")
		if(uilocalization ~= nil) then
		   local langdata = uilocalization:GetLangDataList()
		   local ConfigMgr = require("Main.ConfigMgr");
		   local count = langdata.Count
		   local langid = 0
		   local str = ""
		   for i=0,count-1 do
			   local data = langdata:get_Item(i)
			   if(data ~= nil) then
				  langid = data.m_LangId
				  str = ConfigMgr.GetDataByKey("language_cfg",langid)
				  data:SetText(str)
			   end
		   end
		end   
	end


	---------------
	-- 基础函数----
	---------------
	--[[
		在创建好 GUI 元素后触发，创建失败时不会触发
	]]
	def.virtual().OnCreate = function (self)
	end

	--[[
		在将要销毁 GUI 元素时触发
	]]
	def.virtual().OnDestroy = function (self)
	end

	--[[
		在销毁 GUI 元素后触发
	]]
	def.virtual().AfterDestroy = function (self)
	end
	--[[
		在添加消息事件之前调用
	]]
	def.virtual().OnBeforeAddMsg =function(self)
	end

	def.virtual("=>","boolean").IsShow = function  (self)
		if not self.m_groupObj or self.m_groupObj.isnil then return false end
		return self.m_groupObj.activeSelf
	end
	--[[
            在组所在面板显示和隐藏时触发
            param bShow: 显示时为 true，隐藏时为 false
        ]]
	def.virtual("boolean").OnShow = function (self, bShow)
	end

	--[[
		在创建好 GUI 元素后触发。子类重载时需调用父类函数
	]]
	def.virtual().OnCreateInternal = function (self)
		--self:InvokeSubGroupsFunction("OnCreateInternal")
		self:DoUILang()
		self:OnCreate()
	end

	--[[
		在将要销毁 GUI 元素时触发。子类重载时需调用父类函数
	]]
	def.virtual().OnDestroyInternal = function (self)
		--self:InvokeSubGroupsFunction("OnDestroyInternal")
		self:OnDestroy()
	end

	--[[
		在销毁 GUI 元素后触发。子类重载时需调用父类函数
	]]
	def.virtual().AfterDestroyInternal = function (self)
		--self:InvokeSubGroupsFunction("AfterDestroyInternal")
		self:AfterDestroy()
	end

	--[[
		在组所在面板显示和隐藏时触发。子类重载时需调动父类函数
		param bShow: 显示时为 true，隐藏时为 false
	]]
	def.virtual("boolean").OnShowInternal = function (self, bShow)
		self:OnShow(bShow)
		--self:InvokeSubGroupsFunction("OnShowInternal", bShow)
	end

	local function logError (str, errLevel)
		Debug.LogError(debug.traceback(str, (errLevel or 0)+1))
	end
end
TBPanelLoader.Commit()

return TBPanelLoader
