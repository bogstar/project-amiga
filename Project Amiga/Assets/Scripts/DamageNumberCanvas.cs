using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageNumberCanvas : MonoBehaviour
{
	public Text number;
	public float speed;
	public float timeout;

	float expirationTime;

	void Start()
	{
		expirationTime = timeout + Time.time;
	}

	void Update()
	{
		if(Time.time > expirationTime)
		{
			GameObject.Destroy(gameObject);
		}
		transform.position += Vector3.up * speed * Time.deltaTime;
	}

	public void Display(string text, Color color)
	{
		number.text = text;
		number.color = color;
	}
}