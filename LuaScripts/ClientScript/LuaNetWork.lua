--[[********************************************************************
	created:	2018-1-29
	author:		suxin
	purpose:	lua网络层封装，这里要注意luasock接收数据是以'\n'来表示数据结束的
                如果没有接收到会超时，如果用luasocket的话需要在服务器发包最后添加'\n'
*********************************************************************--]]

local socket = socket;

if protected_.NetWork==nil  then
	print("protected_.NetWork reset");
	protected_.NetWork = {
		__index = protected_.index_,
		__newindex = protected_.newindex_,
		connect_id_gen_ = 0,
		waiting_close_conn_ = {},
		recv_stms_ = {},
		send_stms_={},
		tcp_clients_ = {},
		tcp_stms_ = {},
	};
end
protected_.NetWork.removeSendStm = function(self, stm)
	for i,v in ipairs(self.send_stms_) do
		if v==stm then
			table.remove(self.send_stms_, i);
			return;
		end
	end
end

protected_.NetWork.last_ping_time_ = 0;
protected_.NetWork.update_run = function(self)
	local cur_time = socket.gettime()*1000;
	local net_time_1 = socket.gettime()*1000;
	--LuaProfiler.BeginSample(LuaProfiler.GetID("update_run"))
	local recv, send, err = socket.select(self.recv_stms_, self.send_stms_, 0);
	--LuaProfiler.EndSample()
	local net_time_2 = socket.gettime()*1000;
	if net_time_2-net_time_1>100 then
		print("protected_.NetWork.update_run select", net_time_2-net_time_1);
	end
    for _, stm in ipairs(recv) do
		self:readMessage(self.tcp_stms_[stm]);
    end
	-- local net_time_3 = socket.gettime()*1000;
	-- if net_time_3-net_time_2>10 then
	-- 	print("protected_.NetWork.update_run read", net_time_3-net_time_2);
	-- end
	if cur_time-protected_.NetWork.last_ping_time_>5000 then
		protected_.NetWork.last_ping_time_ = cur_time;
		for k,v in pairs (protected_.clients_) do
			self:send_ping(k);
		end
	end
    for _,stm in ipairs(send) do
		self:writeMessage(self.tcp_stms_[stm]);
    end
	-- local net_time_4 = socket.gettime()*1000;
	-- if net_time_4-net_time_3>10 then
	-- 	print("protected_.NetWork.update_run send", net_time_4-net_time_3);
	-- end
    for i, connect_id in ipairs(self.waiting_close_conn_) do
		self:close_client(connect_id);
    end
	-- local net_time_5 = socket.gettime()*1000;
	-- if net_time_5-net_time_4>10 then
	-- 	print("protected_.NetWork.update_run close", net_time_5-net_time_4);
	-- end
	-- if net_time_5-net_time_1>10 then
	-- 	print("##################protected_.NetWork.update_run all", net_time_5-net_time_1);
	-- end
    if #self.waiting_close_conn_ > 0 then
        self.waiting_close_conn_ = {};
    end
end

protected_.NetWork.initNetWork = function(self)
    print("init network");
end

protected_.NetWork.closeNetWork = function(self)
   for i, client in pairs(self.tcp_clients_) do
	self:close_client(i);
   end
   self.tcp_stms = nil;
   self.tcp_stms = {};
end

protected_.NetWork.genConnectID = function(self)
     self.connect_id_gen_ = self.connect_id_gen_ + 1;
     return self.connect_id_gen_;
end

protected_.NetWork.createTcpClient = function(self)
	local tcp_client = {};
	tcp_client.send_queue_ = {};
	tcp_client.send_buff_ = "";
	tcp_client.connect_id_ = 0;
	tcp_client.recv_buff_="";
	tcp_client.recv_left_=0;
	tcp_client.msg_len_ = 0;
	tcp_client.conn_state_ = -1;
	tcp_client.send_flag_ = 0;
	tcp_client.stm_ = socket.tcp();
	self.tcp_stms_[tcp_client.stm_] = tcp_client;
	return tcp_client;
