using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FIMSpace.BonesStimulation
{
    public partial class BonesStimulator : UnityEngine.EventSystems.IDropHandler, IFHierarchyIcon
    {
        
        #region Hierarchy Icon

        public string EditorIconPath { get { if (PlayerPrefs.GetInt("AnimsH", 1) == 0) return ""; else return "Bones Stimulator/BonesStimulator"; } }
        public void OnDrop(UnityEngine.EventSystems.PointerEventData data) { }

        #endregion


        #region Editor Helpers

        [HideInInspector] public string _editor_Title = " Bones Stimulator";

        [HideInInspector] public bool _editor_DrawSetup = true;
        [HideInInspector] public bool _editor_DrawTweaking = false;

        [HideInInspector] public int _editor_DisplayedPreset = 0;
        [HideInInspector] public bool _editor_DrawGizmos = true;

        public bool DrawGizmos = true;
        //[HideInInspector] public Type _editor_ViewCategory;
        //[HideInInspector] public EMD_SetupCategory _editor_SetupCategory = EMD_SetupCategory.Movement;

        #endregion


        #region Performance Measuring

        void MeasurePerformanceUpdate(bool start)
        {
#if UNITY_EDITOR
            if (start) _perf_preUpd.Start(gameObject); else _perf_preUpd.Finish();
#endif
        }

        void MeasurePerformanceMain(bool start)
        {
#if UNITY_EDITOR
            if (start) _perf_main.Start(gameObject); else _perf_main.Finish();
#endif
        }



#if UNITY_EDITOR

        public FDebug_PerformanceTest _perf_preUpd = new FDebug_PerformanceTest();
        public FDebug_PerformanceTest _perf_main = new FDebug_PerformanceTest();

#endif

        #endregion


        public enum EStimulationMode
        { Muscles, Vibrate, Squeezing, Collisions }
        public EStimulationMode _editor_SelCategory = EStimulationMode.Muscles;
    }
}