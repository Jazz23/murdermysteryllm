using UnityEngine;
using UnityEngine.InputSystem.Utilities;

[CreateAssetMenu(fileName = "ClueData", menuName = "Scriptable Objects/ClueData")]
public class ClueData : ScriptableObject
{
    [SerializeField]
    private string _name;

    public string Name { get; set; }

    [SerializeField]

    private Sprite _sprite;

    [SerializeField]
    [TextArea(3, 5)]
    private string _description;
    
    public string Description { get; set;}



}
