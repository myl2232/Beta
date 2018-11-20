

protected_.msg_handlers_.handlePacket = function(self, client_id, msg)
	table.insert(protected_.handle_protos, msg[1]);
	--print("####protected_.msg_handlers_.handlePacket: ", msg[1]);
	local msg_handle_time_0 = socket.gettime()*1000;

	--local ret = self[msg[1]](client_id, msg);
	local msg_call = self[msg[1]];
	if msg_call==nil then
		gameError("Server Proto["..msg[1].."] not exist!");
		return -1;
	end
	local r, ret = pcall(msg_call, client_id, msg);
    if not r then
        gameError("handlePacket error["..msg[1].."] error: "..ret);
        return -1;
    end
	local msg_handle_time_1 = socket.gettime()*1000;
	if msg_handle_time_1-msg_handle_time_0>=5 then
		print("protected_.msg_handlers_.handlePacket", msg_handle_time_1-msg_handle_time_0, msg[1]);
	end

	return ret;
end
protected_.msg_handlers_[ES2CProtocol.ES2CProtocol_Ping] = function(client_id, msg)
	-- print("Recv: ES2CProtocol_Test");
    local index = 2;
	local ping_context = msg[index];
	index = index +1;
	local ping_count = msg[index];
	index = index +1;
    local client_time = msg[index];
	index = index +1;
    local server_time = msg[index];
	index = index +1;
    local cur_time = getTime();
    local dms = cur_time-client_time;
	gameLog("Ping "..dms.."ms ...");
    if dms>100 then
	    gameLog("Ping Detail "..(server_time-client_time).."ms, "..(cur_time-server_time).."ms");
    end
	local smsg = {EC2SProtocol["EC2SProtocol_Ping"], ping_context, ping_count+1, client_time, server_time, cur_time};
	sendMessage(client_id, smsg);
end
protected_.msg_handlers_[ES2CProtocol.ES2CProtocol_ServerTime] = function(client_id, msg)
	-- print("Recv: ES2CProtocol_ServerTime");
	local index = 2;
	local system_time=msg[index]; -- msec
	index = index+1;
--	client_recall_("onServerTimer", system_time);
	local dtime = system_time-getTime();
	protected_.server_time_offset_ = protected_.server_time_offset_+dtime;
	print("Adjust Server Time: ", protected_.server_time_offset_);
	return 0;
end
protected_.msg_handlers_[ES2CProtocol.ES2CProtocol_Response] = function(client_id, msg)
	--print("Recv: ES2CProtocol_Response");
	local index = 2;
	local proto_id = msg[index];
	index = index + 1;
	local result = msg[index];
	index = index + 1;
	local msg_context = msg[index];
	index = index + 1;
	if result == 0 then
	    if protected_.msg_responses_[proto_id] then
	        protected_.msg_responses_[proto_id](client_id, result, msg_context);
	    end
	else
		print("Recv: " .. EC2SProtocolStr[proto_id] .. " failed result = " .. dump_obj(result))
		client_recall_("onResponseError", client_id);
	end
	-- commands_:handleMessage(client_id, {proto_id_ = proto_id, result_ = result_})
	return 0;
end
protected_.msg_handlers_[ES2CProtocol.ES2CProtocol_Hint] = function(client_id, msg)
	-- print("Recv: ES2CProtocol_Hint -------- ")
	client_recall_("onHint", client_id, msg);
	return 0;
end

protected_.msg_handlers_[ES2CProtocol.ES2CProtocol_LoginOK] = function(client_id, msg)
	print("Recv: ES2CProtocol_LoginOK ")
	local index = 2;
	local client = protected_.clients_[client_id];
	client.player_id_ = msg[index];
	index = index +1;
	client.reconnect_key_ = msg[index];
	index = index +1;
	client.u_id_ = msg[index];
	index = index +1;
	client.suid_ = msg[index];
	index = index +1;
	client.server_id_ = msg[index];
	index = index +1;
	protected_.playerClients_[client.player_id_] = client;
	return 0;
end
protected_.msg_handlers_[ES2CProtocol.ES2CProtocol_UserOK] = function(client_id, msg)
	local client = protected_.clients_[client_id];
	if client.client_state_~=2 and client.client_state_~=1 and client.client_state_ ~= 3 then
		print("login error state : "..protected_.clients_[client_id].client_state_);
		return 0;
	end
	client.client_state_ = 13; --Runing login ok..
    --客户端加载资源相关的东西
	client_recall_("onUserdataOk", client_id);
	return 0;
end
protected_.msg_handlers_[ES2CProtocol.ES2CProtocol_UserData] = function(client_id, msg)
	print("Recv: ES2CProtocol_UserData ")
	local index = 2;
	local game_player = msg[index];
	index = index +1;
	-- print("ES2CProtocol_UserData: ", dump_obj(game_player));
	local client = protected_.clients_[client_id];
	local player = protected_.constructObject(protected_.MetaGamePlayer, game_player, client, "player_");
	client.player_ = player;
	client.dirtys_={["marks_"]= {}};
	player.item_bag_:initSidIndex();
--	player.exp_ = 10000;
	return 0;
end
protected_.msg_handlers_[ES2CProtocol.ES2CProtocol_PetBag] = function(client_id, msg)
	print("Recv: ES2CProtocol_PetBag ")
	local game_player = protected_.clients_[client_id].player_;
	local index = 2;
	local pet_bag = msg[index];
	index = index +1;
	--print("ES2CProtocol_PetBag: ", dump_obj(pet_bag));
	pet_bag = protected_.constructObject(protected_.MetaPetBag, pet_bag, game_player, "pet_bag_");
	return 0;
end
protected_.msg_handlers_[ES2CProtocol.ES2CProtocol_HeroBag] = function(client_id, msg)
	print("Recv: ES2CProtocol_HeroBag ")
	local game_player = protected_.clients_[client_id].player_;
	local index = 2;
	local hero_bag = msg[index];
	index = index +1;
	--print("ES2CProtocol_PetBag: ", dump_obj(pet_bag));
	hero_bag = protected_.constructObject(protected_.MetaHeroBag, hero_bag, game_player, "hero_bag_");
	return 0;
end
protected_.msg_handlers_[ES2CProtocol.ES2CProtocol_PlayerSeason] = function(client_id, msg)
	print("Recv: ES2CProtocol_PlayerSeason ")
	local game_player = protected_.clients_[client_id].player_;
	local index = 2;
	local player_season = msg[index];
	index = index +1;
	--print("ES2CProtocol_PetBag: ", dump_obj(pet_bag));
	player_season = protected_.constructObject(protected_.MetaSeason, player_season, game_player, "player_season_");
	return 0;
end
protected_.msg_handlers_[ES2CProtocol.ES2CProtocol_CreateChar] = function(client_id, msg)
	print("Recv: ES2CProtocol_CreateChar");
	local index = 2;
	local failed_name = msg[index];
	index = index +1;
    if protected_.console_connect_type == 1 then
        protected_.change_state_to_createchar(client_id);
    else
		client_recall_("onCreateChar", client_id, failed_name);
    end
	return 0;
end
protected_.msg_handlers_[ES2CProtocol.ES2CProtocol_CreateCharOK] = function(client_id, msg)
	print("Recv: ES2CProtocol_CreateCharOK");
	client_recall_("onCreateCharOk", client_id);
	return 0;
