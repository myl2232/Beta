---
--- Created by jinguanghua.
--- DateTime: 2018/3/9 12:07
---

local Lplus = require "Lplus"
local TBNet = Lplus.Class("TBNet")
local socket = require "socket"
local def = TBNet.define

local instance = nil

def.field("table").listeners = nil

def.static("=>", TBNet).Instance = function ()
    if instance == nil then
        instance = TBNet()
        instance.listeners = {}
    end
    return instance
end

def.method().Start =  function (self)
    --初始化
    --instance.SetServerInfo(self, "127.0.0.1", 20012)
    --初始化服务器共用lua脚本相关 {ClientScript, CommonScript, configs}
    protected_ = {}
    Enum = {}

    common_path_ = "CommonScript/";
    config_path_ = "configs/"; --服务器的表--
    script_path_ = "ClientScript/";

    print(script_path_)
    dofile(script_path_ .. "ClientBase.lua")

end

--------------公用对外方法-------
--[[添加网络回调事件
    protoinfo: 协议信息，服务器现在定的是一串字符串
    callback: 回调函数
--]]
def.final("string", "function").AddResponse = function (protoinfo, callback)
    instance:_AddResponse(protoinfo, callback)
end
--[[移除网络回调事件--]]
def.final("string", "function").RemoveResponse = function (protoinfo, callback)
    instance:_RemoveResponse(protoinfo, callback)
end
--[[触发网络回调事件--]]
def.final("string", "table").DispatchResponse = function (protoinfo, param)
    instance:_DispatchResponse(protoinfo, param)
end

--------------事件部分-----------

def.method("string", "function")._AddResponse = function(self, protoinfo, callback)
    local listeners = self.listeners[protoinfo]
    if listeners == nil then
        listeners = {}
        self.listeners[protoinfo] = listeners
    end
    for _, v in pairs(listeners) do
        if v.f == callback then
            return
        end
    end
    table.insert(listeners, {f = callback})
    --instance:RegistServerCallback(protoinfo) --注册ClientCallback
end

def.method("string", "function")._RemoveResponse = function(self, protoinfo, callback)
    local listeners = self.listeners[protoinfo]
    if listeners == nil then return end
    for k, v in pairs(listeners) do
        if(v.f == callback) then
            table.remove(listeners, k)
            return
        end
    end
end

def.method("string", "table")._DispatchResponse = function (self, protoinfo, param)
    local listeners = instance.listeners[protoinfo]
    if listeners == nil then return end
    for _, v in ipairs(listeners) do
        v.f(param)
    end
end

--[[
    --设置全局变量 ip和port 用来后面连接--
    def.method("string", "number").SetServerInfo = function(self, ip, port)
        server_ip = ip
        server_port = port
        print("IP:"..server_ip..":"..server_port)
    end
--]]

-------------------------与服务器代码连接部分----------------------------
--[[向服务器发送协议
    protoinfo: 协议信息，服务器现在定的是一串字符串，比如登录是login
    params: 协议内容，table格式，每个单元的数据根据具体协议填写
--]]
def.final("string", "table").SendRequest = function( protoinfo, params)
    print("SendRequest",protoinfo)
    if params then
        doLuaCommand(protoinfo,protected_.cur_client_id_ or 1, unpack(params))
    else
        doLuaCommand(protoinfo,protected_.cur_client_id_ or 1)
    end 
end

def.final("string", "table").SendChat = function( protoinfo, params)
    print("SendRequest",protoinfo)
    if params then
        local a = serialize(params)
        doLuaCommand(protoinfo,protected_.cur_client_id_ or 1, a)
    else
        doLuaCommand(protoinfo,protected_.cur_client_id_ or 1)
    end 
end

def.final("string", "string").SendVoiceChat = function( protoinfo, params)
    if params then
        doLuaCommand(protoinfo,protected_.cur_client_id_ or 1, params)
    else
        doLuaCommand(protoinfo,protected_.cur_client_id_ or 1)
    end 
end

def.final("string","string").SendRequestStr = function (proto,param)
    doConsoleCommand(proto,param)
end
--内部专用，用来处理与服务器代码部分的连接
--[[def.method("string").RegistServerCallback = function(self, proto)
    --TODO 等光哥做完基础回调， 然后回调统一放下面的OnServerRes
    --已更改为下面的_G.client_recall_
end]]--
--服务器返回的回调--
def.final("string", "table").OnServerRes = function(proto, params)
    TBNet.DispatchResponse(proto, params)
end
--服务器更新消息监听部分--
def.method().OnUpdate = function(self)
    protected_:updateScript()
end

--对应服务器版脚本的Log--
function gameAssert (self, x, ...)
    local arg = {...}
    local logstr = table.concat(arg, ",")
    if logstr~="" then
        print(logstr)
    end
end

TBNet.Commit()
TBNet.Instance()
_G.TBNet =TBNet
_G.client_recall_ = function (proto,...)
    local arg ={...}
    TBNet.OnServerRes(proto,arg)
end
return TBNet