using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
	protected bool isDestroyed = false;

	private static T m_instance;
	public static T Instance
	{
		get
		{
			return m_instance;
		}
	}

	protected virtual void Awake()
	{
		if(m_instance == null)
		{
			m_instance = this as T;
			DontDestroyOnLoad(this);
		}
		else
		{
			isDestroyed = true;
			Destroy(gameObject);
		}
	}
}