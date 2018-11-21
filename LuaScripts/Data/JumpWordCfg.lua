return {
    [DamageType.Cure] = {
        --type_ = "治疗",                           -- 伤害类型

        normal_color_ = {r = 0 / 255, g = 215 / 255, b = 0 / 255, a = 1},  -- 常规颜色
        bomb_color1_ =  {r = 0 / 255, g = 215 / 255, b = 0 / 255, a = 1},  -- 暴击颜色1
        bomb_color2_ =  {r = 0 / 255, g = 215 / 255, b = 0 / 255, a = 1},   -- 暴击颜色2(暴击渐变色)
        
        normal_scale_from_ = 1.4,                 -- 常规进场scale
        normal_scale_to_ = 0.9,                   -- 常规最终scale

        bomb_scale_from_ = 1.8,                   -- 暴击进场scale
        bomb_scale_to_ = 1.2                        -- 暴击最终scale
    },
    
    [DamageType.Real] = {
        --type_ = "真实",
        
        normal_color_ = {r = 255 / 255, g = 255 / 255, b = 255 / 255, a = 1},
        bomb_color1_ =  {r = 255 / 255, g = 255 / 255, b = 255 / 255, a = 1},
        bomb_color2_ =  {r = 255 / 255, g = 255 / 255, b = 255 / 255, a = 1},

        normal_scale_from_ = 1.4,                 -- 常规进场scale
        normal_scale_to_ = 0.9,                   -- 常规最终scale

        bomb_scale_from_ = 1.8,                   -- 暴击进场scale
        bomb_scale_to_ = 1.2                        -- 暴击最终scale
    },
    
    [DamageType.Physical] = {
        --type_ = "物理",
        
        normal_color_ = {r = 254 / 255, g = 44 / 255, b = 44 / 255, a = 1},
        bomb_color1_ =  {r = 254 / 255, g = 44 / 255, b = 44 / 255, a = 1},
        bomb_color2_ =  {r = 254 / 255, g = 44 / 255, b = 44 / 255, a = 1},

        normal_scale_from_ = 1.4,                 -- 常规进场scale
        normal_scale_to_ = 0.9,                   -- 常规最终scale

        bomb_scale_from_ = 1.8,                   -- 暴击进场scale
        bomb_scale_to_ = 1.2                        -- 暴击最终scale
    },
    
    [DamageType.Spell] = {
        --type_ = "法术",
        
        normal_color_ = {r = 53 / 255, g = 123 / 255, b = 255 / 255, a = 1},
        bomb_color1_ =  {r = 53 / 255, g = 123 / 255, b = 255 / 255, a = 1},
        bomb_color2_ =  {r = 53 / 255, g = 123 / 255, b = 255 / 255, a = 1},

        normal_scale_from_ = 1.4,                 -- 常规进场scale
        normal_scale_to_ = 0.9,                   -- 常规最终scale

        bomb_scale_from_ = 1.8,                   -- 暴击进场scale
        bomb_scale_to_ = 1.2                        -- 暴击最终scale
    },
    
    [DamageType.Shield] = {
        --type_ = "护盾",
        
        normal_color_ = {r = 200 / 255, g = 200 / 255, b = 200 / 255, a = 1},
        bomb_color1_ =  {r = 200 / 255, g = 200 / 255, b = 200 / 255, a = 1},
        bomb_color2_ =  {r = 200 / 255, g = 200 / 255, b = 200 / 255, a = 1},

        normal_scale_from_ = 1.4,                 -- 常规进场scale
        normal_scale_to_ = 0.9,                   -- 常规最终scale

        bomb_scale_from_ = 1.8,                   -- 暴击进场scale
        bomb_scale_to_ = 1.2                        -- 暴击最终scale
    },
    [DamageType.Failed] = {
        --type_ = "未命中类",
        
        normal_color_ = {r = 255 / 255, g = 255 / 255, b = 153 / 255, a = 1},

        normal_scale_from_ = 1.2,                 -- 常规进场scale
        normal_scale_to_ = 0.6,                   -- 常规最终scale
	},
} 