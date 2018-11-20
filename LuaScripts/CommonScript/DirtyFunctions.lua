--[[********************************************************************
	created:	2018/01/19
	author:		lixuguang_cx

	purpose:	根节点的脏数据处理函数
*********************************************************************--]]

protected_.game_players_.handleDirtyMark = function(self)
	local context = {};
	context.path={};
    self.update_dirty_count_ = self.update_dirty_count_+1;
    local save_flag = 0;
    -- 600次脏数据同步一次存盘同步
    if protected_.quit_flag_~=0 or self.update_dirty_count_ >= 600 then
        save_flag = 1;
        self.update_dirty_count_ = 0;
        local marks = self.dirtys_.marks_;
        for k,v in pairs(self.save_pool) do
            if marks[k]==nil then
                marks[k] = v;
            end
        end
        self.save_pool = {};
    end
	if self.dirtys_.marks_==nil or next(self.dirtys_.marks_)==nil then
		return;
	end
    self.begin_time_ = getCurrentTime();
	for k,v in pairs(self.dirtys_.marks_) do
        local game_player = self[k];
        if game_player==nil then
            gameLog("Error!! User["..k.."] is nill!!!");
	        self.dirtys_.marks_[k]=nil;
		else
            protected_.GameCoroutine.weightCall();
			if game_player.dirtys_.marks_ and game_player.handleDirtyMark~=nil then
				context.context_key_ = k;
                local save_ret, syn_table = game_player:handleDirtyMark(context, save_flag, 1);
				if save_ret==1 then --
					protected_.saveUserCall(k, k, game_player);
				end
                if syn_table and next(syn_table) then
                    protected_.sendDirtyMessage(k, syn_table);
                end
				context.context_key_ = 0;
				context.path={};
				context.proto=nil;
				context.proto_contexts = nil;
                if save_flag==0 then
                    self.save_pool[k] = v;
                end
	            self.dirtys_.marks_[k]=nil;
                --print("#####update player end ", k);
			end
        end
	end
    if self.begin_time_ then
        -- gameLog("protected_.game_players_.handleDirtyMark "..(getCurrentTime()-self.begin_time_).."ms");
        self.begin_time_ = 0;
    end
    if save_flag==1 then
        --print("#####game_players_.handleDirtyMark save end!");
    end
end

protected_.game_scenes_.handleDirtyMark = function(self)
	local context = {};
	context.path={};
    self.update_dirty_count_ = self.update_dirty_count_+1;
    local save_flag = 0;
    -- 600次脏数据同步一次存盘同步
    if protected_.quit_flag_~=0 or self.update_dirty_count_ >= 600 then
        save_flag = 1;
        self.update_dirty_count_ = 0;
        local marks = self.dirtys_.marks_;
        for k,v in pairs(self.save_pool) do
            if marks[k]==nil then
                marks[k] = v;
            end
        end
        self.save_pool = {};
    end
	if self.dirtys_.marks_==nil or next(self.dirtys_.marks_)==nil then
		return;
	end
    self.begin_time_ = getCurrentTime();
	for k,v in pairs(self.dirtys_.marks_) do
        local game_scene = self[k];
        if game_scene==nil then
            gameLog("Error!! User["..k.."] is nill!!!");
	        self.dirtys_.marks_[k]=nil;
		else
            protected_.GameCoroutine.weightCall();
			if game_scene.dirtys_.marks_ and game_scene.handleDirtyMark~=nil then
				context.context_key_ = k;
                local save_ret, syn_table = game_scene:handleDirtyMark(context, save_flag, 1);
				if save_ret==1 then --
					protected_.saveSceneCall(k, k, game_scene);
				end
                -- if syn_table and next(syn_table) then
                --     protected_.sendDirtyMessage(k, syn_table);
                -- end
				context.context_key_ = 0;
				context.path={};
				context.proto=nil;
				context.proto_contexts = nil;
                if save_flag==0 then
                    self.save_pool[k] = v;
                end
	            self.dirtys_.marks_[k]=nil;
                --print("#####update player end ", k);
			end
        end
	end
    if self.begin_time_ then
        -- gameLog("protected_.game_players_.handleDirtyMark "..(getCurrentTime()-self.begin_time_).."ms");
        self.begin_time_ = 0;
    end
    if save_flag==1 then
        --print("#####game_players_.handleDirtyMark save end!");
    end
end