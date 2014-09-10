using UnityEngine;

public class ButtonRetry:Button
{
	override protected void Clicked()
	{
		Application.LoadLevel("Main");
	}
}
