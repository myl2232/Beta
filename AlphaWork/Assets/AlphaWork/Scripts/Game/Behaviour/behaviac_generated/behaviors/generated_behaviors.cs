﻿// ---------------------------------------------------------------------
// THIS FILE IS AUTO-GENERATED BY BEHAVIAC DESIGNER, SO PLEASE DON'T MODIFY IT BY YOURSELF!
// Export file: ../../Beta/AlphaWork/Assets/AlphaWork/Scripts/Game/Behaviour/behaviac_generated/behaviors/generated_behaviors.cs
// ---------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace behaviac
{
	// Source file: AttackDispatch

	[behaviac.GeneratedTypeMetaInfo()]
	class SelectorProbability_bt_AttackDispatch_node3 : behaviac.SelectorProbability
	{
		public SelectorProbability_bt_AttackDispatch_node3()
		{
		}
	}

	[behaviac.GeneratedTypeMetaInfo()]
	class DecoratorWeight_bt_AttackDispatch_node4 : behaviac.DecoratorWeight
	{
		public DecoratorWeight_bt_AttackDispatch_node4()
		{
			m_bDecorateWhenChildEnds = false;
		}
		protected override int GetWeight(Agent pAgent)
		{
			return 30;
		}
	}

	[behaviac.GeneratedTypeMetaInfo()]
	class Assignment_bt_AttackDispatch_node5 : behaviac.Assignment
	{
		public Assignment_bt_AttackDispatch_node5()
		{
		}
		protected override EBTStatus update_impl(behaviac.Agent pAgent, behaviac.EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			float opr = 0.2f;
			pAgent.SetVariable<float>("attackParam", 2100782252u, opr);
			return result;
		}
	}

	[behaviac.GeneratedTypeMetaInfo()]
	class DecoratorWeight_bt_AttackDispatch_node6 : behaviac.DecoratorWeight
	{
		public DecoratorWeight_bt_AttackDispatch_node6()
		{
			m_bDecorateWhenChildEnds = false;
		}
		protected override int GetWeight(Agent pAgent)
		{
			return 35;
		}
	}

	[behaviac.GeneratedTypeMetaInfo()]
	class Assignment_bt_AttackDispatch_node7 : behaviac.Assignment
	{
		public Assignment_bt_AttackDispatch_node7()
		{
		}
		protected override EBTStatus update_impl(behaviac.Agent pAgent, behaviac.EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			float opr = 0.6f;
			pAgent.SetVariable<float>("attackParam", 2100782252u, opr);
			return result;
		}
	}

	[behaviac.GeneratedTypeMetaInfo()]
	class DecoratorWeight_bt_AttackDispatch_node8 : behaviac.DecoratorWeight
	{
		public DecoratorWeight_bt_AttackDispatch_node8()
		{
			m_bDecorateWhenChildEnds = false;
		}
		protected override int GetWeight(Agent pAgent)
		{
			return 35;
		}
	}

	[behaviac.GeneratedTypeMetaInfo()]
	class Assignment_bt_AttackDispatch_node9 : behaviac.Assignment
	{
		public Assignment_bt_AttackDispatch_node9()
		{
		}
		protected override EBTStatus update_impl(behaviac.Agent pAgent, behaviac.EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			float opr = 0.85f;
			pAgent.SetVariable<float>("attackParam", 2100782252u, opr);
			return result;
		}
	}

	public static class bt_AttackDispatch
	{
		public static bool build_behavior_tree(BehaviorTree bt)
		{
			bt.SetClassNameString("BehaviorTree");
			bt.SetId(-1);
			bt.SetName("AttackDispatch");
			bt.IsFSM = false;
#if !BEHAVIAC_RELEASE
			bt.SetAgentType("EnemyAgent");
#endif
			// children
			{
				SelectorProbability_bt_AttackDispatch_node3 node3 = new SelectorProbability_bt_AttackDispatch_node3();
				node3.SetClassNameString("SelectorProbability");
				node3.SetId(3);
#if !BEHAVIAC_RELEASE
				node3.SetAgentType("EnemyAgent");
#endif
				bt.AddChild(node3);
				{
					DecoratorWeight_bt_AttackDispatch_node4 node4 = new DecoratorWeight_bt_AttackDispatch_node4();
					node4.SetClassNameString("DecoratorWeight");
					node4.SetId(4);
#if !BEHAVIAC_RELEASE
					node4.SetAgentType("EnemyAgent");
#endif
					node3.AddChild(node4);
					{
						Assignment_bt_AttackDispatch_node5 node5 = new Assignment_bt_AttackDispatch_node5();
						node5.SetClassNameString("Assignment");
						node5.SetId(5);
#if !BEHAVIAC_RELEASE
						node5.SetAgentType("EnemyAgent");
#endif
						node4.AddChild(node5);
						node4.SetHasEvents(node4.HasEvents() | node5.HasEvents());
					}
					node3.SetHasEvents(node3.HasEvents() | node4.HasEvents());
				}
				{
					DecoratorWeight_bt_AttackDispatch_node6 node6 = new DecoratorWeight_bt_AttackDispatch_node6();
					node6.SetClassNameString("DecoratorWeight");
					node6.SetId(6);
#if !BEHAVIAC_RELEASE
					node6.SetAgentType("EnemyAgent");
#endif
					node3.AddChild(node6);
					{
						Assignment_bt_AttackDispatch_node7 node7 = new Assignment_bt_AttackDispatch_node7();
						node7.SetClassNameString("Assignment");
						node7.SetId(7);
#if !BEHAVIAC_RELEASE
						node7.SetAgentType("EnemyAgent");
#endif
						node6.AddChild(node7);
						node6.SetHasEvents(node6.HasEvents() | node7.HasEvents());
					}
					node3.SetHasEvents(node3.HasEvents() | node6.HasEvents());
				}
				{
					DecoratorWeight_bt_AttackDispatch_node8 node8 = new DecoratorWeight_bt_AttackDispatch_node8();
					node8.SetClassNameString("DecoratorWeight");
					node8.SetId(8);
#if !BEHAVIAC_RELEASE
					node8.SetAgentType("EnemyAgent");
#endif
					node3.AddChild(node8);
					{
						Assignment_bt_AttackDispatch_node9 node9 = new Assignment_bt_AttackDispatch_node9();
						node9.SetClassNameString("Assignment");
						node9.SetId(9);
#if !BEHAVIAC_RELEASE
						node9.SetAgentType("EnemyAgent");
#endif
						node8.AddChild(node9);
						node8.SetHasEvents(node8.HasEvents() | node9.HasEvents());
					}
					node3.SetHasEvents(node3.HasEvents() | node8.HasEvents());
				}
				bt.SetHasEvents(bt.HasEvents() | node3.HasEvents());
			}
			return true;
		}
	}

	// Source file: Base

	[behaviac.GeneratedTypeMetaInfo()]
	class State_bt_Base_node1 : behaviac.State
	{
		public State_bt_Base_node1()
		{
			this.m_bIsEndState = true;
			method_p0 = 0.2f;
		}
		protected override EBTStatus update_impl(behaviac.Agent pAgent, behaviac.EBTStatus childStatus)
		{
			((EnemyAgent)pAgent).ActionPatrol(method_p0);
			return behaviac.EBTStatus.BT_RUNNING;
		}
		float method_p0;
	}

	[behaviac.GeneratedTypeMetaInfo()]
	class Transition_bt_Base_attach6 : behaviac.Transition
	{
		public Transition_bt_Base_attach6()
		{
			this.TargetStateId = 2;
		}
		protected override EBTStatus update_impl(behaviac.Agent pAgent, behaviac.EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			LogicStatus opl = (LogicStatus)AgentMetaVisitor.GetProperty(pAgent, "logicStatus");
			LogicStatus opr2 = LogicStatus.ELogic_TRACK;
			bool op = (opl == opr2);
			if (!op)
				result = EBTStatus.BT_FAILURE;
			return result;
		}
	}

	[behaviac.GeneratedTypeMetaInfo()]
	class Transition_bt_Base_attach8 : behaviac.Transition
	{
		public Transition_bt_Base_attach8()
		{
			this.TargetStateId = 3;
		}
		protected override EBTStatus update_impl(behaviac.Agent pAgent, behaviac.EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			LogicStatus opl = (LogicStatus)AgentMetaVisitor.GetProperty(pAgent, "logicStatus");
			LogicStatus opr2 = LogicStatus.ELogic_ATTACK;
			bool op = (opl == opr2);
			if (!op)
				result = EBTStatus.BT_FAILURE;
			return result;
		}
	}

	[behaviac.GeneratedTypeMetaInfo()]
	class State_bt_Base_node2 : behaviac.State
	{
		public State_bt_Base_node2()
		{
			this.m_bIsEndState = false;
			method_p0 = 0.6f;
		}
		protected override EBTStatus update_impl(behaviac.Agent pAgent, behaviac.EBTStatus childStatus)
		{
			((EnemyAgent)pAgent).ActionPatrol(method_p0);
			return behaviac.EBTStatus.BT_RUNNING;
		}
		float method_p0;
	}

	[behaviac.GeneratedTypeMetaInfo()]
	class Transition_bt_Base_attach5 : behaviac.Transition
	{
		public Transition_bt_Base_attach5()
		{
			this.TargetStateId = 1;
		}
		protected override EBTStatus update_impl(behaviac.Agent pAgent, behaviac.EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			LogicStatus opl = (LogicStatus)AgentMetaVisitor.GetProperty(pAgent, "logicStatus");
			LogicStatus opr2 = LogicStatus.ELogic_PATROL;
			bool op = (opl == opr2);
			if (!op)
				result = EBTStatus.BT_FAILURE;
			return result;
		}
	}

	[behaviac.GeneratedTypeMetaInfo()]
	class Transition_bt_Base_attach4 : behaviac.Transition
	{
		public Transition_bt_Base_attach4()
		{
			this.TargetStateId = 3;
		}
		protected override EBTStatus update_impl(behaviac.Agent pAgent, behaviac.EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			LogicStatus opl = (LogicStatus)AgentMetaVisitor.GetProperty(pAgent, "logicStatus");
			LogicStatus opr2 = LogicStatus.ELogic_ATTACK;
			bool op = (opl == opr2);
			if (!op)
				result = EBTStatus.BT_FAILURE;
			return result;
		}
	}

	[behaviac.GeneratedTypeMetaInfo()]
	class State_bt_Base_node3 : behaviac.State
	{
		public State_bt_Base_node3()
		{
			this.m_bIsEndState = false;
		}
		protected override EBTStatus update_impl(behaviac.Agent pAgent, behaviac.EBTStatus childStatus)
		{
			((EnemyAgent)pAgent).ActionAttack();
			return behaviac.EBTStatus.BT_RUNNING;
		}
	}

	[behaviac.GeneratedTypeMetaInfo()]
	class Transition_bt_Base_attach7 : behaviac.Transition
	{
		public Transition_bt_Base_attach7()
		{
			this.TargetStateId = 1;
		}
		protected override EBTStatus update_impl(behaviac.Agent pAgent, behaviac.EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			LogicStatus opl = (LogicStatus)AgentMetaVisitor.GetProperty(pAgent, "logicStatus");
			LogicStatus opr2 = LogicStatus.ELogic_PATROL;
			bool op = (opl == opr2);
			if (!op)
				result = EBTStatus.BT_FAILURE;
			return result;
		}
	}

	public static class bt_Base
	{
		public static bool build_behavior_tree(BehaviorTree bt)
		{
			bt.SetClassNameString("BehaviorTree");
			bt.SetId(-1);
			bt.SetName("Base");
			bt.IsFSM = true;
#if !BEHAVIAC_RELEASE
			bt.SetAgentType("EnemyAgent");
#endif
			// attachments
			// children
			{
				FSM fsm = new FSM();
				fsm.SetClassNameString("FSM");
				fsm.SetId(-1);
				fsm.InitialId = 1;
#if !BEHAVIAC_RELEASE
				fsm.SetAgentType("EnemyAgent");
#endif
				{
					State_bt_Base_node1 node1 = new State_bt_Base_node1();
					node1.SetClassNameString("State");
					node1.SetId(1);
#if !BEHAVIAC_RELEASE
					node1.SetAgentType("EnemyAgent");
#endif
					// attachments
					{
						Transition_bt_Base_attach6 attach6 = new Transition_bt_Base_attach6();
						attach6.SetClassNameString("Transition");
						attach6.SetId(6);
#if !BEHAVIAC_RELEASE
						attach6.SetAgentType("EnemyAgent");
#endif
						node1.Attach(attach6, false, false, true);
					}
					{
						Transition_bt_Base_attach8 attach8 = new Transition_bt_Base_attach8();
						attach8.SetClassNameString("Transition");
						attach8.SetId(8);
#if !BEHAVIAC_RELEASE
						attach8.SetAgentType("EnemyAgent");
#endif
						node1.Attach(attach8, false, false, true);
					}
					fsm.AddChild(node1);
					fsm.SetHasEvents(fsm.HasEvents() | node1.HasEvents());
				}
				{
					State_bt_Base_node2 node2 = new State_bt_Base_node2();
					node2.SetClassNameString("State");
					node2.SetId(2);
#if !BEHAVIAC_RELEASE
					node2.SetAgentType("EnemyAgent");
#endif
					// attachments
					{
						Transition_bt_Base_attach5 attach5 = new Transition_bt_Base_attach5();
						attach5.SetClassNameString("Transition");
						attach5.SetId(5);
#if !BEHAVIAC_RELEASE
						attach5.SetAgentType("EnemyAgent");
#endif
						node2.Attach(attach5, false, false, true);
					}
					{
						Transition_bt_Base_attach4 attach4 = new Transition_bt_Base_attach4();
						attach4.SetClassNameString("Transition");
						attach4.SetId(4);
#if !BEHAVIAC_RELEASE
						attach4.SetAgentType("EnemyAgent");
#endif
						node2.Attach(attach4, false, false, true);
					}
					fsm.AddChild(node2);
					fsm.SetHasEvents(fsm.HasEvents() | node2.HasEvents());
				}
				{
					State_bt_Base_node3 node3 = new State_bt_Base_node3();
					node3.SetClassNameString("State");
					node3.SetId(3);
#if !BEHAVIAC_RELEASE
					node3.SetAgentType("EnemyAgent");
#endif
					// attachments
					{
						Transition_bt_Base_attach7 attach7 = new Transition_bt_Base_attach7();
						attach7.SetClassNameString("Transition");
						attach7.SetId(7);
#if !BEHAVIAC_RELEASE
						attach7.SetAgentType("EnemyAgent");
#endif
						node3.Attach(attach7, false, false, true);
					}
					fsm.AddChild(node3);
					fsm.SetHasEvents(fsm.HasEvents() | node3.HasEvents());
				}
				bt.AddChild(fsm);
			}
			return true;
		}
	}

	// Source file: EnemyAvatar

	[behaviac.GeneratedTypeMetaInfo()]
	class Parallel_bt_EnemyAvatar_node0 : behaviac.Parallel
	{
		public Parallel_bt_EnemyAvatar_node0()
		{
			m_failPolicy = behaviac.FAILURE_POLICY.FAIL_ON_ONE;
			m_succeedPolicy = behaviac.SUCCESS_POLICY.SUCCEED_ON_ALL;
			m_exitPolicy = behaviac.EXIT_POLICY.EXIT_ABORT_RUNNINGSIBLINGS;
			m_childFinishPolicy = behaviac.CHILDFINISH_POLICY.CHILDFINISH_LOOP;
		}
	}

	[behaviac.GeneratedTypeMetaInfo()]
	class Condition_bt_EnemyAvatar_node4 : behaviac.Condition
	{
		public Condition_bt_EnemyAvatar_node4()
		{
		}
		protected override EBTStatus update_impl(behaviac.Agent pAgent, behaviac.EBTStatus childStatus)
		{
			bool opl = (bool)AgentMetaVisitor.GetProperty(pAgent, "bAwakeSense");
			bool opr = true;
			bool op = opl == opr;
			return op ? EBTStatus.BT_SUCCESS : EBTStatus.BT_FAILURE;
		}
	}

	[behaviac.GeneratedTypeMetaInfo()]
	class Action_bt_EnemyAvatar_node5 : behaviac.Action
	{
		public Action_bt_EnemyAvatar_node5()
		{
			this.m_resultOption = EBTStatus.BT_SUCCESS;
		}
		protected override EBTStatus update_impl(behaviac.Agent pAgent, behaviac.EBTStatus childStatus)
		{
			((EnemyAgent)pAgent).FlushSensor();
			return EBTStatus.BT_SUCCESS;
		}
	}

	[behaviac.GeneratedTypeMetaInfo()]
	class Action_bt_EnemyAvatar_node9 : behaviac.Action
	{
		public Action_bt_EnemyAvatar_node9()
		{
			this.m_resultOption = EBTStatus.BT_SUCCESS;
		}
		protected override EBTStatus update_impl(behaviac.Agent pAgent, behaviac.EBTStatus childStatus)
		{
			((EnemyAgent)pAgent).CheckSensor();
			return EBTStatus.BT_SUCCESS;
		}
	}

	[behaviac.GeneratedTypeMetaInfo()]
	class ReferencedBehavior_bt_EnemyAvatar_node7 : behaviac.ReferencedBehavior
	{
		public ReferencedBehavior_bt_EnemyAvatar_node7()
		{
			string szTreePath = this.GetReferencedTree(null);
			if (!string.IsNullOrEmpty(szTreePath)) {
			BehaviorTree behaviorTree = Workspace.Instance.LoadBehaviorTree(szTreePath);
			if (behaviorTree != null)
			{
				this.m_bHasEvents |= behaviorTree.HasEvents();
			}
			}
		}
		public override string GetReferencedTree(Agent pAgent)
		{
			return "AttackDispatch";
		}
	}

	public static class bt_EnemyAvatar
	{
		public static bool build_behavior_tree(BehaviorTree bt)
		{
			bt.SetClassNameString("BehaviorTree");
			bt.SetId(-1);
			bt.SetName("EnemyAvatar");
			bt.IsFSM = false;
#if !BEHAVIAC_RELEASE
			bt.SetAgentType("EnemyAgent");
#endif
			// children
			{
				Parallel_bt_EnemyAvatar_node0 node0 = new Parallel_bt_EnemyAvatar_node0();
				node0.SetClassNameString("Parallel");
				node0.SetId(0);
#if !BEHAVIAC_RELEASE
				node0.SetAgentType("EnemyAgent");
#endif
				bt.AddChild(node0);
				{
					Sequence node1 = new Sequence();
					node1.SetClassNameString("Sequence");
					node1.SetId(1);
#if !BEHAVIAC_RELEASE
					node1.SetAgentType("EnemyAgent");
#endif
					node0.AddChild(node1);
					{
						WaitforSignal node3 = new WaitforSignal();
						node3.SetClassNameString("WaitforSignal");
						node3.SetId(3);
#if !BEHAVIAC_RELEASE
						node3.SetAgentType("EnemyAgent");
#endif
						node1.AddChild(node3);
						{
							Condition_bt_EnemyAvatar_node4 node4 = new Condition_bt_EnemyAvatar_node4();
							node4.SetClassNameString("Condition");
							node4.SetId(4);
#if !BEHAVIAC_RELEASE
							node4.SetAgentType("EnemyAgent");
#endif
							node3.SetCustomCondition(node4);
							node3.SetHasEvents(node3.HasEvents() | node4.HasEvents());
						}
						node1.SetHasEvents(node1.HasEvents() | node3.HasEvents());
					}
					{
						Action_bt_EnemyAvatar_node5 node5 = new Action_bt_EnemyAvatar_node5();
						node5.SetClassNameString("Action");
						node5.SetId(5);
#if !BEHAVIAC_RELEASE
						node5.SetAgentType("EnemyAgent");
#endif
						node1.AddChild(node5);
						node1.SetHasEvents(node1.HasEvents() | node5.HasEvents());
					}
					{
						Action_bt_EnemyAvatar_node9 node9 = new Action_bt_EnemyAvatar_node9();
						node9.SetClassNameString("Action");
						node9.SetId(9);
#if !BEHAVIAC_RELEASE
						node9.SetAgentType("EnemyAgent");
#endif
						node1.AddChild(node9);
						node1.SetHasEvents(node1.HasEvents() | node9.HasEvents());
					}
					{
						ReferencedBehavior_bt_EnemyAvatar_node7 node7 = new ReferencedBehavior_bt_EnemyAvatar_node7();
						node7.SetClassNameString("ReferencedBehavior");
						node7.SetId(7);
#if !BEHAVIAC_RELEASE
						node7.SetAgentType("EnemyAgent");
#endif
						node1.AddChild(node7);
						node1.SetHasEvents(node1.HasEvents() | node7.HasEvents());
					}
					node0.SetHasEvents(node0.HasEvents() | node1.HasEvents());
				}
				bt.SetHasEvents(bt.HasEvents() | node0.HasEvents());
			}
			return true;
		}
	}

}
