
console_cmds_ = {};
protected_.callConsoleCommand = function (cmd_name, params)
	local call_func = console_cmds_[cmd_name];
	if call_func==nil then
		print("Command [" .. cmd_name .."] not exist: ", unpack(params));
	end

	if call_func(params)==0 then
		--print("Do command [" .. cmd_name .."] success: ", unpack(params));
	else
		--print("Do command [" .. cmd_name .."] failed: ", unpack(params));
	end
end
function doConsoleCommand(cmd_name, cmd_str)
	print("doConsoleCommand: ", cmd_name, cmd_str);
	if #cmd_str==0 then
		cmd_str = "{"..cmd_str.."}";
	else
		local ch = string.find(cmd_str, "%s*{");
		if ch~=1 then
			local ss = string.gsub(cmd_str, "[^%s]+", "{\"%1\"", 1);
			ss = string.gsub(ss, "([^%s]+)%s+", "%1,");
			cmd_str = ss.."}";
			print("parse str: ", cmd_name, cmd_str);
		end 
	end
	local ret, params = pcall (loadstring('return '..cmd_str));
	if not ret or not params then
		print("doConsoleCommand error:", params);
		return;
	end
--    if protected_.cur_client_id_ ~= nil and protected_.console_connect_type == 1 then
--        local client = protected_.clients_[protected_.cur_client_id_]
--        if client.conn_state_ == 1 and client.client_state_ == nil then
--            login_console_cmds_(cmd_name, params);
--            return;
--        end
--        if client.conn_state_ == 1 and client.client_state_ == 2 then
--           createchar_console_cmds(cmd_name, params);
--           return;
--        end
--    end
	protected_.callConsoleCommand(cmd_name, params);
end

doLuaCommand = function(cmd_name, ...)
	protected_.callConsoleCommand(cmd_name, {...});
end


console_cmds_["connect"] = function (params)
	local client_id = params[1] or (protected_.gen_client_id());
	local config_server_ip = params[2] or "127.0.0.1";
	local config_server_port = params[3] or 20012;
    protected_.console_connect_type = 2;
    --local conn_ret = protected_.NetWork:connect_server(client_id, config_server_ip, config_server_port);
    local conn_ret = lua_connect_server(client_id, config_server_ip, config_server_port);
	client_recall_("onConnectResult", client_id, conn_ret, config_server_ip, config_server_port);
	return 0;
end

console_cmds_["discon"] = function (params)
--    protected_.NetWork:disconnect_server(params[1] or protected_.cur_client_id_);
    lua_disconnect_server(params[1] or protected_.cur_client_id_);
	return 0;
end

console_cmds_["GM"] = function (params)
	if protected_.cur_client_id_==nil then
		print("Cur client not exist!!!");
	end
	table.insert(params, 1, EC2SProtocol["EC2SProtocol_GM"]);
	sendMessage(protected_.cur_client_id_, params);
	return 0;
end
console_cmds_["send"] = function (params)
    local cmsg = params;
	local lua_t = type(cmsg[1]);
	if lua_t=='string' then
		cmsg[1] = EC2SProtocol[cmsg[1]];
	end
	if type(cmsg[1])~='number' then
		return 0;
	end
	sendMessage(protected_.cur_client_id_, cmsg);
	return 0;
end

console_cmds_["login"] = function (params)
	local index = 1;
    local client_id = tonumber(params[index]) or (protected_.gen_client_id());
	index = index + 1;
	local acc_name = params[index];
	index = index + 1;
	if acc_name then
		protected_.clients_[client_id].account_name_ = acc_name;
	end
	if protected_.clients_[client_id].account_name_==nil then
		protected_.clients_[client_id].account_name_ = "test-" .. (protected_.client_beg_+client_id-1);
	end
    protected_.clients_[client_id].client_state_ = 1;
    local smsg = {EC2SProtocol["EC2SProtocol_Login"], protected_.clients_[client_id].account_name_, "1"};
    sendMessage(client_id, smsg);
    return 0;
end

console_cmds_["createPlayer"] = function (params)
	local index = 1;
    local client_id = tonumber(params[index]) or (protected_.gen_client_id());
	index = index + 1;
	local player_name = params[index];
	index = index + 1;

	local smsg = {EC2SProtocol["EC2SProtocol_CreateChar"], player_name};--"alliance_"..client_id+1000
	sendMessage(client_id, smsg);
    return 0;
end

console_cmds_["set_cur"] = function (params)
    local client_id = params[1] or protected_.cur_client_id_;
    if protected_.clients_[client_id] == nil then
        print("error cur client_id");
    end
    protected_.cur_client_id_ = client_id;
	protected_.cur_client_ = protected_.clients_[client_id];
    return 0;
end

console_cmds_["reconnect"] = function (params)
    local client_id = params[1] or 1;
    local config_server_ip = params[2] or "127.0.0.1";
	local config_server_port = params[3] or 20012;
    local conn_ret = protected_.NetWork:connect_server(client_id, config_server_ip, config_server_port);
	client_recall_("onReconnectResult", client_id, conn_ret, config_server_ip, config_server_port);
    return 0;
end

console_cmds_["enterWorld"] = function (params)
    local client_id = params[1] or protected_.cur_client_id_ or 1;
	local smsg = {EC2SProtocol["EC2SProtocol_EnterWorld"]};
	sendMessage(client_id, smsg);

--	local client = protected_.clients_[client_id];
--	if client==nil then
--		return 0;
--	end
--	protected_.MetaDialog.tryPlayerDialog(client.player_.player_id_, 1);
    return 0;
end

console_cmds_["adjustTime"] = function (params)
    local client_id = params[1] or protected_.cur_client_id_ or 1;
	smsg = {EC2SProtocol["EC2SProtocol_ServerTime"]};
	sendMessage(client_id, smsg);
    return 0;
end

login_console_cmds_ = function(cmd_name, params)
    local account = tostring(cmd_name) or ("test" .. protected_.cur_client_id_);
    local password = tostring(params[1]) or "123456";
    protected_.on_login_(protected_.cur_client_id_, account, password);
end

createchar_console_cmds = function (cmd_name, params)
    local charname = tostring(cmd_name) or ("test" .. protected_.cur_client_id_);
    protected_.on_create_char_(protected_.cur_client_id_, charname);
end

console_cmds_["show_prop"] = function(params)
    local heroid = params or 1;
    protected_.show_hero(heroid);
end

console_cmds_["createTroop"] = function (params)
	local indx = 1;
    local client_id = params[indx] or protected_.cur_client_id_ or 1;
	indx = indx+1;
	local fight_sid = params[indx] or 1;
	indx = indx+1;
	local cmsg = {EC2SProtocol["EC2SProtocol_CreateTroop"], fight_sid};
	sendMessage(client_id, cmsg);
	return 0;
end

console_cmds_["newcreateTroop"] = function (params)
	local indx = 1;
    local client_id = params[indx] or protected_.cur_client_id_ or 1;
	indx = indx+1;
	local fight_sid = params[indx] or 1;
	indx = indx+1;
	local cmsg = {EC2SProtocol["EC2SProtocol_NewCreateTroop"], fight_sid};
	sendMessage(client_id, cmsg);
	return 0;
end

console_cmds_["quitTroop"] = function (params)
	local indx = 1;
    local client_id = params[indx] or protected_.cur_client_id_ or 1;
	indx = indx+1;
	local client = protected_.clients_[client_id];
	if client.cur_troop_==nil or client.cur_troop_.state_>Enum.ETroop_Math then
		--return 0;
	end
	local cmsg = {EC2SProtocol["EC2SProtocol_QuitTroop"]};
	sendMessage(client_id, cmsg);
	return 0;
