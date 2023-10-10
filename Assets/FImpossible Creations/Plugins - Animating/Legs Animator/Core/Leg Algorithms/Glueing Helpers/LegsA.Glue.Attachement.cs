using UnityEngine;

namespace FIMSpace.FProceduralAnimation
{
    public partial class LegsAnimator
    {
        public partial class Leg
        {
            private GlueAttachement G_Attachement;

            struct GlueAttachement
            {
                public RaycastHit AttachHit;
                public Transform AttachedTo;

                public Vector3 PosInAttachementLocal;
                public Vector3 NormalInAttachementLocal;

                public Quaternion RotInAttachementLocal;
                bool noTransform;

                public GlueAttachement(Leg leg, RaycastHit legGroundHit)
                {
                    AttachHit = legGroundHit;
                    AttachedTo = legGroundHit.transform;

                    if (legGroundHit.transform == null)
                    {
                        noTransform = true;
                        PosInAttachementLocal = legGroundHit.point;
                        NormalInAttachementLocal = legGroundHit.normal;
                        RotInAttachementLocal = leg._PreviousFinalIKRot;
                    }
                    else
                    {
                        noTransform = false;
                        PosInAttachementLocal = legGroundHit.transform.InverseTransformPoint(legGroundHit.point);
                        NormalInAttachementLocal = legGroundHit.transform.InverseTransformDirection(legGroundHit.normal);

                        if (!leg.Owner.AnimateFeet) RotInAttachementLocal = Quaternion.identity;
                        else RotInAttachementLocal = FEngineering.QToLocal(AttachedTo.rotation, leg.GetAlignedOnGroundHitRot(leg._SourceIKRot, legGroundHit.normal));
                    }
                }

                internal Vector3 GetRelevantAlignedHitPoint(Leg leg)
                {
                    Vector3 hit = GetRelevantHitPoint();
                    return leg.GetAlignedOnGroundHitPos(leg.ToRootLocalSpace(hit), hit, GetRelevantNormal());
                }

                internal Vector3 GetRelevantHitPoint()
                {
                    if (noTransform) return PosInAttachementLocal;
                    return AttachedTo.TransformPoint(PosInAttachementLocal);
                }

                internal Vector3 GetRelevantNormal()
                {
                    if (noTransform) return NormalInAttachementLocal;
                    return AttachedTo.TransformDirection(NormalInAttachementLocal);
                }

                internal Quaternion GetRelevantAttachementRotation()
                {
                    if (noTransform) return RotInAttachementLocal;
                    return FEngineering.QToWorld(AttachedTo.rotation, RotInAttachementLocal);
                }

                internal void OverwritePosition(Vector3 legAnimPos)
                {
                    if (AttachedTo == null)
                        PosInAttachementLocal = legAnimPos;
                    else
                        PosInAttachementLocal = AttachedTo.transform.InverseTransformPoint(legAnimPos);
                }
            }
        }

    }
}