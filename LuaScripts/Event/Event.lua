local Lplus = require "Lplus"
local Event = Lplus.Class("Event")
local bit = require "bit"
local def = Event.define
local instance = nil
_G.Event = Event

def.field("table").listeners = nil

def.static("=>", Event).Instance = function ()
    if(instance == nil) then
	    instance = Event()
        instance.listeners = {}
    end
	return instance
end
def.final("number", "number", "table","function").RegisterMemberEvent = function (moduleId, eventId,funcTable, func)
    local k = bit.lshift(moduleId, 16) + eventId
    instance:_AddEventListener(k, func, funcTable)
end

def.final("number", "number", "function").RegisterEvent = function (moduleId, eventId, func)
    local k = bit.lshift(moduleId, 16) + eventId
    instance:_AddEventListener(k, func, nil)
end

def.final("number", "number", "function").UnregisterEvent = function (moduleId, eventId, func)
    local k = bit.lshift(moduleId, 16) + eventId
    instance:_RemoveEventListener(k, func)
end

def.final("number", "number", "function").UnRegisterEvent = function (moduleId, eventId, func)
    instance:UnregisterEvent(moduleId, eventId, func)
end

def.final("number", "number", "dynamic").DispatchEvent = function (moduleId, eventId, param)
    local k = bit.lshift(moduleId, 16) + eventId
    if _G.EventLog then
        local modulename = "nil"
        for k,v in pairs(ModuleId) do
            if moduleId == v then
                modulename = k
            end
        end
        local eventidname = "nil"
        if gmodule.notifyId[modulename] then
            for k,v in pairs(gmodule.notifyId[modulename]) do
                if eventId == v then
                    eventidname = k
                end
            end
        end
        local listeners = instance.listeners[k]
        local count = listeners and #listeners or 0
        warn("#LOG#DispatchEvent ====== ModuleId."..modulename,"gmodule.notifyId."..modulename.."."..eventidname,count.." listeners] fr:"..Time.frameCount.."==============================",debug.traceback())
    end
    instance:_DispatchEvent(k, param)
end

def.method("number", "function", "table")._AddEventListener = function (self, eventId, func, param)
    local listeners = self.listeners[eventId]
    if listeners == nil then
        listeners = {}
        self.listeners[eventId] = listeners
    end
    for _, v in pairs(listeners) do
        if(v.f == func) then
            return
        end
    end
    table.insert(listeners, {f=func, p=param})
end

def.method("number", "function")._RemoveEventListener = function (self, eventId, func)
    local listeners = self.listeners[eventId]
    if listeners == nil then return end
    for k, v in pairs(listeners) do
        if(v.f == func) then
            table.remove(listeners, k)
            return
        end
    end
end

def.method("number", "dynamic")._DispatchEvent = function (self, eventId, param)
    local listeners = instance.listeners[eventId]
    if listeners == nil then return end
    for _, v in ipairs(listeners) do
        if v.p then
            v.f(v.p, param)
        else
            v.f(param)  --should be: v.f(param)
        end
    end
end

return Event.Commit()