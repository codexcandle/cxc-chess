// ------------------------------------------------------------------------------
//  taken from "unity live training session" - 
//  http://unity3d.com/learn/tutorials/modules/beginner/live-training-archive/object-pooling
// ------------------------------------------------------------------------------
using System;
using UnityEngine;
using System.Collections.Generic;

// TODO - refactor this class; possibly include "PrefabNames.cs" class from "config" dir
namespace cbc.cbcchess
{
	public class PieceManager:MonoBehaviour
	{
		public static PieceManager current;

		#region TEAM PIECES #1
		// ... pawns
		public GameObject pooledLightPawnObject;
		public const int pooledLightPawnAmount = 8;
		public const bool willLightPawnCountGrow = false;
		List<GameObject> pooledLightPawnObjects;

		// ... knights
		public GameObject pooledLightKnightObject;
		public const int pooledLightKnightAmount = 2;
		public const bool willLightKnightCountGrow = false;
		List<GameObject> pooledLightKnightObjects;

		// ... bishop
		public GameObject pooledLightBishopObject;
		public const int pooledLightBishopAmount = 2;
		public const bool willLightBishopCountGrow = false;
		List<GameObject> pooledLightBishopObjects;

		// ... rook
		public GameObject pooledLightRookObject;
		public const int pooledLightRookAmount = 2;
		public const bool willLightRookpCountGrow = false;
		List<GameObject> pooledLightRookObjects;

		// ... queen
		public GameObject pooledLightQueenObject;
		public const int pooledLightQueenAmount = 1;
		public const bool willLightQueenCountGrow = false;
		List<GameObject> pooledLightQueenObjects;

		// ... king
		public GameObject pooledLightKingObject;
		public const int pooledLightKingAmount = 1;
		public const bool willLightKingCountGrow = false;
		List<GameObject> pooledLightKingObjects;
		#endregion


		#region TEAM PIECES #2
		// ... pawns
		public GameObject pooledDarkPawnObject;
		public const int pooledDarkPawnAmount = 8;
		public const bool willDarkPawnCountGrow = false;
		List<GameObject> pooledDarkPawnObjects;

		// ... knights
		public GameObject pooledDarkKnightObject;
		public const int pooledDarkKnightAmount = 2;
		public const bool willDarkKnightCountGrow = false;
		List<GameObject> pooledDarkKnightObjects;

		// ... bishop
		public GameObject pooledDarkBishopObject;
		public const int pooledDarkBishopAmount = 2;
		public const bool willDarkBishopCountGrow = false;
		List<GameObject> pooledDarkBishopObjects;

		// ... rook
		public GameObject pooledDarkRookObject;
		public const int pooledDarkRookAmount = 2;
		public const bool willDarkRookpCountGrow = false;
		List<GameObject> pooledDarkRookObjects;
		
		// ... queen
		public GameObject pooledDarkQueenObject;
		public const int pooledDarkQueenAmount = 1;
		public const bool willDarkQueenCountGrow = false;
		List<GameObject> pooledDarkQueenObjects;
		
		// ... king
		public GameObject pooledDarkKingObject;
		public const int pooledDarkKingAmount = 1;
		public const bool willDarkKingCountGrow = false;
		List<GameObject> pooledDarkKingObjects;
		#endregion

		public GameObject GetPooledPiece(int team, string piecePrefabID)
		{
			GameObject go = GetPiecePrefabFromID(piecePrefabID);
			
			List<GameObject> pool = GetPiecePoolFromID(piecePrefabID);
			int count = pool.Count;
			for(int i = 0; i < count; i++)
			{
				GameObject goPooled = pool[i];
				if(!goPooled.activeInHierarchy)
				{
					return goPooled;
				}
			}
			
			// TODO - revisit below; should object pool grow - insert class-values per piece type
			if(true/*willLightPawnCountGrow*/)
			{
				GameObject obj = (GameObject)Instantiate(go);
				pool.Add(obj);
				
				return obj;
			}
			
			return null;
		}

		void Awake()
		{
			current = this;
		}
		
