#if IPA
using IllusionInjector;
#else
using BepInEx;
using UnityEngine.SceneManagement;
#endif
using System;
using UnityEngine;
using System.Reflection;

namespace ToolBox
{
    public abstract class GenericPlugin
#if BEPINEX
       : BaseUnityPlugin
#endif
    {
        internal Binary _binary;
        internal int _level = -1;
#if IPA
        private static PluginComponent _pluginComponent;
        private Component _onGUIDispatcher = null;

        public abstract string Name { get; }
        public abstract string Version { get; }
        public abstract string[] Filter { get; }
        public GameObject gameObject
        {
            get
            {
                if (_pluginComponent == null)
                    _pluginComponent = UnityEngine.Object.FindObjectOfType<PluginComponent>();
                return _pluginComponent.gameObject;
            }
        }

        public void OnApplicationStart()
        {
            this.Awake();
        }

        public void OnApplicationQuit()
        {
            this.OnDestroy();
        }

        public void OnLevelWasInitialized(int level)
        {
        }

        public void OnLevelWasLoaded(int level)
        {
            this._level = level;
            this.LevelLoaded(level);
        }

        public void OnUpdate()
        {
            this.Update();
        }

        public void OnFixedUpdate()
        {
            this.FixedUpdate();
        }

        public void OnLateUpdate()
        {
            this.LateUpdate();
        }
#endif

        protected virtual void Awake()
        {
            UnityEngine.Debug.LogError("Toolbox.GenericPlugin.Awake" + Application.productName);
#if BEPINEX
            SceneManager.sceneLoaded += this.LevelLoaded; 
#endif
            switch (Application.productName)
            {
#if HONEYSELECT
                case "HoneySelect":
                case "Honey Select Unlimited":
#elif KOIKATSU
                case "Koikatsu Party":
                case "Koikatu":
#elif AISHOUJO
                case "AI-Syoujyo":
#elif PLAYHOME
                case "PlayHome":
#elif HONEYSELECT2
                case "HoneySelect2":
#endif
                    this._binary = Binary.Game;
                    break;
#if HONEYSELECT
                case "StudioNEO":
#elif KOIKATSU
                case "CharaStudio":
#elif AISHOUJO || HONEYSELECT2
                case "StudioNEOV2":
#elif PLAYHOME
                case "PlayHomeStudio":
#endif
                default:
                    this._binary = Binary.Studio;
                    break;
            }
#if IPA

            Component[] components = this.gameObject.GetComponents<Component>();
            foreach (Component c in components)
            {
                if (c.GetType().Name == nameof(OnGUIDispatcher))
                {
                    this._onGUIDispatcher = c;
                    break;
                }
            }
            if (this._onGUIDispatcher == null)
                this._onGUIDispatcher = this.gameObject.gameObject.AddComponent<OnGUIDispatcher>();
            this._onGUIDispatcher.GetType().GetMethod(nameof(OnGUIDispatcher.AddListener), BindingFlags.Instance | BindingFlags.Public).Invoke(this._onGUIDispatcher, new object[]{new Action(this.OnGUI)});
#endif
        }

        protected virtual void OnDestroy()
        {
#if IPA
            if (this._onGUIDispatcher != null)
            this._onGUIDispatcher.GetType().GetMethod(nameof(OnGUIDispatcher.RemoveListener), BindingFlags.Instance | BindingFlags.Public).Invoke(this._onGUIDispatcher, new object[]{new Action(this.OnGUI)});
#endif
        }

        protected virtual void LevelLoaded(int level)
        {
        }

#if BEPINEX
        protected virtual void LevelLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
        {
            if (mode == LoadSceneMode.Single)
            {
                this._level = scene.buildIndex;
                this.LevelLoaded(scene.buildIndex);
            }
        }
#endif

        protected virtual void Update()
        {

        }

        protected virtual void LateUpdate()
        {

        }

        protected virtual void FixedUpdate()
        {

        }

        protected virtual void OnGUI()
        {

        }
    }

#if IPA
    internal class OnGUIDispatcher : MonoBehaviour
    {
        private event Action _onGUI;

        public void AddListener(Action listener)
        {
            this._onGUI += listener;
        }

        public void RemoveListener(Action listener)
        {
            this._onGUI -= listener;
        }

        private void OnGUI()
        {
            if (this._onGUI != null)
                this._onGUI();
        }
    }
#endif

    public enum Binary
    {
        Game,
        Studio
    }
}
