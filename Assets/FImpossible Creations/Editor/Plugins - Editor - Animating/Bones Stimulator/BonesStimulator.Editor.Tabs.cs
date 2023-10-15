using FIMSpace.FEditor;
using UnityEditor;
using UnityEngine;


namespace FIMSpace.BonesStimulation
{
    public partial class BonesStimulator_Editor
    {
        private void Tab_DrawSetup()
        {
            EditorGUILayout.BeginVertical(FGUI_Resources.BGInBoxStyle);

            GUILayout.Space(2);
            EditorGUILayout.PropertyField(sp_Amount);
            GUILayout.Space(3);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button(new GUIContent(" " + FGUI_Resources.GetFoldSimbol(Get._editor_DrawSetup, 10, "►") + " "), FGUI_Resources.FoldStyle, new GUILayoutOption[] { GUILayout.Width(24), GUILayout.Height(24) })) { Get._editor_DrawSetup = !Get._editor_DrawSetup; }

            if (Application.isPlaying) GUI.enabled = false;
            if (Get.Bones.Count == 0)
            {
                GUI.color = new Color(1f, 1f, 1f, 0.7f);
                Transform defaultTr = (Transform)EditorGUILayout.ObjectField(Get.transform, typeof(Transform), true);
                if (defaultTr != null) if (defaultTr != Get.transform) Get.Bones.Add(new BonesStimulator.Bone() { transform = defaultTr });
                GUI.color = c;
            }
            else
            {
                if (Get.Bones.Count == 1) if (Get.Bones[0].transform == null) Get.Bones.Clear();
                if (sp_Bones != null) if (sp_Bones.arraySize > 0)
                    {
                        SerializedProperty prppp = sp_Bones.GetArrayElementAtIndex(0).FindPropertyRelative("transform");
                        if (prppp != null) EditorGUILayout.PropertyField(prppp, GUIContent.none);
                    }
            }

            if (Get.UseCollisions)
            {
                bool preE = GUI.enabled;
                GUI.enabled = true;
                EditorGUIUtility.labelWidth = 26;
                if (sp_Bones != null)
                {
                    if (sp_Bones.arraySize > 0)
                    {
                        SerializedProperty pprp = sp_Bones.GetArrayElementAtIndex(0);
                        if (pprp != null)
                        {
                            SerializedProperty pprp2 = pprp.FindPropertyRelative("EnableCollisions");
                            if (pprp2 != null) EditorGUILayout.PropertyField(pprp2, new GUIContent("", FGUI_Resources.Tex_Collider, "If collisions should be detected on this segment"), new GUILayoutOption[] { GUILayout.Width(44), GUILayout.Height(18) });
                        }
                    }
                }

                EditorGUIUtility.labelWidth = 0;
                GUI.enabled = preE;
            }

            if (GUILayout.Button("+", GUILayout.Width(22), GUILayout.Height(18)))
            {
                AddBoneTo(Get);
                EditorUtility.SetDirty(Get);
                serializedObject.Update();

                foreach (var st in SelectedStimulators())
                {
                    if (st == Get) continue;
                    AddBoneTo(st);
                }

                return;
            }

            EditorGUILayout.EndHorizontal();

