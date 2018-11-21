if loads_["UnitHeader"]==nil then
	dofile(script_path_.."UnitHeader.lua")
	loads_["UnitHeader"]=true;
end

protected_.MetaSkillState.createState = function(unit, msg, index)
	local state_id = msg[index];
	index = index + 1;
	local state_sid = msg[index];
	index = index + 1;
	if unit==nil then
		gameError("protected_.MetaSkillState.createState "..state_sid.." unit is nil");
	end
	local state = protected_.constructObject(protected_.MetaSkillState, nil, unit.states_, state_id);
	state.state_id_ = state_id;
	state.state_sid_ = state_sid
	state.left_round_ = msg[index];
	index = index + 1;
	state.src_unit_ = msg[index];
	index = index + 1;
	if state.left_round_ == -1 then
		state.end_round_ = -1;
	else
		state.end_round_ = unit.dirtys_.parent_.round_count_ + state.left_round_;
	end
--	state.src_skill_sid_ = msg[index];
--	index = index + 1;
--	state.state_param_ = msg[index];
--	index = index + 1;
	unit.states_[state.state_id_]=state;
	table.insert(unit.states_list_, state.state_id_);
	return index, state;
end
protected_.MetaSkillState.removeState = function(unit, msg, index)
	local state_id = msg[index];
	index = index + 1;
	local state = unit.states_[state_id];
	unit.states_[state_id]=nil;
	for k,v in ipairs(unit.states_list_) do
		if v==state_id then
			table.remove(unit.states_list_, k);
			break;
		end
	end
	return index, state;
end

protected_.MetaUnit.getUnitDisplay = function(self)
	local same_chk = configs_.display_shows_[self.unit_sid_];
	if self.cur_unit_sid_==self.unit_sid_ 
		or (same_chk and same_chk[self.cur_unit_sid_]) then
		return {self.cur_unit_sid_, self.display_id_ or 0};
	end
	return {self.cur_unit_sid_, 0};
end

protected_.MetaUnit.createUnit = function(client, msg, index)
	local unit_id = msg[index];
	index = index + 1;
	local unit = protected_.constructObject(protected_.MetaUnit, nil, client.fight_.fight_worker_, unit_id);
	unit.unit_id_ = unit_id;
	unit.unit_data_sid_ = msg[index]; --cur_unit_sid_ display_id_
	index = index + 1;
	unit.suid_ = msg[index];
	index = index + 1;
	unit.pet_indx_ = msg[index];
	index = index + 1;
	unit.dead_flag = msg[index];
	index = index + 1;
	if unit.dead_flag~=0 then
		unit.dead_flag = true;
	else
		unit.dead_flag = false;
	end
	unit.corp_id_ = msg[index];
	index = index + 1;
	unit.slot_id_ = msg[index];
	index = index + 1;
	local prop_count = msg[index];
	index = index + 1;
	for i=1, prop_count do
		local prop_indx = msg[index];
		index = index + 1;
		unit.base_[prop_indx+1] = msg[index];
		index = index + 1;
	end
	local state_count = msg[index];
	index = index + 1;
	unit.skills_ = {};
	unit.skill_lists_ = {};
	unit.skill_raw_lists_ = {};
	unit.states_ = {};
	unit.states_list_ = {};
	for i=1, state_count do
		index = protected_.MetaSkillState.createState(unit, msg, index);
	end
	local skill_count = msg[index];
	index = index + 1;
	for i=1, skill_count do
		local skill_raw_id = msg[index];
		index = index + 1;
--		if skill_raw_id>0 then
--			print("**skill_raw_id** ", skill_raw_id);
--		end
		local skill_data = configs_.SkillCfg[skill_raw_id];
		if skill_data and skill_data.cast_type~=Enum.ECastType_Passive then
			unit.skill_lists_[i]=skill_raw_id;
		else
			unit.skill_lists_[i]=0;
		end
		--print(unit_id, "**skilllist** ", i, unit.skill_lists_[i]);
		unit.skill_raw_lists_[i] = skill_raw_id;
		unit.skills_[skill_raw_id] = {slot=i};
	end
	if skill_count<Enum.ESkill_MAX then
		for i=skill_count+1, Enum.ESkill_MAX do
			unit.skill_lists_[i]=0;
			--print(unit_id, "**skilllist** ", i, unit.skill_lists_[i]);
		end
	end
	for i,v in ipairs(configs_.base_skills_) do
		if protected_.isHero(unit.unit_sid_) or v~=1600102 then
			unit.skill_lists_[Enum.ESkill_MAX+i]=v;
			unit.skills_[v] = {slot=Enum.ESkill_MAX+i};
		else
			unit.skill_lists_[Enum.ESkill_MAX+i]=0;
		end
	end
	unit.owner_name_ = "";
	if unit.suid_>0 then
		local troop_id;
		if unit.corp_id_==0 then
			troop_id = client.fight_.troop1_;
		else
			troop_id = client.fight_.troop2_;
		end
		local troop = client.troops_[troop_id];
		if troop then
			local unit_owner = troop[unit.suid_];
			if unit_owner then
				unit.raw_owner_name_ = unit_owner.player_name_;
				if protected_.isHero(unit.unit_sid_) then
					unit.owner_name_ = unit_owner.player_name_;
				elseif protected_.isMaskUnit(unit.cur_unit_sid_) then
					local own_name = "";
					local unit_data = configs_.UnitCfg[unit.unit_sid_];
					if unit_data then
						own_name = unit_data.name_ or "not exist!!!!!!!";
					end
					unit.owner_name_ = own_name;
				else
					local own_name = "";
					local unit_data = configs_.UnitCfg[unit.cur_unit_sid_];
					if unit_data then
						own_name = unit_data.name_ or "not exist!!!!!!!";
					end
					unit.owner_name_ = own_name;
				end
			end
		end
	end

	-- print("unit skills", unit.unit_sid_, unit.hero_sid_, #unit.skill_lists_, unit.skill_lists_[1], unit.skill_lists_[2]);
	return index, unit;
end
protected_.MetaUnit.getUnitSkillListObj = function(self)
	local skillListObj = {};
	skillListObj.skill_={};
	for i=1,Enum.ESkill_EP-1 do
		if self.skill_lists_[i]>0 then
			table.insert(skillListObj.skill_, self.skill_lists_[i]);
		end
	end
	if self.skill_lists_[Enum.ESkill_EP]>0 then
		skillListObj.skill_ep = self.skill_lists_[Enum.ESkill_EP];
	end
	if self.skill_lists_[Enum.ESkill_MAX]>0 then
		skillListObj.skill_friend = self.skill_lists_[Enum.ESkill_MAX];
	end
	return skillListObj;
end
