using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using RootMotion.FinalIK;
using Studio;
using ToolBox.Extensions;
using UnityEngine;
using UnityEngine.Assertions.Comparers;

namespace HSPE.AMModules
{
    public class IKEditor : AdvancedModeModule
    {
        #region Private Types
        private enum IKType
        {
            Unknown,
            FABRIK,
            FABRIKRoot,
            AimIK,
            CCDIK,
            FullBodyBipedIK
        }

        private class IKWrapper
        {
            public readonly IK ik;
            public readonly IKSolver solver;
            public readonly IKType type;

            public IKWrapper(IK ik)
            {
                this.ik = ik;
                this.solver = this.ik.GetIKSolver();
                if (this.ik is CCDIK)
                    this.type = IKType.CCDIK;
                else if (this.ik is FABRIK)
                    this.type = IKType.FABRIK;
                else if (this.ik is FABRIKRoot)
                    this.type = IKType.FABRIKRoot;
                else if (this.ik is AimIK)
                    this.type = IKType.AimIK;
                else if (this.ik is FullBodyBipedIK)
                    this.type = IKType.FullBodyBipedIK;
            }
        }

        private class IKData
        {
            public EditableValue<bool> originalEnabled;
            public EditableValue<float> originalWeight;

            protected IKData() { }

            protected IKData(IKData other)
            {
                this.originalEnabled = other.originalEnabled;
                this.originalWeight = other.originalWeight;
            }
        }

        private class CCDIKData : IKData
        {
            public EditableValue<float> originalTolerance;
            public EditableValue<int> originalMaxIterations;

            public CCDIKData() { }

            public CCDIKData(CCDIKData other) : base(other)
            {
                this.originalTolerance = other.originalTolerance;
                this.originalMaxIterations = other.originalMaxIterations;
            }
        }

        private class FABRIKData : IKData
        {
            public EditableValue<float> originalTolerance;
            public EditableValue<int> originalMaxIterations;

            public FABRIKData() { }

            public FABRIKData(FABRIKData other) : base(other)
            {
                this.originalTolerance = other.originalTolerance;
                this.originalMaxIterations = other.originalMaxIterations;
            }
        }

        private class FABRIKRootData : IKData
        {
            public EditableValue<float> originalRootPin;
            public EditableValue<int> originalIterations;

            public FABRIKRootData() { }

            public FABRIKRootData(FABRIKRootData other) : base(other)
            {
                this.originalRootPin = other.originalRootPin;
                this.originalIterations = other.originalIterations;
            }
        }

        private class AimIKData : IKData
        {
            public EditableValue<float> originalTolerance;
            public EditableValue<int> originalMaxIterations;
            public EditableValue<Vector3> originalAxis;
            public EditableValue<float> originalClampWeight;
            public EditableValue<int> originalClampSmoothing;
            public EditableValue<Vector3> originalPoleAxis;
            public EditableValue<Vector3> originalPolePosition;
            public EditableValue<float> originalPoleWeight;

            public AimIKData() { }

            public AimIKData(AimIKData other) : base(other)
            {
                this.originalTolerance = other.originalTolerance;
                this.originalMaxIterations = other.originalMaxIterations;
                this.originalAxis = other.originalAxis;
                this.originalClampWeight = other.originalClampWeight;
                this.originalClampSmoothing = other.originalClampSmoothing;
                this.originalPoleAxis = other.originalPoleAxis;
                this.originalPolePosition = other.originalPolePosition;
                this.originalPoleWeight = other.originalPoleWeight;
            }
        }

        private class FullBodyBipedIKData : IKData
        {
            public class EffectorData
            {
                public EditableValue<float> originalPositionWeight;
                public float currentPositionWeight;
                public EditableValue<float> originalRotationWeight;
                public float currentRotationWeight;

                public EffectorData() { }

                public EffectorData(EffectorData other)
                {
                    this.originalPositionWeight = other.originalPositionWeight;
                    this.originalRotationWeight = other.originalRotationWeight;

                    this.currentPositionWeight = other.currentPositionWeight;
                    this.currentRotationWeight = other.currentRotationWeight;
                }
            }

            public class ConstraintBendData
            {
                public EditableValue<float> originalWeight;
                public float currentWeight;

                public ConstraintBendData() { }

                public ConstraintBendData(ConstraintBendData other)
                {
                    this.originalWeight = other.originalWeight;
                    this.currentWeight = other.currentWeight;
                }
            }

            public readonly EffectorData body;
            public readonly EffectorData leftShoulder;
            public readonly EffectorData leftHand;
            public readonly ConstraintBendData leftArm;
            public readonly EffectorData rightShoulder;
            public readonly EffectorData rightHand;
            public readonly ConstraintBendData rightArm;
            public readonly EffectorData leftThigh;
            public readonly EffectorData leftFoot;
            public readonly ConstraintBendData leftLeg;
            public readonly EffectorData rightThigh;
            public readonly EffectorData rightFoot;
            public readonly ConstraintBendData rightLeg;

            public FullBodyBipedIKData()
            {
                this.body = new EffectorData();
                this.leftShoulder = new EffectorData();
                this.leftHand = new EffectorData();
                this.leftArm = new ConstraintBendData();
                this.rightShoulder = new EffectorData();
                this.rightHand = new EffectorData();
                this.rightArm = new ConstraintBendData();
                this.leftThigh = new EffectorData();
                this.leftFoot = new EffectorData();
                this.leftLeg = new ConstraintBendData();
                this.rightThigh = new EffectorData();
                this.rightFoot = new EffectorData();
                this.rightLeg = new ConstraintBendData();
            }

            public FullBodyBipedIKData(FullBodyBipedIKData other) : base(other)
            {
                this.body = new EffectorData(other.body);
                this.leftShoulder = new EffectorData(other.leftShoulder);
                this.leftHand = new EffectorData(other.leftHand);
                this.leftArm = new ConstraintBendData(other.leftArm);
                this.rightShoulder = new EffectorData(other.rightShoulder);
                this.rightHand = new EffectorData(other.rightHand);
                this.rightArm = new ConstraintBendData(other.rightArm);
                this.leftThigh = new EffectorData(other.leftThigh);
                this.leftFoot = new EffectorData(other.leftFoot);
                this.leftLeg = new ConstraintBendData(other.leftLeg);
                this.rightThigh = new EffectorData(other.rightThigh);
                this.rightFoot = new EffectorData(other.rightFoot);
                this.rightLeg = new ConstraintBendData(other.rightLeg);
            }
        }
        #endregion

        #region Private Variables
        private readonly IKWrapper _ik;
        private IKData _ikData;
        private readonly GenericOCITarget _target;
        private readonly bool _isCharacter;
        private Vector2 _scroll;
        #endregion

        #region Public Accessors
        public override AdvancedModeModuleType type { get { return AdvancedModeModuleType.IK; } }
        public override string displayName { get { return "IK"; } }
        public override bool shouldDisplay { get { return this._ik != null && this._ik.type != IKType.Unknown; } }
        #endregion

        #region Unity Methods
        public IKEditor(PoseController parent, GenericOCITarget target) : base(parent)
        {
            this._target = target;
            IK ik = this._parent.GetComponentInChildren<IK>();
            if (ik != null && this._parent._childObjects.Any(o => ik.transform.IsChildOf(o.transform)))
                ik = null;
            if (ik != null)
            {
                this._ik = new IKWrapper(ik);
                switch (this._ik.type)
                {
                    case IKType.FABRIK:
                        this._ikData = new FABRIKData();
                        break;
                    case IKType.FABRIKRoot:
                        this._ikData = new FABRIKRootData();
                        break;
                    case IKType.AimIK:
                        this._ikData = new AimIKData();
                        break;
                    case IKType.CCDIK:
                        this._ikData = new CCDIKData();
                        break;
                    case IKType.FullBodyBipedIK:
                        this._ikData = new FullBodyBipedIKData();
                        break;
                }
                if (this._ik.type == IKType.FullBodyBipedIK)
                    this._parent.onLateUpdate += this.LateUpdate;
                this._isCharacter = this._target.type == GenericOCITarget.Type.Character;
            }
            this._incIndex = -1;
        }

        private void LateUpdate()
        {
            IKSolverFullBodyBiped solver = (IKSolverFullBodyBiped)this._ik.solver;
            FullBodyBipedIKData data = (FullBodyBipedIKData)this._ikData;
            if (this._isCharacter == false || this._target.ociChar.oiCharInfo.enableIK)
            {
                if (this._isCharacter == false || this._target.ociChar.oiCharInfo.activeIK[0])
                    this.ApplyFullBodyBipedEffectorData(solver.bodyEffector, data.body);
                if (this._isCharacter == false || this._target.ociChar.oiCharInfo.activeIK[4])
                {
                    this.ApplyFullBodyBipedEffectorData(solver.leftShoulderEffector, data.leftShoulder);
                    this.ApplyFullBodyBipedConstraintBendData(solver.leftArmChain.bendConstraint, data.leftArm);
                    this.ApplyFullBodyBipedEffectorData(solver.leftHandEffector, data.leftHand);
                }
                if (this._isCharacter == false || this._target.ociChar.oiCharInfo.activeIK[3])
                {
                    this.ApplyFullBodyBipedEffectorData(solver.rightShoulderEffector, data.rightShoulder);
                    this.ApplyFullBodyBipedConstraintBendData(solver.rightArmChain.bendConstraint, data.rightArm);
                    this.ApplyFullBodyBipedEffectorData(solver.rightHandEffector, data.rightHand);
                }
                if (this._isCharacter == false || this._target.ociChar.oiCharInfo.activeIK[2])
                {
                    this.ApplyFullBodyBipedEffectorData(solver.leftThighEffector, data.leftThigh);
                    this.ApplyFullBodyBipedConstraintBendData(solver.leftLegChain.bendConstraint, data.leftLeg);
                    this.ApplyFullBodyBipedEffectorData(solver.leftFootEffector, data.leftFoot);
                }
                if (this._isCharacter == false || this._target.ociChar.oiCharInfo.activeIK[1])
                {
                    this.ApplyFullBodyBipedEffectorData(solver.rightThighEffector, data.rightThigh);
                    this.ApplyFullBodyBipedConstraintBendData(solver.rightLegChain.bendConstraint, data.rightLeg);
                    this.ApplyFullBodyBipedEffectorData(solver.rightFootEffector, data.rightFoot);
                }
            }
        }

