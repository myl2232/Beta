
--权重随机获取id   参数：random_data = {{权重，其他信息...}, ...}
protected_.random_weight = function(random_data)
    if type(random_data) ~= "table" then
        return
    end
    if random_data.rands == nil then
        random_data = {rands = random_data};
    end
    if random_data.rand_total == nil then
        random_data.rand_total = 0;
        for i, v in ipairs(random_data.rands) do
            random_data.rand_total = random_data.rand_total + v[1];
        end
    end

    local r_weight = math.random() * random_data.rand_total
    local cur_weight = 0
    for i, v in ipairs(random_data.rands) do
        cur_weight = cur_weight + v[1];
        if r_weight <= cur_weight then
            return v;
        end
    end
end
protected_.getFightReward = {
    ["money_"] = function(fight_data, rate)
        local fight_time = fight_data.fight_time;
        local fight_sid = fight_data.fight_sid;
        local is_win = fight_data.win_flag;
        local score = fight_data.score;

        local k = configs_.Fight_Reward_Fail_K;
        local k2 = configs_.Fight_Reward_Fail_K2;
        if is_win then
            k = configs_.Fight_Reward_Win_K;
        end
        local t = math.floor(fight_time/60);
        -- local b1 = score * 5 * 2 / 7;
        -- local b1, x = math.modf(b1);
        -- if x >= 0.5 then
        -- 	b1 = b1 + 1;
        -- end
        -- local b2 = configs_.Fight_Reward_B2;
        local b = 0;  --表现分
        local member_num = configs_.FightField[fight_sid].troop_num_;
        local val = 0;
        if t >= configs_.Fight_Reward_Time then
            val = member_num * k * rate + b;
        else
            if is_win then
                val =  member_num * t * rate + b;
            else
                val =  k2 * member_num * t * rate + b;
            end
        end
        return math.floor(val);
    end,
    ["exp_"] = function(fight_data, rate)
        local fight_time = fight_data.fight_time;
        local fight_sid = fight_data.fight_sid;
        local is_win = fight_data.win_flag;
        local score = fight_data.score;
        rate = rate or 1;

        local k = configs_.Fight_Reward_Exp_Fail_K;
        if is_win then
            k = configs_.Fight_Reward_Exp_Win_K;
        end
        local t = math.floor(fight_time/60);
        local member_num = configs_.FightField[fight_sid].troop_num_;
        local val = 0;
        if t >= configs_.Fight_Reward_Time then
            val = configs_.Fight_Reward_Time * member_num * k * rate;
        else
            val = t * member_num * k * rate;
        end
        return math.floor( val );
    end, 
    ["pet_money_"] = function(fight_data, rate)
        local fight_time = fight_data.fight_time;
        local fight_sid = fight_data.fight_sid;
        local is_win = fight_data.win_flag;
        local score = fight_data.score;
        rate = rate or 1;

        local k1 = configs_.Fight_Reward_Pet_Fail_K;
        local k2 = configs_.Fight_Reward_Pet_Fail_K2;
        if is_win then
            k1 = configs_.Fight_Reward_Pet_Win_K;
            k2 = configs_.Fight_Reward_Pet_Win_K2;
        end

        local t = math.floor(fight_time/60);
        local member_num = configs_.FightField[fight_sid].troop_num_;
        local val = 0;
        if t >= configs_.Fight_Reward_Time then
            val = member_num * k1 * rate;
        else
            val = t * member_num * k2 * rate;
        end
        return math.floor( val );
    end,       
}

protected_.getSkillsData = function()
    return configs_.SkillCfg;
end
protected_.getSkillData = function(skill_id)
    local skill_data = protected_.getSkillsData()[skill_id];
    if not skill_data then
        log.warn("getSkillData", nil, "skill_data is nil  skill_id=", skill_id);
    end
    return skill_data;
end
protected_.getTableLen = function(t)
    if type(t) ~= "table" then
        return 0;
    end
    local len = 0;
    for k, v in pairs(t) do
        len = len + 1;
    end
    return len;
end

protected_.getRewardCmd = function(reward_id)
    return configs_.Reward[reward_id];
