
protected_.MetaFight.EFightState_None = 0;
protected_.MetaFight.EFightState_Wait = 1;
protected_.MetaFight.EFightState_Runing = 2;
protected_.MetaFight.EFightState_End = 3;
protected_.MetaFight.no_wait_messages={
[Enum.EFightProto_RoundBegin]=1,
[Enum.EFightProto_Result]=1,
--[Enum.EFightProto_End]=1,
[Enum.EFightProto_Start]=1,
--[Enum.EFightProto_RoundPrepare]=1,
}
protected_.MetaFight.handleMessage = function(client_id, msg, index)
    local client = protected_.clients_[client_id];
    if client == nil then
        gameAssert(false, "error! ES2CProtocol_Fight");
    end
	local fight_proto = msg[index];
	index = index +1;
	local dround_tick = msg[index];
	index = index +1;
	--print("###protected_.MetaFight.handleMessage ", fight_proto, dround_tick);
	local handle_call = protected_.fightHandlers[fight_proto];
	if handle_call==nil then
		return -1;
	end
	if protected_.MetaFight.no_wait_messages[fight_proto]==1 then
		handle_call(client, msg, index);
		return 0;
	end
	local tick_node = {
		proto_ = fight_proto,
		round_tick_ = dround_tick,
		msg_ = msg,
		msg_index_ = index
	}
	if client.fight_ then
		table.insert(client.fight_.fight_worker_.tick_actions_, tick_node);
	end
	return 0;
end

protected_.MetaFight.doFightUpdate = function(client, cur_time)
	local fight_worker = client.fight_.fight_worker_;
	if fight_worker==nil or fight_worker.state_~=protected_.MetaFight.EFightState_Runing then
		return;
	end
	local tick_dog = 0;
	while fight_worker.cur_time_<cur_time do
		fight_worker.cur_time_ = fight_worker.cur_time_+protected_.tick_interval_;
		fight_worker.tick_count_ = fight_worker.tick_count_ + 1;
		fight_worker.cur_round_tick_ = fight_worker.cur_round_tick_ + 1;--当前round的tick值
		local tick_node = fight_worker.tick_actions_[1];
		while tick_node and fight_worker.cur_round_tick_>=tick_node.round_tick_ do
			table.remove(fight_worker.tick_actions_, 1);
			local handle_call = protected_.fightHandlers[tick_node.proto_];
			if handle_call then
				local r, s = handle_call( client, tick_node.msg_, tick_node.msg_index_);
				if not r then
					-- gameError("doFightUpdate error["..tick_node.proto_.."] error: "..s);
				end
				-- handle_call(client, tick_node.msg_, tick_node.msg_index_);
			end
			if client.fight_==nil or client.fight_.fight_worker_==nil then
				return;
			end
			tick_node = fight_worker.tick_actions_[1];
			tick_dog = tick_dog + 1;
			if tick_dog>10000 then
				gameError("doFightUpdate tick error: "..tick_dog..">1000!");
				break;
			end
		end
	end
end

protected_.MetaFight.createFight = function(client, fight_data, troop1_data, troop2_data)
	local fight = protected_.unserialObject(protected_.MetaFight, fight_data, client, "fight_");
	client.fight_ = fight;
	if client.troops_==nil then
		client.troops_ = {};
	end
	client.fight_.my_corp_id_ = -1;
	local troop1 = protected_.unserialObject(protected_.MetaTroop, troop1_data);
	if troop1.marks_==nil then
		troop1.marks_ = protected_.constructObject(protected_.MetaMap, nil, troop1, "marks_");
	end
    if client.troops_[troop1.id_] == nil then
        gameAssert(0, "");
        return nil;
    end
    client.troops_[troop1.id_] = nil
	client.troops_[troop1.id_] = troop1;
    if client.cur_troop_.id_ == troop1.id_ then
		client.fight_.my_corp_id_ = 0;
		--断线重连，从fightserver同步来的troop没有指挥和标记信息，在worker发来时对其设置
        client.cur_troop_ = nil;
        client.cur_troop_ = troop1;
		if troop1.marks_.map_ then
			for k, v in pairs(troop1.marks_.map_) do
				client.cur_troop_.marks_[k] = v;
			end
		end
	end
	local troop2 = protected_.unserialObject(protected_.MetaTroop, troop2_data);
	if troop2.marks_==nil then
		troop2.marks_ = protected_.constructObject(protected_.MetaMap, nil, troop2, "marks_");
	end
	if client.troops_[troop2.id_]==nil then
        gameAssert(0, "");
        return nil;
    end

    client.troops_[troop2.id_] = nil;
    client.troops_[troop2.id_] = troop2;
	if client.cur_troop_.id_ == troop2.id_ then
		gameAssert(client.fight_.my_corp_id_<0, "");
		client.fight_.my_corp_id_ = 1;
		client.cur_troop_ = nil;
        client.cur_troop_ = troop2;
		if troop2.marks_.map_ then
			for k, v in pairs(troop2.marks_.map_) do
				client.cur_troop_.marks_[k] = v;
			end
		end
	end
	return fight;
end
protected_.MetaFight.getFightAttack = function (fight_worker, attack_id)
	if fight_worker.cur_attack and fight_worker.cur_attack.attack_id==attack_id then
		if fight_worker.attacks then
			return fight_worker.cur_attack, #fight_worker.attacks;
		else
			return fight_worker.cur_attack, 0;
		end
	elseif fight_worker.attacks then
		for i,k in ipairs(fight_worker.attacks) do
			if k.attack_id==attack_id then
				return k, i;
			end
		end
	end
	return nil;
