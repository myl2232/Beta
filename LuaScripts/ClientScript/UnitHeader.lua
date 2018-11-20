

protected_.registMetaProp(protected_.MetaUnitSkill, "skill_index_", 0);
protected_.registMetaProp(protected_.MetaUnitSkill, "skill_sid_", 0);
protected_.registMetaProp(protected_.MetaUnitSkill, "level_", 0);

protected_.registMetaProp(protected_.MetaSkillState, "state_id_", 0);
protected_.registMetaProp(protected_.MetaSkillState, "state_sid_", 0);
protected_.registMetaProp(protected_.MetaSkillState, "left_round_", 0);
protected_.registMetaProp(protected_.MetaSkillState, "src_unit_", 0);
protected_.registMetaProp(protected_.MetaSkillState, "end_round_", 0);

local prop_indx = 0;
local prop_name = PropIndex[prop_indx];
--注意prop_indx从0开始的，protected_.MetaUnit的属性从1开始的，存在1的错位，同步数据时小心
while prop_name do
	protected_.registMetaProp(protected_.MetaUnit, prop_name, 0);
	prop_indx = prop_indx + 1;
	prop_name = PropIndex[prop_indx];
end

protected_.registMetaProp(protected_.MetaUnit, "states_", nil, nil, protected_.MetaMap);

--unit.unit_id_		--本场战斗中 战斗单位实例id
--unit.unit_sid_	--本战斗单元 的英雄id; unit_data_sid_与unit_sid_是一个
--unit.cur_unit_sid_	--本战斗单元 变身后的英雄id，如果protected_.isMaskUnit(unit.cur_unit_sid_)，需要读取unit_sid_对应的英雄技能，否则读取此英雄id的技能
--unit.suid_		--玩家的suid， 通过server_id和player_id获得
--unit.corp_id_		--阵营id
--unit.slot_id_		--战斗单元 站位
--unit.skills_