using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
	public static ObjectPool Instance;

	public GameObject prefab;
	public int poolSize = 50;

	private Queue<GameObject> pool = new Queue<GameObject>();

	void Awake()
	{
		Instance = this;

		for (int i = 0; i < poolSize; i++)
		{
			GameObject obj = Instantiate(prefab);
			obj.SetActive(false);
			pool.Enqueue(obj);
		}
	}

	public GameObject GetObject()
	{
		if (pool.Count > 0)
		{
			GameObject obj = pool.Dequeue();
			obj.SetActive(true);
			return obj;
		}

		GameObject newObj = Instantiate(prefab);
		return newObj;
	}

	public void ReturnObject(GameObject obj)
	{
		obj.SetActive(false);
		pool.Enqueue(obj);
	}
}