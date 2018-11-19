
PropIndex={};
PropDesc={};
local prop_v_=0;

PropIndex[prop_v_]="unit_sid_";
PropDesc.unit_sid_ = prop_v_;
prop_v_ = prop_v_+1;

PropIndex[prop_v_]="force_id_";
PropDesc.force_id_ = prop_v_;
prop_v_ = prop_v_+1;

PropIndex[prop_v_]="display_id_";
PropDesc.display_id_ = prop_v_;
prop_v_ = prop_v_+1;

PropIndex[prop_v_]="cur_unit_sid_";
PropDesc.cur_unit_sid_ = prop_v_;
prop_v_ = prop_v_+1;

PropIndex[prop_v_]="level_";
PropDesc.level_ = prop_v_;
prop_v_ = prop_v_+1;

PropIndex[prop_v_]="control_flag_";
PropDesc.control_flag_ = prop_v_;
prop_v_ = prop_v_+1;

PropIndex[prop_v_]="speed_";
PropDesc.speed_ = prop_v_;
prop_v_ = prop_v_+1;

PropIndex[prop_v_]="hp_max_";
PropDesc.hp_max_ = prop_v_;
prop_v_ = prop_v_+1;

PropIndex[prop_v_]="hp_";
PropDesc.hp_ = prop_v_;
prop_v_ = prop_v_+1;

PropIndex[prop_v_]="mp_max_";
PropDesc.mp_max_ = prop_v_;
prop_v_ = prop_v_+1;

PropIndex[prop_v_]="mp_";
PropDesc.mp_ = prop_v_;
prop_v_ = prop_v_+1;

PropIndex[prop_v_]="angry_max_";
PropDesc.angry_max_ = prop_v_;
prop_v_ = prop_v_+1;

PropIndex[prop_v_]="angry_";
PropDesc.angry_ = prop_v_;
prop_v_ = prop_v_+1;

PropIndex[prop_v_]="vampire_phy_";
PropDesc.vampire_phy_ = prop_v_;
prop_v_ = prop_v_+1;

PropIndex[prop_v_]="vampire_mgc_";
PropDesc.vampire_mgc_ = prop_v_;
prop_v_ = prop_v_+1;

PropIndex[prop_v_]="hp_recover_";
PropDesc.hp_recover_ = prop_v_;
prop_v_ = prop_v_+1;

PropIndex[prop_v_]="mp_recover_";
PropDesc.mp_recover_ = prop_v_;
prop_v_ = prop_v_+1;

PropIndex[prop_v_]="silence_rate_";
PropDesc.silence_rate_ = prop_v_;
prop_v_ = prop_v_+1;

PropIndex[prop_v_]="dodge_rate_";
PropDesc.dodge_rate_ = prop_v_;
prop_v_ = prop_v_+1;

PropIndex[prop_v_]="counteratk_rate_";
PropDesc.counteratk_rate_ = prop_v_;
prop_v_ = prop_v_+1;

PropIndex[prop_v_]="hp_atk_heal_";
PropDesc.hp_atk_heal_ = prop_v_;
prop_v_ = prop_v_+1;

PropIndex[prop_v_]="hp_atk_heal_rate_";
PropDesc.hp_atk_heal_rate_ = prop_v_;
prop_v_ = prop_v_+1;

PropIndex[prop_v_]="trigger_rate_";
PropDesc.trigger_rate_ = prop_v_;
prop_v_ = prop_v_+1;

PropIndex[prop_v_]="sex_";
PropDesc.sex_ = prop_v_;
prop_v_ = prop_v_+1;

PropIndex[prop_v_]="damage_back_";
PropDesc.damage_back_ = prop_v_;
prop_v_ = prop_v_+1;
									
PropIndex[prop_v_]="protect_count_";
PropDesc.protect_count_ = prop_v_;
prop_v_ = prop_v_+1;

PropIndex[prop_v_]="damage_min_";
PropDesc.damage_min_ = prop_v_;
prop_v_ = prop_v_+1;
									
PropIndex[prop_v_]="damage_max_";
PropDesc.damage_max_ = prop_v_;
prop_v_ = prop_v_+1;
									
PropIndex[prop_v_]="last_round_";
PropDesc.last_round_ = prop_v_;
prop_v_ = prop_v_+1;

PropIndex[prop_v_]="pet_id_";
PropDesc.pet_id_ = prop_v_;
prop_v_ = prop_v_+1;

PropIndex[prop_v_]="trump_skill_id_";
PropDesc.trump_skill_id_ = prop_v_;
prop_v_ = prop_v_+1;

PropIndex[prop_v_]="ai_control_";
PropDesc.ai_control_ = prop_v_;
prop_v_ = prop_v_+1;
 
skill_cd_={ };
for i=0, ESkill_MAX-1 do
	local n = "skill_cd_".."["..EnumDesc["ESkill"][i].."]";
	PropIndex[prop_v_]=n;
	PropDesc[n] = prop_v_;
	prop_v_ = prop_v_+1;
	skill_cd_[i] = n;
