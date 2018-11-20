
protected_.registMetaProp(protected_.MetaFight, "id_", 0);      --protected_.fight_id_gen_
protected_.registMetaProp(protected_.MetaFight, "fight_sid_", 0);   --Troop.fight_sid_  1v1 2v2等匹配模式
protected_.registMetaProp(protected_.MetaFight, "state_", 0);   -- 0未开始战斗， 1 开始加载战斗
protected_.registMetaProp(protected_.MetaFight, "worker_", 0);  --使用过的worker线程处理战斗
protected_.registMetaProp(protected_.MetaFight, "troop1_", 0);   --战斗部队1 id
protected_.registMetaProp(protected_.MetaFight, "troop2_", 0);  --战斗部队2 id
protected_.registMetaProp(protected_.MetaFight, "fight_type_", 0);  --战斗类型， 匹配，排位
--protected_.registMetaProp(protected_.MetaFight, "stage_", 1);  --战斗阶段
--protected_.registMetaProp(protected_.MetaFight, "counters_", {});  --计数器

-- client
-- my_corp_id_
