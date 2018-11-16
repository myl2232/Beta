
local preRequire = require("preRequire")
--启动方法的
function Start()
    while(not preRequire()) do

    end

    TBGame.Instance():Start()
end

--场景切换通知
function OnLevelWasLoaded(level)
	collectgarbage("collect")
	Time.timeSinceLevelLoad = 0
end

function OnApplicationQuit()
    --TBGame.Instance():ApplicationQuit()
end

function OnApplicationPause()
    SaveToFile()
end