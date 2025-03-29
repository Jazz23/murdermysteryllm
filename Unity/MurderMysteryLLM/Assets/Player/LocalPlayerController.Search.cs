using UnityEngine;

public partial class LocalPlayerController
{
    public void Search(GameObject obj)
    {
        var clue = obj.GetComponent<Searchable>().Search();
        CluesFound.Add(clue);
        Clue.UpdateClueStickys(CluesFound);
        Clue.DisplayClue(clue);
    }
}