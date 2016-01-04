using System;
using UnityEngine;
using System.Collections;

namespace Atlas
{
    public class SystemInfo
    {
        public ISystem system { get { return _system; } }
        public string systemName { get { return _systemName; } }
        public double totalExecutionDuration { get { return _totalExecutionDuration; } }
        public double minExecutionDuration { get { return _minExecutionDuration; } }
        public double maxExecutionDuration { get { return _maxExecutionDuration; } }
        public double averageExecutionDuration
        {
            get { return _durationsCount == 0 ? 0 : _totalExecutionDuration / _durationsCount; }
        }

        public bool isActive;

        ISystem _system;
        string _systemName;
        double _totalExecutionDuration = -1;
        double _minExecutionDuration;
        double _maxExecutionDuration;
        int _durationsCount;

        const string SYSTEM_SUFFIX = "System";

        public SystemInfo(ISystem system, Type systemType)
        {
            _system = system;


            _systemName = systemType.Name.EndsWith(SYSTEM_SUFFIX, StringComparison.Ordinal)
                ? systemType.Name.Substring(0, systemType.Name.Length - SYSTEM_SUFFIX.Length)
                : systemType.Name;

            isActive = true;
        }

        public void AddExecutionDuration(double executionDuration)
        {
            if (executionDuration < _minExecutionDuration || _totalExecutionDuration == -1)
            {
                _minExecutionDuration = executionDuration;
                if (_totalExecutionDuration == -1)
                {
                    _totalExecutionDuration = 0;
                }
            }
            if (executionDuration > _maxExecutionDuration)
            {
                _maxExecutionDuration = executionDuration;
            }

            _totalExecutionDuration += executionDuration;
            _durationsCount += 1;
        }

        public void Reset()
        {
            _totalExecutionDuration = 0;
            _durationsCount = 0;
        }
    }

    public enum AvgResetInterval
    {
        EveryFrame = 1,
        Every30Frames = 30,
        Every60Frames = 60,
        Every120Frames = 120,
        Every300Frames = 300,
        Never = int.MaxValue
    }
}