end
protected_.NetWork.getTcpClientByStm = function(self, stm)
	return self.tcp_stms_[stm];
end
protected_.NetWork.connect_server = function(self, connect_id, server_ip, server_port)
    connect_id = connect_id or self:genConnectID();
    if self.tcp_clients_[connect_id] ~= nil then
        print("the connect_id:".. connect_id .. " has connected!");
        return 1;
    end
	local tcp_client = self:createTcpClient();
	if tcp_client==nil then
		gameLog("createTcpClient failed!!");
		return -1;
	end
	local ok, err = tcp_client.stm_:connect(server_ip, server_port);
    if not ok then
        print("can not connect to ".. server_ip ..":"..server_port);
		self:close_client_imp(tcp_client);
        return -1;
    end 
	tcp_client.connect_id_ = connect_id;
	tcp_client.stm_:setoption('keepalive', true);
	tcp_client.stm_:settimeout(0);
	tcp_client.conn_state_ = 1;
    self.tcp_clients_[connect_id] = tcp_client;
	table.insert(self.recv_stms_, tcp_client.stm_);
    protected_.on_connect(connect_id);
	return 0;
end

protected_.NetWork.disconnect_server = function(self, connect_id)
	--print("disconnect_server: ", connect_id, debug.traceback());
    local tcp_client = self.tcp_clients_[connect_id];
    print(tcp_client);
    if tcp_client == nil then
        return;
    end
	local stm = tcp_client.stm_;
	if tcp_client.conn_state_>0 then
		for i,v in ipairs(self.recv_stms_) do
			if v==stm then
				table.remove(self.recv_stms_,i);
				break;
			end
		end
		stm:shutdown("both");
	end
	tcp_client.conn_state_ = -1;
    table.insert(self.waiting_close_conn_, connect_id);
end

protected_.NetWork.close_client_imp = function(self, tcp_client)
	--print("close_client_imp: ", connect_id, debug.traceback());
	local stm = tcp_client.stm_;
	if tcp_client.conn_state_>0 then
		for i,v in ipairs(self.recv_stms_) do
			if v==stm then
				table.remove(self.recv_stms_,i);
				break;
			end
		end
		stm:shutdown("both");
	end
	tcp_client.conn_state_ = -1;
	stm:close();
	self.tcp_stms_[stm] = nil;
	if tcp_client.connect_id_>0 then
		self.tcp_clients_[tcp_client.connect_id_] = nil;
	end
	protected_.destroyClient(tcp_client.connect_id_);
end

protected_.NetWork.close_client = function(self, connect_id)
	local tcp_client = self.tcp_clients_[connect_id];
    if tcp_client == nil then
        return;
    end
	client_recall_("onClientClose", tcp_client.connect_id_);
	self:close_client_imp(tcp_client);
end

protected_.NetWork.disconnect = function(self, tcp_client)
	self:disconnect_server(tcp_client.connect_id_);
end

protected_.NetWork.send_ping = function(self, client_id)
	protected_.NetWork.send_message(self, client_id, string.char(0, 0, 0, 0));
end

protected_.NetWork.send_message = function(self, client_id, smsg_str)
   
    local tcp_client = self.tcp_clients_[client_id];
    if tcp_client == nil then
        return;
    end
    if tcp_client.send_queue_ == nil then
        return;
    end
	if tcp_client.send_flag_==0 then
		table.insert(self.send_stms_, tcp_client.stm_);
	end
    table.insert(tcp_client.send_queue_, smsg_str);
end

protected_.NetWork.getsocketid = function(self, tcp_client)
    return tcp_client.connect_id_;
end

