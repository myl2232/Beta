
--protected_.MetaGamePlayer.dirty_recall = function(self, object, key, old_val)
--	gameLog("RRRRRRRRRRRRRRRRR ", key, old_val, object[key]);
--end
protected_.getUnitName = function(client_id, unit)
	if unit==nil then
		gameLog("protected_.getUnitName: ", debug.traceback());
	end
    local client = protected_.clients_[client_id];
	local unit_data = configs_.UnitCfg[unit.unit_sid_]
	if unit_data==nil then
		gameLog("VVVVVVVVVVVVVVVV", unit.unit_sid_, debug.traceback());
	end
	if protected_.isHero(unit.unit_sid_) then
		return "阵营"..unit.corp_id_.."的"..unit.unit_sid_;
	elseif unit.raw_owner_name_ then
		if #unit.raw_owner_name_>0 then
			return unit.raw_owner_name_.."的"..unit.unit_sid_;
		else
			return "阵营"..unit.corp_id_.."的"..unit.unit_sid_;
		end
	else
		return "阵营"..unit.corp_id_.."的"..unit.unit_sid_;
	end
end
protected_.callbacks_.onStart = function()
	--以后用协程处理
	for i=1,protected_.client_count_ do
		doLuaCommand("connect", i, protected_.server_ip_, protected_.server_port_);
		--protected_.callConsoleCommand("connect", {i, protected_.server_ip_, protected_.server_port_});
	end
end

protected_.callbacks_.onConnect = function(client_id)
	local acc_name = "test-" .. (protected_.client_beg_+client_id-1);
	doLuaCommand("login", client_id, acc_name);
end
protected_.callbacks_.onReconnect = function(client_id)
end
protected_.callbacks_.onConnectResult = function(client_id, conn_ret, server_ip, server_port)
	gameLog("Connect["..client_id.."] result: ", conn_ret);
end
protected_.callbacks_.onReconnectResult = function(client_id, conn_ret, server_ip, server_port)
	gameLog("Reconnect["..client_id.."] result: ", conn_ret);
end
protected_.callbacks_.onDisconnect = function(client_id)
end
protected_.callbacks_.onHint = function(client_id, msg)
	local index = 2;
	local hint_type = msg[index];
	index = index + 1;
	local hint_value = msg[index];
	index = index + 1;
	local hint_params = {};
	local p = msg[index];
	while p do
		table.insert(hint_params, p);
		index = index + 1;
		p = msg[index];
	end
	if type(hint_value)=='number' then
		gameLog(hint_type, hint_value, unpack(hint_params));
	elseif type(hint_value)=='string' then
		local r, s = pcall(string.format, hint_value, unpack(hint_params));
		if not r then
			gameLog("formatLog hint_str="..hint_value.." error!");
			gameLog("formatLog Error: ["..s.."] error: "..debug.traceback());
		else
			gameLog(s);
		end
	end
end
protected_.callbacks_.onCreateChar = function(client_id)
    gameLog("Send: EC2SProtocol_CreateChar");
	local player_name = "test-" .. (protected_.client_beg_+client_id-1);
	doLuaCommand("createPlayer", client_id, player_name);
end
protected_.callbacks_.onCreateCharOk = function(client_id)
end
protected_.callbacks_.onUserdataOk = function(client_id)
	doLuaCommand("enterWorld", client_id);
	local client = protected_.clients_[client_id];
    gameLog("User login ", client.account_name_," OK");
end
protected_.callbacks_.onEnterWorld = function(client_id)
    gameLog("welcom to the world");
	--todo-- doLuaCommand("createTroop", client_id, 1);
end

protected_.callbacks_.onTroopInit = function(client_id, troop)
	gameLog("Troop init OK...");
	--todo-- doLuaCommand("matchTroop", client_id);
end

protected_.callbacks_.onTroopMerge = function(client_id, old_troop, troop)
	gameLog("Troop merge...");
end

protected_.callbacks_.onTroopInvite = function(client_id, player_name, invite_suid, invite_server_id, fight_sid)
	local fight_data = configs_.FightField[fight_sid];
	local name = fight_data and fight_data.name_ or "";
	gameLog("Troop invite: ", player_name, ",", invite_suid, ",", invite_server_id, ",", name);
    console_cmds_["joinTroop"]({client_id, invite_suid, 2, invite_server_id});
end

protected_.callbacks_.onTroopAdd = function(client_id, new_mem)
	local client = protected_.clients_[client_id];
	gameLog("JoinTroop: ", new_mem.player_name_, ",", client.cur_team_troop_.count_);
end

protected_.callbacks_.onTroopQuit = function(client_id)
	gameLog("Troop is dismissed");
end