end

function synDirtyValue(client_object, dirty_object)
	local mt = getmetatable(client_object);
	if mt then
		--print("mt: ", mt);
		if client_object.base_==nil then
			client_object.base_ = {};
		end
		for k,v in pairs(dirty_object) do
			--print("dirty_object: ", k);
			local meta_prop = mt.meta_props_[k];
			if meta_prop then
				local k_name = meta_prop[Enum.EMetaProp_Name];
				--print("meta_prop: ", k_name);
				if k_name=="map_" then
					for kk, vv in pairs(v) do
						if type(vv)~="table" then
							--print("map : set value ", kk);
							client_object[kk] = vv;
						else
							local v_k, v_v = next(vv);
							if v_k==nil then
								--print("map : del value ", kk);
								client_object[kk]=nil;
							elseif v_k==0 then
								--print("map : constructObject", kk);
								client_object[kk] = protected_.constructObject(meta_prop[Enum.EMetaProp_Class], v_v, client_object, kk);
							else
								--print("map : syn value ", kk);
								synDirtyValue(client_object[kk], vv);
							end
						end
					end
				elseif meta_prop[Enum.EMetaProp_Class] then
					local v_k, v_v = next(v);
					if v_k==nil then
						--print("del Object: ", k_name);
						client_object[k_name] = nil;
					elseif v_k==0 then
						--print("constructObject: ", k_name);
						client_object[k_name] = protected_.constructObject(meta_prop[Enum.EMetaProp_Class], v_v, client_object, k_name);
					else
						--print("syn Object: ", k_name);
						synDirtyValue(client_object[k_name], v);
					end
				elseif protected_.isMetaExProp(mt, k_name) then
					local v_k, v_v = next(v);
					if v_k==nil then
						--print("del Exprop: ", k_name);
						protected_.constructObject(mt.ex_props_[k_name][1], nil, client_object, k_name);
					elseif v_k==0 then
						--print("construct Exprop: ", k_name);
						protected_.constructObject(mt.ex_props_[k_name][1], v_v, client_object, k_name);
					else
						--print("syn Exprop: ", k_name);
						synDirtyValue(client_object[k_name], v);
					end
				else
					--print("set prop: ", k_name, v);
					if type(v)~="table" then
						client_object[k_name] = v;
					else
						local v_k, v_v = next(v);
						if v_k==nil then
							client_object[k_name] = nil;
						elseif v_k==0 then
							client_object[k_name] = v_v;
						else
							--没有metaclass还有同步机制，按说不会到这里
							gameLog("Warning setProp exception: "..k_name);
							synDirtyValue(client_object[k_name], v);
						end
					end
				end
			else
				--不一定错，服务器如果更新了代码，客户的没有更新，meta class 不一致，会出现这种情况
				gameLog("warning, syn not exist prop!!!");
				if type(v)~="table" then
					client_object.base_[k] = v;
				else
					local v_k, v_v = next(v);
					if v_k==nil then
						client_object.base_[k] = nil;
					elseif v_k==0 then
						client_object.base_[k] = v_v;
					else
						--服务器客户的meta class不一致，数据同步乱套了！！！
						gameLog("Warning synProp error: "..k);
						client_object.base_[k] = v_v;
					end
				end
			end
		end
	else
		--服务器和客户的meta class 不一致，会出现这种情况
		for k,v in pairs(dirty_object) do
			gameLog("Warning synProp error: "..k);
			if type(v)~="table" then
				print("##", k, v);
				client_object[k] = v;
			else
				local v_k, v_v = next(v);
				if v_k==nil then
					client_object[k] = nil;
				elseif v_k==0 then
					client_object[k] = v_v;
				else
					client_object[k] = v_v;
				end
			end
		end
	end
end
protected_.msg_handlers_[ES2CProtocol.ES2CProtocol_SynDirty] = function(client_id, msg)
	--print("Recv: ES2CProtocol_SynDirty");
	local game_player = protected_.clients_[client_id].player_;
	local index = 2;
	local dirty_syn_table = msg[index];
	index = index +1;
	if not dirty_syn_table then
		return 0;
	end
	local v_k, v_v = next(dirty_syn_table);
	gameAssert(v_k and v_k~=0, "");
	--print("ES2CProtocol_SynDirty: ", dump_obj(dirty_syn_table));
	synDirtyValue(game_player, dirty_syn_table);
	--print("ES2CProtocol_SynDirty Player: ", dump_obj(game_player));
	return 0;
end

protected_.msg_handlers_[ES2CProtocol.ES2CProtocol_EnterWorldOK] = function(client_id, msg)
    print("Recv: ES2CProtocol_EnterWorldOK");
    local game_player = protected_.clients_[client_id].player_;
    if game_player == nil then
        gameAssert(false, "error state! ES2CProtocol_EnterWorldOK");
    end
    --进入成功
	client_recall_("onEnterWorld", client_id);
	return 0;
end

protected_.msg_handlers_[ES2CProtocol.ES2CProtocol_InitTroop] = function(client_id, msg)
	--print("Recv: ES2CProtocol_InitTroop");
	local client = protected_.clients_[client_id];
	local index = 2;
	local suid = msg[index];
	index = index +1;
	local troop = msg[index];
	index = index +1;
   -- printObject(msg)
	if client.suid_~=suid then
		print("ES2CProtocol_InitTroop error: ", client.suid_, suid);
	end
	gameAssert(client.suid_==suid, "");
	client.cur_team_troop_ = protected_.unserialObject(protected_.MetaTeamTroop, troop);
	if client.cur_team_troop_.marks_==nil then
		client.cur_team_troop_.marks_ = protected_.constructObject(protected_.MetaMap, nil, client.cur_team_troop_, "marks_");
	end
	--client.teamtroops_ = {};
	protected_.team_troops_[client.suid_] = client.cur_team_troop_;
    
	local mem_list = {};
	client.cur_team_troop_.list = mem_list;
	for k,v in pairs(client.cur_team_troop_.map_) do
		mem_list[v.troop_pos_] = k;
	end
	client_recall_("onTroopInit", client_id, client.cur_team_troop_);
	return 0;
end

protected_.msg_handlers_[ES2CProtocol.ES2CProtocol_MergeTroop] = function(client_id, msg)
	-- print("Recv: ES2CProtocol_MergeTroop");
	local client = protected_.clients_[client_id];
	local index = 2;
	local troop = msg[index];
	index = index +1;
	local old_troop = client.cur_troop_;
	if old_troop then
		client.troops_[client.cur_troop_.id_] = nil;
	end
	client.cur_troop_ = protected_.unserialObject(protected_.MetaTroop, troop);
	if client.cur_troop_.marks_==nil then
		client.cur_troop_.marks_ = protected_.constructObject(protected_.MetaMap, nil, client.cur_troop_, "marks_");
	end
	client.troops_ = {};
	client.troops_[client.cur_troop_.id_] = client.cur_troop_;
	local mem_list = {};
	client.cur_troop_.list = mem_list;
	for k,v in pairs(client.cur_troop_.map_) do
		mem_list[v.troop_pos_] = k;
	end
	client_recall_("onTroopMerge", client_id, old_troop, client.cur_troop_);
	return 0;
