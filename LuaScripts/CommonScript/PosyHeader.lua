

protected_.registMetaProp(protected_.MetaPosyProgram, "id_", 0);     --
protected_.registMetaProp(protected_.MetaPosyProgram, "lock_", 0);     --解锁状态 0未解锁 1解锁
protected_.registMetaProp(protected_.MetaPosyProgram, "name_", nil);     --铭文名字
protected_.registMetaProp(protected_.MetaPosyProgram, "assembly_",nil, nil, protected_.MetaMap);  --装配  {slot_id = item_sid,}

--铭文系统
protected_.registMetaProp(protected_.MetaPosySys, "buy_num_", 0);     
protected_.registMetaProp(protected_.MetaPosySys, "last_use_id_", 0);     
protected_.registMetaMap(protected_.MetaPosySys, 0, nil, protected_.MetaPosyProgram);	

protected_.MetaPosySys.getSlotData = function(slot_id)
    local slot_data = configs_.Posy.slot[slot_id];
    if not slot_data then
        log.error("MetaPosySys.getSlotData", nil, "posy slot_data is nil slot_id=", slot_id);
    end
    return slot_data;
end

protected_.MetaPosySys.getProgramData = function(program_id)
    local program_data = configs_.Posy.program[program_id];
    if not program_data then
        log.error("MetaPosySys.getProgramData", nil, "posy program_data is nil program_id=", program_id);
    end
    return program_data;
end
protected_.MetaPosySys.getProgramsData = function()
    return configs_.Posy.program;
end

--获取铭文的属性影响
protected_.MetaPosyProgram.getPropEffect = function(self)
    local props = {};
    for slot_id, item_sid in pairs(self.assembly_.map_) do
        local items = configs_.Item;
        local item_data = items[item_sid];
        if item_data and item_data.posy_prop then
            for prop_name, val in pairs(item_data.posy_prop) do
                props[prop_name] = (props[prop_name] or 0) + val;
            end
        end
    end
    return props;
end

--该槽位是否解锁
protected_.MetaPosySys.getSlotUnlockState = function(self, slot_id)
    local game_player = self.dirtys_.parent_;
    local slot_data = protected_.MetaPosySys.getSlotData(slot_id);
    if not slot_data then
        return false;
    end
    if slot_data.level and slot_data.level > game_player.player_level_ then
        return false;
    end
    return true;
end
--该方案的解锁状态
protected_.MetaPosySys.getProgramUnlockState = function(self, program_id)
    local game_player = self.dirtys_.parent_;
    local program_data = protected_.MetaPosySys.getProgramData(program_id);
    if not program_data then
        return false;
    end
    local program = self[program_id];
    if program and program.lock_ == 1 then
        return true;
    end
    return false;
end
--获得剩余道具数量
protected_.MetaPosySys.getItemNum = function(self, program_id, item_sid)
    local game_player = self.dirtys_.parent_;
    local item_bag = game_player.item_bag_;
    local item_num = item_bag:getCountBySid(item_sid);
    local use_num = self:getUseItemNum(program_id, item_sid);
    return item_num - use_num;
end
--获得本方案使用了多少该道具
protected_.MetaPosySys.getUseItemNum = function(self, program_id, item_sid)
    local game_player = self.dirtys_.parent_;
    local use_num = 0;
    local program = self[program_id];
    if program then
        for k, v in pairs(program.assembly_.map_) do
            if v == item_sid then
                use_num = use_num + 1;
            end
        end
    end
    return use_num;
end
--获得槽位类型
protected_.MetaPosySys.getSlotType = function(slot_id)
    return math.floor(slot_id / 100);
end
--是否可以装配
protected_.MetaPosySys.canInstall = function(self, program_id, slot_id, item_sid)
    local game_player = self.dirtys_.parent_;
    local program_lock_state = self:getProgramUnlockState(program_id);
    --1.方案解锁
    if not program_lock_state then
        log.warn("MetaPosySys.canInstall", game_player, "program_lock_state is false  program_id=", program_id);
        return false;
    end
    --2.槽位解锁
    local slot_lock_state = self:getSlotUnlockState(slot_id);
    if not slot_lock_state then
        log.warn("MetaPosySys.canInstall", game_player, "slot_lock_state is false  slot_id=", slot_id);
        return false;
    end
    local program = self[program_id];
    if program and program.assembly_[slot_id] == item_sid then
        log.warn("MetaPosySys.canInstall", game_player, "slot item_sid is same  item_sid=", item_sid);
        return false;
    end
    --3.道具判断
    local item_data = protected_.MetaItemBag.getItemData(item_sid);
    if not item_data then
        return false;
    end
    --3.1 道具类型判断
    local slot_type = protected_.MetaPosySys.getSlotType(slot_id)
    if item_data.item_type ~= Enum.ItemType_Posy or item_data.posy_type ~= slot_type then
        log.warn("MetaPosySys.canInstall", game_player, "item item_type or posy_type error  item_sid=", item_sid, "slot_type=", slot_type);
        return false;
    end
    --3.2 道具数量判断
    local num = self:getItemNum(program_id, item_sid);
    if num < 1 then
        log.warn("MetaPosySys.canInstall", game_player, "item num not enough  item_sid=", item_sid, "num=", num);
        return false;
    end

    return true;
end
--获得方案评分
protected_.MetaPosySys.getTotalScore = function(self, program_id)
    local total_score = 0;
    local program = self[program_id];
    if not program then
        return total_score;
    end
    for slot_id, item_sid in pairs(program.assembly_.map_) do
        local items = configs_.Item;
        local item_data = items[item_sid];
        if item_data and type(item_data.posy_quality) == "number" then
            local score = configs_.Posy.score[item_data.posy_quality] or 0;
            total_score = total_score + score;
        end
    end
    return total_score;
end
--前端使用 
protected_.MetaPosySys.getNoUseItemNum = function(self, item_sid)
    local game_player = self.dirtys_.parent_;
    local item_bag = game_player.item_bag_;
    local item_num = item_bag:getCountBySid(item_sid);
    local max_use_num = 0;
    for k, v in pairs(self.map_) do
        local use_num = self:getUseItemNum(k, item_sid);
        if use_num > max_use_num then
            max_use_num = use_num;
        end
    end
    return item_num - max_use_num;
end

--缓存本方案的道具个数情况
--[[
protected_.MetaPosySys.initItemUse = function(self)
    local game_player = self.dirtys_.parent_;
    for k, v in pairs(self.map_) do
        for slot_id, item_sid in pairs(v.assembly_.map_) do
            self:addItemUse(k, item_sid);
        end
    end
end
protected_.MetaPosySys.addItemUse = function(self, program_id, slot_id, item_sid)
    local game_player = self.dirtys_.parent_;
    local program = self[program_id];
    if not program then
        return ;
    end
    if not program.item_use_ then
        program.item_use_ = {};
    end
    program.item_use_[item_sid] = (program.item_use_[item_sid] or 0) + 1;
end
protected_.MetaPosySys.removeItemUse = function(self, program_id, item_sid)
    local game_player = self.dirtys_.parent_;
    local program = self[program_id];
    if not program then
        gameAssert(false, debug.traceback());
        return ;
    end
    if not program.item_use_ then
        gameAssert(false, debug.traceback());
        return ;
    end
    local use_num = program.item_use_[item_sid] or 0;
    if use_num < 1 then
        gameAssert(false, debug.traceback());
        return ;
    end
    program.item_use_[item_sid] = program.item_use_[item_sid] - 1;
end
]]