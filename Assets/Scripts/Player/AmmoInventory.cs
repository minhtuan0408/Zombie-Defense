using UnityEngine;
public enum AmmoType
{
	Type1,
	Type2,
	Type3
}
public class AmmoInventory : MonoBehaviour
{
	public int ammoType1;
	public int ammoType2;
	public int ammoType3;

	public void AddAmmo(AmmoType type, int amount)
	{
		switch (type)
		{
			case AmmoType.Type1:
				ammoType1 += amount;
				break;

			case AmmoType.Type2:
				ammoType2 += amount;
				break;

			case AmmoType.Type3:
				ammoType3 += amount;
				break;
		}
	}

	public bool UseAmmo(AmmoType type, int amount)
	{
		switch (type)
		{
			case AmmoType.Type1:
				if (ammoType1 < amount) return false;
				ammoType1 -= amount;
				return true;

			case AmmoType.Type2:
				if (ammoType2 < amount) return false;
				ammoType2 -= amount;
				return true;

			case AmmoType.Type3:
				if (ammoType3 < amount) return false;
				ammoType3 -= amount;
				return true;
		}

		return false;
	}
}