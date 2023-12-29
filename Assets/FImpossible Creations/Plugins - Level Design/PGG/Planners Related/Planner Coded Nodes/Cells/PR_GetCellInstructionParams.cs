using FIMSpace.Graph;
using UnityEngine;
using System.Collections.Generic;

namespace FIMSpace.Generating.Planning.PlannerNodes.Cells
{

    public class PR_GetCellInstructionParams : PlannerRuleBase
    {
        public override string GetDisplayName(float maxWidth = 120) { return wasCreated ? "Instruction Parameters" : "Get Cell Instruction Parameters"; }
        public override string GetNodeTooltipDescription { get { return "Accessing some parameters of provided cell instruction reference"; } }
        public override Color GetNodeColor() { return new Color(0.64f, 0.9f, 0.0f, 0.9f); }
        public override Vector2 NodeSize { get { return new Vector2(200, 120); } }
        public override bool DrawInputConnector { get { return listed != null; } }
        public override bool DrawOutputConnector { get { return listed != null; } }
        public override bool IsFoldable { get { return false; } }

        public override EPlannerNodeType NodeType { get { return EPlannerNodeType.ReadData; } }

        [Port(EPortPinType.Input)] public PGGUniversalPort CellInstruction;
        [Port(EPortPinType.Output, EPortNameDisplay.Default, EPortValueDisplay.NotEditable)] public IntPort InstructionID;
        [Port(EPortPinType.Output, EPortNameDisplay.Default, EPortValueDisplay.NotEditable)] public PGGVector3Port Direction;

        List<SpawnInstructionGuide> listed = null;
        int listedI = 0;

        public override void PreGeneratePrepare()
        {
            listed = null;
            base.PreGeneratePrepare();
        }

        public override void Execute(PlanGenerationPrint print, PlannerResult newResult)
        {
            listed = null;
            base.Execute(print, newResult);
        }

        public override void DONT_USE_IT_YET_OnReadPort(IFGraphPort port)
        {
            // Call only on one of the ports read
            if (InstructionID.IsConnected && port != InstructionID) return;
            else if (InstructionID.IsConnected == false && Direction.IsConnected && port != Direction) return;

            SpawnInstructionGuide guide = null;
            InstructionID.Value = 0;
            Direction.Value = Vector3.zero;

            if (listed == null)
            {
                CellInstruction.TriggerReadPort(true);
                object cellVal = CellInstruction.GetPortValueSafe;


                if (cellVal is SpawnInstructionGuide)
                {
                    guide = cellVal as SpawnInstructionGuide;
                }
                else if (cellVal is List<SpawnInstructionGuide>)
                {
                    listed = cellVal as List<SpawnInstructionGuide>;
                    guide = listed[0];
                }

                if (guide == null) return;

                if (listed != null) // re-execute for list support
                {
                    for (int i = 0; i < listed.Count; i++)
                    {
                        listedI = i;
                        guide = listed[i];
                    }

                    for (int i = 0; i < listed.Count; i++)
                    {
                        listedI = i;
                        guide = listed[i];

                        if (FirstOutputConnection != null)
                        {
                            InstructionID.Value = guide.Id;
                            if (guide.UseDirection) Direction.Value = guide.rot * Vector3.forward;
                            else Direction.Value = Vector3.zero;

                            //UnityEngine.Debug.Log("exec on " + FirstOutputConnection.GetDisplayName() + " with id = " + guide.Id);
                            CallOtherExecution(FirstOutputConnection, null);
                        }
                    }

                    return;
                }

            }
            else // Listed call
            {
                if (listed.ContainsIndex(listedI)) guide = listed[listedI];
            }

            if (guide == null) return;

            //UnityEngine.Debug.Log("listed[" + listedI + "] = " + guide.Id);
            InstructionID.Value = guide.Id;

            if (guide.UseDirection)
                Direction.Value = guide.rot * Vector3.forward;
            else
                Direction.Value = Vector3.zero;

        }

    }
}