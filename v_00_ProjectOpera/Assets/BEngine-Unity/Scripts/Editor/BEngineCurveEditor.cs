using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace BEngine {

    [CustomEditor(typeof(BEngineCurve))]
    public class CurveTestEditor : Editor
    {

        BEngineCurve _curveComponent;

        protected const int QUAD_SIZE = 12;

        protected static Color _unselectedCurvePointColor = new Color(0.8f, 0.8f, 0.8f);
        protected static Color _selectedCurvePointColor = new Color(1f, 0.8f, 0.3f);
        protected GUIStyle _curvePointUnselectedStyle = new GUIStyle();
        protected GUIStyle _curvePointSelectedStyle = new GUIStyle();

        protected List<CurvePoint> _selectedPointsList;

        protected Vector3 _handleOrigin = Vector3.zero;
        protected Vector3 _handleOffset = Vector3.zero;

        protected Plane _plane;

        // protected float _curveStepPrev;
        protected CurveType _curveDataTypePrev;
        protected float _endDistMultiplier;
        protected float _SplineAlpha;
        protected bool _mousePressed = false;



        void OnEnable() {
            _curveComponent = target as BEngineCurve;

            var curvSetts = _curveComponent.CurveSettings;

            // Set Points Styles
            Texture2D curvePointTex_1 = new Texture2D(1, 1);
            curvePointTex_1.SetPixel(0, 0, _unselectedCurvePointColor);
            curvePointTex_1.Apply();

            _curvePointUnselectedStyle.normal.background = curvePointTex_1;

            Texture2D curvePointTex_2 = new Texture2D(1, 1);
            curvePointTex_2.SetPixel(0, 0, _selectedCurvePointColor);
            curvePointTex_2.Apply();

            _curvePointSelectedStyle.normal.background = curvePointTex_2;

            // Other stuff
            _selectedPointsList = new List<CurvePoint>();

            ResetMoveHandle();

            // Set Previous Values
            // _curveStepPrev = curvSetts.CurveStep;
            _curveDataTypePrev = curvSetts.CurveDataType;
            _endDistMultiplier = curvSetts.EndDistMultiplier;
            _SplineAlpha = curvSetts.SplineAlpha;

        }


        void OnSceneGUI()
        {

            Event guiEvent = Event.current;
            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            var controlType = guiEvent.GetTypeForControl(controlID);

            var curvSetts = _curveComponent.CurveSettings;

            // Transform Handle
            if (_selectedPointsList.Count > 0) {
                _handleOffset = Handles.PositionHandle(_handleOffset, Quaternion.identity);
            }

            if (guiEvent.type == EventType.Layout) {
                HandleUtility.AddDefaultControl(controlID);
            }

            if (guiEvent.type == EventType.KeyUp) {

                // New Curve
                if (guiEvent.keyCode == KeyCode.N && guiEvent.modifiers == EventModifiers.Shift) {
                    Undo.RegisterCompleteObjectUndo(target, "Changed BEngine Curve");

                    CreateNewCurve();

                } else if (guiEvent.keyCode == KeyCode.S && guiEvent.modifiers == EventModifiers.Shift) {
                    Undo.RegisterCompleteObjectUndo(target, "Changed BEngine Curve");
                    Select_DeselectPoints();
                }

                // Add Point
                else if (guiEvent.keyCode == KeyCode.A && guiEvent.modifiers == EventModifiers.Shift) {
                    if (_selectedPointsList.Count > 0) {
                        Undo.RegisterCompleteObjectUndo(target, "Changed BEngine Curve");
                        
                        var curPoint = _selectedPointsList[0];

                        var newPoint = AddPointBySelection(curPoint);

                        if (newPoint != null) {
                            _selectedPointsList.Clear();
                            _selectedPointsList.Add(newPoint);
                            CenterHandle();

                            GenerateSelectedSplineSegments();
                        }
                    }
                }

                // Delete Point
                else if (guiEvent.keyCode == KeyCode.X && guiEvent.modifiers == EventModifiers.Shift) {
                    Undo.RegisterCompleteObjectUndo(target, "Changed BEngine Curve");
                    DeleteSelectedPoints();
                }
            } 

            // Update Positions if a Transform Tool has been changed
            if (_selectedPointsList.Count > 0) {
                UpdatePointsPositions(controlType);
            }

            // Points UI
            Handles.BeginGUI();

            foreach (var curveData in _curveComponent.Curves) {
                foreach (var curvePoint in curveData.CurvePoints) {

                    if (!IsOnScreen(curvePoint.position)) continue;

                    Vector3 guiPos = HandleUtility.WorldToGUIPoint(curvePoint.position);

                    var pointStyle = _curvePointUnselectedStyle;
                    if (_selectedPointsList.Contains(curvePoint)) pointStyle = _curvePointSelectedStyle;

                    // Point Event
                    if(Point2D(guiPos, pointStyle)) {

                        // Select/Deselect Points
                        // if(guiEvent.type == EventType.do)
                        if(guiEvent.modifiers == EventModifiers.Control) {
                            if (_selectedPointsList.Contains(curvePoint)) {
                                _selectedPointsList.Remove(curvePoint);
                            }
                            else {
                                _selectedPointsList.Add(curvePoint);
                            }
                        }
                        else {
                            _selectedPointsList.Clear();
                            _selectedPointsList.Add(curvePoint);
                        }

                        CenterHandle();

                    }
                }
            }

            Handles.EndGUI();

            if (GUI.changed)
                EditorUtility.SetDirty(target);

        }


        public override void OnInspectorGUI()
        {
            
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.LabelField("AddPoint: Shft+A");
            EditorGUILayout.LabelField("Select/Deselect: Shft+S");
            EditorGUILayout.LabelField("NewCurve: Shft+N");
            EditorGUILayout.LabelField("DeleteCurve: Shft+X");

            if(GUILayout.Button("New Curve")) {
                Undo.RegisterCompleteObjectUndo(target, "Changed BEngine Curve");

                CreateNewCurve();
                SceneView.RepaintAll();
            }

            if(GUILayout.Button("Select/Deselect Points")) {
                Undo.RegisterCompleteObjectUndo(target, "Changed BEngine Curve");

                Select_DeselectPoints();
                SceneView.RepaintAll();
            }

            EditorGUILayout.Separator();

            if(GUILayout.Button("Closed/Open")) {
                Undo.RegisterCompleteObjectUndo(target, "Changed BEngine Curve");

                foreach(var curvData in _curveComponent.Curves) {
                    foreach (var selPoint in _selectedPointsList) {
                        if (curvData.CurvePoints.Contains(selPoint)) {

                            // Cahenge IsClosed Value
                            if(curvData.IsClosed) {
                                curvData.IsClosed = false;
                            } else {
                                curvData.IsClosed = true;
                            }

                            if (_curveComponent.CurveSettings.CurveDataType == CurveType.Spline) {
                                curvData.GenerateSplineSegments(curvData.IsClosed, _curveComponent.CurveSettings);
                            }

                            break;

                        }
                    }
                }

                SceneView.RepaintAll();
            }

            if(GUILayout.Button("Change Curve Direction")) {
                Undo.RegisterCompleteObjectUndo(target, "Changed BEngine Curve");

                // List<CurveData> curvesToSort = new List<CurveData>();

                foreach(var curvData in _curveComponent.Curves) {
                    bool reverseSorting = false;

                    foreach (var selPoint in _selectedPointsList) {
                        if (curvData.CurvePoints.Contains(selPoint)) {

                            reverseSorting = true;
                            break;
                        }
                    }

                    if (reverseSorting) {
                        curvData.CurvePoints.Reverse();

                        if (_curveComponent.CurveSettings.CurveDataType == CurveType.Spline) {
                            curvData.GenerateSplineSegments(curvData.IsClosed, _curveComponent.CurveSettings);
                        }
                    }
                }

                SceneView.RepaintAll();
            }

            if(GUILayout.Button("Delete Points")) {
                Undo.RegisterCompleteObjectUndo(target, "Changed BEngine Curve");

                DeleteSelectedPoints();
                SceneView.RepaintAll();
            }

            EditorGUILayout.Separator();

            if(GUILayout.Button("Update Curves")) {
                Undo.RegisterCompleteObjectUndo(target, "Changed BEngine Curve");

                foreach (var curveData in _curveComponent.Curves) {
                    curveData.GenerateSplineSegments(curveData.IsClosed, _curveComponent.CurveSettings);
                }

                SceneView.RepaintAll();
            }


            base.OnInspectorGUI();

            // EditorGUI.BeginChangeCheck();

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(target); 

                // Recalculate Splines if Some Parameters has been changed
                CheckMainChanges();

                serializedObject.ApplyModifiedProperties();
            }

        }


        public CurveData CreateNewCurve() {
            var newCurveData = new CurveData();
            _curveComponent.Curves.Add(newCurveData);
            newCurveData.CurvePoints.Add(new CurvePoint(new Vector3(0, 0, 1) + _curveComponent.transform.position));
            newCurveData.CurvePoints.Add(new CurvePoint(new Vector3(0, 0, 5) + _curveComponent.transform.position));

            if (_curveComponent.CurveSettings.CurveDataType == CurveType.Spline) {
                newCurveData.GenerateSplineSegments(newCurveData.IsClosed, _curveComponent.CurveSettings);
            }

            return newCurveData;
        }


        public void DeleteSelectedPoints() {
            if (_selectedPointsList.Count > 0) {
                Undo.RecordObject(target, "Changed BEngine Curve");

                List<CurveData> removeCurveDataList = new List<CurveData>();

                foreach (var curveData in _curveComponent.Curves) {
                    bool hasDeletion = false;

                    foreach (var curvePoint in curveData.CurvePoints.ToArray()) {
                        if (_selectedPointsList.Contains(curvePoint)) {
                            curveData.CurvePoints.Remove(curvePoint);
                            _selectedPointsList.Remove(curvePoint);
                            hasDeletion = true;
                        }
                    }

                    // Update Splines
                    if (hasDeletion && _curveComponent.CurveSettings.CurveDataType == CurveType.Spline) {
                        curveData.GenerateSplineSegments(curveData.IsClosed, _curveComponent.CurveSettings);
                    }

                    // If No Points
                    if (curveData.CurvePoints.Count == 0) removeCurveDataList.Add(curveData);
                }

                ResetMoveHandle();

                if(removeCurveDataList.Count > 0) _curveComponent.Curves.RemoveAll(l => removeCurveDataList.Contains(l));
            }
        }


        public void Select_DeselectPoints() {
            if (_selectedPointsList.Count > 0) {
                _selectedPointsList.Clear();
            }
            else {
                
                foreach (var curveData in _curveComponent.Curves) {
                    foreach (var curvePoint in curveData.CurvePoints) {
                        _selectedPointsList.Add(curvePoint);
                    }
                }

                CenterHandle();
            }
        }


        private void UpdatePointsPositions(EventType controlType) {
            // Transform Curve Points
            if (_handleOrigin.x != _handleOffset.x || _handleOrigin.y != _handleOffset.y || _handleOrigin.z != _handleOffset.z) {
                
                if (!_mousePressed) {
                    Undo.RegisterCompleteObjectUndo(target, "Moved BEngine Curve");
                    _mousePressed = true;
                }
                
                var handleAdd =  _handleOffset - _handleOrigin;

                foreach (var selectedPoint in _selectedPointsList) {
                    selectedPoint.position += handleAdd;
                }

                _handleOrigin = (Vector3)_handleOffset;

                // If it's a spline then regenerate segments
                GenerateSelectedSplineSegments();
            }

            if ((int)controlType == 1 && _mousePressed == true) {
                _mousePressed = false;
            }
        }


        private void CheckMainChanges() {

                bool isChanged = false;
                var curvSetts = _curveComponent.CurveSettings;

                // if (curvSetts.CurveStep != _curveStepPrev) {
                //     _curveStepPrev = curvSetts.CurveStep;
                //     isChanged = true;
                // }

                if (curvSetts.CurveDataType != _curveDataTypePrev) {
                    _curveDataTypePrev = curvSetts.CurveDataType;
                    isChanged = true;
                }
                else if (curvSetts.EndDistMultiplier != _endDistMultiplier){
                    _endDistMultiplier = curvSetts.EndDistMultiplier;
                    isChanged = true;
                }
                else if (curvSetts.SplineAlpha != _SplineAlpha){
                    _SplineAlpha = curvSetts.SplineAlpha;
                    isChanged = true;
                }

                if(isChanged) {
                    if (curvSetts.CurveDataType == CurveType.Spline) {
                        _curveComponent.GenerateAllSplines();
                    }
                    // else if (_curveComponent.CurveDataType == CurveType.PolyLine){
                    // }
                }
        }

        static bool Point2D(Vector2 position, GUIStyle style) {
            return GUI.Button(new Rect(position - new Vector2(QUAD_SIZE / 2, QUAD_SIZE / 2), new Vector2(QUAD_SIZE, QUAD_SIZE)), GUIContent.none, style);
        }


        public static bool IsOnScreen(Vector3 position) {
            Vector3 onScreen = Camera.current.WorldToViewportPoint(position);
            return onScreen.z > 0 && onScreen.x > 0 && onScreen.y > 0 && onScreen.x < 1 && onScreen.y < 1;
        }


        Vector3 NewPointPosition(Event guiEvent, CurvePoint curPoint)
        {
            Vector3 newVec = Vector3.zero;
            Ray mouseRay = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition);

            RaycastHit hit;
            if(_curveComponent.CurveSettings.SnapNewPoints && Physics.Raycast( mouseRay, out hit ) )
            {
                newVec = hit.point;

            } else {
                var vieportDirection = SceneView.currentDrawingSceneView.rotation * Vector3.forward;

                _plane.SetNormalAndPosition(GetBestDirectionXYZ(vieportDirection), curPoint.position);
                float enter = 0.0f;

                if (_plane.Raycast(mouseRay, out enter)) {
                    newVec = mouseRay.GetPoint(enter);
                }
            }

            return newVec;

        }

        Vector3 GetBestDirectionXYZ(Vector3 direction) {
            float angleX, angleY, angleZ, angleNegX, angleNegY, angleNegZ;

            angleX = Vector3.Angle(direction, Vector3.right);
            angleY = Vector3.Angle(direction, Vector3.up);
            angleZ = Vector3.Angle(direction, Vector3.forward);

            angleNegX = Vector3.Angle(direction, Vector3.left);
            angleNegY = Vector3.Angle(direction, Vector3.down);
            angleNegZ = Vector3.Angle(direction, Vector3.back);

            float minVal = angleX;
            Vector3 returnVec = Vector3.right;

            if (angleY < minVal) {
                returnVec = Vector3.up;
                minVal = angleY;
            } 
            if  (angleZ < minVal) {
                returnVec = Vector3.forward;
                minVal = angleZ;
            }
            if  (angleNegX < minVal) {
                returnVec = Vector3.left;
                minVal = angleNegX;
            }
            if  (angleNegY < minVal) {
                returnVec = Vector3.down;
                minVal = angleNegY;
            }
            if  (angleNegZ < minVal) {
                returnVec = Vector3.back;
                minVal = angleNegZ;
            }

            return returnVec;
        }
 

        Vector3 CenterOfPoints() {
                float totalX = 0;
                float totalY = 0;
                float totalZ = 0;

                Vector3 posVector = new Vector3();

                foreach (var point in _selectedPointsList) {
                    totalX += point.position.x;
                    totalY += point.position.y;
                    totalZ += point.position.z;
                }

                posVector.x = totalX / _selectedPointsList.Count;
                posVector.y = totalY / _selectedPointsList.Count;
                posVector.z = totalZ / _selectedPointsList.Count;

                return posVector;
        }


        CurvePoint AddPointBySelection(CurvePoint curPoint) {
            CurvePoint newPoint;

            Vector3 pointPos = NewPointPosition(Event.current, curPoint);

            foreach (var curveData in _curveComponent.Curves) {
                foreach (var curvePoint in curveData.CurvePoints) {
                    if(curPoint == curvePoint) {
                        int index = curveData.CurvePoints.IndexOf(curvePoint);

                        if (_selectedPointsList.Count == 1) {
                            if(index == curveData.CurvePoints.Count - 1) {
                                newPoint = new CurvePoint(pointPos);

                                curveData.CurvePoints.Add(newPoint);
                                return newPoint;
                            } 
                            else if (index == 0) {
                                newPoint = new CurvePoint(pointPos);

                                curveData.CurvePoints.Insert(0, newPoint);
                                return newPoint;
                            }
                        }
                        else if (_selectedPointsList.Count > 1) {
                            if ( index < (curveData.CurvePoints.Count - 1) && _selectedPointsList.Contains(curveData.CurvePoints[index + 1])) {
                                newPoint = new CurvePoint(pointPos);
                                
                                curveData.CurvePoints.Insert(index + 1, newPoint);
                                return newPoint;
                            }

                            if (index > 0 &&_selectedPointsList.Contains(curveData.CurvePoints[index - 1])) {
                                newPoint = new CurvePoint(pointPos);

                                curveData.CurvePoints.Insert(index, newPoint);
                                return newPoint;

                            }
                        }
                    }
                }
            }

            return null;
        }


        private void GenerateSelectedSplineSegments() {
            if (_curveComponent.CurveSettings.CurveDataType == CurveType.Spline) {
                foreach(var curve in _curveComponent.Curves) {
                    foreach (var selectedPoint in _selectedPointsList) {
                        if (curve.CurvePoints.Contains(selectedPoint)) {
                            curve.GenerateSplineSegments(curve.IsClosed, _curveComponent.CurveSettings);
                            break;
                        }

                    }
                }
            }
        }


        void CenterHandle() {
            _handleOrigin = CenterOfPoints();
            _handleOffset = (Vector3)_handleOrigin;
        }


        protected void ResetMoveHandle() {
            _handleOrigin = Vector3.zero;
            _handleOffset = Vector3.zero;
        }


        [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected)]
        private static void DisplayCurve(BEngineCurve curveComp, GizmoType gizmoType) {

            if (Event.current.type == EventType.Repaint) {
            
                foreach (var curveData in curveComp.Curves) {
                    List<Vector3> allPoints = new List<Vector3>();

                    // ValidateSplines(curveData, curveComp.IsClosed, curveComp.CurveStep);

                    // Polyline
                    if (curveComp.CurveSettings.CurveDataType == CurveType.PolyLine) {
                        Handles.DrawAAPolyLine(2.0f, curveData.GetPoints(curveData.IsClosed, 
                                                                        curveComp.CurveSettings.CurveDataType).ToArray());
                    }

                    // Spline
                    else if (curveComp.CurveSettings.CurveDataType == CurveType.Spline){
                        if (curveData.CurveSplineSegments == null) {
                            curveData.GenerateSplineSegments(curveData.IsClosed, curveComp.CurveSettings);
                        }

                        foreach (var curveSegment in curveData.CurveSplineSegments) {
                            if (curveSegment.Points != null) {
                                Handles.DrawAAPolyLine(2.0f, curveSegment.Points.ToArray());
                            }
                        }
                    }
                }
            }
        }

    }
}