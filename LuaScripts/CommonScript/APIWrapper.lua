--[[********************************************************************
	created:	2018/01/19
	author:		lixuguang_cx

	purpose:	对外部API的包装
*********************************************************************--]]

sendMessage = function (client_id, smsg)
    local game_player = protected_.online_users_[client_id];
    if game_player==nil or game_player.conn_state_~=1 then
        return;
    end
	if smsg[1]==nil then
		gameAssert(false, debug.traceback());
    else
        --print("**sendMessage "..smsg[1]);
	end
	local r, s = pcall(protected_.mp_.pack, smsg);
    if not r then
        gameLog("sendMessage["..client_id.."] error: "..s);
        return;
    end
	local n = #s;
	-- 大端 网络字节顺序
	lua_sendMessage(client_id, smsg[1], string.char(math.floor(n / 0x1000000), math.floor(n / 0x10000) % 0x100, math.floor(n / 0x100) % 0x100, n % 0x100)..s);
end
