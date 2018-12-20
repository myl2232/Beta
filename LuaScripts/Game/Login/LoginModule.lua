require("Main.Module.ModuleId");
local Lplus = require "Lplus"
local ModuleBase = require "Main.Module.ModuleBase"
local LoginModule = Lplus.Extend(ModuleBase,"LoginModule")
local def = LoginModule.define
local m_Instance = nil
local UI = AlphaWork.GameEntry.UI;

--ocal mPlayers = nil;
--def.field("table").mPlayers = nil;

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

function LoginModule.FillData(players)
    -- body
    print(".......players....."..players[0].user)
    local params = {}
    params[0] = players[0];

    Event.DispatchEvent(ModuleId.Login, gmodule.notifyId.Login.CREATEPLAYER, params)
end

--[[ def.method().FetchPlayers = function ( playerList )
    -- body
    mPlayers = playerList;
end

function LoginModule.FetchPlayers ( playerList )
    -- body
    --print("....type Players:........"..playerList[0].user)
    self.mPlayers = playerList
end ]]

LoginModule.Commit()
return LoginModule