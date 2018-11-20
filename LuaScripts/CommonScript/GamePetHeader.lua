--[[********************************************************************
	created:	2018/01/19
	author:		lixuguang_cx

	purpose:	
*********************************************************************--]]
protected_.registMetaProp(protected_.MetaPet, "pet_id_", 0);
protected_.registMetaProp(protected_.MetaPet, "pet_sid_", 0);
protected_.registMetaProp(protected_.MetaPet, "level_", 1);
protected_.registMetaProp(protected_.MetaPet, "exp_", 0);
protected_.registMetaProp(protected_.MetaPet, "skill1_", 0);
protected_.registMetaProp(protected_.MetaPet, "skill2_", 0);
protected_.registMetaProp(protected_.MetaPet, "skill3_", 0);
protected_.registMetaProp(protected_.MetaPet, "troop_slot_", nil, nil, protected_.MetaMap); --存放pet 在pet troop中的位置 {troop_id = slot_id, ...}
protected_.registMetaProp(protected_.MetaPet, "awake_state_", 0); --神兽觉醒状态 0未觉醒，1觉醒
protected_.registMetaProp(protected_.MetaPet, "prop_", nil, nil, protected_.MetaMap);	--神兽洗练神兽洗练
protected_.registMetaProp(protected_.MetaPet, "get_time_", 0); --获得时间


----------------------------------------PetBag----------------------------------------

protected_.registMetaProp(protected_.MetaPetBag, "pet_num_", 0);
protected_.registMetaProp(protected_.MetaPetBag, "add_iter_", 1, 2); --添加pet时的pet_id迭代, 2只存盘不同步
protected_.registMetaMap(protected_.MetaPetBag, 0, nil, protected_.MetaPet)
protected_.registMetaProp(protected_.MetaPetBag, "pet_types_", nil, nil, protected_.MetaMap); --用于存放已拥有的神兽类型
protected_.registMetaProp(protected_.MetaPetBag, "temp_prop_", nil, nil, protected_.MetaMap); --用于缓存刷新出的属性
protected_.registMetaProp(protected_.MetaPetBag, "temp_pet_", 0);	--用于缓存刷新的神兽id ，当该神兽拆解等，需要重置

---------------------------------------PetTroopBag------------------------------------

protected_.registMetaMap(protected_.MetaPetTBag, 0, nil, protected_.MetaPetTroop)
protected_.registMetaProp(protected_.MetaPetTBag, "pay_num_", 0)

protected_.registMetaMap(protected_.MetaPetTroop, 0)
protected_.registMetaProp(protected_.MetaPetTroop, "id_", 0);
protected_.registMetaProp(protected_.MetaPetTroop, "name_", nil);
protected_.registMetaProp(protected_.MetaPetTroop, "lock_", 0);

--protected_.MetaPetTroop
protected_.MetaPetTroop.getMaxSlotId = function()
	return configs_.pet_troop_slot_num;
end
--pet_id是否可以放入部队
protected_.MetaPetTroop.isCanAdd = function(self, pet_id, slot_id)
	return true;
end
protected_.MetaPetTBag.isCanUse = function(self, troop_id)
	if self[troop_id] and self[troop_id].lock_ > 0 then
		return true;
	end
	return false;
end
protected_.MetaPetTBag.isCanPay = function(self, troop_id)
	if self:isCanUse(troop_id) then
		return false;
	end
	if self.pay_num_ >= configs_.pay_pet_troop then
		return false;
	end 
	return true;
end

--protected_.MetaPetBag
protected_.MetaPetBag.getPetData = function(pet_sid)
	if not configs_.UnitCfg[pet_sid] or not protected_.isPet(pet_sid) then
		log.warn("MetaPetBag.getPetData", nil, "pet_sid error pet_sid=", pet_sid);
		return ;
	end
	return configs_.UnitCfg[pet_sid];
end
protected_.MetaPetBag.getRefreshData = function()
	return configs_.PetRefresh;
end

protected_.MetaPetBag.getSkillProp = function(slot_id)
	if type(slot_id) == "number" and slot_id > 0 and slot_id <= protected_.MetaPetBag.getSkillSlotMax() then
		return "skill" .. slot_id .. "_";
	end
	log.warn("MetaPetBag.getSkillProp", nil, "slot_id error");
	return nil;
end
protected_.MetaPetBag.getSkillSlotMax = function()
	return 3;
