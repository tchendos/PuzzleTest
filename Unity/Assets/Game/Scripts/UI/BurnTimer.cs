using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BurnTimer:MonoBehaviour
{
	Animator _animator;
	float _timeOut;
	EndScreen _endScreen;

	public void Init(float timeOut, EndScreen endScreen)
	{
		_timeOut = timeOut;
		_endScreen = endScreen;

		gameObject.SetActive(true);

		_animator = GetComponent<Animator>();
		_animator.speed = 1.0f / timeOut;
	}
	public float GetTimeOut()
	{
		return _timeOut;
	}

	const float BOOM_TIME = 0.2f;
	public IEnumerator Boom(int score)
	{
		_animator.speed = 1.0f;
		_animator.Play("Boom");

		_endScreen.Init(score, BOOM_TIME);

		yield return new WaitForSeconds(BOOM_TIME);
		gameObject.SetActive(false);
	}
}
