local Lplus = require "Lplus"
local ModuleMgr = Lplus.Class("ModuleMgr");
local ModuleBase = require "Main.Module.ModuleBase"
local def = ModuleMgr.define
local instance = nil
local modules = {}
local preloadModules = {}

def.static("=>", ModuleMgr).Instance = function ()
    if(instance == nil) then
        instance = ModuleMgr()
    end
	return instance
end

def.method("table").RegisterModule = function(self, _module)
    local moduleId = _module:GetModuleId();
--    if moduleId <= 0 then
--        error("invalid module id")
--    end
    modules[moduleId] = _module;
end

def.method("number").AddToPreload = function(self, moduleId)
    local _module = modules[moduleId]
    preloadModules[moduleId] = _module;
end

def.method("number", "=>", ModuleBase).GetModule = function(self, moduleId)
    return modules[moduleId];
end

def.method("number").InitModule = function(self, moduleId)
    local _module = modules[moduleId]
    if(_module:IsInited() == false) then
        _module:Init()
    end
end

def.method().InitAllModules = function(self)
    for i = 1, #gmodule.modules do
        local _module = gmodule.modules[i].Instance()
        if(_module:IsInited() == false) then
            _module:Init()
        end
    end
end

def.method().RegisterAllModules = function(self)
--    warn("gmodule count = ", #gmodule.modules )
    for i = 1, #gmodule.modules do
        local moduleItem = gmodule.modules[i]
        self:RegisterModule(moduleItem.Instance())
    end
end

def.method().ResetAllModules = function(self)
    self:ResetAllModulesExcept(nil)
end

def.method("table").ResetAllModulesExcept = function(self, except)
    for i = 1, #gmodule.modules do
        local _module = gmodule.modules[i].Instance()
        local moduleId = _module:GetModuleId()
        if except == nil or not except[moduleId] then
            _module:Reset()
        end
    end
end

def.static("=>", "boolean").isAutoConnect = function()
    return true
end
ModuleMgr.Commit()
return ModuleMgr