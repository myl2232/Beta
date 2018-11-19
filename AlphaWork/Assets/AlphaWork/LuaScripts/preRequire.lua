
-- local l_warn_old = warn
-- function _G.warn(...)
-- 	l_warn_old(...,debug.traceback())
-- end

--居然在c#端执行的
local step = 0
local function preRequire()
	--
	--禁止 global
	--
	step = step + 1
    print("*******************",step)
	if step == 1 then
		require("Data.ConfigDefine")
		require("Game.GDefine")
		--require("Net.TBNet") --myl:暂时不用服务器功能
		require("Net.Protodefine")
		require "Event.Event".Instance()
		_G.GameUtil = require "Main.GameUtil"
		_G.TBGame = require "Main.TBGame"
	else
		return true
	end

	return false

end

return preRequire