protected_.callbacks_.onMatchFailed = function(client_id)
	gameLog("Troop matching is failed");
end


protected_.callbacks_.onMemberLeave = function(client_id, member)
	gameLog("Troop member", member.player_name_, "is leaved");
end


protected_.callbacks_.onSetLeader = function(client_id, leader_id)
	local client = protected_.clients_[client_id];
	local member = client.cur_team_troop_[leader_id];
	gameLog("SetLeader: ", member.player_name_);
end

protected_.callbacks_.onWaitMatch = function(client_id)
	gameLog("Troop wait match...");
end

protected_.callbacks_.onChooseBegin = function(client_id)
	gameLog("ChooseBegin.... ");
	--todo-- doLuaCommand("chooseHero", client_id, 2);
	--todo-- doLuaCommand("choosePet", client_id, 1);
	--todo-- doLuaCommand("setMemPos", client_id, 1, 1, 1);
	--todo-- doLuaCommand("beginFight", client_id);
end

protected_.callbacks_.onChooseHero = function(client_id, member, hero_id, skin)
	gameLog("ChooseHero: ", member.player_name_, " ",  hero_id, ",", skin);
end

protected_.callbacks_.onChooseTrump = function(client_id, member, trump_id)
	gameLog("ChooseTrump: ", member.player_name_, " ",  trump_id);
end

protected_.callbacks_.onChooseHeroConfirm = function(client_id, member, hero_id, skin)
	gameLog("onChooseHeroConfirm: ", member.player_name_, ",", hero_id, ",", skin);
end

protected_.callbacks_.onChooseHeroCancel = function(client_id, member, hero_id)
	gameLog("onChooseHeroCancel: ", member.player_name_, ",", hero_id);
end

protected_.callbacks_.onChoosePet = function(client_id, member, pet)
	if pet then
		gameLog("ChoosePet: ", member.player_name_, ",", pet.pet_sid_);
	else
		gameLog("Cancel choosePet: ", member.player_name_);
	end
end

protected_.callbacks_.onSetFormation = function(client_id, form_id)
	gameLog("SetFormation: ", form_id);
end

protected_.callbacks_.onSetMemPos = function(client_id, member, x, y)
	gameLog("SetMemPos: ", member.player_name_, "=>", x, ", ", y);
end

protected_.callbacks_.onConfirmFight = function(client_id, corp_id)
	gameLog("ConfirmFight: ", corp_id, "confirm fight");
end

protected_.callbacks_.onCreateFight = function(client_id)
    local client = protected_.clients_[client_id];
	--client.fight_.fight_worker_;
	doLuaCommand("clientBegin", client_id);
end

protected_.callbacks_.onPrepareRound = function(client_id, round_count, exp_time, player_attack_state)
	gameLog("=============================PrepareRound[", round_count, "]: ", (exp_time-getTime())/1000, "sec==================================");
    local client = protected_.clients_[client_id];
	local fight_worker = client.fight_.fight_worker_;
	if round_count<1 then
		return;
	end
	if fight_worker.my_unit_id_>0 then
--		local slot_id = protected_.chooseAISkill(fight_worker.my_unit_id_, fight_worker);
--		local choose_result = protected_.chooseAISkillTarget(fight_worker.my_unit_id_, fight_worker, slot_id);

--		doLuaCommand("setHeroAttack", client_id, slot_id, choose_result.corp_id, choose_result.target_slot_id);
 		if fight_worker.my_unit_id_==1 then
 			if round_count>=1 and round_count<2 then
 				doLuaCommand("setHeroAttack", client_id, 2);
 --			elseif round_count<3 then
 --				doLuaCommand("setHeroAttack", client_id, round_count);
 --			elseif round_count>=3 and round_count<5 then
 --				doLuaCommand("setHeroAttack", client_id, round_count+1);
 			else
 				doLuaCommand("setHeroAttack", client_id, 2);
 			end
 		else
 			doLuaCommand("setHeroAttack", client_id, 2);
 		end
	end
	if fight_worker.my_pet_id_>0 then
--		local slot_id = protected_.chooseAISkill(fight_worker.my_pet_id_, fight_worker);
--		local choose_result = protected_.chooseAISkillTarget(fight_worker.my_pet_id_, fight_worker, slot_id);

--		doLuaCommand("setPetAttack", client_id, slot_id, choose_result.corp_id, choose_result.target_slot_id);
		 if round_count<3 then
		 	doLuaCommand("setPetAttack", client_id, 2);
		 else
		 	doLuaCommand("setPetAttack", client_id, 2);
		 end
	end
end


protected_.callbacks_.onClientLoad = function(client_id, player_suid, load_rate)
	gameLog("Player["..player_suid.."] load "..load_rate.."%...");