        public override void GUILogic()
        {
            GUILayout.BeginVertical(GUI.skin.box);
            this._scroll = GUILayout.BeginScrollView(this._scroll);
            Color c = GUI.color;
            if (this._isCharacter == false)
            {
                bool b = this._ik.ik.enabled;
                GUILayout.BeginHorizontal();
                if (this._ikData.originalEnabled.hasValue)
                    GUI.color = Color.magenta;
                b = GUILayout.Toggle(b, "Enabled", GUILayout.ExpandWidth(false));
                GUI.color = c;

                GUILayout.FlexibleSpace();

                GUI.color = Color.red;
                if (GUILayout.Button("Reset", GUILayout.ExpandWidth(false)))
                {
                    if (this._ikData.originalEnabled.hasValue)
                    {
                        this._ik.ik.enabled = this._ikData.originalEnabled;
                        this._ikData.originalEnabled.Reset();
                        if (this._ik.ik.enabled == false)
                            this._ik.solver.FixTransforms();
                    }
                }
                GUI.color = c;
                GUILayout.EndHorizontal();

                if (b != this._ik.ik.enabled)
                    this.SetIKEnabled(b);
            }

            {
                if (this._ikData.originalWeight.hasValue)
                    GUI.color = Color.magenta;
                float v = this._ik.solver.GetIKPositionWeight();
                v = this.FloatEditor(v, 0f, 1f, "Weight\t\t", "0.0000", onReset: (value) =>
                {
                    if (this._ikData.originalWeight.hasValue)
                    {
                        this._ik.solver.SetIKPositionWeight(this._ikData.originalWeight);
                        this._ikData.originalWeight.Reset();
                        return this._ik.solver.GetIKPositionWeight();
                    }
                    return value;
                });
                GUI.color = c;
                if (!Mathf.Approximately(this._ik.solver.GetIKPositionWeight(), v))
                    this.SetIKPositionWeight(v);
            }

            switch (this._ik.type)
            {
                case IKType.FABRIK:
                    this.FABRIKFields();
                    break;
                case IKType.FABRIKRoot:
                    this.FABRIKRootFields();
                    break;
                case IKType.CCDIK:
                    this.CCDIKFields();
                    break;
                case IKType.AimIK:
                    this.AimIKFields();
                    break;
                case IKType.FullBodyBipedIK:
                    this.FullBodyBipedIKFields();
                    break;
            }
            GUILayout.EndScrollView();

            GUILayout.BeginHorizontal();

            if (this._isCharacter == false && GUILayout.Button("Copy to FK", GUILayout.ExpandWidth(false)))
                this.CopyToFK();

            GUI.color = Color.red;
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Reset", GUILayout.ExpandWidth(false)))
                this.SetNotDirty();
            GUI.color = c;
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            if (this._ik != null && this._ik.type == IKType.FullBodyBipedIK)
                this._parent.onLateUpdate -= this.LateUpdate;
        }
        #endregion

        #region Public Methods
        public void LoadFrom(IKEditor other)
        {
            if (this._ik == null)
                return;
            MainWindow._self.ExecuteDelayed(() =>
            {
                if (this._ik.type != other._ik.type || this._ik.type == IKType.Unknown)
                    return;

                if (other._ikData.originalEnabled.hasValue)
                    this._ik.ik.enabled = other._ik.ik.enabled;
                if (other._ikData.originalWeight.hasValue)
                    this._ik.solver.SetIKPositionWeight(other._ik.solver.GetIKPositionWeight());

                switch (this._ik.type)
                {
                    case IKType.FABRIK:
                        {
                            IKSolverFABRIK otherSolver = (IKSolverFABRIK)other._ik.solver;
                            FABRIKData otherData = (FABRIKData)other._ikData;
                            IKSolverFABRIK solver = (IKSolverFABRIK)this._ik.solver;
                            if (otherData.originalTolerance.hasValue)
                                solver.tolerance = otherSolver.tolerance;
                            if (otherData.originalMaxIterations.hasValue)
                                solver.maxIterations = otherSolver.maxIterations;
                            this._ikData = new FABRIKData(otherData);
                            break;
                        }
                    case IKType.FABRIKRoot:
                        {
                            IKSolverFABRIKRoot otherSolver = (IKSolverFABRIKRoot)other._ik.solver;
                            FABRIKRootData otherData = (FABRIKRootData)other._ikData;
                            IKSolverFABRIKRoot solver = (IKSolverFABRIKRoot)this._ik.solver;
                            if (otherData.originalRootPin.hasValue)
                                solver.rootPin = otherSolver.rootPin;
                            if (otherData.originalIterations.hasValue)
                                solver.iterations = otherSolver.iterations;
                            this._ikData = new FABRIKRootData(otherData);
                            break;
                        }
                    case IKType.AimIK:
                        {
                            IKSolverAim otherSolver = (IKSolverAim)other._ik.solver;
                            AimIKData otherData = (AimIKData)other._ikData;
                            IKSolverAim solver = (IKSolverAim)this._ik.solver;
                            if (otherData.originalTolerance.hasValue)
                                solver.tolerance = otherSolver.tolerance;
                            if (otherData.originalMaxIterations.hasValue)
                                solver.maxIterations = otherSolver.maxIterations;
                            if (otherData.originalAxis.hasValue)
                                solver.axis = otherSolver.axis;
                            if (otherData.originalClampWeight.hasValue)
                                solver.clampWeight = otherSolver.clampWeight;
                            if (otherData.originalClampSmoothing.hasValue)
                                solver.clampSmoothing = otherSolver.clampSmoothing;
                            if (otherData.originalPoleAxis.hasValue)
                                solver.poleAxis = otherSolver.poleAxis;
                            if (otherData.originalPolePosition.hasValue)
                                solver.polePosition = otherSolver.polePosition;
                            if (otherData.originalPoleWeight.hasValue)
                                solver.poleWeight = otherSolver.poleWeight;
                            this._ikData = new AimIKData(otherData);
                            break;
                        }
                    case IKType.CCDIK:
                        {
                            IKSolverCCD otherSolver = (IKSolverCCD)other._ik.solver;
                            CCDIKData otherData = (CCDIKData)other._ikData;
                            IKSolverCCD solver = (IKSolverCCD)this._ik.solver;
                            if (otherData.originalTolerance.hasValue)
                                solver.tolerance = otherSolver.tolerance;
                            if (otherData.originalMaxIterations.hasValue)
                                solver.maxIterations = otherSolver.maxIterations;
                            this._ikData = new CCDIKData(otherData);
                            break;
                        }
                    case IKType.FullBodyBipedIK:
                        {
                            //IKSolverFullBodyBiped otherSolver = (IKSolverFullBodyBiped)other._ik.solver;
                            FullBodyBipedIKData otherData = (FullBodyBipedIKData)other._ikData;
                            //IKSolverFullBodyBiped solver = (IKSolverFullBodyBiped)this._ik.solver;
                            this._ikData = new FullBodyBipedIKData(otherData);
                            break;
                        }
                }

            }, 2);
        }
        #endregion

        #region Private Methods
        private void FABRIKFields()
        {
            IKSolverFABRIK solver = (IKSolverFABRIK)this._ik.solver;
            FABRIKData data = (FABRIKData)this._ikData;
            Color c = GUI.color;

            {
                if (data.originalTolerance.hasValue)
                    GUI.color = Color.magenta;
                float v = solver.tolerance;
                v = this.FloatEditor(v, 0f, 1f, "Tolerance\t\t", "0.0000", onReset: (value) =>
                {
                    if (data.originalTolerance.hasValue)
                    {
                        solver.tolerance = data.originalTolerance;
                        data.originalTolerance.Reset();
                        return solver.tolerance;
                    }
                    return value;
                });
                GUI.color = c;
                if (!Mathf.Approximately(solver.tolerance, v))
                    this.SetFABRIKTolerance(v);
            }

            {
                if (data.originalMaxIterations.hasValue)
                    GUI.color = Color.magenta;
                int v = solver.maxIterations;
                v = Mathf.RoundToInt(this.FloatEditor(v, 0f, 100f, "Max iterations\t", "0", onReset: (value) =>
                {
                    if (data.originalMaxIterations.hasValue)
                    {
                        solver.maxIterations = data.originalMaxIterations;
                        data.originalMaxIterations.Reset();
                        return solver.maxIterations;
                    }
                    return value;
                }));
                GUI.color = c;
                if (v != solver.maxIterations)
                    this.SetFABRIKMaxIterations(v);
            }
        }

        private void FABRIKRootFields()
        {
            IKSolverFABRIKRoot solver = (IKSolverFABRIKRoot)this._ik.solver;
            FABRIKRootData data = (FABRIKRootData)this._ikData;
            Color c = GUI.color;

            {
                if (data.originalRootPin.hasValue)
                    GUI.color = Color.magenta;
                float v = solver.rootPin;
                v = this.FloatEditor(v, 0f, 1f, "Tolerance\t\t", "0.0000", onReset: (value) =>
                {
                    if (data.originalRootPin.hasValue)
                    {
                        solver.rootPin = data.originalRootPin;
                        data.originalRootPin.Reset();
                        return solver.rootPin;
                    }
                    return value;
                });
                GUI.color = c;
                if (!Mathf.Approximately(solver.rootPin, v))
                    this.SetFABRIKRootRootPin(v);
            }

            {
                if (data.originalIterations.hasValue)
                    GUI.color = Color.magenta;
                int v = solver.iterations;
                v = Mathf.RoundToInt(this.FloatEditor(v, 0f, 100f, "Max iterations\t", "0", onReset: (value) =>
                {
                    if (data.originalIterations.hasValue)
                    {
                        solver.iterations = data.originalIterations;
                        data.originalIterations.Reset();
                        return solver.iterations;
                    }
                    return value;
                }));
                GUI.color = c;
                if (v != solver.iterations)
                    this.SetFABRIKRootIterations(v);
            }
        }

        private void CCDIKFields()
        {
            IKSolverCCD solver = (IKSolverCCD)this._ik.solver;
            CCDIKData data = (CCDIKData)this._ikData;
            Color c = GUI.color;

            {
                if (data.originalTolerance.hasValue)
                    GUI.color = Color.magenta;
                float v = solver.tolerance;
                v = this.FloatEditor(v, 0f, 1f, "Tolerance\t\t", "0.0000", onReset: (value) =>
                {
                    if (data.originalTolerance.hasValue)
                    {
                        solver.tolerance = data.originalTolerance;
                        data.originalTolerance.Reset();
                        return solver.tolerance;
                    }
                    return value;
                });
                GUI.color = c;
                if (!Mathf.Approximately(solver.tolerance, v))
                    this.SetCCDIKTolerance(v);
            }

            {
                if (data.originalMaxIterations.hasValue)
                    GUI.color = Color.magenta;
                int v = solver.maxIterations;
                v = Mathf.RoundToInt(this.FloatEditor(v, 0f, 100f, "Max iterations\t", "0", onReset: (value) =>
                {
                    if (data.originalMaxIterations.hasValue)
                    {
                        solver.maxIterations = data.originalMaxIterations;
                        data.originalMaxIterations.Reset();
                        return solver.maxIterations;
                    }
                    return value;
                }));
                GUI.color = c;
                if (v != solver.maxIterations)
                    this.SetCCDIKMaxIterations(v);
            }
        }

