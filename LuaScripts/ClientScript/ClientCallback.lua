
--初始化脚本 initClientWork
protected_.initScript = function ()
    print(protected_.client_net_);
    if protected_.client_net_ ~= 1 then
	    protected_.NetWork:initNetWork();
    end
end

--环境加载完毕，启动脚本
protected_.startScript = function ()
	print("initClientWork ...");
	client_recall_("onStart");
--    for k, v in pairs(protected_.skill_table_army) do
--        protected_.skill_table[k] = v;
--    end
--    for k, v in pairs(protected_.skill_state_data_army) do
--        protected_.skill_state_data[k] = v;
--    end
--	--GameTaskRuner.doTest();
--	-- doConsoleCommand("fight_tile")
end

protected_.console_connect_type = 1; --1 login 2 connect

protected_.cur_client_id = 1;

protected_.gen_client_id = function ()
    
    local ret = protected_.cur_client_id;
    protected_.cur_client_id = protected_.cur_client_id+1;
    print("~~~~~~~~~~~~~~~~");
    return ret;
end

protected_.updateScript = function ()
    local cur_time = getTime();
    if protected_.client_net_ ~= 1 then
	    protected_.NetWork:update_run();
    end
    -- 其他tick处理
	for k,v in pairs(protected_.fightUpdaters_) do
		if v.fight_ then
			if v.fight_.fight_worker_==nil or v.fight_.fight_worker_.client_begin_ then
				protected_.MetaFight.doFightUpdate(v, cur_time);
			end
		end
	end
	if client_define_ then
		protected_.updateDirtys();
	end
end

protected_.closeScript = function ()
	protected_.NetWork:closeNetWork();
end

protected_.handleDirtys = function( obj )
	if obj.dirtys_==nil then
		return;
	end
	if obj.dirtys_.marks_==nil then
		return;
	end
	local mt = getmetatable(obj);
	for k,v in pairs(obj.dirtys_.marks_) do
		if v==7 then
			if mt and mt.dirty_recall then
				mt:dirty_recall(obj, k, obj.old_[k]);--(object, key, old_val)--obj[k] cur_val ; old_val
			end
		else
			if obj[k]==nil then
				print("protected_.handleDirtys error: ", k, v);
			else
				protected_.handleDirtys(obj[k]);
			end
		end
	end
end

protected_.updateDirtys = function ()
	if protected_.cur_client_==nil then
		return;
	end
	protected_.handleDirtys(protected_.cur_client_, {});
	protected_.clearDirty(protected_.cur_client_);
end
protected_.destroyClient = function(client_id)
	local client = protected_.clients_[client_id];
	if client then
		if client.player_id_ then
			protected_.playerClients_[client.player_id_] = nil;
		end
		protected_.clients_[client_id]=nil;
	end
	protected_.cur_client_id_ = nil;
	protected_.cur_client_ = nil;
end
protected_.on_connect = function(client_id)
	print("protected_.on_connect", client_id, protected_.clients_[client_id]);
    gameLog("a new connect is id: " .. client_id);
	doLuaCommand("adjustTime", client_id);
    if protected_.clients_[client_id] == nil then
        -- 发送登录协议
        protected_.clients_[client_id] = {};
        protected_.clients_[client_id].conn_state_ = 1;
        protected_.clients_[client_id].client_id_ = client_id;
		if protected_.cur_client_id_== nil or protected_.console_connect_type == 1 then
			protected_.cur_client_id_ = client_id;
			protected_.cur_client_ = protected_.clients_[client_id];
		end
		client_recall_("onConnect", client_id);
        return;
    elseif protected_.clients_[client_id].player_ then
        local client = protected_.clients_[client_id];
        client.conn_state_ = 1;
        client.client_state_ = 3; --reconnect
        local smsg = {EC2SProtocol["EC2SProtocol_ReConnect"], client.player_id_, client.reconnect_key_};
        sendMessage(client_id, smsg);
		client_recall_("onReconnect", client_id);
		return ;
    end

--	protected_.clients_[client_id].client_id_ = client_id;
--    protected_.clients_[client_id].account_name_ = "test-" .. (client_id);
--	protected_.clients_[client_id].conn_state_ = 1;
--    protected_.clients_[client_id].client_state_ = 1; -- logining..
    --proto_id, serverid, acc_server_id, account, pwd
--    local smsg = {EC2SProtocol["EC2SProtocol_Login"], protected_.clients_[client_id].account_name_, "1"};
--   sendMessage(client_id, smsg);
end

