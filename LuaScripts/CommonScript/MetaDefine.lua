--[[********************************************************************
	created:	2018/01/19
	author:		lixuguang_cx

	purpose:	对象模型的基础函数
		对象模型初始化过程分三个阶段：
	构造阶段(protected_.constructObject)：只构造对象关系，不对属性运算； 
	[根对象]初始化阶段(init)：名字必须叫init，函数玩家自定义，系统调用; 当[根对象]本身完整创建后调用，
		这个函数里需要指定start函数的调用时机，如果服务器已经完整载入就需要立即调用start，否则根据优先级指定对应start函数
	全局初始化阶段(start)：当服务器数据完整载入后调用；这个函数可能有多个，属于自定义函数，被init调用。protected_.server_state_==Enum.EServerState_Run
*********************************************************************--]]

Enum.EServerState_Load = 1;
Enum.EServerState_Run = 2;
Enum.EServerState_Close = 3;
--
Enum.EMetaProp_Name = 1;				--名字
Enum.EMetaProp_Default = 2; 			--默认值
Enum.EMetaProp_SynMode = 3;			--同步模式
Enum.EMetaProp_Class = 4;			--属性的meta类型
----------------------------------------MetaClass辅助函数----------------------------------------------
setmetatable(core_, {__index = protected_.index_, __newindex = protected_.newindex_;});
-- index_funcs是一个表{__index, __newindex}，如果为空则默认{protected_.index_,  protected_.newindex_}
-- call_funcs是一个表{handler_, handleDirtyMark}，如果为空则默认{protected_.markDirtyCall, protected_.handleDirtyMark_}
-- 默认base_的第一个属性是desc属性，非默认情况需要重写getDesc函数
protected_.declareMetaClass = function(meta_name, index_funcs, call_funcs)
	if meta_name==nil then
		print("protected_.declareMetaClass Error: ", debug.traceback());
	end
	index_funcs = index_funcs or {protected_.index_,  protected_.newindex_};
	call_funcs = call_funcs or protected_.meta_calls_;
    protected_[meta_name]={
        __index = index_funcs[1],
	    __newindex = index_funcs[2],
	    handlers_ = {},
        updaters_={},
	    handler_ = call_funcs[1],
	    handleDirtyMark = call_funcs[2],
		getDesc = protected_.getDesc_,
		meta_props_ = {}
    };
end;

--默认第一个非map属性是desc
protected_.getDesc_ = function (object)
	if object==nil then
		return nil;
	end
	local mt = getmetatable(object);
	if mt==nil then
		return nil;
	end
	if object.base_==nil then
		return nil;
	end
	local prop = object.base_[1];
	if prop==nil then
		return nil;
	end
	local tt = type(prop);
	if tt=="string" or tt=="number" then
		return prop;
	end
	return nil;
end
--注册为map
--syn_mode： nil or 0 即同步也存盘 1只同步 2只存盘 100什么都不做; init_func(meta_class, obj, prop_name)
-- keyProp {名字， 默认值， 同步模式, class}
protected_.registMetaProp = function(meta_class, prop_name, default_func, syn_mode, prop_mt)
	if meta_class==nil then
		print("MetaClass is nil: ", debug.traceback());
	end
	gameAssert(meta_class and (prop_mt==nil or type(prop_mt)=='table'), "");
    local prop;
	local prop_indx;
	if meta_class.prop_indexes_==nil then
		prop={};
	else
		prop_indx = meta_class.prop_indexes_[prop_name];
		if prop_indx then
			prop = meta_class.meta_props_[prop_indx];
		else
			prop = {};
		end
	end
	gameAssert(type(prop_name)=="string", "");
    prop[Enum.EMetaProp_Name] = prop_name;
    if default_func~=nil then
        prop[Enum.EMetaProp_Default] = default_func;
    end
    if syn_mode~=nil then
        prop[Enum.EMetaProp_SynMode] = syn_mode;
    end
	if prop_mt then
		prop[Enum.EMetaProp_Class] = prop_mt;
	end
    if meta_class.meta_props_==nil then
        meta_class.meta_props_ = {}
    end
	if prop_indx==nil then
		table.insert(meta_class.meta_props_, prop);
		if meta_class.prop_indexes_==nil then
			meta_class.prop_indexes_ = {}
		end
		meta_class.prop_indexes_[prop_name] = #meta_class.meta_props_;
	end
