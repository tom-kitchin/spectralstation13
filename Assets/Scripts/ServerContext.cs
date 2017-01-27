using System;
using Svelto.Context;
using Svelto.ECS;
using Svelto.Factories;
using UnityEngine;
using UnityEngine.Networking;
using Services.Networking;
using Services.EntityDescriptors;
using Config;
using Config.Loaders;
using Config.Parsers;
using Config.Serializers;
using Factories;
using Implementers.Networking;
using Implementers.Control;
using Engines;
using Engines.Server.Networking;
using Engines.General.Motion;

/*
 * Main is the Application Composition Root.
 * Composition Root is the place where the framework can be initialised.
 */
public class Server : ICompositionRoot
{
    public Server ()
    {
        _onSetupComplete += StartServer;
        SpectreServer.onServerStart += OnServerStart;
        SetupEnginesAndComponents();
    }

    /**
     * The main work of preparing all Engines and Components occurs here.
     * After this, Engines will do the work of actually running the game.
     */
    void SetupEnginesAndComponents ()
    {
        _entityFactory = _enginesRoot = new EnginesRoot();

        // Load entity and map config.
        string mapName = "mapTest";
        WindowsFileConfigLoader configLoader = new WindowsFileConfigLoader(mapName);
        _config = configLoader.Load(new JsonConfigParser());
        _config.mapName = mapName;
        _factory = new NetworkGameObjectFromConfigFactory(_config);
        _playerPrefab = Resources.Load<GameObject>("Prefabs/PlayerManager");

		// Start engines.
		AddEngine(new NPCPositionUpdateEngine());
        AddEngine(new MovementEngine());
        
        if (_onSetupComplete != null) { _onSetupComplete(); }
    }
    
    /**
     * Get this party started. As it were. SpectreServer does all the heavy lifting.
     */
    void StartServer ()
    {
        SpectreServer.onCreatePlayer += ServerCreatePlayer;
        SpectreServer.Serializer = new WindowsFileConfigSerializer(_config.mapName);
        SpectreServer.StartServer();
    }

    /**
     * Triggered when SpectreServer is up and running.
     */
    void OnServerStart ()
    {
        // Build initial entities.
        _testRobot = _factory.Build("robot");
        _entityFactory.BuildEntity(_testRobot.GetInstanceID(), EntityDescriptorBuilder.BuildEntityDescriptorForGameObject(_testRobot));
        SpectreServer.Spawn(_testRobot);

        // Testing bullshit
        TestMoveControls tester = GameObject.FindObjectOfType<TestMoveControls>();
        tester.testEntity = _testRobot;
    }

    /**
     * Delegate for building a player object when a new player connects to the server.
     */
    GameObject ServerCreatePlayer (NetworkConnection conn, UnityEngine.Networking.NetworkSystem.AddPlayerMessage message)
    {
        Debug.Log("ServerContext:ServerCreatePlayer");
        
        GameObject player = _factory.Build(_playerPrefab);
        PlayerManager manager = player.GetComponent<PlayerManager>();
        manager.connection = conn;
        manager.identity = player.GetComponent<NetworkIdentity>();
        manager.currentBody = _testRobot;
        manager.currentBodyDispatcher = new DispatchOnChange<GameObject>(manager.GetInstanceID());
        manager.currentBodyDispatcher.value = _testRobot;
        MovementControl movementControl = player.GetComponent<MovementControl>();
        movementControl.listening = true;
        _entityFactory.BuildEntity(player.GetInstanceID(), EntityDescriptorBuilder.BuildEntityDescriptorForGameObject(player));
        Debug.Log(player);
        return player;
    }

    /**
     * Initialise engines.
     * If they're Tickable engines, sets them up to tick properly.
     */
    void AddEngine (IEngine engine)
    {
        _enginesRoot.AddEngine(engine);

        // Trigger ILateInitEngine LateInit calls.
        if (engine is ILateInitEngine)
        {
            (engine as ILateInitEngine).LateInit();
        }
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
    WorldConfig _config;
    event Action _onSetupComplete;
    GameObject _playerPrefab;
    GameObject _testRobot;
}

/*
 * A GameObject containing UnityContext must be present in the scene
 * All the monobehaviours present in the scene statically that need
 * to notify the Context, must belong to GameObjects children of UnityContext.
 */
public class ServerContext : UnityContext<Server> { }