end
protected_.MetaFight.getActionAttack = function (fight_worker)
	if fight_worker.attacks==nil then
		return nil;
	end
	for i, attack in ipairs(fight_worker.attacks) do
		local skill_cfg = configs_.SkillCfg[attack.skill_sid];
		if (skill_cfg == nil) then
			gameError(string.format("skill_sid = %d is not found in SkillCfg!", attack.skill_sid));
		elseif skill_cfg.action_time>0 then
			return attack;
		end
	end
	return nil;
end
protected_.MetaFight.getPetIndex = function(client)
	local fight_worker = client.fight_.fight_worker_;
	return fight_worker.my_pet_indx_;
end
protected_.MetaFight.getPetByIndex = function(client, pet_indx)
	local fight_worker = client.fight_.fight_worker_;
	local indx_name = protected_.member_pet_ids[pet_indx];
	if indx_name==nil or client.cur_troop_==nil then
		return nil;
	end
	local local_mem = client.cur_troop_[client.suid_];
	return local_mem[indx_name];
end
protected_.MetaFight.createFightWorker = function(client, msg, index)
	print("createFightWorker...");
	local fight_worker = protected_.constructObject(protected_.MetaFightWorker, nil, client.fight_, "fight_worker_");
	fight_worker.fight_server_id_ = msg[index];
	index = index + 1;
	fight_worker.field_id_ = msg[index];
	index = index + 1;
	fight_worker.field_sid_ = msg[index];
	index = index + 1;
	fight_worker.round_count_ = msg[index];
	index = index + 1;
	fight_worker.round_unit_id_ = msg[index];
	index = index + 1;
	
	local my_fight_flag = client.fight_.my_corp_id_>=0;
	fight_worker.worker_state_ = 0;
	fight_worker.corps_ ={};
	fight_worker.corps_[0] = {}
	fight_worker.corps_[1] = {}
	local unit_num = msg[index];
	index = index + 1;
	print("createFightWorker unit_num=", unit_num);
	for i=1, unit_num do
		local unit;
		index,unit = protected_.MetaUnit.createUnit(client, msg, index);
		fight_worker[unit.unit_id_] = unit;
		print("createFightWorker createUnit=", unit.unit_id_, unit, unit.corp_id_, unit.slot_id_, unit.unit_sid_);
		if not protected_.isSlotUnit(unit.unit_sid_) then
			fight_worker.corps_[unit.corp_id_][unit.slot_id_] = unit;
		end
		if my_fight_flag and unit.suid_==client.suid_ then
			if protected_.isHero(unit.unit_sid_) then
				fight_worker.my_unit_id_ = unit.unit_id_;
			elseif protected_.isPet(unit.unit_sid_) then
				fight_worker.my_pet_id_ = unit.unit_id_;
				fight_worker.my_pet_indx_ = unit.pet_indx_;
			end
		end
	end
	local unit_order_num = msg[index] or 0;
	index = index + 1;
	local unit_order = {};
	for i = 1, unit_order_num do
		unit_order[msg[index]] = i;
		index = index + 1;
	end
	local action_end_num = msg[index] or 0;
	index = index + 1;
	local action_end_unit = {};
	for i = 1, action_end_num do
		action_end_unit[msg[index]] = 1;
		index = index + 1;
	end
	fight_worker.unit_order_ = unit_order;
	fight_worker.action_end_unit_ = action_end_unit;
	client.fight_.fight_worker_ = fight_worker;
	fight_worker.state_=protected_.MetaFight.EFightState_Wait;
	fight_worker.tick_actions_ = {};
	fight_worker.tick_count_ = 0;
	fight_worker.client_begin_ = false;
	fight_worker.fight_run_ = false;
	protected_.registFightUpdater(client);
	return fight_worker;
end

protected_.MetaFight.onClientRecall = function(recall_func, client, ...)
	local fight_worker = client.fight_.fight_worker_;
	if not fight_worker.client_begin_ then
		fight_worker.fight_run_ = true;
		return;
	end
	client_recall_(recall_func, client.client_id_, ...);
end

protected_.fightHandlers[Enum.EFightProto_Begin] = function(client, msg, index)
	protected_.MetaFight.onClientRecall("onFightBegin", client);
end
protected_.fightHandlers[Enum.EFightProto_RoundPrepare] = function(client, msg, index)
	local fight_worker = client.fight_.fight_worker_;
    local round_count = msg[index];
	index = index + 1;
    local exp_time = msg[index];
	index = index + 1;
	local player_attack_state = msg[index] or 0;
	index = index + 1;
	fight_worker.round_count_ = round_count;
	protected_.MetaFight.onClientRecall("onPrepareRound", client, round_count, exp_time, player_attack_state);
	fight_worker.state_=protected_.MetaFight.EFightState_Wait;
end
protected_.fightHandlers[Enum.EFightProto_RoundBegin] = function(client, msg, index)
	local fight_worker = client.fight_.fight_worker_;
	if fight_worker.worker_state_==0 then
		fight_worker.worker_state_ = 1;
	end
	local round_count = msg[index];
	index = index + 1;
	local round_beg_time = msg[index];
	index = index + 1;
	local cur_tick = msg[index];
	index = index + 1;
    local cur_time = round_beg_time or getTime();
	if fight_worker.cur_time_ and round_count>0 then
		if fight_worker.cur_time_>cur_time then
			cur_time = fight_worker.cur_time_;
		end
		while #client.fight_.fight_worker_.tick_actions_>0 do
			cur_time = cur_time + 1000;
			fight_worker.state_=protected_.MetaFight.EFightState_Runing;
			protected_.MetaFight.doFightUpdate(client, cur_time);
		end
	end
	fight_worker.cur_round_tick_ = cur_tick;
	fight_worker.round_count_ = round_count;
	fight_worker.cur_time_ = getTime();
	protected_.clearTempCondition(fight_worker);
	if round_count==0 then
		protected_.MetaFight.onClientRecall("onReadyRoundBegin", client);
	else
		protected_.MetaFight.onClientRecall("onRoundBegin", client, round_count);
	end
	fight_worker.state_=protected_.MetaFight.EFightState_Runing;
