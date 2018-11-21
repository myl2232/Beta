--[[********************************************************************
	created:	2018/01/22
	author:		lixuguang_cx

	purpose:	
*********************************************************************--]]
------------------------------------加载root环境--------------------------------------
loads_ = {};	---相当于package.loaded
-- dirty_recall_func_ = nil 是一个脏回调函数function(path, parent, k) path是当前路径，parent为父节点对象，k是父节点key, 当前对象是parent[k]
--gameLog = nil;
if gameError==nil then
	gameError = function(...)
		local args = {...};
		local str = "";
		for i, s in ipairs(args) do
			str = str..(tostring(s));
		end
		if _gameLog then
			_gameLog(str);
		else
			error(str);
		end
	end
end
if gameLog==nil then
	gameLog = function(...)
		if FileLog then
			local args = {...};
			local str = "";
			for i, s in ipairs(args) do
				str = str..(tostring(s));
			end
			FileLog(str);
		else
			if _gameLog then
				local args = {...};
				local str = "";
				for i, s in ipairs(args) do
					str = str..(tostring(s));
				end
				_gameLog(str);
			else
				print(...);
			end
		end
	end
end
if error==nil then
	error = function(...)
		print(...);
	end
end

function getTime()
    return socket.gettime() *1000+protected_.server_time_offset_;
end
--不用require是因为担心载入同名文件引起误会
dofile(common_path_.."EnumDefine.lua")
dofile(common_path_.."GameDefine.lua")
dofile(common_path_.."GameProp.lua")

-- doLuaCommand 输入命令
--全局回调借口
protected_.default_recall = function(call_name, ...)
	if protected_.callbacks_[call_name] then
		protected_.callbacks_[call_name](...);
	end
end

--local client_recall = client_recall_;
--protected_.log_recall = function(call_name, ...)
--	if protected_.callbacks_[call_name] then
--		protected_.callbacks_[call_name](...);
--	end
--	if client_recall then
--		client_recall(call_name, ...);
--	end
--end

--client_recall_ = protected_.log_recall;

if client_recall_==nil then
	client_recall_ = protected_.default_recall;
end
--屏蔽脏数据回调
--mt.dirty_recall------------
if client_define_ then
	protected_.dirty_old_ = true;
	protected_.meta_calls_ = {protected_.markDirtyCall};
else
	protected_.meta_calls_ = {};
end

protected_.mp_ = dofile(common_path_.."MessagePack.lua")

lua_sendMessage = function(connect_id, proto_id, msg_str)
	--print("proto_id="..proto_id)
    if protected_.client_net_ == 1 then
        send_message(connect_id, proto_id, msg_str);
    else
        protected_.NetWork:send_message(connect_id, msg_str);
    end
	
end

lua_connect_server = function(client_id, server_ip, port)
    local ret;
    if protected_.client_net_ == 1 then
        ret = connect_server(client_id, server_ip, port);
    else
        ret = protected_.NetWork:connect_server(client_id, server_ip, port);
    end
    return ret;
end

lua_disconnect_server = function(client_id)
    if protected_.client_net_ == 1 then
        disconnect_server(client_id);
    else
        protected_.NetWork:disconnect_server(client_id);
    end
end
--------------------------------------环境代码load------------------------------------------
dofile(common_path_.."GameLog.lua")
dofile(common_path_.."GameProtocol.lua")
dofile(common_path_.."CommonFunctions.lua")
dofile(common_path_.."APIWrapper.lua")
dofile(common_path_.."EventWork.lua")
dofile(common_path_.."CallbackFunctions.lua")
dofile(common_path_.."DirtyFunctions.lua")
dofile(common_path_.."GameCoroutine.lua")
dofile(common_path_.."Command.lua")
dofile(common_path_.."Utils.lua")

EC2SProtocolStr = {}

for k, v in pairs(EC2SProtocol) do
	EC2SProtocolStr[v] = k;
end

dofile(script_path_.."ClientMetaHeader.lua")
------------------------------------配置load--------------------------------------------
dofile(script_path_.."ConfigLoads.lua")

-----------------------------------逻辑代码load---------------------------------------------
--[[ dofile(common_path_.."GamePetHeader.lua")
dofile(common_path_.."QuestHeader.lua")
dofile(common_path_.."GameUserHeader.lua")
dofile(common_path_.."FightHeader.lua")
dofile(common_path_.."TroopHeader.lua")
dofile(common_path_.."GameHeroHeader.lua")
dofile(common_path_.."ItemHeader.lua")
dofile(common_path_.."TrumpHeader.lua")
dofile(common_path_.."SummonHeader.lua")
dofile(common_path_.."PosyHeader.lua")
dofile(common_path_.."SeasonHeader.lua")
dofile(common_path_.."GameDialog.lua")
dofile(common_path_.."GainRateHeader.lua")
dofile(common_path_.."TeamTroopHeader.lua")
dofile(common_path_.."FightWinRetHeader.lua")

dofile(script_path_.."LuaNetWork.lua");
dofile(script_path_.."ClientCallback.lua")
dofile(script_path_.."ClientCommand.lua")
dofile(script_path_.."MessageHandlers.lua")
dofile(script_path_.."Unit.lua")
dofile(script_path_.."FightWorkerHeader.lua")
dofile(script_path_.."Fight.lua")
dofile(script_path_.."ClientGame.lua")
dofile(script_path_.."ConsoleClient.lua")
dofile(script_path_.."MessageResponse.lua")
dofile(script_path_.."DataChange.lua") ]]

math.randomseed(os.time());