end

--默认在base_中创建一个map_表。map_mode: nil or 0表示key为数字map；其他表示key为任何的map，只要不在meta_props_中的属性都会被放入map_中
protected_.registMetaMap = function(meta_class, map_mode, syn_mode, prop_mt)
	gameAssert(meta_class and (prop_mt==nil or type(prop_mt)=='table'), "");
	protected_.registMetaProp(meta_class, "map_", nil, syn_mode, prop_mt);
	meta_class.map_mode_ = map_mode or 0;
end

--作为object的普通定义属性，是否只读取决于class定义
protected_.registExProp = function(meta_class, prop_name, prop_mt, save_call)
	gameAssert(meta_class and (prop_mt==nil or type(prop_mt)=='table'), "");
	gameAssert(save_call~=nil, "");
    if meta_class.ex_props_==nil then
        meta_class.ex_props_ = {}
    end
	-- 注册到基础属性里，只在base_占一个空位不用
	protected_.registMetaProp(meta_class, prop_name, nil, 1);
    meta_class.ex_props_[prop_name] = {prop_mt, save_call};
end

protected_.getMetaProp = function(meta_class, prop_name)
	if meta_class.prop_indexes_==nil then
		return nil;
	end
	local indx = meta_class.prop_indexes_[prop_name];
	if indx then
		return meta_class.meta_props_[indx];
	end
	if meta_class.map_mode_==nil then
		return nil;
	elseif meta_class.map_mode_==0 then
		if type(prop_name)=="number" then
			indx = meta_class.prop_indexes_["map_"];
		else
			return nil;
		end
	else
		indx = meta_class.prop_indexes_["map_"];
	end
	return meta_class.meta_props_[indx];
end
protected_.getMetaPropIndex = function(meta_class, prop_name)
	if meta_class.prop_indexes_==nil then
		return nil;
	end
	local indx = meta_class.prop_indexes_[prop_name];
	return indx;
end
protected_.getMetaPropName = function(meta_class, prop_index)
	if meta_class.meta_props_==nil then
		return nil;
	end
	local prop = meta_class.meta_props_[prop_index];
	if not prop then
		return ;
	end
	return prop[Enum.EMetaProp_Name];
end

protected_.isPropSynMessage = function(syn_mode)
    return syn_mode==nil or syn_mode==0 or syn_mode==1;
end

protected_.isPropSave = function(syn_mode)
    return syn_mode==nil or syn_mode==0 or syn_mode==2;
end

protected_.isMetaExProp = function(meta_class, prop_name)
    if meta_class.ex_props_==nil then
        return false;
    end
    return meta_class.ex_props_[prop_name]~=nil;
end

protected_.setParentObject = function(object, parent, parent_key)
	object.dirtys_={
		["parent_"] = parent,
		["parent_key_"] = parent_key,
		["marks_"]		= {}
	}
	local pmt = getmetatable(parent);
	if pmt and protected_.isMetaExProp(pmt, parent_key) then
		rawset(parent, parent_key, object);
	else
		parent[parent_key] = object;
	end
end
--如果object不空，形式必然为{base_={}}
protected_.constructObject = function(mt, object, parent, parent_key)
	object = object or {};
	if getmetatable(object) then
		return object;
	end
	if object.base_==nil then
		object = {base_=object};
	end
