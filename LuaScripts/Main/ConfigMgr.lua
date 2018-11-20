local Lplus = require "Lplus"
local ConfigMgr = Lplus.Class("ConfigMgr")
local def = ConfigMgr.define
local _instance = nil

--缓存表格数据
local _cachedConfig = {}


--获取配表的实例方法
--参数name , 不带拓展名
--返回表格table
def.final("string","=>","table").GetConfig = function (name)
	local temp = configs_[name]
	if temp ~= nil then 
		return temp
	end 
	temp = _cachedConfig[name]
	if nil ~= temp then
		return temp
	else
		local fileName = "Data/" .. name .. ".lua"
		local data = dofile(fileName)
		_cachedConfig[name] = data
		return _cachedConfig[name]
	end
	error([[cant't find config : ']] .. name)
	return nil
end

--[[
	根据key值获取配置表的value值 返回值根据具体表格确定
]]
def.final("string","dynamic","=>","dynamic").GetDataByKey = function (name,key)
	local config = ConfigMgr.GetConfig(name)
		local data = config[key]
		if data then
			return data
		else
			error("Config : " .. name .. " 表找不到key = "..(key or "nil") )
			return nil
		end
		return nil 
end

--[[
	根据key值获取配置表的value值 返回值根据具体表格确定
]]
def.final("string","dynamic","dynamic","=>","dynamic").GetDataBySecKey = function (name,key,secKey)
	local data = ConfigMgr.GetDataByKey(name,key);
	if data~=nil then 
		if data[secKey]~=nil then
			return  data[secKey];
		else
			error("Config : " .. name .." key = "..key.. " doen't exits seckey "..(secKey or "nil") )
		end
	end
	return nil;
end

--[[
	直接获取某个代表语言表id的属性转换成对应文字 name[表明] key[主键值]  propName[属性名]
]]
def.final("string","dynamic","string","=>","dynamic").GetDataPropWord = function (name,key,propName)
	-- print("********************GetDataPropWord ",key)
	local data = ConfigMgr.GetDataByKey(name,key)
	local resultStr = "not find "
	if data then 
		if not propName or propName =="" then 
			resultStr = ConfigMgr.GetLanguageValue(data)
		else 
			resultStr = ConfigMgr.GetLanguageValue(data[propName])
		end 
	end
	return resultStr
end

def.final("dynamic","=>","string").GetLanguageValue = function (key)
	local resultStr = ConfigMgr.GetDataByKey("language_cfg",key);
	if resultStr == nil then 
		resultStr ="not find"..key;
	end
	return resultStr
end
--根据模型静态Id和皮肤Id找到模型加载路径
def.final("number","number","boolean","=>","table").GetHeroModelPath = function (sId,skinId,isHigh)
	local entityData =ConfigMgr.GetDataBySecKey("entity_show_cfg",sId,skinId);
	if entityData~=nil then 
		local result =
		{
			type= 1,
			priority = 0,
		};
		if isHigh then 
			local skillResCfg = ConfigMgr.GetConfig("skillres_cfg");
			local hRes =entityData.ResourceName.."_h";
			if skillResCfg[hRes]==nil then 
				result.path = ConfigMgr.GetDataByKey("skillres_cfg",entityData.ResourceName);
			else
				result.path = skillResCfg[hRes];
			end
			return  result;
		else
			result.path = ConfigMgr.GetDataByKey("skillres_cfg",entityData.ResourceName);
			return  result
		end
	end
	return nil;
end
return ConfigMgr.Commit()