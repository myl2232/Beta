local Module = {}

_G.gmodule = Module

Module.moduleId = require("Main.Module.ModuleId");
Module.notifyId = require("Main.Module.NotifyId");
Module.moduleMgr = require "Main.module.ModuleMgr".Instance()
Module.modules =
{
    --require("Game.Login.LoginModule"),

}

function Module.gameStart()

    Module.moduleMgr:RegisterAllModules()
    Module.moduleMgr:InitAllModules()
    Module.ProLoadRes()
end

function Module.ProLoadRes( )


end

return Module
