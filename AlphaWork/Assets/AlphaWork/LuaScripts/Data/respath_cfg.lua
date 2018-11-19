return 
{
    --id段 使用六位数 分为2段 前2位为对应类型 后面为编号
    -- path：对应资源目录
    -- type：资源类型 （1.普通 2.特殊）
    -- priority：优先级（一些常驻型的资源或者更重要的这里要>0）

    --10 0000 ~ 10 9999 ：UI
    --20 0000 ~ 20 9999 ：图集
    --30 0000 ~ 30 9999 ：模型
    --40 0000 ~ 40 9999 ：动画
    [100000]={
        path = "arts/ui/prefab/uiword_battle.prefab",
        type= 1,
        priority = 0,
    },
	[100001]={
        path = "alphawork/ui/uiforms/loginform.prefab",
        type= 1,
        priority = 0,
    },
	
}
