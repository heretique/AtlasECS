using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

namespace Atlas.VisualDebugging
{
    [CustomEditor(typeof(AtlasRootDebug), true)]
    public class DebugSystemsInspector : Editor
    {
        SystemsMonitor _systemsMonitor;
        Queue<float> _systemMonitorData;
        const int SYSTEM_MONITOR_DATA_LENGTH = 60;

        public override void OnInspectorGUI()
        {
            var systems = (Atlas.AtlasRootDebug)target;
            systems.InitSystemsInfo();
            if (_systemsMonitor == null)
            {
                _systemsMonitor = new SystemsMonitor(SYSTEM_MONITOR_DATA_LENGTH);
                _systemMonitorData = new Queue<float>(new float[SYSTEM_MONITOR_DATA_LENGTH]);
                if (EditorApplication.update != Repaint)
                {
                    EditorApplication.update += Repaint;
                }
            }



            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.LabelField(systems.name, EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Initialize Systems", systems.initializeSystemsCount.ToString());
            EditorGUILayout.LabelField("Update Systems", systems.updateSystemsCount.ToString());
            EditorGUILayout.LabelField("FixedUpdate Systems", systems.fixedUpdateSystemsCount.ToString());
            EditorGUILayout.LabelField("LateUpdate Systems", systems.lateUpdateSystemsCount.ToString());
            EditorGUILayout.LabelField("Suspended Systems", systems.suspendedSystemsCount.ToString());
            EditorGUILayout.LabelField("Total Systems", systems.totalSystemsCount.ToString());
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.LabelField("Execution duration", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Total", systems.totalDuration.ToString());
            EditorGUILayout.Space();

            if (!EditorApplication.isPaused)
            {
                addDuration((float)systems.totalDuration);
            }
            _systemsMonitor.Draw(_systemMonitorData.ToArray(), 80f);

            systems.threshold = EditorGUILayout.Slider("Threshold", systems.threshold, 0f, 100f);
            EditorGUILayout.Space();

            drawSystemInfos(systems.systemInfos, false);

            EditorGUILayout.EndVertical();

            EditorUtility.SetDirty(target);
        }

        void drawSystemInfos(SystemInfo[] systemInfos, bool isChildSysem)
        {
            var orderedSystemInfos = systemInfos
                .OrderByDescending(systemInfo => systemInfo.averageExecutionDuration)
                .ToArray();

            foreach (var systemInfo in orderedSystemInfos)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUI.BeginDisabledGroup(isChildSysem);
                systemInfo.isActive = EditorGUILayout.Toggle(systemInfo.isActive, GUILayout.Width(20));
                EditorGUI.EndDisabledGroup();

                var avg = string.Format("Ø {0:0.000}", systemInfo.averageExecutionDuration).PadRight(9);
                var min = string.Format("min {0:0.000}", systemInfo.minExecutionDuration).PadRight(11);
                var max = string.Format("max {0:0.000}", systemInfo.maxExecutionDuration);
                EditorGUILayout.LabelField(systemInfo.systemName, avg + "\t" + min + "\t" + max);
                EditorGUILayout.EndHorizontal();
            }
        }

        void addDuration(float duration)
        {
            if (_systemMonitorData.Count >= SYSTEM_MONITOR_DATA_LENGTH)
            {
                _systemMonitorData.Dequeue();
            }

            _systemMonitorData.Enqueue(duration);
        }
    }
}