end

protected_.msg_handlers_[ES2CProtocol.ES2CProtocol_TroopInvite] = function(client_id, msg)
	-- print("Recv: ES2CProtocol_TroopInvite");
	local client = protected_.clients_[client_id];
	local index = 2;
	local suid = msg[index];
	index = index +1;
	local player_name = msg[index];
	index = index +1;
	local fight_server_id = msg[index];
	index = index +1;
	local fight_sid = msg[index];
	index = index +1;
	
	client_recall_("onTroopInvite", client_id, player_name, suid, fight_server_id, fight_sid);
	return 0;
end

protected_.msg_handlers_[ES2CProtocol.ES2CProtocol_JoinTroop] = function(client_id, msg)
	-- print("Recv: ES2CProtocol_JoinTroop");
	local client = protected_.clients_[client_id];
	local index = 2;
	local member = msg[index];
	index = index +1;
	local new_mem = protected_.unserialObject(protected_.MetaTeamMember, member);
	if client.cur_team_troop_[new_mem.player_suid_]==nil then
		client.cur_team_troop_.count_ = client.cur_team_troop_.count_+1;
	end
	protected_.setParentObject(new_mem, client.cur_team_troop_, new_mem.player_suid_);
	client.cur_team_troop_.list[new_mem.troop_pos_] = new_mem.player_suid_;

	client_recall_("onTroopAdd", client_id, new_mem);
	return 0;
end

protected_.msg_handlers_[ES2CProtocol.ES2CProtocol_QuitTroop] = function(client_id, msg)
	--print("Recv: ES2CProtocol_QuitTroop");
	local client = protected_.clients_[client_id];
	local index = 2;
	local member_id = msg[index];
	index = index +1;
	local leader_id = msg[index];
	index = index +1;
	local member = client.cur_team_troop_[member_id];
	if member==nil then
		return 0;
	end
	if member_id==client.suid_ then
        protected_.team_troops_[client.suid_] = nil;
		client.cur_team_troop_ = nil;
		client_recall_("onTroopQuit", client_id);
	else
--		if client.cur_troop_.state_ == Enum.ETroop_Math then
--			client.cur_troop_.state_ = Enum.ETroop_Create;
--			client_recall_("onMatchFailed", client_id);
--		end
		client.cur_team_troop_[member_id] = nil;
		client.cur_team_troop_.list[member.troop_pos_] = nil;
		client.cur_team_troop_.count_ = client.cur_team_troop_.count_-1;
		if leader_id and leader_id>0 and client.cur_team_troop_.leader_~=leader_id then
			client.cur_team_troop_.leader_ = leader_id;
			client_recall_("onSetLeader", client_id, leader_id);
		end
		client_recall_("onMemberLeave", client_id, member);
	end

	return 0;
end

protected_.msg_handlers_[ES2CProtocol.ES2CProtocol_SetLeader] = function(client_id, msg)
	-- print("Recv: ES2CProtocol_SetLeader");
	local client = protected_.clients_[client_id];
	local index = 2;
	local leader_id = msg[index];
	index = index +1;
	client.cur_team_troop_.leader_ = leader_id;
	local member = client.cur_team_troop_[leader_id];
	client_recall_("onSetLeader", client_id, leader_id);
	return 0;
end

protected_.msg_handlers_[ES2CProtocol.ES2CProtocol_WaitMatch] = function(client_id, msg)
	-- print("Recv: ES2CProtocol_WaitMatch");
	local client = protected_.clients_[client_id];
	client.cur_team_troop_.state_ = Enum.ETroop_Math;
	client_recall_("onWaitMatch", client_id);
	return 0;
end

protected_.msg_handlers_[ES2CProtocol.ES2CProtocol_ChooseBegin] = function(client_id, msg)
	-- print("Recv: ES2CProtocol_ChooseBegin");
	local client = protected_.clients_[client_id];
	client.cur_troop_.state_ = Enum.ETroop_Choose;
	client_recall_("onChooseBegin", client_id);
	return 0;
end

protected_.msg_handlers_[ES2CProtocol.ES2CProtocol_ChooseHero] = function(client_id, msg)
	-- print("Recv: ES2CProtocol_ChooseHero");
	local index = 2;
	local player_id = msg[index];
	index = index +1;
	local hero_id = msg[index];
	index = index +1;
	local skin = msg[index];
	index = index +1;
	local client = protected_.clients_[client_id];
	local member = client.cur_troop_[player_id];
	member.hero_ = hero_id;
	client_recall_("onChooseHero", client_id, member, hero_id, skin);
	return 0;
end

protected_.msg_handlers_[ES2CProtocol.ES2CProtocol_ChooseTrump] = function(client_id, msg)
    local index = 2;
    local index = 2;
	local player_id = msg[index];
	index = index +1;
	local trump_id = msg[index];
	index = index +1;
	local client = protected_.clients_[client_id];
	local member = client.cur_troop_[player_id];
	member.trump_ = trump_id;
	client_recall_("onChooseTrump", client_id, member, trump_id);
	return 0;
end

protected_.msg_handlers_[ES2CProtocol.ES2CProtocol_ConfirmChooseHero] = function(client_id, msg)
	-- print("Recv: ES2CProtocol_ConfirmChooseHero");
	local index = 2;
	local player_id = msg[index];
	index = index +1;
	local hero_id = msg[index];
	index = index +1;
	local skin = msg[index];
	index = index +1;
	local client = protected_.clients_[client_id];
	local member = client.cur_troop_[player_id];
	member.hero_ = hero_id;
	client_recall_("onChooseHeroConfirm", client_id, member, hero_id, skin);
	return 0;
end

protected_.msg_handlers_[ES2CProtocol.ES2CProtocol_CancelChooseHero] = function(client_id, msg)
	-- print("Recv: EC2SProtocol_CancelChooseHero");
	local index = 2;
	local player_id = msg[index];
	index = index +1;
	local hero_id = msg[index];
	index = index +1;
	local client = protected_.clients_[client_id];
	local member = client.cur_troop_[player_id];
	member.hero_ = hero_id;
	client_recall_("onChooseHeroCancel", client_id, member, hero_id);
	return 0;
end

protected_.msg_handlers_[ES2CProtocol.ES2CProtocol_ChoosePet] = function(client_id, msg)
	-- print("Recv: ES2CProtocol_ChoosePet");
	local index = 2;
	local player_id = msg[index];
	index = index +1;
	local sel_pet = msg[index];
	index = index +1;
	local client = protected_.clients_[client_id];
	local member = client.cur_troop_[player_id];
	member.pet_ = sel_pet; -- pet index 1---3
	local pet = member["pet"..sel_pet.."_"];
	client_recall_("onChoosePet", client_id, member, pet);
	return 0;
end

protected_.msg_handlers_[ES2CProtocol.ES2CProtocol_SetFormation] = function(client_id, msg)
	-- print("Recv: ES2CProtocol_SetFormation");
	local index = 2;
	local form_id = msg[index];
	index = index +1;
	local client = protected_.clients_[client_id];
	client.cur_troop_.formation_ = form_id;
	client_recall_("onSetFormation", client_id, form_id);
	return 0;
