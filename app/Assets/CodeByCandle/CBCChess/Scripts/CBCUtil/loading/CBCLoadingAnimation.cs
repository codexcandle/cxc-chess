using UnityEngine;
using System.Collections;

public class CBCLoadingAnimation:MonoBehaviour
{
	public class LoadingAnimation:MonoBehaviour
	{
		Animator loadingAnimation;

		void Awake()
		{
			loadingAnimation = GetComponent<Animator>();
		}

		void Start()
		{
			loadingAnimation.SetBool("Loading", true);

			StartCoroutine(LoadLevel());
		}

		// A simple coroutine to wait 3 seconds and load the level
		IEnumerator LoadLevel()
		{
			for (int i = 0; i < 3; i++)
			{
				yield return new WaitForSeconds(1);
			}

			loadingAnimation.SetBool("Loading", false);

			//Application.LoadLevel("Main Menu");
		}
	}
}