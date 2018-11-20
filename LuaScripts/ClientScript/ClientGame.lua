--[[********************************************************************
	created:	2018-1-30
	author:		suxin
	purpose:	lua客户端主逻辑
*********************************************************************]]--
protected_.registFightUpdater = function(client)
    protected_.fightUpdaters_[client.client_id_] = client;
end
protected_.unregistFightUpdater = function(client)
    protected_.fightUpdaters_[client.client_id_] = nil;
end