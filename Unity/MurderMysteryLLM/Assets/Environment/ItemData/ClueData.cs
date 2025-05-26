using UnityEngine;

[CreateAssetMenu(fileName = "ClueData", menuName = "Scriptable Objects/ClueData")]
public class ClueData : ItemData
{

	[SerializeField]
	private bool _is_red_herring;

	public bool Is_red_herring { get; set; }


}
