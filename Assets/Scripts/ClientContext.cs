﻿using System;
using Svelto.Context;
using Svelto.ES;
using Svelto.Ticker;
using Svelto.Factories;
using Services.Networking;
using Config;
using Config.Loaders;
using Config.Parsers;
using Config.Serializers;
using Factories;
using Engines.Motion;

/*
 * Main is the Application Composition Root.
 * Composition Root is the place where the framework can be initialised.
 */
public class Client : ICompositionRoot
{
    public Client ()
    {
        _onConfigReady += SetupEnginesAndComponents;
        BaseEngineSetup();
        ConnectToServer();
    }

    /**
     * Set up anything we can without knowing server config, which isn't much.
     * Also prepare the connection engine.
     */
    void BaseEngineSetup ()
    {
        _tickEngine = new UnityTicker();
        _entityFactory = _enginesRoot = new EnginesRoot(_tickEngine);
    }

    /**
     * We can't meaningfully progress with config and engine setup without
     * connecting to the server, because we need the server to send the 
     * game config.
     */
    void ConnectToServer ()
    {
        SpectreClient.onConfigDataValidated += LoadWorldConfigFromConfigData;
        SpectreClient.StartClient();
    }

    /**
     * Once we have the config sent from the server, we can load it up and then
     * get on with the usual business.
     */
    void LoadWorldConfigFromConfigData (byte[] configData)
    {
        SerializedConfigLoader configLoader = new SerializedConfigLoader(configData, new ConfigDeserializer());
        _config = configLoader.Load(new JsonConfigParser());
        _onConfigReady();
    }

    /**
     * The main work of preparing all Engines and Components occurs here.
     * After this, Engines will do the work of actually running the game.
     */
    void SetupEnginesAndComponents ()
    {
        // Load entity and map data.
        _factory = new NetworkGameObjectFromConfigFactory(_config);
        ConfigFactorySpawnManager.Initialize(_factory, _entityFactory, _config);

        // Start engines.
        AddEngine(new MovementEngine());
        
        if (_onSetupComplete != null) { _onSetupComplete(); }

        // Tell the server we're ready to proceed.
        SpectreClient.Ready();
    }

    /**
     * Initialise engines.
     * If they're Tickable engines, sets them up to tick properly.
     */
    void AddEngine (IEngine engine)
    {
        if (engine is ITickableBase)
            _tickEngine.Add(engine as ITickableBase);

        _enginesRoot.AddEngine(engine);
    }
    
    void ICompositionRoot.OnContextCreated (UnityContext contextHolder) { }

    void ICompositionRoot.OnContextInitialized () { }

    void ICompositionRoot.OnContextDestroyed () { }

    EnginesRoot _enginesRoot;
    IGameObjectFactory _factory;
    IEntityFactory _entityFactory;
    UnityTicker _tickEngine;
    WorldConfig _config;
    event Action _onConfigReady;
    event Action _onSetupComplete;
}

/*
 * A GameObject containing UnityContext must be present in the scene
 * All the monobehaviours present in the scene statically that need
 * to notify the Context, must belong to GameObjects children of UnityContext.
 */
public class ClientContext : UnityContext<Client> { }