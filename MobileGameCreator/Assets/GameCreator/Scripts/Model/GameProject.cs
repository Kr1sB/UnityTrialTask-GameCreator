using System.Collections.Generic;
using GameCreator.Elements;


namespace GameCreator.Model
{
    public class GameProject
    {
        private uint idCounter;
        private uint nextId => ++idCounter;

        public Dictionary<uint, GameElement> elements = new Dictionary<uint, GameElement>();

        public void Add(GameElement element)
        {
            element.instanceId = nextId;
            elements[element.instanceId] = element;
        }

        public bool Remove(uint id) =>
            elements.Remove(id);

        public GameElement Get(uint id)
        {
            if (elements.TryGetValue(id, out GameElement element))
                return element;

            return null;
        }

        public bool ContainsAnyOf(GameElement.Category category)
        {
            foreach (var e in elements.Values)
                if (e.metadata.category == category)
                    return true;

            return false;
        }

        public static GameProject Default()
        {
            var p = new GameProject();
            return p;
        }
    }
}