end
--检查学习技能参数
protected_.MetaPetBag.checkLearnSkillParams = function(self, pet_id, params)
	local player = self.dirtys_.parent_;
	local pet = self[pet_id];
	if not pet then
		log.warn("MetaPetBag.checkLearnSkillParams", player, "pet is nil  pet_id=", pet_id);
		return false;
	end
	if type(params) ~= "table" or not next(params) then
		log.warn("MetaPetBag.checkLearnSkillParams", player, "params error");
		return false;
	end
	local pet_data = protected_.MetaPetBag.getPetData(pet.pet_sid_);
	local exclude_skills = pet_data.exclude_skills_ or {};
	local pet_skill_group = {};  --神兽已经学习 存储技能组  group_id = {品质, 槽位}
	for i = 1, protected_.MetaPetBag.getSkillSlotMax() do
		local skill_id = pet[protected_.MetaPetBag.getSkillProp(i)];
		if skill_id ~= 0 then
			local skill_data = protected_.getSkillData(skill_id);
			if not skill_data then
				return false;
			end
			pet_skill_group[skill_data.skill_group_id] = {skill_data.skill_group_quality, i, skill_id};
		end
	end

	local params_skill_group = {}; --将要学习的 存储技能组  group_id = {品质, 槽位}
	for slot_id, item_sid in pairs(params) do
		local skill_prop = protected_.MetaPetBag.getSkillProp(slot_id);
		if not skill_prop then
			log.warn("MetaPetBag.checkLearnSkillParams", player, "slot_id error  slot_id=", slot_id);
			return false;
		end
		--判断是否满足装配条件 同组的技能只能装配一个，只能使用同组更高级或不同组的技能替换当前槽位技能
		local item_data = protected_.MetaItemBag.getItemData(item_sid);
		if item_data.item_type ~= Enum.ItemType_Skill then
			log.warn("MetaPetBag.checkLearnSkillParams", player, "item item_type error  item_sid=", item_sid);
			return false;
		end
		local skill_data = protected_.getSkillData(item_data.skill_id);
		if not skill_data then	
			log.warn("MetaPetBag.checkLearnSkillParams", player, "skill_data is nil  skill_id=", item_data.skill_id);
			return false;
		end
		--神兽排除技能
		if exclude_skills[item_sid] then
			log.warn("MetaPetBag.checkLearnSkillParams", player, "pet exclude_skills pet_sid=", pet.pet_sid_, "item_sid=", item_sid);
			return false;
		end
		--参数中同组判断
		if params_skill_group[skill_data.skill_group_id] then
			log.warn("MetaPetBag.checkLearnSkillParams", player, "params_skill_group is equal error  skill_id=", item_data.skill_id, "equal_skill_id=", params_skill_group[skill_data.skill_group_id][3]);
			log.warn("MetaPetBag.checkLearnSkillParams", player, "params", params);
			return false;
		end
		--神兽已装配技能 同组判断
		if pet_skill_group[skill_data.skill_group_id] then
			if pet_skill_group[skill_data.skill_group_id][2] ~= slot_id then
				log.warn("MetaPetBag.checkLearnSkillParams", player, "pet_skill_group is equal error  skill_id=", item_data.skill_id, "equal_slot_id=", pet_skill_group[skill_data.skill_group_id][2],  "equal_slot_skill_id=", pet_skill_group[skill_data.skill_group_id][3]);
				return false;
			else
				if pet_skill_group[skill_data.skill_group_id][1] >= skill_data.skill_group_quality then
					log.warn("MetaPetBag.checkLearnSkillParams", player, "skill_group_quality is low error slot_id=", slot_id, "quality=", skill_data.skill_group_quality, "pet_quality=", pet_skill_group[skill_data.skill_group_id][1]);
					return false;
				end
			end
		end
		--判断道具是否足够
		local item_ids = player.item_bag_.sid_index_[item_sid];
		if not item_ids or not next(item_ids) then
			log.warn("MetaPetBag.checkLearnSkillParams", player, "item_sid is 0  item_sid=", item_sid);
			return false;
		end
		params_skill_group[skill_data.skill_group_id] = {skill_data.skill_group_quality, slot_id, item_data.skill_id};
	end
	return true;
end
--获取属性的品质
protected_.MetaPetBag.getQuality = function(val)
	local refresh_data = protected_.MetaPetBag.getRefreshData();
	local scope_data = refresh_data.quality.scope;
	if val < scope_data[1] or val > scope_data[#scope_data] then
		log.warn("MetaPetBag.getQuality", nil, "val error  val =", val);
		return ;
	end
	local level, v = lowerBound(scope_data, val);
	if level > #refresh_data.quality then
		level = #refresh_data.quality;
	end
	return level;
end
--获取称号
protected_.MetaPetBag.getTitle = function(val)
	local refresh_data = protected_.MetaPetBag.getRefreshData();
	local scope_data = refresh_data.title.scope;
	if val < scope_data[1] or val > scope_data[#scope_data] then
		log.warn("MetaPetBag.getTitle", nil, "val error  val =", val);
		return ;
	end
	local level, v = lowerBound(scope_data, val);
	if level > #refresh_data.title then
		level = #refresh_data.title;
	end
	return level;
end

--根据属性名字如：attack_[EDamageType_Physical] 获取 洗练的id
protected_.MetaPet.getPropId = function(prop_name)
	local refresh_data = protected_.MetaPetBag.getRefreshData();
	if not refresh_data.temp_prop then
		refresh_data.temp_prop = {};
	end
	if refresh_data.temp_prop[prop_name] then
		return refresh_data.temp_prop[prop_name];
	end
	for prop_id, one_data in pairs(refresh_data.prop) do
		if one_data.effect_prop then
			for _, effect_prop_name in pairs(one_data.effect_prop) do
				refresh_data.temp_prop[effect_prop_name] = prop_id;
				if prop_name == effect_prop_name then
					return prop_id;
				end
			end
		end
	end
	return -1;
end
protected_.MetaPet.getPropRate = function(self, prop_name)
	local prop_id = protected_.MetaPet.getPropId(prop_name);
	local rate_val = self.prop_[prop_id];
	if not rate_val then
		return 1;
	end
	return rate_val / 10000;  --洗练随机出的值 / 10000
end