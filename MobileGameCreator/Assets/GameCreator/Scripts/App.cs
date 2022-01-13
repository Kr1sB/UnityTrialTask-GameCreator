using UnityEngine;
using GameCreator.Creator;
using GameCreator.Game;
using GameCreator.Model;

namespace GameCreator
{
    public class App : MonoBehaviour
    {
        private enum Mode
        {
            Init,
            Creator,
            Game
        }

        public static App instance { get; private set; }

        public Rendering.ScreenFader screenFader;
        public float modeSwitchFadeDuration = .3f;

        public bool isInModeTransition { get; private set; }
        private Mode mode = Mode.Init;

        public bool isInteractionLocked { get; private set; }
        public event System.Action<bool> onInteractionLockChanged;

        public GameProject gameProject;

        public void LockInteraction(bool locked = true)
        {
            isInteractionLocked = locked;
            onInteractionLockChanged?.Invoke(locked);
        }

        public void UnlockInteraction() =>
            LockInteraction(false);

        private void Awake()
        {
            if (instance != null)
                throw new System.Exception("Multiple instances of " + GetType());

            instance = this;
        }

        private void Start()
        {
            Application.targetFrameRate = 60;
            gameProject = GameProject.Default();
            EnterModeImmediate(Mode.Creator);
            screenFader.FadeIn(modeSwitchFadeDuration);
        }



        private void EnterMode(Mode newMode)
        {
            LockInteraction();
            isInModeTransition = true;

            screenFader.FadeOut(modeSwitchFadeDuration, () =>
            {
                EnterModeImmediate(newMode);
                isInModeTransition = false;

                screenFader.FadeIn(modeSwitchFadeDuration);
                UnlockInteraction();

            });
        }

        private void EnterModeImmediate(Mode newMode)
        {
            if (mode == newMode)
                return;

            switch (mode)
            {
                case Mode.Game:
                    GameController.instance.OnExit();
                    break;

                case Mode.Creator:
                    CreatorController.instance.OnExit();
                    break;
            }

            mode = newMode;

            switch (newMode)
            {
                case Mode.Creator:
                    CreatorController.instance.OnEnter();
                    CreatorController.instance.LoadProject(gameProject);
                    break;

                case Mode.Game:
                    GameController.instance.OnEnter();
                    GameController.instance.LoadProject(gameProject);
                    break;

                default:
                    throw new System.Exception("Unsupported mode: " + mode);
            }
        }


        public void SwitchToGame() =>
            EnterMode(Mode.Game);

        public void SwitchToCreator() =>
            EnterMode(Mode.Creator);


        public void EnableScreenSleep(bool enable = true)
        {
            if (enable)
                Screen.sleepTimeout = SleepTimeout.SystemSetting;
            else
                Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }
    }
}