end

console_cmds_["troopInvite"] = function (params)
	local indx = 1;
    local client_id = params[indx] or protected_.cur_client_id_ or 1;
	indx = indx+1;
	local target_name = params[indx];
	indx = indx+1;
	local cmsg = {EC2SProtocol["EC2SProtocol_TroopInvite"], target_name};
	sendMessage(client_id, cmsg);
	return 0;
end

console_cmds_["joinTroop"] = function (params)
	local indx = 1;
    local client_id = params[indx] or protected_.cur_client_id_ or 1;
	indx = indx+1;
	local invite_suid = params[indx];
	indx = indx+1;
	local confirm_flag = params[indx];
	indx = indx+1;
	local fight_server_id = params[indx];
	indx = indx+1;
    local time_refuse_flag = params[indx]
    indx = indx+1
	local cmsg = {EC2SProtocol["EC2SProtocol_JoinTroop"], invite_suid, confirm_flag, fight_server_id, time_refuse_flag};
	sendMessage(client_id, cmsg);
	return 0;
end

console_cmds_["matchTroop"] = function (params)
	local indx = 1;
    local client_id = params[indx] or protected_.cur_client_id_ or 1;
	indx = indx+1;
	local cmsg = {EC2SProtocol["EC2SProtocol_MatchTroop"]};
	sendMessage(client_id, cmsg);
	return 0;
end

console_cmds_["newmatchTroop"] = function (params)
	local indx = 1;
    local client_id = params[indx] or protected_.cur_client_id_ or 1;
	indx = indx+1;
	local cmsg = {EC2SProtocol["EC2SProtocol_NewMatchTroop"]};
	sendMessage(client_id, cmsg);
	return 0;
end

console_cmds_["rankmatchTroop"] = function (params)
	local indx = 1;
    local client_id = params[indx] or protected_.cur_client_id_ or 1;
	indx = indx+1;
	local cmsg = {EC2SProtocol["EC2SProtocol_RankMatchTroop"]};
	sendMessage(client_id, cmsg);
	return 0;
end

console_cmds_["chooseHero"] = function (params)
	local indx = 1;
    local client_id = params[indx] or protected_.cur_client_id_ or 1;
	indx = indx+1;
	local hero_sid = params[indx];
	indx = indx+1;
	local cmsg = {EC2SProtocol["EC2SProtocol_ChooseHero"], hero_sid};
	sendMessage(client_id, cmsg);
	return 0;
end

console_cmds_["chooseTrump"] = function (params)
    local index = 1;
    local client_id = params[index] or protected_.cur_client_id_ or 1;
    index = index + 1;
    local trump_sid = params[index];
    index = index+1;
    local cmsg = {EC2SProtocol["EC2SProtocol_ChooseTrump"], trump_sid};
    sendMessage(client_id, cmsg);
	return 0;
end

console_cmds_["confirmchooseHero"] = function (params)
	local indx = 1;
	local client_id = params[indx] or protected_.cur_client_id_ or 1;
	indx = indx+1;
	local hero_sid = params[indx];
	indx = indx+1;
	local cmsg = {EC2SProtocol["EC2SProtocol_ConfirmChooseHero"], hero_sid};
	sendMessage(client_id, cmsg);
	return 0;
end

console_cmds_["CancelChooseHero"] = function (params)
	local indx = 1;
	local client_id = params[indx] or protected_.cur_client_id_ or 1;
	indx = indx+1;
	local hero_sid = params[indx];
	indx = indx+1;
	local cmsg = {EC2SProtocol["EC2SProtocol_CancelChooseHero"], hero_sid};
	sendMessage(client_id, cmsg);
	return 0;
end

console_cmds_["choosePet"] = function (params)
	local indx = 1;
    local client_id = params[indx] or protected_.cur_client_id_ or 1;
	indx = indx+1;
	local pet_indx = params[indx];
	indx = indx+1;
	local cmsg = {EC2SProtocol["EC2SProtocol_ChoosePet"], pet_indx};
	sendMessage(client_id, cmsg);
	return 0;
end

console_cmds_["setFormation"] = function (params)
	local indx = 1;
    local client_id = params[indx] or protected_.cur_client_id_ or 1;
	indx = indx+1;
	local formation_sid = params[indx];
	indx = indx+1;
	local cmsg = {EC2SProtocol["EC2SProtocol_SetFormation"], formation_sid};
	sendMessage(client_id, cmsg);
	return 0;
end

console_cmds_["setMemPos"] = function (params)
	local indx = 1;
    local client_id = params[indx] or protected_.cur_client_id_ or 1;
	indx = indx+1;
	local mem_troop_indx = params[indx];
	indx = indx+1;
	local slot_id = params[indx];
	indx = indx+1;
	local client = protected_.clients_[client_id];
	local mem_suid = client.cur_troop_.list[mem_troop_indx];
	local cmsg = {EC2SProtocol["EC2SProtocol_SetMemPos"], mem_suid, slot_id};
	sendMessage(client_id, cmsg);
	return 0;
end

console_cmds_["synClientLoad"] = function (params)
	local indx = 1;
    local client_id = params[indx] or protected_.cur_client_id_ or 1;
	indx = indx+1;
	local load_rate = params[indx];
	indx = indx+1;
	local cmsg = {EC2SProtocol["EC2SProtocol_FightClientLoad"], load_rate};
	sendMessage(client_id, cmsg);
	return 0;
end

console_cmds_["beginFight"] = function (params)
	local indx = 1;
    local client_id = params[indx] or protected_.cur_client_id_ or 1;
	indx = indx+1;
	local cmsg = {EC2SProtocol["EC2SProtocol_BeginField"]};
	sendMessage(client_id, cmsg);
	return 0;
end

console_cmds_["clientBegin"] = function (params)
	local indx = 1;
    local client_id = params[indx] or protected_.cur_client_id_ or 1;
	indx = indx+1;
	local client = protected_.clients_[client_id];
	local fight_worker = client.fight_.fight_worker_;
	print("clientBegin.....");
	if fight_worker==nil then
		print("clientBegin onFightEnd");
		client_recall_("onFightEnd", client_id);
	else
		fight_worker.client_begin_ = true;
		if fight_worker.fight_run_ then
			print("clientBegin onFightBegin");
			client_recall_("onFightBegin", client_id);
		else
			print("clientBegin send enter field");
			sendMessage(client_id, {EC2SProtocol["EC2SProtocol_EnterField"]});
		end
	end
	return 0;
end

console_cmds_["setHeroAttack"] = function (params)
	local indx = 1;
    local client_id = params[indx] or protected_.cur_client_id_ or 1;
	indx = indx+1;
	local client = protected_.clients_[client_id];
	local fight_worker = client.fight_.fight_worker_;
	local unit_id = fight_worker.my_unit_id_;
	local skill_indx = params[indx] or 0;
	indx = indx +1;
	-- print("setHeroAttack skill_index", skill_indx);
