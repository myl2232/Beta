
protected_.commands_ = {};
--[[ 
params 是 table or nil
key 如下： 
	times 次数
]]
protected_.doCommand = function(game_player, cmd_t, params, context)
	local cmd = protected_.commands_[cmd_t[1]];
	if cmd==nil then
        log.warn("doCommand", nil, "cmd is nil ", cmd_t[1]);
		return false;
	end
	return cmd(game_player, cmd_t, params, context);
end;
protected_.doCommands = function(game_player, cmds, params, contexts)
    if cmds==nil then
        return false;
    end
	local ret = true
	for k,v in pairs(cmds) do
		if type(v) ~= "table" then
			return false;
		end
		local cmd = protected_.commands_[v[1]];
		if cmd~=nil then
			contexts = contexts or {};
			local context;
			if contexts then
				context = contexts[k] or {};
				contexts[k] = context;
			end
			if not cmd(game_player, v, params, context) then
				ret = false
			end
		else
            log.warn("doCommands", nil, "Command not exist", v[1]);
			return false;
		end
	end
	return ret
end;
protected_.checkCommands = function(game_player, cmds, params, contexts)
    if cmds==nil then
        return false;
    end
	for k,v in pairs(cmds) do
		if type(v) ~= "table" then
			return false;
		end
		local cond = protected_.commands_[v[1].."_cond"];
        if not cond then
            log.warn("checkCommands", nil, "Command not exist", v[1]);
			return false
		end
		contexts = contexts or {};
		local context;
		if contexts then
			context = {};
			contexts[k] = context;
		end
		if cond~=nil and not cond(game_player, v, params, context) then
			return false;
		end
	end
	return true;
end;
protected_.undoCommands = function(game_player, cmds, params, contexts)
	for k,v in pairs(cmds) do
		local undo = protected_.commands_[v[1].."_undo"];
		if undo~=nil then
			contexts = contexts or {};
			local context;
			if contexts then
				context = contexts[k] or {};
				contexts[k] = context;
			end
			undo(game_player, v, params, context);
		else
			log.warn("undoCommands", nil, "Command not exist", v[1]);
			return;
		end
	end
end;
--addProp
protected_.commands_.addProp = function (game_player, cmd_t, params, context)
	local prop = cmd_t[2];
	local val = cmd_t[3];
	local prop_val = game_player[prop];
	if prop_val == nil or type(val) ~= "number" or val < 0 then
        return false;
    end
    local times = protected_.commands_.getArgument(params, "times") or 1;

    local v = prop_val + val * times;
    if v < 0 then
        return false;
    end
	game_player[prop] = v;
	if context then
		context.get_cmd = {{cmd_t[1], cmd_t[2], val*times}};
	end
    return true;
end
protected_.commands_.addProp_cond = function (game_player, cmd_t, params, context)
	local prop = cmd_t[2];
	local val = cmd_t[3];
	local prop_val = game_player[prop];
	if prop_val == nil or type(val) ~= "number" or val < 0 then
        return false;
    end
    return true;
end
protected_.commands_.addProp_undo = function (game_player, cmd_t, params, context)
	local prop = cmd_t[2];
	local val = cmd_t[3];
	local prop_val = game_player[prop];
	if prop_val == nil or type(val) ~= "number" or val < 0 then
        return false;
    end
    local times = protected_.commands_.getArgument(params, "times") or 1;
    local v = prop_val - val * times;
    if v < 0 then
        return false;
    end
    game_player[prop] = v;
    return true;
end
--delProp
protected_.commands_.delProp = function (game_player, cmd_t, params, context)
    local prop = cmd_t[2];
	local val = cmd_t[3];
	local prop_val = game_player[prop];
	if prop_val == nil or type(val) ~= "number" or val < 0 then
        return false;
    end
    local times = protected_.commands_.getArgument(params, "times") or 1;
    local v = prop_val - val * times;
    if v < 0 then
        return false;
    end
    game_player[prop] = v;
    return true;
end
protected_.commands_.delProp_cond = function (game_player, cmd_t, params, context)
	local prop = cmd_t[2];
	local val = cmd_t[3];
	local prop_val = game_player[prop];
	if prop_val == nil or type(val) ~= "number" or val < 0 then
        return false;
    end
    local times = protected_.commands_.getArgument(params, "times") or 1;
    return prop_val >= (val * times);