        private void AimIKFields()
        {
            IKSolverAim solver = (IKSolverAim)this._ik.solver;
            AimIKData data = (AimIKData)this._ikData;
            Color c = GUI.color;

            {
                if (data.originalTolerance.hasValue)
                    GUI.color = Color.magenta;
                float v = solver.tolerance;
                v = this.FloatEditor(v, 0f, 1f, "Tolerance\t\t", "0.0000", onReset: (value) =>
                {
                    if (data.originalTolerance.hasValue)
                    {
                        solver.tolerance = data.originalTolerance;
                        data.originalTolerance.Reset();
                        return solver.tolerance;
                    }
                    return value;
                });
                GUI.color = c;
                if (!Mathf.Approximately(solver.tolerance, v))
                    this.SetAimIKTolerance(v);
            }

            {
                if (data.originalMaxIterations.hasValue)
                    GUI.color = Color.magenta;
                int v = solver.maxIterations;
                v = Mathf.RoundToInt(this.FloatEditor(v, 0f, 100f, "Max iterations\t", "0", onReset: (value) =>
                {
                    if (data.originalMaxIterations.hasValue)
                    {
                        solver.maxIterations = data.originalMaxIterations;
                        data.originalMaxIterations.Reset();
                        return solver.maxIterations;
                    }
                    return value;
                }));
                GUI.color = c;
                if (v != solver.maxIterations)
                    this.SetAimIKMaxIterations(v);
            }

            {
                GUILayout.BeginVertical();
                if (data.originalAxis.hasValue)
                    GUI.color = Color.magenta;
                GUILayout.Label("Axis");
                GUILayout.BeginHorizontal();
                Vector3 v = this.Vector3Editor(solver.axis);
                if (v != solver.axis)
                    this.SetAimIKAxis(v);
                GUI.color = c;
                this.IncEditor();
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUI.color = Color.red;
                if (GUILayout.Button("Reset", GUILayout.ExpandWidth(false)) && data.originalAxis.hasValue)
                {
                    solver.axis = data.originalAxis;
                    data.originalAxis.Reset();
                }
                GUI.color = c;
                GUILayout.EndHorizontal();

                GUILayout.EndVertical();
            }

            {
                if (data.originalClampWeight.hasValue)
                    GUI.color = Color.magenta;
                float v = solver.clampWeight;
                v = this.FloatEditor(v, 0f, 1f, "Clamp Weight\t", "0.0000", onReset: (value) =>
                {
                    if (data.originalClampWeight.hasValue)
                    {
                        solver.clampWeight = data.originalClampWeight;
                        data.originalClampWeight.Reset();
                        return solver.clampWeight;
                    }
                    return value;
                });
                GUI.color = c;
                if (!Mathf.Approximately(solver.clampWeight, v))
                    this.SetAimIKClampWeight(v);
            }

            {
                if (data.originalClampSmoothing.hasValue)
                    GUI.color = Color.magenta;
                int v = solver.clampSmoothing;
                v = Mathf.RoundToInt(this.FloatEditor(v, 0f, 2f, "Clamp Smoothing\t", "0", onReset: (value) =>
                {
                    if (data.originalClampSmoothing.hasValue)
                    {
                        solver.clampSmoothing = data.originalClampSmoothing;
                        data.originalClampSmoothing.Reset();
                        return solver.clampSmoothing;
                    }
                    return value;
                }));
                GUI.color = c;
                if (solver.clampSmoothing != v)
                    this.SetAimIKClampSmoothing(v);
            }

            {
                GUILayout.BeginVertical();
                if (data.originalPoleAxis.hasValue)
                    GUI.color = Color.magenta;
                GUILayout.Label("Pole Axis");
                GUILayout.BeginHorizontal();
                Vector3 v = this.Vector3Editor(solver.poleAxis);
                if (v != solver.poleAxis)
                    this.SetAimIKPoleAxis(v);
                GUI.color = c;
                this.IncEditor();
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUI.color = Color.red;
                if (GUILayout.Button("Reset", GUILayout.ExpandWidth(false)) && data.originalPoleAxis.hasValue)
                {
                    solver.poleAxis = data.originalPoleAxis;
                    data.originalPoleAxis.Reset();
                }
                GUI.color = c;
                GUILayout.EndHorizontal();

                GUILayout.EndVertical();
            }

            if (solver.poleTarget == null)
            {
                GUILayout.BeginVertical();
                if (data.originalPolePosition.hasValue)
                    GUI.color = Color.magenta;
                GUILayout.Label("Pole Position");

                GUILayout.BeginHorizontal();
                Vector3 v = this.Vector3Editor(solver.polePosition);
                if (v != solver.polePosition)
                    this.SetAimIKPolePosition(v);
                GUI.color = c;
                this.IncEditor();
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUI.color = Color.red;
                if (GUILayout.Button("Reset", GUILayout.ExpandWidth(false)) && data.originalPolePosition.hasValue)
                {
                    solver.polePosition = data.originalPolePosition;
                    data.originalPolePosition.Reset();
                }
                GUI.color = c;
                GUILayout.EndHorizontal();

                GUILayout.EndVertical();
            }

            {
                if (data.originalPoleWeight.hasValue)
                    GUI.color = Color.magenta;
                float v = solver.poleWeight;
                v = this.FloatEditor(v, 0f, 1f, "Pole Weight\t", "0.0000", onReset: (value) =>
                {
                    if (data.originalPoleWeight.hasValue)
                    {
                        solver.poleWeight = data.originalPoleWeight;
                        data.originalPoleWeight.Reset();
                        return solver.poleWeight;
                    }
                    return value;
                });
                GUI.color = c;
                if (!Mathf.Approximately(solver.poleWeight, v))
                    this.SetAimIKPoleWeight(v);
            }
        }

        private void FullBodyBipedIKFields()
        {
            IKSolverFullBodyBiped solver = (IKSolverFullBodyBiped)this._ik.solver;
            FullBodyBipedIKData data = (FullBodyBipedIKData)this._ikData;
            GUILayout.BeginHorizontal();
            this.DisplayEffectorFields("Right shoulder", solver.rightShoulderEffector, data.rightShoulder, true, false);
            this.DisplayEffectorFields("Left shoulder", solver.leftShoulderEffector, data.leftShoulder, true, false);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            this.DisplayConstraintBendFields(solver.rightArmChain.bendConstraint, data.rightArm);
            this.DisplayConstraintBendFields(solver.leftArmChain.bendConstraint, data.leftArm);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            this.DisplayEffectorFields("Right hand", solver.rightHandEffector, data.rightHand);
            this.DisplayEffectorFields("Left hand", solver.leftHandEffector, data.leftHand);
            GUILayout.EndHorizontal();

            this.DisplayEffectorFields("Body", solver.bodyEffector, data.body, true, false);

            GUILayout.BeginHorizontal();
            this.DisplayEffectorFields("Right thigh", solver.rightThighEffector, data.rightThigh, true, false);
            this.DisplayEffectorFields("Left thigh", solver.leftThighEffector, data.leftThigh, true, false);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            this.DisplayConstraintBendFields(solver.rightLegChain.bendConstraint, data.rightLeg);
            this.DisplayConstraintBendFields(solver.leftLegChain.bendConstraint, data.leftLeg);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            this.DisplayEffectorFields("Right foot", solver.rightFootEffector, data.rightFoot);
            this.DisplayEffectorFields("Left foot", solver.leftFootEffector, data.leftFoot);
            GUILayout.EndHorizontal();
        }

        private void DisplayEffectorFields(string name, IKEffector effector, FullBodyBipedIKData.EffectorData data, bool showPos = true, bool showRot = true)
        {
            GUILayout.BeginVertical(GUI.skin.box);
            GUILayout.BeginHorizontal();

            GUILayout.Box(name, GUILayout.ExpandWidth(false));
            //GUILayout.FlexibleSpace();
            //effector.effectChildNodes = GUILayout.Toggle(effector.effectChildNodes, "Affect child nodes", GUILayout.ExpandWidth(false));
            GUILayout.EndHorizontal();
            //effector.maintainRelativePositionWeight = this.FloatEditor(effector.maintainRelativePositionWeight, 0f, 1f, "Maintain relative pos", onReset: value => { return value; });
            Color c = GUI.color;
            if (showPos)
            {
                if (data.originalPositionWeight.hasValue)
                    GUI.color = Color.magenta;
                float newWeight = this.FloatEditor(effector.positionWeight, 0f, 1f, "Pos weight", onReset: value =>
                {
                    if (data.originalPositionWeight.hasValue)
                    {
                        effector.positionWeight = data.originalPositionWeight;
                        data.originalPositionWeight.Reset();
                        return effector.positionWeight;
                    }
                    return value;
                });
                GUI.color = c;
                if (Mathf.Approximately(newWeight, effector.positionWeight) == false)
                    this.SetEffectorPositionWeight(newWeight, effector, data);
            }
            if (showRot)
            {
                if (data.originalRotationWeight.hasValue)
                    GUI.color = Color.magenta;
                float newWeight = this.FloatEditor(effector.rotationWeight, 0f, 1f, "Rot weight", onReset: value =>
                {
                    if (data.originalRotationWeight.hasValue)
                    {
                        effector.rotationWeight = data.originalRotationWeight;
                        data.originalRotationWeight.Reset();
                        return effector.rotationWeight;
                    }
                    return value;
                });
                GUI.color = c;
                if (Mathf.Approximately(newWeight, effector.rotationWeight) == false)
                    this.SetEffectorRotationWeight(newWeight, effector, data);
            }
            GUILayout.EndVertical();
        }

        private void DisplayConstraintBendFields(IKConstraintBend constraint, FullBodyBipedIKData.ConstraintBendData data)
        {
            Color c = GUI.color;
            if (data.originalWeight.hasValue)
                GUI.color = Color.magenta;
            float newWeight = this.FloatEditor(constraint.weight, 0f, 1f, "Bend Weight", onReset: value =>
            {
                if (data.originalWeight.hasValue)
                {
                    constraint.weight = data.originalWeight;
                    data.originalWeight.Reset();
                    return constraint.weight;
                }
                return value;
            });
            GUI.color = c;
            if (Mathf.Approximately(newWeight, constraint.weight) == false)
                this.SetConstraintBendWeight(newWeight, constraint, data);
        }

        private bool GetIKEnabled()
        {
            return this._ik.ik.enabled;
        }

        private void SetIKEnabled(bool b)
        {
            if (this._ikData.originalEnabled.hasValue == false)
                this._ikData.originalEnabled = this._ik.ik.enabled;
            this._ik.ik.enabled = b;
            if (this._ik.ik.enabled == false)
                this._ik.solver.FixTransforms();
        }

        private float GetIKPositionWeight()
        {
            return this._ik.solver.GetIKPositionWeight();
        }

        private void SetIKPositionWeight(float v)
        {
            if (this._ikData.originalWeight.hasValue == false)
                this._ikData.originalWeight = this._ik.solver.GetIKPositionWeight();
            this._ik.solver.SetIKPositionWeight(v);
        }

        private float GetFABRIKTolerance()
        {
            return ((IKSolverFABRIK)this._ik.solver).tolerance;
        }

        private void SetFABRIKTolerance(float v)
        {
            IKSolverFABRIK solver = (IKSolverFABRIK)this._ik.solver;
            FABRIKData data = (FABRIKData)this._ikData;
            if (data.originalTolerance.hasValue == false)
                data.originalTolerance = solver.tolerance;
            solver.tolerance = v;
        }

        private int GetFABRIKMaxIterations()
        {
            return ((IKSolverFABRIK)this._ik.solver).maxIterations;
        }

        private void SetFABRIKMaxIterations(int v)
        {
            IKSolverFABRIK solver = (IKSolverFABRIK)this._ik.solver;
            FABRIKData data = (FABRIKData)this._ikData;
            if (data.originalMaxIterations.hasValue == false)
                data.originalMaxIterations = solver.maxIterations;
            solver.maxIterations = v;
        }

        private float GetFABRIKRootRootPin()
        {
            return ((IKSolverFABRIKRoot)this._ik.solver).rootPin;
        }

        private void SetFABRIKRootRootPin(float v)
        {
            IKSolverFABRIKRoot solver = (IKSolverFABRIKRoot)this._ik.solver;
            FABRIKRootData data = (FABRIKRootData)this._ikData;
            if (data.originalRootPin.hasValue == false)
                data.originalRootPin = solver.rootPin;
            solver.rootPin = v;
        }

        private int GetFABRIKRootIterations()
        {
            return ((IKSolverFABRIKRoot)this._ik.solver).iterations;
        }

        private void SetFABRIKRootIterations(int v)
        {
            IKSolverFABRIKRoot solver = (IKSolverFABRIKRoot)this._ik.solver;
            FABRIKRootData data = (FABRIKRootData)this._ikData;
            if (data.originalIterations.hasValue == false)
                data.originalIterations = solver.iterations;
            solver.iterations = v;
        }
        
        private float GetCCDIKTolerance()
        {
            return ((IKSolverCCD)this._ik.solver).tolerance;
        }

        private void SetCCDIKTolerance(float v)
        {
            IKSolverCCD solver = (IKSolverCCD)this._ik.solver;
            CCDIKData data = (CCDIKData)this._ikData;
            if (data.originalTolerance.hasValue == false)
                data.originalTolerance = solver.tolerance;
            solver.tolerance = v;
        }

        private int GetCCDIKMaxIterations()
        {
            return ((IKSolverCCD)this._ik.solver).maxIterations;
        }

