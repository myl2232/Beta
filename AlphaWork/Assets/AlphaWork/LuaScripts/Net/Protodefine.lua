--[[
    收发协议字段定义
]]
_G.ProtoRecive=
{
    onConnect           = "onConnect",
    onReconnect         = "onReconnect",
    onDisconnect        = "onDisconnect",
    onCreateChar        = "onCreateChar",
    onCreateCharOk      = "onCreateCharOk",
    onUserdataOk        = "onUserdataOk",
    onEnterWorld        = "onEnterWorld",
    onTroopInit         = "onTroopInit",
    onTroopInvite       = "onTroopInvite",
    onTroopAdd          = "onTroopAdd",
    onTroopQuit         = "onTroopQuit",
    onMemberLeave       = "onMemberLeave",
    onSetLeader         = "onSetLeader",
    onWaitMatch         = "onWaitMatch",
    onTroopMerge        = "onTroopMerge",
    onMatchFailed       = "onMatchFailed",
    onChooseBegin       = "onChooseBegin",
    onChooseHero        = "onChooseHero",
    onChooseHeroConfirm = "onChooseHeroConfirm",
    onChooseHeroCancel  = "onChooseHeroCancel",
    onConfirmPetTroop   = "onConfirmPetTroop",
    onChoosePet         = "onChoosePet",
    onSetFormation      = "onSetFormation",
    onSetMemPos         = "onSetMemPos",
    onConfirmFight      = "onConfirmFight",
    onCreateFight       = "onCreateFight",
    onPrepareRound      = "onPrepareRound",
    onFightBegin        = "onFightBegin",
    onReadyRoundBegin   = "onReadyRoundBegin",
    onRoundBegin        = "onRoundBegin",
    onReadyRoundEnd     = "onReadyRoundEnd",
    onRoundEnd          = "onRoundEnd",
    onUnitBegin         = "onUnitBegin",
    onSkillStateAdd     = "onSkillStateAdd",
    onSkillStateDel     = "onSkillStateDel",
    onUnitPropChange    = "onUnitPropChange",
    onAttackFailed      = "onAttackFailed",
    onAttackBegin       = "onAttackBegin",
    onDamage            = "onDamage",
    onAttackEnd         = "onAttackEnd",
    onDead              = "onDead",
    onSlotChange        = "onSlotChange",
    onUnitEnd           = "onUnitEnd",
    onFightOverData     = "onFightOverData",
    onThumbsUp          = "onThumbsUp",
    onFightResult       = "onFightResult",
    onFightEnd          = "onFightEnd",
    onSetAttack         = "onSetAttack",
    onAddUnit           = "onAddUnit",
    onDelUnit           = "onDelUnit",
    onClientClose       = "onClientClose",
    onConnectResult     = "onConnectResult",
    onReconnectResult   = "onReconnectResult",
    onDamages           = "onDamages",
    onAttackStages      = "onAttackStages",
    onGoBack            = "onGoBack",
    onUnitShift         = "onUnitShift",
    onRevive            = "onRevive",
    onSummonPet         = "onSummonPet",
    onBuyHero           = "onBuyHero",
    onSwapSlot          = "onSwapSlot",
    onSellPet           = "onSellPet",
    onPetSlot           = "petTroopChange",
    onCommander         = "onCommander",
    onMarkUnit          = "onMarkUnit",
    onHint              = "onHint",
    onProProtect        = "onProProtect",
    onServerTimer       = "onServerTimer",
    onMoneyChange       = "onMoneyChange",
    onAttackerQTE       = "onAttackerQTE",
    onTargetQTE         = "onTargetQTE",
    --聊天
    onJoinChannel       = "onJoinChannel",
    onLeaveChannel      = "onLeaveChannel",
    onChatInChannel     = "onChatInChannel",
    onChatInTroop       = "onChatInTroop",
    onChatToPlayer      = "onChatToPlayer",
    onChatToRoom        = "onChatToRoom",
    onStateFailed       = "onStateFailed",

    --背包
    onUseItem           = "onUseItem",    
    onSellItem          = "onSellItem",
    onRefreshProp       = "onRefreshProp",
    onReplaceProp       = "onReplaceProp",   
    onLearnSkill        = "onLearnSkill",
    onResolvePet        = "onResolvePet",
    onSellItemBySid     = "onSellItemBySid",
    onBuyItem           = "onBuyItem",
    onItemChange        = "onItemChange",
    
    --选择法宝
    onChooseTrump       = "onChooseTrump",
    onUnitOrder         = "onUnitOrder",

    --美食
    onInstallPosy       = "onInstallPosy",
    onUninstallPosy     = "onUninstallPosy",
    onUninstallAllPosy  = "onUninstallAllPosy",
    onUnlockProgram     = "onUnlockProgram",
    onProgramSetName    = "onProgramSetName",

    onLottery           = "onLottery",
    --组队
    onChooseTroopReadyStatus = "onChooseTroopReadyStatus",
    onKickOffFromTroop  = "onKickOffFromTroop",
    onTroopUseDialogue  = "onTroopUseDialogue",
    --排行榜
    onRequestRankInfo   = "onRequestRankInfo",
    --引导
    onDialogOpen        = "onDialogOpen",
    onDialogClose       = "onDialogClose",
    --获得奖励
    onFetchSeasonReward = "onFetchSeasonReward",
    --匹配成功
    onWaitIntoMatch     = "onWaitIntoMatch",
    --匹配成功准备广播
    onReadyToMatch      = "onReadyToMatch",
    --匹配超时
    onFailedReady       = "onFailedReady",
    --停止匹配
    onStopMatch         = "onStopMatch",
    --选择铭文
    onConfirmPosyProgram= "onConfirmPosyProgram", 
    --增加等级
    onUpgrade           = "onUpgrade",
    --成长任务状态
    onQuestStateChange  = "onQuestStateChange",
    --等级奖励领取成功
    onGetLevelReward    = "onGetLevelReward",
    onInitQuestInfo     = "onInitQuestInfo",
    onUnlockPetTroop    = "onUnlockPetTroop",
}

