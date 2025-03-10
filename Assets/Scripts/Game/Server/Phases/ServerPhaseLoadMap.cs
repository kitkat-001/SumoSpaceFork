﻿using System.Collections.Generic;
using FishNet.Connection;
using Game.Client.SceneLoading;
using Game.Common.Instances;
using Game.Common.Networking;
using Game.Common.Networking.Misc;
using Game.Common.Phases;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Server.Phases
{
    public class ServerPhaseLoadMap : IGamePhase
    {
        
        private GamePhaseNetworkManager _phaseNetworkManager;

        private List<int> loadedPlayers = new List<int>();

        private bool sceneLoaded = false;

        private SceneLoader _sceneLoader;
        
        public ServerPhaseLoadMap(GamePhaseNetworkManager phaseNetworkManager, SceneLoader sceneLoader)
        {
            _phaseNetworkManager = phaseNetworkManager;
            _sceneLoader = sceneLoader;
        }
        
        public void PhaseStart()
        {
            var _matchNetworkTimerManager = MainPersistantInstances.Get<MatchNetworkTimerManager>();
            
            _phaseNetworkManager.SendPhaseUpdate(_phaseNetworkManager.CurrentPhase, new byte[]{(byte)_phaseNetworkManager.masterSettings.matchSettings.SelectedMapItem.index} );
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadScene(_phaseNetworkManager.masterSettings.matchSettings.SelectedMapItem.sceneName);

            _phaseNetworkManager.masterSettings.matchSettings.timerIDs.mainMatchTimer =
                _matchNetworkTimerManager.CreateTimer().ID;
        }

        public void PhaseUpdate()
        {
            if (sceneLoaded && loadedPlayers.Count == _phaseNetworkManager.gameMatchSettings.MaxPlayerCount)
            {
                _phaseNetworkManager.SendPhaseUpdate(_phaseNetworkManager.CurrentPhase, new byte[]{0, 0});
                _phaseNetworkManager.ServerNextPhase();
                sceneLoaded = false;
            }
        }

        public void PhaseCleanUp()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            sceneLoaded = false;
        }

        public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            sceneLoaded = true;
        }

        public void OnUpdateReceived(NetworkConnection conn, byte[] data)
        {

            if (!_phaseNetworkManager.masterSettings.playerIDRegistry.HasNetworkID(conn.ClientId))
            {
                return;
            }
            
            if (loadedPlayers.Contains(conn.ClientId))
            {
                Debug.LogError("Duplicate update received for temp load map solution");
                return;
            }
            
            
            loadedPlayers.Add(conn.ClientId);

        }
    }
}