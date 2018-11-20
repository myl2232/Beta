protected_.MetaArray = {
	__index = protected_.index_,
	__newindex = protected_.newindex_,
    insert = function (self, pos, k)
        if k==nil then
            table.insert(self.map_, pos);
        else
            table.insert(self.map_, pos, k);
        end
        protected_.markDirty(self.dirtys_.parent_, self.dirtys_.parent_key_);
    end,
    remove = function (self, k)
        table.remove(self.map_, k);
        protected_.markDirty(self.dirtys_.parent_, self.dirtys_.parent_key_);
    end,
    removeValue = function (self, val)
        local li = nil;
        local ln = next(self.map_);
        while(ln) do
            if self.map_[ln]==val then
                table.remove(self.map_, ln);
                ln = li;
            else
                li = ln;
            end
            ln = next(self.map_,ln);
        end
        protected_.markDirty(self.dirtys_.parent_, self.dirtys_.parent_key_);
    end,
    find = function (self, val)
        for i,v in ipairs(self.map_) do
            if v==val then
                return i;
            end
        end
        return nil;
    end,
    back = function(self)
        local count = #self.map_;
        if count==0 then
            return nil;
        end
        return self.map_[count];
    end,
    handler_ = function (self, k, old_val, v)
	    if t.dirtys_==nil then
		    return;
	    end
	    if old_val==v then
		    return;
	    end
        protected_.markDirty(self.dirtys_.parent_, self.dirtys_.parent_key_);
    end
};
protected_.MetaArray.initArray=function (t, parent, parent_key)
    t = t or {};
    if t.base_ == nil then
	    local r = {};
	    r.base_ = t;
        t = r;
    end
	t.dirtys_ ={};
	t.dirtys_.parent_ = parent;
	t.dirtys_.parent_key_ = parent_key;
	t.dirtys_.marks_ = {};
	setmetatable(t, protected_.MetaArray);
    return t;
end;

--------------------------------------------------类注册区------------------------------------------
--[[
	如果更新版本，Class注册属性只能增加不可删除，至少需要一个空占位符
--]]

--普通的数组类
protected_.registMetaMap(protected_.MetaArray, 0);

--普通的map表
protected_.declareMetaClass("MetaMap");
protected_.registMetaMap(protected_.MetaMap, 0);

--事件类
protected_.declareMetaClass("LuaEventManager", nil, {});
protected_.declareMetaClass("EventContextMeta", nil, {});

protected_.declareMetaClass("MetaDialog");
