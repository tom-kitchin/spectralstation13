﻿using Svelto.Context;
using Svelto.ES;
using Svelto.Ticker;
using UnityEngine;
using Config;
using Config.Json;
using Factories;

using Engines.Test;

/*
 * Main is the Application Composition Root.
 * Composition Root is the place where the framework can be initialised.
 */
public class Main : ICompositionRoot {
    public Main () {
        _contextInitialized = new Dispatcher<int>(0);
        SetupEnginesAndComponents();
    }

    /**
     * The main work of preparing all Engines and Components occurs here.
     * After this, Engines will do the work of actually running the game.
     */
    void SetupEnginesAndComponents () {
        _tickEngine = new UnityTicker();
        _entityFactory = _enginesRoot = new EnginesRoot(_tickEngine);

        GameObjectFactory factory = new GameObjectFactory();

        // Load entity and map data.
        JsonFileConfigLoader configLoader = new JsonFileConfigLoader("mapTest");
        _config = configLoader.WorldConfig;
 
        GameObjectFromConfigFactory configFactory = new GameObjectFromConfigFactory(_config);

        //AddEngine(new CounterIncrementEngine());

        // Build starting entities.
        configFactory.Build("robot");
    }

    /**
     * Initialise engines.
     * If they're Tickable engines, sets them up to tick properly.
     */
    void AddEngine (IEngine engine) {
        if (engine is ITickableBase)
            _tickEngine.Add(engine as ITickableBase);

        _enginesRoot.AddEngine(engine);
    }

    /**
     * Builds GameComponents which use the IEntityDescriptorHolder to define their components in the Unity UI
     */
    void ICompositionRoot.OnContextCreated (UnityContext contextHolder) {
        IEntityDescriptorHolder[] entities = contextHolder.GetComponentsInChildren<IEntityDescriptorHolder>();

        for (int i = 0; i < entities.Length; i++) {
            _entityFactory.BuildEntity((entities[i] as MonoBehaviour).gameObject.GetInstanceID(), entities[i].BuildDescriptorType());
        }
    }

    void ICompositionRoot.OnContextInitialized () {
        contextInitialized.Dispatch();
    }

    void ICompositionRoot.OnContextDestroyed () { }

    EnginesRoot _enginesRoot;
    IEntityFactory _entityFactory;
    UnityTicker _tickEngine;
    WorldConfig _config;

    Dispatcher<int> contextInitialized { get { return _contextInitialized; } }
    Dispatcher<int> _contextInitialized;
}

/*
 * A GameObject containing UnityContext must be present in the scene
 * All the monobehaviours present in the scene statically that need
 * to notify the Context, must belong to GameObjects children of UnityContext.
 */
public class MainContext : UnityContext<Main> { }