end

protected_.callbacks_.onFightBegin = function(client_id)
    gameLog("战斗开始...");
end

protected_.callbacks_.onReadyRoundBegin = function(client_id)
	gameLog(" 准备回合开始...");
end

protected_.callbacks_.onRoundBegin = function(client_id, round_count)
	gameLog("==========================第"..round_count.."回合开始=======================================");
end

protected_.callbacks_.onReadyRoundEnd = function(client_id)
	gameLog(" 准备回合结束...");
end

protected_.callbacks_.onRoundEnd = function(client_id, round_count)
	gameLog("### 第"..round_count.."回合结束...");
end

protected_.callbacks_.onUnitShift = function(client_id, unit)
	gameLog("  "..protected_.getUnitName(client_id, unit).."变身了");
end

protected_.callbacks_.onUnitBegin = function(client_id, unit)
	gameLog("  "..protected_.getUnitName(client_id, unit).."开始行动了");
end

protected_.callbacks_.attack_fails = {
[500005]="发起者死亡",
[500006]="发起者休息",
[500007]="发起者无法普攻",
[500008]="发起者无法技能",
[500009]="目标距离过远",
[500010]="目标不符合条件",
[500011]="怒气不足",
[500012]="不满足释放条件",
[500101]="无法复活",
[10292]="状态未命中",
[10293]="不能上DEBUFF",
}
protected_.callbacks_.onAttackFailed = function(client_id, unit, skill_sid, reason)
	local reason_str = protected_.callbacks_.attack_fails[reason];
	if reason_str==nil then
		reason_str = protected_.callbacks_.attack_fails[500100];
	end
    gameLog("  "..protected_.getUnitName(client_id, unit).."攻击失败："..reason_str);
end

protected_.callbacks_.onStateFailed = function(client_id, unit, state_id, reason)
    local reason_str = protected_.callbacks_.attack_fails[reason];
	if reason_str==nil then
		reason_str = protected_.callbacks_.attack_fails[10292];
	end
    gameLog("  "..protected_.getUnitName(client_id, unit).."攻击状态："..reason_str);
end

protected_.callbacks_.onSwapSlot = function(client, corp_id, slot_id1, slot_id2)
	gameLog("  "..corp_id.."阵营的"..slot_id1.."与"..slot_id2.."交换了位置");
end

protected_.callbacks_.onSkillStateAdd = function(client_id, unit, state)
	local state_name = state.state_sid_;
	local state_cfg = configs_.SkillStateCfg[state.state_sid_];
	if state_cfg and state_cfg.name_ then
		--state_name = state_cfg.name_;
	end
	gameLog("   "..protected_.getUnitName(client_id, unit).."被施加["..state_name.."]状态"..state.left_round_.."回合");
end

protected_.callbacks_.onSkillStateDel = function(client_id, unit, state)
	local state_name = state.state_sid_;
	local state_cfg = configs_.SkillStateCfg[state.state_sid_];
	if state_cfg and state_cfg.name_ then
		--state_name = state_cfg.name_;
	end
	gameLog("   "..protected_.getUnitName(client_id, unit).."状态["..state_name.."]结束了");
end

protected_.callbacks_.onSlotStateAdd = function(client_id, corp_id_, slot_id_, state)
	local state_name = state.state_sid_;
	local state_cfg = configs_.SkillStateCfg[state.state_sid_];
	if state_cfg and state_cfg.name_ then
		--state_name = state_cfg.name_;
	end
	gameLog("   "..corp_id_.."阵营"..slot_id_.."地格被施加了["..state_name.."]状态"..state.left_round_.."回合");
end

protected_.callbacks_.onSlotStateDel = function(client_id, corp_id_, slot_id_, state)
	local state_name = state.state_sid_;
	local state_cfg = configs_.SkillStateCfg[state.state_sid_];
	if state_cfg and state_cfg.name_ then
		--state_name = state_cfg.name_;
	end
	gameLog("   "..corp_id_.."阵营"..slot_id_.."地格的["..state_name.."]状态结束了");
end

protected_.callbacks_.onUnitPropChange = function(client_id, unit, prop_name, cur_val, d_val, old_val)
	gameLog("    "..protected_.getUnitName(client_id, unit).."属性["..prop_name.."]="..old_val.."("..d_val..")", cur_val);
end

