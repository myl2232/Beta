﻿// ---------------------------------------------------------------------
// THIS FILE IS AUTO-GENERATED BY BEHAVIAC DESIGNER, SO PLEASE DON'T MODIFY IT BY YOURSELF!
// ---------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;

namespace behaviac
{
	public class CompareValue_LogicStatus : ICompareValue<LogicStatus>
	{
		public override bool Equal(LogicStatus lhs, LogicStatus rhs)
		{
			return lhs == rhs;
		}
		public override bool NotEqual(LogicStatus lhs, LogicStatus rhs)
		{
			return lhs != rhs;
		}
		public override bool Greater(LogicStatus lhs, LogicStatus rhs)
		{
			return lhs > rhs;
		}
		public override bool GreaterEqual(LogicStatus lhs, LogicStatus rhs)
		{
			return lhs >= rhs;
		}
		public override bool Less(LogicStatus lhs, LogicStatus rhs)
		{
			return lhs < rhs;
		}
		public override bool LessEqual(LogicStatus lhs, LogicStatus rhs)
		{
			return lhs <= rhs;
		}
	}


	public class BehaviorLoaderImplement : BehaviorLoader
	{
		private class CMethod_behaviac_Agent_VectorAdd : CAgentMethodVoidBase
		{
			IInstanceMember _param0;
			IInstanceMember _param1;

			public CMethod_behaviac_Agent_VectorAdd()
			{
			}

			public CMethod_behaviac_Agent_VectorAdd(CMethod_behaviac_Agent_VectorAdd rhs) : base(rhs)
			{
			}

			public override IMethod Clone()
			{
				return new CMethod_behaviac_Agent_VectorAdd(this);
			}

			public override void Load(string instance, string[] paramStrs)
			{
				Debug.Check(paramStrs.Length == 2);

				_instance = instance;
				_param0 = AgentMeta.ParseProperty<IList>(paramStrs[0]);
				_param1 = AgentMeta.ParseProperty<System.Object>(paramStrs[1]);
			}

			public override void Run(Agent self)
			{
				Debug.Check(_param0 != null);
				Debug.Check(_param1 != null);

				behaviac.Agent.VectorAdd((IList)_param0.GetValueObject(self), (System.Object)_param1.GetValueObject(self));
			}
		}

		private class CMethod_behaviac_Agent_VectorClear : CAgentMethodVoidBase
		{
			IInstanceMember _param0;

			public CMethod_behaviac_Agent_VectorClear()
			{
			}

			public CMethod_behaviac_Agent_VectorClear(CMethod_behaviac_Agent_VectorClear rhs) : base(rhs)
			{
			}

			public override IMethod Clone()
			{
				return new CMethod_behaviac_Agent_VectorClear(this);
			}

			public override void Load(string instance, string[] paramStrs)
			{
				Debug.Check(paramStrs.Length == 1);

				_instance = instance;
				_param0 = AgentMeta.ParseProperty<IList>(paramStrs[0]);
			}

			public override void Run(Agent self)
			{
				Debug.Check(_param0 != null);

				behaviac.Agent.VectorClear((IList)_param0.GetValueObject(self));
			}
		}

		private class CMethod_behaviac_Agent_VectorContains : CAgentMethodBase<bool>
		{
			IInstanceMember _param0;
			IInstanceMember _param1;

			public CMethod_behaviac_Agent_VectorContains()
			{
			}

			public CMethod_behaviac_Agent_VectorContains(CMethod_behaviac_Agent_VectorContains rhs) : base(rhs)
			{
			}

			public override IMethod Clone()
			{
				return new CMethod_behaviac_Agent_VectorContains(this);
			}

			public override void Load(string instance, string[] paramStrs)
			{
				Debug.Check(paramStrs.Length == 2);

				_instance = instance;
				_param0 = AgentMeta.ParseProperty<IList>(paramStrs[0]);
				_param1 = AgentMeta.ParseProperty<System.Object>(paramStrs[1]);
			}