end

protected_.msg_handlers_[ES2CProtocol.ES2CProtocol_SetMemPos] = function(client_id, msg)
	-- print("Recv: ES2CProtocol_SetMemPos");
	local index = 2;
	local mem_id = msg[index];
	index = index +1;
	local x = msg[index];
	index = index +1;
	local y = msg[index];
	index = index +1;
	local client = protected_.clients_[client_id];
	local member = client.cur_troop_[mem_id];
	member.x_ = x;
	member.y_ = y;
	client_recall_("onSetMemPos", client_id, member, x, y);
	return 0;
end

protected_.msg_handlers_[ES2CProtocol.ES2CProtocol_ConfirmFight] = function(client_id, msg)
	-- print("Recv: ES2CProtocol_ConfirmFight");
	local index = 2;
	local corp_id = msg[index];
	index = index +1;
	client_recall_("onConfirmFight", client_id, corp_id);
	return 0;
end


protected_.msg_handlers_[ES2CProtocol.ES2CProtocol_StartField] = function(client_id, msg)
    print("Recv: ES2CProtocol_StartField");
    local client = protected_.clients_[client_id];
	if client==nil then
		return 0;
	end
	local index = 2;
	local fight_sid = msg[index];
	index = index +1;
	local fight = msg[index];
	index = index +1;
	local troop1 = msg[index];
	index = index +1;
	local troop2 = msg[index];
	index = index +1;
	protected_.MetaFight.createFight(client, fight, troop1, troop2);
	return 0;
end

protected_.msg_handlers_[ES2CProtocol.ES2CProtocol_InitField] = function(client_id, msg)
    print("Recv: ES2CProtocol_InitField");
    local client = protected_.clients_[client_id];
	if client==nil then
		return 0;
	end
	protected_.MetaFight.createFightWorker(client, msg, 2);
	client_recall_("onCreateFight", client_id);
	return 0;
end

--protected_.msg_handlers_[ES2CProtocol.ES2CProtocol_PrepareRound] = function(client_id, msg)
--	print("Recv: ES2CProtocol_PrepareRound");
--	local index = 2;
--	local round_count = msg[index];
--	index = index +1;
--	local exp_time = msg[index];
--	index = index +1;
--	client_recall_("onPrepareRound", client_id, round_count, exp_time);
--	return 0;
--end

--战斗相关
protected_.msg_handlers_[ES2CProtocol.ES2CProtocol_Fight] = function(client_id, msg)
    -- print("Recv: ES2CProtocol_Fight");
	local fight_handle_time_0 = socket.gettime()*1000;

	local ret = protected_.MetaFight.handleMessage(client_id, msg, 2);

	local fight_handle_time_1 = socket.gettime()*1000;
	if fight_handle_time_1-fight_handle_time_0>=5 then
		print("protected_.MetaFight.handleMessage", fight_handle_time_1-fight_handle_time_0, msg[2]);
	end
	return ret;
end
protected_.msg_handlers_[ES2CProtocol.ES2CProtocol_MessageFlag] = function(client_id, msg)
	local index = 2;
	local fight_id = msg[index];
	index = index +1;
	local msg_flag = msg[index];
	index = index +1;
	local len = #msg_flag;
	gameError("ES2CProtocol_MessageFlag["..len.."]: "..string.sub(msg_flag, len-128, len));
	return 0;
end

protected_.msg_handlers_[ES2CProtocol.ES2CProtocol_FightClose] = function(client_id, msg)
	local index = 2;
	local fight_id = msg[index];
	index = index +1;
	
--	local client = protected_.clients_[client_id];
--	client.fight_ = nil;
--	client.troops_ = {};
--	client.cur_troop_ = nil;
	return 0;
end
protected_.msg_handlers_[ES2CProtocol.ES2CProtocol_Attack] = function(client_id, msg)
	local index = 2;
	local unit_id = msg[index] or 0;
	index = index +1;
	local skill_indx = msg[index] or 0;
	index = index +1;
	local target_id = msg[index] or 0;
	index = index +1;
	local corp_id = msg[index] or 0;
	index = index +1;
	local slot_id = msg[index] or 0;
	index = index +1;
	
	local client = protected_.clients_[client_id];
	local fight_worker = client.fight_.fight_worker_;
	local unit = fight_worker[unit_id];
	if unit==nil then
		return 0;
	end
	local skill_sid = unit.skill_lists_[skill_indx];
	client_recall_("onSetAttack", client_id, unit, skill_sid, target_id, corp_id, slot_id);
	return 0;
end

protected_.msg_handlers_[ES2CProtocol.ES2CProtocol_ReConnect] = function(client_id, msg)
	local index = 2;
	local reconn_msg = msg[index];
	print("ES2CProtocol_ReConnect ", reconn_msg[1]);
	protected_.msg_reconnect_flag_ = true;
	protected_.msg_handlers_[reconn_msg[1]](client_id, reconn_msg);
	protected_.msg_reconnect_flag_ = false;
	return 0;
end

protected_.msg_handlers_[ES2CProtocol.ES2CProtocol_Close] = function(client_id, msg)
	print("ES2CProtocol.ES2CProtocol_Close");
	client_recall_("onClientClose", client_id);
	return 0;
end

protected_.msg_handlers_[ES2CProtocol.ES2CProtocol_ThumbsUp] = function(client_id, msg)
	-- print("ES2CProtocol.ES2CProtocol_ThumbsUp");
	local index = 2;
	local thumbsup_fight_serverid = msg[index];--点赞的战斗服务器id
	index = index + 1;
	local thumbsup_fight_id = msg[index];--点赞的战斗id
	index = index + 1;
	local thumbsup_target_sid = msg[index];--被点赞的玩家id
	index = index + 1;
	local thumbsup_src_sid = msg[index];--点赞的玩家id
	index = index + 1;
	local thumbsup_src_hero_id = msg[index];--点赞的玩家英雄id
	index = index + 1;
	local client = protected_.clients_[client_id];
	local MysUid = client.suid_;
	if client.fight_~=nil and  client.fight_.fight_worker_~=nil  then --战斗信息还没被清空，说明是在战斗服中匹配成功之后。或者是在gameserver中还开着结算界面。进行下一步判断
		local fight_worker = client.fight_.fight_worker_;
		local local_fight_id = fight_worker.fight_id;--本次战斗id
		local local_fight_server_id = fight_worker.fight_server_id_; --本地fight_serverID
		if local_fight_id and local_fight_id==thumbsup_fight_id and local_fight_server_id==thumbsup_fight_serverid then --可以点赞了
			if thumbsup_src_sid==MysUid then --如果是点赞的玩家客户端，标记该客户端不能再给目标玩家点赞了
				fight_worker[thumbsup_target_sid] = 1;--标记目标玩家被该客户端点赞了
			end
			--在本客户端记录目标玩家被点赞次数
			local thumbs_count = fight_worker.target_thumbs_count or {};
			local count = thumbs_count[thumbsup_target_sid] or 0;
			thumbs_count[thumbsup_target_sid] = count + 1;
			fight_worker.target_thumbs_count = thumbs_count;
			--记录目标点赞的玩家
			if not fight_worker.target_thumbs_player then
				fight_worker.target_thumbs_player = {}; --traget_sid = {src_sid = 1}
			end
			if not fight_worker.target_thumbs_player[thumbsup_target_sid] then
				fight_worker.target_thumbs_player[thumbsup_target_sid] = {};
			end
			fight_worker.target_thumbs_player[thumbsup_target_sid][thumbsup_src_sid] = 1;

			if client.suid_ == thumbsup_src_sid or client.suid_ == thumbsup_target_sid or fight_worker.target_thumbs_player[thumbsup_target_sid][client.suid_] then
				client_recall_("onThumbsUp", client_id, thumbsup_src_sid, thumbsup_target_sid, count + 1, thumbsup_src_hero_id);
			end
		end
	end
	return 0;
