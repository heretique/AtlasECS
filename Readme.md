# Atlas ECS
Atlas is an Entity Component System for Unity3D. Created as a hobby project and greatly inspired by [RobotArms](https://bitbucket.org/dkoontz/robotarms) and [Entitas](https://github.com/sschmid/Entitas-CSharp) component systems for Unity. I took what I considered was best from them for my needs while trying to create a flexible new ECS.

## Design
There are a few design choices that were taken in order to allow for easy integration and usage with Unity. While *Entitas* uses simple classes that contain only data I chose to use MonoBehaviours that contain only data. I made this decision in order to preserve the serialization of components to allow for easy tweaking of values in the editor. *RobotArms* uses a similar approach but where Atlas sets itself apart is in the way Systems are defined and used.
### Systems
All systems inherit from the abstract class *AtlasSystem\<T\>* where *T* is an *IMatcher* interface. This allows for easy definition of systems with complex component requirements. At the same time a system can inherit from one of the following _"System"_ interfaces *IInitializeSystem*, *IUpdateSystem*, *ILateUpdateSystem*, *IFixedUpdateSystem* and *IReactiveSystem\<T\>*.
Examples:

	public class TransportingSystem : AtlasSystem<Matcher<Requires<Transporter>, Rejects<Stopped>, Rejects<WaitForNewChunk>>>, 
	    IInitializeSystem,
	    IUpdateSystem, 
	    IReactiveSystem<SetTrack>
	{
	}

	public class CharacterControllerSystem : AtlasSystem<Matcher<Requires<CharacterController>, 
    Requires<Transporter>, 
    Requires<Animator>,
    Accepts<Stopped>>>,
    IInitializeSystem,
    IReactiveSystem<InputEvent>
	{
	}

### Components
For an entity to be registered with the ECS it needs to have at least a component that inherits AtlasComponent. This class provides automatic registration and de-registration of the entity holding this component with Atlas. Entities in our case are considered any GameObjects that hold at least one AtlasComponent.

### Utilities
Atlas comes with a built-in Object Pool for those who want to minimize allocations as well as some builtin useful systems and components.

## Usage
Import this folder directly into your Unity Project's Asset folder.
Create a class derived from *AtlasRoot*, let's call it *AppRoot*, this will be our application Composition Root.
	public class AppRoot : AtlasRoot { }
Before using Atlas you need to make sure the correct Script Execution Order is in place and that the "*AtlasPool*" and "*AppRoot*" are attached to a persistent object inside your scene.
![script execution order](https://cloud.githubusercontent.com/assets/139596/12090499/33b85252-b2f8-11e5-86bc-62516fa3867c.png)
![persistent root](https://cloud.githubusercontent.com/assets/139596/12090496/2dbf5116-b2f8-11e5-982c-ba9e89b5e437.png)

Now ovveride *AtlasRoot*'s *Initialize* method and add here all the systems you will be using:

	public class AppRoot : AtlasRoot
	{
	    public override void Initialize()
	    {
	        AddSystem<ChunkTrackSystem>();
	        ChunkMapSpawningSystem SpawningSystem = new ChunkMapSpawningSystem(MonoBehaviourSingleton<PropsFactory>.Singleton);
	        AddSystem(SpawningSystem);
	        AddSystem<TransportingSystem>();
	        AddSystem<CharacterControllerSystem>();
	    }
	}

### Systems internals

Internaly Atlas uses a pool to store all the game objects (entities) that have at least one AtlasComponent derived MonoBehaviour attached to them. For each system matcher (required components) the pool holds an AtlasGroup object that in turn holds an up to date list of all the entitites that match the system's requirements. This list is updated every time a component is added or removed.
For the system to act on these components we provide a member *_entityGroup* that can be used to retrieve all the entities for this system.
The base system classes are defined like this:

    public abstract class AtlasSystem<T> : AtlasSystem where T : IMatcher
    {

    }

    public abstract class AtlasSystem
    {
        public IMatcher Matcher { get { return _matcher; } }
        IMatcher _matcher;
        protected IAtlasGroup _entityGroup;
        protected AtlasSystem()
        {
            Type[] types = GetType().BaseType.GetGenericArguments();
            _matcher = (IMatcher)Activator.CreateInstance(types[0]);
            _entityGroup = AtlasExtensions.GetGroup(_matcher);
		}
    } 

The access the components from the entities you usually have to make a call to *GetComponent\<T\>()*. This does implies a cost and this cost can add up pretty quick if your system is handling a huge number of entities. To overcome this performance issue *AtlasGroup* class provide a way of caching the required components for entities. In order to enable this caching you have to call *_entityGroup.EnableComponentCachingFor()*, in the system's constructor, with a list of component types you need to be cached. These have to be from the ones specified as generic arguments during system definition.
An example of usage is bellow:

	public class ExpirableSystem : AtlasSystem<Requires<Expirable>>, IUpdateSystem
	{

        public ExpirableSystem()
            : base()
        {
            _entityGroup.EnableComponentCachingFor(typeof(Expirable));
        }

        void IUpdateSystem.Update(float deltaTime)
        {
            var entities = _entityGroup.GetCachedEntities();
            for (var i = 0; i < entities.Count; ++i)
            {
                Expirable expirable = (Expirable)entities[i].Components[0];
                expirable.TimeRemaining -= Time.deltaTime;

                if (expirable.TimeRemaining <= 0)
                {
                    expirable.Target.Destroy();
                }
            }
        }
	}
