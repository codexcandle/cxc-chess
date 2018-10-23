// ------------------------------------------------------------------------------
//  taken from "unity live training session" - 
//  http://unity3d.com/learn/tutorials/modules/beginner/live-training-archive/object-pooling
// ------------------------------------------------------------------------------
using System;
using UnityEngine;
using System.Collections.Generic;

public class ObjectPooler:MonoBehaviour
{
	public static ObjectPooler current;
	public GameObject pooledObject;
	public int pooledAmount = 20;
	public bool willGrow = false;

	List<GameObject> pooledObjects;

	void Awake()
	{
		current = this;
	}

	void Start()
	{
		pooledObjects = new List<GameObject>();
		for(int i = 0; i < pooledAmount; i++)
		{
			GameObject obj = (GameObject)Instantiate (pooledObject);
			obj.SetActive(false);
			pooledObjects.Add(obj);
		}

		Debug.Log("test @ ObjectPooler");
	}

	public GameObject GetPooledObject()
	{
		for(int i = 0; i < pooledObjects.Count; i++)
		{
			if(!pooledObjects[i].activeInHierarchy)
			{
				return pooledObjects[i];
			}
		}

		if(willGrow)
		{
			GameObject obj = (GameObject)Instantiate(pooledObject);
			pooledObjects.Add(obj);

			return obj;
		}

		return null;
	}
}



/*
 * e.g. use in class like below...
 * 
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ButtletFireScript:MonoBehaviour
{
	public float fireTime = 0.05f;

	void Start()
	{
		InvokeRepeating("Fire", fireTime, fireTime);
	}

	void Fire()
	{
		GameObject obj = ObjectPooler.current.GetPooledObject();

		if(obj == null) return;

		obj.transform.position = transform.position;
		obj.transform.rotation = transform.rotation;
		obj.SetActive (true);
	}
}
*/

/*
 * 
 *  * e.g. use in "destroy" call like below...
 * 
 * 
using UnityEngine;
using System.Collections;

public class BulletDestroyScript:MonoBheaviour
{
	void OnEnable()
	{
		Invoke("Destroy", 2f);
	}

	void Destroy()
	{
		gameObject.SetActive(false);
	}

	void OnDisable()
	{
		CancelInvoke();
	}
}
*/

