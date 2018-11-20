--[[********************************************************************
	created:	2018/01/19
	author:		lixuguang_cx

	purpose:	日志相关
*********************************************************************--]]
local dump_objs_={};
function dump_obj(obj)
	if type(obj)~="table" then
		return tostring(obj);
	end
    dump_objs_={[obj]="."};
    local ret = _dump_obj(obj, nil, nil, nil, dump_objs_)
    dump_objs_=nil;
    return ret or "";
end
function _dump_obj(obj, key, sp, lv, st)
    sp = sp or '  '
 
    if type(obj) ~= 'table' then
        return sp..(key or '')..' = '..tostring(obj)..'\n'
    end
 
--    if obj.base_ then
--        obj = obj.base_;
--    end
    local ks, vs, s= { mxl = 0 }, {}
    lv, st =  lv or 1, st or {}
 
    st[obj] = key or '.' -- map it!
    key = key or ''
    for k, v in pairs(obj) do
        if type(k) == "string" then
            k = '"' .. k .. '"'
        end
        if type(v)=='table' then
            if st[v] then -- a dumped table?
                table.insert(vs,'['.. st[v]..']')
                s = sp:rep(lv)..tostring(k)
                table.insert(ks, s)
                ks.mxl = math.max(#s, ks.mxl)
            else
                st[v] =key..'.'..k -- map it!
                table.insert(vs,
                    _dump_obj(v, st[v], sp, lv+1, st)
                )
                s = sp:rep(lv)..tostring(k)
                table.insert(ks, s)
                ks.mxl = math.max(#s, ks.mxl)
            end
        else
            if type(v)=='string' then
                table.insert(vs,
                    (('%q'):format(v)
                        :gsub('\\\10','\\n')
                        :gsub('\\r\\n', '\\n')
                    )
                )
            elseif type(v)=='userdata' then
                table.insert(vs, tostring(v))
            else
                table.insert(vs, tostring(v))
            end
            s = sp:rep(lv)..tostring(k)
            table.insert(ks, s)
            ks.mxl = math.max(#s, ks.mxl);
        end
    end
 
    s = ks.mxl
    for i, v in ipairs(ks) do
        vs[i] = v..(' '):rep(s-#v)..' = '..vs[i]..'\n'
    end
 
    return '{\n'..table.concat(vs)..sp:rep(lv-1)..'}'
end
--[[
    快速打印变量 var
]]
function printObject(var,name)
    name = name or "var"
    if(var == nil)then
        print(name .. "is nil ------------- " ); 
        return   
    end
    -- name = name .. " : type == " .. type(var) .. ", value"

    if type(var) == "table" then
        print(name .. " : type == " .. type(var) .. ", value === " .. dump_obj(var))
    else
        print(name .. " : type == " .. type(var) .. ", value === " .. tostring(var))
    end
end

GameDebug = {}
GameDebug.writeLog = function(tag, debug_level, desc)
    gameLog(tag, Enum.ELog_Error, desc);
end
GameDebug.getObjectDesc = function( object )
	local tt = type(object);
	if tt=="string" or tt=="number" then
		return object;
	elseif tt=="table" then
		if object.getDesc then
			return object:getDesc();
		end
	end
	return "[type: "..tt.."]";
end
GameDebug.formatLog = function(tag, debug_level, fmt_str, ...)
	local r, s = pcall(string.format, fmt_str, unpack(arg));
    if not r then
		if type(fmt_str)=="string" then
			gameLog("GameDebug.formatLog ["..fmt_str.."] error!");
		end
		gameLog("GameDebug.formatLog Error: ["..s.."] stack: "..debug.traceback());
	else
		GameDebug.writeLog(tag, debug_level, s);
    end
end
GameDebug.gameLog = function(tag, debug_level, ...)
	local log_str = "";
	for i,v in ipairs(arg) do
		log_str = log_str..(GameDebug.getObjectDesc(v) or "nil");
	end
	GameDebug.writeLog(tag, debug_level, log_str);
end
GameDebug.error = function(tag, ...)
    GameDebug.gameLog(tag, Enum.ELog_Error, ...);
end
GameDebug.warn = function(tag, game_player, ...)
    GameDebug.gameLog(tag, Enum.ELog_Warning, ...);
end
GameDebug.info = function(tag, game_player, ...)
    GameDebug.gameLog(tag, Enum.ELog_Info, ...);
end
GameDebug.debug = function(tag, game_player, ...)  --debug调试   = 3
    GameDebug.gameLog(tag, Enum.ELog_Debug, ...);
end
GameDebug.trace = function(tag, game_player, ...)
    GameDebug.gameLog(tag, Enum.ELog_Trace, ...);
end

GameDebug.to_file = function(t, file_name)
    local file_name = "../" .. file_name or "test.lua"
    print("save_file .. " .. file_name)
    local log = io.open(file_name, "w")
    io.output(log)
    io.write(dump_obj(t))
    io.flush()
    io.close()
    print("save_file .. " .. file_name .. " end")
end

log = {}
log.error = function(tag, game_player, ...)
    local msg = log.getMsg(game_player, ...);
    gameLog(tag, Enum.ELog_Error, msg);
end
log.warn = function(tag, game_player, ...)
    local msg = log.getMsg(game_player, ...);
    gameLog(tag, Enum.ELog_Warning, msg);
end
log.info = function(tag, game_player, ...)
    local msg = log.getMsg(game_player, ...);
    gameLog(tag, Enum.ELog_Info, msg);
end
log.debug = function(tag, game_player, ...)
    local msg = log.getMsg(game_player, ...);
    gameLog(tag, Enum.ELog_Debug, msg );
end
log.trace = function(tag, game_player, ...)
    local msg = log.getMsg(game_player, ...);
    msg = msg .. debug.traceback();
    gameLog(tag, Enum.ELog_Log, msg);
end
log.log = function(tag, game_player, ...)
    local msg = log.getMsg(game_player, ...);
    gameLog(tag, Enum.ELog_Log, msg);
end
log.verbose = function(tag, game_player, ...)
    local msg = log.getMsg(game_player, ...);
    gameLog(tag, Enum.ELog_Verbose, msg);
end
log.var = function(var, name, level)
    if level == nil then
        level = Enum.ELog_Verbose
    end
    
    name = name or "var"
    if(var == nil)then
        gameLog("var", level, name .. "is nil ------------- " ); 
        return   
    end

    if type(var) == "table" then
        gameLog("var", level, name .. " : type == " .. type(var) .. ", value === " .. dump_obj(var))
    else
        gameLog("var", level, name .. " : type == " .. type(var) .. ", value === " .. tostring(var))
    end
end

log.getMsg = function(game_player, ...)
    local msg;
    local player_name_ ;
    if game_player then
        player_name_ = game_player.player_name_;
    else
        player_name_ = "system";
    end

    local description = log.getArgStr(...);

    msg = "<p:" .. player_name_ .. ">";
    if type(description) == "table" then
        msg = msg .. " : " .. dump_obj(description);
    else
        msg = msg .. " : " .. tostring(description);
    end
    return msg;
end
log.getArgStr = function(...)
    local str = "";
    local arg = {...}
    for _, v in ipairs(arg) do
        str = str .. (log.getObjectDesc(v) or "nil");
        str = str .. "  ";
    end
    return str;
end
log.getObjectDesc = function( object )
	local tt = type(object);
	if tt=="string" or tt=="number" then
		return object;
	elseif tt=="table" then
		return dump_obj(object);
	end
	return "[type: "..tt.."]";
end

log.to_file = function(t, file_name)
    local file_name = "../" .. (file_name or "save_file.lua");
    print("save_file .. " .. file_name)
    local log = io.open(file_name, "w")
    io.output(log)
    io.write(dump_obj(t))
    io.flush()
    io.close()
    print("save_file .. " .. file_name .. " end")
end