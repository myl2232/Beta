--[[********************************************************************
	created:	2018/01/19
	author:		lixuguang_cx

	purpose:	MetaBase属于第一个包含文件定义了基础管理表，MetaDefine定义了基础函数, 
				MetaClassHeader是MetaClass定义的头文件，DirtyFunction放置脏数据处理函数
*********************************************************************--]]
-- 
--
protected_ = {};
core_={};
core_.protected_ = protected_;
protected_.lua_datas_={};          --配置表
configs_ = protected_.lua_datas_;	--和客户端共享配置的引用

--关于对象模型，应该定义对象的构造通过MetaClass的属性表的默认值构造的，属性表属性注册注意依赖顺序

--协程的命名空间
protected_.GameCoroutine={};

if Enum==nil then
	Enum = {};                          --Enum名字空间，在C++中定义了
end

--在玩家数据加载完成后的回调，大地图slg类游戏需要，按优先级分类{[1]={},[2]={},[3]={}}，数字越低优先级越高
protected_.start_calls_={};

--协议定义表
EC2SProtocol={["EC2SProtocol_None"]=0};
ES2CProtocol={["ES2CProtocol_None"]=0};

--消息处理分发函数
protected_.msg_handlers_={};
--消息处理上下文收集
protected_.message_context_={};

--事件处理器lightdata映射上下文
protected_.handler_contexts_={};
--事件处理器对象映射上下文
protected_.obj_contexts_={};

--redis 的异步调用表
protected_.redis_callbacks_={};

--redis 发布订阅
protected_.redis_handlers_={};
--------------------------------------------------------------------------------
----------------------------根对象管理表-----------------------------------------
---------------------------------------------------------------------------------

----------------------------协程对象的管理表-------------------------------------
protected_.coroutines_={};

-------------------------------------玩家对象的管理表-----------------------------
--在线玩家管理表
protected_.online_users_ = {};
protected_.game_players_ = {};
protected_.game_players_.dirtys_ = {
	["parent_"] = nil,
	["parent_key_"]= nil,
	["marks_"] = {}
};
protected_.game_players_.save_pool={};
protected_.game_players_.update_dirty_count_=0;

--全服数据管理表
protected_.game_scenes_ = {};
protected_.game_scenes_.dirtys_ = {
	["parent_"] = nil,
	["parent_key_"]= nil,
	["marks_"] = {}
};
protected_.game_scenes_.save_pool={};
protected_.game_scenes_.update_dirty_count_=0;