--	if mt==nil or type(mt)~="table" then
--		print("MMMMMMMMMMMMMMMMMMMMMMMMmmm ", debug.traceback());
--	end
	setmetatable(object, mt);
	if mt.meta_props_==nil then
		print("protected_.constructObject Error: ", debug.traceback());
	end
	--对于ex_prop 因为class注册class是空所以不会被construct
	for k,v in pairs(mt.meta_props_) do
		local syn_mode = v[Enum.EMetaProp_SynMode];
		if syn_mode==nil or syn_mode<=2 then
			local p_mt = v[Enum.EMetaProp_Class];
			if p_mt then
				if v[1]=="map_" then
					if object.base_[k] then
						for kk,vv in pairs(object.base_[k]) do
							protected_.constructObject(p_mt, vv, object, kk);
						end
					end
				else
					protected_.constructObject(p_mt, (object.base_ and object.base_[k]) or nil, object, v[Enum.EMetaProp_Name]);
				end
			end
		else
			object.base_[k] = nil;
		end
	end

	--因为dirtys_还没有设置，所以都没有脏
	if parent then
		object.dirtys_={
			["parent_"] = parent,
			["parent_key_"] = parent_key,
			["marks_"]		= {}
		}
		local pmt = getmetatable(parent);
		if pmt and protected_.isMetaExProp(pmt, parent_key) then
			rawset(parent, parent_key, object);
		else
			if parent_key==nil then
				print("FFFFFFFFFFFFFFFFFFFFFF ", debug.traceback());
			end
			parent[parent_key] = object;
		end
		--如果是从父对象constructObject调用进入的，由于父对象的dirtys_还没有创建，所以父节点的脏是无法设定的
		protected_.markDirty(parent, parent_key);
	end
	return object;
end

--为了便于使用添加此接口，如果object为{}或者nil时返回nil
protected_.unserialObject = function(mt, object, parent, parent_key)
	if object==nil or not next(object) then
		object = nil;
	else
		if getmetatable(object) then
			return object;
		end
		if object.base_==nil then
			object = {base_=object};
		end
	--	if mt==nil or type(mt)~="table" then
	--		print("MMMMMMMMMMMMMMMMMMMMMMMMmmm ", debug.traceback());
	--	end
		setmetatable(object, mt);
		if mt.meta_props_==nil then
			print("protected_.unserialObject Error: ", debug.traceback());
		end
		--对于ex_prop 因为class注册class是空所以不会被construct
		for k,v in pairs(mt.meta_props_) do
			local syn_mode = v[Enum.EMetaProp_SynMode];
			if syn_mode==nil or syn_mode<=2 then
				local p_mt = v[Enum.EMetaProp_Class];
				if p_mt then
					if v[1]=="map_" then
						if object.base_[k] then
							for kk,vv in pairs(object.base_[k]) do
								protected_.unserialObject(p_mt, vv, object, kk);
							end
						end
					else
						protected_.unserialObject(p_mt, (object.base_ and object.base_[k]) or nil, object, v[Enum.EMetaProp_Name]);
					end
				end
			else
				object.base_[k] = nil;
			end
		end
		
	end

	--因为dirtys_还没有设置，所以都没有脏
	if parent then
		if object then
			object.dirtys_={
				["parent_"] = parent,
				["parent_key_"] = parent_key,
				["marks_"]		= {}
			}
		end
		local pmt = getmetatable(parent);
		if pmt and protected_.isMetaExProp(pmt, parent_key) then
			rawset(parent, parent_key, object);
		else
			if parent_key==nil then
				print("FFFFFFFFFFFFFFFFFFFFFF ", debug.traceback());
			end
			parent[parent_key] = object;
		end
		--如果是从父对象unserialObject调用进入的，由于父对象的dirtys_还没有创建，所以父节点的脏是无法设定的
		protected_.markDirty(parent, parent_key);
	end
	return object;
end

--需要手动调用，例如当玩家数据完全加载后调用
protected_.initObject = function(object)
--遍历调用init
	local mt = getmetatable(object);
	if mt.meta_props_==nil then
		return;
	end
	if object.preInitObject then
		object:preInitObject();
	end
	for k,v in pairs(mt.meta_props_) do
		local p_mt = v[Enum.EMetaProp_Class];
		if p_mt then
			if v[1]=="map_" then
				if object.base_ and object.base_[k] then
					for kk,vv in pairs(object.base_[k]) do
						if vv.initObject then
							vv:initObject();
						end
					end
				end
			else
				local obj = object.base_[k];
				if obj and p_mt.initObject then
					obj:initObject();
				end
			end
		end
	end
	if object.initObject then
		object:initObject();
	end