protected_.on_disconnect = function(client_id)
    gameLog("diconnect the id: " .. client_id);
	client_recall_("onDisconnect", client_id);
	-- 断线重连以后考虑
	local client = protected_.clients_[client_id];
	if client then
		if client.player_id_ then
			protected_.playerClients_[client.player_id_] = nil;
		end
		protected_.clients_[client_id]=nil;
	end
	if protected_.cur_client_id_==client_id then
		protected_.cur_client_id_ = nil;
		protected_.cur_client_ = nil;
		for c_id, cc in pairs(protected_.clients_) do
			if type(c_id)=="number" and cc.conn_state_==1 then
				protected_.cur_client_id_ = c_id;
				protected_.cur_client_ = protected_.clients_[c_id];
				break;
			end
		end
	end
end

protected_.on_login_ = function (client_id, account, pwd)
    print(pwd);
    local client = protected_.clients_[client_id];
    if client == nil or client.conn_state_ ~= 1 or client.client_state_ ~= nil then
        gameAssert(false, "login error state");
    end
    protected_.clients_[client_id].account_name_ = account;
    protected_.clients_[client_id].client_state_ = 1; -- logining..
    local smsg = {EC2SProtocol["EC2SProtocol_Login"], account, pwd};
    sendMessage(client_id, smsg);
end

protected_.change_state_to_createchar = function(client_id)
    local client = protected_.clients_[client_id];
    if client == nil or client.conn_state_ ~= 1 or client.client_state_ ~= 1 then
        gameAssert(false, "error state to change to createchar");
    end
    protected_.clients_[client_id].client_state_ = 2; -- create char state..
    --进入创建角色状态，
    print("please input the No."..client_id.." char name:");
end

protected_.on_create_char_ = function(client_id, charname)
    local client = protected_.clients_[client_id];
    if client == nil or client.conn_state_ ~= 1 or client.client_state_ ~= 2 then
         gameAssert(false, "error state to createchar");
    end
    local smsg = {EC2SProtocol["EC2SProtocol_CreateChar"], charname};--"alliance_"..client_id+1000
	sendMessage(client_id, smsg);
end

protected_.show_hero = function(hero_id)
    if configs_.Hero == nil or configs_.Hero[hero_id] == nil then
        return;
    end
    for k, v in pairs(configs_.UnitCfg[hero_id]) do
		print(k);
		print(dump_obj(v));
    end
end

protected_.get_area = function(area_id, slot_id)
	if area_id==0 then
		return {};
	end
	local area_data = configs_.SkillAreaCfg[area_id];
	if area_data==nil then
		gameLog("Skill area "..area_id.." not exist!!!");
		return {};
	end
	local area_type = area_data[1];
	if Enum.EAreaMode_RelativeRow==area_type then
		slot_id = math.floor(slot_id/10)*10;
		area_type = Enum.EAreaMode_RelativePos;
	elseif Enum.EAreaMode_RelativeCol==area_type then
		slot_id = slot_id%10;
		area_type = Enum.EAreaMode_RelativePos;
	end

	if Enum.EAreaMode_RelativePos==area_type then
		local r_slots = {};
		for i, v in ipairs(area_data[2]) do
			local pv = slot_id + v;
			if pv>0 then
				local mm = pv%10;
				if mm>0 and mm<=Enum.ESlotMax_Col then
					table.insert(r_slots, pv);
				end
			end
		end
		return r_slots;
	elseif Enum.EAreaMode_AbsolutePos==area_type then
		return area_data[2];
	end
	return {};
end
--获取图鉴的顺序  参数：type  0 全部，1 已拥有，2 未拥有  is_pet 是否是pet
protected_.getUnitSort = function(client_id, have_type, is_pet)
	local units = {};
	protected_.cur_sort_client_id_ = client_id;
	local client = protected_.clients_[client_id];
	local player = client.player_;
	local bag = player.hero_bag_;
	local comp_func = protected_.heroComp;
	if is_pet then
		bag = player.pet_bag_.pet_types_;
		comp_func = protected_.petComp;
	end
	for k, v in pairs(bag.map_) do
		print("----------", k, v);
	end
	if have_type == 0 then
		local unit_ids = protected_.getAllUnit(configs_.UnitCfg, is_pet);
		table.sort(unit_ids, comp_func);
		units = unit_ids;

	elseif have_type == 1 then
		local unit_ids = protected_.getAllUnit(bag.map_, is_pet);
		table.sort(unit_ids, comp_func);
		units = unit_ids;
	elseif have_type == 2 then
		local unit_ids = protected_.getNoUnit(bag, is_pet);
		table.sort(unit_ids, comp_func);
		units = unit_ids;
	end
	protected_.cur_sort_client_id_ = nil;
	return units;
