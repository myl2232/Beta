_G.EFuncType =
{
    Start = 0,--启动
    Login =1,--登陆
    GameHall =2,--大厅
    Pet =3,--宠物
    BattleMatch = 4,--匹配
    Tips = 5,
    SelectHero = 6,--选人
    HeroArray = 7,
    Loading = 8,--加载
    -- TempHead = 9,
    Settlement = 10,
    Combat = 11,--战斗界面
    UnitHead = 12,--血条
    HeroArchive = 20,
    HeroInfo = 21,
    SummonPet = 22,
    PetDetail = 23,
    BuyTips = 24,
    BuyDisplay = 25,
    SkillInfoPanel = 26,
    PetEdit = 27,
    Reward = 28,
    GetCoin = 29,
    BattleHeroInfo = 30,
    PetArchive = 31,
    CenterNotice = 32,
    Waiting = 33,
    Chat = 34,
    ChatMain = 35,
    FlashDialog =36,
    PetInfo =37,
    PlayerHeadInfo =38,
    MagicWeapon = 39,
    Money = 40,    
    PetTrain_SubPageMain = 41, --宠物培养分页签主面板
    Pet_LearnskillMain   = 42,--学习技能一级界面
    FULLSCREEN_OPR   = 43,--全屏带操作按钮的界面
    NORMAL_REWARD    = 44,--普通奖励界面
    MagicWeaponView = 45,
    SelectTrump     = 46,--选择法宝
    TrumpEffect     = 47,--法宝动画
    Bag = 48,--背包
    SelectionMode = 49,
    UIPet_Summon = 50,
    Posy = 51,
    Posy_Court = 52,
    Posy_Lottery = 53,
    Posy_Main = 54,
    UITeam_Match = 55,
    UITeamInvite = 56,
    UIMatchHang = 57,
    UITeaminvited = 58,
    UIRankingMatch = 59,
    UIRank = 60,
    UISegmentDisplay = 61,
    UISeasonReward = 62,
    Foreword       = 63, --序章
    UISelectHeroNew = 64, --战前选人
    Hero_Illustration = 65,--新.祀灵图鉴
    HeroInfo_Main = 66, --新.祀灵信息
    Hero_Display = 67, --新.祀灵展示

    UISingleHang = 68, --匹配等待
    Task_Main = 69, --三界之路 
    Task_Grow = 70, --个人成长（新手）
    UIPet_Illustration = 71, --神兽图鉴
    UIPetDetailNew = 72,     --神兽详情
}
-- 伤害类型
_G.DamageType = {
    Cure = 0,           -- 治疗
    Real = 1,           -- 真实
    Physical = 2,       -- 物理
    Spell = 3,          -- 法术
    Shield = 4,         -- 护盾
    Failed  = 100,      -- 未击中目标
}

-- 人物类型
_G.UnitType = {
    My_Hero = 0,            -- 自己英雄
    My_Pet = 1,             -- 自己宠物

    Friend_Hero = 2,        -- 友方英雄
    Friend_Pet = 3,         -- 友方宠物

    Enemy_Hero = 4,         -- 敌方英雄
    Enemy_Pet = 5,          -- 敌方宠物
}
--MsgBox Button类型
_G.MsgBoxBtnType = 
{
    BTN_ONE = 0,
    BTN_TWO = 1,
}
--宠物背景框颜色 白 绿 蓝 紫 橙
_G.ItemQulityImg = 
{  
    [1] = "diban_wupinkuang_putong",
    [2] = "diban_wupinkuang_lv",
    [3] = "diban_wupinkuang_lan",
    [4] = "diban_wupinkuang_zi",
    [5] = "diban_wupinkuang_cheng",
}
--宠物Title 品质颜色
_G.PetRareTitleImag = 
{
    [0] = "tubiao_shenshou_pinzhi_lv",
    [1] = "tubiao_shenshou_pinzhi_lv",
    [2] = "tubiao_shenshou_pinzhi_lv",
    [3] = "tubiao_shenshou_pinzhi_lan",
    [4] = "tubiao_shenshou_pinzhi_zi",
    [5] = "tubiao_shenshou_pinzhi_cheng",
}
--神兽职业类型
_G.PetProfessionType = 
{
    Physical_Atk = 1, --物理攻击
    Magic_Atk    = 2, --魔法攻击
    Treat        = 3,--治疗
    Assist       = 4,--辅助
    Tank         = 5,--坦克
}
--神兽职业类型对应Icon名称
_G.PetProfessionTypeIcon = 
{
   [1] = "tubiao_leixing1",
   [2] = "tubiao_leixing2" ,
   [3] = "tubiao_leixing3" ,
   [4] = "tubiao_leixing4" ,
   [5] = "tubiao_leixing5" ,
}
--神兽属性值对应的slider fill img name
_G.PetPropQulitySliderImg =
{
    [1] = "jindutiao_lv",
    [2] = "jindutiao_lan",
    [3] = "jindutiao_zi",
    [4] = "jindutiao_cheng",
 }
 --神兽属性对应的字体颜色
 _G.PetPropQulityTextColor = 
 {
    [1] = "<color=#5a8b00>(%s)</color>",
    [2] = "<color=#287cc6>(%s)</color>",
    [3] = "<color=#852acb>(%s)</color>",
    [4] = "<color=#e66018>(%s)</color>",
 }
 --神兽洗练title iamg name 
 _G.PetRefreshTitle = 
 {
     [1] = "zi_pingpingwuqi",
     [2] = "zi_jingtiaoxixuan",
     [3] = "zi_qianlitiaoyi",
     [4] = "zi_guanjuetianxia"
 }
 _G.PanelMaskImgName = "zhezhao_menghei"

 _G.FlashColorInterval = 0.5
 _G.FlashColor = UnityEngine.Color(1,0,0)

