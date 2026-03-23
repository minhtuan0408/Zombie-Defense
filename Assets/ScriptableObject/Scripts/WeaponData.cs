using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[CreateAssetMenu(fileName ="WeaponData", menuName ="Game/Weapon Data")]
public class WeaponData : ScriptableObject
{
	public string weaponName;
	public string maxBullet;
	public Texture icon;
	public BulletType bulletType;
}