--	local target_id = params[indx] or 0;
--	indx = indx +1;
	local corp_id = params[indx] or 0;
	indx = indx +1;
	local slot_id = params[indx] or 0;
	indx = indx +1;
	local params = params[indx];
	indx = indx +1;
	local unit = fight_worker[unit_id];
	if 1600102==unit.skill_lists_[skill_indx] then
		if params==nil or #params==0 then
			local local_mem = client.cur_troop_[client.suid_];
			local pet_index_0 = fight_worker.my_pet_indx_ or 0;
			local pet_index = (pet_index_0 % 3)+1;
			while pet_index~=pet_index_0 do
				local pet = local_mem[protected_.member_pet_ids[pet_index]];
				if pet and not pet.is_dead then
					params={pet_index}
					break;
				end
				if pet_index_0==0 then
					pet_index_0 = pet_index;
				end
				pet_index = (pet_index % 3)+1;
			end
		end
	end

	local skill_sid;
	if skill_indx>Enum.ESkill_MAX then
		skill_sid = configs_.base_skills_[skill_indx-Enum.ESkill_MAX];
	else
		skill_sid = unit.skill_lists_[skill_indx];
	end
	if skill_sid==nil then
		return 0;
	end
	local skill_data = configs_.SkillCfg[skill_sid];
	if skill_data==nil then
		return 0;
	end
	local aggression_value = skill_data.aggression_value or 0;
	if aggression_value>0 then
		if corp_id==client.fight_.my_corp_id_ then
			corp_id = 0;
			slot_id = 0;
		end
	elseif aggression_value<0 then
		if corp_id~=client.fight_.my_corp_id_ then
			corp_id = 0;
			slot_id = 0;
		end
	end
	local target_id = 0;
	local target = fight_worker.corps_[corp_id][slot_id];
	if target then
		target_id = target.unit_id_;
	end

	local cmsg = {EC2SProtocol["EC2SProtocol_Attack"], unit_id, skill_indx, target_id, corp_id, slot_id, params};
	sendMessage(client_id, cmsg);
	return 0;
end
console_cmds_["setPetAttack"] = function (params)
	local indx = 1;
    local client_id = params[indx] or protected_.cur_client_id_ or 1;
	indx = indx+1;
	local client = protected_.clients_[client_id];
	local fight_worker = client.fight_.fight_worker_;
	local unit_id = fight_worker.my_pet_id_;
	local skill_indx = params[indx] or 0;
	indx = indx +1;
--	local target_id = params[indx] or 0;
--	indx = indx +1;
	local corp_id = params[indx] or 0;
	indx = indx +1;
	local slot_id = params[indx] or 0;
	indx = indx +1;
	local params = params[indx];
	indx = indx +1;

	local skill_sid;
	if skill_indx>Enum.ESkill_MAX then
		skill_sid = configs_.base_skills_[skill_indx-Enum.ESkill_MAX];
	else
		local unit = fight_worker[unit_id];
		if unit then
			skill_sid = unit.skill_lists_[skill_indx];
		end
	end
	if skill_sid==nil then
		return 0;
	end
	local skill_data = configs_.SkillCfg[skill_sid];
	if skill_data==nil then
		return 0;
	end
	local aggression_value = skill_data.aggression_value or 0;
	if aggression_value>0 then
		if corp_id==client.fight_.my_corp_id_ then
			corp_id = 0;
			slot_id = 0;
		end
	elseif aggression_value<0 then
		if corp_id~=client.fight_.my_corp_id_ then
			corp_id = 0;
			slot_id = 0;
		end
	end
	local target_id = 0;
	local target = fight_worker.corps_[corp_id][slot_id];
	if target then
		target_id = target.unit_id_;
	end

	local cmsg = {EC2SProtocol["EC2SProtocol_Attack"], unit_id, skill_indx, target_id, corp_id, slot_id, params};
	sendMessage(client_id, cmsg);
	return 0;
end
console_cmds_["setHeroSkill"] = function (params)
	local indx = 1;
    local client_id = params[indx] or protected_.cur_client_id_ or 1;
	indx = indx+1;
	local client = protected_.clients_[client_id];
	local fight_worker = client.fight_.fight_worker_;
	local unit_id = fight_worker.my_unit_id_;
	local unit = fight_worker[unit_id];
	if unit==nil then
		return -1;
	end
	local skill_sid = params[indx] or 0;
	indx = indx +1;
	local skill_indx = unit.skills_[skill_sid];
	if skill_indx==nil or skill_indx.slot==nil then
		return -1;
	end
	skill_indx = skill_indx.slot;
	print("setHeroSkill ", skill_sid, skill_indx);
--	local target_id = params[indx] or 0;
--	indx = indx +1;
	local corp_id = params[indx] or 0;
	indx = indx +1;
	local slot_id = params[indx] or 0;
	indx = indx +1;
	local params = params[indx];
	indx = indx +1;
	if 1600102==skill_sid then
		if params==nil or #params==0 then
			local local_mem = client.cur_troop_[client.suid_];
			local pet_index_0 = fight_worker.my_pet_indx_ or 0;
			local pet_index = (pet_index_0 % 3)+1;
			while pet_index~=pet_index_0 do
				local pet = local_mem[protected_.member_pet_ids[pet_index]];
				if pet and not pet.is_dead then
					params={pet_index}
					break;
				end
				if pet_index_0==0 then
					pet_index_0 = pet_index;
				end
				pet_index = (pet_index % 3)+1;
			end
		else
			print("paramsparamsparamsparams")
			print(indx)
		end
	end

	local skill_data = configs_.SkillCfg[skill_sid];
	if skill_data==nil then
		return 0;
	end
	local aggression_value = skill_data.aggression_value or 0;
	if aggression_value>0 then
		if corp_id==client.fight_.my_corp_id_ then
			corp_id = 0;
			slot_id = 0;
		end
	elseif aggression_value<0 then
		if corp_id~=client.fight_.my_corp_id_ then
			corp_id = 0;
			slot_id = 0;
		end
	end
	local target_id = 0;
	local target = fight_worker.corps_[corp_id][slot_id];
	if target then
		target_id = target.unit_id_;
	end
	print("petindex",pet_index)
	print("target_id",target_id)

	local cmsg = {EC2SProtocol["EC2SProtocol_Attack"], unit_id, skill_indx, target_id, corp_id, slot_id, params};
	sendMessage(client_id, cmsg);
	return 0;
end
console_cmds_["setPetSkill"] = function (params)
	local indx = 1;
    local client_id = params[indx] or protected_.cur_client_id_ or 1;
	indx = indx+1;
	local client = protected_.clients_[client_id];
	local fight_worker = client.fight_.fight_worker_;
	local unit_id = fight_worker.my_pet_id_;
	local unit = fight_worker[unit_id];
	if unit==nil then
		return -1;
	end
	local skill_sid = params[indx] or 0;
	indx = indx +1;
	local skill_indx = unit.skills_[skill_sid];
	if skill_indx==nil or skill_indx.slot==nil then
		return -1;
	end
	skill_indx = skill_indx.slot;
	print("setPetSkill ", skill_sid, skill_indx);
--	local target_id = params[indx] or 0;
--	indx = indx +1;
	local corp_id = params[indx] or 0;
	indx = indx +1;
	local slot_id = params[indx] or 0;
	indx = indx +1;
	local params = params[indx];
	indx = indx +1;

	local skill_data = configs_.SkillCfg[skill_sid];
	if skill_data==nil then
		return 0;
	end
	local aggression_value = skill_data.aggression_value or 0;
	if aggression_value>0 then
		if corp_id==client.fight_.my_corp_id_ then
			corp_id = 0;
			slot_id = 0;
		end
	elseif aggression_value<0 then
		if corp_id~=client.fight_.my_corp_id_ then
			corp_id = 0;
			slot_id = 0;
		end
	end
	local target_id = 0;
	local target = fight_worker.corps_[corp_id][slot_id];
	if target then
		target_id = target.unit_id_;
	end

	local cmsg = {EC2SProtocol["EC2SProtocol_Attack"], unit_id, skill_indx, target_id, corp_id, slot_id, params};
	sendMessage(client_id, cmsg);
	return 0;
end

