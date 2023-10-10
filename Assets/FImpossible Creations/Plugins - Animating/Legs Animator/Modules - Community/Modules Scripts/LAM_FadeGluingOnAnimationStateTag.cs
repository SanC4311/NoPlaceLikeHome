#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using static FIMSpace.FProceduralAnimation.LegsAnimator;

namespace FIMSpace.FProceduralAnimation
{
    [CreateAssetMenu(fileName = "LAM_FadeGluingOnAnimationStateTag", menuName = "FImpossible Creations/Legs Animator/LAM_FadeGluingOnAnimationStateTag", order = 2)]
    public class LAM_FadeGluingOnAnimationStateTag : LegsAnimatorControlModuleBase
    {
        int _hash = -1;
        public Variable LayerVar { get; private set; }

        public override void OnInit(LegsAnimator.LegsAnimatorCustomModuleHelper helper)
        {
            string tagName = helper.RequestVariable("Disable Gluing On Tag", "Animator Param Name").GetString();
            LayerVar = helper.RequestVariable("Layer To Check Clip On", 0);
            _hash = Animator.StringToHash(tagName);
        }

        public override void OnUpdate(LegsAnimatorCustomModuleHelper helper)
        {
            if (!helper.Parent.Mecanim) return;

            var anim = helper.Parent.Mecanim;
            int layer = LayerVar.GetInt();
            AnimatorStateInfo animatorInfo = anim.IsInTransition(layer) ? anim.GetNextAnimatorStateInfo(layer) : anim.GetCurrentAnimatorStateInfo(layer);

            bool fadeOut = false;
            if (animatorInfo.tagHash == _hash) { fadeOut = true; }

            if ( fadeOut)
            {
                helper.Parent.MainGlueBlend = Mathf.MoveTowards(helper.Parent.MainGlueBlend, 0.001f, Time.deltaTime * 7f);
            }
            else
            {
                helper.Parent.MainGlueBlend = Mathf.MoveTowards(helper.Parent.MainGlueBlend, 1f, Time.deltaTime * 7f);
            }
        }

        #region Editor Code

#if UNITY_EDITOR

        public override void Editor_InspectorGUI(LegsAnimator legsAnimator, LegsAnimator.LegsAnimatorCustomModuleHelper helper)
        {
            EditorGUILayout.HelpBox("Fade off Legs Animator gluing when animator is playing tagged animation clip.", MessageType.Info);
            GUILayout.Space(3);

            if (Initialized) GUI.enabled = false;
            var tagName = helper.RequestVariable("Disable Gluing On Tag", "Tag");
            tagName.Editor_DisplayVariableGUI();
            if (Initialized) GUI.enabled = true;

            var layertoCheck = helper.RequestVariable("Layer To Check Clip On", 0);
            layertoCheck.Editor_DisplayVariableGUI();

            if ( helper.Parent.Mecanim == null)
            {
                EditorGUILayout.HelpBox("This module requires animator to be assigned under Legs Animator 'Extra -> Control' bookmark!", MessageType.Warning);
            }

            if (Initialized)
            {
                if (!legsAnimator.Mecanim) return;
                EditorGUILayout.LabelField("Hash " + _hash + " value for animator is = " + (helper.Parent.Mecanim.GetCurrentAnimatorStateInfo(layertoCheck.GetInt()).tagHash == _hash));
            }
        }

#endif

        #endregion

    }
}