end

protected_.getHideScore = function(val, win_flag)
    if win_flag then
        return (1/(1 + 10^(val/400))) * configs_.hide_score_k;
    else
        return -(1 - 1/(1 + 10^(val/400))) * configs_.hide_score_k;
    end
end

protected_.getSeasonReward = {
    [Enum.ESeasonRewardCond_Dan] = function(player_season, reward_data, cond_index)
        local dan_grading = player_season.dan_grading_;
        if not reward_data.cond then
            return 0;
        end
        local cond = reward_data.cond[cond_index];
        if type(cond) ~= "number" then
            return 0;
        end
        if dan_grading < cond then
            return 0;
        end
        return #reward_data;
    end,
    [Enum.ESeasonRewardCond_Win] = function(player_season, reward_data, cond_index)
        local win_num = player_season.win_num_;
        if not reward_data.cond then
            return 0;
        end
        local cond = reward_data.cond[cond_index];
        if type(cond) ~= "number" then
            return 0;
        end
        if win_num < cond then
            return 0;
        end
        return #reward_data;
    end,   
    [Enum.ESeasonRewardCond_DanStep] = function(player_season, reward_data, cond_index)
        local dan_grading = player_season.dan_grading_;
        if not reward_data.cond then
            return 0;
        end
        local cond = reward_data.cond[cond_index];
        if type(cond) ~= "table" then
            return 0;
        end
        local index = lowerBound(cond, dan_grading) or 0;
        if index > #reward_data then
            index = #reward_data;
        end
        return index;
    end,   
}

protected_.time_str = function(m_timer)
    if not m_timer then
        return
    end
    m_timer =  m_timer / 1000
    return os.date("%Y-%m-%d %H:%M:%S", math.floor(m_timer))
end

protected_.getEventkey = function(counter)
	local quest_event = counter[1]
	local k_param = quest_event[2] or 0
    if not quest_event[1] then
		log.trace("protected_.getEventkey quest_event is invalid!!!!")
	end
	local ekey = protected_.getKey(quest_event[1], k_param)

    if not ekey then
        log.trace("protected_.getEventkey ekey is nil!!!!")
    end
	return ekey;
end

protected_.getKey = function(kk, k)
	return kk * 0x100000000 + k;
end
protected_.parseKey = function(key)
    local kk = math.floor(key / 0x100000000);
    local k = key % 0x100000000;
    return kk, k;
end
protected_.getFightType = function(fight_sid)
    local field_data = configs_.FightField[fight_sid];
    if field_data and field_data.rank_ then
        return Enum.EFightType_Ranking;
    end
    return Enum.EFightType_Random;
end
--获得职业奖牌
protected_.getFightMedal = function(fight_data)
    local unit_sid = fight_data.sunit_id;
    local unit_data = configs_.UnitCfg[unit_sid];
    local medal_id = 0;
    if not unit_data then
        return medal_id;
    end
    local profession = unit_data.profession_;
    local profession_score = configs_.profession_score.pass_data[profession];
    if not profession_score or not profession_score.pass_score or not configs_.profession_score.medal then
        return medal_id;
    end
    local pass_num = 0;
    for k, v in pairs(profession_score.pass_score) do
        if fight_data.statics_[k] >= v then
            pass_num = pass_num + 1;
        end
    end
    for k, v in ipairs(configs_.profession_score.medal) do
        if v.func and v.func(pass_num) then
            medal_id = k;
        end
    end
    return medal_id;
end

protected_.is_member = function(ta, member)
    if not ta or not member or not next(ta) then
        return false
    end

    if type(member) == "table" then
        return false
    end

    local length = protected_.getTableLen(ta) 
    if length >= 1000 then
        gameLog("warning : is_member() table lengh["..lengh.."] is too long!!!!!!!")
    end

    for _, v in pairs(ta) do
        if v == member then
            return true
        end
    end
    return false
end

--是否计算隐藏分
protected_.isCalcHideScore = function(fight_sid)
    local field_data = configs_.FightField[fight_sid];
    if not field_data then
        return false;
    end
    if field_data.pve_ then
        return false;
    end
    return true;
end