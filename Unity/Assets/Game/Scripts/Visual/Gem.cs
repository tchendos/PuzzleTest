using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/**
 * Visual representation of the Gem.
 * It can be animated in time (using Tweener classes).
 * It features moving flag, which indicates it is still in motion
 * and should not be used for matching the gems.
 */
public class Gem : MonoBehaviour 
{
	TweenPosition _positionTween;

	bool _moving;
	int _type;

	GameObject _visualPrefab;
	Cache _cache;

	public void Init(Cache cache, Vector2i gridPos, int gemType)
	{
		_cache = cache;

		SetPosition(null, gridPos);

		SetGemType(gemType);
	}

	public void SetPosition(Gem bottomGem, Vector2i gridPos)
	{
		Vector3 position = CalculatePosition(gridPos);
		if (bottomGem != null)
		{
			Vector3 nextPosition = bottomGem.transform.localPosition + new Vector3(0, ArtData.FIELD_GRID_Y, 0);
			if (position.y < nextPosition.y)
				position = nextPosition;
		}

		TweenPosition.Begin(gameObject, 0, position);
	}

	public float MoveTo(Vector2i gridPos, float velocity)
	{
		Vector3 position = CalculatePosition(gridPos);
		
		float duration = (transform.localPosition - position).magnitude / velocity;
		_positionTween = TweenPosition.Begin(gameObject, duration, position);
		return duration;
	}
	
	public void SetMoving(bool state)
	{
		_moving = state;
	}

	public bool IsMoving()
	{
		return _moving;
	}

	public bool InPlace()
	{
		return _moving && !_positionTween.enabled;
	}

	public void FadeOut(float time)
	{
		StartCoroutine(FadeOutCoroutine(time));
	}

	public IEnumerator FadeOutCoroutine(float time)
	{
		GameObject go = (GameObject)GameObject.Instantiate(gameObject);
		go.transform.parent = transform.parent;
		go.transform.localPosition = transform.localPosition;
		go.transform.localScale = transform.localScale;
		SortingLayer.ForceLayerID(go, "GemsBack");

		TweenColor.BeginOnChildren(go, time, new Color (1, 1, 1, 0), Tweener.Method.EaseIn);
		TweenScale tweenScale = TweenScale.Begin(go, time, Vector3.one / 4);
		tweenScale.method = Tweener.Method.EaseIn;

		yield return new WaitForSeconds(time);

		GameObject.Destroy(go);
	}
	
	public void SetGemType(int typeIndex)
	{
		if (typeIndex != _type)
		{
			if (_visualPrefab != null)
				_cache.ReturnGemPrefab(_type, _visualPrefab);
			
			_type = typeIndex;
			
			_visualPrefab = _cache.LendGemPrefab(_type, transform);
		}
	}
	
	void Awake()
	{
		_type = -1;
		_positionTween = gameObject.GetComponent<TweenPosition>();
		
		if (_positionTween == null)
		{
			_positionTween = gameObject.AddComponent<TweenPosition>();
			_positionTween.enabled = false;
			_positionTween.method = Tweener.Method.Linear;
			_positionTween.steeperCurves = false;
		}
	}
	
	static Vector3 CalculatePosition(Vector2i gridPos)
	{
		return new Vector3(-ArtData.FIELD_GRID_X * ArtData.FIELD_SIZE_X / 2 + ArtData.FIELD_GRID_X / 2 + gridPos.x * ArtData.FIELD_GRID_X, 
		                   ArtData.FIELD_GRID_Y * ArtData.FIELD_SIZE_Y / 2 - ArtData.FIELD_GRID_Y / 2 - gridPos.y * ArtData.FIELD_GRID_Y, 0);
	}
}
