---
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by jinguanghua.
--- DateTime: 2018/3/12 14:03
---
local Lplus = require "Lplus"
local GameUtil = Lplus.Class("GameUtil")
require "Event.Event".Instance()
local def = GameUtil.define
local ConfigMgr = require("Main.ConfigMgr")
local instance = nil

def.static("=>", GameUtil).Instance = function ()
    if instance==nil then
        instance = GameUtil()
    end
    return instance
end


--加载资源，通过同respath_cfg.lua相同表结构读取信息
--resInfo: 同respath_cfg.lua结构，至少要有path参数
--callback(UnityEngine.Object): 加载成功回调函数
def.final("table", "function").AsyncLoadRes = function(resInfo, callback)
    local path = resInfo["path"] or ""
    local priority = resInfo["priority"] or 0
    --ResourceManager.LoadResourceAsyncWithPath(path, callback, priority)
end
def.final("string","number", "function").AsyncLoadResByPath = function(path,priority, callback)
    --ResourceManager.LoadResourceAsyncWithPath(path, callback, priority)
end
def.final("string","function","boolean","number").LoadBank = function ( path,callback ,immediately,priority)
    --ResourceManager.LoadBank(path,true,callback,immediately,priority);
end

def.final("string").UnLoadBank = function ( path )
    --ResourceManager.UnLoadBank(path);
end
--加载Sprite
--atlasName: 图集名称，通常跟资源名一样也等同于Tag
--spriteName: sprite的名称
--callback(Sprite)
def.final("string", "string", "function").AsyncLoadSprite = function(atlasName, spriteName, callback)
    SpriteManager.LoadSprite(atlasName, spriteName, callback)
end
def.final("string", "function").RequestAtlas = function(atlasName, callback)
    SpriteManager.Instance:RequestAtlas(atlasName, callback)
end
--计时器，用于延时调用函数
--<param> timeInterval: 调用间隔
--<param> loopTime: 调用次数(-1为无限次数)
--<param> func: 调用的函数(两种: 无参数; 带int参数, 当前计时器剩余调用次数, 0表示计时器即将结束被回收)
--<return> timeId
def.final("number", "number", "function", "=>", "number").AddGlobalTimer = function(timeInterval, loopTime, func)
    return TBTimer.AddTimer(timeInterval, loopTime, func);
end

--删除计时器
--timeId: 同上面AddGlobalTimer中返回的timeId
def.final("number").RemoveTimer = function (timerId)
    TBTimer.RemoveTimer(timerId);
end

--为parentobj追加go为子物体
def.final("userdata", "userdata").SetParent = function(go, parentobj)
    if go == nil or parentobj == nil then
        return
    end
    go.transform:SetParent(parentobj.transform, false)
end

--复制并附加到一个物体上变为子物体
--go：要复制的目标GameObject
--parentobj：父物体GameObject
--return：复制出的新物体
def.final("userdata", "userdata", "=>", "userdata").InstantiateWithParent = function(go, parentobj)
    if go == nil or parentobj == nil then
        return nil
    end
    local obj = UnityEngine.Object.Instantiate(go,parentobj.transform)
    obj.name = go.name;
    return obj
end

-- 实例化一个GameObject
def.final("userdata", "=>", "userdata").Instantiate = function (go)
    if (go == nil) then
        return nil;
    end

    local obj = UnityEngine.Object.Instantiate(go);
    obj.name = go.name;
    return obj
end

--寻找GameObject的子物体--
--parent：父节点(GameObject)
--name：子节点路径
--return：子节点GameObject
def.final("userdata", "string", "=>", "userdata").FindChild = function(parent, name)
    local child_transform = parent.transform:Find(name)
    if not child_transform or child_transform == nil then return nil end
    return child_transform.gameObject
end

--获取资源路径
--id：资源ID，对应respath_cfg表内id
def.final("number", "=>", "table").GetResPath = function(id)
    local data = ConfigMgr.GetDataByKey("respath_cfg", id)
    return data