console_cmds_["QTECommit"] = function (params)
	local index = 1;
	local client_id = params[index] or protected_.cur_client_id_ or 1;
	index = index+1;
	local client = protected_.clients_[client_id];
	
	local fight_worker = client.fight_.fight_worker_;
	local unit_id = params[index] or client.my_unit_id_;
	index = index + 1;
	local commit_val = params[index]
	index = index + 1;
	
	print("QTECommit: ", unit_id, commit_val);

	local cmsg = {EC2SProtocol["EC2SProtocol_QTECommit"], unit_id, commit_val};
	sendMessage(client_id, cmsg);
	return 0;
end

console_cmds_["thumbsup"] = function (params)
	local index = 1;
	local client_id = params[index] or protected_.cur_client_id_ or 1;
	index = index+1;
	--local fight_server_id = params[index] or 1;
	--index = index+1;
	local targetsuid = params[index];
	index = index+1;
	local client = protected_.clients_[client_id];
	local MysUid = client.suid_;
	local fight_worker = client.fight_.fight_worker_;
	--不能给自己点赞
	if targetsuid==MysUid then
		print("thumbsup", "targetsuid==MysUid")
		return 1;
	end
	--已经点过赞
	if fight_worker[targetsuid]~=nil then
		print("thumbsup", "fight_worker[targetsuid]~=nil")
		return 1;
	end
	local result_data = fight_worker.fight_result_data_ or {};
	local fight_id = fight_worker.fight_id;
	local fight_server_id = fight_worker.fight_server_id_; --本地fight_serverID
	local msg = {};
	local src_hero_id = 0;
	for i = 1, #result_data do
		--print("i="..i.."result_data[i].player_sid = "..result_data[i].player_sid);
		msg[i] = result_data[i].player_sid;
		if MysUid==result_data[i].player_sid then
			src_hero_id = result_data[i].sunit_id
		end
	end
	local cmsg = {EC2SProtocol["EC2SProtocol_ThumbsUp"], fight_server_id,fight_id,targetsuid,MysUid,src_hero_id,msg};
	sendMessage(client_id, cmsg);
	return 0;
end
--离开结算界面时，一定要调用
console_cmds_["closeFightResult"] = function (params)
	local indx = 1;
	local client_id = params[indx] or protected_.cur_client_id_ or 1;
	indx = indx+1;
	local client = protected_.clients_[client_id];
	local fight_worker = client.fight_.fight_worker_;
	fight_worker.state_=protected_.MetaFight.EFightState_None;
	client.fight_.fight_worker_ = nil;
	client.fight_ = nil;
	client.troops_ = {};
	client.cur_troop_ = nil;
	protected_.unregistFightUpdater(client);
	return 0;
end

console_cmds_["buyHero"] = function (params)
	local indx = 1;
	local client_id = params[indx] or protected_.cur_client_id_ or 1;
	indx = indx+1;
	local hero_sid = params[indx];
	indx = indx + 1;
	local cost_index = params[indx];
	indx = indx + 1;

	local game_player = protected_.clients_[client_id].player_;
	local hero_bag = game_player.hero_bag_;
	--已有永久的此英雄 就不要买了
	if hero_bag[hero_sid] and hero_bag[hero_sid].hero_type_ == Enum.EHeroUseType_Permanent then
		print("hero is exist  EHeroUseType_Permanent!")
		return 0;
	end
	local client = protected_.clients_[client_id];
	local cmsg = {EC2SProtocol["EC2SProtocol_BuyHero"], hero_sid, cost_index};
	sendMessage(client_id, cmsg);
	return 0;
end

console_cmds_["thumbsupByNmae"] = function (params)
	local index = 1;
	local client_id = params[index] or protected_.cur_client_id_ or 1;
	index = index+1;
	local client = protected_.clients_[client_id];
	local fight_worker = client.fight_.fight_worker_;

	local player_name = params[index];
	local usid ;
	if fight_worker then
		for k, v in pairs(client.troops_) do
			for kk, vv in pairs(v.map_) do
				if player_name == vv.player_name_ then
					usid = vv.player_suid_;
				end
			end			
		end
	end
	if not usid then
		return 1;
	end
	doLuaCommand("thumbsup", client_id, usid);
	return 0;
end

--抽取神兽
console_cmds_["summonPet"] = function (params)
	local index = 1;
	local client_id = params[index] or protected_.cur_client_id_ or 1;
	index = index+1;
	local client = protected_.clients_[client_id];
	
	local pack_id = params[index] or 1;
	index = index + 1;
	if type(pack_id) ~= "number" then
		return 1;
	end

	local pack_data = configs_.SummonData.pet_pack[pack_id];
	if not pack_data then
		return 1;
	end

	local cmsg = {EC2SProtocol["EC2SProtocol_SummonPet"], pack_id};
	sendMessage(client_id, cmsg);
	return 0;
end
console_cmds_["petTroopAddPet"] = function (params)
	local index = 1;
	local client_id = params[index] or protected_.cur_client_id_ or 1;
	index = index+1;
	local client = protected_.clients_[client_id];
	
	local troop_id = params[index];
	index = index + 1;
	local pet_id = params[index];
	index = index + 1;
	local slot_id = params[index];
	index = index + 1;
	if type(troop_id) ~= "number" or type(pet_id) ~= "number" or type(slot_id) ~= "number"  then
		return 1;
	end

	local cmsg = {EC2SProtocol["EC2SProtocol_PetTroopAddPet"], troop_id, pet_id, slot_id};
	sendMessage(client_id, cmsg);
	return 0;
end
console_cmds_["petTroopRemovePet"] = function (params)
	local index = 1;
	local client_id = params[index] or protected_.cur_client_id_ or 1;
	index = index+1;
	local client = protected_.clients_[client_id];
	
	local troop_id = params[index];
	index = index + 1;
	local slot_id = params[index];
	index = index + 1;
	if type(troop_id) ~= "number"  or type(slot_id) ~= "number"  then
		return 1;
	end

	local cmsg = {EC2SProtocol["EC2SProtocol_PetTroopRemovePet"], troop_id, slot_id};
	sendMessage(client_id, cmsg);
	return 0;
end
console_cmds_["sellPet"] = function (params)
	local index = 1;
	local client_id = params[index] or protected_.cur_client_id_ or 1;
	index = index+1;
	local client = protected_.clients_[client_id];
	
	local pet_id = params[index];
	index = index + 1;

	if type(pet_id) ~= "number" then
		return 1;
	end

	local cmsg = {EC2SProtocol["EC2SProtocol_SellPet"], pet_id};
	sendMessage(client_id, cmsg);
	return 0;
end
console_cmds_["setCommander"] = function (params)
	local index = 1;
	local client_id = params[index] or protected_.cur_client_id_ or 1;
	index = index+1;
	local client = protected_.clients_[client_id];
	
	local player_id = params[index];
	
	local cmsg = {EC2SProtocol["EC2SProtocol_SetCommander"], player_id};
	sendMessage(client_id, cmsg);
	return 0;
end
console_cmds_["removeCommander"] = function (params)
	local index = 1;
	local client_id = params[index] or protected_.cur_client_id_ or 1;
	index = index+1;
	local client = protected_.clients_[client_id];
		
	local cmsg = {EC2SProtocol["EC2SProtocol_RemoveCommander"]};
	sendMessage(client_id, cmsg);
	return 0;
end
console_cmds_["markUnit"] = function (params)
	local index = 1;
	local client_id = params[index] or protected_.cur_client_id_ or 1;
	index = index+1;
	local client = protected_.clients_[client_id];
	
	local unit_id = params[index];
	index = index + 1;
	local mark_content = params[index]
	index = index + 1;
	
	local cmsg = {EC2SProtocol["EC2SProtocol_MarkUnit"], unit_id, mark_content};
	sendMessage(client_id, cmsg);
	return 0;
