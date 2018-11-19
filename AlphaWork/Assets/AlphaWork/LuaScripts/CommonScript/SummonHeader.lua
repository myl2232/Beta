
protected_.registMetaProp(protected_.MetaSummonSys, "num_", 0); --抽取的次数

--TODO神兽抽取和抽奖功能 将来合为一起
--抽奖
protected_.registMetaProp(protected_.MetaLotterySys, "num_", 0); --抽取的次数
protected_.registMetaMap(protected_.MetaLotterySys, 0, nil, protected_.MetaLotteryPack);

protected_.registMetaProp(protected_.MetaLotteryPack, "id_", 0); 
protected_.registMetaProp(protected_.MetaLotteryPack, "num_", 0); --奖包的次数
protected_.registMetaProp(protected_.MetaLotteryPack, "type_", 0); --奖包的类型
protected_.registMetaProp(protected_.MetaLotteryPack, "end_time_", 0); --奖包的结束时间
protected_.registMetaProp(protected_.MetaLotteryPack, "next_ref_num_time_", 0); --下一次次数刷新时间


protected_.MetaLotterySys.getPackData = function(pack_id)
    local pack_data = configs_.LotteryPack[pack_id];
    if not pack_data then
        log.error("MetaLotterySys.getPackData", nil, "pack_data is nil pack_id=", pack_id);
    end
    return pack_data;
end
protected_.MetaLotterySys.getPoolData = function(pool_id)
    local pool_data = configs_.LotteryPool[pool_id];
    if not pool_data then
        log.error("MetaLotterySys.getPoolData", nil, "pool_data is nil pool_id=", pool_id);
    end
    return pool_data;
end
--获取可以抽取的卡包  包括两种情况的卡包：1.一直可以抽取、2.需要开启
protected_.MetaLotterySys.getCanLotteryPack = function(self)
    local packs = {};
    local packs_temp = {};
    local packs_data = configs_.LotteryPack;
    for _, pack_id in ipairs(packs_data.default) do
        if not packs_temp[pack_id] then
            packs_temp[pack_id] = 1;
            table.insert(packs, pack_id);
        end
    end
    for pack_id, pack in pairs(self.map_) do
        if not packs_temp[pack_id] then
            if pack.end_time_ == 0 or pack.end_time_ > getTime() then
                packs_temp[pack_id] = 1;
                table.insert(packs, pack_id);
            end
        end
    end
    return packs;
end
protected_.MetaLotterySys.canAddPack = function(self, pack_id)
    local pack = self[pack_id];
    if pack and pack.end_time_ > getTime() then --已经添加了该卡包 还没到期
        log.error("MetaLotterySys.canAddPack", self.player_, "pack addpack error! pack_id=", pack_id);
        return false;
    end
    return true;
end
--此卡包已经抽取次数
protected_.MetaLotterySys.lotteryTimes = function(self, pack_id)
    if self[pack_id] then
        if self[pack_id].next_ref_num_time_ > 0 and  getTime() > self[pack_id].next_ref_num_time_ then  --次数刷新
            return 0;
        else
            return self[pack_id].num_;
        end
    end
    return 0;
end
--检查抽取次数
protected_.MetaLotterySys.checkTimes = function(self, pack_id, times)
    local pack_data = protected_.MetaLotterySys.getPackData(pack_id);
    if not pack_data then
        return false;
    end
    local max_num = pack_data.days_max_num;
    local lottery_num = self:lotteryTimes(pack_id);
    if max_num and (lottery_num + times) > max_num then
        log.warn("MetaLotterySys.checkTimes", self.player_, "lottery_num > max_num   lottery_num", lottery_num, "times=", times, "max_num=", max_num, "pack_id=", pack_id);
        return false;
    end
    return true;
end
--到期判断
protected_.MetaLotterySys.timeLimit = function(self, pack_id)
    local pack_data = protected_.MetaLotterySys.getPackData(pack_id);
    if not pack_data then
        return false;
    end
    if self[pack_id] and self[pack_id].end_time_ ~= 0 and self[pack_id].end_time_ < getTime() then
        log.warn("MetaLotterySys.timeLimit", self.player_, "time limit!! cur_time=", getTime(), "end_time_=", self[pack_id].end_time_, "pack_id=", pack_id);
        return false;
    end
    return true;
end
--抽取条件判断
protected_.MetaLotterySys.canLottery = function(self, pack_id, times)
    local pack_data = protected_.MetaLotterySys.getPackData(pack_id);
    if not pack_data then
        return false;
    end
    local cost_cmd = pack_data.cost[times];
    local count = 1;
    local mul_flag = pack_data.mul_flag or 0;
    if mul_flag > 0 and not cost_cmd then --卡包次数限制
        log.warn("MetaLotterySys.canLottery", self.player_, "times limit!! pack_id=", pack_id, "times=",times);
        return false;
    end
    if not self:checkTimes(pack_id, times) or not self:timeLimit(pack_id) then
        return false;
    end
    return true;
end