end

--call_func=function(object, context)
protected_.registStartCall = function(call_func, object, prior, context)
	prior = prior or 100;
	if prior<1 then
		prior = 1;
	elseif prior>100 then
		prior = 100;
	else
		prior = math.floor(prior);
	end
	local p_que = protected_.start_calls_[prior];
	if p_que==nil then
		p_que = {};
		protected_.start_calls_[prior] = p_que;
	end
	table.insert(p_que, {call_func, object, context});
end

------------------------------------newindex index定义---------------------------------------------------

protected_.k_updater_={};           --防止k读回调导致递归调用
protected_.newindex_ = function (t, k, v)					-- class_newindex
	local mt = getmetatable(t);
	if mt[k] then 
		gameLog("Try to set MetaData: ["..k.."]!!!!!!!!!");
		-- mt[k] = v;--静态成员
	else
		local meta_prop = protected_.getMetaProp(mt, k);
		if meta_prop==nil then
			rawset(t, k, v);
			return;
		end
        if protected_.isMetaExProp(mt, k) then
			gameLog("Protected newindex: ["..k.."]!!!!!!!!!");
            return ;
        end
		local handle_k = k;
        local k_id;
        local old_val;
        local obj_base = rawget(t, "base_");
		if obj_base==nil then
            obj_base = {};
			rawset(t, "base_", obj_base);
		end
        k_id = mt.prop_indexes_[k];
		if k_id==nil then
			if mt.map_mode_==nil then
				rawset(t, k, v);
				return;
			else
				if mt.map_mode_==0 and type(k)~="number" then
					rawset(t, k, v);
					return;
				end
				handle_k = "map_";
				local map_id = mt.prop_indexes_["map_"];
				k_id = k;
				if obj_base[map_id]==nil then
					obj_base[map_id] = {};
				end
				obj_base = obj_base[map_id];
			end
		end
		old_val = protected_.getRawValue(t, k);
        obj_base[k_id] = v;
		if mt.handler_~=nil then
			mt.handler_(t, k, old_val, v);
		end
		if mt.handlers_ and mt.handlers_[handle_k]~=nil then
			mt.handlers_[handle_k](t, k, old_val, v);
		end
	end
end

protected_.index_ = function (t, k)					-- class_index
	local mt = getmetatable(t);
	if mt[k] then 
		return mt[k];
	else
        if mt.updaters_ and mt.updaters_[k] then
            if protected_.k_updater_[t]==nil then
                protected_.k_updater_[t]={};
            end
            if protected_.k_updater_[t][k]==nil then
                protected_.k_updater_[t][k]=1;
                mt.updaters_[k](t, k);--防止递归调用
                protected_.k_updater_[t][k]=nil;
            end
            if next(protected_.k_updater_[t])==nil then
                protected_.k_updater_[t] = nil;
            end
        end
        return protected_.getRawValue(t, k);
	end
end

protected_.getRawValue = function (t, k)
    local mt = getmetatable(t);
    if mt==nil then
        return t[k];
	elseif mt[k] then 
		return mt[k];
	else
		local meta_prop = protected_.getMetaProp(mt, k);
		if meta_prop==nil then
			return rawget(t, k);
		end
        local obj_base = rawget(t, "base_");
		if obj_base==nil then
			obj_base = {};
			rawset(t, "base_", obj_base);
		end
		local k_id = mt.prop_indexes_[k];
		if k_id==nil then
			if mt.map_mode_==nil then
				return rawget(t, k);
			else
				if mt.map_mode_==0 and type(k)~="number" then
					return rawget(t, k);
				end
				local map_id = mt.prop_indexes_["map_"];
				k_id = k;
				if obj_base[map_id]==nil then
					obj_base[map_id] = {};
				end
				obj_base = obj_base[map_id];
			end
		else
			if k=="map_" then
				if obj_base[k_id]==nil then
					obj_base[k_id] = {}
				end
				return obj_base[k_id];
			elseif protected_.isMetaExProp(mt, k) then
				return rawget(t, k);
			end
		end
		if obj_base[k_id] then
			return obj_base[k_id];
		end
		if meta_prop[Enum.EMetaProp_Class] then
			return nil;
		end
		default_v = meta_prop[Enum.EMetaProp_Default];
		if type(default_v)=="function" then
			--防止default_v(t)无限递归
			obj_base[k_id] = 0;
			local default_v = default_v(t);
			obj_base[k_id] = default_v;
			return default_v;
		end
		return default_v;
	end