end
protected_.fightHandlers[Enum.EFightProto_Start] = function(client, msg, index)
	local fight_worker = client.fight_.fight_worker_;
	fight_worker.cur_round_tick_ = msg[index];
	index = index + 1;
	fight_worker.cur_time_ = getTime();
	fight_worker.state_=protected_.MetaFight.EFightState_Runing;
end

protected_.fightHandlers[Enum.EFightProto_RoundEnd] = function(client, msg, index)
	local fight_worker = client.fight_.fight_worker_;
	local round_count = msg[index];
	index = index + 1;
	if round_count==0 then
		protected_.MetaFight.onClientRecall("onReadyRoundEnd", client);
	else
		protected_.MetaFight.onClientRecall("onRoundEnd", client, round_count);
	end
	fight_worker.action_end_unit_ = {};
end
protected_.fightHandlers[Enum.EFightProto_UnitBegin] = function(client, msg, index)
    local unit_id = msg[index];
	index = index + 1;
	local fight_worker = client.fight_.fight_worker_;
	local unit = fight_worker[unit_id];
	fight_worker.attacks = {};
	fight_worker.cur_attack=nil;
	protected_.MetaFight.onClientRecall("onUnitBegin", client, unit);
end

protected_.attack_fails = {
500005,			--发起者死亡
500006,			--发起者休息
500007,			--发起者无法普攻
500008,			--发起者无法技能
500009,			--目标距离过远
500010,			--目标不符合条件
500011,			--怒气不足
500012,			--不满足释放条件
500101,         --不允许复活
10292,          --技能状态没中
10293,          --debuff不能上
}
protected_.fightHandlers[Enum.EFightProto_AttackFailed] = function(client, msg, index)
    local unit_id = msg[index];
    index = index + 1;
	local skill_sid = msg[index];
    index = index + 1;
    local fight_worker = client.fight_.fight_worker_;
    local unit = fight_worker[unit_id];
    local reason = msg[index];
    index = index + 1;
	local show_reason;
	if unit and reason == Enum.EAttackFailed_DummyState then
		local dummys = {[1]=500006,[5]=500006};--[状态sid]=>提示内容
		for k,v in pairs(unit.states_) do
			show_reason = dummys[v.state_sid_];
			if show_reason then
				break;
			end
		end
	end
	if show_reason==nil then
		show_reason = protected_.attack_fails[reason];
		if show_reason==nil then
			show_reason = 10292;
		end
	end
	local unit_data = configs_.UnitCfg[unit.unit_sid_];
	if unit_data and unit_data.attack_fail_tips_ == 1 then
		return ;
	end
    protected_.MetaFight.onClientRecall("onAttackFailed", client, unit, skill_sid, show_reason);--onUnableAttack
end

protected_.fightHandlers.stateFailed = function(client, msg, index)
	local fight_worker = client.fight_.fight_worker_;
	local unit_id = msg[index];
    index = index + 1;
	local state_sid = msg[index];
    index = index + 1;
    local fight_worker = client.fight_.fight_worker_;
    local unit = fight_worker[unit_id];
    local reason = msg[index];
    index = index + 1;
	local show_reason;
	if show_reason==nil then
		show_reason = protected_.attack_fails[reason];
		if show_reason==nil then
			show_reason = 10292;
		end
	end
    protected_.MetaFight.onClientRecall("onStateFailed", client, unit, state_sid, show_reason);--onUnableAttack
end

protected_.fightHandlers[Enum.EFightProto_StateFailed] = function(client, msg, index)
	local fight_worker = client.fight_.fight_worker_;
	local attack = protected_.MetaFight.getActionAttack(fight_worker);
	if attack then
		table.insert(attack.attackEndEvents, {"stateFailed", msg, index});
	else
		protected_.fightHandlers.stateFailed(client, msg, index);
	end
    
end

protected_.fightHandlers.handleStateOp = function(client, msg, index)
	local fight_worker = client.fight_.fight_worker_;
    local unit_id = msg[index];
	index = index + 1;
	local unit = fight_worker[unit_id];
	if unit==nil then
		gameError("handleStateOp unit[", unit_id, "] is nil");
		return ;
	end
    local op = msg[index];
	index = index + 1;
	local state;
	if op==1 then
		index, state = protected_.MetaSkillState.createState(unit, msg, index);
		if state then
			if protected_.isSlotUnit(unit.unit_sid_) then
				protected_.MetaFight.onClientRecall("onSlotStateAdd", client, unit.corp_id_, unit.slot_id_, state);
			else
				protected_.MetaFight.onClientRecall("onSkillStateAdd", client, unit, state);
			end
		end
	else
		index, state = protected_.MetaSkillState.removeState(unit, msg, index);
		if state then
			if protected_.isSlotUnit(unit.unit_sid_) then
				protected_.MetaFight.onClientRecall("onSlotStateDel", client, unit.corp_id_, unit.slot_id_, state);
			else
				protected_.MetaFight.onClientRecall("onSkillStateDel", client, unit, state);
			end
		end
	end