end

def.final("=>","string").GetAssetsPath = function ()
    return ResourceHelper.LocalSavePath
end

----SoundStart

--播放3D音效，要确认FMOD项目内此音效为3D
--<param> soundEventName: fmod声音事件
--<param> obj: 要绑定的3D物体
def.final("string", "userdata").PlaySound = function(soundEventName, obj)
    SoundUtil.PlaySound(soundEventName, obj)
end

--播放背景音乐，播放时旧的会停止然后播放新的
--<param> soundEventName: fmod声音事件
def.final("string").PlayBGM = function(soundEventName)
    SoundUtil.PlayBGM(soundEventName)
end

--停止当前播放的背景音乐
def.final().StopBGM = function()
    SoundUtil.StopBGM()
end

--播放音效，相同音效播放时，旧的将停止然后播放新的
--<param> soundEventName: fmod声音事件
def.final("string").PlaySE = function(soundEventName)
    SoundUtil.PlaySE(soundEventName)
end

--停止音效播放
--<param> soundEventName: fmod声音事件
def.final("string").StopSE = function(soundEventName)
    SoundUtil.StopSE(soundEventName)
end
----SoundEnd

def.final("number","table","=>","table").GetUGUIAnchorPos = function( layer,pos)
    return {x=0,y=0,z=0}
end

-- 格式化战斗时间 分:秒
def.final("number", "=>", "string").GetTimeStr = function (seconds)
	local min = math.modf(seconds / 60);
	local sec = seconds -  min * 60;

	local timerStr = "";
	if (min < 10) then
		timerStr = timerStr .. "0";
	end

	timerStr = timerStr .. min .. ":";

	if (sec < 10) then
		timerStr = timerStr .. "0";
	end

	timerStr = timerStr .. sec;

	return timerStr;
end

def.final("userdata","userdata","=>","userdata").GetAddComponent = function ( obj,compnent)
    local result  = obj:GetComponent(compnent);
    if result == nil then 
        result = obj:AddComponent(compnent);
    end
    return result;
end
--判断一个游戏物体是否为空
def.final("userdata","=>","boolean").IsNil = function (obj)
    if(obj == nil) then return true end
    return tolua.isnull(obj)
end
--根据Gm指令 获取物品信息 代币{货币类型,数量,是否是代币}
def.final("table","=>","table").GetItemcmdInfo = function (cmds)
    local rt = {}
    if type(cmds) ~= "table" then
        return 
    end
    for _, cmd in pairs(cmds) do
        local cmd_type = cmd[1];
        if cmd_type == "addProp" or cmd_type == "delProp" then
            if(type(cmd[2]) == "string") then
                local cointype = GMProxyCoin[cmd[2]]
                table.insert(rt,{cointype,cmd[3],true})
            elseif (type(cmd[2])== "table") then
                for k, v in pairs(cmd[2]) do
                    local cointype = GMProxyCoin[k]
                    table.insert(rt, {cointype,v,true});
                end
            end
        elseif cmd_type == "addItem" then
            if type(cmd[2]) == "number" then
                table.insert(rt, {cmd[2], cmd[3],false});

            elseif type(cmd[2]) == "table" then
                for item_sid, count in pairs(cmd[2]) do
                    table.insert(rt, {item_sid,count,false});
                end
            end
        elseif cmd_type == "randomRewards" then
            for __, vv in pairs(cmd[2]) do
                local c_rt = GameUtil.GetItemcmdInfo(vv[2]);
                for ___, c_rt_cmd in pairs(c_rt) do
                    table.insert(rt, c_rt_cmd);
                end
            end
        end
    end
    return rt;    
end
def.final("number").SetTimeScale= function(timeScale)
    UnityEngine.Time.timeScale = timeScale;
