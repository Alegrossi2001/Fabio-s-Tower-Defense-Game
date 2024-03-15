using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dypsloom.Shared
{
    /// <summary>
    /// A Toolbox class is used as a master singleton.
    /// </summary>
    public class Toolbox : MonoBehaviour
    {
        [SerializeField] protected bool m_DontDestroyOnLoad = false;
        
        private static bool _shuttingDown = false;
        private static object _lock = new object();
        private static Toolbox _instance;

        // Used to track any global components added at runtime.
        private Dictionary<Type, Dictionary<uint,object>> m_Dictionary 
            = new Dictionary<Type, Dictionary<uint,object>>();

        // Prevent constructor use.
        protected Toolbox()
        { }
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void InitializeOnLoadMethod()
        {
            _shuttingDown = false;
            _lock = new object();
            _instance = null;
        }

        private void Awake()
        {
            // Put initialization code here.
            if (Instance != this) {
                DestroyImmediate(gameObject);
                return;
            }
            
            if (m_DontDestroyOnLoad) {
                DontDestroyOnLoad(gameObject);
            }
            m_Dictionary = new Dictionary<Type, Dictionary<uint, object>>();
        }
        
        /// <summary>
        /// Access singleton instance through this propriety.
        /// </summary>
        public static Toolbox Instance
        {
            get
            {
                if (_shuttingDown)
                {
                    Debug.LogWarning("[Singleton] Instance '" + typeof(Toolbox) +
                                     "' already destroyed. Returning null.");
                    return null;
                }
 
                lock (_lock)
                {
                    if (_instance != null) { return _instance; }

                    // Search for existing instance.
#if UNITY_2021_3_OR_NEWER
                    _instance = FindFirstObjectByType<Toolbox>();
#else
                    _instance = FindObjectOfType<Toolbox>();
#endif
 
                    // Create new instance if one doesn't already exist.
                    if (_instance != null) { return _instance; }

                    // Need to create a new GameObject to attach the singleton to.
                    var singletonObject = new GameObject();
                    _instance = singletonObject.AddComponent<Toolbox>();

                    return _instance;
                }
            }
        }
 
        /// <summary>
        /// On Application Quit.
        /// </summary>
        private void OnApplicationQuit()
        {
            _shuttingDown = true;
        }

        /// <summary>
        /// Set an object inside the toolbox.
        /// </summary>
        /// <param name="obj">The object to set.</param>
        /// <param name="id">The id for that object.</param>
        /// <typeparam name="T">The type of that object.</typeparam>
        public static void Set<T>(T obj, uint id = 0)
        {
            var typeDictionary = Instance.m_Dictionary;
            var type = typeof(T);
            
            if (typeDictionary.ContainsKey(type)) {
                var objDictionary = typeDictionary[type];
                
                

                if (objDictionary.TryGetValue(id, out var value)) {

                    if (value != null ) {
                        Debug.LogWarning("[Toolbox] Global component of type <"+typeof(T).Name+"> ID \""
                                         +id + "\" already exist!" );
                        return;
                    }
                    
                }
                
                objDictionary.Add(id, obj);
                typeDictionary[type] = objDictionary;
                return;
            }
            
            var newDictionary = new Dictionary<uint, object>();
            newDictionary.Add(id, obj);
            
            typeDictionary.Add(type, newDictionary);
        }
        
        /// <summary>
        /// Get an object from the toolbox.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <returns>The object.</returns>
        public static T Get<T>(uint id = 0)
        {
            if (_shuttingDown) { return default;}
            
            var typeDictionary = Instance.m_Dictionary;
            
            var type = typeof(T);
            
            if (typeDictionary.ContainsKey(type) == false) {
                Debug.LogWarning("[Toolbox] Global component of type <"+typeof(T).Name+"> ID \""
                                 + id + "\" doesn't exist! Typo?");
                return default;
            }
            
            var objDictionary = typeDictionary[type];
            if (objDictionary.ContainsKey(id) == false) {
                Debug.LogWarning("[Toolbox] Global component of type <"+typeof(T).Name+"> ID \""
                                 + id + "\" doesn't exist! Typo?" );
                return default;
            }

            return (T)objDictionary[id];
        }
    }
}