end
protected_.heroComp = function(id1, id2)
	local hero1 = configs_.UnitCfg[id1];
	local hero2 = configs_.UnitCfg[id2];
	local client = protected_.clients_[protected_.cur_sort_client_id_];
	local player = client.player_;
	local hero_bag = player.hero_bag_;
	--排序规则， 拥有在前
	if protected_.isCanUse(hero_bag, id1) then
		if protected_.isCanUse(hero_bag, id2) then
			return id1 < id2;
		else
			return true;
		end
	end
	if protected_.isCanUse(hero_bag, id2) then
		if protected_.isCanUse(hero_bag, id1) then
			return id1 < id2;
		else
			return false;
		end
	end
	return id1 < id2;
end
protected_.petComp = function(id1, id2)
	local pet1 = configs_.UnitCfg[id1];
	local pet2 = configs_.UnitCfg[id2];
	local client = protected_.clients_[protected_.cur_sort_client_id_];
	local player = client.player_;
	local pet_types = player.pet_bag_.pet_types_;
	--排序规则， 拥有在前
	if protected_.isCanUse(pet_types, id1, true) then
		if protected_.isCanUse(pet_types, id2, true) then
			return id1 < id2;
		else
			return true;
		end
	end
	if protected_.isCanUse(pet_types, id2, true) then
		if protected_.isCanUse(pet_types, id1, true) then
			return id1 < id2;
		else
			return false;
		end
	end
	return id1 < id2;
end
protected_.getAllUnit = function(t, is_pet)
	local unit_ids = {};
	local unit_type_func = protected_.isHero;
	if is_pet then
		unit_type_func = protected_.isPet;
	end
	for k, v in pairs(t) do
		local unit_data = configs_.UnitCfg[k];
		if unit_data and unit_type_func(k) and unit_data.is_show_ == 1 then
			table.insert(unit_ids, k);
		end
	end
	return unit_ids;
end
protected_.getNoUnit = function(bag, is_pet)
	local unit_ids = {};
	local unit_type_func = protected_.isHero;
	if is_pet then
		unit_type_func = protected_.isPet;
	end
	for k, v in pairs(configs_.UnitCfg) do
		if not protected_.isCanUse(bag, k, is_pet) and unit_type_func(k) and v.is_show_ == 1 then
			table.insert(unit_ids, k);
		end
	end
	return unit_ids;
end
protected_.isCanUse = function(t, unit_id, is_pet)
	if is_pet then
		if t[unit_id] then
			return true;
		else
			return false;
		end
	end
	local hero = t[unit_id];
	if hero and hero.hero_type_  == Enum.EHeroUseType_Permanent then
		return true;
	end
	if hero and hero.hero_type_ == Enum.EHeroUseType_TimeLimit and hero.hero_expire_ > getTime() then
		return true;
	end
	return false;
end

local score_config = configs_.ai.score_config;
local hp_scope = configs_.ai.hp_scope;
local hp_scope_score = configs_.ai.hp_scope_score;