end
console_cmds_["removeMarkUnit"] = function (params)
	local index = 1;
	local client_id = params[index] or protected_.cur_client_id_ or 1;
	index = index+1;
	local client = protected_.clients_[client_id];
	
	local unit_id = params[index];
	index = index + 1;
	
	local cmsg = {EC2SProtocol["EC2SProtocol_RemoveMarkUnit"], unit_id};
	sendMessage(client_id, cmsg);
	return 0;
end
console_cmds_["addFightMark"] = function (params)
	local index = 1;
	local client_id = params[index] or protected_.cur_client_id_ or 1;
	index = index+1;
	local client = protected_.clients_[client_id];
	
	local mark_type = params[index];
	index = index + 1;
	local content = params[index];
	index = index + 1;
	
	if mark_type ~= 1 or mark_type ~= 2 then
		return 1;
	end
	if type(content) ~= "string" or string.len(content) == 0 then
		return 1;
	end
	local fight_marks = client.player_.friend_marks_;
	local default_marks = configs_.FightMark.friend;
	if mark_type == 2 then --1 友方，2 敌方
		fight_marks = client.player_.enemy_marks_;
		default_marks = configs_.FightMark.enemy;
	end
	local id ;
	for i = #default_marks, configs_.FightMark.mark_max do
		if not fight_marks[i] then
			id = i;
			break;
		end
	end
	if not id then
		print("no empty id");
		return 1;
	end

	local cmsg = {EC2SProtocol["EC2SProtocol_AddFightMark"], mark_type, id, content};
	sendMessage(client_id, cmsg);
	return 0;
end
console_cmds_["delFightMark"] = function (params)
	local index = 1;
	local client_id = params[index] or protected_.cur_client_id_ or 1;
	index = index+1;
	local client = protected_.clients_[client_id];
	
	local mark_type = params[index];
	index = index + 1;
	local id = params[index];
	index = index + 1;
	
	if mark_type ~= 1 or mark_type ~= 2 then
		return 1;
	end
	local fight_marks = client.player_.friend_marks_;
	local default_marks = configs_.FightMark.friend;
	if mark_type == 2 then --1 友方，2 敌方
		fight_marks = client.player_.enemy_marks_;
		default_marks = configs_.FightMark.enemy;
	end
	if id <= #default_marks or id > configs_.FightMark.mark_max then
		print("id error", id);
		return 1;
	end

	local cmsg = {EC2SProtocol["EC2SProtocol_DelFightMark"], mark_type, id};
	sendMessage(client_id, cmsg);
	return 0;
end
console_cmds_["editFightMark"] = function (params)
	local index = 1;
	local client_id = params[index] or protected_.cur_client_id_ or 1;
	index = index+1;
	local client = protected_.clients_[client_id];
	
	local mark_type = params[index];
	index = index + 1;
	local id = params[index];
	index = index + 1;
	local content = params[index];
	index = index + 1;
	
	if mark_type ~= 1 or mark_type ~= 2 then
		return 1;
	end
	if type(content) ~= "string" or string.len(content) == 0 then
		return 1;
	end
	local fight_marks = client.player_.friend_marks_;
	local default_marks = configs_.FightMark.friend;
	if mark_type == 2 then --1 友方，2 敌方
		fight_marks = client.player_.enemy_marks_;
		default_marks = configs_.FightMark.enemy;
	end
	if id > configs_.FightMark.mark_max then
		print("id error", id);
		return 1;
	end

	local cmsg = {EC2SProtocol["EC2SProtocol_EditFightMark"], mark_type, id, content};
	sendMessage(client_id, cmsg);
	return 0;
end
console_cmds_["clearFightMark"] = function (params)
	local index = 1;
	local client_id = params[index] or protected_.cur_client_id_ or 1;
	index = index+1;
	local client = protected_.clients_[client_id];
	
	local mark_type = params[index];
	index = index + 1;
	
	if mark_type ~= 1 or mark_type ~= 2 then
		return 1;
	end

	local cmsg = {EC2SProtocol["EC2SProtocol_ClearFightMark"], mark_type};
	sendMessage(client_id, cmsg);
	return 0;
end
console_cmds_["selectPetTroop"] = function (params)
	local index = 1;
	local client_id = params[index] or protected_.cur_client_id_ or 1;
	index = index+1;
	local client = protected_.clients_[client_id];
	
	local pet_troop_id = params[index];
	index = index + 1;
	
	if pet_troop_id > configs_.pet_troop_max_num then
		return 1;
	end
	if not client.cur_troop_ then
		return 1;
	end
	local member = client.cur_troop_[client.suid_];
	if not member or pet_troop_id == member.pet_troop_id_ then
		return 0;
	end

	local cmsg = {EC2SProtocol["EC2SProtocol_SelectPetTroop"], pet_troop_id};
	sendMessage(client_id, cmsg);
	return 0;
end
console_cmds_["useItem"] = function (params)
	local index = 1;
	local client_id = params[index] or protected_.cur_client_id_ or 1;
	index = index+1;
	local client = protected_.clients_[client_id];
	
	local item_id = params[index];
	index = index + 1;
	local count = params[index] or 1;
	index = index + 1;
	local param = params[index];
	index = index + 1;

	local item_bag = client.player_.item_bag_;
	local item = item_bag[item_id];
	local item_data = protected_.MetaItemBag.getItemData(item.item_sid_);
	if not item_data then
		client_recall_("onHint", client_id, {nil, 1, "没有道具" .. item.item_sid_ .. "配置"});
		return 1;
	end
	if not item_bag:checkCount(item_id, count) then
		client_recall_("onHint", client_id, {nil, 1, "道具不足"});
		return 1;
	end
	local cmsg = {EC2SProtocol["EC2SProtocol_UseItem"], item_id, count, param};
	sendMessage(client_id, cmsg);
	return 0;
end
console_cmds_["sellItem"] = function (params)
	local index = 1;
	local client_id = params[index] or protected_.cur_client_id_ or 1;
	index = index+1;
	local client = protected_.clients_[client_id];
	
	local param = params[index];
	index = index + 1;

	local item_bag = client.player_.item_bag_;
	for item_id, count in pairs(param) do
		local item = item_bag[item_id];
		local item_data = protected_.MetaItemBag.getItemData(item.item_sid_);
		if not item_data then
			client_recall_("onHint", client_id, {nil, 1, "没有道具" .. item.item_sid_ .. "配置"});
			return 1;
		end
		if not item_bag:checkCount(item_id, count) then
			client_recall_("onHint", client_id, {nil, 1, "道具不足"});
			return 1;
		end
	end

	local cmsg = {EC2SProtocol["EC2SProtocol_SellItem"], param};
	sendMessage(client_id, cmsg);
	return 0;
end
console_cmds_["sellItemBySid"] = function (params)
	local index = 1;
	local client_id = params[index] or protected_.cur_client_id_ or 1;
	index = index+1;
	local client = protected_.clients_[client_id];
	
	local param = params[index]; --{[item_sid] = count, ...}
	index = index + 1;
	
	if not next(param) then
		return 1;
	end

	for item_sid, count in pairs(param) do
		local item_data = protected_.MetaItemBag.getItemData(item_sid);
		if not item_data then
			client_recall_("onHint", client_id, {nil, 1, "没有道具" .. item_sid .. "配置"});
			return 1;
		end
		local item_bag = client.player_.item_bag_;
		local total_count = item_bag:getCountBySid(item_sid);
		if total_count < count then
			client_recall_("onHint", client_id, {nil, 1, "道具不足"});
			return 1;
		end
	end
	
	local cmsg = {EC2SProtocol["EC2SProtocol_SellItemBySid"], param};
	sendMessage(client_id, cmsg);
	return 0;
