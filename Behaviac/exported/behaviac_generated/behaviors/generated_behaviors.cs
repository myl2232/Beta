﻿// ---------------------------------------------------------------------
// THIS FILE IS AUTO-GENERATED BY BEHAVIAC DESIGNER, SO PLEASE DON'T MODIFY IT BY YOURSELF!
// Export file: exported/behaviac_generated/behaviors/generated_behaviors.cs
// ---------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace behaviac
{
	// Source file: EnemyAvartar

	[behaviac.GeneratedTypeMetaInfo()]
	class Parallel_bt_EnemyAvartar_node0 : behaviac.Parallel
	{
		public Parallel_bt_EnemyAvartar_node0()
		{
			m_failPolicy = behaviac.FAILURE_POLICY.FAIL_ON_ONE;
			m_succeedPolicy = behaviac.SUCCESS_POLICY.SUCCEED_ON_ALL;
			m_exitPolicy = behaviac.EXIT_POLICY.EXIT_ABORT_RUNNINGSIBLINGS;
			m_childFinishPolicy = behaviac.CHILDFINISH_POLICY.CHILDFINISH_LOOP;
		}
	}

	[behaviac.GeneratedTypeMetaInfo()]
	class Condition_bt_EnemyAvartar_node2 : behaviac.Condition
	{
		public Condition_bt_EnemyAvartar_node2()
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
	class Action_bt_EnemyAvartar_node3 : behaviac.Action
	{
		public Action_bt_EnemyAvartar_node3()
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
	class Action_bt_EnemyAvartar_node4 : behaviac.Action
	{
		public Action_bt_EnemyAvartar_node4()
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
	class SelectorProbability_bt_EnemyAvartar_node5 : behaviac.SelectorProbability
	{
		public SelectorProbability_bt_EnemyAvartar_node5()
		{
		}
	}

	[behaviac.GeneratedTypeMetaInfo()]
	class DecoratorWeight_bt_EnemyAvartar_node6 : behaviac.DecoratorWeight
	{
		public DecoratorWeight_bt_EnemyAvartar_node6()
		{
			m_bDecorateWhenChildEnds = false;
		}
		protected override int GetWeight(Agent pAgent)
		{
			return 35;
		}
	}

	[behaviac.GeneratedTypeMetaInfo()]
	class Assignment_bt_EnemyAvartar_node9 : behaviac.Assignment
	{
		public Assignment_bt_EnemyAvartar_node9()
		{
		}
		protected override EBTStatus update_impl(behaviac.Agent pAgent, behaviac.EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			float opr = 0f;
			pAgent.SetVariable<float>("attackParam", 2100782252u, opr);
			return result;
		}
	}

	[behaviac.GeneratedTypeMetaInfo()]
	class DecoratorWeight_bt_EnemyAvartar_node7 : behaviac.DecoratorWeight
	{
		public DecoratorWeight_bt_EnemyAvartar_node7()
		{
			m_bDecorateWhenChildEnds = false;
		}
		protected override int GetWeight(Agent pAgent)
		{
			return 35;
		}
	}

	[behaviac.GeneratedTypeMetaInfo()]
	class Assignment_bt_EnemyAvartar_node8 : behaviac.Assignment
	{
		public Assignment_bt_EnemyAvartar_node8()
		{
		}
		protected override EBTStatus update_impl(behaviac.Agent pAgent, behaviac.EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			float opr = 0.5f;
			pAgent.SetVariable<float>("attackParam", 2100782252u, opr);
			return result;
		}
	}

	[behaviac.GeneratedTypeMetaInfo()]
	class DecoratorWeight_bt_EnemyAvartar_node10 : behaviac.DecoratorWeight
	{
		public DecoratorWeight_bt_EnemyAvartar_node10()
		{
			m_bDecorateWhenChildEnds = false;
		}
		protected override int GetWeight(Agent pAgent)
		{
			return 15;
		}
	}

	[behaviac.GeneratedTypeMetaInfo()]
	class Assignment_bt_EnemyAvartar_node11 : behaviac.Assignment
	{
		public Assignment_bt_EnemyAvartar_node11()
		{
		}
		protected override EBTStatus update_impl(behaviac.Agent pAgent, behaviac.EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			float opr = 1f;
			pAgent.SetVariable<float>("attackParam", 2100782252u, opr);
			return result;
		}
	}

	public static class bt_EnemyAvartar
	{
		public static bool build_behavior_tree(BehaviorTree bt)
		{
			bt.SetClassNameString("BehaviorTree");
			bt.SetId(-1);
			bt.SetName("EnemyAvartar");
			bt.IsFSM = false;
#if !BEHAVIAC_RELEASE
			bt.SetAgentType("EnemyAgent");
#endif
			// children
			{
				Parallel_bt_EnemyAvartar_node0 node0 = new Parallel_bt_EnemyAvartar_node0();
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
						WaitforSignal node12 = new WaitforSignal();
						node12.SetClassNameString("WaitforSignal");
						node12.SetId(12);
#if !BEHAVIAC_RELEASE
						node12.SetAgentType("EnemyAgent");
#endif
						node1.AddChild(node12);
						{
							Condition_bt_EnemyAvartar_node2 node2 = new Condition_bt_EnemyAvartar_node2();
							node2.SetClassNameString("Condition");
							node2.SetId(2);
#if !BEHAVIAC_RELEASE
							node2.SetAgentType("EnemyAgent");
#endif
							node12.SetCustomCondition(node2);
							node12.SetHasEvents(node12.HasEvents() | node2.HasEvents());
						}
						node1.SetHasEvents(node1.HasEvents() | node12.HasEvents());
					}
					{
						Action_bt_EnemyAvartar_node3 node3 = new Action_bt_EnemyAvartar_node3();
						node3.SetClassNameString("Action");
						node3.SetId(3);
#if !BEHAVIAC_RELEASE
						node3.SetAgentType("EnemyAgent");
#endif
						node1.AddChild(node3);
						node1.SetHasEvents(node1.HasEvents() | node3.HasEvents());
					}
					{
						Action_bt_EnemyAvartar_node4 node4 = new Action_bt_EnemyAvartar_node4();
						node4.SetClassNameString("Action");
						node4.SetId(4);
#if !BEHAVIAC_RELEASE
						node4.SetAgentType("EnemyAgent");
#endif
						node1.AddChild(node4);
						node1.SetHasEvents(node1.HasEvents() | node4.HasEvents());
					}
					{
						SelectorProbability_bt_EnemyAvartar_node5 node5 = new SelectorProbability_bt_EnemyAvartar_node5();
						node5.SetClassNameString("SelectorProbability");
						node5.SetId(5);
#if !BEHAVIAC_RELEASE
						node5.SetAgentType("EnemyAgent");
#endif
						node1.AddChild(node5);
						{
							DecoratorWeight_bt_EnemyAvartar_node6 node6 = new DecoratorWeight_bt_EnemyAvartar_node6();
							node6.SetClassNameString("DecoratorWeight");
							node6.SetId(6);
#if !BEHAVIAC_RELEASE
							node6.SetAgentType("EnemyAgent");
#endif
							node5.AddChild(node6);
							{
								Assignment_bt_EnemyAvartar_node9 node9 = new Assignment_bt_EnemyAvartar_node9();
								node9.SetClassNameString("Assignment");
								node9.SetId(9);
#if !BEHAVIAC_RELEASE
								node9.SetAgentType("EnemyAgent");
#endif
								node6.AddChild(node9);
								node6.SetHasEvents(node6.HasEvents() | node9.HasEvents());
							}
							node5.SetHasEvents(node5.HasEvents() | node6.HasEvents());
						}
						{
							DecoratorWeight_bt_EnemyAvartar_node7 node7 = new DecoratorWeight_bt_EnemyAvartar_node7();
							node7.SetClassNameString("DecoratorWeight");
							node7.SetId(7);
#if !BEHAVIAC_RELEASE
							node7.SetAgentType("EnemyAgent");
#endif
							node5.AddChild(node7);
							{
								Assignment_bt_EnemyAvartar_node8 node8 = new Assignment_bt_EnemyAvartar_node8();
								node8.SetClassNameString("Assignment");
								node8.SetId(8);
#if !BEHAVIAC_RELEASE
								node8.SetAgentType("EnemyAgent");
#endif
								node7.AddChild(node8);
								node7.SetHasEvents(node7.HasEvents() | node8.HasEvents());
							}
							node5.SetHasEvents(node5.HasEvents() | node7.HasEvents());
						}
						{
							DecoratorWeight_bt_EnemyAvartar_node10 node10 = new DecoratorWeight_bt_EnemyAvartar_node10();
							node10.SetClassNameString("DecoratorWeight");
							node10.SetId(10);
#if !BEHAVIAC_RELEASE
							node10.SetAgentType("EnemyAgent");
#endif
							node5.AddChild(node10);
							{
								Assignment_bt_EnemyAvartar_node11 node11 = new Assignment_bt_EnemyAvartar_node11();
								node11.SetClassNameString("Assignment");
								node11.SetId(11);
#if !BEHAVIAC_RELEASE
								node11.SetAgentType("EnemyAgent");
#endif
								node10.AddChild(node11);
								node10.SetHasEvents(node10.HasEvents() | node11.HasEvents());
							}
							node5.SetHasEvents(node5.HasEvents() | node10.HasEvents());
						}
						node1.SetHasEvents(node1.HasEvents() | node5.HasEvents());
					}
					node0.SetHasEvents(node0.HasEvents() | node1.HasEvents());
				}
				bt.SetHasEvents(bt.HasEvents() | node0.HasEvents());
			}
			return true;
		}
	}

}
