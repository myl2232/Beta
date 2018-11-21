protected_.registMetaProp(protected_.MetaTeamMember, "player_suid_", 0);    --玩家suid
protected_.registMetaProp(protected_.MetaTeamMember, "player_name_", "");   --玩家名字
protected_.registMetaProp(protected_.MetaTeamMember, "troop_pos_", 0);  --在部队中的位置（选人界面中的位置）
protected_.registMetaProp(protected_.MetaTeamMember, "score_", 0);  --玩家的分数
protected_.registMetaProp(protected_.MetaTeamMember, "server_id_", 0);  --玩家所在服务器id
protected_.registMetaProp(protected_.MetaTeamMember, "onhook_status_", 2);
protected_.registMetaProp(protected_.MetaTeamMember, "rank_score_",0);
protected_.registMetaProp(protected_.MetaTeamMember, "rematch_", 0);

protected_.registMetaProp(protected_.MetaTeamTroop, "id_", 0);
protected_.registMetaProp(protected_.MetaTeamTroop, "state_", 0);-- ETroop_Math 进入math队列 ETroop_Choose匹配完成开始选英雄 ETroop_Fight开始战斗
protected_.registMetaProp(protected_.MetaTeamTroop, "leader_", 0);  --队长
protected_.registMetaProp(protected_.MetaTeamTroop, "count_", 0);   --部队人数
protected_.registMetaProp(protected_.MetaTeamTroop, "fight_id_", 0);   --protected_.fight_id_gen_ 
protected_.registMetaProp(protected_.MetaTeamTroop, "count_", 0);
protected_.registMetaMap(protected_.MetaTeamTroop, 0, 1, protected_.MetaTeamMember);
protected_.registMetaProp(protected_.MetaTeamTroop, "fight_capacity_", 0);  --部队战斗力
protected_.registMetaProp(protected_.MetaTeamTroop, "average_score_", 0);  --平均隐藏分
protected_.registMetaProp(protected_.MetaTeamTroop, "dialog_contexts_", nil, nil, protected_.MetaMap);
protected_.registMetaProp(protected_.MetaTeamTroop, "marks_", nil, nil, protected_.MetaMap);
protected_.registMetaProp(protected_.MetaTeamTroop, "max_score_", 0);
protected_.registMetaProp(protected_.MetaTeamTroop, "dan_grading_", 0); --段位
protected_.registMetaProp(protected_.MetaTeamTroop, "max_rank_score_", 0);
protected_.registMetaProp(protected_.MetaTeamTroop, "fight_sid_",0);