end
protected_.commands_.delProp_undo = function (game_player, cmd_t, params, context)
	local prop = cmd_t[2];
	local val = cmd_t[3];
	local prop_val = game_player[prop];
	if prop_val == nil or type(val) ~= "number" or val < 0 then
        return false;
    end
    local times = protected_.commands_.getArgument(params, "times") or 1;
    local v = prop_val + val * times;
    if v < 0 then
        return false;
    end
    game_player[prop] = v;
    return true;
end
--addItem
protected_.commands_.addItem = function (game_player, cmd_t, params, context)
	local cmd_type = cmd_t[2];
	if type(cmd_type) == "number" then --{"addItem", item_sid, count}
		local item_sid = cmd_t[2];
		local count = cmd_t[3];
		local rt = game_player.item_bag_:addItems({[item_sid] = count}, context.judge_flag);
		if rt then
			context.get_cmd = {cmd_t};
			context.judge_flag = nil;
		end
		return rt;
	elseif type(cmd_type) == "table" then --{"addItem", {[item_sid] = count, [item_sid] = count}}
		local items = cmd_t[2];
		local rt = game_player.item_bag_:addItems(items, context.judge_flag);
		if rt then
			context.get_cmd = {cmd_t};
			context.judge_flag = nil;
		end
		return rt;
	end
	return false;
end
protected_.commands_.addItem_cond = function (game_player, cmd_t, params, context)
	local cmd_type = cmd_t[2];
	if type(cmd_type) == "number" then 
		local item_sid = cmd_t[2];
		local count = cmd_t[3];
		local rt = game_player.item_bag_:canAddItems({[item_sid] = count});
		context.judge_flag = rt;
		return rt;
	elseif type(cmd_type) == "table" then 
		local items = cmd_t[2];
		local rt = game_player.item_bag_:canAddItems(items);
		context.judge_flag = rt;
		return rt;
	end
	return false;
end
protected_.commands_.addItem_undo = function (game_player, cmd_t, params, context)
	local cmd_type = cmd_t[2];
	if type(cmd_type) == "number" then 
		local item_sid = cmd_t[2];
		local count = cmd_t[3];
		return game_player.item_bag_:removeItemsBySid({[item_sid] = count});
	elseif type(cmd_type) == "table" then 
		local items = cmd_t[2];
		return game_player.item_bag_:removeItemsBySid(items);
	end
	return false;
end
--delItem
protected_.commands_.delItem = function (game_player, cmd_t, params, context)
	local cmd_type = cmd_t[2];
	if type(cmd_type) == "number" then 
		local item_sid = cmd_t[2];
		local count = cmd_t[3];
		local rt = game_player.item_bag_:removeItemsBySid({[item_sid] = count}, context.judge_flag);
		context.judge_flag = nil;
		return rt;
	elseif type(cmd_type) == "table" then 
		local items = cmd_t[2];
		local rt = game_player.item_bag_:removeItemsBySid(items, context.judge_flag);
		context.judge_flag = nil;
		return rt;
	end
	return false;
end
protected_.commands_.delItem_cond = function (game_player, cmd_t, params, context)
	local cmd_type = cmd_t[2];
	if type(cmd_type) == "number" then 
		local item_sid = cmd_t[2];
		local count = cmd_t[3];
		local rt = game_player.item_bag_:canRemoveItemsBySid({[item_sid] = count});
		context.judge_flag = rt;
		return rt;
	elseif type(cmd_type) == "table" then 
		local items = cmd_t[2];
		local rt = game_player.item_bag_:canRemoveItemsBySid(items);
		context.judge_flag = rt;
		return rt;
	end
	return false;
end
protected_.commands_.delItem_undo = function (game_player, cmd_t, params, context)
	local cmd_type = cmd_t[2];
	if type(cmd_type) == "number" then 
		local item_sid = cmd_t[2];
		local count = cmd_t[3];
		local rt = game_player.item_bag_:canAddItems({[item_sid] = count});
		if rt then
			game_player.item_bag_:addItems({[item_sid] = count}, true);
			return true;
		end
	elseif type(cmd_type) == "table" then 
		local items = cmd_t[2];
		local rt = game_player.item_bag_:canAddItems(items);
		if rt then
			game_player.item_bag_:addItems(items, true);
			return true;
		end
	end
	return false;
