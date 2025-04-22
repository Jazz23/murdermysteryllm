using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]
public abstract class ItemData : ScriptableObject
{
	[SerializeField]
	private string _name;

	public string Name { get; set; }

	[SerializeField]
	private Sprite icon;

	private string _description;
	[TextArea]
	public string Description { get; set; }


}