		void Start()
		{
			// POOL TEAM #1 ///////////////////////////////////////
			// pawn
			pooledLightPawnObjects = GetPooledObjects(pooledLightPawnObject, pooledLightPawnAmount);
			
			// knight
			pooledLightKnightObjects = GetPooledObjects(pooledLightKnightObject, pooledLightKnightAmount);
			
			// bishop
			pooledLightBishopObjects = GetPooledObjects(pooledLightBishopObject, pooledLightBishopAmount);				
			
			// rook
			pooledLightRookObjects = GetPooledObjects(pooledLightRookObject, pooledLightRookAmount);
			
			// queen
			pooledLightQueenObjects = GetPooledObjects(pooledLightQueenObject, pooledLightQueenAmount);
			
			// king
			pooledLightKingObjects = GetPooledObjects(pooledLightKingObject, pooledLightKingAmount);
			///////////////////////////////////////////////////////

			// POOL TEAM #2 ///////////////////////////////////////
			// pawn
			pooledDarkPawnObjects = GetPooledObjects(pooledDarkPawnObject, pooledDarkPawnAmount);
			
			// knight
			pooledDarkKnightObjects = GetPooledObjects(pooledDarkKnightObject, pooledDarkKnightAmount);
			
			// bishop
			pooledDarkBishopObjects = GetPooledObjects(pooledDarkBishopObject, pooledDarkBishopAmount);				
			
			// rook
			pooledDarkRookObjects = GetPooledObjects(pooledDarkRookObject, pooledDarkRookAmount);
			
			// queen
			pooledDarkQueenObjects = GetPooledObjects(pooledDarkQueenObject, pooledDarkQueenAmount);
			
			// king
			pooledDarkKingObjects = GetPooledObjects(pooledDarkKingObject, pooledDarkKingAmount);
			///////////////////////////////////////////////////////
		}

		private List<GameObject> GetPooledObjects(GameObject prefab, int count)
		{
			List<GameObject> gos = new List<GameObject>();
			for(int i = 0; i < count; i++)
			{
				GameObject go = (GameObject)Instantiate(prefab);
				go.SetActive(false);

				gos.Add(go);
			}

			return gos;
		}

		private GameObject GetPiecePrefabFromID(string piecePrefabID)
		{
			switch(piecePrefabID)
			{
				// team #1
				case PrefabNames.PAWN_LIGHT:
					return pooledLightPawnObject;
				case PrefabNames.KNIGHT_LIGHT:
					return pooledLightKnightObject;				
				case PrefabNames.BISHOP_LIGHT:
					return pooledLightBishopObject;
				case PrefabNames.ROOK_LIGHT:
					return pooledLightRookObject;	
				case PrefabNames.QUEEN_LIGHT:
					return pooledLightQueenObject;
				case PrefabNames.KING_LIGHT:
					return pooledLightKingObject;

				// team #2
				case PrefabNames.PAWN_DARK:
					return pooledDarkPawnObject;
				case PrefabNames.KNIGHT_DARK:
					return pooledDarkKnightObject;				
				case PrefabNames.BISHOP_DARK:
					return pooledDarkBishopObject;
				case PrefabNames.ROOK_DARK:
					return pooledDarkRookObject;	
				case PrefabNames.QUEEN_DARK:
					return pooledDarkQueenObject;
				case PrefabNames.KING_DARK:
					return pooledDarkKingObject;
			}

			return null;
		}

		private List<GameObject> GetPiecePoolFromID(string piecePrefabID)
		{
			switch(piecePrefabID)
			{
				// team #1
				case PrefabNames.PAWN_LIGHT:
					return pooledLightPawnObjects;
				case PrefabNames.KNIGHT_LIGHT:
					return pooledLightKnightObjects;				
				case PrefabNames.BISHOP_LIGHT:
					return pooledLightBishopObjects;
				case PrefabNames.ROOK_LIGHT:
					return pooledLightRookObjects;	
				case PrefabNames.QUEEN_LIGHT:
					return pooledLightQueenObjects;
				case PrefabNames.KING_LIGHT:
					return pooledLightKingObjects;
				
				// team #2
				case PrefabNames.PAWN_DARK:
					return pooledDarkPawnObjects;
				case PrefabNames.KNIGHT_DARK:
					return pooledDarkKnightObjects;				
				case PrefabNames.BISHOP_DARK:
					return pooledDarkBishopObjects;
				case PrefabNames.ROOK_DARK:
					return pooledDarkRookObjects;	
				case PrefabNames.QUEEN_DARK:
					return pooledDarkQueenObjects;
				case PrefabNames.KING_DARK:
					return pooledDarkKingObjects;
			}
			
			return null;
		}
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