end
--randomRewards
protected_.commands_.randomRewards = function (game_player, cmd_t, params, context)
	--随机获得奖励
	local drop_wegiht = cmd_t[2];
	if type(drop_wegiht) ~= "table" then
		return false;
	end
	if context.random_data then
		--执行随机产生的随机内容
		local rewards_cmd = context.random_data[2];
		local contexts = context.contexts or {};
		protected_.doCommands(game_player, rewards_cmd, params, contexts);
		protected_.commands_.mergeContext(context, contexts);
		context.random_data = nil;
	else
		log.error("protected_.commands_.randomRewards", game_player, "no execute checkCommands()!!!");
		gameAssert(false, debug.traceback());
		return false;
	end
	return true;
end
protected_.commands_.randomRewards_cond = function (game_player, cmd_t, params, context)
	--随机获得奖励
	local drop_wegiht = cmd_t[2];
	if type(drop_wegiht) ~= "table" then
		return false;
	end
	local random_data = protected_.random_weight(drop_wegiht); 
	local rewards_cmd = random_data[2];
	local contexts = {};
	local rt = protected_.checkCommands(game_player, rewards_cmd, params, contexts);
	if rt then
		context.random_data = random_data;
	end
	context.contexts = contexts;
	return rt;
end
protected_.commands_.randomRewards_undo = function (game_player, cmd_t, params, context)
	--随机获得奖励
	local drop_wegiht = cmd_t[2];
	if type(drop_wegiht) ~= "table" then
		return false;
	end
	if context.random_data then
		--执行随机产生的随机内容
		local rewards_cmd = context.random_data[2];
		protected_.undoCommands(game_player, rewards_cmd, params, contexts);
		context.random_data = nil;
	else
		log.error("protected_.commands_.randomRewards", game_player, "no execute checkCommands()!!!");
		gameAssert(false, debug.traceback());
		return false;
	end
	return true;
end
--addHero 添加英雄体验卡
protected_.commands_.addHero = function (game_player, cmd_t, params, context)
	local hero_id = cmd_t[2];
	local time = cmd_t[3];	-- <0 永久  单位秒
	if not configs_.UnitCfg[hero_id] and not protected_.isHero(hero_id) then
		return false;
	end

	local hero_type = Enum.EHeroUseType_TimeLimit;
	if time < 0 then
		hero_type = Enum.EHeroUseType_Permanent;
	end

	local hero_bag = game_player.hero_bag_;
	local hero = hero_bag[hero_id];
	if hero and hero.hero_type_ == Enum.EHeroUseType_Permanent then
		--有永久的
		local reward_cmd;
		local count = 1;
		if hero_type == Enum.EHeroUseType_Permanent then
			reward_cmd = configs_.repeat_hero[-1];
		elseif hero_type == Enum.EHeroUseType_TimeLimit then
			local day = math.floor(time / (24 * 60 * 60));
			reward_cmd = configs_.repeat_hero[1];
			count = day;
		end
		local contexts = {};
		protected_.doCommands(game_player, reward_cmd, {["times"] = count}, contexts);
		protected_.commands_.mergeContext(context, contexts);
	else
		hero_bag:addHero(hero_id, hero_type, time*1000);
		context.get_cmd = {cmd_t};
	end
	return true;
end
protected_.commands_.addHero_cond = function (game_player, cmd_t, params, context)
	local hero_id = cmd_t[2];
	local time = cmd_t[3];	-- -1永久  单位秒

	if not configs_.UnitCfg[hero_id] and not protected_.isHero(hero_id) then
		return false;
	end
	return true;
end
protected_.commands_.addHero_undo = function (game_player, cmd_t, params, context)
	local hero_id = cmd_t[2];
	local time = cmd_t[3];	-- -1永久  单位秒
	if not configs_.UnitCfg[hero_id] and not protected_.isHero(hero_id) then
		return false;
	end
	local hero_type = Enum.EHeroUseType_TimeLimit;
	if time < 0 then
		hero_type = Enum.EHeroUseType_Permanent;
	end
	local hero_bag = game_player.hero_bag_;
	local hero = hero_bag[hero_id];
	return hero_bag:removeHero(hero_id, hero_type, time);