protected_.callbacks_.onAttackBegin = function(client_id, unit, skill_sid, attack_count, targets, dest_corp, dest_slot)
	local skill_name = skill_sid;
	local skill_data = configs_.SkillCfg[skill_sid];
	if skill_data and skill_data.name_ then
		-- skill_name = skill_data.name_;
	end
	local client = protected_.clients_[client_id];
	local fight_worker = client.fight_.fight_worker_;
	if #targets==0 and dest_slot>0 then
		gameLog("   "..protected_.getUnitName(client_id, unit).."向"..dest_corp.."阵营的"..dest_slot.."位置发动技能 "..skill_name);
	else
		if #targets==0 then
			gameLog("   "..protected_.getUnitName(client_id, unit).."发动技能 "..skill_name);
		elseif #targets==1 then
			local dest_unit_id=targets[1];
			if dest_unit_id==unit.unit_id_ then
				gameLog("   "..protected_.getUnitName(client_id, unit).."发动技能 "..skill_name);
			else
				dest_unit = fight_worker[dest_unit_id];
                if dest_unit ~= nil then
                    gameLog("   "..protected_.getUnitName(client_id, unit).."向"..protected_.getUnitName(client_id, dest_unit).."目标发动技能 "..skill_name);
                end
				
			end
		else
			for _, dest_unit_id in ipairs(targets) do
				local dest_unit = fight_worker[dest_unit_id];
				gameLog("   "..protected_.getUnitName(client_id, unit).."向"..protected_.getUnitName(client_id, dest_unit).."目标发动技能 "..skill_name);
			end
		end
	end
	--gameLog("onAttackBegin: ", getTime(), os.clock());
end

protected_.callbacks_.onUnitProtect = function(client_id, target_pre_id, target_id)
	local client = protected_.clients_[client_id];
	local fight_worker = client.fight_.fight_worker_;
	local target_pre = fight_worker[target_pre_id];
	local target = fight_worker[target_id];

	gameLog("    "..protected_.getUnitName(client_id, target).."援护了"..protected_.getUnitName(client_id, target_pre));
end

protected_.callbacks_.onDamage = function(client_id, damage)
	-- damage.src_slot,  damage.stage_id, damage.skill_state
	-- if damage.skill_state then damage.skill_state.state_sid_ end
	if damage.damage_result==Enum.ESkillResult_MISSING then
		gameLog("    "..protected_.getUnitName(client_id,  damage.dest_unit).."闪避了");
	elseif damage.damage_result==Enum.ESkillResult_CRITICAL then
		gameLog("    "..protected_.getUnitName(client_id,  damage.dest_unit).."被暴击了");
    elseif damage.damage_result==Enum.ESkillResult_BLOCK then
		gameLog("    "..protected_.getUnitName(client_id,  damage.dest_unit).."被格挡了");
	end
    if damage.damage_p > 0 then
        gameLog("    "..protected_.getUnitName(client_id,  damage.dest_unit).."受到了"..damage.damage_p.."物理伤害");
    end
    if damage.damage_m > 0 then
        gameLog("    "..protected_.getUnitName(client_id,  damage.dest_unit).."受到了"..damage.damage_m.."魔法伤害");
    elseif damage.damage_m < 0 then
        gameLog("    "..protected_.getUnitName(client_id,  damage.dest_unit).."治愈"..damage.damage_m.."点生命");
    end
    if damage.damage_r > 0 then
        gameLog("    "..protected_.getUnitName(client_id,  damage.dest_unit).."受到了"..damage.damage_r.."纯粹伤害");
    end
	if damage.damage_sum>0 then
		gameLog("    "..protected_.getUnitName(client_id,  damage.dest_unit).."总共受到了"..damage.damage_sum.."伤害");
	elseif damage.damage_sum<0 then
		gameLog("    "..protected_.getUnitName(client_id,  damage.dest_unit).."被治愈了"..-damage.damage_sum.."点生命");
	end
	if damage.damage_back>0 then
		gameLog("    "..protected_.getUnitName(client_id, damage.src_unit).."被反击了"..damage.damage_back.."伤害");
	end
	if damage.absorb_hp>0 then
		gameLog("    "..protected_.getUnitName(client_id, damage.src_unit).."吸取了"..damage.absorb_hp.."点生命");
	end
    if damage.damage_immue > 0 then
        gameLog("    "..protected_.getUnitName(client_id, damage.dest_unit).."护盾抵挡了"..damage.damage_immue.."点生命");
    end
    if damage.back_dmg_immue > 0 then
        gameLog("    "..protected_.getUnitName(client_id, damage.src_unit).."护盾抵挡了"..damage.back_dmg_immue.."点生命");
    end
end