        private void SetCCDIKMaxIterations(int v)
        {
            IKSolverCCD solver = (IKSolverCCD)this._ik.solver;
            CCDIKData data = (CCDIKData)this._ikData;
            if (data.originalMaxIterations.hasValue == false)
                data.originalMaxIterations = solver.maxIterations;
            solver.maxIterations = v;
        }

        private float GetAimIKTolerance()
        {
            return ((IKSolverAim)this._ik.solver).tolerance;
        }

        private void SetAimIKTolerance(float v)
        {
            IKSolverAim solver = (IKSolverAim)this._ik.solver;
            AimIKData data = (AimIKData)this._ikData;
            if (data.originalTolerance.hasValue == false)
                data.originalTolerance = solver.tolerance;
            solver.tolerance = v;
        }

        private int GetAimIKMaxIterations()
        {
            return ((IKSolverAim)this._ik.solver).maxIterations;
        }

        private void SetAimIKMaxIterations(int v)
        {
            IKSolverAim solver = (IKSolverAim)this._ik.solver;
            AimIKData data = (AimIKData)this._ikData;
            if (data.originalMaxIterations.hasValue == false)
                data.originalMaxIterations = solver.maxIterations;
            solver.maxIterations = v;
        }

        private Vector3 GetAimIKAxis()
        {
            return ((IKSolverAim)this._ik.solver).axis;
        }

        private void SetAimIKAxis(Vector3 v)
        {
            IKSolverAim solver = (IKSolverAim)this._ik.solver;
            AimIKData data = (AimIKData)this._ikData;
            if (data.originalAxis.hasValue == false)
                data.originalAxis = solver.axis;
            solver.axis = v;
        }

        private float GetAimIKClampWeight()
        {
            return ((IKSolverAim)this._ik.solver).clampWeight;
        }

        private void SetAimIKClampWeight(float v)
        {
            IKSolverAim solver = (IKSolverAim)this._ik.solver;
            AimIKData data = (AimIKData)this._ikData;
            if (data.originalClampWeight.hasValue == false)
                data.originalClampWeight = solver.clampWeight;
            solver.clampWeight = v;
        }

        private int GetAimIKClampSmoothing()
        {
            return ((IKSolverAim)this._ik.solver).clampSmoothing;
        }

        private void SetAimIKClampSmoothing(int v)
        {
            IKSolverAim solver = (IKSolverAim)this._ik.solver;
            AimIKData data = (AimIKData)this._ikData;
            if (data.originalClampSmoothing.hasValue == false)
                data.originalClampSmoothing = solver.clampSmoothing;
            solver.clampSmoothing = v;
        }

        private Vector3 GetAimIKPoleAxis()
        {
            return ((IKSolverAim)this._ik.solver).poleAxis;
        }

        private void SetAimIKPoleAxis(Vector3 v)
        {
            IKSolverAim solver = (IKSolverAim)this._ik.solver;
            AimIKData data = (AimIKData)this._ikData;
            if (data.originalPoleAxis.hasValue == false)
                data.originalPoleAxis = solver.poleAxis;
            solver.poleAxis = v;
        }

        private Vector3 GetAimIKPolePosition()
        {
            return ((IKSolverAim)this._ik.solver).polePosition;
        }

        private void SetAimIKPolePosition(Vector3 v)
        {
            IKSolverAim solver = (IKSolverAim)this._ik.solver;
            AimIKData data = (AimIKData)this._ikData;
            if (data.originalPolePosition.hasValue == false)
                data.originalPolePosition = solver.polePosition;
            solver.polePosition = v;
        }

        private float GetAimIKPoleWeight()
        {
            return ((IKSolverAim)this._ik.solver).poleWeight;
        }

        private void SetAimIKPoleWeight(float v)
        {
            IKSolverAim solver = (IKSolverAim)this._ik.solver;
            AimIKData data = (AimIKData)this._ikData;
            if (data.originalPoleWeight.hasValue == false)
                data.originalPoleWeight = solver.poleWeight;
            solver.poleWeight = v;
        }

        private float GetEffectorPositionWeight(IKEffector effector)
        {
            return effector.positionWeight;
        }

        private void SetEffectorPositionWeight(float newWeight, IKEffector effector, FullBodyBipedIKData.EffectorData data)
        {
            if (data.originalPositionWeight.hasValue == false)
                data.originalPositionWeight = effector.positionWeight;
            data.currentPositionWeight = newWeight;
        }

        private float GetEffectorRotationWeight(IKEffector effector)
        {
            return effector.rotationWeight;
        }

        private void SetEffectorRotationWeight(float newWeight, IKEffector effector, FullBodyBipedIKData.EffectorData data)
        {
            if (data.originalRotationWeight.hasValue == false)
                data.originalRotationWeight = effector.rotationWeight;
            data.currentRotationWeight = newWeight;
        }

        private float GetConstraintBendWeight(IKConstraintBend constraint)
        {
            return constraint.weight;
        }

        private void SetConstraintBendWeight(float newWeight, IKConstraintBend constraint, FullBodyBipedIKData.ConstraintBendData data)
        {
            if (data.originalWeight.hasValue == false)
                data.originalWeight = constraint.weight;
            data.currentWeight = newWeight;
        }

        private void CopyToFK()
        {
            List<GuideCommand.EqualsInfo> infos = new List<GuideCommand.EqualsInfo>();
            {
                foreach (IKSolver.Point point in this._ik.solver.GetPoints())
                {
                    OCIChar.BoneInfo boneInfo;
                    if (this._target.fkObjects.TryGetValue(point.transform.gameObject, out boneInfo))
                    {
                        Vector3 oldValue = boneInfo.guideObject.changeAmount.rot;
                        boneInfo.guideObject.changeAmount.rot = point.transform.localEulerAngles;
                        infos.Add(new GuideCommand.EqualsInfo()
                        {
                            dicKey = boneInfo.guideObject.dicKey,
                            oldValue = oldValue,
                            newValue = boneInfo.guideObject.changeAmount.rot
                        });
                    }
                }
            }
            UndoRedoManager.Instance.Push(new GuideCommand.RotationEqualsCommand(infos.ToArray()));
        }

        private void ApplyFullBodyBipedEffectorData(IKEffector effector, FullBodyBipedIKData.EffectorData data)
        {
            if (data.originalPositionWeight.hasValue)
                effector.positionWeight = data.currentPositionWeight;
            if (data.originalRotationWeight.hasValue)
                effector.rotationWeight = data.currentRotationWeight;
        }

        private void ApplyFullBodyBipedConstraintBendData(IKConstraintBend constraint, FullBodyBipedIKData.ConstraintBendData data)
        {
            if (data.originalWeight.hasValue)
                constraint.weight = data.currentWeight;
        }
        #endregion

        #region Saves
        public override int SaveXml(XmlTextWriter xmlWriter)
        {
            if (this._ik == null || this._ik.type == IKType.Unknown)
                return 0;

            int written = 0;
            {
                xmlWriter.WriteStartElement("iks");
                {
                    xmlWriter.WriteStartElement("ik");
                    xmlWriter.WriteAttributeString("root", this._ik.ik.transform.GetPathFrom(this._parent.transform));
                    xmlWriter.WriteAttributeString("type", XmlConvert.ToString((int)this._ik.type));

                    if (this._ikData.originalEnabled.hasValue)
                        xmlWriter.WriteAttributeString("enabled", XmlConvert.ToString(this._ik.ik.enabled));
                    if (this._ikData.originalWeight.hasValue)
                        xmlWriter.WriteAttributeString("weight", XmlConvert.ToString(this._ik.solver.GetIKPositionWeight()));

                    switch (this._ik.type)
                    {
                        case IKType.FABRIK:
                            {
                                IKSolverFABRIK solver = (IKSolverFABRIK)this._ik.solver;
                                FABRIKData data = (FABRIKData)this._ikData;
                                if (data.originalTolerance.hasValue)
                                    xmlWriter.WriteAttributeString("tolerance", XmlConvert.ToString(solver.tolerance));
                                if (data.originalMaxIterations.hasValue)
                                    xmlWriter.WriteAttributeString("maxIterations", XmlConvert.ToString(solver.maxIterations));
                                break;
                            }
                        case IKType.FABRIKRoot:
                            {
                                IKSolverFABRIKRoot solver = (IKSolverFABRIKRoot)this._ik.solver;
                                FABRIKRootData data = (FABRIKRootData)this._ikData;
                                if (data.originalRootPin.hasValue)
                                    xmlWriter.WriteAttributeString("tolerance", XmlConvert.ToString(solver.rootPin));
                                if (data.originalIterations.hasValue)
                                    xmlWriter.WriteAttributeString("maxIterations", XmlConvert.ToString(solver.iterations));
                                break;
                            }
                        case IKType.AimIK:
                            {
                                IKSolverAim solver = (IKSolverAim)this._ik.solver;
                                AimIKData data = (AimIKData)this._ikData;
                                if (data.originalTolerance.hasValue)
                                    xmlWriter.WriteAttributeString("tolerance", XmlConvert.ToString(solver.tolerance));
                                if (data.originalMaxIterations.hasValue)
                                    xmlWriter.WriteAttributeString("maxIterations", XmlConvert.ToString(solver.maxIterations));
                                if (data.originalAxis.hasValue)
                                {
                                    xmlWriter.WriteAttributeString("axisX", XmlConvert.ToString(solver.axis.x));
                                    xmlWriter.WriteAttributeString("axisY", XmlConvert.ToString(solver.axis.y));
                                    xmlWriter.WriteAttributeString("axisZ", XmlConvert.ToString(solver.axis.z));
                                }
                                if (data.originalClampWeight.hasValue)
                                    xmlWriter.WriteAttributeString("clampWeight", XmlConvert.ToString(solver.clampWeight));
                                if (data.originalClampSmoothing.hasValue)
                                    xmlWriter.WriteAttributeString("clampSmoothing", XmlConvert.ToString(solver.clampSmoothing));
                                if (data.originalPoleAxis.hasValue)
                                {
                                    xmlWriter.WriteAttributeString("poleAxisX", XmlConvert.ToString(solver.poleAxis.x));
                                    xmlWriter.WriteAttributeString("poleAxisY", XmlConvert.ToString(solver.poleAxis.y));
                                    xmlWriter.WriteAttributeString("poleAxisZ", XmlConvert.ToString(solver.poleAxis.z));
                                }
                                if (data.originalPolePosition.hasValue)
                                {
                                    xmlWriter.WriteAttributeString("polePositionX", XmlConvert.ToString(solver.polePosition.x));
                                    xmlWriter.WriteAttributeString("polePositionY", XmlConvert.ToString(solver.polePosition.y));
                                    xmlWriter.WriteAttributeString("polePositionZ", XmlConvert.ToString(solver.polePosition.z));
                                }
                                if (data.originalPoleWeight.hasValue)
                                    xmlWriter.WriteAttributeString("poleWeight", XmlConvert.ToString(solver.poleWeight));

                                break;
                            }
                        case IKType.CCDIK:
                            {
                                IKSolverCCD solver = (IKSolverCCD)this._ik.solver;
                                CCDIKData data = (CCDIKData)this._ikData;
                                if (data.originalTolerance.hasValue)
                                    xmlWriter.WriteAttributeString("tolerance", XmlConvert.ToString(solver.tolerance));
                                if (data.originalMaxIterations.hasValue)
                                    xmlWriter.WriteAttributeString("maxIterations", XmlConvert.ToString(solver.maxIterations));
                                break;
                            }
                        case IKType.FullBodyBipedIK:
                            {
                                FullBodyBipedIKData data = (FullBodyBipedIKData)this._ikData;
                                xmlWriter.WriteStartElement("bodyEffector");
                                this.SaveFullBodyBipedEffectorData(xmlWriter, data.body);
                                xmlWriter.WriteEndElement();

                                xmlWriter.WriteStartElement("leftShoulderEffector");
                                this.SaveFullBodyBipedEffectorData(xmlWriter, data.leftShoulder);
                                xmlWriter.WriteEndElement();

                                xmlWriter.WriteStartElement("leftHandEffector");
                                this.SaveFullBodyBipedEffectorData(xmlWriter, data.leftHand);
                                xmlWriter.WriteEndElement();

                                xmlWriter.WriteStartElement("rightShoulderEffector");
                                this.SaveFullBodyBipedEffectorData(xmlWriter, data.rightShoulder);
                                xmlWriter.WriteEndElement();

                                xmlWriter.WriteStartElement("rightHandEffector");
                                this.SaveFullBodyBipedEffectorData(xmlWriter, data.rightHand);
                                xmlWriter.WriteEndElement();

                                xmlWriter.WriteStartElement("leftThighEffector");
                                this.SaveFullBodyBipedEffectorData(xmlWriter, data.leftThigh);
                                xmlWriter.WriteEndElement();

                                xmlWriter.WriteStartElement("leftFootEffector");
                                this.SaveFullBodyBipedEffectorData(xmlWriter, data.leftFoot);
                                xmlWriter.WriteEndElement();

                                xmlWriter.WriteStartElement("rightThighEffector");
                                this.SaveFullBodyBipedEffectorData(xmlWriter, data.rightThigh);
                                xmlWriter.WriteEndElement();

                                xmlWriter.WriteStartElement("rightFootEffector");
                                this.SaveFullBodyBipedEffectorData(xmlWriter, data.rightFoot);
                                xmlWriter.WriteEndElement();

                                xmlWriter.WriteStartElement("leftArmConstraint");
                                this.SaveFullBodyBipedConstraintBend(xmlWriter, data.leftArm);
                                xmlWriter.WriteEndElement();

                                xmlWriter.WriteStartElement("rightArmConstraint");
                                this.SaveFullBodyBipedConstraintBend(xmlWriter, data.rightArm);
                                xmlWriter.WriteEndElement();

                                xmlWriter.WriteStartElement("leftLegConstraint");
                                this.SaveFullBodyBipedConstraintBend(xmlWriter, data.leftLeg);
                                xmlWriter.WriteEndElement();

                                xmlWriter.WriteStartElement("rightLegConstraint");
                                this.SaveFullBodyBipedConstraintBend(xmlWriter, data.rightLeg);
                                xmlWriter.WriteEndElement();

                                break;
                            }
                    }
                    xmlWriter.WriteEndElement();
                    ++written;
                }
                xmlWriter.WriteEndElement();
            }

            return written;
        }