end

------------------------------------------脏数据处理基础函数----------------------------------------------

--mark==nil是第一层，外部调用只能使用markDirty(parent, parent_key)
-- return 0表示祖先节点已经有更高级脏了，1表示设置成功
protected_.markDirtyCall = function (t, k, old_val, v)
	if t.dirtys_==nil then
		return;
	end
	if old_val==v then
		return;
	end
	if protected_.dirty_old_ then
		if t.old_==nil then
			t.old_={[k]=old_val};
		elseif t.old_[k]==nil then
			t.old_[k]=old_val;
		end
	end
	protected_.markDirty(t, k);
end
protected_.markDirty = function (parent, parent_key, mark)
    if parent_key==nil then
	    print("#######################: "..debug.traceback());
    end
	if parent.dirtys_==nil then
		return;
	end
	if mark == nil then
		mark = 7; -- 111
	end
	if parent.dirtys_.marks_==nil then
		parent.dirtys_.marks_={}
	end
	-- print("#####markDirty parent: ", parent.dirtys_.parent_key_, parent_key, mark);
	-- mark=111时，说明子节点全部已经被更换，后面的dirty内容应该为空
	-- parent mark==111时，说明需要整个更新，子节点就不需要再设脏了
    if parent.dirtys_.marks_[parent_key] then
        if parent.dirtys_.marks_[parent_key]>=7 then
            return false;
	    elseif mark>=parent.dirtys_.marks_[parent_key] then
            parent.dirtys_.marks_[parent_key] = mark;--111
        else
            -- print("XXXXXXXXXXXXXX ", parent.dirtys_.marks_[parent_key], mark);
            mark = parent.dirtys_.marks_[parent_key];
--	    elseif mark==1 and parent.dirtys_.marks_[parent_key]==1 then
--		    return true;
	    end
    end
	if parent.dirtys_.parent_~=nil then
		if not protected_.markDirty(parent.dirtys_.parent_, parent.dirtys_.parent_key_, 1) then
			return false;
		end
	end
--    if mark==7 then
--	    print("+++++++++++++++++markDirty: "..debug.traceback());
--    end
    parent.dirtys_.marks_ = parent.dirtys_.marks_ or {};
	parent.dirtys_.marks_[parent_key] = mark;--1 表示子节点数据脏了，7表示消息和存盘都脏的位置
	return true;
end
protected_.clearDirty = function (t, keep_save)
	if type(t)~="table" or t.dirtys_==nil or t.dirtys_.marks_==nil then
		return;
	end
    keep_save = keep_save or false;
	if protected_.dirty_old_ then
		t.old_ = nil;
	end
	local count = 0;
	for k,v in pairs(t.dirtys_.marks_) do
        if keep_save and v>3 then
            t.dirtys_.marks_[k] = 3;
        else
            t.dirtys_.marks_[k] = nil;
        end
		protected_.clearDirty(protected_.getRawValue(t,k), keep_save);
	end
end
--{v}这种形式可以兼容v=nil情况，虽然效率稍低，但是形式简单
protected_.setSynDirtyValue = function( syn_table, mt, k, v )
    if syn_table==nil then
        syn_table = {};
    end
	local k_id = mt.prop_indexes_[k];
	if k_id then
		syn_table[k_id]=v;
		return syn_table;
	end
	if mt.map_mode_==nil then
		return syn_table;
	elseif mt.map_mode_==0 and type(k)~="number" then
		return syn_table;
	end
	k_id = mt.prop_indexes_["map_"];
	gameAssert(k_id, "");
	local mp_t = syn_table[k_id];
	if mp_t==nil then
		mp_t = {};
		syn_table[k_id] = mp_t;
	end
	mp_t[k] = v;
    return syn_table;