end
def.final("string","string","=>","string").GetRealAnimationName= function(animationName,resName)
    local result = animationName;
    local find =string.find(animationName,"{0}");
    if ( find~=nil and find>0) then
        result = string.gsub(animationName,"{0}", resName,1);
    end
    return result;
end
def.final("string","string","=>","string").GetRealConnectAnimName= function(animationName,resName)
    local result ="";
    local realAnimName = GameUtil.GetRealAnimationName(animationName,resName);
    local configs = ConfigMgr.GetConfig("connect_anim_cfg");
    local connectAnimName = configs[animationName];
    if connectAnimName~=nil then 
        result = GameUtil.GetRealAnimationName(connectAnimName,resName)
    end 
    return result;
end

def.final("number","=>","number").GetOwnMoney = function(cointype)
    local own_money = 0

    if cointype == ProxyCoinType.Gold then
        own_money = protected_.cur_client_.player_.money_
    elseif cointype == ProxyCoinType.Sliver then

    elseif cointype == ProxyCoinType.Soul_Stone then
        own_money = protected_.cur_client_.player_.soul_stone_
    elseif cointype == ProxyCoinType.Spirit_Chip then
        own_money = protected_.cur_client_.player_.spirit_chip_
    elseif cointype == ProxyCoinType.Ofuda then
        own_money = protected_.cur_client_.player_.ofuda_
    end
    return own_money
end
def.final("userdata","number").SetLayer = function(obj,layer)
    for i=0,obj.transform.childCount-1 do
        local childobj = obj.transform:GetChild(i).gameObject
        childobj.layer = layer
        GameUtil.SetLayer(childobj,layer)
    end
end


--
-- lua
-- 判断utf8字符byte长度
-- 0xxxxxxx - 1 byte
-- 110yxxxx - 192, 2 byte
-- 1110yyyy - 225, 3 byte
-- 11110zzz - 240, 4 byte
def.final("number","=>","number").chsize = function(char)
    if not char then
        print("not char")
        return 0
    elseif char > 240 then
        return 4
    elseif char > 225 then
        return 3
    elseif char > 192 then
        return 2
    else
        return 1
    end
end

-- 计算utf8字符串字符数, 各种字符都按一个字符计算
-- 例如utf8len("1你好") => 3
def.final("string").utf8len = function(self,str)
    local len = 0
    local currentIndex = 1
    while currentIndex <= #str do
        local char = string.byte(str, currentIndex)
        currentIndex = currentIndex + GameUtil.chsize(char)
        len = len +1
    end
    return len
end

-- 截取utf8 字符串
-- str:         要截取的字符串
-- startChar:   开始字符下标,从1开始
-- numChars:    要截取的字符长度
def.final("string","number","number","=>","string").utf8sub = function(str,startChar,numChars)
    local startIndex = 1
    while startChar > 1 do
        local char = string.byte(str, startIndex)
        startIndex = startIndex + GameUtil.chsize(char)
        startChar = startChar - 1
    end

    local currentIndex = startIndex

    while numChars > 0 and currentIndex <= #str do
        local char = string.byte(str, currentIndex)
        currentIndex = currentIndex + GameUtil.chsize(char)
        numChars = numChars -1
    end
    return str:sub(startIndex, currentIndex - 1)
end
--设置粒子特效和Mesh特效的层级
def.final("userdata","number").SetEffectSortLayer = function(obj,sortlayer)
    local meshrenders = obj:GetComponentsInChildren(typeof(UnityEngine.MeshRenderer))
    if(meshrenders ~= nil) then
        for i=0,meshrenders.Length -1 do
            meshrenders[i].sortingOrder = sortlayer
        end
    end
    local renders =obj:GetComponentsInChildren(typeof(UnityEngine.Renderer))
    if(renders ~= nil) then
        if(renders ~= nil) then
            for i=0,renders.Length -1 do
                renders[i].sortingOrder = sortlayer
            end
        end
    end
end
GameUtil.Commit()
GameUtil.Instance()

return GameUtil