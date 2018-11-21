--[[********************************************************************
	created:	2018/01/19
	author:		lixuguang_cx

	purpose:	
*********************************************************************--]]
protected_.MetaQuest.EQuest_None = 0;	-- 空
protected_.MetaQuest.EQuest_acceptable = 1; -- 已经在手了，等待领取
protected_.MetaQuest.EQuest_accepted = 2; -- 已领取 正在做任务
protected_.MetaQuest.EQuest_Finish = 3;	-- 完成，未获取奖励
protected_.MetaQuest.EQuest_Fetched = 4;	-- 以获取奖励
protected_.registMetaProp(protected_.MetaQuest, "quest_sid_", 0);
protected_.registMetaProp(protected_.MetaQuest, "state_", 0);
protected_.registMetaProp(protected_.MetaQuest, "counter_", nil, nil, protected_.MetaMap)
protected_.registMetaProp(protected_.MetaQuest, "player_level_", 0) --当前任务所属等级


------------------------------------QuestGroup--------------------------------------------------
protected_.registMetaProp(protected_.MetaQuestGroup, "group_id_", 0);
protected_.registMetaProp(protected_.MetaQuestGroup, "end_time_", 0);	
protected_.registMetaProp(protected_.MetaQuestGroup, "next_ref_time_", 0); 
protected_.registMetaProp(protected_.MetaQuestGroup, "root_", nil, nil, protected_.MetaMap);
protected_.registMetaProp(protected_.MetaQuestGroup, "circle_", 0);
protected_.registMetaProp(protected_.MetaQuestGroup, "nodes_", 1)
protected_.registMetaMap(protected_.MetaQuestGroup, 0, nil, protected_.MetaQuest)

------------------------------------QuestBag--------------------------------------------------
protected_.registMetaMap(protected_.MetaQuestBag, 0, nil, protected_.MetaQuestGroup)

protected_.MetaQuestBag.getCfgData = function(group_id, quest_sid)
    gameAssert(group_id, "get questgroup cfg : group_id is invalid")
    local group = configs_.QuestGroup
    local content = "return "..group[group_id].path
    local cfg = loadstring(content)()
    if quest_sid then
        gameAssert(cfg.quest[quest_sid])
        return cfg.quest[quest_sid]
    end
    return cfg
end
