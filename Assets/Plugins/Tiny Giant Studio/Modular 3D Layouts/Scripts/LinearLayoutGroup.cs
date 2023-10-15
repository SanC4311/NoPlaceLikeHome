using System.Collections.Generic;
using UnityEngine;
#if MODULAR_3D_TEXT
#endif


namespace TinyGiantStudio.Layout
{
    [HelpURL("https://ferdowsur.gitbook.io/layout-system/layout-group/linear-layout-group")]
    [AddComponentMenu("Tiny Giant Studio/Layout/Linear Layout Group (M3D)", 30003)]
    public class LinearLayoutGroup : LayoutGroup
    {
        #region Variable Declaration
        public float spacing = 0;
        public Alignment alignment = Alignment.HorizontalMiddle;

        private bool startLoopFromEnd = true;

        public enum Alignment
        {
            Top,
            VerticleMiddle,
            Bottom,
            Left,
            HorizontalMiddle,
            Right
        }
        #endregion Variable Declaration



        public override void UpdateLayout(int startRepositioningFrom = 0)
        {
            if (TotalActiveChildCount() == 0)
                return;

            bounds = GetAllChildBounds();
            float x = 0;
            float y = 0;

            GetPositionValuesAccordingToSelectedAlignment(ref x, ref y, bounds);

            GetLoopStart();

            if (startLoopFromEnd)
            {
                for (int i = transform.childCount - 1; i >= 0; i--)
                {
                    if (IgnoreChildBound(bounds, i))
                        continue;

                    SetChildPosition(ref x, ref y, i, bounds[i], startRepositioningFrom);
                }
            }
            else
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    if (IgnoreChildBound(bounds, i))
                        continue;

                    SetChildPosition(ref x, ref y, i, bounds[i], startRepositioningFrom);
                }
            }
        }

        public override List<MeshLayout> GetPositions(List<MeshLayout> meshLayouts)
        {
            if (meshLayouts.Count == 0)
                return null;

            Bounds[] bounds = GetAllChildBounds(meshLayouts);

            float x = 0;
            float y = 0;

            GetPositionValuesAccordingToSelectedAlignment(ref x, ref y, bounds);

            GetLoopStart();

            if (startLoopFromEnd)
            {
                for (int i = meshLayouts.Count - 1; i >= 0; i--)
                {
                    meshLayouts[i] = SetMeshPosition(ref x, ref y, bounds[i], meshLayouts[i]);
                }
            }
            else
            {
                for (int i = 0; i < meshLayouts.Count; i++)
                {
                    meshLayouts[i] = SetMeshPosition(ref x, ref y, bounds[i], meshLayouts[i]);
                }
            }

            return meshLayouts;
        }






        MeshLayout SetMeshPosition(ref float x, ref float y, Bounds bound, MeshLayout meshLayout)
        {
            float toAddX = 0;
            float toAddY = 0;

            if (alignment == Alignment.Bottom || alignment == Alignment.VerticleMiddle)
            {
                toAddY -= (spacing / 2) + (bound.size.y) / 2;
                y -= bound.center.y;
            }
            else if (alignment == Alignment.Top)
            {
                toAddY += (spacing / 2) + (bound.size.y) / 2;
                y -= bound.center.y;
            }
            else if (alignment == Alignment.Left)
            {
                toAddX += (spacing / 2) + (bound.size.x) / 2;
                x -= bound.center.x;
            }
            else if (alignment == Alignment.Right || alignment == Alignment.HorizontalMiddle)
            {
                toAddX -= (spacing / 2) + (bound.size.x) / 2;


                x -= bound.center.x;
            }
            x += toAddX;
            y += toAddY;

            meshLayout.position = RemoveNaNErrorIfAny(new Vector3(x, y, 0));

            //transform.GetChild(i).localPosition = RemoveNaNErrorIfAny(new Vector3(x, y, 0));

            if (alignment == Alignment.Bottom || alignment == Alignment.VerticleMiddle || alignment == Alignment.Top)
            {
                y += bound.center.y;
            }
            else if (alignment == Alignment.Left || alignment == Alignment.HorizontalMiddle || alignment == Alignment.Right)
            {
                x += bound.center.x;
            }
            x += toAddX;
            y += toAddY;

            return meshLayout;
        }

        void SetChildPosition(ref float x, ref float y, int i, Bounds bound, int startRepositioningFrom)
        {
            float toAddX = 0;
            float toAddY = 0;

            if (alignment == Alignment.Bottom || alignment == Alignment.VerticleMiddle)
            {
                toAddY -= (spacing / 2) + (bound.size.y) / 2;
                y -= bound.center.y;
            }
            else if (alignment == Alignment.Top)
            {
                toAddY += (spacing / 2) + (bound.size.y) / 2;
                y -= bound.center.y;
            }
            else if (alignment == Alignment.Left)
            {
                toAddX += (spacing / 2) + (bound.size.x) / 2;
                x -= bound.center.x;
            }
            else if (alignment == Alignment.Right || alignment == Alignment.HorizontalMiddle)
            {
                toAddX -= (spacing / 2) + (bound.size.x) / 2;


                x -= bound.center.x;
            }
            x += toAddX;
            y += toAddY;

            Vector3 targetPosition = RemoveNaNErrorIfAny(new Vector3(x, y, 0));

            if (i >= startRepositioningFrom)
                SetLocalPosition(transform.GetChild(i), targetPosition);

            if (alignment == Alignment.Bottom || alignment == Alignment.VerticleMiddle || alignment == Alignment.Top)
            {
                y += bound.center.y;
            }
            else if (alignment == Alignment.Left || alignment == Alignment.HorizontalMiddle || alignment == Alignment.Right)
            {
                x += bound.center.x;
            }
            x += toAddX;
            y += toAddY;
        }

        void SetLocalPosition(Transform target, Vector3 targetPosition)
        {
            if (elementUpdater.module)
            {
                elementUpdater.module.UpdateLocalPosition(target, elementUpdater.variableHolders, targetPosition);
            }
            else
            {
                if (target.localPosition != targetPosition)
                    target.localPosition = targetPosition;
            }
        }




        void GetPositionValuesAccordingToSelectedAlignment(ref float x, ref float y, Bounds[] bounds)
        {
            if (alignment == Alignment.Bottom)
            {
                y = spacing / 2;
            }
            else if (alignment == Alignment.VerticleMiddle)
            {
                for (int i = 0; i < bounds.Length; i++)
                {
                    if (bounds[i].size == Vector3.zero)
                        continue;

                    y += bounds[i].size.y + spacing;
                }

                y /= 2;
            }
            else if (alignment == Alignment.Top)
            {
                y = -spacing / 2;
            }


            else if (alignment == Alignment.Left)
            {
                x = -spacing / 2;
            }
            else if (alignment == Alignment.HorizontalMiddle)
            {
                for (int i = 0; i < bounds.Length; i++)
                {
                    if (transform.childCount > i)
                        if (transform.GetChild(i))
                            if (transform.GetChild(i).GetComponent<LayoutElement>())
                                if (transform.GetChild(i).GetComponent<LayoutElement>().ignoreElement)
                                    continue;

                    if (bounds[i].size == Vector3.zero)
                        continue;

                    x += bounds[i].size.x + spacing;
                }

                x /= 2;
            }
            else if (alignment == Alignment.Right)
            {
                x = (spacing / 2);
            }
        }

        void GetLoopStart()
        {
            if (alignment == Alignment.Top)
            {
                startLoopFromEnd = true;
            }
            else if (alignment == Alignment.Bottom)
            {
                startLoopFromEnd = false;
            }
            else if (alignment == Alignment.VerticleMiddle)
            {
                startLoopFromEnd = false;
            }

            else if (alignment == Alignment.Left)
            {
                startLoopFromEnd = false;
            }
            else if (alignment == Alignment.HorizontalMiddle)
            {
                startLoopFromEnd = true;
            }
            else if (alignment == Alignment.Right)
            {
                startLoopFromEnd = true;
            }
        }
    }
}