            if (Get._editor_DrawSetup)
            {
                if (Application.isPlaying) GUI.enabled = false;
                if (Get.Bones.Count < 2)
                {
                    EditorGUILayout.LabelField("Hit '+' button to add bones to stimulate", FGUI_Resources.HeaderStyle);
                }

                GUILayout.Space(2);


                for (int i = 1; i < sp_Bones.arraySize; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    SerializedProperty prop = sp_Bones.GetArrayElementAtIndex(i);
                    if (prop != null)
                    {
                        SerializedProperty pt = prop.FindPropertyRelative("transform");
                        if (pt != null) EditorGUILayout.PropertyField(pt, GUIContent.none);
                    }

                    if (Get.UseCollisions)
                    {
                        bool preE = GUI.enabled;
                        GUI.enabled = true;
                        EditorGUIUtility.labelWidth = 26;

                        if (prop != null)
                        {
                            SerializedProperty prpp = prop.FindPropertyRelative("EnableCollisions");
                            if (prpp != null) EditorGUILayout.PropertyField(prpp, new GUIContent("", FGUI_Resources.Tex_Collider, "If collisions should be detected on this segment"), new GUILayoutOption[] { GUILayout.Width(44), GUILayout.Height(18) });
                        }

                        EditorGUIUtility.labelWidth = 0;
                        GUI.enabled = preE;
                    }

                    if (GUILayout.Button("X", GUILayout.Width(22), GUILayout.Height(18)))
                    {
                        Get.Bones.RemoveAt(i);
                        EditorUtility.SetDirty(Get);
                        return;
                    }

                    EditorGUILayout.EndHorizontal();
                }
                if (Application.isPlaying) GUI.enabled = e;

                if (Get.Bones.Count > 1)
                {
                    GUILayout.Space(3);
                    EditorGUILayout.LabelField(Lang("Above"), FGUI_Resources.HeaderStyle);
                }
                if (Application.isPlaying) GUI.enabled = e;




                // End tip offset --------------------
                if (Get.MovementMuscles > 0f || Get.RotationSpaceMuscles > 0f)
                    if (Get.GetLastTransform() != null)
                        if (Get.GetLastTransform().parent)
                        {
                            if (Application.isPlaying) GUI.enabled = false;
                            EditorGUILayout.BeginHorizontal(FGUI_Resources.BGInBoxStyle);
                            EditorGUILayout.PropertyField(sp_AutoHelperOffset);

                            if (Get.AutoHelperOffset)
                            {
                                GUI.enabled = false;
                                if (!Application.isPlaying) Get.HelperOffset = Get.GetLastTransform().InverseTransformVector(Get.GetEndTipWorldOffset() / 2f);
                                SerializedProperty sp = sp_AutoHelperOffset.Copy(); sp.NextVisible(false);
                                EditorGUILayout.PropertyField(sp, GUIContent.none);
                                if (Application.isPlaying == false) GUI.enabled = true;
                            }
                            else
                            {
                                SerializedProperty sp = sp_AutoHelperOffset.Copy(); sp.NextVisible(false);
                                if (!Application.isPlaying)
                                    EditorGUILayout.PropertyField(sp, GUIContent.none);
                                else
                                    EditorGUILayout.PropertyField(sp);

                            }

                            EditorGUILayout.EndHorizontal();
                            if (Application.isPlaying) GUI.enabled = e;
                        }


                GUILayout.Space(5);
                SerializedProperty dprop = sp_Compens.Copy();
                EditorGUILayout.BeginVertical(FGUI_Resources.BGInBoxStyle); EditorGUIUtility.labelWidth = 160;
                dprop.NextVisible(false);
                EditorGUILayout.BeginHorizontal();
                EditorGUIUtility.labelWidth = 110;
                EditorGUILayout.PropertyField(dprop);
                dprop.NextVisible(false);
                EditorGUILayout.PropertyField(dprop);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUIUtility.labelWidth = 110;
                EditorGUILayout.PropertyField(sp_Rate, GUILayout.MaxWidth(142));

                if (sp_Rate.intValue <= 0)
                {
                    sp_Rate.intValue = 0;
                    EditorGUILayout.LabelField("  (unlimited - default)", GUILayout.Width(126));
                    GUILayout.FlexibleSpace();
                }

                EditorGUILayout.EndHorizontal();

                dprop.NextVisible(false);

                EditorGUIUtility.labelWidth = 0;
                El_HideWith(dprop); dprop.NextVisible(false);
                El_Distance(dprop);
                GUILayout.Space(2);
                EditorGUILayout.PropertyField(sp_Compens);
                GUILayout.Space(2);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(sp_Grav, new GUIContent("Gravity (Experimental)", sp_Grav.tooltip));
                EditorGUIUtility.labelWidth = 18;
                GUILayout.Space(4);
                if (Get.GravityEffectForce != Vector3.zero)
                    EditorGUILayout.PropertyField(sp_GravH, new GUIContent("H:", sp_GravH.tooltip), GUILayout.Width(42));
                EditorGUIUtility.labelWidth = 0;
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(2);

                EditorGUILayout.EndVertical();




                FGUI_Inspector.FoldHeaderStart(ref _foldoutExtraSettings, " Extra Settings", FGUI_Resources.BGInBoxStyle, FGUI_Resources.Tex_Tweaks, 19);

                if (_foldoutExtraSettings)
                {
                    SerializedProperty sp_freeze = sp_BlendXAxis.Copy();
                    //EditorGUILayout.BeginHorizontal();
                    //EditorGUIUtility.labelWidth = 87;
                    //EditorGUILayout.PropertyField(sp_freeze); sp_freeze.Next(false);
                    //EditorGUILayout.PropertyField(sp_freeze); sp_freeze.Next(false);
                    //EditorGUILayout.PropertyField(sp_freeze); sp_freeze.Next(false);
                    //EditorGUIUtility.labelWidth = 0;
                    //EditorGUILayout.EndHorizontal();

                    GUILayout.Space(6);
                    EditorGUIUtility.labelWidth = 110;
                    EditorGUILayout.PropertyField(sp_freeze); sp_freeze.Next(false);
                    EditorGUILayout.PropertyField(sp_freeze); sp_freeze.Next(false);
                    EditorGUILayout.PropertyField(sp_freeze); sp_freeze.Next(false);

                    EditorGUIUtility.labelWidth = 140;
                    GUILayout.Space(6);
                    EditorGUILayout.PropertyField(sp_freeze); sp_freeze.Next(false);
                    EditorGUILayout.PropertyField(sp_freeze); sp_freeze.Next(false);
                    EditorGUILayout.PropertyField(sp_freeze); sp_freeze.Next(false);
                    EditorGUILayout.PropertyField(sp_freeze); sp_freeze.Next(false);
                    EditorGUILayout.PropertyField(sp_freeze); sp_freeze.Next(false);
                    EditorGUILayout.PropertyField(sp_freeze); sp_freeze.Next(false);
                    GUILayout.Space(5);
                    EditorGUIUtility.labelWidth = 0;

                }

                GUILayout.EndVertical();

                GUILayout.Space(5);

            }
            if (Application.isPlaying) GUI.enabled = e;