end
protected_.fightHandlers[Enum.EFightProto_State] = function(client, msg, index)
	local fight_worker = client.fight_.fight_worker_;
    local unit_id = msg[index];
	index = index + 1;
	local unit = fight_worker[unit_id];
	if unit==nil then
		gameError("EFightProto_State unit["..unit_id.."] is nil");
		return ;
	end
    local op = msg[index];
	index = index + 1;
	local state;
	local state_recall;
	if op==1 then
		index, state = protected_.MetaSkillState.createState(unit, msg, index);
		if state then
			if protected_.isSlotUnit(unit.unit_sid_) then
				state_recall = {"onSlotStateAdd", client, unit.corp_id_, unit.slot_id_, state};
				--protected_.MetaFight.onClientRecall("onSlotStateAdd", client, unit.corp_id_, unit.slot_id_, state);
			else
				state_recall = {"onSkillStateAdd", client, unit, state};
				--protected_.MetaFight.onClientRecall("onSkillStateAdd", client, unit, state);
			end
		end
	else
		index, state = protected_.MetaSkillState.removeState(unit, msg, index);
		if state then
			if protected_.isSlotUnit(unit.unit_sid_) then
				state_recall = {"onSlotStateDel", client, unit.corp_id_, unit.slot_id_, state};
				--protected_.MetaFight.onClientRecall("onSlotStateDel", client, unit.corp_id_, unit.slot_id_, state);
			else
				state_recall = {"onSkillStateDel", client, unit, state};
				--protected_.MetaFight.onClientRecall("onSkillStateDel", client, unit, state);
			end
		end
	end
	if state_recall then
		local cur_attack = fight_worker.cur_attack;
		if cur_attack and fight_worker.cur_stage then
			local skill_cfg = configs_.SkillCfg[cur_attack.skill_sid];
			if skill_cfg and (skill_cfg.action_time and skill_cfg.action_time>0) then
				table.insert(fight_worker.cur_stage.states, state_recall);
				return;
			end
		end
		protected_.MetaFight.onClientRecall(unpack(state_recall));
	end
end
protected_.fightHandlers[Enum.EFightProto_PropChange] = function(client, msg, index)
	local fight_worker = client.fight_.fight_worker_;
    local unit_id = msg[index];
	index = index + 1;
	local prop_index = msg[index];
	index = index + 1;
	local d_val = msg[index];
	index = index + 1;
	local cur_val = msg[index];
	index = index + 1;
	local unit = fight_worker[unit_id];
	if unit==nil or protected_.isSlotUnit(unit.unit_sid_) then
		return; -- 刚出生的unit也会发送属性更新
	end

	local old_val = unit.base_[prop_index+1];
	unit.base_[prop_index+1] = cur_val;
	local prop_name = PropIndex[prop_index];
	protected_.MetaFight.onClientRecall("onUnitPropChange", client, unit, prop_name, cur_val, d_val, old_val);
end
protected_.fightHandlers[Enum.EFightProto_UnitShift] = function(client, msg, index)
	local fight_worker = client.fight_.fight_worker_;
	local unit_id = msg[index];
	index = index + 1;
	local cur_unit_sid = msg[index];
	index = index + 1;
	local skill_count = msg[index];
	index = index + 1;
	local unit = fight_worker[unit_id];
	if unit==nil then
		return;
	end
	print("##EFightProto_UnitShift", unit_id, cur_unit_sid);
	if skill_count>0 then
		unit.skills_ = {};
		unit.skill_lists_ = {};
		unit.skill_raw_lists_={};
		for i=1, skill_count do
			local skill_raw_id = msg[index];
			index = index + 1;
			local skill_data = configs_.SkillCfg[skill_raw_id];
			if skill_data and skill_data.cast_type~=Enum.ECastType_Passive then
				unit.skill_lists_[i]=skill_raw_id;
			else
				unit.skill_lists_[i]=0;
			end
			print("EFightProto_UnitShift", i, unit.skill_lists_[i]);
			unit.skill_raw_lists_[i] = skill_raw_id;
			unit.skills_[skill_raw_id] = {slot=i};
		end
		if skill_count<Enum.ESkill_MAX then
			for i=skill_count+1, Enum.ESkill_MAX do
				print("EFightProto_UnitShift", i, 0);
				unit.skill_lists_[i]=0;
			end
		end
		for i,v in ipairs(configs_.base_skills_) do
			if protected_.isHero(unit.unit_sid_) or v~=1600102 then
				unit.skill_lists_[Enum.ESkill_MAX+i]=v;
				unit.skills_[v] = {slot=Enum.ESkill_MAX+i};
			else
				unit.skill_lists_[Enum.ESkill_MAX+i]=0;
			end
			print("EFightProto_UnitShift", Enum.ESkill_MAX+i, unit.skill_lists_[Enum.ESkill_MAX+i]);
		end
	end
	for k,v in pairs(unit.skills_) do
		print("==EFightProto_UnitShift", k, v.slot);
	end
	protected_.MetaFight.onClientRecall("onUnitShift", client, unit);
end
protected_.fightHandlers[Enum.EFightProto_AttackBegin] = function(client, msg, index)
	local fight_worker = client.fight_.fight_worker_;
	local attack={};
    attack.attack_id = msg[index];
	index = index + 1;
    attack.unit_id = msg[index];
	index = index + 1;
    attack.skill_sid = msg[index];
	index = index + 1;
    attack.skill_level = msg[index];
	index = index + 1;
	attack.dest_count = msg[index];
	index = index + 1;
	attack.targets = {};
	for i=1,attack.dest_count do 
		local dest_unit_id = msg[index];
		index = index + 1;
		if dest_unit_id>0 then
			if fight_worker[dest_unit_id]==nil then
				print("EFightProto_AttackBegin target error: ", attack.skill_sid, dest_unit_id);
			end
			table.insert(attack.targets, dest_unit_id);
		end
	end
    attack.attack_count = msg[index];
	index = index + 1;
    attack.dest_corp = msg[index];
	index = index + 1;
    attack.dest_slot = msg[index];
	index = index + 1;

	attack.src_unit = fight_worker[attack.unit_id];
	if attack.src_unit==nil then
		print("EFightProto_AttackBegin source error: ", attack.unit_id);
	end
	if protected_.isSlotUnit(attack.src_unit.unit_sid_) then
		attack.src_slot = {attack.src_unit.corp_id_, attack.src_unit.slot_id_};
	end

	local skill_cfg = configs_.SkillCfg[attack.skill_sid];
	if (skill_cfg == nil) then
		gameError(string.format("skill_sid = %d is not found in SkillCfg!", attack.skill_sid));
		return;
	end

	local target_type = skill_cfg.target_type or Enum.ETargetType_Unit;
	attack.target_slot, attack.target = protected_.fightHandlers.AttackTargets[target_type](fight_worker, attack, attack.targets);
	attack.stages={};
	attack.attackEndEvents={};
	attack.delUnits={};
	attack.addUnits={};
	if fight_worker.attacks then
		table.insert(fight_worker.attacks, attack);
	end
	fight_worker.cur_attack=attack;
	if (client_recall_ == protected_.default_recall) or (skill_cfg.action_time and skill_cfg.action_time>0) then
		--if skill_cfg.attack_back and skill_cfg.attack_back > 0 and attack.src_unit and attack.src_unit.goback_attack_==nil then
		if attack.src_unit and attack.src_unit.goback_attack_==nil then
			attack.src_unit.goback_attack_=attack.attack_id;
		end
		protected_.MetaFight.onClientRecall("onAttackBegin", client, attack.src_unit, attack.skill_sid, attack.attack_count, attack.targets, attack.dest_corp, attack.dest_slot);
	end
