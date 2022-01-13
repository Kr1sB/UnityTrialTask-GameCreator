using UnityEngine;

namespace GameCreator.Elements
{
    [CreateAssetMenu(
        fileName = "New Game Element Palette",
        menuName = "GameCreator/Game Element Palette"
    )]
    public class GameElementPalette : ScriptableObject
    {
        public GameElement[] elements;
    }
}