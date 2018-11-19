
protected_.member_pet_ids = {"pet1_","pet2_","pet3_"}
protected_.registMetaProp(protected_.MetaMember, "player_suid_", 0);    --玩家suid
protected_.registMetaProp(protected_.MetaMember, "player_name_", "");   --玩家名字
protected_.registMetaProp(protected_.MetaMember, "troop_pos_", 0);  --在部队中的位置（选人界面中的位置）
protected_.registMetaProp(protected_.MetaMember, "score_", 0);  --玩家的分数
protected_.registMetaProp(protected_.MetaMember, "server_id_", 0);  --玩家所在服务器id
protected_.registMetaProp(protected_.MetaMember, "hero_", 0);    --玩家选中的hero_id
protected_.registMetaProp(protected_.MetaMember, "skin_", 0);    --英雄的皮肤
protected_.registMetaProp(protected_.MetaMember, "pet_", 0);    --选中 宠物 索引 (选中的宠物部队的索引 1, 2, 3)
protected_.registMetaProp(protected_.MetaMember, "x_", 0);  
protected_.registMetaProp(protected_.MetaMember, "y_", 0);
protected_.registMetaProp(protected_.MetaMember, "pet_x_", 0);  
protected_.registMetaProp(protected_.MetaMember, "pet_y_", 0);
protected_.registMetaProp(protected_.MetaMember, "formations_", {});
protected_.registMetaProp(protected_.MetaMember, "pet1_", nil, nil, protected_.MetaPet);
protected_.registMetaProp(protected_.MetaMember, "pet2_", nil, nil, protected_.MetaPet);    
protected_.registMetaProp(protected_.MetaMember, "pet3_", nil, nil, protected_.MetaPet);
protected_.registMetaProp(protected_.MetaMember, "pet_troop_id_", 1);   --玩家选中神兽部队id
protected_.registMetaProp(protected_.MetaMember, "trump_bag_",nil, nil, protected_.MetaTrumpBag);
protected_.registMetaProp(protected_.MetaMember, "hero_trump_map_",nil, nil, protected_.MetaMap);
protected_.registMetaProp(protected_.MetaMember, "trump_", 0);
protected_.registMetaProp(protected_.MetaMember, "posy_program_",nil, nil, protected_.MetaPosyProgram); --铭文
protected_.registMetaProp(protected_.MetaMember, "dialog_contexts_", nil, nil, protected_.MetaMap);
protected_.registMetaProp(protected_.MetaMember, "onhook_status_", 2);
protected_.registMetaProp(protected_.MetaMember, "hero_pet_troop_",nil, nil, protected_.MetaMap);	--英雄选择神兽部队存储
protected_.registMetaProp(protected_.MetaMember, "ai_control_", 0);	--英雄选择神兽部队存储

protected_.registMetaProp(protected_.MetaTroop, "id_", 0);  -- protected_.troop_id_gen_
protected_.registMetaProp(protected_.MetaTroop, "fight_sid_", 0); -- 1v1 2v2等匹配模式
protected_.registMetaProp(protected_.MetaTroop, "state_", 0);-- ETroop_Math 进入math队列 ETroop_Choose匹配完成开始选英雄 ETroop_Fight开始战斗
protected_.registMetaProp(protected_.MetaTroop, "leader_", 0);  --队长
protected_.registMetaProp(protected_.MetaTroop, "formation_", 0);   --阵型
protected_.registMetaProp(protected_.MetaTroop, "worker_", 0);  --处理战斗的worker 开始战斗时才会赋值
protected_.registMetaProp(protected_.MetaTroop, "count_", 0);   --部队人数
protected_.registMetaProp(protected_.MetaTroop, "fight_id_", 0);   --protected_.fight_id_gen_ 
protected_.registMetaMap(protected_.MetaTroop, 0, 1, protected_.MetaMember)
protected_.registMetaProp(protected_.MetaTroop, "commander_", 0);   --部队的指挥者
protected_.registMetaProp(protected_.MetaTroop, "marks_", nil, nil, protected_.MetaMap);   --部队的标记记录 unit = 标记内容
protected_.registMetaProp(protected_.MetaTroop, "fight_capacity_", 0);  --部队战斗力
protected_.registMetaProp(protected_.MetaTroop, "average_score_", 0);  --平均隐藏分
protected_.registMetaProp(protected_.MetaTroop, "player_num_", 0);  
protected_.registMetaProp(protected_.MetaTroop, "ai_num_", 0);  

--[[fightServer]]
--hero_chooses_
--list_
--invites_

protected_.MetaMember.getUID = function(self)
	return self.player_suid_;
end