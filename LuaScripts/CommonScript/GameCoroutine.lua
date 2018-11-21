
protected_.GameCoroutine.createCoroutine=function (id, weight, func, arg)
    gameAssert(protected_.coroutines_[id]==nil, "");
    local co = coroutine.create(protected_.GameCoroutine.run);
    protected_.coroutines_[id]={co, weight or 1, func, arg};
end
protected_.GameCoroutine.closeCoroutine=function (id)
    protected_.coroutines_[id]=nil;
end
protected_.GameCoroutine.resumeAll=function (weight_rate)
    weight_rate = weight_rate or 1;
    for k,v in pairs(protected_.coroutines_) do
        protected_.co_weight_ = math.floor(v[2]*weight_rate);
        coroutine.resume(v[1], v[3], v[4]);
    end
end
protected_.GameCoroutine.closeAll=function ()
    for k,v in pairs(protected_.coroutines_) do
        protected_.GameCoroutine.closeCoroutine(k);
    end
end
protected_.GameCoroutine.weightCall=function (w)
    w = w or 1;
    -- print("protected_.GameCoroutine.weightCall ", protected_.quit_flag_, protected_.co_weight_);
    if protected_.quit_flag_==0 and protected_.co_weight_<=0 then
        protected_.GameCoroutine[coroutine.running()]=nil;
        --gameLog("weightCall yield "..tostring(coroutine.running()));
        coroutine.yield();
    end
    --gameLog("weightCall "..protected_.co_weight_);
    protected_.co_weight_ = protected_.co_weight_-w;
end
protected_.GameCoroutine.run = function(func, ...)
    while true do
        --gameLog("while new begin "..tostring(coroutine.running()));
        protected_.GameCoroutine[coroutine.running()] = 1;
        func(...);
        if protected_.GameCoroutine[coroutine.running()] then
            protected_.GameCoroutine[coroutine.running()]=nil;
            --gameLog("while end yield "..tostring(coroutine.running()));
            coroutine.yield();
        end
    end
end
