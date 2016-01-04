using UnityEngine;
using System;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;

namespace Atlas
{
    public class AtlasRootDebug : MonoBehaviour, ISystemAggregator
    {
        [SerializeField]
        private bool Debug = false;

        public float threshold;
        public AvgResetInterval avgResetInterval = AvgResetInterval.Never;
        double _totalDuration;
        Dictionary<Type, SystemInfo> _systemInfos;
        Stopwatch _stopwatch;

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

        public double totalDuration { get { return _totalDuration; } }
        public SystemInfo[] systemInfos
        {
            get
            {
                return _systemInfos.Values
                  .Where(systemInfo => systemInfo.averageExecutionDuration >= threshold)
                  .ToArray();
            }
        }

        string _name;

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
                getSystemInfo((IInitializeSystem)system);
            }
                

            if (Array.Exists<Type>(interfaces, i => i == typeof(IUpdateSystem)))
            {
                _updateSystems.Add((IUpdateSystem)system);
                getSystemInfo((IUpdateSystem)system);
            }
                

            if (Array.Exists<Type>(interfaces, i => i == typeof(ILateUpdateSystem)))
            {
                _lateUpdateSystems.Add((ILateUpdateSystem)system);
                getSystemInfo((ILateUpdateSystem)system);
            }
                

            if (Array.Exists<Type>(interfaces, i => i == typeof(IFixedUpdateSystem)))
            {
                _fixedUpdateSystems.Add((IFixedUpdateSystem)system);
                getSystemInfo((IFixedUpdateSystem)system);
            }
                

            if (Array.Exists<Type>(interfaces, i => i == typeof(IReactiveSystem)))
            {
                _reactiveSystems.Add((IReactiveSystem)system);
                AtlasExtensions.SubscribeEvent(system);
                getSystemInfo((IReactiveSystem)system);
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

        SystemInfo getSystemInfo(ISystem system)
        {

            SystemInfo systemInfo;
            var systemType = system.GetType();

            if (!_systemInfos.TryGetValue(systemType, out systemInfo))
            {
                systemInfo = new SystemInfo(system, systemType);
                _systemInfos.Add(systemType, systemInfo);
            }

            return systemInfo;
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

            IAtlasPool pool = new AtlasPool();
            AtlasExtensions.SetEntityPool(pool);

            InitSystemsInfo();
        }


        public void InitSystemsInfo()
        {
            if (_systemInfos == null)
            {
                _systemInfos = new Dictionary<Type, SystemInfo>();
                _stopwatch = new Stopwatch();
            }
        }


        void Start()
        {
            Initialize();
            _totalDuration = 0;
            for (var i = 0; i < _initializeSystems.Count; ++i)
            {
                var systemInfo = getSystemInfo(_initializeSystems[i]);
                _stopwatch.Reset();
                _stopwatch.Start();
                _initializeSystems[i].Initialize();
                _stopwatch.Stop();
                var duration = _stopwatch.Elapsed.TotalMilliseconds;
                _totalDuration += duration;
                systemInfo.AddExecutionDuration(duration);
            }
        }

        void Update()
        {
            _totalDuration = 0;
            if (Time.frameCount % (int)avgResetInterval == 0)
            {
                ResetSystemInfos();
            }
            for (var i = 0; i < _updateSystems.Count; ++i)
            {
                var systemInfo = getSystemInfo(_updateSystems[i]);
                _stopwatch.Reset();
                _stopwatch.Start();
                _updateSystems[i].Update(Time.deltaTime);
                _stopwatch.Stop();
                var duration = _stopwatch.Elapsed.TotalMilliseconds;
                _totalDuration += duration;
                systemInfo.AddExecutionDuration(duration);
            }
            UpdateName();
        }

        void FixedUpdate()
        {
            for (var i = 0; i < _fixedUpdateSystems.Count; ++i)
            {
                var systemInfo = getSystemInfo(_fixedUpdateSystems[i]);
                _stopwatch.Reset();
                _stopwatch.Start();
                _fixedUpdateSystems[i].FixedUpdate(Time.fixedDeltaTime);
                _stopwatch.Stop();
                var duration = _stopwatch.Elapsed.TotalMilliseconds;
                _totalDuration += duration;
                systemInfo.AddExecutionDuration(duration);
            } 
        }

        void LateUpdate()
        {
            for (var i = 0; i < _lateUpdateSystems.Count; ++i)
            {
                var systemInfo = getSystemInfo(_lateUpdateSystems[i]);
                _stopwatch.Reset();
                _stopwatch.Start();
                _lateUpdateSystems[i].LateUpdate(Time.deltaTime);
                _stopwatch.Stop();
                var duration = _stopwatch.Elapsed.TotalMilliseconds;
                _totalDuration += duration;
                systemInfo.AddExecutionDuration(duration);
            }
        }

        public void ResetSystemInfos()
        {
            foreach (var systemInfo in _systemInfos.Values)
            {
                systemInfo.Reset();
            }
        }

        void UpdateName()
        {
            gameObject.name = string.Format("{0} : {1:0.###} ms", _name, _totalDuration);
        }
    }
}


