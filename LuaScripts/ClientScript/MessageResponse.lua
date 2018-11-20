protected_.msg_responses_[EC2SProtocol["EC2SProtocol_ReConnect"]] = function(client_id, result, msg_context)
     print("recv EC2SProtocol_ReConnect and result == "..result);
	if result==1 then
		protected_.clients_[client_id].client_state_ = nil;
	elseif result~=0 then
		protected_.clients_[client_id].client_state_ = nil;
	else
		--protected_.clients_[client_id].client_state_ = 3; -- Reconnecting
        print("reconnect the server ok!!!!");
	end
end

protected_.dealCmd = function(cmds, times, temp_cmd)
	if not temp_cmd then
		return ;
	end
	times = times or 1;
	for _, cmd in pairs(cmds) do
		if #cmd == 2 then
			if type(cmd[2]) == "number" then
				temp_cmd[cmd[1]] = (temp_cmd[cmd[1]] or 0) + cmd[2] * times;
			elseif type(cmd[2]) == "table" then
				local t = cmd[2];
				for key, num in pairs(cmd[2]) do
					if not temp_cmd[cmd[1]] then
						temp_cmd[cmd[1]] = {};
					end
					if type(temp_cmd[cmd[1]]) ~= "table" then
						print(" cmd[1] error " , cmd[1]);
						break;
					end
					temp_cmd[cmd[1]][key] = (temp_cmd[cmd[1]][key] or 0) + num * times;
				end
			end
		elseif #cmd == 3 then
			if not temp_cmd[cmd[1]] then
				temp_cmd[cmd[1]] = {};
			end
			temp_cmd[cmd[1]][cmd[2]] = (temp_cmd[cmd[1]][cmd[2]] or 0) + cmd[3] * times;
		end
	end
end

protected_.msg_responses_[EC2SProtocol["EC2SProtocol_Login"]] = function(client_id, result, msg_context)
    if result~=0 then
		protected_.clients_[client_id] = nil;
	end
end

protected_.msg_responses_[EC2SProtocol["EC2SProtocol_SummonPet"]] = function(client_id, result, msg_context)
	if result == 0 then
		client_recall_("onSummonPet", client_id, msg_context);
	end
end
protected_.msg_responses_[EC2SProtocol["EC2SProtocol_BuyHero"]] = function(client_id, result, msg_context)
	if result == 0 then
		client_recall_("onBuyHero", client_id, msg_context[1]);
	end
end
protected_.msg_responses_[EC2SProtocol["EC2SProtocol_PetTroopAddPet"]] = function(client_id, result, msg_context)
	if result == 0 then
		client_recall_("petTroopChange", client_id);
	end
end
protected_.msg_responses_[EC2SProtocol["EC2SProtocol_PetTroopRemovePet"]] = function(client_id, result, msg_context)
	if result == 0 then
		client_recall_("petTroopChange", client_id);
	end
end
protected_.msg_responses_[EC2SProtocol["EC2SProtocol_SellPet"]] = function(client_id, result, msg_context)
	if result == 0 then
		client_recall_("onSellPet", client_id, msg_context[1]);
	end
end

protected_.msg_responses_[EC2SProtocol["EC2SProtocol_ResolvePet"]] = function(client_id, result, msg_context)
	if result == 0 then
		client_recall_("onResolvePet", client_id, msg_context);
	end
end
protected_.msg_responses_[EC2SProtocol["EC2SProtocol_RefreshProp"]] = function(client_id, result, msg_context)
	if result == 0 then
		client_recall_("onRefreshProp", client_id, msg_context[1]);
	end
end
protected_.msg_responses_[EC2SProtocol["EC2SProtocol_ReplaceProp"]] = function(client_id, result, msg_context)
	if result == 0 then
		client_recall_("onReplaceProp", client_id, msg_context[1]);
	end
end
protected_.msg_responses_[EC2SProtocol["EC2SProtocol_LearnSkill"]] = function(client_id, result, msg_context)
	if result == 0 then
		client_recall_("onLearnSkill", client_id, msg_context);
	end
end
protected_.msg_responses_[EC2SProtocol["EC2SProtocol_SellItem"]] = function(client_id, result, msg_context)
	if result == 0 then
		local get_cmd = {};
		for item_sid, count in pairs(msg_context) do
			local item_data = protected_.MetaItemBag.getItemData(item_sid);
			local sell_cmd = item_data.sell;
			protected_.dealCmd(sell_cmd, count, get_cmd);
		end
		local rt_cmd = protected_.commands_.recoverCmd(get_cmd);
		client_recall_("onSellItem", client_id, rt_cmd);
		-- client_recall_("onGetThing", client_id, rt_cmd);
	end