            EditorGUILayout.EndVertical();
        }

        bool _foldoutExtraSettings = false;

        private void AddBoneTo(BonesStimulator stim)
        {
            if (stim.Bones == null) stim.Bones = new System.Collections.Generic.List<BonesStimulator.Bone>();

            if (stim.Bones.Count == 0)
            {
                stim.Bones.Add(new BonesStimulator.Bone() { transform = stim.transform });
                EditorUtility.SetDirty(Get);
            }

            Transform lateTr = stim.Bones[stim.Bones.Count - 1].transform;
            if (lateTr)
            {
                if (lateTr.childCount > 0)
                {
                    Transform nextChild = stim.Bones[stim.Bones.Count - 1].transform;
                    if (nextChild != null) if (nextChild.childCount > 0) nextChild = nextChild.GetChild(0);
                    stim.Bones.Add(new BonesStimulator.Bone() { transform = nextChild });
                    EditorUtility.SetDirty(Get);
                }
                else
                {
                    if (Get == stim)
                    {
                        Debug.Log("[Bone Stimulator] Can't find more child transforms!");
                    }
                }
            }
        }

        private void Tab_DrawTweaking()
        {
            EditorGUILayout.BeginHorizontal();
            int height = 26;

            if (GUILayout.Button(Get._editor_DrawTweaking ? "▼" : "►", FGUI_Resources.ButtonStyle, new GUILayoutOption[] { GUILayout.Width(height + 4), GUILayout.Height(height) })) { Get._editor_DrawTweaking = !Get._editor_DrawTweaking; }

            float vv = 0.65f;
            float gAmount = 0.5f;
            if (Get.MovementMuscles > 0f) gAmount = vv;
            if (Get.RotationSpaceMuscles > 0f) gAmount = vv;
            if (gAmount > 0.6f) GUI.color = new Color(0.8f, 1f, 0.8f); else GUI.color = c;

            if (Get._editor_DrawTweaking) if (Get._editor_SelCategory == BonesStimulator.EStimulationMode.Muscles) GUI.color = new Color(0.5f, gAmount, 0.5f, 1f);
            if (GUILayout.Button(new GUIContent(_TexMuscleIcon), FGUI_Resources.ButtonStyle, GUILayout.Height(height))) { Get._editor_SelCategory = BonesStimulator.EStimulationMode.Muscles; Get._editor_DrawTweaking = true; }

            gAmount = 0.5f;
            if (Get.VibrateAmount > 0f) gAmount = vv;
            if (gAmount > 0.6f) GUI.color = new Color(0.8f, 1f, 0.8f); else GUI.color = c;

            if (Get._editor_DrawTweaking) if (Get._editor_SelCategory == BonesStimulator.EStimulationMode.Vibrate) GUI.color = new Color(0.5f, gAmount, 0.5f, 1f);
            if (GUILayout.Button(new GUIContent(_TexRotationAuto), FGUI_Resources.ButtonStyle, GUILayout.Height(height))) { Get._editor_SelCategory = BonesStimulator.EStimulationMode.Vibrate; Get._editor_DrawTweaking = true; }

            gAmount = 0.5f;
            if (Get.SqueezingAmount > 0f) gAmount = vv;
            if (gAmount > 0.6f) GUI.color = new Color(0.8f, 1f, 0.8f); else GUI.color = c;

            if (Get._editor_DrawTweaking) if (Get._editor_SelCategory == BonesStimulator.EStimulationMode.Squeezing) GUI.color = new Color(0.5f, gAmount, 0.5f, 1f);
            if (GUILayout.Button(new GUIContent(_TexSquashing), FGUI_Resources.ButtonStyle, GUILayout.Height(height))) { Get._editor_SelCategory = BonesStimulator.EStimulationMode.Squeezing; Get._editor_DrawTweaking = true; }

            gAmount = 0.5f;
            if (Get.UseCollisions) gAmount = vv;
            if (gAmount > 0.6f) GUI.color = new Color(0.8f, 1f, 0.8f); else GUI.color = c;

            if (Get._editor_DrawTweaking) if (Get._editor_SelCategory == BonesStimulator.EStimulationMode.Collisions) GUI.color = new Color(0.5f, gAmount, 0.5f, 1f);
            if (GUILayout.Button(new GUIContent(FGUI_Resources.Tex_Collider), FGUI_Resources.ButtonStyle, GUILayout.Height(height))) { Get._editor_SelCategory = BonesStimulator.EStimulationMode.Collisions; Get._editor_DrawTweaking = true; }

            EditorGUILayout.EndHorizontal();
            GUI.color = c;

            if (Get._editor_DrawTweaking)
                switch (Get._editor_SelCategory)
                {
                    case BonesStimulator.EStimulationMode.Muscles: Tab_DrawMuscles(); break;
                    case BonesStimulator.EStimulationMode.Vibrate: Tab_DrawAutoRotation(); break;
                    case BonesStimulator.EStimulationMode.Squeezing: Tab_DrawSquashing(); break;
                    case BonesStimulator.EStimulationMode.Collisions: Tab_DrawCollisions(); break;
                }


        }