protected_.callbacks_.onAttackStages = function(client_id, attack, attack_stages)

	local client = protected_.clients_[client_id];
	local fight_worker = client.fight_.fight_worker_;
	local skill_cfg = configs_.SkillCfg[attack.skill_sid];
	local target_type = skill_cfg.target_type or Enum.ETargetType_Unit;

	local target_slot = attack.target_slot;
	local target = attack.target;

	for i, cur_stage in ipairs(attack_stages) do
		protected_.MetaFight.onClientRecall("onDamages", client, attack, target_slot, target, cur_stage.damages, cur_stage.trigger_damages);
		for ii, state_recall in ipairs (cur_stage.states) do
			protected_.MetaFight.onClientRecall(unpack(state_recall));
		end
	end
end

protected_.callbacks_.onDamages = function(client_id, attack, target, target_slot, damages, trigger_damages)
	local t_s = target_slot or {};
	gameLog("    onDamages...");
	for i, v in ipairs(damages) do
		if v.damage_result==Enum.ESkillResult_MISSING then
			gameLog("    "..protected_.getUnitName(client_id, v.dest_unit).."闪避了");
		elseif v.damage_result==Enum.ESkillResult_CRITICAL then
			gameLog("    "..protected_.getUnitName(client_id, v.dest_unit).."被暴击了");
        elseif v.damage_result==Enum.ESkillResult_BLOCK then
		    gameLog("    "..protected_.getUnitName(client_id,  v.dest_unit).."被格挡了");
		end
        if v.damage_p > 0 then
        gameLog("    "..protected_.getUnitName(client_id, v.dest_unit).."受到了"..v.damage_p.."物理伤害");
        end
        if v.damage_m > 0 then
            gameLog("    "..protected_.getUnitName(client_id, v.dest_unit).."受到了"..v.damage_m.."魔法伤害");
        elseif v.damage_m < 0 then
            gameLog("    "..protected_.getUnitName(client_id, v.dest_unit).."治愈"..v.damage_m.."点生命");
        end
        if v.damage_r > 0 then
            gameLog("    "..protected_.getUnitName(client_id, v.dest_unit).."受到了"..v.damage_r.."纯粹伤害");
        end
		if v.damage_sum>0 then
			gameLog("    "..protected_.getUnitName(client_id, v.dest_unit).."受到了"..v.damage_sum.."伤害");
		elseif v.damage_sum<0 then
			gameLog("    "..protected_.getUnitName(client_id, v.dest_unit).."被治愈了"..-v.damage_sum.."点生命");
		end
		if v.damage_back>0 then
			gameLog("    "..protected_.getUnitName(client_id, v.src_unit).."被反击了"..v.damage_back.."伤害");
		end
		if v.absorb_hp>0 then
			gameLog("    "..protected_.getUnitName(client_id, v.src_unit).."吸取了"..v.absorb_hp.."点生命");
		end
        if v.damage_immue > 0 then
            gameLog("    "..protected_.getUnitName(client_id, v.dest_unit).."护盾抵挡了"..v.damage_immue.."点生命");
        end
        if v.back_dmg_immue > 0 then
            gameLog("    "..protected_.getUnitName(client_id, v.src_unit).."护盾抵挡了"..v.back_dmg_immue.."点生命");
        end
	end
	gameLog("    trigger_damages...");
	for i, v in ipairs(trigger_damages) do
		if v.damage_result==Enum.ESkillResult_MISSING then
			gameLog("    "..protected_.getUnitName(client_id, v.dest_unit).."闪避了");
		elseif v.damage_result==Enum.ESkillResult_CRITICAL then
			gameLog("    "..protected_.getUnitName(client_id, v.dest_unit).."被暴击了");
        elseif v.damage_result==Enum.ESkillResult_BLOCK then
		    gameLog("    "..protected_.getUnitName(client_id,  v.dest_unit).."被格挡了");
		end
         if v.damage_p > 0 then
        gameLog("    "..protected_.getUnitName(client_id, v.dest_unit).."受到了"..v.damage_p.."物理伤害");
        end
        if v.damage_m > 0 then
            gameLog("    "..protected_.getUnitName(client_id, v.dest_unit).."受到了"..v.damage_m.."魔法伤害");
        elseif v.damage_m < 0 then
            gameLog("    "..protected_.getUnitName(client_id, v.dest_unit).."治愈"..v.damage_m.."点生命");
        end
        if v.damage_r > 0 then
            gameLog("    "..protected_.getUnitName(client_id, v.dest_unit).."受到了"..v.damage_r.."纯粹伤害");
        end
		if v.damage_sum>0 then
			gameLog("    "..protected_.getUnitName(client_id, v.dest_unit).."受到了"..v.damage_sum.."伤害");
		elseif v.damage_sum<0 then
			gameLog("    "..protected_.getUnitName(client_id, v.dest_unit).."被治愈了"..-v.damage_sum.."点生命");
		end
		if v.damage_back>0 then
			gameLog("    "..protected_.getUnitName(client_id, v.src_unit).."被反击了"..v.damage_back.."伤害");
		end
		if v.absorb_hp>0 then
			gameLog("    "..protected_.getUnitName(client_id, v.src_unit).."吸取了"..v.absorb_hp.."点生命");
		end
        if v.damage_immue > 0 then
            gameLog("    "..protected_.getUnitName(client_id, v.dest_unit).."护盾抵挡了"..v.damage_immue.."点生命");
        end
        if v.back_dmg_immue > 0 then
            gameLog("    "..protected_.getUnitName(client_id, v.src_unit).."护盾抵挡了"..v.back_dmg_immue.."点生命");
        end
	end
	--gameLog("onDamages: ", getTime(), os.clock());
