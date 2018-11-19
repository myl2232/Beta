
protected_.registMetaProp(protected_.MetaGameScene, "server_id_", 0);  


protected_.MetaGameScene.createGameScene = function(server_id)
    local game_scene = protected_.constructObject(protected_.MetaGameScene, nil, protected_.game_scenes_, server_id);
    if game_scene == nil then
		return 0;
    end
	game_scene.server_id_ = server_id;
    protected_.initObject(game_scene);
    return 1;
end
protected_.MetaGameScene.loadGameScene = function (server_id, scene_msg)
	local r1, r2, r3 = pcall(protected_.mp_.game_unpack, scene_msg);
    if not r1 then
		gameLog("loadGameScene unpacket error!");
        return 1;
    end
	local game_scene = r2;
	local cur_i = r3;
	if cur_i < #scene_msg then
		gameLog("loadGameScene unpacket error!");
		return 2;
	end
	if protected_.game_scenes_[server_id] then
		print("XXXXXXXXXXXXXXXXXXXXXXXX ", server_id);
	end
	gameAssert(protected_.game_scenes_[server_id]==nil, "");
	log.debug("MetaGameScene.loadGameScene", nil, "loadGameScene ",  server_id);
	game_scene = protected_.constructObject(protected_.MetaGameScene, game_scene, protected_.game_scenes_, server_id);
	if game_scene==nil then
		return 3;
    end
    protected_.initObject(game_scene);
	log.debug("MetaGameScene.loadGameScene", nil, "server_id = ", server_id);
	return 0;
end