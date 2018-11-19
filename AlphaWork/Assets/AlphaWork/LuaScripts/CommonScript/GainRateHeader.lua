


protected_.registMetaMap(protected_.MetaGainRateSys, 0, nil, protected_.MetaGainRates);

protected_.registMetaProp(protected_.MetaGainRates, "prop_id_", 0);
protected_.registMetaProp(protected_.MetaGainRates, "today_num_", 0);   --今天获得的数量
protected_.registMetaProp(protected_.MetaGainRates, "week_num_", 0);   --今天获得的数量
protected_.registMetaProp(protected_.MetaGainRates, "next_ref_time_", 0);   --下次刷新时间
protected_.registMetaProp(protected_.MetaGainRates, "next_week_ref_time_", 0);   --下次刷新时间
protected_.registMetaMap(protected_.MetaGainRates, 0, nil, protected_.MetaGainRate);

protected_.registMetaProp(protected_.MetaGainRate, "key_", 0);
protected_.registMetaProp(protected_.MetaGainRate, "rate_", 0);
protected_.registMetaProp(protected_.MetaGainRate, "type_", 0);
protected_.registMetaProp(protected_.MetaGainRate, "num_", 0);
protected_.registMetaProp(protected_.MetaGainRate, "end_time_", 0);


protected_.MetaGainRate.isEffective = function(self)
    if self.type_ == Enum.ERewardRate_Time then
        if self.end_time_ > getTime() then
            return true;
        end
    elseif self.type_ == Enum.ERewardRate_Number then
        if self.num_ > 0 then
            return true;
        end
    end
    return false;
end
--获取某资源的增加倍率
protected_.MetaGainRateSys.getGainRate = function(self, prop_name)
    local rate = 1;
    local prop_id = protected_.getMetaPropIndex(protected_.MetaGamePlayer, prop_name);
    if not prop_id then
        return rate;
    end
    local reward_rates = self[prop_id];
    if not reward_rates then
        return rate;
    end
    for k, v in pairs(reward_rates.map_) do 
        local rate_type, rate = protected_.parseKey(k);
        if v:isEffective() then
            rate = rate + v.rate_;
        end
    end
    return rate;
end

--获取所有资源加倍信息
protected_.MetaGainRateSys.getAllGainRate = function(self)
    local all_rate = {};
    for prop_id, reward_rates in pairs(self.map_) do
        local prop_name = protected_.getMetaPropName(protected_.MetaGamePlayer, prop_id);
        if prop_name then
            for key, reward_rate in pairs(reward_rates.map_) do
                if reward_rate:isEffective() then
                    local rate_data = {};
                    if reward_rate.type_ == Enum.ERewardRate_Time then
                        table.insert(all_rate, {prop_name, reward_rate.rate_, reward_rate.type_, reward_rate.end_time_});
                    elseif reward_rate.type_ == Enum.ERewardRate_Number then
                        table.insert(all_rate, {prop_name, reward_rate.rate_, reward_rate.type_, reward_rate.num_});
                    end
                end
            end
        end
    end
    return all_rate;
end
--获取某资源今天获得的数量
protected_.MetaGainRateSys.getTodayNum = function(self, prop_name)
    local val = 0;
    local prop_id = protected_.getMetaPropIndex(protected_.MetaGamePlayer, prop_name);
    if not prop_id then
        return val;
    end
    local reward_rates = self[prop_id];
    if not reward_rates then
        return val;
    end
    if getTime() < reward_rates.next_ref_time_ then
        val = reward_rates.today_num_;
    end
    return val;
end
--获取某资源这周获得的数量
protected_.MetaGainRateSys.getWeekNum = function(self, prop_name)
    local val = 0;
    local prop_id = protected_.getMetaPropIndex(protected_.MetaGamePlayer, prop_name);
    if not prop_id then
        return val;
    end
    local reward_rates = self[prop_id];
    if not reward_rates then
        return val;
    end
    if getTime() < reward_rates.next_week_ref_time_ then
        val = reward_rates.week_num_;
    end
    return val;
end