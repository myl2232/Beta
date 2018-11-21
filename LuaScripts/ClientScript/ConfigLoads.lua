--[[********************************************************************
	created:	2018/01/19
	author:		lixuguang_cx

	purpose:	配置文件加载
*********************************************************************--]]
function doLoadConfig(file_name)
	configs_[file_name] = dofile(config_path_..file_name..".lua");
end

------------------------------------加载配置--------------------------------------------

--[[ doLoadConfig("GameConfigs");
doLoadConfig("FightField");
doLoadConfig("UnitCfg");
doLoadConfig("HeroProfessionCfg");
doLoadConfig("SkillCfg");
doLoadConfig("SkillAreaCfg");
doLoadConfig("SkillStateCfg");
doLoadConfig("FormationCfg");
doLoadConfig("SummonData");
doLoadConfig("FightMark");
doLoadConfig("QTECfg");
doLoadConfig("Item");
doLoadConfig("PetRefresh");
doLoadConfig("TrumpCfg");
doLoadConfig("LotteryPack");
doLoadConfig("LotteryPool");
doLoadConfig("Posy");
doLoadConfig("SeasonData");
doLoadConfig("SeasonCfg");
doLoadConfig("Dialog");
doLoadConfig("Reward");
doLoadConfig("Quest")
doLoadConfig("QuestGroup")
doLoadConfig("PlayerLevel")
doLoadConfig("BeckonCfg")
doLoadConfig("BeckonPool") ]]

--[[ protected_.getPlayer = function(player_uid)
	local client = protected_.playerClients_[player_uid];
	if client==nil then
		return nil;
	end
	return client.player_;
end
protected_.handleDialog = function(player_uid, smsg)
	local client = protected_.playerClients_[player_uid];
	if client==nil then
		return -1;
	end
	return protected_.msg_handlers_:handlePacket(client.client_id_, smsg);
end
protected_.setDialogContext = function(player_uid, dialog_sid, context)
	local client = protected_.playerClients_[player_uid];
	if client==nil then
		return;
	end
	client.player_.dialog_contexts_[dialog_sid] = context;
	sendMessage(client.client_id_, {EC2SProtocol["EC2SProtocol_DialogSave"], player_uid, dialog_sid, context});
end

configs_.base_skills_ = {};
table.insert(configs_.base_skills_, configs_.defend_skill_);--13
table.insert(configs_.base_skills_, configs_.summon_skill);--14
table.insert(configs_.base_skills_, configs_.protect_skill);--15
for i,v in ipairs(configs_.medicine_skills_) do
	table.insert(configs_.base_skills_, v);
end ]]
