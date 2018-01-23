#ifndef COMMON_CALCULATE_INCLUDE  
#define COMMON_CALCULATE_INCLUDE  	
	
	#define VAR_COMMON_CALCULATE_NEED
		float3 _CenterWorldPos;		//记录渲染物体世界坐标，在没有UV的时候，用来算相对位置
	
	#define GET_UV_WITH_POS_NOR_CENTER_POS(wPos,wNormal) GET_UV_CENTER_POS(wPos,wNormal);
		float2 GET_UV_CENTER_POS(float3 worldPos,float3 worldNormal) 
		{
			float2 uvTiling;
			float3 newPos = worldPos - _CenterWorldPos;
			uvTiling.y = newPos.y;
			uvTiling.x = abs(worldNormal.x) > abs(worldNormal.z) ?  newPos.z : newPos.x;	
			uvTiling = abs(worldNormal.y) > 0.95 ? newPos.xz : uvTiling;
			return uvTiling;
		}
	
	#define GET_UV_WITH_POS_NOR(wPos,wNormal) GET_UV(wPos,wNormal);
		float2 GET_UV(float3 worldPos,float3 worldNormal) 
		{
			float2 uvTiling;
			uvTiling.y = worldPos.y;
			uvTiling.x = abs(worldNormal.x) > abs(worldNormal.z) ?  worldPos.z : worldPos.x;	
			uvTiling = abs(worldNormal.y) > 0.95 ? worldPos.xz : uvTiling;
			return uvTiling;
		}

#endif 