end
--return: 第一个表示是否需要存盘，第二个是同步脏数据表
protected_.handleDirtyMark_ = function (self, context, save_flag, send_flag)
	if self.dirtys_==nil or self.dirtys_.marks_==nil  or next(self.dirtys_.marks_)==nil then
		return 1, nil;
	end
	local self_save_flag=0;
    local dirty_syn_table;--表[1]base_，[2]map_
    local mt = getmetatable(self);
	for k,v in pairs(self.dirtys_.marks_) do
		table.insert(context.path,k);
		local svflag = save_flag;
		local sndflag = send_flag;
		local key_syn_object;
        local key_prop = protected_.getMetaProp(mt, k);
        gameAssert(key_prop, "");
        local v_t = protected_.getRawValue(self, k);
		if type(v_t)~="table" or v_t.handleDirtyMark==nil then
            if svflag~=0 then
				self_save_flag = 1;
				svflag = 0;
			end
			if sndflag~=0 then
                if protected_.isPropSynMessage(key_prop[Enum.EMetaProp_SynMode]) then
					if v>=7 then
						if v_t == nil then
							key_syn_object = {[0] = v_t};
						else
							key_syn_object = v_t;
						end
                        self.dirtys_.marks_[k]=3; -- 存脏
		                protected_.clearDirty(v_t);
				        sndflag = 0;
                    end
                else
				    sndflag = 0;
					--虽然针对此属性不存盘，但是上层的存盘依然会裹挟到这里
					if protected_.isPropSave(key_prop[Enum.EMetaProp_SynMode]) then
						self.dirtys_.marks_[k]=3; -- 存脏
					else
						self.dirtys_.marks_[k]=0;
					end
		            protected_.clearDirty(v_t);
                end
			end
        else
            if sndflag~=0 then
                if protected_.isPropSynMessage(key_prop[Enum.EMetaProp_SynMode]) then
                    if v>=7 then
						if v_t==nil or type(v_t)=="table" then
							key_syn_object = {[0]=v_t}; --prop index=0不可能被占用，这样的节点就是叶子节点
						else
							key_syn_object = v_t;
						end
                        self.dirtys_.marks_[k]=3; -- 存脏
		                protected_.clearDirty(v_t, true);
				        sndflag = 0;
                    end
                else
                    self.dirtys_.marks_[k]=3; -- 存脏
		            protected_.clearDirty(v_t, true);
					sndflag = 0;
                end
            end
			if v>=1 and svflag~=0 then
				if self.ex_props_ and self.ex_props_[k]~=nil then
					self.ex_props_[k][2](context.context_key_, k, v_t, context);	-- (player_id, save_key, save_data, context)
					-- print("saveDB: "..k);
					svflag=0;
				elseif v>=3 then --v>=3时还没有遇到ex_props_，那就是save game player了	
					--虽然针对此属性不存盘，但是上层的存盘依然会裹挟到这里
					if protected_.isPropSave(key_prop[Enum.EMetaProp_SynMode]) then
						self_save_flag = 1;
					end
					svflag = 0;
				else
					-- gameAssert(v<2, "Lua Save Error: "..k);
				end;
			end
            if svflag~=0 or sndflag~=0 then
				local tmp;
				if svflag~=0 then
					tmp, key_syn_object=v_t.handleDirtyMark(v_t, context, svflag, sndflag);
					if tmp==1 then
						self_save_flag = 1;
					end
				else
					tmp,key_syn_object = v_t.handleDirtyMark(v_t, context, svflag, sndflag);
				end
			end
        end
        if key_syn_object then
            dirty_syn_table = protected_.setSynDirtyValue(dirty_syn_table, mt, k, key_syn_object);
		end
		table.remove(context.path);
    end
    if save_flag~=0 then
		protected_.clearDirty(self);
    end
	if self_save_flag~=0 then
		return 1, dirty_syn_table;
		-- protected_.saveUserCall(context.context_key_, self.dirtys_.parent_key_, self, context);
		-- print("saveThis player["..context.context_key_.."] key["..self.dirtys_.parent_key_.."]");
	end
	return 0, dirty_syn_table;