end
console_cmds_["buyItem"] = function (params)
	local index = 1;
	local client_id = params[index] or protected_.cur_client_id_ or 1;
	index = index+1;
	local client = protected_.clients_[client_id];
	
	local item_sid = params[index];
	index = index + 1;
	local count = params[index] or 1;
	index = index + 1;
	
	if count <= 0 then
		return 1;
	end

	local item_bag = client.player_.item_bag_;
	local item_data = protected_.MetaItemBag.getItemData(item_sid);
	if not item_data then
		client_recall_("onHint", client_id, {nil, 1, "没有道具" .. item_sid .. "配置"});
		return 1;
	end
	if item_bag:getCountBySid(item_sid) + count > item_data.own_max then
        client_recall_("onHint", client_id, {nil, 1, "购买数量超过最大"});
		return 1;
    end

	local cmsg = {EC2SProtocol["EC2SProtocol_BuyItem"], item_sid, count};
	sendMessage(client_id, cmsg);
	return 0;
end
console_cmds_["chatInChannel"] = function (params)
	local index = 1;
	local client_id = params[index] or protected_.cur_client_id_ or 1;
	index = index+1;
	local chat_str = params[index];
	index = index + 1;
	
	print("chatInChannel: ", chat_str);

	local cmsg = {EC2SProtocol["EC2SProtocol_ChatInChannel"], chat_str};
	sendMessage(client_id, cmsg);
	return 0;
end

console_cmds_["chatInTroop"] = function (params)
	local index = 1;
	local client_id = params[index] or protected_.cur_client_id_ or 1;
	index = index+1;
	local chat_str = params[index];
	index = index + 1;
	
	print("chatInTroop: ", chat_str);

	local cmsg = {EC2SProtocol["EC2SProtocol_ChatInTroop"], chat_str};
	sendMessage(client_id, cmsg);
	return 0;
end

console_cmds_["chatToRoom"] = function (params)
	local index = 1;
	local client_id = params[index] or protected_.cur_client_id_ or 1;
	index = index+1;
	local chat_str = params[index];
	index = index + 1;
	
	print("chatToRoom: ", chat_str);

	local cmsg = {EC2SProtocol["EC2SProtocol_ChatToRoom"], chat_str};
	sendMessage(client_id, cmsg);
	return 0;
end

console_cmds_["chatToPlayer"] = function (params)
	local index = 1;
	local client_id = params[index] or protected_.cur_client_id_ or 1;
	index = index+1;
	local p_suid = params[index];
	index = index + 1;
	local chat_str = params[index];
	index = index + 1;
	
	print("chatToPlayer: ", p_suid, chat_str);

	local cmsg = {EC2SProtocol["EC2SProtocol_ChatToPlayer"], p_suid, chat_str};
	sendMessage(client_id, cmsg);
	return 0;
end

console_cmds_["joinChannel"] = function (params)
	local index = 1;
	local client_id = params[index] or protected_.cur_client_id_ or 1;
	index = index+1;
	local channel_id = params[index];
	index = index + 1;
	
	print("joinChannel: ", channel_id);

	local cmsg = {EC2SProtocol["EC2SProtocol_JoinChannel"], channel_id};
	sendMessage(client_id, cmsg);
	return 0;
end
console_cmds_["resolvePet"] = function (params)
	local index = 1;
	local client_id = params[index] or protected_.cur_client_id_ or 1;
	index = index+1;
	local pet_ids = params[index];
	index = index + 1;
	
	local cmsg = {EC2SProtocol["EC2SProtocol_ResolvePet"], pet_ids};
	sendMessage(client_id, cmsg);
	return 0;
end
console_cmds_["refreshProp"] = function (params)
	local index = 1;
	local client_id = params[index] or protected_.cur_client_id_ or 1;
	index = index+1;
	local pet_id = params[index];
	index = index + 1;
	
	local cmsg = {EC2SProtocol["EC2SProtocol_RefreshProp"], pet_id};
	sendMessage(client_id, cmsg);
	return 0;
end
console_cmds_["replaceProp"] = function (params)
	local index = 1;
	local client_id = params[index] or protected_.cur_client_id_ or 1;
	index = index+1;
	local pet_id = params[index];
	index = index + 1;
	
	local cmsg = {EC2SProtocol["EC2SProtocol_ReplaceProp"], pet_id};
	sendMessage(client_id, cmsg);
	return 0;
end
console_cmds_["learnSkill"] = function (params)
	local index = 1;
	local client_id = params[index] or protected_.cur_client_id_ or 1;
	index = index+1;
	local pet_id = params[index];
	index = index + 1;
	local learn_params = params[index];
	index = index + 1;
	
	local client = protected_.clients_[client_id];
	local pet_bag = client.player_.pet_bag_;
	if not pet_bag:checkLearnSkillParams(pet_id, learn_params) then
		client_recall_("onHint", client_id, {nil, 1, "参数错误"});
		return 1;
	end

	local cmsg = {EC2SProtocol["EC2SProtocol_LearnSkill"], pet_id, learn_params};
	sendMessage(client_id, cmsg);
	return 0;
end

console_cmds_["createAlliance"] = function(params)
    local index = 1;
    local client_id = params[index] or protected_.cur_client_id_ or 1
    print("start createAliance", client_id)
    index = index + 1
    local alliance_name = params[index]

    local client = protected_.clients_[client_id]
    local cmsg = {EC2SProtocol["EC2SProtocol_CreateAlliance"], alliance_name};
    sendMessage(client_id, cmsg)
end

console_cmds_["joinAlliance"] = function(params)
    local index = 1;
    local client_id = params[index] or protected_.cur_client_id_ or 1
    
    index = index + 1
    local  allliance_id = params[index]
    print("start joinAlliance", allliance_id)
    local client = protected_.clients_[client_id]
    local cmsg = {EC2SProtocol["EC2SProtocol_JoinAlliance"], allliance_id}
    sendMessage(client_id, cmsg)
end

console_cmds_["leaveAlliance"] = function(params)
    local index = 1;
    local client_id = params[index] or protected_.cur_client_id_ or 1
    
    print("start leaveAlliance")
    local client = protected_.clients_[client_id]
    local cmsg = {EC2SProtocol["EC2SProtocol_LeaveAlliance"]}
    sendMessage(client_id, cmsg)
end

console_cmds_["getAllianceMembers"] = function(params)
    local index = 1;
    local client_id = params[index] or protected_.cur_client_id_ or 1
    print("get allianceMembers")
    index = index + 1
    local allliance_id = params[index]
    local client  = protected_.clients_[client_id]
    local cmsg = {EC2SProtocol["EC2SProtocol_GetAllianceMembers"], allliance_id}
    sendMessage(client_id, cmsg)
end
console_cmds_["lottery"] = function (params)
	local index = 1;
	local client_id = params[index] or protected_.cur_client_id_ or 1;
	index = index+1;
	local client = protected_.clients_[client_id];
	
	local pack_id = params[index];
	index = index + 1;
	local times = params[index] or 1;
	index = index + 1;
	local param = params[index];	--额外参数， 1.选择抽取消耗索引
	index = index + 1;

	local lottery_sys = client.player_.lottery_sys_;
	if not lottery_sys:canLottery(pack_id, times) then
		return 1;
	end

	local cmsg = {EC2SProtocol["EC2SProtocol_Lottery"], pack_id, times, param};
	sendMessage(client_id, cmsg);
	return 0;