        private void SaveFullBodyBipedEffectorData(XmlTextWriter writer, FullBodyBipedIKData.EffectorData data)
        {
            if (data.originalPositionWeight.hasValue)
                writer.WriteAttributeString("positionWeight", XmlConvert.ToString(data.currentPositionWeight));
            if (data.originalRotationWeight.hasValue)
                writer.WriteAttributeString("rotationWeight", XmlConvert.ToString(data.currentRotationWeight));
        }

        private void SaveFullBodyBipedConstraintBend(XmlTextWriter writer, FullBodyBipedIKData.ConstraintBendData data)
        {
            if (data.originalWeight.hasValue)
                writer.WriteAttributeString("weight", XmlConvert.ToString(data.currentWeight));
        }

        public override bool LoadXml(XmlNode xmlNode)
        {
            if (this._ik == null || this._ik.type == IKType.Unknown)
                return false;
            bool changed = false;

            this.ResetAll();

            XmlNode ikNodes = xmlNode.FindChildNode("iks");

            if (ikNodes != null)
            {
                foreach (XmlNode node in ikNodes.ChildNodes)
                {
                    try
                    {
                        string root = node.Attributes["root"].Value; // For the future maybe.
                        if (XmlConvert.ToInt32(node.Attributes["type"].Value) != (int)this._ik.type)
                            continue;

                        if (node.Attributes["enabled"] != null)
                        {
                            bool enabled = XmlConvert.ToBoolean(node.Attributes["enabled"].Value);
                            this._ikData.originalEnabled = this._ik.ik.enabled;
                            this._ik.ik.enabled = enabled;
                            changed = true;
                        }

                        if (node.Attributes["weight"] != null)
                        {
                            float weight = XmlConvert.ToSingle(node.Attributes["weight"].Value);
                            this._ikData.originalWeight = this._ik.solver.GetIKPositionWeight();
                            this._ik.solver.SetIKPositionWeight(weight);
                            changed = true;
                        }

                        switch (this._ik.type)
                        {
                            case IKType.FABRIK:
                                {
                                    IKSolverFABRIK solver = (IKSolverFABRIK)this._ik.solver;
                                    FABRIKData data = (FABRIKData)this._ikData;
                                    if (node.Attributes["tolerance"] != null)
                                    {
                                        float tolerance = XmlConvert.ToSingle(node.Attributes["tolerance"].Value);
                                        data.originalTolerance = solver.tolerance;
                                        solver.tolerance = tolerance;
                                        changed = true;
                                    }
                                    if (node.Attributes["maxIterations"] != null)
                                    {
                                        int maxIterations = XmlConvert.ToInt32(node.Attributes["maxIterations"].Value);
                                        data.originalMaxIterations = solver.maxIterations;
                                        solver.maxIterations = maxIterations;
                                        changed = true;
                                    }
                                    break;
                                }
                            case IKType.FABRIKRoot:
                                {
                                    IKSolverFABRIKRoot solver = (IKSolverFABRIKRoot)this._ik.solver;
                                    FABRIKRootData data = (FABRIKRootData)this._ikData;
                                    if (node.Attributes["tolerance"] != null)
                                    {
                                        float tolerance = XmlConvert.ToSingle(node.Attributes["tolerance"].Value);
                                        data.originalRootPin = solver.rootPin;
                                        solver.rootPin = tolerance;
                                        changed = true;
                                    }
                                    if (node.Attributes["maxIterations"] != null)
                                    {
                                        int maxIterations = XmlConvert.ToInt32(node.Attributes["maxIterations"].Value);
                                        data.originalIterations = solver.iterations;
                                        solver.iterations = maxIterations;
                                        changed = true;
                                    }
                                    break;
                                }
                            case IKType.AimIK:
                                {
                                    IKSolverAim solver = (IKSolverAim)this._ik.solver;
                                    AimIKData data = (AimIKData)this._ikData;
                                    if (node.Attributes["tolerance"] != null)
                                    {
                                        float tolerance = XmlConvert.ToSingle(node.Attributes["tolerance"].Value);
                                        data.originalTolerance = solver.tolerance;
                                        solver.tolerance = tolerance;
                                        changed = true;
                                    }
                                    if (node.Attributes["maxIterations"] != null)
                                    {
                                        int maxIterations = XmlConvert.ToInt32(node.Attributes["maxIterations"].Value);
                                        data.originalMaxIterations = solver.maxIterations;
                                        solver.maxIterations = maxIterations;
                                        changed = true;
                                    }
                                    if (node.Attributes["axisX"] != null)
                                    {
                                        Vector3 axis = new Vector3(
                                                XmlConvert.ToSingle(node.Attributes["axisX"].Value),
                                                XmlConvert.ToSingle(node.Attributes["axisY"].Value),
                                                XmlConvert.ToSingle(node.Attributes["axisZ"].Value)
                                        );
                                        data.originalAxis = solver.axis;
                                        solver.axis = axis;
                                        changed = true;
                                    }
                                    if (node.Attributes["clampWeight"] != null)
                                    {
                                        float clampWeight = XmlConvert.ToSingle(node.Attributes["clampWeight"].Value);
                                        data.originalClampWeight = solver.clampWeight;
                                        solver.clampWeight = clampWeight;
                                        changed = true;
                                    }
                                    if (node.Attributes["clampSmoothing"] != null)
                                    {
                                        int clampSmoothing = XmlConvert.ToInt32(node.Attributes["clampSmoothing"].Value);
                                        data.originalClampSmoothing = solver.clampSmoothing;
                                        solver.clampSmoothing = clampSmoothing;
                                        changed = true;
                                    }
                                    if (node.Attributes["poleAxisX"] != null)
                                    {
                                        Vector3 poleAxis = new Vector3(
                                                XmlConvert.ToSingle(node.Attributes["poleAxisX"].Value),
                                                XmlConvert.ToSingle(node.Attributes["poleAxisY"].Value),
                                                XmlConvert.ToSingle(node.Attributes["poleAxisZ"].Value)
                                        );
                                        data.originalPoleAxis = solver.poleAxis;
                                        solver.poleAxis = poleAxis;
                                        changed = true;
                                    }
                                    if (node.Attributes["polePositionX"] != null)
                                    {
                                        Vector3 polePosition = new Vector3(
                                                XmlConvert.ToSingle(node.Attributes["polePositionX"].Value),
                                                XmlConvert.ToSingle(node.Attributes["polePositionY"].Value),
                                                XmlConvert.ToSingle(node.Attributes["polePositionZ"].Value)
                                        );
                                        data.originalPolePosition = solver.polePosition;
                                        solver.polePosition = polePosition;
                                        changed = true;
                                    }
                                    if (node.Attributes["poleWeight"] != null)
                                    {
                                        float poleWeight = XmlConvert.ToSingle(node.Attributes["poleWeight"].Value);
                                        data.originalPoleWeight = solver.poleWeight;
                                        solver.poleWeight = poleWeight;
                                        changed = true;
                                    }
                                    break;
                                }
                            case IKType.CCDIK:
                                {
                                    IKSolverCCD solver = (IKSolverCCD)this._ik.solver;
                                    CCDIKData data = (CCDIKData)this._ikData;
                                    if (node.Attributes["tolerance"] != null)
                                    {
                                        float tolerance = XmlConvert.ToSingle(node.Attributes["tolerance"].Value);
                                        data.originalTolerance = solver.tolerance;
                                        solver.tolerance = tolerance;
                                    }
                                    if (node.Attributes["maxIterations"] != null)
                                    {
                                        int maxIterations = XmlConvert.ToInt32(node.Attributes["maxIterations"].Value);
                                        data.originalMaxIterations = solver.maxIterations;
                                        solver.maxIterations = maxIterations;
                                        changed = true;
                                    }
                                    break;
                                }
                            case IKType.FullBodyBipedIK:
                                {
                                    IKSolverFullBodyBiped solver = (IKSolverFullBodyBiped)this._ik.solver;
                                    FullBodyBipedIKData data = (FullBodyBipedIKData)this._ikData;
                                    changed = this.LoadFullBodyBipedEffectorData(node.FindChildNode("bodyEffector"), solver.bodyEffector, data.body) || changed;
                                    changed = this.LoadFullBodyBipedEffectorData(node.FindChildNode("leftShoulderEffector"), solver.leftShoulderEffector, data.leftShoulder) || changed;
                                    changed = this.LoadFullBodyBipedEffectorData(node.FindChildNode("leftHandEffector"), solver.leftHandEffector, data.leftHand) || changed;
                                    changed = this.LoadFullBodyBipedEffectorData(node.FindChildNode("rightShoulderEffector"), solver.rightShoulderEffector, data.rightShoulder) || changed;
                                    changed = this.LoadFullBodyBipedEffectorData(node.FindChildNode("rightHandEffector"), solver.rightHandEffector, data.rightHand) || changed;
                                    changed = this.LoadFullBodyBipedEffectorData(node.FindChildNode("leftThighEffector"), solver.leftThighEffector, data.leftThigh) || changed;
                                    changed = this.LoadFullBodyBipedEffectorData(node.FindChildNode("leftFootEffector"), solver.leftFootEffector, data.leftFoot) || changed;
                                    changed = this.LoadFullBodyBipedEffectorData(node.FindChildNode("rightThighEffector"), solver.rightThighEffector, data.rightThigh) || changed;
                                    changed = this.LoadFullBodyBipedEffectorData(node.FindChildNode("rightFootEffector"), solver.rightFootEffector, data.rightFoot) || changed;

                                    changed = this.LoadFullBodyBipedConstraintBendData(node.FindChildNode("leftArmConstraint"), solver.leftArmChain.bendConstraint, data.leftArm) || changed;
                                    changed = this.LoadFullBodyBipedConstraintBendData(node.FindChildNode("rightArmConstraint"), solver.rightArmChain.bendConstraint, data.rightArm) || changed;
                                    changed = this.LoadFullBodyBipedConstraintBendData(node.FindChildNode("leftLegConstraint"), solver.leftLegChain.bendConstraint, data.leftLeg) || changed;
                                    changed = this.LoadFullBodyBipedConstraintBendData(node.FindChildNode("rightLegConstraint"), solver.rightLegChain.bendConstraint, data.rightLeg) || changed;
                                    break;
                                }
                        }
                    }
                    catch (Exception e)
                    {
                        UnityEngine.Debug.LogError("HSPE: Couldn't load ik for object " + this._parent.name + " " + node.OuterXml + "\n" + e);
                    }
                }
            }

            return changed;
        }