end

protected_.callbacks_.onGoBack = function(client_id, unit)
	gameLog("   "..protected_.getUnitName(client_id, unit).."返回");
	--gameLog("onGoBack: ", getTime(), os.clock());
end

protected_.callbacks_.onAttackEnd = function(client_id, attack_unit_id, src_slot, attack_id, skill_id)
	gameLog("   攻击结束...");
	--gameLog("onAttackEnd: ", getTime(), os.clock());
end

protected_.callbacks_.onDead = function(client_id, unit)
	if protected_.isHero(unit.unit_sid_) then
		gameLog("   "..protected_.getUnitName(client_id, unit).."死了...");
	else
		gameLog("   "..protected_.getUnitName(client_id, unit).."逃跑了...");
	end
end
protected_.callbacks_.onAddUnit = function(client_id, unit)
	gameLog("   "..protected_.getUnitName(client_id, unit).."进场了...");
end
protected_.callbacks_.onDelUnit = function(client_id, unit, del_mode)
	gameLog("   "..protected_.getUnitName(client_id, unit).."出场了...");
end
protected_.callbacks_.onRevive = function(client_id, unit)
	gameLog("   "..protected_.getUnitName(client_id, unit).."复活了...");
end

protected_.callbacks_.onUnitUnDead = function(client_id, unit)
   gameLog("   "..protected_.getUnitName(client_id, unit).."免死...");
end

protected_.callbacks_.onSlotChange = function(client_id, unit, corp_id, slot_id)
	gameLog("   没实现，闹鬼了...");
end

protected_.callbacks_.onUnitEnd = function(client_id, unit)
	gameLog("  "..protected_.getUnitName(client_id, unit).."行动结束了");
end
protected_.callbacks_.onFightOverData = function(client_id, fight_result, data)
	gameLog(" onFightOverData...");
end
protected_.callbacks_.onFightResult = function(client_id, fight_result, statistics)
	local client = protected_.clients_[client_id];
	local fight_worker = client.fight_.fight_worker_;
	if fight_result<0 then
		gameLog(" 这是一场意外的结局，平局...");
	elseif client.fight_.my_corp_id_==fight_result then
		gameLog(" 我方胜利了...");
	elseif client.fight_.my_corp_id_>=0 then
		gameLog(" 我方失败了...");
	else
		if fight_result==0 then
			gameLog(" 0阵营获得了胜利...");
		elseif fight_result==1 then
			gameLog(" 1阵营获得了胜利...");
		else
			gameLog(" 这是一场意外的结局，平局...");
		end
	end
	gameLog("战斗结束了...");
end

protected_.callbacks_.onFightEnd = function(client_id)
	gameLog("Fight end....");
end

protected_.callbacks_.onSetAttack = function(client_id, unit, skill_sid, target_id, corp_id, slot_id)
	local skill_name = skill_sid;
	local skill_data = configs_.SkillCfg[skill_sid];
	if skill_data and skill_data.name_ then
		--skill_name = skill_data.name_ or skill_sid;
	end
	local client = protected_.clients_[client_id];
	local fight_worker = client.fight_.fight_worker_;
	if target_id>0 then
		local target = fight_worker[target_id];
		gameLog(" "..protected_.getUnitName(client_id, unit).."向"..protected_.getUnitName(client_id, target).."发动了技能 "..skill_name);
	elseif slot_id>0 then
		gameLog(" "..protected_.getUnitName(client_id, unit).."向"..corp_id.."阵营的"..slot_id.."位置".."发动了技能 "..skill_name);
	else
		gameLog(" "..protected_.getUnitName(client_id, unit).."发动了技能 "..skill_name);
	end
end

protected_.callbacks_.onClientClose = function(client_id)
	gameLog("ClientClose");
end

protected_.callbacks_.onThumbsUp = function(client_id,target_suid,count,src_hero_id)
	gameLog("onThumbsUp "..target_suid.."count "..count.."hero_id"..src_hero_id);
