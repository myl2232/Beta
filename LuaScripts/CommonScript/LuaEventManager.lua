
protected_.LuaEventManager.getEventHandlers = function(self, game_event)
	local mm = self;
	for i=1,3 do
		local kk = game_event[i];
		if not kk then
			kk = 0;
		end
		local m = mm[kk];
		if not m then
			return nil;
		end
		mm = m;
	end
	return mm;
end
protected_.LuaEventManager.clearEventHandlers = function (self, game_event)
	local parents = {self};
	for i=1,3 do
		local kk = game_event[i];
		if not kk then
			kk = 0;
		end
		local m = parents[i][kk];
		if not m then
			break;
		end
		table.insert(parents, m);
	end
	local indx = 3;
	local pp = parents[indx];
	local kk = game_event[indx] or 0;
	if pp and pp[kk] then
		pp[kk] = nil;
		for i=indx-1,1,-1 do
			if next(pp) then
				break;
			end
			pp = parents[i];
			kk = game_event[i] or 0;
			pp[kk] = nil;
		end
	end
end
protected_.LuaEventManager.registEventHandler = function (self, game_event , handle_even_func, handle_close_func, context)
    local handler_context = {};
    setmetatable(handler_context, protected_.EventContextMeta);
    handler_context.game_event_ = game_event;
    handler_context.context_ = context;
    -- local trace_str = debug.traceback();
    --handler_context.trace = debug.traceback();
    -- protected_.tick_tracebacks[str] = (protected_.tick_tracebacks[str] or 0)+1;
    if handle_even_func then
        handler_context.handle_func = handle_even_func;
    end
    if handle_close_func then
        handler_context.close_func = handle_close_func;
    end
	local mm = self;
	for i=1,3 do
		local kk = game_event[i];
		if not kk then
			kk = 0;
		end
		local m = mm[kk];
		if not m then
			m = {};
			mm[kk] = m;
		end
		mm = m;
	end
	mm[handler_context] = 1;
	return handler_context;
end
protected_.LuaEventManager.removeEventHandlers = function (self, handler_context)
	local game_event = handler_context.game_event_;
	if game_event==nil then
		return;
	end
	pp = self:getEventHandlers(game_event);
	if pp==nil then
		return;
	end
	pp[handler_context] = nil;
	if next(pp)==nil then
		local indx = 3;
		local kk = game_event[indx] or 0;
		local parents = {self};
		for i=1,3 do
			local kk = game_event[i];
			if not kk then
				kk = 0;
			end
			local m = parents[i][kk];
			if not m then
				break;
			end
			table.insert(parents, m);
		end
		for i=indx-1,1,-1 do
			if next(pp) then
				break;
			end
			pp = parents[i];
			kk = game_event[i] or 0;
			pp[kk] = nil;
		end
	end
end
protected_.LuaEventManager.raiseEvent = function (self, game_event, context)
	handlers = self:getEventHandlers(game_event);
	if handlers==nil then
		return;
	end
	for k,v in pairs(handlers) do
		local handle_ret = 0;
		if k.handle_func then
			handle_ret = k.handle_func(k, context) or 0;
		end
		if handle_ret<=0 then
			if k.close_func then
				k.close_func(k);
			end
			handlers[k]=nil;
		end
	end
end