        private bool LoadFullBodyBipedEffectorData(XmlNode node, IKEffector effector, FullBodyBipedIKData.EffectorData data)
        {
            if (node == null)
                return false;
            bool changed = false;
            if (node.Attributes["positionWeight"] != null)
            {
                float positionWeight = XmlConvert.ToSingle(node.Attributes["positionWeight"].Value);
                data.originalPositionWeight = effector.positionWeight;
                data.currentPositionWeight = positionWeight;
                changed = true;
            }
            if (node.Attributes["rotationWeight"] != null)
            {
                float rotationWeight = XmlConvert.ToSingle(node.Attributes["rotationWeight"].Value);
                data.originalRotationWeight = effector.rotationWeight;
                data.currentRotationWeight = rotationWeight;
                changed = true;
            }
            return changed;
        }

        private bool LoadFullBodyBipedConstraintBendData(XmlNode node, IKConstraintBend constraint, FullBodyBipedIKData.ConstraintBendData data)
        {
            if (node == null)
                return false;
            bool changed = false;
            if (node.Attributes["weight"] != null)
            {
                float weight = XmlConvert.ToSingle(node.Attributes["weight"].Value);
                data.originalWeight = constraint.weight;
                data.currentWeight = weight;
                changed = true;
            }
            return changed;
        }

        private void SetNotDirty()
        {
            if (this._ikData.originalEnabled.hasValue)
            {
                this._ik.ik.enabled = this._ikData.originalEnabled;
                this._ikData.originalEnabled.Reset();
            }

            if (this._ikData.originalWeight.hasValue)
            {
                this._ik.solver.SetIKPositionWeight(this._ikData.originalWeight);
                this._ikData.originalWeight.Reset();
            }

            switch (this._ik.type)
            {
                case IKType.FABRIK:
                    {
                        IKSolverFABRIK solver = (IKSolverFABRIK)this._ik.solver;
                        FABRIKData data = (FABRIKData)this._ikData;
                        if (data.originalTolerance.hasValue)
                        {
                            solver.tolerance = data.originalTolerance;
                            data.originalTolerance.Reset();
                        }
                        if (data.originalMaxIterations.hasValue)
                        {
                            solver.maxIterations = data.originalMaxIterations;
                            data.originalMaxIterations.Reset();
                        }
                        break;
                    }
                case IKType.FABRIKRoot:
                    {
                        IKSolverFABRIKRoot solver = (IKSolverFABRIKRoot)this._ik.solver;
                        FABRIKRootData data = (FABRIKRootData)this._ikData;
                        if (data.originalRootPin.hasValue)
                        {
                            solver.rootPin = data.originalRootPin;
                            data.originalRootPin.Reset();
                        }
                        if (data.originalIterations.hasValue)
                        {
                            solver.iterations = data.originalIterations;
                            data.originalIterations.Reset();
                        }
                        break;
                    }
                case IKType.AimIK:
                    {
                        IKSolverAim solver = (IKSolverAim)this._ik.solver;
                        AimIKData data = (AimIKData)this._ikData;
                        if (data.originalTolerance.hasValue)
                        {
                            solver.tolerance = data.originalTolerance;
                            data.originalTolerance.Reset();
                        }
                        if (data.originalMaxIterations.hasValue)
                        {
                            solver.maxIterations = data.originalMaxIterations;
                            data.originalMaxIterations.Reset();
                        }
                        if (data.originalAxis.hasValue)
                        {
                            solver.axis = data.originalAxis;
                            data.originalAxis.Reset();
                        }
                        if (data.originalClampWeight.hasValue)
                        {
                            solver.clampWeight = data.originalClampWeight;
                            data.originalClampWeight.Reset();
                        }
                        if (data.originalClampSmoothing.hasValue)
                        {
                            solver.clampSmoothing = data.originalClampSmoothing;
                            data.originalClampSmoothing.Reset();
                        }
                        if (data.originalPoleAxis.hasValue)
                        {
                            solver.poleAxis = data.originalPoleAxis;
                            data.originalPoleAxis.Reset();
                        }
                        if (data.originalPolePosition.hasValue)
                        {
                            solver.polePosition = data.originalPolePosition;
                            data.originalPolePosition.Reset();
                        }
                        if (data.originalPoleWeight.hasValue)
                        {
                            solver.poleWeight = data.originalPoleWeight;
                            data.originalPoleWeight.Reset();
                        }
                        break;
                    }
                case IKType.CCDIK:
                    {
                        IKSolverCCD solver = (IKSolverCCD)this._ik.solver;
                        CCDIKData data = (CCDIKData)this._ikData;
                        if (data.originalTolerance.hasValue)
                        {
                            solver.tolerance = data.originalTolerance;
                            data.originalTolerance.Reset();
                        }
                        if (data.originalMaxIterations.hasValue)
                        {
                            solver.maxIterations = data.originalMaxIterations;
                            data.originalMaxIterations.Reset();
                        }
                        break;
                    }
                case IKType.FullBodyBipedIK:
                    {
                        IKSolverFullBodyBiped solver = (IKSolverFullBodyBiped)this._ik.solver;
                        FullBodyBipedIKData data = (FullBodyBipedIKData)this._ikData;
                        this.SetFullBodyBipedEffectorNotDirty(solver.bodyEffector, data.body);
                        this.SetFullBodyBipedEffectorNotDirty(solver.leftShoulderEffector, data.leftShoulder);
                        this.SetFullBodyBipedEffectorNotDirty(solver.leftHandEffector, data.leftHand);
                        this.SetFullBodyBipedEffectorNotDirty(solver.rightShoulderEffector, data.rightShoulder);
                        this.SetFullBodyBipedEffectorNotDirty(solver.rightHandEffector, data.rightHand);
                        this.SetFullBodyBipedEffectorNotDirty(solver.leftThighEffector, data.leftThigh);
                        this.SetFullBodyBipedEffectorNotDirty(solver.leftFootEffector, data.leftFoot);
                        this.SetFullBodyBipedEffectorNotDirty(solver.rightThighEffector, data.rightThigh);
                        this.SetFullBodyBipedEffectorNotDirty(solver.rightFootEffector, data.rightFoot);

                        this.SetFullBodyBipedConstraintBendNotDirty(solver.leftArmChain.bendConstraint, data.leftArm);
                        this.SetFullBodyBipedConstraintBendNotDirty(solver.rightArmChain.bendConstraint, data.rightArm);
                        this.SetFullBodyBipedConstraintBendNotDirty(solver.leftLegChain.bendConstraint, data.leftLeg);
                        this.SetFullBodyBipedConstraintBendNotDirty(solver.rightLegChain.bendConstraint, data.rightLeg);
                        break;
                    }
            }
        }

        private void SetFullBodyBipedEffectorNotDirty(IKEffector effector, FullBodyBipedIKData.EffectorData data)
        {
            if (data.originalPositionWeight.hasValue)
            {
                effector.positionWeight = data.originalPositionWeight;
                data.originalPositionWeight.Reset();
            }
            if (data.originalRotationWeight.hasValue)
            {
                effector.rotationWeight = data.originalRotationWeight;
                data.originalRotationWeight.Reset();
            }
        }

        private void SetFullBodyBipedConstraintBendNotDirty(IKConstraintBend constraint, FullBodyBipedIKData.ConstraintBendData data)
        {
            if (data.originalWeight.hasValue)
            {
                constraint.weight = data.originalWeight;
                data.originalWeight.Reset();
            }
        }

        private void ResetAll()
        {
            this.SetNotDirty();
        }
        #endregion

