--[[********************************************************************
	created:	2018/01/19
	author:		lixuguang_cx

	purpose:	Lua层的枚举定义
*********************************************************************--]]
function beginEnum()
	enum_v_=0;
	return enum_v_;
end
function defineEnum()
	enum_v_ = enum_v_+1;
	return enum_v_;
end

Enum.EHeroUseType_None = beginEnum();
Enum.EHeroUseType_Permanent = defineEnum();	--永久有效
Enum.EHeroUseType_TimeLimit = defineEnum();	--时间限制
Enum.EHeroUseType_Max = defineEnum();

Enum.ELog_Error = 0     
Enum.ELog_Warning = 1   
Enum.ELog_Info = 2      
Enum.ELog_Debug = 3     
Enum.ELog_Log = 4		
Enum.ELog_Verbose = 5    
	  
Enum.EQuestEvent_None = beginEnum();
Enum.EQuestEvent_Level = beginEnum();
Enum.EQuestEvent_CommonBattle = defineEnum();       --参与一场战斗                
Enum.EQuestEvent_CommonBattleWin = defineEnum();    --赢得一场战斗                
Enum.EQuestEvent_InviteFriend = defineEnum();       --向一名玩家发送好友邀请       --
Enum.EQuestEvent_Beckon = defineEnum();             --在一场战斗中使用召唤
Enum.EQuestEvent_Revive = defineEnum();             --使用药品复活队友
Enum.EQuestEvent_Damage = defineEnum();             --单场战斗伤害达到5000点
Enum.EQuestEvent_ReceiveDamage = defineEnum();      --单场战斗承受3000点伤害
Enum.EQuestEvent_Cure = defineEnum();               --战斗中治疗
Enum.EQuestEvent_Seal = defineEnum();               --战斗中封印对方
Enum.EQuestEvent_SellFood = defineEnum();           --出售美食                      
Enum.EQuestEvent_BuyFood = defineEnum();            --购买美食                      
Enum.EQuestEvent_DrawCard = defineEnum();           --抽取神兽                      
Enum.EQuestEvent_Disassemble = defineEnum();        --拆解神兽并获得技能书          
Enum.EQuestEvent_UltimateSkill = defineEnum();      --释放怒气技
Enum.EQuestEvent_QualifyBattle = defineEnum();      --参与排位赛                    
Enum.EQuestEvent_QualifyBattleWin = defineEnum();   --赢得排位赛                    
Enum.EQuestEvent_DrawDiamond = defineEnum();        --钻石抽卡
Enum.EQuestEvent_DrawTicket = defineEnum();         --兑换券抽卡
Enum.EQuestEvent_BuyHero = defineEnum();            --购买英雄                      
Enum.EQuestEvent_BuySkin = defineEnum();            --购买皮肤                      
Enum.EQuestEvent_KillMythical = defineEnum();       --杀死神兽次数
Enum.EQuestEvent_CuredByMythical = defineEnum();    --被神兽药品复活
Enum.EQuestEvent_ShareGame = defineEnum();          --分享游戏                      --
Enum.EQuestEvent_InviteFriendBack = defineEnum();   --邀请好友回归                  --
Enum.EQuestEvent_KillCounts = defineEnum();         --战斗中打死对方


Enum.ItemType_None = beginEnum();
Enum.ItemType_Item = defineEnum(); --以下其他道具
Enum.ItemType_Skill = defineEnum(); --技能书
Enum.ItemType_Posy = defineEnum(); --美食 铭文
Enum.ItemType_Card = defineEnum(); --体验卡
Enum.ItemType_Gift = defineEnum(); --礼包
Enum.ItemType_Skin = defineEnum(); --皮肤

Enum.EFightType_None = beginEnum();
Enum.EFightType_Random = defineEnum();	--随机匹配
Enum.EFightType_Ranking = defineEnum();	--排位

--Enum.EMetaProp_None = beginEnum();				--
--Enum.EMetaProp_Name = defineEnum();				--名字
--Enum.EMetaProp_Default = defineEnum(); 			--默认值
--Enum.EMetaProp_SynMode = defineEnum();			--同步模式
--Enum.EMetaProp_Class = defineEnum();			--属性的meta类型
-- Enum.ESkill_None = beginEnum();
--Enum.ESkill_Default = defineEnum();
--Enum.ESkill_1 = defineEnum();
--Enum.ESkill_2 = defineEnum();
--Enum.ESkill_3 = defineEnum();
--Enum.ESkill_ep = defineEnum();
--Enum.ESkill_passive_1 = defineEnum();
--Enum.ESkill_passive_2 = defineEnum();
--Enum.ESkill_passive_3 = defineEnum();
--Enum.ESkill_passive_4 = defineEnum();
--Enum.ESkill_passive_5 = defineEnum();
--Enum.ESkill_friend = defineEnum();
--Enum.ESkill_Max = defineEnum();

Enum.ERank_NONE = beginEnum()
Enum.ERank_QualifyingScore = defineEnum()       --排位赛星星积分

Enum.ETroopAcceptType_NO = beginEnum()  --拒绝组队邀请
Enum.ETroopAcceptType_ONHOOK = defineEnum() --挂机
Enum.ETroopAcceptType_YES = defineEnum()    --同意并直接进组进入准备状态

Enum.ERewardRate_None = beginEnum();
Enum.ERewardRate_Time = defineEnum();	--时间限制
Enum.ERewardRate_Number = defineEnum();	--次数限制

Enum.ESeasonRewardCond_None = beginEnum(); --赛季奖励条件
Enum.ESeasonRewardCond_Dan = defineEnum();	--段位
Enum.ESeasonRewardCond_Win = defineEnum();	--奖励
Enum.ESeasonRewardCond_DanStep = defineEnum();	--阶梯段位

Enum.QuestType_None = beginEnum()           --任务
Enum.QuestType_EveryDay = defineEnum()      --每日任务
Enum.QuestType_MainLine = defineEnum()      --主线
Enum.QuestType_Grow = defineEnum()          --成长


Enum.QuestCountLimit_Grow = 2               --成长类型任务同时最多只能接到两个

Enum.QuestGrowStartId = 2                   --成长任务起始id

Enum.ESkinUseType_None = beginEnum();
Enum.ESkinUseType_Permanent = defineEnum();	--永久有效
Enum.ESkinUseType_TimeLimit = defineEnum();	--时间限制
Enum.ESkinUseType_Max = defineEnum();

Enum.ETroop_Create = 0;--开始组队
Enum.ETroop_Math = 1;--进入math队列
Enum.ETroop_Choose = 2;--匹配完成开始选英雄
Enum.ETroop_WaitFight = 3;
Enum.ETroop_Fight = 4;--开始战斗