			public override void Run(Agent self)
			{
				Debug.Check(_param0 != null);
				Debug.Check(_param1 != null);

				_returnValue.value = behaviac.Agent.VectorContains((IList)_param0.GetValueObject(self), (System.Object)_param1.GetValueObject(self));
			}
		}

		private class CMethod_behaviac_Agent_VectorLength : CAgentMethodBase<int>
		{
			IInstanceMember _param0;

			public CMethod_behaviac_Agent_VectorLength()
			{
			}

			public CMethod_behaviac_Agent_VectorLength(CMethod_behaviac_Agent_VectorLength rhs) : base(rhs)
			{
			}

			public override IMethod Clone()
			{
				return new CMethod_behaviac_Agent_VectorLength(this);
			}

			public override void Load(string instance, string[] paramStrs)
			{
				Debug.Check(paramStrs.Length == 1);

				_instance = instance;
				_param0 = AgentMeta.ParseProperty<IList>(paramStrs[0]);
			}

			public override void Run(Agent self)
			{
				Debug.Check(_param0 != null);

				_returnValue.value = behaviac.Agent.VectorLength((IList)_param0.GetValueObject(self));
			}
		}

		private class CMethod_behaviac_Agent_VectorRemove : CAgentMethodVoidBase
		{
			IInstanceMember _param0;
			IInstanceMember _param1;

			public CMethod_behaviac_Agent_VectorRemove()
			{
			}

			public CMethod_behaviac_Agent_VectorRemove(CMethod_behaviac_Agent_VectorRemove rhs) : base(rhs)
			{
			}

			public override IMethod Clone()
			{
				return new CMethod_behaviac_Agent_VectorRemove(this);
			}

			public override void Load(string instance, string[] paramStrs)
			{
				Debug.Check(paramStrs.Length == 2);

				_instance = instance;
				_param0 = AgentMeta.ParseProperty<IList>(paramStrs[0]);
				_param1 = AgentMeta.ParseProperty<System.Object>(paramStrs[1]);
			}

			public override void Run(Agent self)
			{
				Debug.Check(_param0 != null);
				Debug.Check(_param1 != null);

				behaviac.Agent.VectorRemove((IList)_param0.GetValueObject(self), (System.Object)_param1.GetValueObject(self));
			}
		}