end
--使用各种n倍卡  {{"addGainRate", "money_", 2, Enum.ERewardRate_Time, 2 * 60 * 60}}
protected_.commands_.addGainRate = function (game_player, cmd_t, params, context)
	local prop_name = cmd_t[2];
	local rate_val = cmd_t[3];
	local rate_type = cmd_t[4];
	local val = cmd_t[5];

	game_player.gain_rate_sys_:addRewardRate(prop_name, rate_type, rate_val, val);
	return true;
end
protected_.commands_.addGainRate_cond = function (game_player, cmd_t, params, context)
	local prop_name = cmd_t[2];
	local rate_val = cmd_t[3];
	local rate_type = cmd_t[4];
	local val = cmd_t[5];

	if type(rate_val) ~= "number" or type(rate_type) ~= "number" or type(val) ~= "number" then
		return false;
	end
	return game_player.gain_rate_sys_:canAddRewardRate(prop_name, rate_type, rate_val, val);
end
protected_.commands_.addGainRate_undo = function (game_player, cmd_t, params, context)

end


protected_.commands_.getArgument = function(params, key)
	if params==nil or type(params) ~= "table" then
		return nil;
	end
    return params[key];
end
--contexts 并入 context中  commands_内使用
protected_.commands_.mergeContext = function(context, contexts)
	if not context.get_cmd then
		context.get_cmd = {};
	end
	for k, v in ipairs(contexts) do
		for kk, cmd in ipairs(v.get_cmd) do
			table.insert( context.get_cmd, cmd );
		end
	end
end
--整理 contexts  commands_外使用
protected_.commands_.tidyContexts = function(contexts)
	if type(contexts) ~= "table" then
		return ;
	end
	local get_cmd = contexts.get_cmd or {};
	for _, v in ipairs(contexts) do
		if v.get_cmd then
			for kk, cmd in ipairs(v.get_cmd) do
				-- table.insert(get_cmd, vv);
				if #cmd == 2 then
					if type(cmd[2]) == "number" then	--{"addMoney", 50}
						get_cmd[cmd[1]] = (get_cmd[cmd[1]] or 0) + cmd[2];
					elseif type(cmd[2]) == "table" then	-- {"addItem", {[item_sid] = 1, ...}}
						if not get_cmd[cmd[1]] then
							get_cmd[cmd[1]] = {};
						end
						for kkk, vvv in pairs(cmd[2]) do
							get_cmd[cmd[1]][kkk] = (get_cmd[cmd[1]][kkk] or 0) + vvv;
						end
					end
				elseif #cmd == 3 then	--{"addItem", item_sid, count}
					if not get_cmd[cmd[1]] then
						get_cmd[cmd[1]] = {};
					end
					get_cmd[cmd[1]][cmd[2]] = (get_cmd[cmd[1]][cmd[2]] or 0) + cmd[3];
				end
			end
		end
	end
	contexts.get_cmd = get_cmd;
end
--将上面产生的get_cmd 指令 恢复到 标准cmd指令
protected_.commands_.recoverCmd = function(rt_context)
	if type(rt_context) ~= "table" then
		return ;
	end
	local rt_cmd = {};
	for k, v in pairs(rt_context) do
		table.insert(rt_cmd, {k, v});
	end
	return rt_cmd;
end

protected_.commands_.test = function(game_player, times)
	local contexts = {};
	-- local test_cmd = {
	-- 	{"randomRewards", {
	-- 		{5, { {"addMoney", 1},{"addMoney", 2},{"addItem", 1, 1} }},
	-- 		{5, { {"addItem", 2, 1} }}   
	-- 	} }
	-- }; -- , {5, {{"addItem", 1, 1}}}
	
	local test_cmd = {{"addItem", 1, 1} };

	for i = 1, times do
		local rt = protected_.checkCommands(game_player, test_cmd, {}, contexts);
		-- print("--------------", rt, i);
		if not rt then
			print(" ---------------- end ", i);
			break;
		end
		protected_.doCommands(game_player, test_cmd, {}, contexts);
		protected_.commands_.tidyContexts(contexts);
		-- printObject(contexts)
	end
	-- printObject(contexts);
end