        #region Timeline Compatibility
        internal static class TimelineCompatibility
        {
            public static void Populate()
            {
                UnityEngine.Debug.LogError("AIPE.IKEditor.TimelineCompatibility.Populate");
                ToolBox.TimelineCompatibility.AddInterpolableModelDynamic(
                        owner: HSPE._name,
                        id: "ikEnabled",
                        name: "IK Enabled",
                        interpolateBefore: (oci, parameter, leftValue, rightValue, factor) =>
                        {
                            IKEditor editor = (IKEditor)parameter;
                            bool newEnabled = (bool)leftValue;
                            if (editor.GetIKEnabled() != newEnabled)
                                editor.SetIKEnabled(newEnabled);
                        },
                        interpolateAfter: null,
                        isCompatibleWithTarget: IsCompatibleWithTargetNotFullBodyBipedIK,
                        getValue: (oci, parameter) => ((IKEditor)parameter).GetIKEnabled(),
                        readValueFromXml: (parameter, node) => node.ReadBool("value"),
                        writeValueToXml: (parameter, writer, o) => writer.WriteValue("value", (bool)o),
                        getParameter: GetParameter,
                        readParameterFromXml: null,
                        writeParameterToXml: null,
                        checkIntegrity: CheckIntegrity
                        );
                ToolBox.TimelineCompatibility.AddInterpolableModelDynamic(
                        owner: HSPE._name,
                        id: "ikPositionWeight",
                        name: "IK Weight",
                        interpolateBefore: (oci, parameter, leftValue, rightValue, factor) => ((IKEditor)parameter).SetIKPositionWeight(Mathf.LerpUnclamped((float)leftValue, (float)rightValue, factor)),
                        interpolateAfter: null,
                        isCompatibleWithTarget: IsCompatibleWithTarget,
                        getValue: (oci, parameter) => ((IKEditor)parameter).GetIKPositionWeight(),
                        readValueFromXml: (parameter, node) => node.ReadFloat("value"),
                        writeValueToXml: (parameter, writer, o) => writer.WriteValue("value", (float)o),
                        getParameter: GetParameter,
                        readParameterFromXml: null,
                        writeParameterToXml: null,
                        checkIntegrity: CheckIntegrity
                );
                //FABRIK
                ToolBox.TimelineCompatibility.AddInterpolableModelDynamic(
                        owner: HSPE._name,
                        id: "fabrikTolerance",
                        name: "IK Tolerance",
                        interpolateBefore: (oci, parameter, leftValue, rightValue, factor) => ((IKEditor)parameter).SetFABRIKTolerance(Mathf.LerpUnclamped((float)leftValue, (float)rightValue, factor)),
                        interpolateAfter: null,
                        isCompatibleWithTarget: IsCompatibleWithTargetFABRIK,
                        getValue: (oci, parameter) => ((IKEditor)parameter).GetFABRIKTolerance(),
                        readValueFromXml: (parameter, node) => node.ReadFloat("value"),
                        writeValueToXml: (parameter, writer, o) => writer.WriteValue("value", (float)o),
                        getParameter: GetParameter,
                        readParameterFromXml: null,
                        writeParameterToXml: null,
                        checkIntegrity: CheckIntegrity
                );
                ToolBox.TimelineCompatibility.AddInterpolableModelDynamic(
                        owner: HSPE._name,
                        id: "fabrikMaxIterations",
                        name: "IK Max Iterations",
                        interpolateBefore: (oci, parameter, leftValue, rightValue, factor) => ((IKEditor)parameter).SetFABRIKMaxIterations(Mathf.RoundToInt(Mathf.LerpUnclamped((int)leftValue, (int)rightValue, factor))),
                        interpolateAfter: null,
                        isCompatibleWithTarget: IsCompatibleWithTargetFABRIK,
                        getValue: (oci, parameter) => ((IKEditor)parameter).GetFABRIKMaxIterations(),
                        readValueFromXml: (parameter, node) => node.ReadInt("value"),
                        writeValueToXml: (parameter, writer, o) => writer.WriteValue("value", (int)o),
                        getParameter: GetParameter,
                        readParameterFromXml: null,
                        writeParameterToXml: null,
                        checkIntegrity: CheckIntegrity
                );
                //FABRIKRoot
                ToolBox.TimelineCompatibility.AddInterpolableModelDynamic(
                        owner: HSPE._name,
                        id: "fabrikRootTolerance",
                        name: "IK Tolerance",
                        interpolateBefore: (oci, parameter, leftValue, rightValue, factor) => ((IKEditor)parameter).SetFABRIKRootRootPin(Mathf.LerpUnclamped((float)leftValue, (float)rightValue, factor)),
                        interpolateAfter: null,
                        isCompatibleWithTarget: IsCompatibleWithTargetFABRIKRoot,
                        getValue: (oci, parameter) => ((IKEditor)parameter).GetFABRIKRootRootPin(),
                        readValueFromXml: (parameter, node) => node.ReadFloat("value"),
                        writeValueToXml: (parameter, writer, o) => writer.WriteValue("value", (float)o),
                        getParameter: GetParameter,
                        readParameterFromXml: null,
                        writeParameterToXml: null,
                        checkIntegrity: CheckIntegrity
                );
                ToolBox.TimelineCompatibility.AddInterpolableModelDynamic(
                        owner: HSPE._name,
                        id: "fabrikRootMaxIterations",
                        name: "IK Max Iterations",
                        interpolateBefore: (oci, parameter, leftValue, rightValue, factor) => ((IKEditor)parameter).SetFABRIKRootIterations(Mathf.RoundToInt(Mathf.LerpUnclamped((int)leftValue, (int)rightValue, factor))),
                        interpolateAfter: null,
                        isCompatibleWithTarget: IsCompatibleWithTargetFABRIKRoot,
                        getValue: (oci, parameter) => ((IKEditor)parameter).GetFABRIKRootIterations(),
                        readValueFromXml: (parameter, node) => node.ReadInt("value"),
                        writeValueToXml: (parameter, writer, o) => writer.WriteValue("value", (int)o),
                        getParameter: GetParameter,
                        readParameterFromXml: null,
                        writeParameterToXml: null,
                        checkIntegrity: CheckIntegrity
                );
                //CCDIK
                ToolBox.TimelineCompatibility.AddInterpolableModelDynamic(
                        owner: HSPE._name,
                        id: "ccdikTolerance",
                        name: "IK Tolerance",
                        interpolateBefore: (oci, parameter, leftValue, rightValue, factor) => ((IKEditor)parameter).SetCCDIKTolerance(Mathf.LerpUnclamped((float)leftValue, (float)rightValue, factor)),
                        interpolateAfter: null,
                        isCompatibleWithTarget: IsCompatibleWithTargetCCDIK,
                        getValue: (oci, parameter) => ((IKEditor)parameter).GetCCDIKTolerance(),
                        readValueFromXml: (parameter, node) => node.ReadFloat("value"),
                        writeValueToXml: (parameter, writer, o) => writer.WriteValue("value", (float)o),
                        getParameter: GetParameter,
                        readParameterFromXml: null,
                        writeParameterToXml: null,
                        checkIntegrity: CheckIntegrity
                );
                ToolBox.TimelineCompatibility.AddInterpolableModelDynamic(
                        owner: HSPE._name,
                        id: "ccdikMaxIterations",
                        name: "IK Max Iterations",
                        interpolateBefore: (oci, parameter, leftValue, rightValue, factor) => ((IKEditor)parameter).SetCCDIKMaxIterations(Mathf.RoundToInt(Mathf.LerpUnclamped((int)leftValue, (int)rightValue, factor))),
                        interpolateAfter: null,
                        isCompatibleWithTarget: IsCompatibleWithTargetCCDIK,
                        getValue: (oci, parameter) => ((IKEditor)parameter).GetCCDIKMaxIterations(),
                        readValueFromXml: (parameter, node) => node.ReadInt("value"),
                        writeValueToXml: (parameter, writer, o) => writer.WriteValue("value", (int)o),
                        getParameter: GetParameter,
                        readParameterFromXml: null,
                        writeParameterToXml: null,
                        checkIntegrity: CheckIntegrity
                );
                //AimIK
                ToolBox.TimelineCompatibility.AddInterpolableModelDynamic(
                        owner: HSPE._name,
                        id: "aimikTolerance",
                        name: "IK Tolerance",
                        interpolateBefore: (oci, parameter, leftValue, rightValue, factor) => ((IKEditor)parameter).SetAimIKTolerance(Mathf.LerpUnclamped((float)leftValue, (float)rightValue, factor)),
                        interpolateAfter: null,
                        isCompatibleWithTarget: IsCompatibleWithTargetAimIK,
                        getValue: (oci, parameter) => ((IKEditor)parameter).GetAimIKTolerance(),
                        readValueFromXml: (parameter, node) => node.ReadFloat("value"),
                        writeValueToXml: (parameter, writer, o) => writer.WriteValue("value", (float)o),
                        getParameter: GetParameter,
                        readParameterFromXml: null,
                        writeParameterToXml: null,
                        checkIntegrity: CheckIntegrity
                );
                ToolBox.TimelineCompatibility.AddInterpolableModelDynamic(
                        owner: HSPE._name,
                        id: "aimikMaxIterations",
                        name: "IK Max Iterations",
                        interpolateBefore: (oci, parameter, leftValue, rightValue, factor) => ((IKEditor)parameter).SetAimIKMaxIterations(Mathf.RoundToInt(Mathf.LerpUnclamped((int)leftValue, (int)rightValue, factor))),
                        interpolateAfter: null,
                        isCompatibleWithTarget: IsCompatibleWithTargetAimIK,
                        getValue: (oci, parameter) => ((IKEditor)parameter).GetAimIKMaxIterations(),
                        readValueFromXml: (parameter, node) => node.ReadInt("value"),
                        writeValueToXml: (parameter, writer, o) => writer.WriteValue("value", (int)o),
                        getParameter: GetParameter,
                        readParameterFromXml: null,
                        writeParameterToXml: null,
                        checkIntegrity: CheckIntegrity
                );
                ToolBox.TimelineCompatibility.AddInterpolableModelDynamic(
                        owner: HSPE._name,
                        id: "aimikAxis",
                        name: "IK Axis",
                        interpolateBefore: (oci, parameter, leftValue, rightValue, factor) => ((IKEditor)parameter).SetAimIKAxis(Vector3.LerpUnclamped((Vector3)leftValue, (Vector3)rightValue, factor)),
                        interpolateAfter: null,
                        isCompatibleWithTarget: IsCompatibleWithTargetAimIK,
                        getValue: (oci, parameter) => ((IKEditor)parameter).GetAimIKAxis(),
                        readValueFromXml: (parameter, node) => node.ReadVector3("value"),
                        writeValueToXml: (parameter, writer, o) => writer.WriteValue("value", (Vector3)o),
                        getParameter: GetParameter,
                        readParameterFromXml: null,
                        writeParameterToXml: null,
                        checkIntegrity: CheckIntegrity
                );
                ToolBox.TimelineCompatibility.AddInterpolableModelDynamic(
                        owner: HSPE._name,
                        id: "aimikClampWeight",
                        name: "IK Clamp Weight",
                        interpolateBefore: (oci, parameter, leftValue, rightValue, factor) => ((IKEditor)parameter).SetAimIKClampWeight(Mathf.LerpUnclamped((float)leftValue, (float)rightValue, factor)),
                        interpolateAfter: null,
                        isCompatibleWithTarget: IsCompatibleWithTargetAimIK,
                        getValue: (oci, parameter) => ((IKEditor)parameter).GetAimIKClampWeight(),
                        readValueFromXml: (parameter, node) => node.ReadFloat("value"),
                        writeValueToXml: (parameter, writer, o) => writer.WriteValue("value", (float)o),
                        getParameter: GetParameter,
                        readParameterFromXml: null,
                        writeParameterToXml: null,
                        checkIntegrity: CheckIntegrity
                );                
                ToolBox.TimelineCompatibility.AddInterpolableModelDynamic(
                        owner: HSPE._name,
                        id: "aimikClampSmoothing",
                        name: "IK Clamp Smoothing",
                        interpolateBefore: (oci, parameter, leftValue, rightValue, factor) => ((IKEditor)parameter).SetAimIKClampSmoothing(Mathf.RoundToInt(Mathf.LerpUnclamped((int)leftValue, (int)rightValue, factor))),
                        interpolateAfter: null,
                        isCompatibleWithTarget: IsCompatibleWithTargetAimIK,
                        getValue: (oci, parameter) => ((IKEditor)parameter).GetAimIKClampSmoothing(),
                        readValueFromXml: (parameter, node) => node.ReadInt("value"),
                        writeValueToXml: (parameter, writer, o) => writer.WriteValue("value", (int)o),
                        getParameter: GetParameter,
                        readParameterFromXml: null,
                        writeParameterToXml: null,
                        checkIntegrity: CheckIntegrity
                );
                ToolBox.TimelineCompatibility.AddInterpolableModelDynamic(
                        owner: HSPE._name,
                        id: "aimikPoleAxis",
                        name: "IK Pole Axis",
                        interpolateBefore: (oci, parameter, leftValue, rightValue, factor) => ((IKEditor)parameter).SetAimIKPoleAxis(Vector3.LerpUnclamped((Vector3)leftValue, (Vector3)rightValue, factor)),
                        interpolateAfter: null,
                        isCompatibleWithTarget: IsCompatibleWithTargetAimIK,
                        getValue: (oci, parameter) => ((IKEditor)parameter).GetAimIKPoleAxis(),
                        readValueFromXml: (parameter, node) => node.ReadVector3("value"),
                        writeValueToXml: (parameter, writer, o) => writer.WriteValue("value", (Vector3)o),
                        getParameter: GetParameter,
                        readParameterFromXml: null,
                        writeParameterToXml: null,
                        checkIntegrity: CheckIntegrity
                );
                ToolBox.TimelineCompatibility.AddInterpolableModelDynamic(
                        owner: HSPE._name,
                        id: "aimikPoleAxis",
                        name: "IK Pole Position",
                        interpolateBefore: (oci, parameter, leftValue, rightValue, factor) => ((IKEditor)parameter).SetAimIKPolePosition(Vector3.LerpUnclamped((Vector3)leftValue, (Vector3)rightValue, factor)),
                        interpolateAfter: null,
                        isCompatibleWithTarget: IsCompatibleWithTargetAimIKPoleTargetNull,
                        getValue: (oci, parameter) => ((IKEditor)parameter).GetAimIKPolePosition(),
                        readValueFromXml: (parameter, node) => node.ReadVector3("value"),
                        writeValueToXml: (parameter, writer, o) => writer.WriteValue("value", (Vector3)o),
                        getParameter: GetParameter,
                        readParameterFromXml: null,
                        writeParameterToXml: null,
                        checkIntegrity: CheckIntegrity
                );
                ToolBox.TimelineCompatibility.AddInterpolableModelDynamic(
                        owner: HSPE._name,
                        id: "aimikPoleWeight",
                        name: "IK Pole Weight",
                        interpolateBefore: (oci, parameter, leftValue, rightValue, factor) => ((IKEditor)parameter).SetAimIKPoleWeight(Mathf.LerpUnclamped((float)leftValue, (float)rightValue, factor)),
                        interpolateAfter: null,
                        isCompatibleWithTarget: IsCompatibleWithTargetAimIK,
                        getValue: (oci, parameter) => ((IKEditor)parameter).GetAimIKPoleWeight(),
                        readValueFromXml: (parameter, node) => node.ReadFloat("value"),
                        writeValueToXml: (parameter, writer, o) => writer.WriteValue("value", (float)o),
                        getParameter: GetParameter,
                        readParameterFromXml: null,
                        writeParameterToXml: null,
                        checkIntegrity: CheckIntegrity
                );
                //FullBodyBipedIK
                GenerateEffectorInterpolables("R. Shoulder", e => ((IKSolverFullBodyBiped)e._ik.solver).rightShoulderEffector, e => ((FullBodyBipedIKData)e._ikData).rightShoulder);
                GenerateEffectorInterpolables("L. Shoulder", e => ((IKSolverFullBodyBiped)e._ik.solver).leftShoulderEffector, e => ((FullBodyBipedIKData)e._ikData).leftShoulder);
                GenerateEffectorInterpolables("R. Hand", e => ((IKSolverFullBodyBiped)e._ik.solver).rightHandEffector, e => ((FullBodyBipedIKData)e._ikData).rightHand);
                GenerateEffectorInterpolables("L. Hand", e => ((IKSolverFullBodyBiped)e._ik.solver).leftHandEffector, e => ((FullBodyBipedIKData)e._ikData).leftHand);
                GenerateEffectorInterpolables("Body", e => ((IKSolverFullBodyBiped)e._ik.solver).bodyEffector, e => ((FullBodyBipedIKData)e._ikData).body);
                GenerateEffectorInterpolables("R. Thigh", e => ((IKSolverFullBodyBiped)e._ik.solver).rightThighEffector, e => ((FullBodyBipedIKData)e._ikData).rightThigh);
                GenerateEffectorInterpolables("L. Thigh", e => ((IKSolverFullBodyBiped)e._ik.solver).leftThighEffector, e => ((FullBodyBipedIKData)e._ikData).leftThigh);
                GenerateEffectorInterpolables("R. Foot", e => ((IKSolverFullBodyBiped)e._ik.solver).rightFootEffector, e => ((FullBodyBipedIKData)e._ikData).rightFoot);
                GenerateEffectorInterpolables("L. Foot", e => ((IKSolverFullBodyBiped)e._ik.solver).leftFootEffector, e => ((FullBodyBipedIKData)e._ikData).leftFoot);
                GenerateConstraintBendInterpolables("R. Elbow", e => ((IKSolverFullBodyBiped)e._ik.solver).rightArmChain.bendConstraint, e => ((FullBodyBipedIKData)e._ikData).rightArm);
                GenerateConstraintBendInterpolables("L. Elbow", e => ((IKSolverFullBodyBiped)e._ik.solver).leftArmChain.bendConstraint, e => ((FullBodyBipedIKData)e._ikData).leftArm);
                GenerateConstraintBendInterpolables("R. Knee", e => ((IKSolverFullBodyBiped)e._ik.solver).rightLegChain.bendConstraint, e => ((FullBodyBipedIKData)e._ikData).rightLeg);
                GenerateConstraintBendInterpolables("L. Knee", e => ((IKSolverFullBodyBiped)e._ik.solver).leftLegChain.bendConstraint, e => ((FullBodyBipedIKData)e._ikData).leftLeg);
            }
            