end
protected_.fightHandlers[Enum.EFightProto_UnitProtect] = function(client, msg, index)
	local fight_worker = client.fight_.fight_worker_;
    local target_pre = msg[index];
	index = index + 1;
    local target_id = msg[index];
	index = index + 1;
	protected_.MetaFight.onClientRecall("onUnitProtect", client, target_pre, target_id);
end
protected_.fightHandlers[Enum.EFightProto_ProProtect] = function(client, msg, index)
    local fight_worker = client.fight_.fight_worker;
    local pro_type = msg[index];
    index = index + 1;
    local src_id = msg[index];
    index = index + 1;
    local pro_id = msg[index];
    index = index + 1;
    protected_.MetaFight.onClientRecall("onProProtect", client, pro_type, src_id, pro_id);
end

protected_.fightHandlers.getSelfSlot = function(fight_worker, attack, targets)
	if attack.src_slot then
		return attack.src_slot, nil;
	end
	return nil, attack.src_unit;
end
protected_.fightHandlers.getTargetSlot = function(fight_worker, attack, targets)
	if #targets==0 then
		return nil, nil;
	end
	local target_id = targets[1];
	local target = fight_worker[target_id];
	if target==nil then
		print("setTargetSlot Error!!!!!", target_id);
	end
	return nil, target;
end
protected_.fightHandlers.getDestSlot = function(fight_worker, attack, targets)
	return {attack.dest_corp, attack.dest_slot}, nil;
end
protected_.fightHandlers.AttackTargets={
	[Enum.ETargetType_Unit] = protected_.fightHandlers.getTargetSlot,
	[Enum.ETargetType_FrontUnit] = protected_.fightHandlers.getTargetSlot,
	[Enum.ETargetType_Self] = protected_.fightHandlers.getSelfSlot,
	[Enum.ETargetType_ALL] = protected_.fightHandlers.getSelfSlot,
	[Enum.ETargetType_Slot] = protected_.fightHandlers.getDestSlot,
	[Enum.ETargetType_Targets] = protected_.fightHandlers.getTargetSlot,
};
protected_.fightHandlers[Enum.EFightProto_Damage] = function(client, msg, index)
	local fight_worker = client.fight_.fight_worker_;
	local damage = {};
    damage.attack_id = msg[index];
	index = index + 1;
    damage.attacker_id = msg[index];
	index = index + 1;
    damage.context_id = msg[index];
	index = index + 1;
    damage.target_id = msg[index];
	index = index + 1;
    damage.stage_id = msg[index];
	index = index + 1;
    damage.damage_p = msg[index];
    index = index + 1;
    damage.damage_m = msg[index];
    index = index + 1;
    damage.damage_r = msg[index];
    index = index + 1;
    damage.damage_sum = msg[index];
	index = index + 1;
    damage.damage_back = msg[index];
	index = index + 1;
    damage.absorb_hp = msg[index];
	index = index + 1;
    damage.damage_result = msg[index];
	index = index + 1;
    damage.damage_immue = msg[index];
    index = index + 1;
    damage.back_dmg_immue = msg[index];
    index = index + 1;
	damage.src_unit = fight_worker[damage.attacker_id];
	if damage.src_unit==nil then
		gameError("Error!! EFightProto_Damage Attacker "..damage.attacker_id.." is nil!!! ");
		if fight_worker.cur_attack then
			gameError("Error!! EFightProto_Damage Attacker: "..fight_worker.cur_attack.skill_sid..","..fight_worker.cur_attack.src_unit.unit_data_sid_.."...");
		end
		--damage.src_unit = fight_worker.cur_attack.src_unit;
	end
	if damage.src_unit and protected_.isSlotUnit(damage.src_unit.unit_sid_) then
		damage.src_slot = {damage.src_unit.corp_id_, damage.src_unit.slot_id_};
	end
	damage.dest_unit = fight_worker[damage.target_id];
	if damage.dest_unit==nil then
		gameError("Error!! EFightProto_Damage "..damage.target_id.." is nil!!! "..(damage.src_unit.owner_name_ or "nil").."["..damage.attack_id..","..damage.context_id.."]");
		return ;
	end
	if protected_.isSlotUnit(damage.dest_unit.unit_sid_) then
		return;
	end
	if fight_worker.cur_stage then
		if damage.context_id>0 then
			table.insert(fight_worker.cur_stage.trigger_damages, damage);
		else
			table.insert(fight_worker.cur_stage.damages, damage);
		end
	else
		damage.skill_state = nil;
		if damage.attack_id==0 and damage.dest_unit then
			damage.skill_state = damage.dest_unit.states_[damage.context_id];
		end
		protected_.MetaFight.onClientRecall("onDamage", client, damage);
	end