--ai 选择技能释放
protected_.chooseAISkill = function(unit_id, fight_worker)
    local unit = fight_worker[unit_id];
    if not unit then
        return ;
    end
    local unit_data = configs_.UnitCfg[unit.unit_sid_];
    if not unit_data then
        return ;
    end
    local skill_prior = protected_.getSkillPrior(unit, fight_worker);
    local prior_skills = skill_prior[skill_prior.max];
    if #prior_skills <= 0 then
        return ;
	end
	-- printObject(skill_prior);
	print(" ======= >", #prior_skills)
	-- printObject(prior_skills)
    local skill_raw_id = prior_skills[math.random( #prior_skills )][1];
	local slot_id = unit.skills_[skill_raw_id].slot;
	print(" <<<<<<<<<<<<<<<<<<<<chooseAISkill>>>>>>>>>>>>>>>>>>>>>>>>", unit.unit_sid_, slot_id);
    return slot_id;
end
--ai选择释放的目标
protected_.chooseAISkillTarget = function(unit_id, fight_worker, slot_id)
    local unit = fight_worker[unit_id];
    if not unit then
        return ;
    end
    local skill_raw_id = unit.skill_lists_[slot_id];
    if skill_raw_id <= 0 then
        return ;
    end
	--技能分为， 攻击性、控制性、恢复性
	local choose_result = protected_.chooseSkillTarget(unit, skill_raw_id, fight_worker);

	--printObject(choose_result)
	return choose_result;
end
--对技能选择目标
protected_.chooseSkillTarget = function(unit, skill_raw_id, fight_worker)
	local skill_data = configs_.SkillCfg[skill_raw_id];
	local choose_result = {};
	if not skill_data then
		return choose_result;
	end
	--确定的目标不需要选择了
	if skill_data.target_type == Enum.ETargetType_Self or skill_data.target_type == Enum.ETargetType_FrontUnit or skill_data.target_type == Enum.ETargetType_ALL then
		return choose_result;
	end
	local corp_id = unit.corp_id_;
	if skill_data.aggression_value > 0 then
		if corp_id == 1 then
			corp_id = 0;
		elseif corp_id == 0 then
			corp_id = 1;
		end
	end
	choose_result.corp_id = corp_id;
	local corp = fight_worker.corps_[corp_id];
	local enemy_num = protected_.getTableLen(corp);
	if skill_data.target_type == Enum.ETargetType_Unit then
		local max_score = -1;
		local max_score_unit_id = 0;
		for k, v in pairs(corp) do
			local score = protected_.getDesUnitScore(fight_worker, unit, v, skill_data);
			if score > max_score then
				max_score = score;
				max_score_unit_id = v.unit_id_;
			end
		end
		choose_result.target_id = max_score_unit_id;
		choose_result.target_slot_id = fight_worker[max_score_unit_id].slot_id_;
	end
	if skill_data.target_type == Enum.ETargetType_Slot then
		local area_id = skill_data.damge_area;
		local area_data = configs_.SkillAreaCfg[area_id];
		if area_data then
			--目标是区域
			if area_data[1] == Enum.EAreaMode_RelativeRow then
				--行
				local max_score = -1;
				local max_score_slot_id = 0;
				for i = 0, 3 do
					local slot_id = i * 10 + 1;
					local score, total_des = protected_.getAreaScore(unit, fight_worker, skill_data, corp_id, slot_id);
					if score > max_score then
						max_score = score;
						max_score_slot_id = slot_id;
					end
					if total_des >= enemy_num then
						break;
					end
				end
				if max_score_slot_id > 0 then
					choose_result.target_slot_id = max_score_slot_id;
				end
			elseif area_data[1] == Enum.EAreaMode_RelativeCol then
				--列
				local max_score = -1;
				local max_score_slot_id = 0;
				for i = 1, 5 do
					local score, total_des = protected_.getAreaScore(unit, fight_worker, skill_data, corp_id, i);
					if score > max_score then
						max_score = score;
						max_score_slot_id = i;
					end
					if total_des >= enemy_num then
						break;
					end
				end
				if max_score_slot_id > 0 then
					choose_result.target_slot_id = max_score_slot_id;
				end
			else
				local max_score = -1;
				local max_score_slot_id = 0;
				for i = 0, 3 do
					for j = 1, 5 do
						local slot_id = j + i*10;
						local score, total_des = protected_.getAreaScore(unit, fight_worker, skill_data, corp_id, slot_id);
						if score > max_score then
							max_score = score;
							max_score_slot_id = slot_id;
						end
						if total_des >= enemy_num then
							break;
						end
					end
				end
				if max_score_slot_id > 0 then
					choose_result.target_slot_id = max_score_slot_id;
				end
			end
		else
			--目标是地格
			local max_score = -1;
			local max_score_slot_id = 0;
			for i = 0, 3 do
				for j = 1, 5 do
					local slot_id = j + i*10;
					local score = protected_.getDesSlotScore(fight_worker, unit, corp_id, slot_id, skill_data);
					if score > max_score then
						max_score = score;
						max_score_slot_id = slot_id;
					end
				end
			end
			if max_score_slot_id > 0 then
				choose_result.target_slot_id = max_score_slot_id;
			end
		end
	end
	return choose_result;
end
protected_.getAreaScore = function(unit, fight_worker, skill_data, corp_id, slot_id)
	local area_id = skill_data.damge_area;
	local r_slots = protected_.get_area(area_id, slot_id);
	local corp = fight_worker.corps_[corp_id];
	local score = 0;
	local total_des = 0;
	for _, r_slot_id in pairs(r_slots) do
		local des_unit = corp[r_slot_id];
		if des_unit then
			local r_score = protected_.getDesUnitScore(fight_worker, unit, des_unit, skill_data);
			score = score + r_score;
			total_des = total_des + 1;
		end
	end
	print(" 				<<<<<getAreaScore>>>> ", corp_id, slot_id, score, total_des);
	return score, total_des;
end

--目标加分
protected_.getDesUnitScore = function(fight_worker, src_unit, des_unit, skill_data, context)
	--基本规则加分
	local score = protected_.getBaseScore(src_unit, des_unit, skill_data);
	if score < 0 then
		score = 0;
	end
	--满足条件规则加分
	for _, v in ipairs(protected_.dealCondition(skill_data.condition_score)) do
		local params = {};
		params.src_unit = src_unit;
		params.des_corp_id = des_unit.corp_id_;
		params.des_slot_id = des_unit.slot_id_;
		params.des_unit = des_unit;
		params.skill_data = skill_data;
		params.skill_slot_id = des_unit.slot_id_;
		if context then
			params.skill_slot_id = context.skill_slot_id;
			params.trigger = context.trigger;
		end
		score = score + v[1](fight_worker, v[2], params);
		-- print("  ++++++++++++getDesUnitScore------ ", v[2][1], v[1](des_unit, fight_worker, v[2], params), score);
	end
	-- print(" ...++++++addScore+++++ ", des_unit.unit_sid_, des_unit.unit_id_, des_unit.slot_id_,  score);
	return score;
end

protected_.getDesSlotScore = function(fight_worker, src_unit, des_corp_id, des_slot_id, skill_data)
	--满足条件规则加分
	local score = 0;
	local corp = fight_worker.corps_[des_corp_id];
	local des_unit = corp[des_slot_id];
	for _, v in ipairs(protected_.dealCondition(skill_data.condition_score)) do
		local params = {};
		params.src_unit = src_unit;
		params.des_corp_id = des_corp_id;
		params.des_slot_id = des_slot_id;
		params.skill_data = skill_data;
		params.skill_slot_id = des_slot_id;
		params.des_unit = des_unit;
		score = score + v[1](fight_worker, v[2], params);
	end
	print(" ...++++++getDesSlotScore+++++ ", des_corp_id, des_slot_id,  score);
	return score;
end
--获取基本加分规则的得分
protected_.getBaseScore = function(src_unit, des_unit, skill_data)
	local score = 0;
	local damage_type = skill_data.damage_type;
	local diff_hp = des_unit.hp_max_ - des_unit.hp_;
	if diff_hp < 0 then
		diff_hp = 0;
	end
	local hp_rate = des_unit.hp_ / des_unit.hp_max_;
	local lv_scope = lowerBound(hp_scope, hp_rate * 100);
	local hp_rate_score = hp_scope_score[lv_scope] or 0;
	if damage_type then
		if damage_type >= Enum.EDamageType_Any and  damage_type <= Enum.EDamageType_Magic then
			if des_unit.hp_ > 0  then
				score = score + hp_rate_score * diff_hp;
			end
		end
		if damage_type == Enum.EDamageType_Heal then
			score = score + diff_hp * hp_rate_score;
		end
		if des_unit.hp_ > 0 and damage_type ~= Enum.EDamageType_Any then
			local atk_name = protected_.getUnitPropName("attack_", "EDamageType", damage_type);
			local def_name = protected_.getUnitPropName("defence_", "EDamageType", damage_type);
			score = score + (src_unit[atk_name] - des_unit[def_name]);
		end
	else
		if skill_data.aggression_value == 2 then --造成伤害
			local atk_name = protected_.getUnitPropName("attack_", "EDamageType", Enum.EDamageType_Physical);
			local def_name = protected_.getUnitPropName("defence_", "EDamageType", Enum.EDamageType_Physical);
			score = score + (src_unit[atk_name] - des_unit[def_name]) * 0.5;
			atk_name = protected_.getUnitPropName("attack_", "EDamageType", Enum.EDamageType_Magic);
			def_name = protected_.getUnitPropName("defence_", "EDamageType", Enum.EDamageType_Magic);
			score = score + (src_unit[atk_name] - des_unit[def_name]) * 0.5;

			if des_unit.hp_ > 0 then
				score = score + hp_rate_score * diff_hp;
			end
		end
		if skill_data.aggression_value == -2 then --恢复血量
			score = score + diff_hp * hp_rate_score;
		end
		if skill_data.aggression_value == 1 then --有害
			local p_atk_name = protected_.getUnitPropName("attack_", "EDamageType", Enum.EDamageType_Physical);
			local m_atk_name = protected_.getUnitPropName("attack_", "EDamageType", Enum.EDamageType_Magic);
			if des_unit[p_atk_name] > des_unit[m_atk_name] then
				score = score + des_unit[p_atk_name]
			else
				score = score + des_unit[m_atk_name]
			end
		end
		if skill_data.aggression_value == -1 then --有益
			local p_atk_name = protected_.getUnitPropName("attack_", "EDamageType", Enum.EDamageType_Physical);
			local m_atk_name = protected_.getUnitPropName("attack_", "EDamageType", Enum.EDamageType_Magic);
			if des_unit[p_atk_name] > des_unit[m_atk_name] then
				score = score + des_unit[p_atk_name]
			else
				score = score + des_unit[m_atk_name]
			end
		end
	end
	return score;
end
--获取一单位的血量，或相差一单位属性的分数
protected_.getScore = function(damage_type)
	return score_config[damage_type] or score_config.default;
end
protected_.setTempCondition = function(fight_worker, corp_id, cond, val)
	if not fight_worker.temp_cond then
		fight_worker.temp_cond = {};
	end
	if not fight_worker.temp_cond[corp_id] then
		fight_worker.temp_cond[corp_id] = {};
	end
	fight_worker.temp_cond[corp_id][cond] = val;
end
protected_.clearTempCondition = function(fight_worker)
	fight_worker.temp_cond = nil;
end
protected_.getTempConditionData = function(fight_worker, corp_id, cond)
	if not fight_worker.temp_cond or not fight_worker.temp_cond[corp_id] then
		return nil;
	end
	return fight_worker.temp_cond[cond];
end
protected_.dealCondition = function(conditions)
	local funcs = {};
	if type(conditions) == "string" then
		local conditions_t = protected_.stringSplit(conditions, ",");
		if conditions_t and next(conditions_t) then
			for _, condition in pairs(conditions_t) do
				local condition_t = protected_.stringSplit(protected_.trim(condition), " ");
				-- print(" =====condition_t==== > ", condition_t[1]);
				local func =  protected_.conditionResult[condition_t[1]];
				if func then
					table.insert( funcs, {func, condition_t} );
				end
			end
		end
	end
	return funcs;
end
--获取战斗单位技能的优先级
--版本1 怒气技优先级最高，其他主动技能随机
protected_.getSkillPrior = function(unit, fight_worker)
    local unit_data = configs_.UnitCfg[unit.unit_sid_];
    if not unit_data then
        return ;
    end
    local skill_prior = {};
    for i = Enum.ESkill_Default, Enum.ESkill_MAX do
        if unit.skill_lists_[i] ~= 0 and protected_.checkSkillUseCondition(unit, unit.skill_lists_[i]) then
			local skill_prior_lv = protected_.getSkillPriorLv(unit, unit.skill_lists_[i], fight_worker);
			if i == Enum.ESkill_EP then
				skill_prior_lv = skill_prior_lv + 10;
			end
            if not skill_prior[skill_prior_lv] then
                skill_prior[skill_prior_lv] = {};
            end
            table.insert( skill_prior[skill_prior_lv], {unit.skill_lists_[i], i });
            if skill_prior_lv > (skill_prior.max or 0) then
                skill_prior.max = skill_prior_lv;
            end
        end
	end
    return skill_prior;
end
--获取技能的优先等级
protected_.getSkillPriorLv = function(unit, skill_raw_id, fight_worker)
	local skill_data = configs_.SkillCfg[skill_raw_id];
	local lv = skill_data.prior_lv or 1;
	for _, v in ipairs(protected_.dealCondition(skill_data.condition_prior)) do
		local params = {};
		params.src_unit = unit;
		lv = lv + v[1](fight_worker, v[2], params);
	end
	return lv;
end
--检查技能是否可以释放 解锁状态 cd 和 怒气
protected_.checkSkillUseCondition = function(unit, skill_raw_id)
	local slot_id = unit.skills_[skill_raw_id].slot;
	local prop_name = protected_.getUnitPropName("skill_cd_", "ESkill", slot_id);
	local prop_index = PropDesc[prop_name];
	if unit.base_[prop_index] > 0 then
		print(" --checkSkillUseCondition---cd---- ", skill_raw_id, slot_id, prop_name);
        return false;
    end
	if unit.base_[prop_index] > 0 then
		print(" --checkSkillUseCondition---lock---- ", skill_raw_id, slot_id, prop_name);
        return false;
    end
    local skill_data = configs_.SkillCfg[skill_raw_id];
    if not skill_data then
        return false;
    end
	if skill_data.angry_condition > unit.angry_ then
		print(" --checkSkillUseCondition---angry---- ", skill_raw_id, slot_id, skill_data.angry_condition, unit.angry_);
        return false;
    end
    return true;
end
protected_.getUnitPropName = function(prefix, enum_name, i)
    return prefix .. "[" .. EnumDesc[enum_name][i] .. "]";
end
--配置技能满足条件加分  条件满足 返回 配置分数，不满足 返回0
protected_.conditionResult = {
	--目标血量最低
	["hp_min"] = function(fight_worker, cond_data, params)
		local des_unit = params.des_unit;
		if not des_unit or des_unit.hp_ <= 0 then
			return 0;
		end
		local score = tonumber(cond_data[2]);
		local temp_val = protected_.getTempConditionData(fight_worker, des_unit.corp_id_, cond_data[1]);
		if temp_val then
			if temp_val ~= des_unit.unit_id_ then
				score = 0;
			end
		else
			local corp = fight_worker.corps_[des_unit.corp_id_];
			local min_hp = des_unit.hp_;
			local min_unit_id = des_unit.unit_id_;
			for k, v in pairs(corp) do
				if v.hp_ < min_hp then
					min_hp = v.hp_;
					min_unit_id = v.unit_id_;
				end
			end
			protected_.setTempCondition(fight_worker, des_unit.corp_id_, cond_data[1], min_unit_id);
			if min_unit_id ~= des_unit.unit_id_ then
				score = 0;
			end
		end
		return score;
	end,
	--目标血量比率最低
	["hp_rate_min"] = function(fight_worker, cond_data, params)
		local des_unit = params.des_unit;
		if not des_unit or des_unit.hp_ <= 0 then
			return 0;
		end
		local score = tonumber(cond_data[2]);
		local temp_val = protected_.getTempConditionData(fight_worker, des_unit.corp_id_, cond_data[1]);
		if temp_val then
			if temp_val ~= des_unit.unit_id_ then
				score = 0;
			end
		else
			local corp = fight_worker.corps_[des_unit.corp_id_];
			local min_hp_rate = des_unit.hp_ / des_unit.hp_max_;
			local min_unit_id = des_unit.unit_id_;
			for k, v in pairs(corp) do
				if (v.hp_ / v.hp_max_) < min_hp_rate then
					min_hp_rate = v.hp_ / v.hp_max_;
					min_unit_id = v.unit_id_;
				end
			end
			protected_.setTempCondition(fight_worker, des_unit.corp_id_, cond_data[1], min_unit_id);
			if min_unit_id ~= des_unit.unit_id_ then
				score = 0;
			end
		end
		return score;
	end,
	--目标英雄死亡
	["dead_flag"] = function(fight_worker, cond_data, params)
		local des_unit = params.des_unit;
		if not des_unit then
			return 0;
		end
		if des_unit.hp_ <= 0 and des_unit.dead_flag and protected_.isHero(des_unit.unit_sid_) then
			return tonumber(cond_data[2]);
		end
		return 0;
	end,
	--技能释放位置和目标位置， 需要减分， 适用于 白骨精召唤骷髅
	["has_unit"] = function(fight_worker, cond_data, params)
		local des_unit = params.des_unit;
		local skill_slot_id = params.skill_slot_id;
		-- print(debug.traceback());
		-- print(" <<<<<<<<<<<<<<<<<<<< ", des_unit, des_unit.slot_id_, skill_slot_id);
		if des_unit and des_unit.slot_id_ == skill_slot_id then
			return tonumber(cond_data[2]);
		end
		return 0;
	end,
	["area"] = function(fight_worker, cond_data, params)
		local area_id = tonumber(cond_data[2]);
		local src_unit = params.src_unit;
		local des_corp_id = params.des_corp_id;
		local des_slot_id = params.des_slot_id;
		local skill_data = params.skill_data;
		local trigger = params.trigger;	--防止无限触发  区域调目标 目标再调区域
		if trigger then
			return 0;
		end
		local total_des = 0;
		if not src_unit or not des_corp_id or not des_slot_id or not skill_data then
			return 0;
		end

		local r_slots = protected_.get_area(area_id, des_slot_id);
		local corp = fight_worker.corps_[des_corp_id];
		local score = 0;
		local context = {};
		context.skill_slot_id = des_slot_id;
		context.trigger = true;
		for _, r_slot_id in pairs(r_slots) do
			local des_slot_unit = corp[r_slot_id];
			if des_slot_unit then
				local r_score = protected_.getDesUnitScore(fight_worker, src_unit, des_slot_unit, skill_data, context);
				score = score + r_score;
				-- print(" ++++++++++area++++getDesUnitScore+++++ ", r_score);
			end
		end
		-- print(" <<<<<<<<<<<<<<<<<,++++++++++area+++++++++ ", des_slot_id, score);
		return score;
	end,
	--目标不是英雄
	["not_hero"] = function(fight_worker, cond_data, params)
		local des_unit = params.des_unit;
		if des_unit and not protected_.isHero(des_unit.unit_sid_) then
			return 0;
		end
		return tonumber(cond_data[2]);
	end,




	--condition_prior
	--有英雄死亡  hero_dead_num 阵营(0/1) 死亡数（0无死亡，>=1最低死亡数） 分数
	["hero_dead_num"] = function(fight_worker, cond_data, params)
		local src_unit = params.src_unit;
		if not src_unit then
			return 0;
		end
		local target_corp_id = src_unit.corp_id_;
		local corp_arg = tonumber(cond_data[2]);
		local limit_num = tonumber(cond_data[3]);
		local score = tonumber(cond_data[4]);
		if corp_arg == 0 then
			if target_corp_id == 0 then
				target_corp_id = 1;
			else
				target_corp_id = 0;
			end
		end

		local temp_val = protected_.getTempConditionData(fight_worker, target_corp_id, cond_data[1]);
		if temp_val then
			if limit_num == 0 then
				if temp_val == 0 then
					return score;
				end
			else
				if temp_val >= limit_num then
					return score;
				end
			end
			return 0;
		end

		local target_corp = fight_worker.corps_[target_corp_id];
		local count = 0;
		for slot_id, unit in pairs(target_corp) do
			if unit.hp_ <= 0 and unit.dead_flag and protected_.isHero(unit.unit_sid_) then
				count = count + 1;
			end
		end
		protected_.setTempCondition(fight_worker, target_corp_id, cond_data[1], count);
		if limit_num == 0 then
			if count == 0 then
				return score;
			end
		else
			if count >= limit_num then
				return score;
			end
		end
		return 0;
	end,
	--有单位血量小于
	["unit_hp_lt"] = function(fight_worker, cond_data, params)
		local src_unit = params.src_unit;
		if not src_unit then
			return 0;
		end
		local target_corp_id = src_unit.corp_id_;
		local corp_arg = tonumber(cond_data[2]);
		local limit_num = tonumber(cond_data[3]);
		local score = tonumber(cond_data[4]);
		if corp_arg == 0 then
			if target_corp_id == 0 then
				target_corp_id = 1;
			else
				target_corp_id = 0;
			end
		end

		local temp_val = protected_.getTempConditionData(fight_worker, target_corp_id, cond_data[1]);
		if temp_val and temp_val <= limit_num then
			return score;
		end

		local target_corp = fight_worker.corps_[target_corp_id];
		local min_hp = 999999;
		for slot_id, unit in pairs(target_corp) do
			if unit.hp_ < min_hp then
				min_hp = unit.hp_;
			end
		end
		protected_.setTempCondition(fight_worker, target_corp_id, cond_data[1], min_hp);
		if min_hp <= limit_num then
			return score;
		end
		return 0;
	end,
	["unit_hp_rate_lt"] = function(fight_worker, cond_data, params)
		local src_unit = params.src_unit;
		if not src_unit then
			return 0;
		end
		local target_corp_id = src_unit.corp_id_;
		local corp_arg = tonumber(cond_data[2]);
		local limit_num = tonumber(cond_data[3]);
		local score = tonumber(cond_data[4]);
		if corp_arg == 0 then
			if target_corp_id == 0 then
				target_corp_id = 1;
			else
				target_corp_id = 0;
			end
		end

		local temp_val = protected_.getTempConditionData(fight_worker, target_corp_id, cond_data[1]);
		if temp_val and temp_val <= limit_num then
			return score;
		end

		local target_corp = fight_worker.corps_[target_corp_id];
		local min_rate_hp = 1;
		for slot_id, unit in pairs(target_corp) do
			if (unit.hp_ / unit.hp_max_) < min_rate_hp then
				min_rate_hp = unit.hp_;
			end
		end
		protected_.setTempCondition(fight_worker, target_corp_id, cond_data[1], min_rate_hp);
		if min_rate_hp <= limit_num then
			return score;
		end
		return 0;
	end,
	--某种类型的unit到达极限数
	["unit_type_num"] = function(fight_worker, cond_data, params)
		local src_unit = params.src_unit;
		if not src_unit then
			return 0;
		end
		local target_corp_id = src_unit.corp_id_;
		local corp_arg = tonumber(cond_data[2]);
		local limit_num = tonumber(cond_data[3]);
		local score = tonumber(cond_data[4]);
		if corp_arg == 0 then
			if target_corp_id == 0 then
				target_corp_id = 1;
			else
				target_corp_id = 0;
			end
		end
		local type_num = tonumber(cond_data[5]);
		local unit_type = {};
		local index = 6;
		for i = 1, type_num do
			unit_type[tonumber(cond_data[index])] = 1;
			index = index + 1;
		end

		local temp_val = protected_.getTempConditionData(fight_worker, target_corp_id, cond_data[1]);
		if temp_val and temp_val >= limit_num then
			return score;
		end
		local target_corp = fight_worker.corps_[target_corp_id];
		local count = 0;
		for slot_id, unit in pairs(target_corp) do
			if unit_type[unit.unit_sid_] then
				count = count + 1;
			end
		end
		if count >= limit_num then
			protected_.setTempCondition(fight_worker, target_corp_id, cond_data[1], count);
			return score;
		end
		return 0;
	end,
}