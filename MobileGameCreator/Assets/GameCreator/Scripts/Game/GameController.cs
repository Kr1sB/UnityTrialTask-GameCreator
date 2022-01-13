using GameCreator.Elements;
using GameCreator.Model;
using GameCreator.Weather;
using UnityEngine;


namespace GameCreator.Game
{
    public class GameController : MonoBehaviour
    {
        public static GameController instance { get; private set; }

        [SerializeField] private Transform elementContainer;

        public GameCamera _camera;
        public UI.GameUI ui;

        private void Awake()
        {
            if (instance != null)
                throw new System.Exception("Multiple instances of " + GetType());

            instance = this;

            if (elementContainer == null)
                throw new System.Exception("No element container assigned!");

            elementContainer.gameObject.SetActive(false);
            gameObject.SetActive(false);
            ui.Hide();
        }


        public void OnEnter()
        {
            gameObject.SetActive(true);
            elementContainer.gameObject.SetActive(true);

            App.instance.EnableScreenSleep(false);
            ui.Show();
            _camera.SetActive(true);
            WeatherController.instance.BindEffectBox(_camera.transform);
        }


        public void OnExit()
        {
            ClearObjects();
            elementContainer.gameObject.SetActive(false);

            ui.Hide();
            _camera.SetActive(false);

            gameObject.SetActive(false);
        }


        public void LoadProject(GameProject gameProject)
        {
            foreach(var prefab in gameProject.elements.Values)
            {
                GameElement e = NewElementInstance(prefab);
                e.SpawnInGame();
                e.gameObject.SetActive(true);
            }
        }

        private GameElement NewElementInstance(GameElement prefab)
        {
            GameElement e = Instantiate(prefab);
            e.transform.SetParent(elementContainer, true);
            return e;
        }

        private void ClearObjects()
        {
            while (elementContainer.childCount > 0)
            {
                Transform t = elementContainer.GetChild(0);
                t.parent = null;
                Destroy(t.gameObject);
            }
        }
    }
}