end
protected_.msg_responses_[EC2SProtocol["EC2SProtocol_SellItemBySid"]] = function(client_id, result, msg_context)
	if result == 0 then
		local get_cmd = {};
		for item_sid, count in pairs(msg_context) do
			local item_data = protected_.MetaItemBag.getItemData(item_sid);
			local sell_cmd = item_data.sell;
			protected_.dealCmd(sell_cmd, count, get_cmd);
		end
		local rt_cmd = protected_.commands_.recoverCmd(get_cmd);
		client_recall_("onSellItemBySid", client_id, rt_cmd);
		-- client_recall_("onGetThing", client_id, rt_cmd);
	end
end
protected_.msg_responses_[EC2SProtocol["EC2SProtocol_UseItem"]] = function(client_id, result, msg_context)
	if result == 0 then
		local rt_cmd = protected_.commands_.recoverCmd(msg_context);
		client_recall_("onUseItem", client_id, rt_cmd);
		-- client_recall_("onGetThing", client_id, );
	end
end
protected_.msg_responses_[EC2SProtocol["EC2SProtocol_BuyItem"]] = function(client_id, result, msg_context)
	if result == 0 then
		client_recall_("onBuyItem", client_id, msg_context);
	end
end
protected_.msg_responses_[EC2SProtocol["EC2SProtocol_Lottery"]] = function(client_id, result, msg_context)
	if result == 0 then
		client_recall_("onLottery", client_id, msg_context);
	end
end

protected_.msg_responses_[EC2SProtocol["EC2SProtocol_CreateAlliance"]] = function(client_id, result, msg_context)
    if result == 0 then
        print("create-req ok")
    end
end

protected_.msg_responses_[EC2SProtocol["EC2SProtocol_JoinAlliance"]] = function(client_id, result, msg_context)
    if result == 0 then
        print("join-req ok")
    end
end

protected_.msg_responses_[EC2SProtocol["EC2SProtocol_LeaveAlliance"]] = function(client_id, result, msg_context)
    if result == 0 then
        print("leave-req ok")
    end
end

protected_.msg_responses_[EC2SProtocol["EC2SProtocol_GetAllianceMembers"]] = function(client_id, result, msg_context)
    if result == 0 then
        print("get-req ok", msg_context[1])
    end
end
protected_.msg_responses_[EC2SProtocol["EC2SProtocol_InstallPosy"]] = function(client_id, result, msg_context)
    if result == 0 then
		client_recall_("onInstallPosy", client_id);
    end
end
protected_.msg_responses_[EC2SProtocol["EC2SProtocol_UninstallPosy"]] = function(client_id, result, msg_context)
    if result == 0 then
        client_recall_("onUninstallPosy", client_id);
    end
end
protected_.msg_responses_[EC2SProtocol["EC2SProtocol_UninstallAllPosy"]] = function(client_id, result, msg_context)
    if result == 0 then
        client_recall_("onUninstallAllPosy", client_id);
    end
end
protected_.msg_responses_[EC2SProtocol["EC2SProtocol_UnlockProgram"]] = function(client_id, result, msg_context)
    if result == 0 then
        client_recall_("onUnlockProgram", client_id);
    end
end
protected_.msg_responses_[EC2SProtocol["EC2SProtocol_FetchSeasonReward"]] = function(client_id, result, msg_context)
	if result == 0 then
		local rt_cmd = protected_.commands_.recoverCmd(msg_context);
		client_recall_("onFetchSeasonReward", client_id, rt_cmd);
	end
end
protected_.msg_responses_[EC2SProtocol["EC2SProtocol_ProgramSetName"]] = function(client_id, result, msg_context)
	if result == 0 then
		client_recall_("onProgramSetName", client_id);
	end
end
protected_.msg_responses_[EC2SProtocol["EC2SProtocol_UnlockPetTroop"]] = function(client_id, result, msg_context)
	if result == 0 then
		client_recall_("onUnlockPetTroop", client_id, msg_context[1]);
	end
end
protected_.msg_responses_[EC2SProtocol["EC2SProtocol_PetTroopSetName"]] = function(client_id, result, msg_context)
	if result == 0 then
		client_recall_("onPetTroopSetName", client_id, msg_context[1]);
	end
end

protected_.msg_responses_[EC2SProtocol["EC2SProtocol_BeckonPet"]] = function(client_id, result, msg_context)
    if result == 0 then
        client_recall_("onBeckonPet", client_id, msg_context)
    end
end