local NotifyId = 
{
    PRELOAD_FINISH			= 0,	--预加载回调
	CREATE_PANEL		    = 1,	--创建panel
    CREATE_PANEL_FINISH		= 2,	--创建panel完成事件
    DESTROY_PANEL           = 3,    --销毁panel
    DESTROY_PANEL_FINISH    = 4,    --销毁panel完成事件
    AFTER_DESTROY_PANEL     = 5,    --销毁panel后
    ENTER_WORLD             = 6,    --进入大厅
    LOADING_PROGRESS        = 7,    --更新加载进度
    RESET_LOADING           = 8,    --重置loading进度
    TO_LOGIN                = 9,    --返回登陆
    MONEY_CHANGE            = 10,    --金币变化
    FLASH_CONTENT           = 11,   --flash提示框
    CLOSE_TIP               = 12,   --关闭tip
    ON_GET_ITEMS            = 13,   --获得物品
    HIDE_UI                 = 14,   --隐藏金币和聊天
    SHOW_PANEL              = 15,   --显示Panel
    START_PLOT              = 16,   --显示剧情对话
    FINISH_PLOT             = 17,   --完成剧情
    HIDE_LDDD               = 18,   --3D书背景
    CLICK_BTN               = 19,   --点击按钮
    CLIENT_MSG_HANDLER      = 20,   --客户端消息
    NEW_GUIDE               = 21,   --开始引导
    CLICK_ENTITY            = 22,   --点击实体
    CHOOSE_MONEY_SHOW       = 23,   --金币界面部分显示
    FINISH_GUIDE            = 24,   --完成引导
    CLOSE_GUIDE             = 25,   --关闭引导
    CLOSE_PLOT              = 26,   --关闭剧情
    LOADING_FINISH          = 27,   --进度条结束
}

return NotifyId