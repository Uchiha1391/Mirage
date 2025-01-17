using System.Collections.Generic;
using Mirage.Logging;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Mirage
{
    /// <summary>
    /// Component that controls visibility of networked objects between scenes.
    /// <para>Any object with this component on it will only be visible to other objects in the same scene</para>
    /// <para>This would be used when the server has multiple additive subscenes loaded to isolate players to their respective subscenes</para>
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Network/NetworkSceneChecker")]
    [RequireComponent(typeof(NetworkIdentity))]
    [HelpURL("https://mirror-networking.com/docs/Components/NetworkSceneChecker.html")]
    public class NetworkSceneChecker : NetworkVisibility
    {
        static readonly ILogger logger = LogFactory.GetLogger(typeof(NetworkSceneChecker));

        /// <summary>
        /// Flag to force this object to be hidden from all observers.
        /// <para>If this object is a player object, it will not be hidden for that client.</para>
        /// </summary>
        [Tooltip("Enable to force this object to be hidden from all observers.")]
        public bool forceHidden;

        // Use Scene instead of string scene.name because when additively loading multiples of a subscene the name won't be unique
        static readonly Dictionary<Scene, HashSet<NetworkIdentity>> sceneCheckerObjects = new Dictionary<Scene, HashSet<NetworkIdentity>>();

        Scene currentScene;

        [Server(error = false)]
        void Awake()
        {
            currentScene = gameObject.scene;
            if (logger.LogEnabled()) logger.Log($"NetworkSceneChecker.Awake currentScene: {currentScene}");

            OnStartServer();
        }

        public void OnStartServer()
        {
            if (!sceneCheckerObjects.ContainsKey(currentScene))
                sceneCheckerObjects.Add(currentScene, new HashSet<NetworkIdentity>());

            sceneCheckerObjects[currentScene].Add(Identity);
        }

        [Server(error = false)]
        void Update()
        {
            if (currentScene == gameObject.scene)
                return;

            // This object is in a new scene so observers in the prior scene
            // and the new scene need to rebuild their respective observers lists.

            // Remove this object from the hashset of the scene it just left
            sceneCheckerObjects[currentScene].Remove(Identity);

            // RebuildObservers of all NetworkIdentity's in the scene this object just left
            RebuildSceneObservers();

            // Set this to the new scene this object just entered
            currentScene = gameObject.scene;

            // Make sure this new scene is in the dictionary
            if (!sceneCheckerObjects.ContainsKey(currentScene))
                sceneCheckerObjects.Add(currentScene, new HashSet<NetworkIdentity>());

            // Add this object to the hashset of the new scene
            sceneCheckerObjects[currentScene].Add(Identity);

            // RebuildObservers of all NetworkIdentity's in the scene this object just entered
            RebuildSceneObservers();
        }

        void RebuildSceneObservers()
        {
            foreach (NetworkIdentity networkIdentity in sceneCheckerObjects[currentScene])
                if (networkIdentity != null)
                    networkIdentity.RebuildObservers(false);
        }

        /// <summary>
        /// Callback used by the visibility system to determine if an observer (player) can see this object.
        /// <para>If this function returns true, the network connection will be added as an observer.</para>
        /// </summary>
        /// <param name="player">Network connection of a player.</param>
        /// <returns>True if the player can see this object.</returns>
        public override bool OnCheckObserver(INetworkPlayer player)
        {
            if (forceHidden)
                return false;

            return player.Identity.gameObject.scene == gameObject.scene;
        }

        /// <summary>
        /// Callback used by the visibility system to (re)construct the set of observers that can see this object.
        /// <para>Implementations of this callback should add network connections of players that can see this object to the observers set.</para>
        /// </summary>
        /// <param name="observers">The new set of observers for this object.</param>
        /// <param name="initialize">True if the set of observers is being built for the first time.</param>
        public override void OnRebuildObservers(HashSet<INetworkPlayer> observers, bool initialize)
        {
            // If forceHidden then return without adding any observers.
            if (forceHidden)
                return;

            // Add everything in the hashset for this object's current scene
            foreach (NetworkIdentity networkIdentity in sceneCheckerObjects[currentScene])
                if (networkIdentity != null && networkIdentity.Owner != null)
                    observers.Add(networkIdentity.Owner);
        }
    }
}
