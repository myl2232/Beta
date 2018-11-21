


protected_.registMetaProp(protected_.MetaSeason, "dan_grading_", 1);	--段位 默认段位1
protected_.registMetaProp(protected_.MetaSeason, "dan_grading_star_", 0);	--段位星数
protected_.registMetaProp(protected_.MetaSeason, "reward_score_", 0);	--排位战斗奖励积分
protected_.registMetaProp(protected_.MetaSeason, "win_num_", 0);	--赛季胜利次数
protected_.registMetaProp(protected_.MetaSeason, "balance_time_", 0);	--结算时间
protected_.registMetaProp(protected_.MetaSeason, "balance_season_", 0);	--结算赛季
protected_.registMetaProp(protected_.MetaSeason, "reward_state_", nil, nil, protected_.MetaMap);	--赛季奖励领取状态 {}
protected_.registMetaProp(protected_.MetaSeason, "mail_reward_", nil, nil, protected_.MetaMap);	--赛季结算未领取 邮件发送


protected_.registMetaProp(protected_.MetaFightResult, "fight_star_", 0);  --1胜败的加减星
protected_.registMetaProp(protected_.MetaFightResult, "extra_star_", 0);  --2奖励分额外加星
protected_.registMetaProp(protected_.MetaFightResult, "reward_scores_", nil);  --3各项表现分
protected_.registMetaProp(protected_.MetaFightResult, "add_reward_score_", 0);  --4表现分+的奖励分
protected_.registMetaProp(protected_.MetaFightResult, "sub_reward_score1_", 0);  --5奖励分计算，会加星时，扣除的奖励分
protected_.registMetaProp(protected_.MetaFightResult, "sub_reward_score2_", 0);  --6触发奖励分保护时，扣除奖励分 
protected_.registMetaProp(protected_.MetaFightResult, "new_dan_", 0);  --7计算后的最新的段位
protected_.registMetaProp(protected_.MetaFightResult, "old_dan_", 0);  --8
protected_.registMetaProp(protected_.MetaFightResult, "new_dan_star_", 0);  --9计算后的最新的星星
protected_.registMetaProp(protected_.MetaFightResult, "old_dan_star_", 0);  --10
protected_.registMetaProp(protected_.MetaFightResult, "old_reward_score_", 0);  --11老的奖励分
protected_.registMetaProp(protected_.MetaFightResult, "new_reward_score_", 0);  --12新的奖励分数
protected_.registMetaProp(protected_.MetaFightResult, "fight_medal_", 0);  --13战斗的奖牌

protected_.MetaSeason.getSeasonAllData = function()
    return configs_.SeasonData;
end
protected_.MetaSeason.getDansData = function()
    return protected_.MetaSeason.getSeasonAllData().dan_data;
end
protected_.MetaSeason.getDanData = function(dan_grade)
    local dan_data = protected_.MetaSeason.getDansData()[dan_grade];
    if not dan_data then
        log.error("MetaSeason.getDanData", nil, "dan_data is nil  dan_grade=", dan_grade);
        print(debug.traceback());
    end
    return dan_data;
end

--降级保护 保护分触发  1.没有配置保护，2.降级状态，3.奖励分足够
protected_.MetaSeason.isDegradedScore = function(dan_grading, dan_grading_star, reward_score)
    local cur_dan_data = protected_.MetaSeason.getDanData(dan_grading);
    reward_score = reward_score or 0;
	if not protected_.MetaSeason.isFallStarProtect(dan_grading) and protected_.MetaSeason.isDegradedState(dan_grading, dan_grading_star) and reward_score >= cur_dan_data.reward_score then
		return true;
	end
	return false;
end
--降星保护 段位配置触发
protected_.MetaSeason.isFallStarProtect = function(dan_grading)
	local cur_dan_data = protected_.MetaSeason.getDanData(dan_grading);
	if cur_dan_data.fall_star_protect == 1 then
		return true;
	end
	return false;
end
--降级状态  当前0星
protected_.MetaSeason.isDegradedState = function(dan_grading, dan_grading_star)
	local cur_dan_data = protected_.MetaSeason.getDanData(dan_grading);
	if dan_grading_star == 0 then
		return true;
	end
	return false;