_G.SummonPetColor = UnityEngine.Color(0,1,0)
_G.NotSummonPetColor = UnityEngine.Color(1,0,0)

 _G.SceneModuleLayer = 13
 _G.TalkingSceneCameraDepth = 10
 _G.TalkingSceneLayer = 9
 --技能背景图
 _G.SkillQulityImg = 
 {   
     [1] = "",
     [2] = "jineng_lv",
     [3] = "jineng_lan",
     [4] = "jineng_zi",
     [5] = "jineng_cheng",
 }
 --技能边框
 _G.SkillFrameQulityImg = 
 {   
     [1] = "",
     [2] = "jineng_biankuang_lv",
     [3] = "jineng_biankuang_lan",
     [4] = "jineng_biankuang_zi",
     [5] = "jineng_biankuang_cheng",
 }

--代币类型
_G.ProxyCoinType = 
{
    Gold = 1,    --金币
    Sliver = 2 , --银币
    Soul_Stone = 3 , --魂石
    Spirit_Chip = 4,--元神碎片
    Ofuda = 5,--秘箓
    PetMoney = 6,
    --秘箓
    --蟠桃
    --元神碎片
}
--GM 字符串对用的代币类型
_G.GMProxyCoin = 
{
    money_ = ProxyCoinType.Gold,
    soul_stone_ = ProxyCoinType.Soul_Stone,
    spirit_chip_ = ProxyCoinType.Spirit_Chip,
    ofuda_ = ProxyCoinType.Ofuda,
    pet_money_ = ProxyCoinType.PetMoney,
}
--代币的图标
 _G.CoinIcon = 
 {
    [ProxyCoinType.Gold] = "tongqian_da",
    [ProxyCoinType.Soul_Stone] = "tubiao_huobi4",
    [ProxyCoinType.Spirit_Chip] = "yuanshensuipian",--huobi_yuanshensuipian
    [ProxyCoinType.Ofuda] = "fu",
    [ProxyCoinType.PetMoney] = "fu",
 }
 --代币名称
 _G.CoinName = 
 {
     [ProxyCoinType.Gold] = 10234,
     [ProxyCoinType.Soul_Stone] = 10314,
     [ProxyCoinType.Spirit_Chip] = 10314,
     [ProxyCoinType.Ofuda] = 10317,
     [ProxyCoinType.PetMoney] = 10346,
 }
--代币描述
 _G.CoinDes = 
 {
     [ProxyCoinType.Gold] = 10235,
     [ProxyCoinType.Soul_Stone] = 10311,
     [ProxyCoinType.Spirit_Chip] = 10310,
     [ProxyCoinType.Ofuda] = 10317,
     [ProxyCoinType.PetMoney] = 10346,
 }

