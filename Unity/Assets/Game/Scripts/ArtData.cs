using UnityEngine;
using System.Collections.Generic;

/**
 * Data defined in the scene. This is exposed to artists, so they can change visual easily.
 */
public class ArtData:MonoBehaviour
{
	// quick accessors
	public static int FIELD_SIZE_X { get { return Instance.FieldSizeX; } }
	public static int FIELD_SIZE_Y { get { return Instance.FieldSizeY; } }
	public static float FIELD_GRID_X { get { return Instance.FieldGridX; } }
	public static float FIELD_GRID_Y { get { return Instance.FieldGridY; } }

	public Camera MainCamera;

	public GameObject GemBasePrefab;

	public GameObject[] GemNormalPrefabs;

	public GameObject FieldRoot;

	public EndScreen EndScreen;
	public BurnTimer Timer;

	public float MatchFadeTime = 0.2f;
	public float CollapseFieldVelocity = 3.0f;
	public float SwapGemsVelocity = 3.0f;
	public float SwapGemsFailVelocity = 3.0f;

	public int FieldSizeX = 8;
	public int FieldSizeY = 8;
	public float FieldGridX = 3.0f/8;
	public float FieldGridY = 3.0f/8;

	public static ArtData Instance 
	{
		get
		{
			if (_instance == null) 
				_instance = GameObject.FindGameObjectWithTag("Root").GetComponent<ArtData>(); 

			return _instance;
		} 
	}
	static ArtData _instance;
}