end
protected_.fightHandlers[Enum.EFightProto_StageBegin] = function(client, msg, index)
	local fight_worker = client.fight_.fight_worker_;
	if fight_worker.cur_attack==nil then
		return;
	end
	local stage = {damages={}, trigger_damages={}, states={}};
    stage.stage_id = msg[index];
	index = index + 1;
	fight_worker.cur_stage = stage;
	table.insert(fight_worker.cur_attack.stages, stage);
end
protected_.fightHandlers[Enum.EFightProto_StageEnd] = function(client, msg, index)
	local fight_worker = client.fight_.fight_worker_;
    local stage_id = msg[index];
	index = index + 1;
	if fight_worker.cur_attack==nil or fight_worker.cur_stage==nil then
		return ; --断线重连状态恢复
	end
	if stage_id ~= fight_worker.cur_stage.stage_id then
		gameLog("Stage error: "..fight_worker.cur_attack.skill_sid..", "..stage_id);
	end

--	protected_.MetaFight.onClientRecall("onDamages", client, fight_worker.cur_attack, target_slot, fight_worker.cur_stage.damages, fight_worker.cur_stage.trigger_damages);
--	for i, v in ipairs (fight_worker.cur_stage.states) do
--		protected_.fightHandlers.handleStateOp(client, v[1], v[2]);
--	end
	fight_worker.cur_stage = nil;
end
protected_.fightHandlers[Enum.EFightProto_DoAttack] = function(client, msg, index)
	local fight_worker = client.fight_.fight_worker_;
	local attack_id = msg[index];
	index = index + 1;
    
	local cur_attack, cur_attack_i = protected_.MetaFight.getFightAttack(fight_worker, attack_id);
	if cur_attack==nil then
		return;
	end

	local skill_cfg = configs_.SkillCfg[cur_attack.skill_sid];
	if (skill_cfg == nil) then
		return;
	end

	if (client_recall_ == protected_.default_recall) or (skill_cfg.action_time and skill_cfg.action_time>0) then
		protected_.MetaFight.onClientRecall("onAttackStages", client, cur_attack, cur_attack.stages);
		fight_worker.cur_stage = nil;
	end
end
protected_.fightHandlers[Enum.EFightProto_DoAttackEnd] = function(client, msg, index)
	local fight_worker = client.fight_.fight_worker_;
	local attack_id = msg[index];
	index = index + 1;
    
	local cur_attack, cur_attack_i = protected_.MetaFight.getFightAttack(fight_worker, attack_id);
	if cur_attack==nil then
		return;
	end
	for i,v in ipairs(cur_attack.attackEndEvents) do
		local event_call = protected_.fightHandlers[v[1]];
		if event_call then
			event_call(client, v[2], v[3]);
		end
	end
	cur_attack.attackEndEvents={};
	for i,v in ipairs(cur_attack.delUnits) do
		protected_.fightHandlers.delUnit(client, v[1], v[2]);
	end
	cur_attack.delUnits={};
	for i,v in ipairs(cur_attack.addUnits) do
		protected_.MetaFight.onClientRecall("onAddUnit", client, v);
	end
	cur_attack.addUnits={};

end

protected_.fightHandlers[Enum.EFightProto_GoBack] = function(client, msg, index)
	local fight_worker = client.fight_.fight_worker_;
    local attack_id = msg[index];
	index = index + 1;
	local cur_attack, cur_attack_i = protected_.MetaFight.getFightAttack(fight_worker, attack_id);
	if cur_attack==nil then
		print("EFightProto_GoBack "..attack_id.." is nil! attack="..tostring(cur_attack));
	elseif cur_attack.src_unit==nil then
		gameError("EFightProto_GoBack ["..attack_id.."] src_unit is nil!");
	elseif not protected_.isSlotUnit(cur_attack.src_unit.unit_sid_) then
		-- print("EFightProto_GoBack ", cur_attack.src_unit.unit_sid_, cur_attack.skill_sid, cur_attack.src_unit.goback_attack_, attack_id);
		local skill_cfg = configs_.SkillCfg[cur_attack.skill_sid];
		if (skill_cfg == nil) then
			return;
		end
		if (client_recall_ == protected_.default_recall) or (skill_cfg.action_time and skill_cfg.action_time>0) then
			if cur_attack.src_unit.goback_attack_==attack_id then
				protected_.MetaFight.onClientRecall("onGoBack", client, cur_attack.src_unit);
				cur_attack.src_unit.goback_attack_ = nil;
			end
		end
	end
end
protected_.fightHandlers[Enum.EFightProto_AttackEnd] = function(client, msg, index)
	local fight_worker = client.fight_.fight_worker_;
    local attack_id = msg[index];
	index = index + 1;
    local skill_id = msg[index];
	index = index + 1;

	if fight_worker.cur_attack==nil then
		return ; --断线重连状态恢复
	end
	local cur_attack, cur_attack_i = protected_.MetaFight.getFightAttack(fight_worker, attack_id);
	if cur_attack==nil then
		return;
	end
	local skill_cfg = configs_.SkillCfg[cur_attack.skill_sid];
	if skill_cfg then
		if (client_recall_ == protected_.default_recall) or (skill_cfg.action_time and skill_cfg.action_time>0) then
			if cur_attack.src_unit and cur_attack.src_unit.goback_attack_==attack_id then
				-- print("EFightProto_AttackEnd ", cur_attack.src_unit.unit_sid_, cur_attack.skill_sid, cur_attack.src_unit.goback_attack_, attack_id);
				cur_attack.src_unit.goback_attack_ = nil;
			end
			protected_.MetaFight.onClientRecall("onAttackEnd", client, cur_attack.unit_id, cur_attack.src_slot, attack_id, skill_id);
		end
	end
	for i,v in ipairs(cur_attack.attackEndEvents) do
		local event_call = protected_.fightHandlers[v[1]];
		if event_call then
			event_call(client, v[2], v[3]);
		end
	end
	for i,v in ipairs(cur_attack.delUnits) do
		protected_.fightHandlers.delUnit(client, v[1], v[2]);
	end
	for i,v in ipairs(cur_attack.addUnits) do
		protected_.MetaFight.onClientRecall("onAddUnit", client, v);
	end
	if cur_attack==fight_worker.cur_attack then
		fight_worker.cur_attack = nil;
		if fight_worker.attacks then
			table.remove(fight_worker.attacks);
			local sz = #fight_worker.attacks;
			if sz>0 then
				fight_worker.cur_attack = fight_worker.attacks[sz];
			end
		end
	elseif cur_attack_i then
		table.remove(fight_worker.attacks, cur_attack_i);
	end
