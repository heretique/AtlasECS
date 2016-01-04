using UnityEngine;
using System;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;

namespace Atlas
{
    public class AtlasRoot : MonoBehaviour, ISystemAggregator
    {
        string _name;
        double _initializeDuration;
        double _frameUpdateDuration;
        double _fixedUpdateDuration;
        double _frameLateUpdateDuration;

        Stopwatch _stopwatch = new Stopwatch();

        public int initializeSystemsCount { get { return _initializeSystems.Count; } }
        public int updateSystemsCount { get { return _updateSystems.Count; } }
        public int lateUpdateSystemsCount { get { return _lateUpdateSystems.Count; } }
        public int fixedUpdateSystemsCount { get { return _fixedUpdateSystems.Count; } }
        public int reactiveSystemsCount { get { return _reactiveSystems.Count; } }
        public int suspendedSystemsCount { get { return _suspendedSystems.Count; } }
        public int totalSystemsCount { get { return initializeSystemsCount 
            + updateSystemsCount 
            + lateUpdateSystemsCount 
            + fixedUpdateSystemsCount
            + reactiveSystemsCount
            + suspendedSystemsCount; } }

        List<IInitializeSystem> _initializeSystems = new List<IInitializeSystem>();
        List<IUpdateSystem> _updateSystems = new List<IUpdateSystem>();
        List<ILateUpdateSystem> _lateUpdateSystems = new List<ILateUpdateSystem>();
        List<IFixedUpdateSystem> _fixedUpdateSystems = new List<IFixedUpdateSystem>();
        List<IReactiveSystem> _reactiveSystems = new List<IReactiveSystem>();
        List<ISystem> _suspendedSystems = new List<ISystem>();

        public virtual void Initialize()
        {

        }

        public void AddSystem<T>()
        {
            AddSystem(typeof(T));
        }

        public void AddSystem(Type systemType)
        {
            AddSystem((ISystem)Activator.CreateInstance(systemType));
        }

        public void AddSystem(ISystem system)
        {
            Type[] interfaces = system.GetType().GetInterfaces();

            if (Array.Exists<Type>(interfaces, i => i == typeof(IInitializeSystem)))
            {
                _initializeSystems.Add((IInitializeSystem)system);
            }
                

            if (Array.Exists<Type>(interfaces, i => i == typeof(IUpdateSystem)))
            {
                _updateSystems.Add((IUpdateSystem)system);
            }
                

            if (Array.Exists<Type>(interfaces, i => i == typeof(ILateUpdateSystem)))
            {
                _lateUpdateSystems.Add((ILateUpdateSystem)system);
            }
                

            if (Array.Exists<Type>(interfaces, i => i == typeof(IFixedUpdateSystem)))
            {
                _fixedUpdateSystems.Add((IFixedUpdateSystem)system);
            }
                

            if (Array.Exists<Type>(interfaces, i => i == typeof(IReactiveSystem)))
            {
                _reactiveSystems.Add((IReactiveSystem)system);
                AtlasExtensions.SubscribeEvent(system);
            }

        }
        public void RemoveSystem(ISystem system)
        {
            Type[] interfaces = system.GetType().GetInterfaces();

            if (Array.Exists<Type>(interfaces, i => i == typeof(IInitializeSystem)))
                _initializeSystems.Remove((IInitializeSystem)system);

            if (Array.Exists<Type>(interfaces, i => i == typeof(IUpdateSystem)))
                _updateSystems.Remove((IUpdateSystem)system);

            if (Array.Exists<Type>(interfaces, i => i == typeof(ILateUpdateSystem)))
                _lateUpdateSystems.Remove((ILateUpdateSystem)system);

            if (Array.Exists<Type>(interfaces, i => i == typeof(IFixedUpdateSystem)))
                _fixedUpdateSystems.Remove((IFixedUpdateSystem)system);

            if (Array.Exists<Type>(interfaces, i => i == typeof(IReactiveSystem)))
            {
                _reactiveSystems.Remove((IReactiveSystem)system);
                AtlasExtensions.UnsubscribeEvent(system);
            }
        }

        public void ToggleSystem(ISystem system)
        {
            if (_suspendedSystems.Remove(system))
            {
                AddSystem(system);
            }
            else
            {
                RemoveSystem(system);
                _suspendedSystems.Add(system);
            }
        }

        public ISystem[] Systems()
        {
            throw new System.NotImplementedException();
        }


        void Awake()
        {
            _name = gameObject.name;
            IEventAggregator eventAggregator = new EventAggregator();
            AtlasExtensions.SetEventAggregator(eventAggregator);            
        }


        void Start()
        {
            Initialize();
            _initializeDuration = 0;
            for (var i = 0; i < _initializeSystems.Count; ++i)
            {
                _stopwatch.Reset();
                _stopwatch.Start();
                _initializeSystems[i].Initialize();
                _stopwatch.Stop();
                var duration = _stopwatch.Elapsed.TotalMilliseconds;
                _initializeDuration += duration;
            }
        }

        void Update()
        {
            _frameUpdateDuration = 0;
            for (var i = 0; i < _updateSystems.Count; ++i)
            {
                _stopwatch.Reset();
                _stopwatch.Start();
                _updateSystems[i].Update(Time.deltaTime);
                _stopwatch.Stop();
                var duration = _stopwatch.Elapsed.TotalMilliseconds;
                _frameUpdateDuration += duration;
            }
        }

        void FixedUpdate()
        {
            _fixedUpdateDuration = 0;
            for (var i = 0; i < _fixedUpdateSystems.Count; ++i)
            {
                _stopwatch.Reset();
                _stopwatch.Start();
                _fixedUpdateSystems[i].FixedUpdate(Time.fixedDeltaTime);
                _stopwatch.Stop();
                var duration = _stopwatch.Elapsed.TotalMilliseconds;
                _fixedUpdateDuration += duration;
            } 
        }

        void LateUpdate()
        {
            _frameLateUpdateDuration = 0;
            for (var i = 0; i < _lateUpdateSystems.Count; ++i)
            {
                _stopwatch.Reset();
                _stopwatch.Start();
                _lateUpdateSystems[i].LateUpdate(Time.deltaTime);
                _stopwatch.Stop();
                var duration = _stopwatch.Elapsed.TotalMilliseconds;
                _frameLateUpdateDuration += duration;
            }
            UpdateName();
        }

        void UpdateName()
        {
            gameObject.name = string.Format("{0} - U: {1:0.###}, F: {2:0.###}, L: {3:0.###} ms", _name, _frameUpdateDuration, _fixedUpdateDuration, _frameLateUpdateDuration);
        }
    }
}