protected_.NetWork.writeMessage = function(self, tcp_client)
	tcp_client.send_flag_=0;
	local stm = tcp_client.stm_;
	if tcp_client.conn_state_<=0 then
		self:removeSendStm(stm);
		return;
	end
	local send_len = #tcp_client.send_buff_;
	local msg = tcp_client.send_queue_[1];
	while msg and send_len<10240 do
		send_len = send_len + #msg;
		tcp_client.send_buff_ = tcp_client.send_buff_..msg;
		table.remove(tcp_client.send_queue_, 1);
		msg = tcp_client.send_queue_[1];
	end
	if send_len==0 then
		self:removeSendStm(stm);
		return;
	end
	local slen, err = stm:send(tcp_client.send_buff_, 1, send_len);
	if err and err=="closed" then
		print(err);
        print("send err : connection will be closed");
        self:disconnect(tcp_client);
		return;
	end
	tcp_client.send_flag_=1;
	if slen>=send_len then
		tcp_client.send_buff_="";
	else
		tcp_client.send_buff_ = string.sub(tcp_client.send_buff_, slen+1, send_len-slen);
	end
end

protected_.NetWork.readMessage = function(self, tcp_client)
	if tcp_client.conn_state_<=0 then
		return;
	end
	if tcp_client.recv_left_==0 then
		tcp_client.recv_left_ = 4;
	end
	local stm = tcp_client.stm_;
	local rd_data, err;
	local net_rd_time_0 = socket.gettime()*1000;
	protected_.handle_protos={};
	repeat
		local rd_len = tcp_client.recv_left_;
--		if rd_len>1024*5 then
--			rd_len = 1024*5;
--		end
		rd_data, err, part_data = stm:receive(rd_len);
		if rd_data==nil then
			rd_data = part_data;
			--print("############stm:receive part: ", #part_data, "/", rd_len);
		end
		if rd_data then
--			if tcp_client.msg_len_>10000 then
--				print("################receive ", #rd_data);
--				gameLog("####receive:"..(#rd_data).." ####");
--				for i=1,#rd_data do
--					gameLog(string.byte(rd_data, i));
--				end
--				gameLog("####receive end###########");
--			end
			tcp_client.recv_left_ = tcp_client.recv_left_-#rd_data;
			if tcp_client.recv_buff_==nil then
				tcp_client.recv_buff_ = rd_data;
			else
				tcp_client.recv_buff_ = tcp_client.recv_buff_..rd_data;
			end
			if tcp_client.recv_left_==0 then
				local buffer = tcp_client.recv_buff_;
				if #buffer==4 then
					local body_len = 0;
					body_len = body_len * 0x100 + string.byte(buffer, 1);
					body_len = body_len * 0x100 + string.byte(buffer, 2);
					body_len = body_len * 0x100 + string.byte(buffer, 3);
					body_len = body_len * 0x100 + string.byte(buffer, 4);
					--if body_len>10000 then
						--gameError("Read big message: "..body_len);
					--end
					tcp_client.recv_left_ = body_len;
					tcp_client.msg_len_ = body_len;
					if body_len==0 then
						tcp_client.recv_buff_ = "";
						tcp_client.msg_len_ = 0;
						tcp_client.recv_left_ = 4;
					end
				else
					--print("##readMessage handlePacket: ", tcp_client.msg_len_);
					protected_.handlePacket(protected_.msg_handlers_, tcp_client.connect_id_, string.sub(buffer, 5, tcp_client.msg_len_+4));
					tcp_client.recv_buff_ = "";
					tcp_client.msg_len_ = 0;
					tcp_client.recv_left_ = 4;
				end
			end
		end
		if err~=nil then
			if err~="timeout" then
				gameLog(string.format("recv err[%d]: %s; connection will be closed!!", tcp_client.connect_id_, err));
				self:disconnect(tcp_client);
				return ;
			else
				--print("##readMessage recv timeout");
			end
		end
	until(err);
	local net_rd_time_1 = socket.gettime()*1000;
	if net_rd_time_1-net_rd_time_0>5 then
		--print("protected_.NetWork.readMessage", net_rd_time_1-net_rd_time_0, dump_obj(protected_.handle_protos));
	end
end



