--[[********************************************************************
	created:	2018/01/19
	author:		lixuguang_cx

	purpose:	基础支持函数
*********************************************************************--]]


function unpackMessage(msg_str)
	local dd = {};
	if #msg_str==0 then
		return dd;
	end
	local r1, r2, r3 = pcall(protected_.mp_.game_unpack, msg_str);
    if not r1 then
		gameLog("unpackMessage error: "..dd);
        return nil;
    end
    dd = r2;
	local cur_i = r3;
    if cur_i < #msg_str then
		gameLog("unpackMessage error: "..cur_i.."/"..(#msg_str));
		print("unpackMessage error traceback:", debug.traceback());
        return nil;
    end
	return dd;
end
protected_.stringTable = function (t, lh)
    if t.base_ then
        t=t.base_;
    end
    cur_path = {};
    stack_tab = {};
    stack_tab[t] = "{}";
    return protected_._stringTable(t, lh);
end
protected_._stringTable = function (t, lh)
    if t.base_ then
        t=t.base_;
    end
    lh = lh or "";
	local hs="	";
	hs = lh.."	";
	local line_h = "\n"..hs;
	local str="{"
	local flag = false;
	for k,v in pairs(t) do
		if flag then
			str=str..",";
		end
		if type(k)=="number" then
			str = str..line_h.."["..k.."] = ";
		else
			str = str..line_h..k.." = ";
		end
		if type(v)=="table" then
            if stack_tab[v]==nil then
                table.insert(cur_path, k);
                stack_tab[v] = protected_._stringTable(cur_path);
			    str = str..protected_._stringTable(v, hs);
                table.remove(cur_path);
            else
                str = str..stack_tab[v];
            end;
		elseif type(v)=="string" then
			str = str.."\""..v.."\"";
		else
			str = str..tostring(v)
		end
		flag = true;
	end
	str = str.."\n"..lh.."}";
	return str;
end;

--与C++的lowerbound不同，数组从小到大排序，返回小于等于v的值
function lowerBound( arr, v )
    if type(v)~="number" then
        print("########################"..debug.traceback());
    end
    local left = 1;
    local right = table.getn(arr);
    local ev = arr[left];
    if type(ev)~="number" then
        print("@@########################"..debug.traceback());
    end
    if v<ev then
        return ;
    end
    if v>=arr[right] then
        return right, arr[right];
    end
    local mid = 0;
    while right>left+1 do 
        mid = math.ceil((left+right)/2);
        ev = arr[mid];
        if ev==v then
            return mid,ev;
        elseif ev<v then
            left = mid;
        else
            right = mid;
        end
    end
    return left, arr[left];
end
--与C++的lowerbound不同，数组从小到大排序，返回小于等于v的值
function lowerBound2( arr, v )
    if type(v)~="number" then
        print("########################"..debug.traceback());
        return ;
    end
    local left = 1;
    local right = table.getn(arr);
    local ev = arr[left];
    if type(ev)~="table" then
        print("@@########################"..debug.traceback());
        return ;
    end
    if v<ev[1] then
        return 0, nil;
    end
    if v>=arr[right][1] then
        return right, arr[right];
    end
    local mid = 0;
    while right>left+1 do 
        mid = math.ceil((left+right)/2);
        ev = arr[mid];
        if ev[1]<=v then
            left = mid;
        else
            right = mid;
        end
    end
    return left, arr[left];
end

--垃圾回收
protected_.collectgarbage = function(handler_context)
	local interval_time = handler_context.context_[1] or 1000;
	local step_size = handler_context.context_[2] or 1024;
    local ret = collectgarbage("step", step_size);-- collectgarbage("collect");--30G/3M*10=1000s
    -- gameLog("collectgarbage step return "..tostring(ret));
    if ret then
        regExpireHandler(getTime()+interval_time, nil, protected_.collectgarbage, nil, {})
    else
        regExpireHandler(getTime()+interval_time, nil, protected_.collectgarbage, nil, {})
    end
    return 0;
end

--[[ 
	轮盘赌
	crit的形式是{{weight,....}，{weight,....}，{weight,....} } 
	或者 {rands={{weight,....}，{weight,....}，{weight,....}} }
	第二种形式可以避免反复计算总数
--]]
protected_.rollRand = function(crit)
    if crit.rands==nil then
        crit = {rands=crit};
    end
    if crit.rand_total==nil then
        crit.rand_total = 0;
        for i,v in ipairs(crit.rands) do
            crit.rand_total = crit.rand_total+v[1];
        end
    end
    local r_num = math.random()*crit.rand_total;
    for i,v in ipairs(crit.rands) do
        if r_num<=v[1] then
            return v;
        end
        r_num = r_num-v[1];
    end
    return crit.rands[#crit.rands];
end

protected_.randArray=function(r_set)
	local r_num = #r_set;
	local tmp;
	while r_num>1 do
		local rr = math.random(1, r_num);
		if rr~=r_num then
			tmp = r_set[r_num];
			r_set[r_num] = r_set[rr];
			r_set[rr] = tmp;
			r_num = r_num-1;
		end
	end
	return r_set;
end

protected_.rand_m_n=function(m,n,num)
	local m_ax = m;
	local m_in = n;
	local result = {};
	if m<n then
		m_ax = n;
		m_in = m;
	elseif m==n then
		return {m};
	end
	if m_ax-m_in<num then
		for i=m_in,m_max do
			table.insert(result, i);
		end
		return protected_.randArray(result);
	end
	local r_map={};
	local r_count = num;
	local rr;
	while r_count>0 do
		rr = math.random(m,n);
		if r_map[rr]==nil then
			r_map[rr] = r_count;
			r_count = r_count - 1;
		end
	end
	-- 不这样做，结果排序不是随机的
	for k,v in pairs(r_map) do
		result[v] = k;
	end
	return result;
end

--避免重复写函数
protected_.rand_0_1 = function()
	return math.random();
end

--四舍五入，向0靠近
protected_.round = function(vv)
	local sign = 1;
	if vv<0 then
		sign = -1;
		vv = -vv;
	end
	local b_v = math.floor(vv);
	if vv-b_v >= 0.5 then
		return sign*(b_v+1);
	else
		return sign*b_v;
	end
end

--string库中有好几个好用的函数，尽量使用原生库函数
protected_.stringSplit = function(in_str, seperator)
	local rr = {};
	seperator = seperator or " ";
	local match_str = string.format("[^%s]+", seperator);
	for k, v in string.gmatch(in_str, match_str)do table.insert(rr, k); end
	return rr;
end

-- game_time 以毫秒为单位的绝对时间
protected_.dateToString = function(game_time)
    return os.date("%Y-%m-%d %H:%M:%S", math.floor(game_time / 1000))
end

-- game_time 以毫秒为单位的绝对时间
protected_.dateToTable = function(game_time)
    return os.date("*t", math.floor(game_time / 1000))
end

--获取时区
protected_.getTimezone = function()
  local now = os.time()
  return os.difftime(now, os.time(os.date("!*t", now)))
end

--获取当天指定时刻 game_time 以毫秒为单位的绝对时间
protected_.getDayTime = function(game_time, h, m, s)
    local cur_time = protected_.dateToTable(game_time);
	return os.time({year = cur_time.year, month = cur_time.month, day = cur_time.day, hour = h or cur_time.hour, min = m or cur_time.min, sec = s or cur_time.sec})*1000;
end

-- 计算两个时间点相差天数，不到一天算一天
protected_.countDays = function(begin_time, cur_time)
	if begin_time>=cur_time then
		return 0;
	end
	local zero_time = protected_.getDayTime(begin_time, 0, 0, 0);
	local dt = cur_time-zero_time;
	return math.ceil(dt/(24*3600*1000));
end


protected_.getWideStringCount = function(inputstr)
    -- 计算字符串宽度
    -- 可以计算出字符宽度，用于显示使用
   local lenInByte = #inputstr
   local width = 0
   local i = 1
    while (i<=lenInByte) 
    do
        local curByte = string.byte(inputstr, i)
        local byteCount = 1;
        if curByte>0 and curByte<=127 then
            byteCount = 1                                               --1字节字符
        elseif curByte>=192 and curByte<223 then
            byteCount = 2                                               --双字节字符
        elseif curByte>=224 and curByte<239 then
            byteCount = 3                                               --汉字
        elseif curByte>=240 and curByte<=247 then
            byteCount = 4                                               --4字节字符
        end
         
        i = i + byteCount                                              -- 重置下一字节的索引
        if byteCount >= 3 then -- 汉字3或4个字节
            len = 2
        else
            len = 1
        end
        width = width + len                                             -- 字符的个数（长度）
    end
    return width
end

function IsInTable(value, tbl)
	for k,v in ipairs(tbl) do
		if v == value then
			return true;
		end
	end
	return false;
end

-- trim
protected_.trim = function(s)
    return string.gsub(s, "^%s*(.-)%s*$", "%1");
end