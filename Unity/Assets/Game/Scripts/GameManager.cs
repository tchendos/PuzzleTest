using UnityEngine;
using System.Collections;

/**
 * Game objects inititator. It must be attached to some object in the scene
 */
public class GameManager:MonoBehaviour
{
	void Awake()
	{
		ArtData artData = ArtData.Instance;

		LevelData levelData = new LevelData(artData, 0);
		Cache cache = new Cache(levelData);

		Logic logic = new Logic(levelData);
		Controller controller = FindOrAddComponent<Controller>();
		Visual visual = new Visual(cache, artData);
		Simulation simulation = FindOrAddComponent<Simulation>();
	
		controller.Init(artData.MainCamera, artData.FieldRoot, simulation);
		artData.Timer.Init(levelData.GetTimeOut(), artData.EndScreen);

		simulation.Init(logic, visual, artData.Timer);
	}

	T FindOrAddComponent<T>() where T : Component
	{
		T cmp = gameObject.GetComponent<T>();
		if (cmp == null)
			cmp = gameObject.AddComponent<T>();
		return cmp;
	}
}
