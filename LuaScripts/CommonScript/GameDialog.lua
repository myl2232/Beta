--[[********************************************************************
	created:	2018/10/09
	author:		lixuguang_cx

	purpose:	对话相关，本脚本被FightServer,GameServer以及Client公用
	需要实现接口：protected_.getPlayer, player:getUID, protected_.handleDialog, protected_.setDialogContext, protected_.handleDialogClose
*********************************************************************--]]
protected_.registMetaProp(protected_.MetaDialog, "player_");
protected_.registMetaProp(protected_.MetaDialog, "unit_id_", 0);
protected_.registMetaProp(protected_.MetaDialog, "context_", {});
protected_.registMetaProp(protected_.MetaDialog, "field_id_", 0);
protected_.registMetaProp(protected_.MetaDialog, "dialog_sid_", 0);

protected_.MetaDialog.open = function(self, player, dialog_sid, context, field_id, unit_id)
	if player.dialog_ then
		return ;
	end
	self.player_ = player;
	player.dialog_ = self;
	self.dialog_sid_ = dialog_sid;
	self.context_ = context;
	self.field_id_ = field_id or 0;
	self.unit_id_ = unit_id or 0;
	if self.field_id_>0 and holdToken then
		holdToken(self.field_id_, EHoldToken_Dialog);
	end
end
protected_.MetaDialog.close = function(self)
	local dialog_data = configs_.Dialog[self.dialog_sid_];
	if dialog_data==nil then
		return;
	end
	if dialog_data.save_flag_ and dialog_data.save_flag_>0 then
		local val = self.player_.dialog_contexts_[self.dialog_sid_] or 0;
		val = val+1;
		self.player_.dialog_contexts_[self.dialog_sid_] = val;
		protected_.setDialogContext(self.player_:getUID(), self.dialog_sid_, val);--以GameServer为中心，其他节点不需要同步，如果需要同步另加同步协议
		if self.dialog_sid_<=configs_.guide_dialog_end_ then
			self.player_.guide_dialog_ = self.dialog_sid_;
		end
	end
	if protected_.handleDialogClose then
		protected_.handleDialogClose(self.player_:getUID(), self.dialog_sid_);
	end
	if self.field_id_>0 and releaseToken then
		releaseToken(self.field_id_, EHoldToken_Dialog);
	end
	if self.player_.dialog_==self then
		self.player_.dialog_ = nil;
	end
end

local isNotWaitClient = function(dialog_data)
	return dialog_data.type_==EDialogType_Script or dialog_data.type_==EDialogType_ChangeLevel or dialog_data.type_==EDialogType_SpeakShow;
end

protected_.MetaDialog.nextDialog = function(self, dialog_data)
	local player = self.player_;
	if player==nil then
		return nil;
	end
	if dialog_data.next_==nil or dialog_data.next_==0 then
		return nil;
	end
	local next_dialog_id = dialog_data.next_;
	local next_dialog_data = configs_.Dialog[next_dialog_id];
	if next_dialog_data==nil then
		return nil;
	end
	local dialog = protected_.constructObject(protected_.MetaDialog);
	player.dialog_ = nil;
	dialog:open(player, next_dialog_id, self.context_, self.field_id_, self.unit_id_);
	return dialog;
end
protected_.MetaDialog.dialogCalls={}
protected_.MetaDialog.dialogHandlers = {};
protected_.MetaDialog.dialogHandlers[EDialogType_Script] = function(self, dialog_data)
	local func_name = dialog_data.content_;
	if type(func_name)=='table' then
		func_name = func_name[1];
	end
	local func = configs_.DialogScript[func_name];
	if func==nil then
		return -1;
	end
	return func(dialog_data, self);
end
protected_.MetaDialog.dialogHandlers[EDialogType_ScriptWait] = protected_.MetaDialog.dialogHandlers[EDialogType_Script];

protected_.MetaDialog.doDialog = function(dialog)
	local player = dialog.player_;
	if player==nil then
		protected_.MetaDialog.close(dialog);
		return 0;
	end
	local dialog_data = configs_.Dialog[dialog.dialog_sid_];
	if dialog_data==nil then
		protected_.MetaDialog.close(dialog);
		return 0;
	else
		local handler = protected_.MetaDialog.dialogHandlers[dialog_data.type_];
		if handler then
			handler(dialog, dialog_data);
		else
			--sendPlayer
			protected_.handleDialog(player:getUID(), {ES2CProtocol["ES2CProtocol_DialogOpen"], dialog.dialog_sid_, dialog.context_, dialog.field_id_, dialog.unit_id_});
		end
		if isNotWaitClient(dialog_data) then
			local next_dialog = protected_.MetaDialog.nextDialog(dialog, dialog_data);
			protected_.MetaDialog.close(dialog);
			if next_dialog then
				return protected_.MetaDialog.doDialog(next_dialog);
			end
		else
			--等待客户端关闭或者等待脚本关闭
		end
		return 0;
	end
end
protected_.MetaDialog.doPlayerDialog = function (player, dialog_sid, dialog_context, field_id, unit_id)
	if player.dialog_ then
		return -1;
	end
	local dialog = protected_.constructObject(protected_.MetaDialog);
	dialog:open(player, dialog_sid, dialog_context, field_id, unit_id);
	return protected_.MetaDialog.doDialog(dialog);
end
protected_.MetaDialog.tryPlayerDialog = function (player_uid, dialog_sid, field_id, unit_id)
	local player = protected_.getPlayer(player_uid);
	if player==nil then
		return -1;
	end
	local dialog_data = configs_.Dialog[dialog_sid];
	if dialog_data==nil then
		return 0;
	end
	if player.dialog_contexts_ and dialog_data.save_flag_ then
		if player.dialog_contexts_[dialog_sid] and player.dialog_contexts_[dialog_sid]>=dialog_data.save_flag_ then
			return 0;
		end
	end
	
	return protected_.MetaDialog.doPlayerDialog(player, dialog_sid, dialog_context, field_id, unit_id);
end
protected_.MetaDialog.closePlayerDialog = function (player_uid)
	local member = protected_.getPlayer(player_uid);
	if member==nil then
		return 0;
	end
	local dialog = member.dialog_;
	if dialog==nil then
		return 0;
	end
	local dialog_data = configs_.Dialog[dialog.dialog_sid_];
	if dialog_data == nil then
		dialog:close();
		return 0;
	end
	local next_dialog = dialog:nextDialog(dialog_data);
	dialog:close();
	if next_dialog then
		next_dialog:doDialog();
	end
	return 0;
end