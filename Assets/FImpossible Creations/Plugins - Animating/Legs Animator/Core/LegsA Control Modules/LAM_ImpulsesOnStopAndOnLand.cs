#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace FIMSpace.FProceduralAnimation
{
    [CreateAssetMenu(fileName = "Module Settings-Impulses On Stop And On Land", menuName = "FImpossible Creations/Legs Animator/Module - Impulses on Stop and Land Setup", order = 4)]
    public class LAM_ImpulsesOnStopAndOnLand : LegsAnimatorControlModuleBase
    {
        [FPD_Header("Triggering hips push impulses")]
        public LegsAnimator.PelvisImpulseSettings OnStopImpulse;
        public LegsAnimator.PelvisImpulseSettings OnLandImpulse;
        [FPD_Header("Set Zero Power to Not Use")]
        public LegsAnimator.PelvisImpulseSettings OnStartMoveImpulse;

        readonly string powerMulStrN = "Power Multiplier";
        readonly string durMulStrN = "Duration Multiplier";

        bool lastGrounded = true;
        bool lastMoving = false;

        float lastMovingTime = 0f;
        float lastUngroundedTime = 0f;

        LegsAnimator.Variable _powerMulVar;
        LegsAnimator.Variable _durMulVar;


        private void Reset()
        {
            OnStartMoveImpulse = new LegsAnimator.PelvisImpulseSettings();
            OnStartMoveImpulse.PowerMultiplier = 0f;
            OnStartMoveImpulse.LocalTranslation = new Vector3(0f, -0.05f, -0.001f);
        }

        public override void OnInit(LegsAnimator.LegsAnimatorCustomModuleHelper helper)
        {
            lastGrounded = true;
            lastMoving = false;

            _powerMulVar  = helper.RequestVariable(powerMulStrN, 1f);
            _durMulVar = helper.RequestVariable(durMulStrN, 1f);
        }

        public override void OnUpdate(LegsAnimator.LegsAnimatorCustomModuleHelper helper)
        {
            var l = LA;

            if (l.IsInAir)
            {
                if (l.InAirTime > 0f) lastUngroundedTime = l.InAirTime;
            }

            if (l.IsGrounded != lastGrounded)
            {
                if (OnLandImpulse.PowerMultiplier != 0f)
                    if (l.IsGrounded)
                    {
                        if (lastUngroundedTime > 0.1f)
                            l.User_AddImpulse(OnLandImpulse, _powerMulVar.GetFloat(), _durMulVar.GetFloat());
                    }
            }

            if (l.IsMoving)
            {
                if (l.MovingTime > 0f) lastMovingTime = l.MovingTime;
            }

            if (l.IsMoving != lastMoving)
            {
                if (OnStopImpulse.PowerMultiplier != 0f)
                    if (l.IsMoving == false)
                    {
                        if (lastMovingTime > 0.3f)
                            l.User_AddImpulse(OnStopImpulse, _powerMulVar.GetFloat(), _durMulVar.GetFloat());
                    }
                    else
                    {
                        if (OnStartMoveImpulse.PowerMultiplier != 0f)
                            l.User_AddImpulse(OnStartMoveImpulse, _powerMulVar.GetFloat(), _durMulVar.GetFloat());
                    }
            }

            lastGrounded = l.IsGrounded;
            lastMoving = l.IsMoving;
        }



        #region Editor Code
#if UNITY_EDITOR


        public override void Editor_InspectorGUI(LegsAnimator legsAnimator, LegsAnimator.LegsAnimatorCustomModuleHelper helper)
        {
            EditorGUILayout.HelpBox("Triggering hips push impulses when Legs Animator IsMoving and IsGrounded variables are changing.", MessageType.Info);
            GUILayout.Space(4);

            var powerMulV = helper.RequestVariable(powerMulStrN, 1f);
            powerMulV.SetMinMaxSlider(0f, 3f);
            powerMulV.Editor_DisplayVariableGUI();

            var durMulV = helper.RequestVariable(durMulStrN, 1f);
            durMulV.SetMinMaxSlider(0f, 3f);
            durMulV.Editor_DisplayVariableGUI();

            GUILayout.Space(4);
            if (GUILayout.Button("Go to module file for Push Impulses settings!"))
            {
                Selection.activeObject = helper.CurrentModule;
            }

            if (!legsAnimator.Mecanim)
            {
                UnityEditor.EditorGUILayout.HelpBox("No mecanim to detect parameters!", UnityEditor.MessageType.Warning);
                UnityEditor.EditorGUILayout.HelpBox("You can still use custom coding for legsAnimator.User_SetIsGrounded  and  legsAnimator.User_SetIsMoving", UnityEditor.MessageType.None);
                return;
            }

            bool wasWarning = false;
            if (string.IsNullOrWhiteSpace(legsAnimator.GroundedParameter))
            {
                wasWarning = true;
                UnityEditor.EditorGUILayout.HelpBox("No grounded parameter to detect landing! (Ignore this message if you set grounded state through code)", UnityEditor.MessageType.Warning);
            }

            if (!wasWarning)
            if (string.IsNullOrWhiteSpace(legsAnimator.MovingParameter))
            {
                    wasWarning = true;
                UnityEditor.EditorGUILayout.HelpBox("No IsMoving parameter to detect stopping!  (Ignore this message if you set movement state through code)", UnityEditor.MessageType.Warning);
            }
        }


#endif
        #endregion


        #region Inspector Editor Class Ineritance
#if UNITY_EDITOR

        [UnityEditor.CanEditMultipleObjects]
        [UnityEditor.CustomEditor(typeof(LAM_ImpulsesOnStopAndOnLand))]
        public class LAM_ImpulsesOnStopAndOnLandEditor : LegsAnimatorControlModuleBaseEditor
        {
        }

#endif
        #endregion


    }
}