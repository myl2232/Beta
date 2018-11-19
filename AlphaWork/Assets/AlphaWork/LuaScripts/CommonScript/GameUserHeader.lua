--[[********************************************************************
	created:	2018/01/19
	author:		lixuguang_cx

	purpose:	
*********************************************************************--]]
protected_.registMetaProp(protected_.MetaGamePlayer, "player_name_", "");
protected_.registMetaProp(protected_.MetaGamePlayer, "account_name_", "");
protected_.registMetaProp(protected_.MetaGamePlayer, "player_id_", 0, 1); -- 只同步
protected_.registMetaProp(protected_.MetaGamePlayer, "fight_server_", 0);
protected_.registMetaProp(protected_.MetaGamePlayer, "fight_sid_", 0);
protected_.registMetaProp(protected_.MetaGamePlayer, "create_time_", 0);
protected_.registMetaProp(protected_.MetaGamePlayer, "player_level_", 1);
protected_.registMetaProp(protected_.MetaGamePlayer, "alliance_id_", 0);
protected_.registMetaProp(protected_.MetaGamePlayer, "exp_", 0);
protected_.registMetaProp(protected_.MetaGamePlayer, "match_score_", 0);	--隐藏匹配分
protected_.registMetaProp(protected_.MetaGamePlayer, "money_", 0);   --资源  金币
protected_.registMetaProp(protected_.MetaGamePlayer, "quests_", nil, nil, protected_.MetaQuestBag);
protected_.registMetaProp(protected_.MetaGamePlayer, "pet_tbag_", nil, nil, protected_.MetaPetTBag);
protected_.registMetaProp(protected_.MetaGamePlayer, "guide_dialog_", 0);--新手引导记录
protected_.registMetaProp(protected_.MetaGamePlayer, "dialog_contexts_", nil, nil, protected_.MetaMap);--对话上下文记录


protected_.registExProp(protected_.MetaGamePlayer, "pet_bag_", protected_.MetaPetBag, protected_.saveUserCall);
protected_.registExProp(protected_.MetaGamePlayer, "hero_bag_", protected_.MetaHeroBag, protected_.saveUserCall);
protected_.registExProp(protected_.MetaGamePlayer, "alliances_", protected_.MetaAlliances, protected_.saveUserCall);

protected_.registMetaProp(protected_.MetaGamePlayer, "summon_sys_", nil, nil, protected_.MetaSummonSys);
protected_.registMetaProp(protected_.MetaGamePlayer, "friend_marks_", nil, nil, protected_.MetaMap);	--预设战场标记 友方 指令  id = 标记内容
protected_.registMetaProp(protected_.MetaGamePlayer, "enemy_marks_", nil, nil, protected_.MetaMap);		--预设战场标记 敌方 指令 
protected_.registMetaProp(protected_.MetaGamePlayer, "item_bag_", nil, nil, protected_.MetaItemBag);	--道具背包
protected_.registMetaProp(protected_.MetaGamePlayer, "soul_stone_", 0); --资源 魂石
protected_.registMetaProp(protected_.MetaGamePlayer, "trump_bag_", nil, nil, protected_.MetaTrumpBag); --法宝
protected_.registMetaProp(protected_.MetaGamePlayer, "hero_trump_map_",nil, nil, protected_.MetaMap);
protected_.registMetaProp(protected_.MetaGamePlayer, "lottery_sys_",nil, nil, protected_.MetaLotterySys);	--抽奖
protected_.registMetaProp(protected_.MetaGamePlayer, "fight_win_result_", nil, nil, protected_.MetaFightWins);--匹配胜率
protected_.registMetaProp(protected_.MetaGamePlayer, "posy_sys_",nil, nil, protected_.MetaPosySys);	--美食铭文
protected_.registMetaProp(protected_.MetaGamePlayer, "spirit_chip_", 0);	--资源 元神碎片 用于购买美食铭文
protected_.registMetaProp(protected_.MetaGamePlayer, "ofuda_", 0);	--资源 符箓
protected_.registMetaProp(protected_.MetaGamePlayer, "gain_rate_sys_",nil, nil, protected_.MetaGainRateSys);	--奖励倍率
protected_.registMetaProp(protected_.MetaGamePlayer, "hero_pet_troop_",nil, nil, protected_.MetaMap);	--英雄选择神兽部队存储
protected_.registMetaProp(protected_.MetaGamePlayer, "pet_money_", 0);	--资源 驭兽符
protected_.registMetaProp(protected_.MetaGamePlayer, "rank_score_", 0);	--隐藏排位分

protected_.registExProp(protected_.MetaGamePlayer, "player_season_", protected_.MetaSeason, protected_.saveUserCall);--赛季
protected_.registMetaProp(protected_.MetaGamePlayer, "troop_server_", 0);
protected_.registMetaProp(protected_.MetaGamePlayer, "match_server_", 0);
protected_.registMetaProp(protected_.MetaGamePlayer, "forbid_time_", 0);

protected_.registMetaProp(protected_.MetaGamePlayer, "reward_level_", 0);


protected_.MetaGamePlayer.getUID = function(self)
	return self.player_id_;
end

protected_.MetaGamePlayer.isFirstRankFight = function(self)
	local rank_fight = self.fight_win_result_[Enum.EFightType_Ranking];
	if not rank_fight or rank_fight.fight_total_num_ <= 0 then
		return true;
	end
	return false;
end