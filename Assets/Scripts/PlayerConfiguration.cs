using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Create Player")]
public class PlayerConfiguration : ScriptableObject
{
    public Sprite sprite;
    public RuntimeAnimatorController animatorController;

}
