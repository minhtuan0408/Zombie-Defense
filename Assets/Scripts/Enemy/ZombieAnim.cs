using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAnim : MonoBehaviour
{
	public ZombieAI zombie;
	public BossAI boss;
	public void OnAttackFinish()
	{
		zombie.OnAttackFinish();

	}
	public void BossOnAttack()
	{
		boss.TriggerDamageRegion();
		
	}
	public void BossOnAttackFinish()
	{
		boss.OnAttackFinish();
	}
}
