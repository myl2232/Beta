

--MetaGamePlayer
protected_.MetaGamePlayer.handlers_.money_ = function(self, k, old_val, v)
    print(" -------------- old_val, v, ", old_val, v);
    if old_val ~= v then
        local client = self.dirtys_.parent_;
        if client then
            client_recall_("onMoneyChange", client.client_id_, "money_");
        end
    end
end
protected_.MetaGamePlayer.handlers_.soul_stone_ = function(self, k, old_val, v)
    print(" -------------- old_val, v, ", old_val, v);
    if old_val ~= v then
        local client = self.dirtys_.parent_;
        if client then
            client_recall_("onMoneyChange", client.client_id_, "soul_stone_");
        end
    end
end
protected_.MetaGamePlayer.handlers_.ofuda_ = function(self, k, old_val, v)
    print(" -------------- old_val, v, ", old_val, v);
    if old_val ~= v then
        local client = self.dirtys_.parent_;
        if client then
            client_recall_("onMoneyChange", client.client_id_, "ofuda_");
        end
    end
end
protected_.MetaGamePlayer.handlers_.spirit_chip_ = function(self, k, old_val, v)
    print(" -------------- old_val, v, ", old_val, v);
    if old_val ~= v then
        local client = self.dirtys_.parent_;
        if client then
            client_recall_("onMoneyChange", client.client_id_, "spirit_chip_");
        end
    end
end
protected_.MetaGamePlayer.handlers_.pet_money_ = function(self, k, old_val, v)
    print(" -------------- old_val, v, ", old_val, v);
    if old_val ~= v then
        local client = self.dirtys_.parent_;
        if client then
            client_recall_("onMoneyChange", client.client_id_, "pet_money_");
        end
    end
end

--MetaItemBag
protected_.MetaItemBag.handlers_.map_ = function(self, k, old_val, v)
    if old_val==nil then
		if  v~=nil then
			self:putIndex(v.item_sid_, v.item_id_);
		end
		return;
	else
		if v==nil then
			self:removeIndex(old_val.item_sid_, old_val.item_id_);
		end
    end
    if self.dirtys_ and self.dirtys_.parent_.dirtys_ then
        local client = self.dirtys_.parent_.dirtys_.parent_;   
        client_recall_("onItemChange", client.client_id_);
    end
end
protected_.MetaItem.handlers_.count_ = function(self, k, old_val, v)
    if old_val ~= v then
        local client = self.dirtys_.parent_.dirtys_.parent_.dirtys_.parent_;
        client_recall_("onItemChange", client.client_id_);
    end
end