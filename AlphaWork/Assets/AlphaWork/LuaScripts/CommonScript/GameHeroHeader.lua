---
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by GuoMing.
--- DateTime: 2018/4/16 15:23
---
protected_.registMetaProp(protected_.MetaHero, "hero_id_", 0);	--hero在配置中的唯一id
protected_.registMetaProp(protected_.MetaHero, "hero_type_", Enum.EHeroUseType_Permanent);--区分是限免的还是永久的等类型
protected_.registMetaProp(protected_.MetaHero, "hero_expire_", 0);--英雄到期时间点(毫秒)，对有有效期的英雄而言
protected_.registMetaProp(protected_.MetaHero, "skins_", 0);--已经拥有皮肤
protected_.registMetaProp(protected_.MetaHero, "cur_skin_", 0);--当前皮肤
protected_.registMetaProp(protected_.MetaHeroBag, "hero_num_", 0);--已经拥有永久英雄的个数
protected_.registMetaMap(protected_.MetaHeroBag, 0, nil, protected_.MetaHero);