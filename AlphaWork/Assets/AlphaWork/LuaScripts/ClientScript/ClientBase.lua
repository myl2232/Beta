common_path_ = common_path_ or "../CommonScript/";
config_path_ = config_path_ or "../Configs/";
script_path_ = script_path_ or "../ClientScript/";
dofile(common_path_.."MetaBase.lua")
dofile(common_path_.."MetaDefine.lua")
dofile(common_path_.."BaseClassHeader.lua")
protected_.server_time_offset_ = 0; --ms

if client_define_==nil then
	package.path = ";../luasocket;../luasocket/lua/?.lua";
	package.cpath = ";../luasocket/?.dll";
end
if socket==nil then
	socket = require "socket";
end
protected_.tick_interval_ = 50;--50ms
protected_.clients_ = protected_.online_users_;
protected_.playerClients_={}--uid-->client
protected_.callbacks_ = {};
protected_.fightHandlers = {}
protected_.fightUpdaters_ = {}
protected_.msg_responses_ = {}
protected_.cur_client_id_ = nil;---当前client_id
protected_.cur_client_ = nil;
protected_.team_troops_ = {}
protected_.handle_protos={};
--dofile(script_path_.."ClientLoads.lua")--myl:加载其他预备数据脚本