        private void Tab_DrawMuscles()
        {
            EditorGUILayout.BeginVertical(FGUI_Resources.BGInBoxStyle);
            EditorGUILayout.LabelField(Lang("Muscles") + (Get.RotationSpaceMuscles > 0f || Get.MovementMuscles > 0f ? "" : " (" + Lang("Disabled") + ")"), FGUI_Resources.HeaderStyle);

            GUILayout.Space(4);
            EditorGUILayout.BeginVertical(FGUI_Resources.BGInBoxStyle);
            SerializedProperty p = sp_MovementMuscles.Copy();

            //if (Get.Bones.Count < 2)
            //{
            //    EditorGUILayout.LabelField(new GUIContent(" " + Lang("MusclesReq"), FGUI_Resources.Tex_Info));
            //    GUILayout.Space(4);
            //}

            EditorGUIUtility.labelWidth = 150;

            EditorGUILayout.PropertyField(p, new GUIContent("  Movement Muscles", FGUI_Resources.Tex_Movement, "Using object position and bones position\n(also rotation driven) changes to compute muscles force"));
            EditorGUIUtility.labelWidth = 0;
            p.NextVisible(true);
            EditorGUILayout.PropertyField(p); p.NextVisible(true);
            EditorGUILayout.PropertyField(p); p.NextVisible(true);
            EditorGUILayout.PropertyField(p); p.NextVisible(true);
            EditorGUILayout.PropertyField(p); p.NextVisible(true);
            if (Get.CompensationTransform != null) EditorGUILayout.PropertyField(p);
            EditorGUILayout.EndVertical();

            p = sp_RotationSpaceMuscles.Copy();
            EditorGUILayout.BeginVertical(FGUI_Resources.BGInBoxStyle);
            EditorGUIUtility.labelWidth = 144;
            EditorGUILayout.PropertyField(p, new GUIContent("  Rotation Muscles", FGUI_Resources.Tex_Rotation, "Using only keyframe animation motion and object rotation to compute muscles force"));
            EditorGUIUtility.labelWidth = 132;

            if (p.floatValue > 0f)
            {
                EditorGUILayout.BeginHorizontal();
                p.NextVisible(true); EditorGUILayout.PropertyField(p); p.NextVisible(true);
                if (!Get.UseEulerRotation) EditorGUILayout.PropertyField(p); p.NextVisible(true);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.PropertyField(p); p.NextVisible(true);

                EditorGUILayout.PropertyField(p); p.NextVisible(true);
                EditorGUILayout.PropertyField(p); p.NextVisible(true);
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(FGUI_Resources.BGInBoxStyle);
            EditorGUILayout.PropertyField(sp_MusclesCurve);
            SerializedProperty prp = sp_MusclesCurve.Copy(); prp.NextVisible(false);
            EditorGUIUtility.labelWidth = 162;
            EditorGUILayout.PropertyField(prp);
            if (prp.floatValue < 0f) prp.floatValue = 0f;
            EditorGUILayout.EndVertical();
            EditorGUIUtility.labelWidth = 0;

            EditorGUILayout.EndVertical();
        }

        private void Tab_DrawAutoRotation()
        {
            EditorGUILayout.BeginVertical(FGUI_Resources.BGInBoxStyle);
            EditorGUILayout.LabelField(Lang("Vibrate") + (Get.VibrateAmount <= 0f ? " (" + Lang("Disabled") + ")" : ""), FGUI_Resources.HeaderStyle);
            GUILayout.Space(4);

            SerializedProperty p = sp_VibrateAmount.Copy();

            EditorGUILayout.BeginVertical(FGUI_Resources.BGInBoxStyle);
            EditorGUILayout.PropertyField(p); p.NextVisible(false);
            EditorGUILayout.PropertyField(p); p.NextVisible(false);
            EditorGUILayout.PropertyField(p); p.NextVisible(false);
            EditorGUILayout.PropertyField(p); p.NextVisible(false);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(FGUI_Resources.BGInBoxStyle);
            EditorGUILayout.PropertyField(p); p.NextVisible(false);
            EditorGUILayout.PropertyField(p); p.NextVisible(false);
            EditorGUILayout.PropertyField(p); p.NextVisible(false);
            EditorGUILayout.PropertyField(p); p.NextVisible(false);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(FGUI_Resources.BGInBoxStyle);
            EditorGUILayout.PropertyField(p); p.NextVisible(false);
            if (Get.VibratePosition != 0f) EditorGUILayout.PropertyField(p); p.NextVisible(false);
            if (Get.VibrateRotation != 0f) EditorGUILayout.PropertyField(p); p.NextVisible(false);
            if (Get.VibrateScale != 0f) EditorGUILayout.PropertyField(p);
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndVertical();
        }

        private void Tab_DrawSquashing()
        {
            EditorGUILayout.BeginVertical(FGUI_Resources.BGInBoxStyle);
            EditorGUILayout.LabelField(Lang("Squeeze") + (Get.SqueezingAmount <= 0f ? " (" + Lang("Disabled") + ")" : ""), FGUI_Resources.HeaderStyle);
            GUILayout.Space(4);

            SerializedProperty p = sp_SqueezingAmount.Copy();

            EditorGUILayout.BeginVertical(FGUI_Resources.BGInBoxStyle);
            EditorGUILayout.PropertyField(p); p.NextVisible(true);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(FGUI_Resources.BGInBoxStyle);
            EditorGUILayout.PropertyField(p); p.NextVisible(true);
            EditorGUILayout.PropertyField(p); p.NextVisible(true);
            EditorGUILayout.PropertyField(p);
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndVertical();
        }

        private void Tab_DrawCollisions()
        {
            EditorGUILayout.BeginVertical(FGUI_Resources.BGInBoxStyle);

            //if (Get.MovementMuscles < 0.5f)
            //{
            //    EditorGUILayout.LabelField(new GUIContent("  " + Lang("MusclesNeedEnabled"), FGUI_Resources.Tex_Warning), FGUI_Resources.HeaderStyle);
            //}

            EditorGUILayout.LabelField(Lang("Collision"), FGUI_Resources.HeaderStyle);
            GUILayout.Space(4);

            SerializedProperty p = sp_UseCollisions.Copy();

            EditorGUILayout.BeginVertical(FGUI_Resources.BGInBoxStyle);



            EditorGUILayout.PropertyField(p); p.NextVisible(false); EditorGUIUtility.labelWidth = 180;

            if (Get.UseCollisions)
            {
                if (Get.Bones.Count < 2)
                {
                    GUILayout.Space(2);
                    EditorGUILayout.LabelField(new GUIContent(Lang("CollisionReq"), FGUI_Resources.Tex_Warning));
                    GUILayout.Space(2);
                    GUI.enabled = false;
                }
            }

            if (Get.UseCollisions == false) GUI.enabled = false;
            EditorGUILayout.PropertyField(p); p.NextVisible(false); EditorGUIUtility.labelWidth = 0;
            EditorGUILayout.PropertyField(p); p.NextVisible(false);
            EditorGUILayout.PropertyField(p); p.NextVisible(false);
            EditorGUILayout.PropertyField(p); p.NextVisible(false);
            EditorGUILayout.PropertyField(p); p.NextVisible(false);
            EditorGUILayout.EndVertical();
            GUI.enabled = e;

            if (Get.CollidersScaleMul < 0.01f) Get.CollidersScaleMul = 0.01f;

            El_Colliders();

            EditorGUILayout.EndVertical();
        }
    }
}