return
{
	[2] = 
	{
		Id = 2,
		Actions = 
		{
			{
				Type = 18,
				Start = 0,
				End = -1,
				ResourceName = "{0}_stand",
			},
		}
	},
	[5] = 
	{
		Id = 5,
		Actions = 
		{
			{
				Type = 18,
				Start = 0,
				End = -1,
				ResourceName = "{0}_use",
			},
			{
				Type = 15,
				Start = 0,
				End = -1,
				UseStage = 0,
				ShowAttackedAnim = 0,
			},
			{
				Type = 3,
				Start = 0.0,
				End = 2.0,
				UseStage = 0,
				AttachPoint = "foot",
				ResourceName = "fx_common_pure",
				ToTarget = true,
				StartOffsetPos = { x=0,y=0,z=0},
			},
		}
	},
	[6] = 
	{
		Id = 6,
		Actions = 
		{
			{
				Type = 18,
				Start = 0,
				End = -1,
				ResourceName = "{0}_use",
			},
			{
				Type = 15,
				Start = 0,
				End = -1,
				UseStage = 0,
				ShowAttackedAnim = 0,
			},
			{
				Type = 3,
				Start = 0.0,
				End = 2.0,
				UseStage = 0,
				AttachPoint = "foot",
				ResourceName = "fx_common_revive",
				ToTarget = true,
				StartOffsetPos = { x=0,y=0,z=0},
			},
		}
	},
	
	[12500101] = 
	{
		Id = 12500101,
		Actions = 
		{
			{
				Type = 18,
				Start = 0.0,
				End = 1.48,
				ResourceName = "001_m_jinw_attack",
			},
			{
				Type = 1,
				Start = 0.5,
				End = 0.9,
				UseStage = 0,
				AttachPoint = "head",
				ResourceName = "fx_m_jinw_attack_001",
				StartOffsetPos = { x=0,y=0,z=0},
				TargetOffsetPos = { x=0,y=0,z=0},
				MoveType = 1,
				ToTarget = true,
				StartDelay = 0.0,
				EndEarly  = 0.0,
			},
			{
				Type = 15,
				Start = 0.9,
				End = 1.5,
				UseStage = 0,
				ShowAttackedAnim = 1,
			},
			{
				Type = 16,
				Start = 0.0,
				End = 1.5,
				ResourceName = "event:/Heroes/Beasts/jinw/001_m_jinw_attack",
			},
			{
				Type = 3,
				Start = 0.9,
				End = 1.5,
				UseStage = 0,
				AttachPoint = "hit_point",
				ResourceName = "fx_hit_fire",
				ToTarget = true,
				StartOffsetPos = { x=0,y=0,z=0},
			},
			{
				Type = 16,
				Start = 0.9,
				End = 1.5,
				ResourceName = "event:/Heroes/Act/Hit/Fire",
			},
		}
	},
}