end
protected_.msg_handlers_[ES2CProtocol.ES2CProtocol_FightResult] = function(client_id, msg)
	--print("ES2CProtocol.ES2CProtocol_FightResult");
	--printObject(msg);
	local msg_index = 2;
	local fmsg = msg[msg_index];
	msg_index = msg_index + 1;
	local fight_reward = msg[msg_index];
	msg_index = msg_index + 1;
	local results = msg[msg_index] or {};
	msg_index = msg_index + 1;

	local client = protected_.clients_[client_id];
	local cur_player = client.player_;
	local fight_worker = client.fight_.fight_worker_;
	if not fight_worker then
		return 0;
	end
	local fight_result_data = {};
	local index = 2;
	fight_result_data.fight_id = fmsg[index];
	index = index + 1;
	fight_result_data.fight_result = fmsg[index];
	index = index + 1;
	fight_result_data.fight_round = fmsg[index];--战斗回合数
	index = index + 1;
	fight_result_data.fight_time = fmsg[index];--战斗时长
	index = index + 1;
	fight_result_data.corp_0 = fmsg[index];	--corp 0 战力
	index = index + 1;
	fight_result_data.corp_1 = fmsg[index]; --corp 1 战力
	index = index + 1;
	fight_result_data.fight_sid = fmsg[index]; --1v1 2v2
	index = index + 1;

	fight_result_data.fight_type = protected_.getFightType(fight_result_data.fight_sid);

	local data = {};
	local corps_player_data = {};
	corps_player_data[0] = {};
	corps_player_data[1] = {};

	local length = fmsg[index];
	index = index + 1;
	for i = 1, length do
		local element = {};
		element.player_sid = fmsg[index];--玩家id
		index = index + 1;
		element.player_name = fmsg[index];--玩家名字
		index = index + 1;
		element.corp_id = fmsg[index];--队伍id
		index = index + 1;
		element.sunit_id = fmsg[index];--英雄id
		index = index + 1;
		element.pet_suid = fmsg[index];--宠物id
		index = index + 1;
		local count = fmsg[index];
		index = index + 1;
		element.statics_={};
		for i=1,count do
			element.statics_[i]=fmsg[index];
			index = index + 1;
		end
		element.is_mvp = fmsg[index];--1为mvp
		index = index + 1;
		element.mvp_score = tonumber(string.format("%.2f", (fmsg[index] or 0)));--分数
		index = index + 1;
		element.score_rank = fmsg[index];
		index = index + 1;
		element.no_operation = fmsg[index];	--没有操作的最大回合数
		index = index + 1;
        element.trump_id = fmsg[index];
        index = index + 1;
        element.pet_troop_id = fmsg[index];
        index = index + 1;
        element.posy_program_id = fmsg[index];
        index = index + 1;
		element.pet = {};
		for j = 1, configs_.pet_troop_slot_num do
			element.pet[j] = protected_.constructObject(protected_.MetaPet, fmsg[index], nil, nil);
			index = index + 1;
		end
		element.hide_score = fmsg[index];
		index = index + 1;
		corps_player_data[element.corp_id][i] = element;

	end

	local prop_name_reward = {};
	if type(fight_reward) == "table" then
		for k, v in pairs(fight_reward) do
			local prop_name = protected_.getMetaPropName(protected_.MetaGamePlayer, k);
			if prop_name then
				prop_name_reward[prop_name] = v;
			end
		end
	end

	fight_result_data.corps_player_data = corps_player_data;	
	fight_result_data.cur_player_results = protected_.constructObject(protected_.MetaFightResult, results, nil, nil);
	fight_result_data.reward_data = prop_name_reward;
	fight_worker.fight_result_data_ = fight_result_data;
	client_recall_("onFightOverData", client_id, fight_result_data);
	return 0;
end

protected_.msg_handlers_[ES2CProtocol.ES2CProtocol_SetCommander] = function(client_id, msg)
	print("ES2CProtocol_SetCommander")
	local index = 2;
	local des_player_id = msg[index];

	local client = protected_.clients_[client_id];
	local fight_worker = client.fight_.fight_worker_;
	if not fight_worker then
		return 0;
	end
	local troop = client.cur_troop_;
	local old_player_id = client.cur_troop_.commander_;
	troop.commander_ = des_player_id;

	local des_member = troop[des_player_id];

	client_recall_("onCommander", client_id);
	--队长弹得消息
	if client.suid_ == client.cur_troop_.leader_ then
		client_recall_("onHint", client_id, {nil, 1, 10218, des_member.player_name_});
	end
	--以前指挥弹得消息
	if client.suid_ == old_player_id then
		client_recall_("onHint", client_id, {nil, 1, 10219});
	end
	--被设置为指挥官的弹出消息
	if client.suid_ == des_player_id then
		client_recall_("onHint", client_id, {nil, 1, 10220});
	end
	local old_member = troop[old_player_id];
	if old_member then
		client_recall_("onSysChatTroop", client_id, {nil, 1, 10221, old_member.player_name_});
	end
	client_recall_("onSysChatTroop", client_id, {nil, 1, 10222, des_member.player_name_});
	return 0;
end
protected_.msg_handlers_[ES2CProtocol.ES2CProtocol_MarkUnit] = function(client_id, msg)
	print("ES2CProtocol_MarkUnit")
	local index = 2;
	local unit_id = msg[index];
	index = index + 1;
	local mark_content = msg[index];
	index = index + 1;

	local client = protected_.clients_[client_id];
	local fight_worker = client.fight_.fight_worker_;
	if not fight_worker then
		return 0;
	end

	client.cur_troop_.marks_[unit_id] = mark_content;
	client_recall_("onMarkUnit", client_id, unit_id);
	return 0;
end
protected_.msg_handlers_[ES2CProtocol.ES2CProtocol_RemoveMarkUnit] = function(client_id, msg)
	print("ES2CProtocol_RemoveMarkUnit")
	local index = 2;
	local unit_id = msg[index];
	index = index + 1;

	local client = protected_.clients_[client_id];
	local fight_worker = client.fight_.fight_worker_;
	if not fight_worker then
		return 0;
	end

	client.cur_troop_.marks_[unit_id] = nil;
	client_recall_("onMarkUnit", client_id, unit_id);
	return 0;
