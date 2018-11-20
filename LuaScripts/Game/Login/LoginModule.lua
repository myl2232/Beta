local Lplus = require "Lplus"
local ModuleBase = require "Main.Module.ModuleBase"
local LoginModule = Lplus.Extend(ModuleBase,"LoginModule")
local def = LoginModule.define
local m_Instance = nil

def.static("=>",LoginModule).Instance = function()
    if(m_Instance == nil) then
        m_Instance = LoginModule()
        m_Instance.m_moduleId = ModuleId.Login
    end
    return m_Instance
end

def.override().Init = function(self)
    ModuleBase.Init(self)
   --[[  TBNet.AddResponse(ProtoRecive.onUseItem,LoginModule.onUseItemRev)
    TBNet.AddResponse(ProtoRecive.onSellItem,LoginModule.onSellItemRev) ]]

    
end

LoginModule.Commit()
return LoginModule