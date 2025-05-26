using UnityEngine;
using UnityEngine.UI;

public class Sticky : MonoBehaviour
{
	public string clueText;

	public void Awake()
	{
		GetComponent<Button>().onClick.AddListener(() =>
		{
			Clue.DisplayClue(clueText);
		});
	}
}