		public override bool Load()
		{
			AgentMeta.TotalSignature = 96213612;

			AgentMeta meta;

			// behaviac.Agent
			meta = new AgentMeta(24743406);
			AgentMeta._AgentMetas_[2436498804] = meta;
			meta.RegisterMethod(1045109914, new CAgentStaticMethodVoid<string>(delegate(string param0) { behaviac.Agent.LogMessage(param0); }));
			meta.RegisterMethod(2521019022, new CMethod_behaviac_Agent_VectorAdd());
			meta.RegisterMethod(2306090221, new CMethod_behaviac_Agent_VectorClear());
			meta.RegisterMethod(3483755530, new CMethod_behaviac_Agent_VectorContains());
			meta.RegisterMethod(505785840, new CMethod_behaviac_Agent_VectorLength());
			meta.RegisterMethod(502968959, new CMethod_behaviac_Agent_VectorRemove());

			// BaseAgent
			meta = new AgentMeta(3096401382);
			AgentMeta._AgentMetas_[2774251291] = meta;
			meta.RegisterMemberProperty(2245610422, new CMemberProperty<bool>("bAwakeSense", delegate(Agent self, bool value) { ((BaseAgent)self)._set_bAwakeSense(value); }, delegate(Agent self) { return ((BaseAgent)self)._get_bAwakeSense(); }));
			meta.RegisterMemberProperty(4105861508, new CMemberProperty<LogicStatus>("logicStatus", delegate(Agent self, LogicStatus value) { ((BaseAgent)self)._set_logicStatus(value); }, delegate(Agent self) { return ((BaseAgent)self)._get_logicStatus(); }));
			meta.RegisterMethod(1045109914, new CAgentStaticMethodVoid<string>(delegate(string param0) { BaseAgent.LogMessage(param0); }));
			meta.RegisterMethod(2521019022, new CMethod_behaviac_Agent_VectorAdd());
			meta.RegisterMethod(2306090221, new CMethod_behaviac_Agent_VectorClear());
			meta.RegisterMethod(3483755530, new CMethod_behaviac_Agent_VectorContains());
			meta.RegisterMethod(505785840, new CMethod_behaviac_Agent_VectorLength());
			meta.RegisterMethod(502968959, new CMethod_behaviac_Agent_VectorRemove());

			// EnemyAgent
			meta = new AgentMeta(3951108106);
			AgentMeta._AgentMetas_[3531795815] = meta;
			meta.RegisterMemberProperty(2100782252, new CMemberProperty<float>("attackParam", delegate(Agent self, float value) { ((EnemyAgent)self)._set_attackParam(value); }, delegate(Agent self) { return ((EnemyAgent)self)._get_attackParam(); }));
			meta.RegisterMemberProperty(2245610422, new CMemberProperty<bool>("bAwakeSense", delegate(Agent self, bool value) { ((EnemyAgent)self)._set_bAwakeSense(value); }, delegate(Agent self) { return ((EnemyAgent)self)._get_bAwakeSense(); }));
			meta.RegisterMemberProperty(4105861508, new CMemberProperty<LogicStatus>("logicStatus", delegate(Agent self, LogicStatus value) { ((EnemyAgent)self)._set_logicStatus(value); }, delegate(Agent self) { return ((EnemyAgent)self)._get_logicStatus(); }));
			meta.RegisterMethod(717029417, new CAgentMethodVoid(delegate(Agent self) { ((EnemyAgent)self).CheckSensor(); }));
			meta.RegisterMethod(119980225, new CAgentMethodVoid(delegate(Agent self) { ((EnemyAgent)self).FlushSensor(); }));
			meta.RegisterMethod(1045109914, new CAgentStaticMethodVoid<string>(delegate(string param0) { EnemyAgent.LogMessage(param0); }));
			meta.RegisterMethod(2521019022, new CMethod_behaviac_Agent_VectorAdd());
			meta.RegisterMethod(2306090221, new CMethod_behaviac_Agent_VectorClear());
			meta.RegisterMethod(3483755530, new CMethod_behaviac_Agent_VectorContains());
			meta.RegisterMethod(505785840, new CMethod_behaviac_Agent_VectorLength());
			meta.RegisterMethod(502968959, new CMethod_behaviac_Agent_VectorRemove());

			AgentMeta.Register<behaviac.Agent>("behaviac.Agent");
			AgentMeta.Register<BaseAgent>("BaseAgent");
			AgentMeta.Register<EnemyAgent>("EnemyAgent");
			AgentMeta.Register<LogicStatus>("LogicStatus");
			ComparerRegister.RegisterType<LogicStatus, CompareValue_LogicStatus>();

			Agent.RegisterInstanceName<EnemyAgent>("EnemyAgent");
			Agent.RegisterInstanceName<BaseAgent>("BaseAgent");
			return true;
		}

		public override bool UnLoad()
		{
			AgentMeta.UnRegister<behaviac.Agent>("behaviac.Agent");
			AgentMeta.UnRegister<BaseAgent>("BaseAgent");
			AgentMeta.UnRegister<EnemyAgent>("EnemyAgent");
			AgentMeta.UnRegister<LogicStatus>("LogicStatus");

			Agent.UnRegisterInstanceName<EnemyAgent>("EnemyAgent");
			Agent.UnRegisterInstanceName<BaseAgent>("BaseAgent");
			return true;
		}
	}
}