_G.SkillEffectType = 
{
    Flying = 1,               -- 飞行的火球、弓箭等，需要从攻击者的某骨骼打到受击者的某骨骼
    TargetFlying = 2,         --指定攻击目标飞行特效
    Flash = 3,                -- 闪现一下的特效，不跟随骨骼运动
    Continuous = 4,           -- 持续的特效，跟随骨骼运动
    Position = 5,             -- 放在某个出生点的特效
    Chain = 6,                -- 链式技能
    CameraPrefab = 7,         -- 摄像机粒子
    CameraAnimation = 8,      -- 摄像机动画
    CameraParentAnim = 9,     -- 摄像机父物体动画
    CameraAttach   =10 ,      --摄像机挂到人物某个位置
    CameraPos      =11,       --摄像机放在攻击者出生点
    CameraMask = 12,          -- 摄像机蒙版
    MatProperty = 13,         -- 变颜色
    Scale = 14,               -- 缩放
    Hit = 15,                 -- 头上弹字，哗哗的掉血啊
    Audio = 16,               -- 音效，啊啊啊啊啊啊啊啊啊啊~~~~~~~~~~~~~~~
    HideModel = 17,           -- 隐藏
    ModelAnim =18,            -- 模型动作
    ModelMove =19,            -- 移动到目标位置
    IdleAnimEffect =20,       --休闲状态动作
    HideScene = 21,           -- 隐藏场景
    AnimSpeed = 22,           --控制动画播放速度
    GameSpeed = 23,           --游戏播放速度
    UIEffect = 24,      -- 模板特效
}

--背包角标 左下（英雄/神兽/皮肤）
_G.BagLBMark = 
{
   [1] = "jiaobiao_shiling_yingxiong",
   [2] = "jiaobiao_shiling_pifu",
   [3] = "jiaobiao_shenshou_yingxiong",
   [4] = "jiaobiao_shenshou_pifu",
}

--背包角标 右上（使用天数）
_G.BagRTMark = 
{
   [1] = "jiaobiao_yiri",
   --[2] = "jiaobiao_qiri",
   [3] = "jiaobiao_sanri",
   --[4] = "jiaobiao_qiri",
   [5] = "jiaobiao_wuri",
   --[6] = "jiaobiao_qiri",
   [7] = "jiaobiao_qiri",
}
--战斗选择目标操作类型
_G.CombatOprType = 
{   
    None = 0,
    Select_SingleUnit = 1,--选择单个目标
    Select_SingleUnit_Confirm = 2,--选择单个目标确定
    Select_Slot = 3, --选择地格
    Summon_Pet = 4,--召唤宠物
    Select_Self  = 5,--选择自己
    Opr_End = 6,--操作结束
}
--战斗选择目标操作提示文字id
_G.CombatOprHintLangid = 
{  
    [CombatOprType.Select_SingleUnit] = 500102,
    [CombatOprType.Select_SingleUnit_Confirm] = 500103,
    [CombatOprType.Select_Slot] = 500104,
    [CombatOprType.Summon_Pet] = 500105,
    [CombatOprType.Select_Self] = 500106,
}
--事件触发器类型
_G.EventTriggerType = 
{
    CLICK_BTN = 1,
    ENTER_SCENE = 2,
}
--引导动作类型
_G.GuideActionType = 
{
    SELECT_ENTITY = 1,--选择角色
    CAMERA_ANI    = 2,--摄像机动画
}
--引导确认类型 1 点击按钮确认 2 选择人物确认(主要用于战斗引导)
_G.GuideConfirmType = 
{
    CLICK_UI_BTN = 1,
    CLICK_ENTITY = 2,
    LONG_PRESS_ENTITY = 3,
    ACTION_END      = 4
}
--神兽属性参数
_G.PetPropertyParams = 
{
    k1 = 1.280409731,
    k2 = 1.967845406,
    k3 = 2.482683284,
    k4 = 0.719445164,
    k5 = 1.015228426,
    a1 = 8,
    b1 = 2,
    c1 = 2,
}
--神兽等级区间
_G.PetPropertyLevelSection = 
{
    [1] = 250,
    [2] = 500,
    [3] = 750,
    [4] = 1000,
}
--神兽定位名字
_G.PetPositionLangId = 
{
    10294,
    10295,
    10296,
    10297,
    10298,
}
_G.TalkingContentSpeed = 0.003
_G.TalkingDelay = 0.02
--祀灵职业类型对应Icon名称
_G.HeroProfessionTypeIcon = 
{
   [1] = "tubiao_shengzhan_da",
   [2] = "tubiao_lingwu_da",
   [3] = "tubiao_xianyi_da",
   [4] = "tubiao_shenzhu_da",
   [5] = "tubiao_siyu_da",
}