end
--排位战斗结果星星段位处理
protected_.MetaSeason.danFightResult = function(self, fight_data)
    if fight_data.fight_type ~= Enum.EFightType_Ranking then
		return ;
    end
	local cur_dan_data = protected_.MetaSeason.getDanData(self.dan_grading_);
	if not cur_dan_data then
		return ;
	end
    local results = {};
	local win_add_star = 1;
	local lose_minus_star = 1;
    local star = 0;
    local fight_star = 0;
    local sub_score1 = 0;
    local sub_score2 = 0;
    local extra_star = 0;
    local old_dan = self.dan_grading_;
    local old_dan_star = self.dan_grading_star_;
    local old_reward_score = self.reward_score_;
    
    local score = self.reward_score_;
	-- 1. 胜败
	if fight_data.win_flag then
        star = star + win_add_star;
        fight_star = win_add_star;
	else
        star = star - lose_minus_star;
        fight_star = -lose_minus_star;
        if protected_.MetaSeason.isFallStarProtect(old_dan) then
            fight_star = 0;
        elseif protected_.MetaSeason.isDegradedScore(old_dan, old_dan_star, score) then
            --触发奖励分，降级保护
			star = star + lose_minus_star;
            sub_score2 = cur_dan_data.reward_score;
            score = score - sub_score2;
            fight_star = 0;
        end
    end
    
    -- 3.表现积分  score
	local add_score, scores = self:getFightRewardScore(fight_data);
	score = score + add_score;
	local diff_score = score - cur_dan_data.reward_score_max;
	if diff_score > 0 then
		extra_star = math.floor(diff_score / cur_dan_data.reward_score) + 1;
        star = star + extra_star;
        sub_score1 = extra_star * cur_dan_data.reward_score;
		score = score - sub_score1;
	end

    local new_dan, new_star = self:addDanStar(star);
    local result = protected_.constructObject(protected_.MetaFightResult, nil, nil, nil);
    if fight_star ~= 0 then
        result.fight_star_ = fight_star;
    end
    if extra_star ~= 0 then
        result.extra_star_ = extra_star;
    end
    if scores ~= nil then
        result.reward_scores_ = scores;
    end
    if add_score ~= 0 then
        result.add_reward_score_ = add_score;
    end
    if sub_score1 ~= 0 then
        result.sub_reward_score1_ = sub_score1;
    end
    if sub_score2 ~= 0 then
        result.sub_reward_score2_ = sub_score2;
    end
    
    if new_dan ~= 0 then
        result.new_dan_ = new_dan;
    end
    
    if new_star ~= 0 then
        result.new_dan_star_ = new_star;
    end
    
    if score ~= 0 then
        result.new_reward_score_ = score;
    end
    if old_reward_score ~= 0 then
        result.old_reward_score_ = old_reward_score;
    end
    result.old_dan_ = old_dan;
    if old_dan_star ~= 0 then
        result.old_dan_star_ = old_dan_star;
    end
    -- printObject(result);
    return result;