end

protected_.callbacks_.onSummonPet = function(client_id, pet_sids)
	gameLog(" client_id  SummonPet ");
	--pet_sids = {pet_sid, pet_sid, ...}
	-- printObject(pet_sids);
end
protected_.callbacks_.onBuyHero = function(client_id, hero_id)
	gameLog(" client_id  onBuyHero ", hero_id);
end
protected_.callbacks_.petTroopChange = function(client_id)
	gameLog(" client_id  petTroopChange ");
end
protected_.callbacks_.onSellPet = function(client_id, pet_id)
	gameLog(" client_id  onSellPet " , pet_id);
end
protected_.callbacks_.onCommander = function(client_id)
	gameLog(" client_id  onCommander " );
end
protected_.callbacks_.onMarkUnit = function(client_id)
	gameLog(" client_id  onMarkUnit " );
end

protected_.callbacks_.onProProtect = function (client_id, pro_type, src_id, pro_id)
    local client = protected_.clients_[client_id];
	local fight_worker = client.fight_.fight_worker_;
	local src_uint = fight_worker[src_id];
	local pro_uint = fight_worker[pro_id];
    if pro_type == 0 then
        gameLog("    "..protected_.getUnitName(client_id, pro_uint).."主动援护了"..protected_.getUnitName(client_id, src_uint).."开始");
    elseif pro_type == 1 then
        gameLog("    "..protected_.getUnitName(client_id, pro_uint).."主动援护了"..protected_.getUnitName(client_id, src_uint).."结束");
    end
    
end
protected_.callbacks_.onConfirmPetTroop = function(client_id, player_suid)
	gameLog(" client_id  onConfirmPetTroop ", player_suid);
end
protected_.callbacks_.onMoneyChange = function(client_id, money_type)
	gameLog(" client_id  onMoneyChange ", client_id, money_type);
end
protected_.callbacks_.onAttackerQTE = function(client_id, unit, qte_sid, attack_id)
    local client = protected_.clients_[client_id];
	local fight_worker = client.fight_.fight_worker_;
	local qte_cfg = configs_.QTECfg[qte_sid];
	if unit.suid_==client.suid_ then
		doLuaCommand("QTECommit", client_id, unit.unit_id_, 1);
	end
end
protected_.callbacks_.onTargetQTE = function(client_id, unit, qte_sid)
    local client = protected_.clients_[client_id];
	local fight_worker = client.fight_.fight_worker_;
	if unit.suid_==client.suid_ then
		doLuaCommand("QTECommit", client_id, unit.unit_id_, 2);
	end
end

protected_.callbacks_.onMatchFailed = function (client_id, fight_id)
    gameLog(" 匹配失败 ");
end

protected_.callbacks_.onJoinChannel = function(client_id, channel_id, ret_val)
	print("onJoinChannel: ", channel_id, ret_val);
end
protected_.callbacks_.onLeaveChannel = function(client_id, channel_id)
	print("onLeaveChannel: ", channel_id);
end
protected_.callbacks_.onChatInChannel = function(client_id, channel_id, chat_player, chat_str)
	print("onChatInChannel: ", chat_player.player_name, chat_player.suid, channel_id, chat_str);
end
protected_.callbacks_.onChatInTroop = function(client_id, channel_id, chat_player, chat_str)
	print("onChatInTroop: ", chat_player.player_name, chat_player.suid, channel_id, chat_str);
end
protected_.callbacks_.onChatToPlayer = function(client_id, channel_id, chat_player, chat_str)
	print("onChatToPlayer: ", chat_player.player_name, chat_player.suid, channel_id, chat_str);
end
protected_.callbacks_.onChatToRoom = function(client_id, channel_id, chat_player, chat_str)
	print("onChatToRoom: ", chat_player.player_name, chat_player.suid, channel_id, chat_str);
end
protected_.callbacks_.onResolvePet = function(client_id)
	print("  onResolvePet ");
end
protected_.callbacks_.onRefreshProp = function(client_id)
	print("  onRefreshProp ");
end
protected_.callbacks_.onReplaceProp = function(client_id)
	print("  onReplaceProp ");
end
protected_.callbacks_.onLearnSkill = function(client_id)
	print("  onLearnSkill ");
end
protected_.callbacks_.onSellItem = function(client_id, get_cmd)
	print("  onSellItem ", dump_obj(get_cmd));
end
protected_.callbacks_.onSellItemBySid = function(client_id, get_cmd)
	print("  onSellItemBySid ", dump_obj(get_cmd));
end
protected_.callbacks_.onGetThing = function(client_id, get_cmd)
	print("  onGetThing "); 
	-- printObject(get_cmd)