end

protected_.saveUserCall = function (player_id, save_key, save_data)
	if player_id==save_key then
		--print("*****************Save User["..player_id.."]*******************");
		local db_data = "";
        local r=true, s;
        if save_data then
            r, s = pcall(protected_.mp_.pack, save_data);
            if not r then
                gameLog("saveUserCall["..player_id.."] error: "..s);
                return;
            end
            db_data = s;
        end
		saveUser(player_id, db_data);
	else
        local r=true, s;
		if type(save_key)=="string" then
		    local db_data = "";
            if save_data then
                r, s = pcall(protected_.mp_.pack, save_data);
                if not r then
                    gameLog("saveUserCall2["..player_id.."] error: "..s);
                    return;
                end
                db_data = s;
            end
			-- print("+++++++db_data len="..(#db_data));
			saveUserTable(player_id, save_key, db_data);
		else
			gameLog("!!!! Error Player["..player_id.."] key["..save_key.."]!!!!!!");
		end
	end
end
protected_.saveDBUserTable = function (player_id, save_key, save_data)
	if player_id~=save_key then
		local r=true, s;
		if type(save_key)=="string" then
		    local db_data = "";
            if save_data then
                r, s = pcall(protected_.mp_.pack, save_data);
                if not r then
                    gameLog("saveUserCall2["..player_id.."] error: "..s);
                    return;
                end
                db_data = s;
            end
			-- print("+++++++db_data len="..(#db_data));
			saveDBUserTable(player_id, save_key, db_data);
		else
			gameLog("!!!! Error Player["..player_id.."] key["..save_key.."]!!!!!!");
		end
	end
end
protected_.saveSceneCall = function (server_id, save_key, save_data)
	if server_id==save_key then
		--print("*****************Save User["..player_id.."]*******************");
		local db_data = "";
		local r=true, s;
        if save_data then
            r, s = pcall(protected_.mp_.pack, save_data);
            if not r then
                gameLog("saveSceneCall["..server_id.."] error: "..s);
                return;
            end
            db_data = s;
		end
		saveScene(server_id, db_data);
	end
end
protected_.sendDirtyMessage = function (player_id, syn_table)
	local game_player = protected_.game_players_[player_id];
	if game_player==nil or game_player.connect_id_==nil or game_player.connect_id_==0 or game_player.conn_state_==nil or game_player.conn_state_~=1 then
		return;
	end
	sendMessage(game_player.connect_id_, {ES2CProtocol["ES2CProtocol_SynDirty"], syn_table});
end

--protected_.sendDirtyMessage = function (player_id, syn_table)
--	local game_player = protected_.game_players_[player_id];
--	if game_player==nil or game_player.connect_id_==0 or game_player.conn_state_~=1 then
--		print("protected_.sendDirtyMessage", game_player.connect_id_, game_player.conn_state_);
--		return;
--	end
--	sendMessage(game_player.connect_id_, {ES2CProtocol["ES2CProtocol_SynDirty"], syn_table});
--end

protected_.meta_calls_ = {protected_.markDirtyCall, protected_.handleDirtyMark_};

--protected_.sendDirtyMessage = function (v, context)
--	local game_player = protected_.game_players_[context.context_key_];
--	if game_player==nil or game_player.connect_id_==0 or game_player.conn_state_~=1 then
--		return;
--	end
--	local proto_id = ES2CProtocol["ES2CProtocol_SynDirty"];
--	local location_path = context.path or {};
--    -- if #location_path>0 and location_path[#location_path]=="gas_" then
--    --    print("Dirty gas_ = "..v);
--    -- end
--	sendMessage(game_player.connect_id_, {proto_id, location_path, v});
--end
---------------------------redis call-----------------------------------
protected_.registRedisCall = function (call_table, db_id, db_cmd, table_name, ...)
	-- print("XXXXXXXXXXXX ", "protected_.registRedisCall");
	local req_key = redisCall(db_id, db_cmd, table_name, ...);
	if req_key==nil then
		return -1;
	end
	protected_.redis_callbacks_[req_key] = call_table;
	return 0;
end
protected_.handleRedisCall = function(req_key, r_value)
	-- print("XXXXXXXXXXXX ", "protected_.handleRedisCall");
	local call_table = protected_.redis_callbacks_[req_key];
	if call_table==nil then
		return 0;
	end
	protected_.redis_callbacks_[req_key] = nil;
	call_table:handleCallback(r_value);
	return 0;
end
protected_.registasyncRedisCall = function (call_table, db_cmd, ...)
	-- print("XXXXXXXXXXXX ", "protected_.registRedisCall");
	local req_key = asyncredisCall(db_cmd, ...);
	if req_key==nil then
		return -1;
	end
	protected_.redis_callbacks_[req_key] = call_table;
	return 0;
end
-------------------------------消息处理函数-------------------------------

protected_.handlePacket = function( handlers, client_id, msg_str, ...)
	local msg_len = #msg_str;
	if msg_len == 0 then
		gameLog("handlePacket message is empty!");
		return -1;
	end
	local body_msg = msg_str;
	local dd = unpackMessage(body_msg);
	if dd==nil then
		gameLog("handlePacket nil!");
		-- gameAssert(false, "");
		return -1;
	end
	if #dd==0 then
		gameLog("handlePacket msg_str empty!");
		return 0;
	end
	-- print("XX handlePacket: ", dd[1], msg_len);
	local fun_call = handlers[dd[1]];
	if fun_call==nil then
		gameLog("Client["..client_id.."] error msg["..dd[1].."]!!!!");
		return -1;
	end
	--print("2 handlePacket: ", dd[1]);
    local old_context = nil;
    if protected_.message_context_==nil then
        protected_.message_context_={};
    elseif next(protected_.message_context_)~=nil then
        old_context = protected_.message_context_;
        protected_.message_context_={};
    end
	local ret;
	if handlers.handlePacket then
		-- print("1 protected_.handlePacket ", ...);
		ret = handlers:handlePacket(client_id, dd, ...);
	else
		-- print("2 protected_.handlePacket ", ...);
		ret = fun_call(client_id, dd, ...);
	end

    local msg_context = protected_.message_context_;
    if old_context then
        protected_.message_context_= old_context;
    elseif next(msg_context)~=nil then
        protected_.message_context_={};
    end
	-- print("FFFFFFFFFFFFFFFFFFFFFFFFFFF ", dump_obj(msg_context));
    local rmsg_str = nil;
    if next(msg_context)~=nil then
        local rr;
        rr, rmsg_str = pcall(protected_.mp_.pack, msg_context);
        if not rr then
            gameLog("handlePacket rmsg error ["..client_id.."]: "..rmsg_str);
            rmsg_str = nil
        end
    end
	rmsg_str = rmsg_str or "";
	return ret, rmsg_str;
end

protected_.isHero = function( unit_sid)
	return unit_sid<10000;
end
protected_.isPet = function( unit_sid)
	return unit_sid>10000 and unit_sid<20000;
end
protected_.isSlotUnit = function( unit_sid)
	return unit_sid==30001;
end
--不改变属性和技能的变身
protected_.isMaskUnit = function( unit_sid)
	return unit_sid>20000 and unit_sid<21000;
end
protected_.isOtherUnit = function( unit_sid)
	return unit_sid_ < 30000 and unit_sid_ > 21000;
end
protected_.isAI = function( unit_sid)
	return unit_sid < 22000 and unit_sid > 21000;
end
--不改变属性的可查看变身
protected_.isVisibleMaskUnit = function( unit_sid)
	return unit_sid>20000 and unit_sid < 20800;
end
--不改变属性的不查看变身
protected_.isInvisibleUnit = function( unit_sid)
	return unit_sid > 20800 and unit_sid < 21000;
end