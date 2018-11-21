--[[********************************************************************
	created:	2018/01/19
	author:		lixuguang_cx

	purpose:	事件系统相关
*********************************************************************--]]
function protected_.removeObjContext(game_entity, handler_context)
	local event_contexts = protected_.obj_contexts_[game_entity];
	if event_contexts == nil then
		gameLog("removeObjContext is nil");
		return;
	end
	event_contexts[handler_context]=nil;
end
function handleGameEvent(event_handler)
	local handler_context = protected_.handler_contexts_[event_handler];
	if handler_context==nil then
		return -1;
	end
    local cur_time = getCurrentTime();
	local ret = -1;
	if handler_context.handle_event then
		ret = handler_context:handle_event();
	end
	if ret==nil then
	  return -1;
	end
    local dd = getCurrentTime()-cur_time;
    if handler_context.trace then
        local trace_data = protected_.tick_tracebacks[handler_context.trace];
        if trace_data==nil then
            protected_.tick_tracebacks[handler_context.trace] = {1, dd};
        else
            trace_data[1] = trace_data[1]+1;
            trace_data[2] = trace_data[2]+dd;
        end
    end
	return ret;
end

function handleEventClose(event_handler)
	local handler_context = protected_.handler_contexts_[event_handler];
	if handler_context==nil then
		-- gameLog("handler_context is nil");
		gameLog("handler_context is nil "..tostring(event_handler));
		return;
	end
	if handler_context.handle_close then
		handler_context:handle_close();
	end
--    if gm_cmds.handlers[event_handler] ~= nil then
--        gm_cmds.handlers[event_handler] = nil
--    end
	protected_.closeHandlerContext(handler_context, true);
end

function protected_.closeHandlerContext(handler_context, not_close_call)
	if handler_context==nil then
		gameLog("protected_.closeHandlerContext is nil! "..debug.traceback());
	end
	if handler_context.game_entity_ then
		protected_.removeObjContext(handler_context.game_entity_, handler_context);
	end
    if not_close_call==nil then
	    _closeEventHandler(handler_context.event_handler_);
    end
	protected_.handler_contexts_[handler_context.event_handler_] = nil;
end

function protected_.closeHandlerEntity(game_entity)
	local event_contexts = protected_.obj_contexts_[game_entity];
	if event_contexts == nil then
		return;
	end
	for k in pairs(event_contexts) do
		protected_.closeHandlerContext(k);
	end
end
protected_.registMetaProp(protected_.EventContextMeta, "game_event_");
protected_.registMetaProp(protected_.EventContextMeta, "game_entity_");
protected_.registMetaProp(protected_.EventContextMeta, "context_");
protected_.registMetaProp(protected_.EventContextMeta, "handle_func");
protected_.registMetaProp(protected_.EventContextMeta, "close_func");

protected_.EventContextMeta.handle_event = function (self)
	if self.handle_func==nil then
		return -1;
	end
	return self.handle_func(self);
end;
protected_.EventContextMeta.handle_close = function (self)
	if self.close_func==nil then
		return 0;
	end
	return self.close_func(self);
end;
protected_.EventContextMeta.closeHandler = function (self)
	_closeEventHandler(self.event_handler_);
end;
protected_.tick_tracebacks = {}
function protected_.createHandlerContext(game_event,game_entity,handle_even_func,handle_close_func,context)

--    local handler_context = {
--		game_event_ = game_event,	--对于事件为{event_type, key0, key1}对于超时为expire_time (msec)
--		game_entity_ = game_entity,
--		context_ = context,
--		handle_event = function (self)
--			if handle_even_func==nil then
--				return -1;
--			end
--			return handle_even_func(self);
--		end,
--		handle_close = function (self)
--			if handle_close_func==nil then
--				return 0;
--			end
--			return handle_close_func(self);
--		end,
--		closeHandler = function (self)
--			closeEventHandler(self.handler_);
--		end,
--	};

    local handler_context = {};
    setmetatable(handler_context, protected_.EventContextMeta);
    handler_context.game_event_ = game_event;
    handler_context.game_entity_ = game_entity;
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

    --cclog2(handler_context, "DDDDDDDDDDDDDDDDDDDDD ");
	if game_entity then
		local event_contexts = protected_.obj_contexts_[game_entity];
		if event_contexts==nil then
			event_contexts={};
			protected_.obj_contexts_[game_entity] = event_contexts;
		end
		event_contexts[handler_context] = 1;
	end
	return handler_context;
end

function regEventHandler(game_event,game_entity,handle_even_func,handle_close_func,context)
	local handler_context = protected_.createHandlerContext(game_event,game_entity,handle_even_func,handle_close_func,context);
	handler_context.event_handler_ = _regEventHandler(game_event[1],game_event[2],game_event[3]);
	if handler_context.event_handler_==nil then
		return nil;
	end
	protected_.handler_contexts_[handler_context.event_handler_]=handler_context;
	return handler_context;
end
-- msec
function regExpireHandler(expire_time,game_entity,handle_even_func,handle_close_func,context)
	local handler_context = protected_.createHandlerContext(expire_time,game_entity,handle_even_func,handle_close_func,context);
	handler_context.event_handler_ = _regExpireHandler(expire_time);
	if handler_context.event_handler_==nil then
		return nil;
	end
	protected_.handler_contexts_[handler_context.event_handler_]=handler_context;
	return handler_context;
end

function raiseEvent(game_event)
	_raiseEvent(game_event[1], game_event[2], game_event[3]);
end


function protected_.showEventTrace(flag)
    if flag~=0 then
        for k,v in pairs(protected_.tick_tracebacks) do
            if v[1]>10 or v[2]>80 then
                gameLog("showEventTrace ["..k.."]=>"..v[1]..", "..v[2]);
            end
        end
    end
    protected_.tick_tracebacks={}
end