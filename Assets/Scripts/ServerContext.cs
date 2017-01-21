using System;
using Svelto.Context;
using Svelto.ES;
using Svelto.Factories;
using Svelto.Ticker;
using UnityEngine;
using Config;
using Config.Loaders;
using Config.Parsers;
using Factories;
using Engines.Networking;
using Engines.Motion;

/*
 * Main is the Application Composition Root.
 * Composition Root is the place where the framework can be initialised.
 */
public class Server : ICompositionRoot
{
    public Server ()
    {
        SetupEnginesAndComponents();
        if (_onSetupComplete != null) { _onSetupComplete(); }
    }

    /**
     * The main work of preparing all Engines and Components occurs here.
     * After this, Engines will do the work of actually running the game.
     */
    void SetupEnginesAndComponents ()
    {
        _tickEngine = new UnityTicker();
        _entityFactory = _enginesRoot = new EnginesRoot(_tickEngine);

        // Load entity and map data.
        string mapName = "mapTest";
        WindowsFileConfigLoader configLoader = new WindowsFileConfigLoader();
        _config = configLoader.Load(mapName, new JsonConfigParser());
        _factory = new NetworkGameObjectFromConfigFactory(_config);

        // Start engines.
        AddEngine(new ServerEngine(_factory, _entityFactory, _config, ref _onSetupComplete));
        //AddEngine(new ServerConfigTransmissionEngine(_config));
        AddEngine(new MovementEngine());

        // Build initial entities.
        GameObject testRobot = _factory.Build("robot");
        _entityFactory.BuildEntity(testRobot.GetInstanceID(), testRobot.GetComponent<IEntityDescriptorHolder>().BuildDescriptorType());
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

    /**
     * Builds GameComponents which use the IEntityDescriptorHolder to define their components in the Unity UI
     */
    void ICompositionRoot.OnContextCreated (UnityContext contextHolder)
    {
        IEntityDescriptorHolder[] entities = contextHolder.GetComponentsInChildren<IEntityDescriptorHolder>();

        for (int i = 0; i < entities.Length; i++)
        {
            _entityFactory.BuildEntity((entities[i] as MonoBehaviour).gameObject.GetInstanceID(), entities[i].BuildDescriptorType());
        }
    }

    void ICompositionRoot.OnContextInitialized () { }

    void ICompositionRoot.OnContextDestroyed () { }

    EnginesRoot _enginesRoot;
    IGameObjectFactory _factory;
    IEntityFactory _entityFactory;
    UnityTicker _tickEngine;
    WorldConfig _config;
    event Action _onSetupComplete;
}

/*
 * A GameObject containing UnityContext must be present in the scene
 * All the monobehaviours present in the scene statically that need
 * to notify the Context, must belong to GameObjects children of UnityContext.
 */
public class ServerContext : UnityContext<Server> { }