end
console_cmds_["installPosy"] = function (params)
	local index = 1;
	local client_id = params[index] or protected_.cur_client_id_ or 1;
	index = index+1;
	local client = protected_.clients_[client_id];
	
	local program_id = params[index];
	index = index + 1;
	local slot_id = params[index] or 1;
	index = index + 1;
	local item_sid = params[index];	
	index = index + 1;

	local posy_sys = client.player_.posy_sys_;
	if not posy_sys:canInstall(program_id, slot_id, item_sid) then
		client_recall_("onHint", client_id, {nil, 1, "不可装配"});
		return 1;
	end

	local cmsg = {EC2SProtocol["EC2SProtocol_InstallPosy"], program_id, slot_id, item_sid};
	sendMessage(client_id, cmsg);
	return 0;
end
console_cmds_["uninstallPosy"] = function (params)
	local index = 1;
	local client_id = params[index] or protected_.cur_client_id_ or 1;
	index = index+1;
	local client = protected_.clients_[client_id];
	
	local program_id = params[index];
	index = index + 1;
	local slot_id = params[index] or 1;
	index = index + 1;

	local posy_sys = client.player_.posy_sys_;
	local program = posy_sys[program_id];
	if not program or not program.assembly_[slot_id] then
		return 1;
	end
	
	local cmsg = {EC2SProtocol["EC2SProtocol_UninstallPosy"], program_id, slot_id};
	sendMessage(client_id, cmsg);
	return 0;
end
console_cmds_["uninstallAllPosy"] = function (params)
	local index = 1;
	local client_id = params[index] or protected_.cur_client_id_ or 1;
	index = index+1;
	local client = protected_.clients_[client_id];
	
	local program_id = params[index];
	index = index + 1;

	local posy_sys = client.player_.posy_sys_;
	local program = posy_sys[program_id];
	if not program  then
		return 1;
	end
	
	local cmsg = {EC2SProtocol["EC2SProtocol_UninstallAllPosy"], program_id};
	sendMessage(client_id, cmsg);
	return 0;
end
console_cmds_["ProgramSetName"] = function (params)
	local index = 1;
	local client_id = params[index] or protected_.cur_client_id_ or 1;
	index = index+1;
	local client = protected_.clients_[client_id];
	
	local program_id = params[index];
	index = index + 1;
	local name = params[index];
	index = index + 1;

	local posy_sys = client.player_.posy_sys_;
	local program = posy_sys[program_id];
	if not program  then
		return 1;
	end
	if not name then
		return 1;
	end

	local cmsg = {EC2SProtocol["EC2SProtocol_ProgramSetName"], program_id, name};
	sendMessage(client_id, cmsg);
	return 0;
end
console_cmds_["unlockProgram"] = function (params)
	local index = 1;
	local client_id = params[index] or protected_.cur_client_id_ or 1;
	index = index+1;
	local client = protected_.clients_[client_id];
	
	local posy_sys = client.player_.posy_sys_;
	local programs_data = protected_.MetaPosySys.getProgramsData();
    if not programs_data then
        return ;
	end
	if (posy_sys.buy_num_ + 1) > #programs_data.buy_cost then
		return ;
	end

	local cmsg = {EC2SProtocol["EC2SProtocol_UnlockProgram"]};
	sendMessage(client_id, cmsg);
	return 0;
end
console_cmds_["selectPosyProgram"] = function (params)
	local index = 1;
	local client_id = params[index] or protected_.cur_client_id_ or 1;
	index = index+1;
	local client = protected_.clients_[client_id];
	
	local program_id = params[index];
	index = index + 1;
	
	local posy_sys = client.player_.posy_sys_;
	local program = posy_sys[program_id];
	if program and program.lock_ ~= 1 then
		print(" -----selectPosyProgram---lock_----- ")
		return 1;
	end

	local member = client.cur_troop_[client.suid_];
	if not member or program_id == member.posy_program_.id_ then
		print(" -----selectPosyProgram---error----- ")
		return 1;
	end

	local cmsg = {EC2SProtocol["EC2SProtocol_SelectPosyProgram"], program_id};
	sendMessage(client_id, cmsg);
	return 0;
end
console_cmds_["fetchSeasonReward"] = function (params)
	local index = 1;
	local client_id = params[index] or protected_.cur_client_id_ or 1;
	index = index+1;
	local client = protected_.clients_[client_id];
	local reward_index = params[index];
	index = index + 1;

	local player_season = client.player_.player_season_;
	if player_season:fetchState(reward_index) then
		return ;
	end

	local cmsg = {EC2SProtocol["EC2SProtocol_FetchSeasonReward"], reward_index};
	sendMessage(client_id, cmsg);
	return 0;
end

console_cmds_["testMsg"] = function (params)
	local index = 1;
	local client_id = params[index] or protected_.cur_client_id_ or 1;
	index = index+1;
	local client = protected_.clients_[client_id];
	
	local lottery_sys = client.player_.lottery_sys_;

	-- print("-----", lottery_sys:lotteryTimes(pack_id));
	-- for i = 100, 109 do
	-- 	console_cmds_["installPosy"]({client_id, 1, i, 1});
	-- end
	-- for i = 200, 209 do
	-- 	console_cmds_["installPosy"]({client_id, 1, i, 2});
	-- end
	-- for i = 300, 309 do
	-- 	console_cmds_["installPosy"]({client_id, 1, i, 3});
	-- end

	--printObject(client.player_.player_season_:getAllRewardState(1))

	return 0;
end
console_cmds_["test"] = function (params)
	local index = 1;
	local client_id = params[index] or protected_.cur_client_id_ or 1;
	index = index+1;
	local client = protected_.clients_[client_id];
	
	log.to_file(client.player_, "client_player.lua")

	return 0;
end

console_cmds_["requestRankInfo"] = function(params)
    local index = 1
    local client_id = params[index] or protected_.cur_client_id_ or 1
    local client = protected_.clients_[client_id]
    index = index + 1
    local rank_type = params[index]
    print("requestRankInfo rank type", rank_type)
    local cmsg = {EC2SProtocol["EC2SProtocol_RequestRankInfo"], rank_type}
    sendMessage(client_id, cmsg)
    return 0
end

console_cmds_["openDialog"] = function (params)
    local index = 1
    local client_id = params[1] or protected_.cur_client_id_ or 1;
    index = index + 1
    local client = protected_.clients_[client_id]
	local dialog = client.player_.dialog_;
	if dialog then
		return -1;
	end
	local dialog_sid = params[index];
	index = index + 1;
	local field_id;
	if client.fight_ and client.fight_.fight_worker_ then
		field_id = client.fight_.fight_sid_;
	end
	local unit_id = params[index];
	index = index + 1;
	local smsg = {EC2SProtocol["EC2SProtocol_OpenDialog"], dialog_sid, field_id, unit_id};
	sendMessage(client_id, smsg);
    return 0;
end

console_cmds_["confirmDialog"] = function (params)
    local client_id = params[1] or protected_.cur_client_id_ or 1;
    local client = protected_.clients_[client_id]
	local dialog = client.player_.dialog_;
	print("confirmDialog: ", dialog);
	if dialog==nil then
		return -1;
	end
	client_recall_("onDialogClose", client_id, dialog.dialog_sid_);
	dialog:close();
	local smsg = {EC2SProtocol["EC2SProtocol_ConfirmDialog"], dialog.dialog_sid_};
	sendMessage(client_id, smsg);
    return 0;
end

console_cmds_["chooseTroopReadyStatus"] = function(params)
    local index = 1
    local client_id = params[index] or protected_.cur_client_id_ or 1
    local client = protected_.clients_[client_id]
    index = index + 1
    local onhook_status = params[index]
    print("chooseTroopReadyStatus", onhook_status)
    local smsg = {EC2SProtocol["EC2SProtocol_ChooseTroopReadyStatus"], onhook_status}
    sendMessage(client_id, smsg)
