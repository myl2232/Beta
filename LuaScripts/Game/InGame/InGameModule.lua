local Lplus = require "Lplus"
local ModuleBase = require "Main.Module.ModuleBase"
local InGameModule = Lplus.Extend(ModuleBase,"InGameModule")
local def = InGameModule.define
local m_Instance = nil

def.static("=>",InGameModule).Instance = function()
    if(m_Instance == nil) then
        m_Instance = InGameModule()
        m_Instance.m_moduleId = ModuleId.InGame
    end
    return m_Instance
end

def.override().Init = function(self)
    ModuleBase.Init(self)
   --[[  TBNet.AddResponse(ProtoRecive.onUseItem,LoginModule.onUseItemRev)
    TBNet.AddResponse(ProtoRecive.onSellItem,LoginModule.onSellItemRev) ]]

    
end

InGameModule.Commit()
return InGameModule