protected_.registMetaProp(protected_.MetaTeamMember, "player_suid_", 0);    --���suid
protected_.registMetaProp(protected_.MetaTeamMember, "player_name_", "");   --�������
protected_.registMetaProp(protected_.MetaTeamMember, "troop_pos_", 0);  --�ڲ����е�λ�ã�ѡ�˽����е�λ�ã�
protected_.registMetaProp(protected_.MetaTeamMember, "score_", 0);  --��ҵķ���
protected_.registMetaProp(protected_.MetaTeamMember, "server_id_", 0);  --������ڷ�����id
protected_.registMetaProp(protected_.MetaTeamMember, "onhook_status_", 2);
protected_.registMetaProp(protected_.MetaTeamMember, "rank_score_",0);
protected_.registMetaProp(protected_.MetaTeamMember, "rematch_", 0);

protected_.registMetaProp(protected_.MetaTeamTroop, "id_", 0);
protected_.registMetaProp(protected_.MetaTeamTroop, "state_", 0);-- ETroop_Math ����math���� ETroop_Chooseƥ����ɿ�ʼѡӢ�� ETroop_Fight��ʼս��
protected_.registMetaProp(protected_.MetaTeamTroop, "leader_", 0);  --�ӳ�
protected_.registMetaProp(protected_.MetaTeamTroop, "count_", 0);   --��������
protected_.registMetaProp(protected_.MetaTeamTroop, "fight_id_", 0);   --protected_.fight_id_gen_ 
protected_.registMetaProp(protected_.MetaTeamTroop, "count_", 0);
protected_.registMetaMap(protected_.MetaTeamTroop, 0, 1, protected_.MetaTeamMember);
protected_.registMetaProp(protected_.MetaTeamTroop, "fight_capacity_", 0);  --����ս����
protected_.registMetaProp(protected_.MetaTeamTroop, "average_score_", 0);  --ƽ�����ط�
protected_.registMetaProp(protected_.MetaTeamTroop, "dialog_contexts_", nil, nil, protected_.MetaMap);
protected_.registMetaProp(protected_.MetaTeamTroop, "marks_", nil, nil, protected_.MetaMap);
protected_.registMetaProp(protected_.MetaTeamTroop, "max_score_", 0);
protected_.registMetaProp(protected_.MetaTeamTroop, "dan_grading_", 0); --��λ
protected_.registMetaProp(protected_.MetaTeamTroop, "max_rank_score_", 0);
protected_.registMetaProp(protected_.MetaTeamTroop, "fight_sid_",0);