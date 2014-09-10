using UnityEngine;
using System;
using System.Collections.Generic;

/**
 * Caching of unused gem GameObjects so we don't have to create new ones 
 * for each gem type change.
 */
public class Cache
{
	LevelData _levelData;
	Dictionary<int,List<GameObject>> _gemPrefabs = new Dictionary<int,List<GameObject>>();

	public Cache(LevelData levelData) 
	{
		_levelData = levelData; 
	}

	public GameObject LendGemPrefab(int type, Transform parent)
	{
		List<GameObject> goList;
		GameObject go = null;
		if (_gemPrefabs.TryGetValue(type, out goList))
		{
			if (goList.Count > 0)
			{
				int index = goList.Count - 1;
				go = goList[index];
				goList.RemoveAt(index);
			}
		}
		bool correctScale = false;
		if (go == null)
		{
			go = GameObject.Instantiate(_levelData.GetGemTypeInfo(type).prefab) as GameObject;
			correctScale = true;
		}

		Vector3 scale = go.transform.localScale;
		go.SetActive(true);
		go.transform.parent = parent;
		go.transform.localPosition = Vector3.zero;
		go.transform.localRotation = Quaternion.identity;
		if (correctScale)
			go.transform.localScale = scale;

		return go;
	}

	public void ReturnGemPrefab(int type, GameObject go)
	{
		go.SetActive(false);
		go.transform.parent = null;

		List<GameObject> goList;
		if (_gemPrefabs.TryGetValue(type, out goList))
		{
			goList.Add(go);
		}
		else
		{
			_gemPrefabs.Add(type, new List<GameObject>() { go });
		}
	}
}