end

console_cmds_["kickoffFromTroop"] = function(params)
    local index = 1
    local client_id = params[index] or protected_.cur_client_id_ or 1
    local client = protected_.clients_[client_id]
    index = index + 1
    local player_id = params[index]
    print("kickoffFromTroop player_id", player_id)
    local smsg = {EC2SProtocol["EC2SProtocol_KickOffFromTroop"], player_id}
    sendMessage(client_id, smsg)
end

console_cmds_["troopUseDialogue"] = function(params)
    local index = 1
    local client_id = params[index] or protected_.cur_client_id_ or 1
    local client = protected_.clients_[client_id]
    print("troopUseDialogue")
    sendMessage(client_id, {EC2SProtocol["EC2SProtocol_TroopUseDialogue"]})
end
console_cmds_["unlockPetTroop"] = function (params)
	local index = 1;
	local client_id = params[index] or protected_.cur_client_id_ or 1;
	index = index+1;
	local client = protected_.clients_[client_id];
	
	local pet_troop_bag = client.player_.pet_tbag_;
	local unlock_troop_id = #pet_troop_bag.map_ + 1
	if not pet_troop_bag:isCanPay(unlock_troop_id) then
		return 
	end

	local cmsg = {EC2SProtocol["EC2SProtocol_UnlockPetTroop"]};
	sendMessage(client_id, cmsg);
	return 0;
end
console_cmds_["petTroopSetName"] = function (params)
	local index = 1;
	local client_id = params[index] or protected_.cur_client_id_ or 1;
	index = index+1;
	local client = protected_.clients_[client_id];
	
	local pet_troop_id = params[index];
	index = index + 1;
	local name = params[index];
	index = index + 1;
	
	local pet_troop_bag = client.player_.pet_tbag_;
	if not pet_troop_bag:isCanUse(pet_troop_id) then
		return ;
	end

	local cmsg = {EC2SProtocol["EC2SProtocol_PetTroopSetName"], pet_troop_id, name};
	sendMessage(client_id, cmsg);
	return 0;
end

console_cmds_["readyMatch"] = function (params)
    local index = 1;
	local client_id = params[index] or protected_.cur_client_id_ or 1;
	index = index+1;
	local client = protected_.clients_[client_id];
	
--	if client.cur_troop_ == nil then
--        print("error state in state");
--        return 0;
--    end;
 print("%%%%%%%%%%%%%%%%%%", client.cur_troop_.id_);
	local cmsg = {EC2SProtocol["EC2SProtocol_ReadyToMatch"], client.cur_troop_.id_};
	sendMessage(client_id, cmsg);
	return 0;
end

console_cmds_["stopMatch"] = function (params)
	local indx = 1;
    local client_id = params[indx] or protected_.cur_client_id_ or 1;
	indx = indx+1;
	local cmsg = {EC2SProtocol["EC2SProtocol_StopMatch"]};
	sendMessage(client_id, cmsg);
	return 0;
end

console_cmds_["getQuestReward"] = function(params)
    local index = 1
    local client_id = params[index] or protected_.cur_client_id_ or 1
    index = index + 1
    local client = protected_.clients_[client_id]
    local group_id = params[index]
    if not configs_.QuestGroup[group_id] then
        print("getQuestReward : getreward group_id is error!")
        return
    end
    index = index + 1
    local quest_sid = params[index]
    if not quest_sid or not protected_.MetaQuestBag.getCfgData(group_id, quest_sid) then
        print("getQuestReward : getreward quest_sid is error!")
        return
    end
    local cmsg = {EC2SProtocol["EC2SProtocol_GetQuestReward"], group_id, quest_sid}
    sendMessage(client_id, cmsg)
	return 0
end

console_cmds_["getLevelReward"] = function(params)
    local index = 1
    local client_id = params[index] or protected_.cur_client_id_ or 1
    local client = protected_.clients_[client_id]
    sendMessage(client_id, {EC2SProtocol["EC2SProtocol_GetPlayerLevelReward"]})
    return 0    
end

console_cmds_["createTeamTroops"] = function (params)
	local indx = 1;
    local fight_sid = params[indx] or 1;
    for i,v in pairs(protected_.clients_) do
        console_cmds_["createTroop"]({i, fight_sid});
    end
	return 0;
end

console_cmds_["matchallTeamTroops"] = function(params)
    local index = 1;
    local match_kind = params[index] or "matchTroop";
    if match_kind ~= "matchTroop" and  match_kind ~= "rankmatchTroop" then
        return -1;
    end
    for i, v in pairs(protected_.team_troops_) do
        if i == v.leader_ then
            local client = protected_.playerClients_[i];
            print(i, client.client_id_);
            console_cmds_[match_kind]({client.client_id_});
        end
    end
end

console_cmds_["makeTroops"] = function(params)
    local index = 1;
    local fight_sid = params[index] or 1;
    index = index + 1;
    local troop_num = params[index] or 1;
    index = index + 1;
	sendMessage(protected_.cur_client_id_, {EC2SProtocol["EC2SProtocol_GM"], "makeTroops", fight_sid, troop_num});
end

console_cmds_["buySkin"] = function(params)
    local index = 1
    local client_id = params[index] or protected_.cur_client_id_ or 1
    index = index + 1
    local hero_id = params[index]
    index = index + 1
    local skin_index = params[index]
    local send = {EC2SProtocol["EC2SProtocol_BuyHeroSkin"], hero_id, skin_index}
    sendMessage(client_id, send)
end

console_cmds_["useSkin"] = function(params)
    local index = 1
    local client_id = params[index] or protected_.cur_client_id_ or 1
    index = index + 1
    local hero_id = params[index]
    index = index + 1
    local skin_index = params[index]
    local send = {EC2SProtocol["EC2SProtocol_UseHeroSkin"], hero_id, skin_index}
    sendMessage(client_id, send)
end

console_cmds_["cmd"] = function(params)
    local index = 1;
    local cmd = params[index] or nil;
    index = index + 1;
    local msg = params[index] or {};
    index = index + 1;
    console_cmds_[cmd](msg);
	return 0;
end

console_cmds_["makeRandomTroops"] = function(params)
    local index = 1;
    local fight_sid = params[index] or 1;
    index = index + 1;
    local troop_num = params[index] or 1;
    index = index + 1;
	sendMessage(protected_.cur_client_id_, {EC2SProtocol["EC2SProtocol_GM"], "makeRandomTroops", fight_sid, troop_num});
end

console_cmds_["beckonPets"] = function(params)
    local index = 1
    local client_id = params[index] or protected_.cur_client_id_ or 1
    index = index + 1
    local type_id = params[index] or 1
    local send = {EC2SProtocol["EC2SProtocol_BeckonPet"], type_id}
    sendMessage(client_id, send)
end

console_cmds_["setProps"] = function(params)
    local index = 1;
    local prop = params[index] or "";
    index = index + 1;
    local msg = {EC2SProtocol["EC2SProtocol_GM"], "setProp", prop};
    for i,v in pairs(protected_.clients_) do
        local value = params[index] or nil;
        index = index + 1;
        table.insert(msg, value);
        sendMessage(i, msg);
        table.remove(msg);
    end
end

console_cmds_["getTroopsInfo"] = function(params)
     for i, v in pairs(protected_.team_troops_) do
        if i == v.leader_ then
            local client = protected_.playerClients_[i];
            print("troop:"..v.id_.."troop_count:"..v.count_.."client_id:"..client.client_id_);
        end
    end
end