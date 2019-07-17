using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParticlesEffect : MonoBehaviour
{
	public Type type;
	public Animator animator;
	public float duration;
	//public Text amount;

	float expirationTime;

	public void Init(int amount)
	{
		//this.amount.text = amount.ToString();
	}

	void Start()
	{
		if (type == Type.ParticleEffect)
		{
			expirationTime = Time.time + duration;
		}
	}
	void Update()
	{
		if(type == Type.ParticleEffect)
		{
			if (Time.time > expirationTime)
			{
				if (GetComponent<ParticleSystem>().isPlaying)
				{
					GetComponent<ParticleSystem>().Stop();
				}
				else if (GetComponent<ParticleSystem>().isStopped)
				{
					GameObject.Destroy(gameObject);
				}
			}
		}
		else
		{
			if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
			{
				GameObject.Destroy(gameObject);
			}
		}
	}

	public enum Type { Trail, ParticleEffect }
}