end

skill_lock_={ };
for i=0, ESkill_MAX-1 do
	local n = "skill_lock_".."["..EnumDesc["ESkill"][i].."]";
	PropIndex[prop_v_]=n;
	PropDesc[n] = prop_v_;
	prop_v_ = prop_v_+1;
	skill_lock_[i] = n;
end
	
atk_dmg_d_={ };
for i=0, EDamageType_MAX-1 do
	local n = "atk_dmg_d_".."["..EnumDesc["EDamageType"][i].."]";
	PropIndex[prop_v_]=n;
	PropDesc[n] = prop_v_;
	prop_v_ = prop_v_+1;
	atk_dmg_d_[i] = n;
end
			
def_dmg_d_={ };
for i=0, EDamageType_MAX-1 do
	local n = "def_dmg_d_".."["..EnumDesc["EDamageType"][i].."]";
	PropIndex[prop_v_]=n;
	PropDesc[n] = prop_v_;
	prop_v_ = prop_v_+1;
	def_dmg_d_[i] = n;
end
			
atk_dmg_p_={ };
for i=0, EDamageType_MAX-1 do
	local n = "atk_dmg_p_".."["..EnumDesc["EDamageType"][i].."]";
	PropIndex[prop_v_]=n;
	PropDesc[n] = prop_v_;
	prop_v_ = prop_v_+1;
	atk_dmg_p_[i] = n;
end
			
def_dmg_p_={ };
for i=0, EDamageType_MAX-1 do
	local n = "def_dmg_p_".."["..EnumDesc["EDamageType"][i].."]";
	PropIndex[prop_v_]=n;
	PropDesc[n] = prop_v_;
	prop_v_ = prop_v_+1;
	def_dmg_p_[i] = n;
end
			
attack_={ };
for i=0, EDamageType_MAX-1 do
	local n = "attack_".."["..EnumDesc["EDamageType"][i].."]";
	PropIndex[prop_v_]=n;
	PropDesc[n] = prop_v_;
	prop_v_ = prop_v_+1;
	attack_[i] = n;
end
				
defence_={ };
for i=0, EDamageType_MAX-1 do
	local n = "defence_".."["..EnumDesc["EDamageType"][i].."]";
	PropIndex[prop_v_]=n;
	PropDesc[n] = prop_v_;
	prop_v_ = prop_v_+1;
	defence_[i] = n;
end
				
crit_={ };
for i=0, EDamageType_MAX-1 do
	local n = "crit_".."["..EnumDesc["EDamageType"][i].."]";
	PropIndex[prop_v_]=n;
	PropDesc[n] = prop_v_;
	prop_v_ = prop_v_+1;
	crit_[i] = n;
end
				
anti_crit_={ };
for i=0, EDamageType_MAX-1 do
	local n = "anti_crit_".."["..EnumDesc["EDamageType"][i].."]";
	PropIndex[prop_v_]=n;
	PropDesc[n] = prop_v_;
	prop_v_ = prop_v_+1;
	anti_crit_[i] = n;
end
			
ddc_={ };
for i=0, EDamageType_MAX-1 do
	local n = "ddc_".."["..EnumDesc["EDamageType"][i].."]";
	PropIndex[prop_v_]=n;
	PropDesc[n] = prop_v_;
	prop_v_ = prop_v_+1;
	ddc_[i] = n;
end
					
anti_ddc_={ };
for i=0, EDamageType_MAX-1 do
	local n = "anti_ddc_".."["..EnumDesc["EDamageType"][i].."]";
	PropIndex[prop_v_]=n;
	PropDesc[n] = prop_v_;
	prop_v_ = prop_v_+1;
	anti_ddc_[i] = n;
end

dmg_handle_={ };
for i=0, EDamageType_MAX-1 do
	local n = "dmg_handle_".."["..EnumDesc["EDamageType"][i].."]";
	PropIndex[prop_v_]=n;
	PropDesc[n] = prop_v_;
	prop_v_ = prop_v_+1;
	dmg_handle_[i] = n;
end
 
block_={ };
for i=0, EDamageType_MAX-1 do
	local n = "block_".."["..EnumDesc["EDamageType"][i].."]";
	PropIndex[prop_v_]=n;
	PropDesc[n] = prop_v_;
	prop_v_ = prop_v_+1;
	block_[i] = n;
end

invisible_={ };
for i=0, EDamageType_MAX-1 do
	local n = "invisible_".."["..EnumDesc["EDamageType"][i].."]";
	PropIndex[prop_v_]=n;
	PropDesc[n] = prop_v_;
	prop_v_ = prop_v_+1;
	invisible_[i] = n;
end

antiinvisible_={ };
for i=0, EDamageType_MAX-1 do
	local n = "antiinvisible_".."["..EnumDesc["EDamageType"][i].."]";
	PropIndex[prop_v_]=n;
	PropDesc[n] = prop_v_;
	prop_v_ = prop_v_+1;
	antiinvisible_[i] = n;
end


 