end
protected_.msg_handlers_[ES2CProtocol.ES2CProtocol_RemoveCommander] = function(client_id, msg)
	print("ES2CProtocol_RemoveCommander")
	local index = 2;
	local unit_id = msg[index];
	index = index + 1;

	local client = protected_.clients_[client_id];
	local fight_worker = client.fight_.fight_worker_;
	if not fight_worker then
		return 0;
	end
	local troop = client.cur_troop_;
	local old_player_id = client.cur_troop_.commander_;
	client.cur_troop_.commander_ = 0;
	local old_member = troop[old_player_id];
	client_recall_("onCommander", client_id);
	--队长弹得消息
	if client.suid_ == client.cur_troop_.leader_ then
		client_recall_("onHint", client_id, {nil, 1, 10224, old_member.player_name_});
	end
	--以前指挥弹得消息
	if client.suid_ == old_player_id then
		client_recall_("onHint", client_id, {nil, 1, 10219});
	end
	if old_member then
		client_recall_("onSysChatTroop", client_id, {nil, 1, 10221, old_member.player_name_});
	end
	return 0;
end
protected_.msg_handlers_[ES2CProtocol.ES2CProtocol_ConfirmPetTroop] = function(client_id, msg)
	print("ES2CProtocol_ConfirmPetTroop")
	local index = 2;
	local player_suid = msg[index];
	index = index + 1;
	local pet_troop_id = msg[index];
	index = index + 1;
	local pet1 = msg[index];
	index = index + 1;
	local pet2 = msg[index];
	index = index + 1;
	local pet3 = msg[index];
	index = index + 1;

	local client = protected_.clients_[client_id];
	if client.fight_ and client.fight_.fight_worker_ then
		return 0;
	end
	local member = client.cur_troop_[player_suid];
	if not member then
		return 0;
	end
	member.pet_troop_id_ = pet_troop_id;
	if #pet1>0 then
		protected_.unserialObject(protected_.MetaPet, pet1, member, "pet1_");
	else
		member.pet1_ = nil;
	end
	if #pet2>0 then
		protected_.unserialObject(protected_.MetaPet, pet2, member, "pet2_");
	else
		member.pet2_ = nil;
	end
	if #pet3>0 then
		protected_.unserialObject(protected_.MetaPet, pet3, member, "pet3_");
	else
		member.pet3_ = nil;
	end
	-- log.to_file(member, "client_member.lua");
	client_recall_("onConfirmPetTroop", client_id, player_suid);
	return 0;
end
protected_.msg_handlers_[ES2CProtocol.ES2CProtocol_FightClientLoad] = function(client_id, msg)
	print("ES2CProtocol_FightClientLoad")
	local index = 2;
	local player_suid = msg[index];
	index = index + 1;
	local load_rate = msg[index];
	index = index + 1;
	client_recall_("onClientLoad", client_id, player_suid, load_rate);
end

protected_.msg_handlers_[ES2CProtocol.ES2CProtocol_MatchFailed] = function(client_id, msg)
	local index = 2;
	local fight_id = msg[index];
	index = index +1;
    local client = protected_.clients_[client_id];
	client_recall_("onMatchFailed", client_id, fight_id);
--	local client = protected_.clients_[client_id];
--	client.fight_ = nil;
	client.troops_ = {};
	client.cur_troop_ = nil;
	return 0;
end

protected_.msg_handlers_[ES2CProtocol.ES2CProtocol_ReJoinChannel] = function(client_id, msg)
	print("Recv: ES2CProtocol_ReJoinChannel");
	local index = 2;
	local channel_id=msg[index];
	index = index+1;
	local ret_val=msg[index];
	index = index+1;
	client_recall_("onJoinChannel", client_id, channel_id, ret_val);
	return 0;
end

protected_.msg_handlers_[ES2CProtocol.ES2CProtocol_ReLeaveChannel] = function(client_id, msg)
	-- print("Recv: ES2CProtocol_ReLeaveChannel");
	local index = 2;
	local channel_id=msg[index];
	index = index+1;
	client_recall_("onLeaveChannel", client_id, channel_id);
	return 0;
end
protected_.msg_handlers_.unpackChatMessage = function(msg, index)
	local chat_player = {};
	chat_player.player_name=msg[index];
	index = index+1;
	chat_player.suid=msg[index];
	index = index+1;
	chat_player.chat_time=msg[index];
	index = index+1;
	local channel_id=msg[index];
	index = index+1;
	local chat_str=msg[index];
	index = index+1;
	return channel_id, chat_player, chat_str, index;
end
protected_.msg_handlers_[ES2CProtocol.ES2CProtocol_ChatInChannel] = function(client_id, msg)
	-- print("Recv: ES2CProtocol_ChatInChannel");
	local channel_id, chat_player, chat_str, index = protected_.msg_handlers_.unpackChatMessage(msg, 2);

	client_recall_("onChatInChannel", client_id, channel_id, chat_player, chat_str);
	return 0;
end

protected_.msg_handlers_[ES2CProtocol.ES2CProtocol_ChatInTroop] = function(client_id, msg)
	-- print("Recv: ES2CProtocol_ChatInTroop");
	local channel_id, chat_player, chat_str, index = protected_.msg_handlers_.unpackChatMessage(msg, 2);
	client_recall_("onChatInTroop", client_id, channel_id, chat_player, chat_str);
	return 0;
end

protected_.msg_handlers_[ES2CProtocol.ES2CProtocol_ChatToPlayer] = function(client_id, msg)
	-- print("Recv: ES2CProtocol_ChatInTroop");
	local channel_id, chat_player, chat_str, index = protected_.msg_handlers_.unpackChatMessage(msg, 2);
	client_recall_("onChatToPlayer", client_id, channel_id, chat_player, chat_str);
	return 0;
end

protected_.msg_handlers_[ES2CProtocol.ES2CProtocol_ChatToRoom] = function(client_id, msg)
	-- print("Recv: ES2CProtocol_ChatToRoom");
	local channel_id, chat_player, chat_str, index = protected_.msg_handlers_.unpackChatMessage(msg, 2);
	client_recall_("onChatToRoom", client_id, channel_id, chat_player, chat_str);
	return 0;
end

protected_.msg_handlers_[ES2CProtocol.ES2CProtocol_CreateAlliance] = function(client_id, msg)
    local alliance_id = protected_.msg_handlers_.unpackChatMessage(msg, 2);
    print("create success")
    return 0
end

protected_.msg_handlers_[ES2CProtocol.ES2CProtocol_JoinAlliance] = function(client_id, msg)
    local alliance_id = protected_.msg_handlers_.unpackChatMessage(msg, 2);
    print("join success")
    return 0

end

protected_.msg_handlers_[ES2CProtocol.ES2CProtocol_LeaveAlliance] = function(client_id, msg)
    local alliance_id = protected_.msg_handlers_.unpackChatMessage(msg, 2);
    print("leave success")
    return 0
end

protected_.msg_handlers_[ES2CProtocol.ES2CProtocol_FightResultWinRate] = function(client_id, msg)
   -- printObject(msg)
    local index = 2
    local fight_ret_table =msg[index]

    local index_ret = 1
    local fight_type = fight_ret_table[index_ret]
    index_ret = index_ret + 1
    local total_num = fight_ret_table[index_ret]
    index_ret = index_ret + 1
    local win_num = fight_ret_table[index_ret]
    print("FightResultWinRate", fight_type)
    print(total_num)
    print(win_num)
end