end
--获取排位赛， 战后奖励分
protected_.MetaSeason.getFightRewardScore = function(self, fight_data)
    local score = 0;
    local scores = {};
    local player = self.dirtys_.parent_;
	--1.无挂机，2.局内评分排名，3.连胜，4.对手过强，5.虽败犹荣
	local reward_score_data = protected_.MetaSeason.getDansData().reward_score_data;
	if not reward_score_data then
		return score;
    end
    local add_score = 0;
	--1.无挂机
	if fight_data.no_operation < reward_score_data.hang_condition then
        add_score = reward_score_data.no_hang;
        scores[1] = add_score;
        score = score + add_score;
	end
	--2.局内评分排名
	add_score = reward_score_data.troop_rank[fight_data.score_rank] or reward_score_data.troop_rank.default or 0 ;
    scores[2] = add_score;
    score = score + add_score;

	--3.连胜
	local series_win_times = player.fight_win_result_[fight_data.fight_type].fight_series_win_;
	if series_win_times > 0 then
        add_score = reward_score_data.series_win[series_win_times] or reward_score_data.series_win.default or 0 ;
        scores[3] = add_score;
        score = score + add_score;
	end
	--4.对手过强
	local enemy_corp_id = 0;
	if fight_data.corp_id == enemy_corp_id then
		enemy_corp_id = 1;
	end
	local enemy_capacity = fight_data.fight_capacity[enemy_corp_id] or 0;
	local friend_capacity = fight_data.fight_capacity[fight_data.corp_id] or 0;
	local diff_capacity = enemy_capacity - friend_capacity;
	local diff_lv = math.floor(diff_capacity / reward_score_data.enemy_strong_unit);
	if diff_lv > 0 then
        add_score = reward_score_data.enemy_strong_class[diff_lv] or reward_score_data.enemy_strong_class.default or 0 ;
        scores[4] = add_score;
        score = score + add_score;
	end
	--5.虽败犹荣
	if not fight_data.win_flag then
		local lv = lowerBound(reward_score_data.lose_scope, fight_data.score);
		if fight_data.score >= reward_score_data.lose_scope(#reward_score_data.lose_scope) then
			lv = #reward_score_data.lose_scope;
		end
        add_score = reward_score_data.lose_class[lv] or reward_score_data.lose_class.default or 0 ;
        scores[5] = add_score;
        score = score + add_score;
	end
	return score, scores;
end
--添加排位星星，  add_star < 0是扣星星 (降段位最多为一个)    return: 新段位，新星星
protected_.MetaSeason.addDanStar = function(self, star)
	local cur_dan_data = protected_.MetaSeason.getDanData(self.dan_grading_);
	local dans_data = protected_.MetaSeason.getDansData();
	local add_star = self.dan_grading_star_ + star; 
	local add_dan_grad = 0;
	if add_star < 0 then
		--降段
		if protected_.MetaSeason.isFallStarProtect(self.dan_grading_) then --有保护
			add_star = 0;
		else
			local max_minus_star = dans_data.max_minus_star or 1;
			if -add_star > max_minus_star then
				add_star = -max_minus_star;
			end
			local drop_dan_data = protected_.MetaSeason.getDanData(self.dan_grading_ - 1);
			if -add_star > drop_dan_data.star_max then
				add_star = -drop_dan_data.star_max;
			end
			add_star = drop_dan_data.star_max + add_star;
			add_dan_grad = -1;
		end
	elseif add_star > 0 then
		-- 加星
		local add_dan_grading = self.dan_grading_;
		local next_dan_data = protected_.MetaSeason.getDanData( add_dan_grading + 1 );
		while(next_dan_data and cur_dan_data.star_max and add_star > cur_dan_data.star_max) do
			add_star = add_star - cur_dan_data.star_max;
			add_dan_grad = add_dan_grad + 1;
			add_dan_grading = add_dan_grading + 1;
			cur_dan_data = protected_.MetaSeason.getDanData( add_dan_grading );
		end
	end
    -- self:changeDanStar(self.dan_grading_ + add_dan_grad, add_star);
    return self.dan_grading_ + add_dan_grad, add_star;
end


--获取当前段位0星时的总星星
protected_.MetaSeason.getTotalStar = function(dan_grade)
    local dans_data = protected_.MetaSeason.getDansData();
    local cur_dan_data = dans_data[dan_grade];
    if type(cur_dan_data.total_star) == "number" then
        return cur_dan_data.total_star;
    end
    local total = 0;
    for i = 1, dan_grade-1 do
        local dan_data = dans_data[i];
        dan_data.total_star = total;
        total = total + dan_data.star_max;
    end
    return total;
end
protected_.MetaSeason.getSeasonsData = function()
    return protected_.MetaSeason.getSeasonAllData().season;
end
protected_.MetaSeason.getSeasonData = function(season_id)
    -- local seasons_data = protected_.MetaSeason.getSeasonsData();
    -- local season_data = seasons_data[season_id];
    local season_data = configs_.SeasonCfg[season_id];
    if not season_data then
        log.error("MetaSeason.getSeasonData", nil, "season_data is nil  season_id=", season_id);
        print(debug.traceback());
    end
    return season_data;
end
protected_.MetaSeason.getBalanceSeasonId = function()
    local season_data = protected_.MetaSeason.getSeasonsData();
    return season_data.last_season;
end
protected_.MetaSeason.getCurSeasonId = function()
    local season_data = protected_.MetaSeason.getSeasonsData();
    return season_data.cur_season;
end
--赛季结算段位
protected_.MetaSeason.danBalanceFunc = function(dan_grading, dan_grading_star, season_id)
    local new_dan = dan_grading;
    local new_star = dan_grading_star;
    local season_data = protected_.MetaSeason.getSeasonData(season_id);
    if not season_data then
        return new_dan, new_star;
    end
    
    local drop_dan_rule = season_data.drop_dan;
    if not drop_dan_rule then
        return new_dan, new_star;
    end

    local dan_drop_rule = drop_dan_rule[dan_grading];
    if not dan_drop_rule then
        return new_dan, new_star;
    end
    local star_scope = dan_drop_rule.star_scope;
    local star_scope_id = 1;
    if star_scope then
        if dan_grading_star >= star_scope[#star_scope] then
            star_scope_id = #star_scope;
        else
            star_scope_id = lowerBound(star_scope, dan_grading_star);
        end
    end
    local star_scope_data = dan_drop_rule[star_scope_id];
    if not star_scope_data then
        return new_dan, new_star;
    end
    new_dan = star_scope_data[1];
    new_star = star_scope_data[2];
    return new_dan, new_star;
end
protected_.MetaSeason.isAutoDegrade = function(dan_grading)
    local dans_data = protected_.MetaSeason.getDansData();
    local min_auto_sub_star_dan = dans_data.auto_sub_star_dan or #dans_data;
    if dan_grading >= min_auto_sub_star_dan then
        return true;
    end
    return ;
end
protected_.MetaSeason.getAutoSubStarTime = function()
    local dans_data = protected_.MetaSeason.getDansData();
    return dans_data.auto_sub_star_time or 0;
end
protected_.MetaSeason.autoSubStar = function(cur_dan, cur_star)
    local new_dan = cur_dan;
    local new_star = cur_star;
    if not protected_.MetaSeason.isAutoDegrade(new_dan) then
        return new_dan, new_star;
    end
    new_star = new_star - 1;
    if new_star < 0 then
        new_dan = new_dan - 1;
        local new_dan_data = protected_.MetaSeason.getDanData(new_dan);
        new_star = new_dan_data.star_max - 1;
    end
    return new_dan, new_star;
end
protected_.MetaSeason.getCurSeasonRewards = function()
    local season_data = protected_.MetaSeason.getSeasonData(protected_.MetaSeason.getCurSeasonId());
    local rewards = season_data.reward
    if not rewards then
        log.error("MetaSeason.getCurSeasonRewards", nil, "rewards is nil  season_id=", protected_.MetaSeason.getCurSeasonId());
        print(debug.traceback());
    end
    return rewards;
end
protected_.MetaSeason.getBalanceReward = function(self, season_id)
    local season_data = protected_.MetaSeason.getSeasonData(season_id);
    local rt_reward = {};
    local reward = season_data.reward;
    if not reward then
        return rt_reward;
    end
    for k, v in pairs(reward) do
        local reward_t = self:getRewardId(v);
        if reward_t then
            rt_reward[k] = reward_t.reward_id;
        end
    end
    return rt_reward;
end
protected_.MetaSeason.getRewardId = function(self, reward_data)
    local rt = {};
    local flag = true;
    local reward_t;
    local reward_index = 999;
    if reward_data.cond_type then
        for k, v in ipairs(reward_data.cond_type) do
            local func = protected_.getSeasonReward[v];
            if func then
                local index = func(self, reward_data, k);
                if index > 0 then
                    rt[k] = true;
                    if index < reward_index then
                        reward_index = index;
                    end
                else
                    flag = false;
                    rt[k] = false;
                end
            end
        end
        if flag then
            reward_t = reward_data[reward_index];
        end
    end
    return reward_t, rt;
end
protected_.MetaSeason.isFetchOpt = function(reward_data)
    if type(reward_data) ~= "table" then
        return false;
    end
    if reward_data.balance_flag and reward_data.balance_flag > 0 then
        return false;
    end
    return true;
end
--奖励领取状态
protected_.MetaSeason.fetchState = function(self, key)
    if self.reward_state_[key] and self.reward_state_[key] > 0 then
        return true;
    end
    return false;
end
--前端使用
protected_.MetaSeason.getAllRewardState = function(self, season_id)
    local rt_reward = {};
    local season_data = protected_.MetaSeason.getSeasonData(season_id);
    local rt_reward = {};
    local reward = season_data.reward;
    if not reward then
        return rt_reward;
    end
    for k, v in pairs(reward) do
        local reward_t, can_fetch = self:getRewardId(v);
        if not reward_t then
            reward_t = v[1];
        end
        local state = self:fetchState(k);
        table.insert(rt_reward, {reward_t, state, can_fetch});
    end
    return rt_reward;
end