end

protected_.fightHandlers.handleRevive = function(client, msg, index)
	local fight_worker = client.fight_.fight_worker_;
    local unit_id = msg[index];
	index = index + 1;
	local unit = fight_worker[unit_id];
	unit.dead_flag = nil;
	if fight_worker.my_pet_id_==unit_id then
		local local_mem = client.cur_troop_[client.suid_];
		--fight_worker.my_pet_id_ = 0;
		local indx_name = protected_.member_pet_ids[unit.pet_indx_];
		local pet = local_mem[indx_name];
		if pet then
			pet.is_dead = false;
		end
	end
	protected_.MetaFight.onClientRecall("onRevive", client, unit);
end

protected_.fightHandlers.handleDead = function(client, msg, index)
	local fight_worker = client.fight_.fight_worker_;
    local unit_id = msg[index];
	index = index + 1;
	local unit = fight_worker[unit_id];
	if unit==nil then
		print("handleDead: ", unit_id, "not exist...");
		return;
	end
	unit.dead_flag = true;
	protected_.MetaFight.onClientRecall("onDead", client, unit);
	if not protected_.isHero(unit.unit_sid_) then
		--protected_.MetaFight.onClientRecall("onDelUnit", client, unit, 1);
		--fight_worker[unit.unit_id_] = nil;
		-- print("EFightProto_Dead unit_id remove: ", unit.unit_id_);
		--fight_worker.corps_[unit.corp_id_][unit.slot_id_] = nil;
		if fight_worker.my_pet_id_==unit.unit_id_ then
			local local_mem = client.cur_troop_[client.suid_];
			--fight_worker.my_pet_id_ = 0;
			local indx_name = protected_.member_pet_ids[unit.pet_indx_];
			local pet = local_mem[indx_name];
			if pet then
				pet.is_dead = true;
			end
		end
	end
end
protected_.fightHandlers.addUnit = function(client, msg, index)
	local fight_worker = client.fight_.fight_worker_;
	local unit;
	index,unit = protected_.MetaUnit.createUnit(client, msg, index);
	print("EFightProto_AddUnit createUnit=", unit.unit_id_, unit.unit_sid_, unit.corp_id_, unit.slot_id_);
	fight_worker[unit.unit_id_] = unit;
	if not protected_.isSlotUnit(unit.unit_sid_) then
		fight_worker.corps_[unit.corp_id_][unit.slot_id_] = unit;
	end
	if protected_.isPet(unit.unit_sid_) and client.fight_.my_corp_id_>=0 and unit.suid_==client.suid_ then
		fight_worker.my_pet_id_ = unit.unit_id_;
		fight_worker.my_pet_indx_ = unit.pet_indx_;
	end
	return unit;
end
protected_.fightHandlers.delUnit = function(client, msg, index)
	local fight_worker = client.fight_.fight_worker_;
    local unit_id = msg[index];
	index = index +1;
	local dead_flag = msg[index];
	index = index +1;
	-- gameError("EFightProto_DelUnit delUnit: "..unit_id);
	local fight_worker = client.fight_.fight_worker_;
	local unit = fight_worker[unit_id];
	if unit==nil then
		return;
	end
	protected_.MetaFight.onClientRecall("onDelUnit", client, unit, dead_flag);
	fight_worker[unit.unit_id_] = nil;
	print("EFightProto_DelUnit unit_id remove: ", unit.unit_id_);
	fight_worker.corps_[unit.corp_id_][unit.slot_id_] = nil;
	if fight_worker.my_pet_id_==unit.unit_id_ then
		local local_mem = client.cur_troop_[client.suid_];
		fight_worker.my_pet_id_ = 0;
	end
end
protected_.fightHandlers[Enum.EFightProto_Dead] = function(client, msg, index)
	local fight_worker = client.fight_.fight_worker_;
	local attack = protected_.MetaFight.getActionAttack(fight_worker);
	if attack then
		table.insert(attack.attackEndEvents, {"handleDead", msg, index});
	else
		protected_.fightHandlers.handleDead(client, msg, index);
	end
end
protected_.fightHandlers[Enum.EFightProto_AddUnit] = function(client, msg, index)
	local fight_worker = client.fight_.fight_worker_;
	local unit = protected_.fightHandlers.addUnit(client, msg, index);
	if not protected_.isSlotUnit(unit.unit_sid_) then
		local attack = protected_.MetaFight.getActionAttack(fight_worker);
--		if attack then
--			table.insert(attack.addUnits, unit);
--		else
			protected_.MetaFight.onClientRecall("onAddUnit", client, unit);
--		end
	end
end
protected_.fightHandlers[Enum.EFightProto_DelUnit] = function(client, msg, index)
	local fight_worker = client.fight_.fight_worker_;
	local attack = protected_.MetaFight.getActionAttack(fight_worker);
	if attack then
		table.insert(attack.delUnits, {msg, index});
	else
		protected_.fightHandlers.delUnit(client, msg, index);
	end
