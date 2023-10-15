using FIMSpace.FEditor;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace FIMSpace.BonesStimulation
{
    public partial class BonesStimulator_Editor
    {

        private void El_HideWith(SerializedProperty dprop)
        {
            EditorGUILayout.BeginHorizontal();

            LangProp(dprop);

            if (Get.HideWith == null)
            {
                if (GUILayout.Button("Find", new GUILayoutOption[1] { GUILayout.Width(44) }))
                {
                    if (Get.HideWith == null)
                    {
                        Get.HideWith = Get.transform.GetComponent<Renderer>();
                        if (!Get.HideWith) Get.HideWith = Get.transform.GetComponentInChildren<Renderer>();

                        if (!Get.HideWith) if (Get.transform.parent != null)
                            {
                                Transform par = Get.transform.parent;
                                while (par != null)
                                {
                                    Get.HideWith = par.GetComponentInChildren<Renderer>();
                                    if (Get.HideWith != null) break;
                                    par = par.parent;
                                }
                            }
                    }
                }
            }

            EditorGUILayout.EndHorizontal();

            if (Application.isPlaying)
            {
                if (Get.HideWith != null)
                {
                    GUI.color = c * 0.8f;
                    if (Get.HideWith.isVisible)
                        EditorGUILayout.LabelField("Mesh is Visible for Camera");
                    else
                        EditorGUILayout.LabelField("Mesh is Hidden - Stimulator disabled");
                    GUI.color = c;
                }
            }
        }

        private void El_Distance(SerializedProperty dprop)
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUIUtility.labelWidth = 156;
            EditorGUIUtility.fieldWidth = 32;
            LangProp(dprop);
            EditorGUIUtility.fieldWidth = 0;

            if (dprop.floatValue <= 0f)
            {
                GUILayout.FlexibleSpace();
                EditorGUILayout.LabelField("(not used)", GUILayout.Width(60));
                dprop.floatValue = 0f;
            }
            else
            {
                EditorGUIUtility.labelWidth = 6;

                if (Get.DistanceFrom)
                    EditorGUILayout.PropertyField(sp_DistanceFrom, new GUIContent("  ", sp_DistanceFrom.tooltip));
                else
                {
                    GUI.color = new Color(1f, 1f, 1f, 0.75f);
                    Transform camTr = null; if (Camera.main) camTr = Camera.main.transform;
                    Transform fieldTr = (Transform)EditorGUILayout.ObjectField(new GUIContent("  ", "If you want measure fade out distance from other object than main camera put it here"), camTr, typeof(Transform), true);

                    if (fieldTr != null && fieldTr != camTr)
                    {
                        Get.DistanceFrom = fieldTr;
                        EditorUtility.SetDirty(Get);
                    }

                    GUI.color = c;
                }
                EditorGUIUtility.labelWidth = 0;
            }

            EditorGUILayout.EndHorizontal();

            if (Application.isPlaying)
            {
                if (Get.FadeOutDistance > 0f)
                {
                    GUI.color = c * 0.8f;
                    EditorGUILayout.LabelField("Current Distance: " + System.Math.Round(Get.CurrentFadeDistance, 2) + " Blend: " + Mathf.Round(Get.GetEffectBlendWeight() * 100) + "%");
                    GUI.color = c;
                }
            }
        }


        private bool drawInclud = false;
        private void El_Colliders()
        {
            if (Get.IncludedColliders == null) Get.IncludedColliders = new System.Collections.Generic.List<Collider>();
            GUILayout.Space(1f);
            GUI.color = new Color(0.85f, 1f, 0.85f, 1f);
            EditorGUILayout.BeginHorizontal(FGUI_Resources.HeaderBoxStyleH);
            string f = FGUI_Resources.GetFoldSimbol(drawInclud); int inclC = Get.IncludedColliders.Count;
            GUI.color = c;

            GUILayout.Label(new GUIContent(" "), GUILayout.Width(1));
            string inclColFoldTitle = "";

            inclColFoldTitle = Lang("Collide With") + " (" + (inclC == 0 ? "0 !!!" : inclC.ToString()) + ")";

            if (GUILayout.Button(new GUIContent(" " + f + "  " + inclColFoldTitle, FGUI_Resources.TexBehaviourIcon), FGUI_Resources.FoldStyle, GUILayout.Height(24))) drawInclud = !drawInclud;


            EditorGUILayout.EndHorizontal();

            if (drawInclud)
            {
                FGUI_Inspector.VSpace(-3, -5);
                GUI.color = new Color(0.6f, .9f, 0.6f, 1f);
                EditorGUILayout.BeginVertical(FGUI_Resources.BGInBoxStyleH);
                GUI.color = c;
                GUILayout.Space(5f);


                // Drawing colliders from list
                if (Get.IncludedColliders.Count == 0)
                {
                    EditorGUILayout.LabelField("Please add here colliders", FGUI_Resources.HeaderStyle);
                    GUILayout.Space(2f);
                }
                else
                {
                    Get.CheckForColliderDuplicates();

                    EditorGUI.BeginChangeCheck();
                    for (int i = 0; i < Get.IncludedColliders.Count; i++)
                    {
                        EditorGUILayout.BeginHorizontal();

                        if (Get.IncludedColliders[i] != null)
                        {
                            if (!Get.IncludedColliders[i].gameObject.activeInHierarchy) GUI.color = new Color(1f, 1f, 1f, 0.5f);
                            Get.IncludedColliders[i] = (Collider)EditorGUILayout.ObjectField(Get.IncludedColliders[i], typeof(Collider), true);
                            if (Get.IncludedColliders[i].gameObject) if (!Get.IncludedColliders[i].gameObject.activeInHierarchy) GUI.color = c;
                        }

                        if (GUILayout.Button("X", new GUILayoutOption[2] { GUILayout.MaxWidth(22), GUILayout.MaxHeight(16) }))
                        {
                            foreach (var st in SelectedStimulators())
                            {
                                if (st == Get) continue;
                                if ( st.IncludedColliders != null)
                                {
                                    if ( st.IncludedColliders.Contains(Get.IncludedColliders[i]))
                                        st.IncludedColliders.Remove(Get.IncludedColliders[i]);
                                }
                            }

                            Get.IncludedColliders.RemoveAt(i);
                            serializedObject.Update();
                            EditorUtility.SetDirty(Get);
                            serializedObject.ApplyModifiedProperties();
                            return;
                        }

                        EditorGUILayout.EndHorizontal();
                    }

                    if (EditorGUI.EndChangeCheck())
                    {
                        Get.CheckForColliderDuplicates();
                        serializedObject.Update();
                        EditorUtility.SetDirty(Get);
                        serializedObject.ApplyModifiedProperties();
                    }
                }

                GUILayout.Space(6f);

                // Lock button
                GUILayout.BeginVertical();
                if (ActiveEditorTracker.sharedTracker.isLocked) GUI.color = new Color(0.44f, 0.44f, 0.44f, 0.8f); else GUI.color = new Color(0.95f, 0.95f, 0.99f, 0.9f);
                if (GUILayout.Button(new GUIContent("Lock Inspector for Drag & Drop Colliders", "Drag & drop colliders to 'Included Colliders' List from the hierarchy"), FGUI_Resources.ButtonStyle, GUILayout.Height(18))) ActiveEditorTracker.sharedTracker.isLocked = !ActiveEditorTracker.sharedTracker.isLocked;
                GUI.color = c;
                GUILayout.EndVertical();

                // Drag and drop box
                El_DrawDragAndDropCollidersBox();

                GUILayout.Space(3f);

                if (Get.IncludedColliders.Count > 0)
                    EditorGUILayout.HelpBox("You can disable collider components on the objects - Bones Stimulator will still detect collision. If you deactivate the Game Object with collider - Bones Stimulator will not detect collision with it.", MessageType.Info);

                EditorGUILayout.EndVertical();
            }

        }


        void El_DrawDragAndDropCollidersBox()
        {
            GUILayout.Space(3);

            var drop = GUILayoutUtility.GetRect(0f, 38f, new GUILayoutOption[1] { GUILayout.ExpandWidth(true) });
            GUI.color = new Color(0.5f, 1f, 0.5f, 0.9f);
            GUI.Box(drop, "Drag & Drop New Colliders Here", new GUIStyle(EditorStyles.helpBox) { alignment = TextAnchor.MiddleCenter, fixedHeight = 38 });
            GUI.color = c;
            var dropEvent = Event.current;

            switch (dropEvent.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!drop.Contains(dropEvent.mousePosition)) break;

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (dropEvent.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();

                        foreach (var dragged in DragAndDrop.objectReferences)
                        {
                            GameObject draggedObject = dragged as GameObject;

                            if (draggedObject)
                            {
                                Collider[] coll = draggedObject.GetComponents<Collider>();
                                for (int ci = 0; ci < coll.Length; ci++)
                                {
                                    Collider cc = coll[ci];
                                    Get.AddCollider(cc);

                                    foreach (var stim in SelectedStimulators())
                                    {
                                        if (stim == Get) continue;
                                        if (stim.IncludedColliders == null) stim.IncludedColliders = new List<Collider>();
                                        if (!stim.IncludedColliders.Contains(cc)) stim.IncludedColliders.Add(cc);
                                    }
                                }

                                EditorUtility.SetDirty(target);
                            }
                        }

                    }

                    Event.current.Use();
                    break;
            }
        }

        private List<BonesStimulator> SelectedStimulators()
        {
            List<BonesStimulator> stims = new List<BonesStimulator>();

            foreach (var o in Selection.transforms)
            {
                BonesStimulator stim = o.GetComponent<BonesStimulator>();
                if (stim) if (!stims.Contains(stim)) stims.Add(stim);
            }

            return stims;
        }

    }
}