_G.HeroProfessionDisplayTypeIcon = 
{
   [1] = "tubiao_shengzhan_up",
   [2] = "tubiao_lingwu_up",
   [3] = "tubiao_xianyi_up",
   [4] = "tubiao_shenzhu_up",
   [5] = "tubiao_siyu_up",
}
--祀灵定位名字
_G.PositionLangId = 
{
    10294,
    10295,
    10296,
    10297,
    10298,
}

--代币类型
_G.PropRatioType = 
{
    k1 = 1, 
    k2 = 2, 
    k3 = 3, 
    k4 = 4,
    k5 = 5,
    a1 = 6,
    b1 = 7,
    c1 = 8,
    m1 = 9,
    m2 = 10,
    m3 = 11,
    m4 = 12,
    m5 = 13,
    m6 = 14,
    m7 = 15,
    m8 = 16,
    m9 = 17,
}

--属性参数
-- _G.PropertyParams = 
-- {
--     [PropRatioType.k1] = 1.280409731,
--     [PropRatioType.k2] = 1.967845406,
--     [PropRatioType.k3] = 2.482683284,
--     [PropRatioType.k4] = 0.719445164,
--     [PropRatioType.k5] = 1.015228426,
--     [PropRatioType.a1] = 8,
--     [PropRatioType.b1] = 2,
--     [PropRatioType.c1] = 2,
--     [PropRatioType.m1] = 0.226999907,
--     [PropRatioType.m2] = 1.015228426,
--     [PropRatioType.m3] = 1.325612152,
--     [PropRatioType.m4] = 0.76218395,
--     [PropRatioType.m5] = 1.280413935,
--     [PropRatioType.m6]  = 0.986956984,
--     [PropRatioType.m7] = 2.482692767,
--     [PropRatioType.m8] = 3.823529412,
--     [PropRatioType.m9] = 1.869454551,
-- }
_G.PropertyParams = 
{
    [PropRatioType.k1] = 1.306544831,
    [PropRatioType.k2] = 2.008007036,
    [PropRatioType.k3] = 2.533359966,
    [PropRatioType.k4] = 0.721916967,
    [PropRatioType.k5] = 1.015228426,
    [PropRatioType.a1] = 8,
    [PropRatioType.b1] = 2,
    [PropRatioType.c1] = 2,
    [PropRatioType.m1] = 0.226999907,
    [PropRatioType.m2] = 1.015228426,
    [PropRatioType.m3] = 1.352665462,
    [PropRatioType.m4] = 0.763637352,
    [PropRatioType.m5] = 1.306544831,
    [PropRatioType.m6] = 0.989231139,
    [PropRatioType.m7] = 2.533359966,
    [PropRatioType.m8] = 3.823529412,
    [PropRatioType.m9] = 1.907606684,
}

_G.PropertyBaseIndex = 
{    
    [1] = PropRatioType.m1,
    [2] = PropRatioType.m2,
    [3] = PropRatioType.m3,
    [4] = PropRatioType.m4,
    [5] = PropRatioType.m5,
    [6] = PropRatioType.m6,
    [7] = PropRatioType.m7,
    [8] = PropRatioType.m8,
    [9] = PropRatioType.m9,
}

_G.Difficulty = 
{
    [1] = 34,
    [2] = 59,
}

--等级区间
_G.PropertyLevelSection = 
{
    [1] = 250,
    [2] = 500,
    [3] = 750,
    [4] = 1000,
}

_G.PropertyLevelIcon = 
{
    [1] = "tubiao_silingtuqian_nandu_d",
    [2] = "tubiao_silingtuqian_nandu_c",
    [3] = "tubiao_silingtuqian_nandu_b",
    [4] = "tubiao_silingtuqian_nandu_a",
    [5] = "tubiao_silingtuqian_nandu_s",
}
--战中详情中 宠物的凝魂等级图片配置
_G.PetSetSoulLevelIcon = 
{
    [1] = "meishuzi_pingpingwuqi",
    [2] = "meishuzi_jingtiaoxixuan",
    [3] = "meishuzi_qianlitiaoyi",
    [4] = "meishuzi_guanjuetianxia",
}
--战中详情中 神兽凝魂状态图片配置觉醒 0 为觉醒图标
_G.PetSetSoulStatusIcon = 
{   
    [0] = "tubiao_shenshoujuexing",
    [1] = "tubiao_pingpingwuqi",
    [2] = "tubiao_jingtiaoxixuan",
    [3] = "tubiao_qianlitiaoyi",
    [4] = "tubiao_guanjuetianxia",
}