end

protected_.fightHandlers[Enum.EFightProto_Revive] = function(client, msg, index)
	local fight_worker = client.fight_.fight_worker_;
	local attack = protected_.MetaFight.getActionAttack(fight_worker);
	if attack then
		table.insert(attack.attackEndEvents, {"handleRevive", msg, index});
	else
		protected_.fightHandlers.handleRevive(client, msg, index);
	end
end

protected_.fightHandlers[Enum.EFightProto_UnitUnDead] = function(client, msg, index)
    local unit_id = msg[index];
    index = index + 1;
    local fight_worker = client.fight_.fight_worker_;
    local unit = fight_worker[unit_id];
    protected_.MetaFight.onClientRecall("onUnitUnDead", client, unit);
end


protected_.fightHandlers[Enum.EFightProto_SlotChange] = function(client, msg, index)
	protected_.MetaFight.onClientRecall("onSlotChange", client, unit, corp_id, slot_id);
end
protected_.fightHandlers[Enum.EFightProto_UnitEnd] = function(client, msg, index)
	local fight_worker = client.fight_.fight_worker_;
    local unit_id = msg[index];
	index = index + 1;
	local action_end_flag = msg[index];
	index = index + 1;
	local unit = fight_worker[unit_id];
	if unit then
		protected_.MetaFight.onClientRecall("onUnitEnd", client, unit);
	end
	if action_end_flag == 1 then
		if not fight_worker.action_end_unit_ then
			fight_worker.action_end_unit_ = {};
		end
		fight_worker.action_end_unit_[unit_id] = 1;
		client_recall_("onUnitOrder", client.client_id_);
	end
end
protected_.fightHandlers[Enum.EFightProto_End] = function(client, msg, index)
	local fight_worker = client.fight_.fight_worker_;
    local fight_result = msg[index];
	index = index + 1;
	protected_.MetaFight.onClientRecall("onFightResult", client, fight_result, fight_worker.statistics);
	protected_.MetaFight.onClientRecall("onFightEnd", client);
	fight_worker.worker_state_ = 10;
	--client.fight_.fight_worker_ = nil;
	--client.fight_ = nil;
	--fight_worker.state_=protected_.MetaFight.EFightState_None;
	--protected_.unregistFightUpdater(client);
end
protected_.fightHandlers[Enum.EFightProto_UnitOrder] = function(client, msg, index)
	local fight_worker = client.fight_.fight_worker_;
    local unit_num = msg[index] or 0;
	index = index + 1;
	local unit_order = {};
	for i = 1, unit_num do
		unit_order[msg[index]] = i;
		index = index + 1;
	end
	fight_worker.unit_order_ = unit_order;
	if not fight_worker.action_end_unit_ then
		fight_worker.action_end_unit_ = {};
	end
	client_recall_("onUnitOrder", client.client_id_);
end

protected_.fightHandlers[Enum.EFightProto_SwapSlot] = function(client, msg, index)
	local fight_worker = client.fight_.fight_worker_;
    local corp_id = msg[index];
	index = index + 1;
    local slot_id1 = msg[index];
	index = index + 1;
    local slot_id2 = msg[index];
	index = index + 1;
	local unit1 = fight_worker.corps_[corp_id][slot_id1];
	local unit2 = fight_worker.corps_[corp_id][slot_id2];
	if unit1 then
		unit1.slot_id_ = slot_id2;
		fight_worker.corps_[corp_id][slot_id2] = unit1;
	else
		fight_worker.corps_[corp_id][slot_id2] = nil;
	end
	if unit2 then
		unit2.slot_id_ = slot_id1;
		fight_worker.corps_[corp_id][slot_id1] = unit2;
	else
		fight_worker.corps_[corp_id][slot_id1] = nil;
	end
	protected_.MetaFight.onClientRecall("onSwapSlot", client, corp_id, slot_id1, slot_id2);
end
protected_.fightHandlers[Enum.EFightProto_QTEBegin] = function(client, msg, index)
	local fight_worker = client.fight_.fight_worker_;
    local qte_sid = msg[index];
	index = index + 1;
    local unit_id = msg[index];
	index = index + 1;
    local attack_id = msg[index];
	index = index + 1;
	print("Attacker QTE: ", unit_id, qte_sid);
	local unit = fight_worker[unit_id];
	if unit then
		--unit.suid_==client.suid_判定是否是自己
		protected_.MetaFight.onClientRecall("onAttackerQTE", client, unit, qte_sid, attack_id);
	end
    
end
protected_.fightHandlers[Enum.EFightProto_QTETarget] = function(client, msg, index)
	local fight_worker = client.fight_.fight_worker_;
    local qte_sid = msg[index];
	index = index + 1;
    local unit_id = msg[index];
	index = index + 1;
	print("Target QTE: ", unit_id, qte_sid);
	local unit = fight_worker[unit_id];
	if unit then
		--unit.suid_==client.suid_判定是否是自己
		protected_.MetaFight.onClientRecall("onTargetQTE", client, unit, qte_sid);
	end
end
protected_.fightHandlers[Enum.EFightProto_DialogOpen] = function(client, msg, index)
	local fight_worker = client.fight_.fight_worker_;
	local dialog_sid = msg[index];
	index = index + 1;
	local dialog_context = msg[index];
	index = index + 1;
	local field_id = msg[index];
	index = index + 1;
	local unit_id = msg[index];
	index = index + 1;
	local dialog = protected_.constructObject(protected_.MetaDialog);
	dialog:open(client.player_, dialog_sid, dialog_context, field_id, unit_id);

	client_recall_("onDialogOpen", client_id, dialog_sid, dialog_context, field_id, unit_id);
end
