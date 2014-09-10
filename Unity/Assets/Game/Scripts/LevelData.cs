using UnityEngine;
using System;
using System.Collections.Generic;

/**
 * Game data structures definition. It is supposed to be edited by game designers. 
 * 
 * It features definition of the gems, timeOut, gem set for generating gem field and 
 * set for generating new gems after collapsing.
 * 
 * It should be exposed in some game data editor in the scene, although for the 
 * purposes of the test I have chosen to omit this part.
 */
public class LevelData
{
	public enum GemType
	{
		// standard type of gem; data defines color of the gem
		Normal
	}
	public struct GemTypeInfo
	{
		// visual representation of the gem
		public GameObject prefab;
		// gem gameplay mechanic
		public GemType type;
		// data for specific gameplay mechanic
		public int data;
		// gem can be matched? (data contains color)
		public bool colored;
	}

	//  gem set for generating gem field at the start of the game
	HashSet<int> _startGemIndices;
	//  gem set for generating new gems after collapsing
	HashSet<int> _basicGemIndices;

	int _gemTypeCount;
	GemTypeInfo[] _gemTypeInfo;

	// level time limit in seconds
	float _timeOut;

	public LevelData(ArtData artData, int levelIndex)
	{
		initLevel(artData, 0);
	}

	public HashSet<int> GetBasicGemIndices() { return _basicGemIndices; }
	public HashSet<int> GetStartGemIndices() { return _startGemIndices; }
	
	public GemTypeInfo GetGemTypeInfo(int type) { return _gemTypeInfo[type]; }
	public int GetGemTypeCount() { return _gemTypeCount; }
	
	public float GetTimeOut() { return _timeOut; }

	void initLevel(ArtData artData, int levelIndex)
	{
		switch(levelIndex)
		{
		case 0:
			initLevel1(artData); break;
		default:
			DebugUtils.Assert(false);
			break;
		}
	}

	void initLevel1(ArtData artData)
	{
		initLevelBase(artData);
	}
	
	void initLevelBase(ArtData artData)
	{
		_timeOut = 60.0f;

		_basicGemIndices = new HashSet<int>();
		_startGemIndices = new HashSet<int>();

		_gemTypeCount = artData.GemNormalPrefabs.Length;
		_gemTypeInfo = new GemTypeInfo[_gemTypeCount];
		int i = 0;
		for (i = 0; i < _gemTypeCount; i++)
		{
			_gemTypeInfo[i].prefab = artData.GemNormalPrefabs[i];
			_gemTypeInfo[i].type = GemType.Normal;
			_gemTypeInfo[i].data = i;
			_gemTypeInfo[i].colored = true;
			_basicGemIndices.Add(i); _startGemIndices.Add(i);
		}
	}
}