            private static void GenerateEffectorInterpolables(string effectorName, Func<IKEditor, IKEffector> getEffector, Func<IKEditor, FullBodyBipedIKData.EffectorData> getEffectorData)
            {
                ToolBox.TimelineCompatibility.AddInterpolableModelDynamic(
                        owner: HSPE._name,
                        id: "fbbikEffectorPositionWeight" + effectorName,
                        name: "IK " + effectorName + " Pos Weight",
                        interpolateBefore: (oci, parameter, leftValue, rightValue, factor) =>
                        {
                            IKEditor editor = (IKEditor)parameter;
                            editor.SetEffectorPositionWeight(Mathf.LerpUnclamped((float)leftValue, (float)rightValue, factor), getEffector(editor), getEffectorData(editor));
                        },
                        interpolateAfter: null,
                        isCompatibleWithTarget: IsCompatibleWithTargetFullBodyBipedIK,
                        getValue: (oci, parameter) =>
                        {
                            IKEditor editor = (IKEditor)parameter;
                            return ((IKEditor)parameter).GetEffectorPositionWeight(getEffector(editor));
                        },
                        readValueFromXml: (parameter, node) => node.ReadFloat("value"),
                        writeValueToXml: (parameter, writer, o) => writer.WriteValue("value", (float)o),
                        getParameter: GetParameter,
                        readParameterFromXml: null,
                        writeParameterToXml: null,
                        checkIntegrity: CheckIntegrity
                );
                ToolBox.TimelineCompatibility.AddInterpolableModelDynamic(
                        owner: HSPE._name,
                        id: "fbbikEffectorRotationWeight" + effectorName,
                        name: "IK " + effectorName + " Rot Weight",
                        interpolateBefore: (oci, parameter, leftValue, rightValue, factor) =>
                        {
                            IKEditor editor = (IKEditor)parameter;
                            editor.SetEffectorRotationWeight(Mathf.LerpUnclamped((float)leftValue, (float)rightValue, factor), getEffector(editor), getEffectorData(editor));
                        },
                        interpolateAfter: null,
                        isCompatibleWithTarget: IsCompatibleWithTargetFullBodyBipedIK,
                        getValue: (oci, parameter) =>
                        {
                            IKEditor editor = (IKEditor)parameter;
                            return ((IKEditor)parameter).GetEffectorRotationWeight(getEffector(editor));
                        },
                        readValueFromXml: (parameter, node) => node.ReadFloat("value"),
                        writeValueToXml: (parameter, writer, o) => writer.WriteValue("value", (float)o),
                        getParameter: GetParameter,
                        readParameterFromXml: null,
                        writeParameterToXml: null,
                        checkIntegrity: CheckIntegrity
                );
            }

            private static void GenerateConstraintBendInterpolables(string constraintName, Func<IKEditor, IKConstraintBend> getConstraint, Func<IKEditor, FullBodyBipedIKData.ConstraintBendData> getConstraintData)
            {
                ToolBox.TimelineCompatibility.AddInterpolableModelDynamic(
                        owner: HSPE._name,
                        id: "fbbikConstraintBendWeight" + constraintName,
                        name: "IK " + constraintName + " Weight",
                        interpolateBefore: (oci, parameter, leftValue, rightValue, factor) =>
                        {
                            IKEditor editor = (IKEditor)parameter;
                            editor.SetConstraintBendWeight(Mathf.LerpUnclamped((float)leftValue, (float)rightValue, factor), getConstraint(editor), getConstraintData(editor));
                        },
                        interpolateAfter: null,
                        isCompatibleWithTarget: IsCompatibleWithTargetFullBodyBipedIK,
                        getValue: (oci, parameter) =>
                        {
                            IKEditor editor = (IKEditor)parameter;
                            return ((IKEditor)parameter).GetConstraintBendWeight(getConstraint(editor));
                        },
                        readValueFromXml: (parameter, node) => node.ReadFloat("value"),
                        writeValueToXml: (parameter, writer, o) => writer.WriteValue("value", (float)o),
                        getParameter: GetParameter,
                        readParameterFromXml: null,
                        writeParameterToXml: null,
                        checkIntegrity: CheckIntegrity
                );
            }

            private static bool CheckIntegrity(ObjectCtrlInfo oci, object parameter, object leftValue, object rightValue)
            {
                if (parameter == null)
                    return false;
                IKEditor editor = (IKEditor)parameter;
                return editor.shouldDisplay && editor._ik.ik != null && editor._ik.solver != null;
            }

            private static bool IsCompatibleWithTarget(ObjectCtrlInfo oci)
            {
                PoseController controller;
                return oci != null && oci.guideObject != null && oci.guideObject.transformTarget != null && (controller = oci.guideObject.transformTarget.GetComponent<PoseController>()) != null && controller._ikEditor.shouldDisplay;
            }

            private static bool IsCompatibleWithTargetNotFullBodyBipedIK(ObjectCtrlInfo oci)
            {
                PoseController controller;
                return oci != null && oci.guideObject != null && oci.guideObject.transformTarget != null && (controller = oci.guideObject.transformTarget.GetComponent<PoseController>()) != null && controller._ikEditor.shouldDisplay && controller._ikEditor._ik.type != IKType.FullBodyBipedIK;
            }

            private static bool IsCompatibleWithTargetFABRIK(ObjectCtrlInfo oci)
            {
                PoseController controller;
                return oci != null && oci.guideObject != null && oci.guideObject.transformTarget != null && (controller = oci.guideObject.transformTarget.GetComponent<PoseController>()) != null && controller._ikEditor.shouldDisplay && controller._ikEditor._ik.type == IKType.FABRIK;
            }

            private static bool IsCompatibleWithTargetFABRIKRoot(ObjectCtrlInfo oci)
            {
                PoseController controller;
                return oci != null && oci.guideObject != null && oci.guideObject.transformTarget != null && (controller = oci.guideObject.transformTarget.GetComponent<PoseController>()) != null && controller._ikEditor.shouldDisplay && controller._ikEditor._ik.type == IKType.FABRIKRoot;
            }

            private static bool IsCompatibleWithTargetAimIK(ObjectCtrlInfo oci)
            {
                PoseController controller;
                return oci != null && oci.guideObject != null && oci.guideObject.transformTarget != null && (controller = oci.guideObject.transformTarget.GetComponent<PoseController>()) != null && controller._ikEditor.shouldDisplay && controller._ikEditor._ik.type == IKType.AimIK;
            }

            private static bool IsCompatibleWithTargetAimIKPoleTargetNull(ObjectCtrlInfo oci)
            {
                PoseController controller;
                return oci != null && oci.guideObject != null && oci.guideObject.transformTarget != null && (controller = oci.guideObject.transformTarget.GetComponent<PoseController>()) != null && controller._ikEditor.shouldDisplay && controller._ikEditor._ik.type == IKType.AimIK && ((IKSolverAim)controller._ikEditor._ik.solver).poleTarget == null;
            }

            private static bool IsCompatibleWithTargetCCDIK(ObjectCtrlInfo oci)
            {
                PoseController controller;
                return oci != null && oci.guideObject != null && oci.guideObject.transformTarget != null && (controller = oci.guideObject.transformTarget.GetComponent<PoseController>()) != null && controller._ikEditor.shouldDisplay && controller._ikEditor._ik.type == IKType.CCDIK;
            }

            private static bool IsCompatibleWithTargetFullBodyBipedIK(ObjectCtrlInfo oci)
            {
                PoseController controller;
                return oci != null && oci.guideObject != null && oci.guideObject.transformTarget != null && (controller = oci.guideObject.transformTarget.GetComponent<PoseController>()) != null && controller._ikEditor.shouldDisplay && controller._ikEditor._ik.type == IKType.FullBodyBipedIK;
            }

            private static object GetParameter(ObjectCtrlInfo oci)
            {
                return oci.guideObject.transformTarget.GetComponent<PoseController>()._ikEditor;
            }
        }
        #endregion
    }
}