_G.ProtoSend =
{
    disConnect          = "discon",
    connect             = "connect",
    login               = "login",
    createPlayer        = "createPlayer",
    enterWorld          = "enterWorld",
    chooseHero          = "chooseHero",
    selectPetTroop      = "selectPetTroop",
    choosePet           = "choosePet",
    setMemPos           = "setMemPos",
    beginFight          = "beginFight",
    buyhero             = "buyHero",
    sellpet             = "sellPet",
    chatInChannel       = "chatInChannel",
    chatInTroop         = "chatInTroop",
    chatToPlayer        = "chatToPlayer",
    joinChannel         = "joinChannel",

	useItem             = "useItem",
    sellItem            = "sellItem",
    refreshProp         = "refreshProp",
    replaceProp         = "replaceProp",
    learnskill          = "learnSkill",
    resolvePet          = "resolvePet",
    sellItemBySid       = "sellItemBySid",
    buyItem             = "buyItem",

    chooseTrump         = "chooseTrump",

    installPosy         = "installPosy",
    uninstallPosy       = "uninstallPosy",
    uninstallAllPosy    = "uninstallAllPosy",
    unlockProgram       = "unlockProgram",
    ProgramSetName      = "ProgramSetName",

    lottery             = "lottery",

    chooseTroopReadyStatus = "chooseTroopReadyStatus",
    kickOffFromTroop    = "kickOffFromTroop",
    troopUseDialogue    = "troopUseDialogue",
    requestRankInfo     = "requestRankInfo",

    fetchSeasonReward   = "fetchSeasonReward",   --赛季奖励领奖
    confirmDialog       = "confirmDialog",

    readyMatch          = "readyMatch",          --匹配准备
    stopMatch           = "stopMatch",           --停止匹配
    rankmatchTroop      = "rankmatchTroop",      --排位匹配

    selectPosyProgram   = "selectPosyProgram",
    getLevelReward      = "getLevelReward", --领取等级奖励
    getQuestReward      = "getQuestReward",--领取成长等级奖励

    petTroopSetName     = "petTroopSetName",--修改神兽阵容名称
    unlockPetTroop      = "unlockPetTroop", --解锁神兽阵容
}
--[[
    收发协议字段定义结束
]]
