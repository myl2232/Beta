
protected_.registMetaProp(protected_.MetaItem, "item_id_", 0);
protected_.registMetaProp(protected_.MetaItem, "item_sid_", 0); --配置sid
protected_.registMetaProp(protected_.MetaItem, "count_", 0); --道具数量
protected_.registMetaProp(protected_.MetaItem, "expire_time_", 0);  --到期时间
protected_.registMetaProp(protected_.MetaItem, "limit_time_", 0);  --限制时间
protected_.registMetaProp(protected_.MetaItem, "update_time_", 0);  --最新更新道具数量时间

protected_.registMetaProp(protected_.MetaItemBag, "capacity_", 0);  --当前背包容量
protected_.registMetaProp(protected_.MetaItemBag, "add_iter_", 1, 2);  
protected_.registMetaMap(protected_.MetaItemBag, 0, nil, protected_.MetaItem);
-- protected_.registMetaProp(protected_.MetaItemBag, "buy_record_",nil, nil, protected_.MetaMap);  --购买道具的记录， {item_sid = count, }

protected_.MetaItemBag.getItemsData = function()
    return  configs_.Item;
end
protected_.MetaItemBag.getItemData = function(item_sid)
    local item_data = protected_.MetaItemBag.getItemsData()[item_sid];
    if not item_data then
        log.error("MetaItemBag.getItemData", nil, "item_data is nil item_sid=", item_sid);
        print(debug.traceback());
        return ;
    end
    return item_data;
end
protected_.MetaItemBag.getCountBySid = function(self, item_sid)
    if not self.sid_index_[item_sid] then
        return 0;
    end
    local total = 0;
    for item_id, _ in pairs(self.sid_index_[item_sid]) do
        total = total + self[item_id].count_;
    end
    return total;
end
protected_.MetaItemBag.getCount = function(self, item_id)
    local item = self[item_id];
    if not item then
        return 0;
    end
    return item.count_;
end
--检查道具数量
protected_.MetaItemBag.checkCount = function(self, item_id, count)
    if type(count) ~= "number" or count <= 0 then
        log.warn("MetaItemBag.checkCount", self.player_, "item count error count=", count);
        return false;
    end
    if self:getCount(item_id) < count then
        log.warn("MetaItemBag.checkCount", self.player_, "item count is not enough count=", self:getCount(item_id), "use count=", count);
        return false;
    end
    return true;
end
protected_.MetaItemBag.getItemStackMax = function(item_sid)
    local item_data = protected_.MetaItemBag.getItemData(item_sid);
    local stack_max = protected_.MetaItemBag.getItemsData().stack_max;
    if item_data and item_data.stack_max  then
        stack_max = item_data.stack_max;
    end
    return stack_max;
end
protected_.MetaItemBag.getCapacityMax = function(self)
    return protected_.MetaItemBag.getItemsData().capacity_max;
end

protected_.MetaItemBag.initSidIndex = function(self)
    self.sid_index_ = {};
    for k, v in pairs(self.map_) do
        self:putIndex(v.item_sid_, k);
    end
end

protected_.MetaItemBag.putIndex = function(self, item_sid, item_id)
    if not self.sid_index_ then
        self.sid_index_ = {};
    end
    if not self.sid_index_[item_sid] then
        self.sid_index_[item_sid] = {};
    end
    self.sid_index_[item_sid][item_id] = 1;
end
protected_.MetaItemBag.removeIndex = function(self, item_sid, item_id)
    if not self.sid_index_ then
        gameAssert(false, debug.traceback());
        return ;
    end
    if not self.sid_index_[item_sid] then
        gameAssert(false, debug.traceback());
        return ;
    end 
    if not self.sid_index_[item_sid][item_id] then
        gameAssert(false, debug.traceback());
        return ;
    end
    self.sid_index_[item_sid][item_id] = nil;
end
--获取道具的持续时间 毫秒
protected_.MetaItemBag.getPersistTime = function(item_sid)
    local item_data = protected_.MetaItemBag.getItemData(item_sid);
    local rt_mark = item_data.rt_mark;
    if not rt_mark then
        return 0;
    end
    rt_mark = rt_mark - 100;
    if rt_mark < 0 then
        return 0;
    end
    return rt_mark * 24 * 60 * 60 * 1000;
    -- return rt_mark * 1000 * 60;
end