protected_.msg_handlers_[ES2CProtocol.ES2CProtocol_TroopsMembersInfo] = function(client_id, msg)
   -- printObject(msg)
    print("ES2CProtocol_TroopsMembersInfo success")
end

protected_.msg_handlers_[ES2CProtocol.ES2CProtocol_RequestRankInfo] = function(client_id, msg)
    --printObject(msg)
    local top_map = {}
    local rank_info = msg[2]
    top_map.top_id_ = rank_info.top_id_
	top_map.top_lists_ = rank_info.top_lists_
    top_map.top_high_ = rank_info.top_high_
    top_map.top_max_ = rank_info.top_max_
	top_map.top_low_ = rank_info.top_low_
	top_map.requester_pos_ = rank_info.requester_pos_
    client_recall_("onRequestRankInfo", client_id, top_map)
    print("ES2CProtocol_RequestRankInfo success")
end

protected_.msg_handlers_[ES2CProtocol.ES2CProtocol_DialogOpen] = function(client_id, msg)
	local client = protected_.clients_[client_id];
	if client==nil or client.player_==nil then
		return 0;
	end
	if client.fight_ and client.fight_.fight_worker_ 
		and client.fight_.fight_worker_.worker_state_>0 
		and client.fight_.fight_worker_.worker_state_<10 
		and client.fight_.fight_worker_.state_==protected_.MetaFight.EFightState_Runing then
		table.insert(msg, 2, Enum.EFightProto_DialogOpen);
		table.insert(msg, 3, 0);
		protected_.MetaFight.handleMessage(client_id, msg, 2);
	else
		local index = 2;
		local dialog_sid = msg[index];
		index = index + 1;
		local dialog_context = msg[index];
		index = index + 1;
		local field_id = msg[index];
		index = index + 1;
		local unit_id = msg[index];
		index = index + 1;
			
		local dialog = protected_.constructObject(protected_.MetaDialog);
		dialog:open(client.player_, dialog_sid, dialog_context, field_id, unit_id);

		client_recall_("onDialogOpen", client_id, dialog_sid, dialog_context, field_id, unit_id);
	end


	return 0;
end

protected_.msg_handlers_[ES2CProtocol.ES2CProtocol_DialogClose] = function(client_id, msg)
	local client = protected_.clients_[client_id];
	if client==nil or client.player_==nil then
		return 0;
	end
    local index = 2;
	local dialog_sid = msg[index];
	index = index + 1;
	if client.player_.dialog_==nil then
		return 0;
	end
	if client.player_.dialog_.dialog_sid_~=dialog_sid then
		return 0;
	end
	client_recall_("onDialogClose", client_id, dialog_sid);
end

protected_.msg_handlers_[ES2CProtocol.ES2CProtocol_ChooseTroopReadyStatus] = function(client_id, msg)
    local index = 2
    if not protected_.clients_[client_id] or not protected_.clients_[client_id].player_ then
        return 0
    end
    local player_id = msg[index]
    index = index + 1
    local onhook_status_ = msg[index]
    client_recall_("onChooseTroopReadyStatus", client_id, onhook_status_, player_id)
    print("ES2CProtocol_ChooseTroopReadyStatus success")
end

protected_.msg_handlers_[ES2CProtocol.ES2CProtocol_ModifyTroopReadyStatus] = function(client_id, msg)
    local index = 2
    if not protected_.clients_[client_id] or not protected_.clients_[client_id].player_ then
        return 0
    end
    local player_id = msg[index]
    index = index + 1
    local onhook_status_ = msg[index]
    client_recall_("onModifyTroopReadyStatus", client_id, onhook_status_, player_id)
    print("ES2CProtocol_ModifyTroopReadyStatus success")
end

protected_.msg_handlers_[ES2CProtocol.ES2CProtocol_KickOffFromTroop] = function(client_id, msg)
    local index = 2
    if not protected_.clients_[client_id] or not protected_.clients_[client_id].player_ then
        return 0
    end
    local player_id = msg[index]
    index = index + 1
    local troop_id = msg[index]
    client_recall_("onKickOffFromTroop", client_id, troop_id, player_id)
    print("ES2CProtocol_KickOffFromTroop success")
end

protected_.msg_handlers_[ES2CProtocol.ES2CProtocol_TroopUseDialogue] = function(client_id, msg)
    local index = 2
    if not protected_.clients_[client_id] or not protected_.clients_[client_id].player_ then
        return 0
    end
    local speaker_uid = msg[index]
    client_recall_("onTroopUseDialogue", client_id, speaker_uid)
    print("ES2CProtocol_TroopUseDialogue speaker_id----"..speaker_uid)
end

protected_.msg_handlers_[ES2CProtocol.ES2CProtocol_Upgrade] = function(client_id, msg)
    local index = 2
    if not protected_.clients_[client_id] or not protected_.clients_[client_id].player_ then
        return 0
    end
    local level = msg[index]
    print("ES2CProtocol_Upgrade level modify")
    client_recall_("onUpgrade", client_id, level)
end

protected_.msg_handlers_[ES2CProtocol.ES2CProtocol_FieldCountersInit] = function(client_id, msg)
	print("recv ES2CProtocol_FieldCountersInit");
	local client = protected_.clients_[client_id];
	if client==nil or client.player_==nil then
		return 0;
	end
    local index = 2;
	local stage = msg[index];
	index = index + 1;
	if client.fight_==nil then
		return 0;
	end
	local fight_data = configs_.FightField[client.fight_.fight_sid_];
	if fight_data==nil then
		return 0;
	end
	local corp_id = client.fight_.my_corp_id_+1;
	local counter_stage = fight_data.stages[stage];
	if counter_stage==nil then
		return 0;
	end
	local counters = counter_stage[corp_id];
	if  counters==nil then
		return 0;
	end

	client.fight_.stage_ = stage;
	client.fight_.counters_={};
	for i, v in ipairs(counters) do
		client.fight_.counters_[i] = {0, v[2]};
	end
    client_recall_("onFieldCounterInit", client_id, fight_data);
	return 0;
end

protected_.msg_handlers_[ES2CProtocol.ES2CProtocol_FieldCounter] = function(client_id, msg)
	print("############################################# recv ES2CProtocol_FieldCounter");
	local client = protected_.clients_[client_id];
	if client==nil or client.player_==nil then
		return 0;
	end
	if client.fight_==nil then
		return 0;
	end
    local index = 2;
	local counter_indx = msg[index];
	index = index + 1;
	local event_counter = msg[index];
	index = index + 1;
	local counter_tbl = client.fight_.counters_[counter_indx];
	if counter_tbl==nil then
		return 0;
	end
	counter_tbl[1] = event_counter;
    client_recall_("onFieldCounter", client_id, counter_indx, counter_tbl);
	return 0;
end

protected_.msg_handlers_[ES2CProtocol.ES2CProtocol_FieldCounterClose] = function(client_id, msg)
	print("recv ES2CProtocol_FieldCounterClose");
	local client = protected_.clients_[client_id];
	if client==nil or client.player_==nil then
		return 0;
	end
	if client.fight_==nil then
		return 0;
	end
    client_recall_("onFieldCounterClose", client_id, client.fight_.counters_);
	client.fight_.stage_ = nil;
	client.fight_.counters_ = nil;
	return 0;
