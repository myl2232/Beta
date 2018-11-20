-- ·¨±¦
protected_.registMetaProp(protected_.MetaTrump, "id_", 0); --·¨±¦id
protected_.registMetaProp(protected_.MetaTrump, "lock_", 0); --½âËø×´Ì¬
-----------------------------------------------------------------
protected_.registMetaProp(protected_.MetaTrumpBag, "trump_num_", 0);
protected_.registMetaMap(protected_.MetaTrumpBag, 0, nil, protected_.MetaTrump)

--------------------------------------------------------------------------
protected_.MetaTrumpBag.getTrumpsData = function()
    return configs_.TrumpCfg;
end

protected_.MetaTrumpBag.getTrumpData = function (trump_sid)
    local trump = protected_.MetaTrumpBag.getTrumpsData()[trump_sid];
    if not item_data then
     log.error("MetaTrumpBag.getTrumpData", nil, "trump is nil item_sid=", trump_sid);
        print(debug.traceback());
        return ;
    end
    return trump;
end