end
protected_.callbacks_.onResponseError = function(client_id, get_cmd)
	print("  onResponseError "); 
end
protected_.callbacks_.onUseItem = function(client_id, get_cmd)
	print("  onUseItem "); 
end
protected_.callbacks_.onBuyItem = function(client_id, context)
	print("  onBuyItem " , dump_obj(context)); 
end
protected_.callbacks_.onLottery = function(client_id, context)
	print("  onLottery " , dump_obj(context)); 
end
protected_.callbacks_.onInstallPosy = function(client_id)
	print("  onInstallPosy "); 
end
protected_.callbacks_.onUninstallPosy = function(client_id)
	print("  onUninstallPosy "); 
end
protected_.callbacks_.onUninstallAllPosy = function(client_id)
	print("  onUninstallAllPosy "); 
end
protected_.callbacks_.onUnlockProgram = function(client_id)
	print("  onUnlockProgram "); 
end
protected_.callbacks_.onUnitOrder = function(client_id)
	--print(" <<<<<<<< onUnitOrder >>>>>>>>>"); 
	local client = protected_.clients_[client_id];
	local fight_worker = client.fight_.fight_worker_;
	--printObject(fight_worker.unit_order_);
	--print(" =========== ", protected_.getTableLen(fight_worker.unit_order_))
	--printObject(fight_worker.action_end_unit_);
	--print( "----------", protected_.getTableLen(fight_worker.action_end_unit_))
end
protected_.callbacks_.onFetchSeasonReward = function(client_id, context)
	print("  onFetchSeasonReward " , dump_obj(context)); 
end
protected_.callbacks_.onFetchSeasonReward = function(client_id)
	print("  onProgramSetName " ); 
end

protected_.callbacks_.onDialogOpen = function(client_id, dialog_sid, dialog_context, field_id, unit_id)
	local dialog_data = configs_.Dialog[dialog_sid];
	if dialog_data==nil then
		return ;
	end
	print("onDialogOpen["..dialog_sid.."]: ", dialog_data.content_, field_id, unit_id);
end

protected_.callbacks_.onDialogClose = function(client_id, dialog_sid)
	local dialog_data = configs_.Dialog[dialog_sid];
	if dialog_data==nil then
		return ;
	end
	print("onDialogClose["..dialog_sid.."]: ");
end
protected_.callbacks_.onItemChange = function(client_id)
	print(" onItemChange ");
end

protected_.callbacks_.onFieldCounterInit = function(client_id, fight_data)
	local client = protected_.clients_[client_id];
	local corp_id = client.fight_.my_corp_id_+1;
	local counter_stage = fight_data.stages[client.fight_.stage_];
	if counter_stage==nil then
		return ;
	end
	local counters = counter_stage[corp_id];
	if  counters==nil then
		return ;
	end
	for i, v in ipairs(counters) do
		print("onFieldCounterInit [", v[1][1],",",v[1][2] or 0,",",v[1][3] or 0, "]: 0/"..v[2]);
	end
end

protected_.callbacks_.onFieldCounter = function(client_id, counter_indx, counter_tbl)
	local client = protected_.clients_[client_id];
	local counter = counter_tbl[2];
	print("onFieldCounter [", counter_indx, "]: "..counter_tbl[1].."/"..counter);
end

protected_.callbacks_.onFieldCounterClose = function(client_id)
	local client = protected_.clients_[client_id];
	print("onFieldCounterClose ... ");
end

protected_.callbacks_.onWaitIntoMatch = function(client_id, troop1, troop2)
	local client = protected_.clients_[client_id];
	print("Has matched! ... ");
   
    console_cmds_["readyMatch"]({client_id});
    
end

protected_.callbacks_.onReadyToMatch = function(client_id, troop, member)
	local client = protected_.clients_[client_id];
    print(member.player_name_,"has ready.....");
	--doLuaCommand("readyMatch", client_id, 1);
end

protected_.callbacks_.onFailedReady = function (client_id)
    gameLog(" 等待准备失败 ");
end

protected_.callbacks_.onStopMatch = function (client_id)
    gameLog("Stop Match!.......");
end
protected_.callbacks_.onConfirmPosyProgram = function (client_id, program_id)
    gameLog("onConfirmPosyProgram ", program_id);
end
protected_.callbacks_.onSysChatTroop = function (client_id, msg)
	gameLog("onSysChatTroop ");
	printObject(msg);
end
protected_.callbacks_.onUnlockPetTroop = function (client_id, msg)
	gameLog("onUnlockPetTroop ", msg);
end
protected_.callbacks_.onPetTroopSetName = function (client_id, msg)
	gameLog("onPetTroopSetName ", msg);
end