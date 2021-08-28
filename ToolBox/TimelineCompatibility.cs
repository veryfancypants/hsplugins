using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using Studio;
using ToolBox.Extensions;
using UnityEngine;

namespace ToolBox
{
    internal class TimelineCompatibility
    {
        private static MethodInfo _addInterpolableModelStatic;
        private static MethodInfo _addInterpolableModelDynamic;
        private static Action _refreshInterpolablesList;
        private static Type _interpolableDelegate;
            
        public static void Init(Action onTimelineFound)
        {
            Type timelineType = Type.GetType("Timeline.Timeline,Timeline");
            if (timelineType != null)
            {
                _addInterpolableModelStatic = timelineType.GetMethod("AddInterpolableModelStatic", BindingFlags.Public | BindingFlags.Static);
                _addInterpolableModelDynamic = timelineType.GetMethod("AddInterpolableModelDynamic", BindingFlags.Public | BindingFlags.Static);
                _refreshInterpolablesList = (Action)Delegate.CreateDelegate(typeof(Action), timelineType.GetMethod("RefreshInterpolablesList", BindingFlags.Public | BindingFlags.Static));
                _interpolableDelegate = Type.GetType("Timeline.InterpolableDelegate,Timeline");
                if (onTimelineFound != null)
                {
                    UnityEngine.Debug.LogError("Found timeline");
                    onTimelineFound();
                }
                else
                {
                    UnityEngine.Debug.LogError("Not found timeline");
                }
            }
        }

        /// <summary>
        /// Adds an InterpolableModel to the list with a constant parameter
        /// </summary>
        public static void AddInterpolableModelStatic(string owner,
                                                      string id,
                                                      object parameter,
                                                      string name, 
                                                      ToolBox.Extensions.Action<ObjectCtrlInfo, object, object, object, float> interpolateBefore,
                                                      ToolBox.Extensions.Action<ObjectCtrlInfo, object, object, object, float> interpolateAfter,
                                                      Func<ObjectCtrlInfo, bool> isCompatibleWithTarget,
                                                      Func<ObjectCtrlInfo, object, object> getValue,
                                                      Func<object, XmlNode, object> readValueFromXml, 
                                                      Action<object, XmlTextWriter, object> writeValueToXml,
                                                      Func<ObjectCtrlInfo, XmlNode, object> readParameterFromXml = null,
                                                      Action<ObjectCtrlInfo, XmlTextWriter, object> writeParameterToXml = null,
                                                      Func<ObjectCtrlInfo, object, object, object, bool> checkIntegrity = null,
                                                      bool useOciInHash = true,
                                                      Func<string, ObjectCtrlInfo, object, string> getFinalName = null,
                                                      Func<ObjectCtrlInfo, object, bool> shouldShow = null)
        {
            Delegate ib = null;
            if (interpolateBefore != null)
                ib = Delegate.CreateDelegate(_interpolableDelegate, interpolateBefore.Target, interpolateBefore.Method);
            Delegate ia = null;
            if (interpolateAfter != null)
                ia = Delegate.CreateDelegate(_interpolableDelegate, interpolateAfter.Target, interpolateAfter.Method);
            _addInterpolableModelStatic.Invoke(null, new object[]
            {
                owner,
                id,
                parameter,
                name,
                ib,
                ia,
                isCompatibleWithTarget,
                getValue,
                readValueFromXml,
                writeValueToXml,
                readParameterFromXml,
                writeParameterToXml,
                checkIntegrity,
                useOciInHash,
                getFinalName,
                shouldShow
            });
        }

        /// <summary>
        /// Adds an interpolableModel to the list with a dynamic parameter
        /// </summary>
        public static void AddInterpolableModelDynamic(string owner,
                                                       string id,
                                                       string name,
                                                       ToolBox.Extensions.Action<ObjectCtrlInfo, object, object, object, float> interpolateBefore,
                                                       ToolBox.Extensions.Action<ObjectCtrlInfo, object, object, object, float> interpolateAfter,
                                                       Func<ObjectCtrlInfo, bool> isCompatibleWithTarget,
                                                       Func<ObjectCtrlInfo, object, object> getValue,
                                                       Func<object, XmlNode, object> readValueFromXml,
                                                       Action<object, XmlTextWriter, object> writeValueToXml,
                                                       Func<ObjectCtrlInfo, object> getParameter,
                                                       Func<ObjectCtrlInfo, XmlNode, object> readParameterFromXml = null,
                                                       Action<ObjectCtrlInfo, XmlTextWriter, object> writeParameterToXml = null,
                                                       Func<ObjectCtrlInfo, object, object, object, bool> checkIntegrity = null,
                                                       bool useOciInHash = true,
                                                       Func<string, ObjectCtrlInfo, object, string> getFinalName = null,
                                                       Func<ObjectCtrlInfo, object, bool> shouldShow = null)
        {
            Delegate ib = null;
            if (interpolateBefore != null)
                ib = Delegate.CreateDelegate(_interpolableDelegate, interpolateBefore.Target, interpolateBefore.Method);
            Delegate ia = null;
            if (interpolateAfter != null)
                ia = Delegate.CreateDelegate(_interpolableDelegate, interpolateAfter.Target, interpolateAfter.Method);
            _addInterpolableModelDynamic.Invoke(null, new object[]
            {
                owner,
                id,
                name,
                ib,
                ia,
                isCompatibleWithTarget,
                getValue,
                readValueFromXml,
                writeValueToXml,
                getParameter,
                readParameterFromXml,
                writeParameterToXml,
                checkIntegrity,
                useOciInHash,
                getFinalName,
                shouldShow
            });
        }

        public static void RefreshInterpolablesList()
        {
            if (_refreshInterpolablesList != null)
                _refreshInterpolablesList();
        }
    }
}