end

protected_.msg_handlers_[ES2CProtocol.ES2CProtocol_WaitIntoMatch] = function(client_id, msg)
    print("recv ES2CProtocol_WaitIntoMatch");
    local client = protected_.clients_[client_id];
    if client == nil or client.player_ == nil then
        return 0;
    end
    local index = 2;
    local troop1 = msg[index];
    index = index + 1;
    local troop2 = msg[index];
    index = index + 1;
    local matchtroop_1 = protected_.unserialObject(protected_.MetaTroop, troop1);
    local matchtroop_2 = protected_.unserialObject(protected_.MetaTroop, troop2);
    local find = false;
    for k,v in pairs (matchtroop_1.map_) do
		if k == client.suid_ then
            client.cur_troop_ = matchtroop_1;
            print("1++++");
            find = true;
        end
	end
    if find == false then
        for k,v in pairs (matchtroop_2.map_) do
		    if k == client.suid_ then
                client.cur_troop_ = matchtroop_2;
                print("2++++");
                find = true;
            end
	    end
    end
    if find == false then
        return 0;
    end
    if client.troops_==nil then
		client.troops_ = {};
	end
    client.troops_[matchtroop_1.id_] = matchtroop_1;
    client.troops_[matchtroop_2.id_] = matchtroop_2;
    local mem_list = {};
	client.cur_troop_.list = mem_list;
	for k,v in pairs(client.cur_troop_.map_) do
		mem_list[v.troop_pos_] = k;
        print("~~~~~~~~~~~~~", v.troop_pos_, k);
	end
    client_recall_("onWaitIntoMatch", client_id, matchtroop_1, matchtroop_2);
end


protected_.msg_handlers_[ES2CProtocol.ES2CProtocol_ReadyToMatch] = function(client_id, msg)
    local client = protected_.clients_[client_id];
    if client == nil or client.player_ == nil then
        return 0;
    end
    local index = 2
    local troop_id = msg[index];
    index = index + 1;
    local player_id = msg[index];
    index = index + 1;
    local troop = client.troops_[troop_id];
    if troop == nil then
        return 0;
    end
    local member = troop[player_id];
    if member == nil then
        return 0;
    end
    client_recall_("onReadyToMatch", client_id, troop, member);
    print("ES2CProtocol_ReadyToMatch success");
end

protected_.msg_handlers_[ES2CProtocol.ES2CProtocol_FailedReadyMatch] = function(client_id, msg)
    local index = 2;
	local fight_id = msg[index];
	index = index +1;
    local player_id = msg[index];
    index = index + 1;
    local hook_state = msg[index];
    index = index + 1;
    local client = protected_.clients_[client_id];
	client_recall_("onFailedReady", client_id);
--	local client = protected_.clients_[client_id];
--	client.fight_ = nil;
	client.troops_ = {};
	client.cur_troop_ = nil;
	return 0;
end

protected_.msg_handlers_[ES2CProtocol.ES2CProtocol_UpdateMemberScore] = function(client_id, msg)
    local index = 2;
    if not protected_.clients_[client_id] or not protected_.clients_[client_id].player_ then
        return 0
    end
    local memberid = msg[index];
    index = index + 1;
    local socre = msg[index];
    index = index + 1;
    local rank_score = msg[index];
    index = index + 1;
    local client = protected_.clients_[client_id];
    local member = client.cur_team_troop_[memberid];
    if member == nil then
        return 0;
    end
    member.score_ = score;
    member.rank_score_ = rank_score;
    return 0;
end

protected_.msg_handlers_[ES2CProtocol.ES2CProtocol_StopMatch] = function(client_id, msg)
    local client = protected_.clients_[client_id];
    if client == nil or client.player_ == nil then
        return 0;
    end
    if client.cur_team_troop_ == nil then
        return 0;
    end
    if client.cur_team_troop_.state_ ~= Enum.ETroop_Math then
        return 0;
    end
    client.cur_team_troop_.stage_ = Enum.ETroop_Create;
    client_recall_("onStopMatch", client_id);
end

protected_.msg_handlers_[ES2CProtocol.ES2CProtocol_QuestStateChange] = function(client_id, msg)
    local index = 2
    if not protected_.clients_[client_id] or not protected_.clients_[client_id].player_ then
        return 0
    end
    local group_id = msg[index]
    index = index + 1
    local quest_sid = msg[index]
    index = index + 1
    local state = msg[index]
    index = index + 1
    local quest_level = msg[index]
    print("ES2CProtocol_QuestStateChange success[", group_id, quest_sid, state, quest_level, "]")
    client_recall_("onQuestStateChange", client_id, group_id, quest_sid, state, quest_level)
    return 0
end

protected_.msg_handlers_[ES2CProtocol.ES2CProtocol_ConfirmPosyProgram] = function(client_id, msg)
	print("ES2CProtocol_ConfirmPosyProgram")
	local index = 2;
	local program_id = msg[index];
	index = index + 1;

	local client = protected_.clients_[client_id];
	if client.fight_ and client.fight_.fight_worker_ then
		return 0;
	end
	local player_suid = client.player_:getUID();
	local member = client.cur_troop_[player_suid];
	if not member then
		return 0;
	end
	member.posy_program_ = client.player_.posy_sys_[program_id];
	-- log.to_file(member, "client_member.lua");
	client_recall_("onConfirmPosyProgram", client_id, program_id);
	return 0;
end

protected_.msg_handlers_[ES2CProtocol.ES2CProtocol_QuestLoginInfo] = function(client_id, msg)
    local index = 2
    if not protected_.clients_[client_id] or not protected_.clients_[client_id].player_ then
        return 0
    end
    local quest_info = msg[index]
    print("ES2CProtocol_QuestLoginInfo success")
    client_recall_("onInitQuestInfo", client_id, quest_info)
    return 0
end

protected_.msg_handlers_[ES2CProtocol.ES2CProtocol_GetLevelReward] = function(client_id, msg)
    local index = 2
    if not protected_.clients_[client_id] or not protected_.clients_[client_id].player_ then
        return 0
    end
    local reward_level = msg[index]
    print("ES2CProtocol_GetLevelReward success")
    client_recall_("onGetLevelReward", client_id, reward_level)
    return 0
end

protected_.msg_handlers_[ES2CProtocol.ES2CProtocol_BuyHeroSkin] = function(client_id, msg)
    local index = 2
    if not protected_.clients_[client_id] or not protected_.clients_[client_id].player_ then
        return 0
    end
    local hero_id = msg[index]
    index = index + 1
    local skin_id = msg[index]
    print("EC2SProtocol_BuyHeroSkin success", hero_id, skin_id)
    client_recall_("onBuyHeroSkin", hero_id, skin_id)
    return 0
end

protected_.msg_handlers_[ES2CProtocol.ES2CProtocol_UseHeroSkin] = function(client_id, msg)
    local index = 2
    if not protected_.clients_[client_id] or not protected_.clients_[client_id].player_ then
        return 0
    end
    local hero_id = msg[index]
    index = index + 1
    local skin_id = msg[index]
    print("EC2SProtocol_UseHeroSkin success", hero_id, skin_id)
    client_recall_("onUseHeroSkin", hero_id, skin_id)
    return 0
end