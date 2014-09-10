using UnityEngine;

public class EndScreen:MonoBehaviour
{
	public void Init(int score, float duration)
	{
		gameObject.SetActive(true);
		
		TweenColor.BeginOnChildren(gameObject, 0, new Color(1,1,1,0), Tweener.Method.Linear);
		TweenColor.BeginOnChildren(gameObject, duration, new Color(1,1,1,0.9f), Tweener.Method.Linear);
		
		TextMesh scoreMesh = GetComponentInChildren<TextMesh>();
		scoreMesh.text = "Score: " + score.ToString();
	}
}
