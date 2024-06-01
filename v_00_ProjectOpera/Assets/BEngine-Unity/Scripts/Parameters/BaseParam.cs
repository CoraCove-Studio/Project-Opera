using System;
using UnityEngine;


namespace BEngine
{

    [Serializable]
    public class BaseParam
    {

        public ParameterType Type;
        public string Name;
        public string Identifier;


        public bool DefaultBoolValue;
        public bool BoolValue;

        public Color DefaultColorValue;
        public Color ColorValue;

        public float DefaultFloatMinValue;
        public float DefaultFloatMaxValue;
        public float DefaultFloatValue;
        public float FloatValue;

        public Texture ImageValue;

        public int DefaultIntMinValue;
        public int DefaultIntMaxValue;
        public int DefaultIntValue;
        public int IntValue;

        public Material MaterialValue;

        public UnityEngine.GameObject ObjectValue;  // Object or Collection

        public string DefaultStringValue;
        public string StringValue;

        // FouldOut
        public bool IsOpenFoldout = false;

        public Vector3 DefaultVector3Value;
        public Vector3 Vector3Value;

        public BaseParam(string identifier, ParameterType type) {
            Type = type;
            Identifier = identifier;

        }

        public static void ReplaceParamsValue(BaseParam newParam, BaseParam oldParam) {
            switch (newParam.Type) {
                case ParameterType.Boolean:
                    newParam.BoolValue = oldParam.BoolValue;

                    break;

                case ParameterType.Color:
                    newParam.ColorValue = oldParam.ColorValue;

                    break;
                case ParameterType.Float:

                    newParam.FloatValue = Mathf.Clamp(oldParam.FloatValue, newParam.DefaultFloatMinValue, newParam.DefaultFloatMaxValue);

                    break;
                case ParameterType.FoldOut:

                    newParam.IsOpenFoldout = oldParam.IsOpenFoldout;

                    break;
                case ParameterType.Image:

                    newParam.ImageValue = oldParam.ImageValue;

                    break;
                case ParameterType.Integer:

                    newParam.IntValue = Mathf.Clamp(oldParam.IntValue, newParam.DefaultIntMinValue, newParam.DefaultIntMaxValue);

                    break;
                case ParameterType.Material:

                    newParam.MaterialValue = oldParam.MaterialValue;

                    break;
                case ParameterType.Object:

                    newParam.ObjectValue = oldParam.ObjectValue;

                    break;
                case ParameterType.Collection:

                    newParam.ObjectValue = oldParam.ObjectValue;

                    break;
                case ParameterType.String:

                    newParam.StringValue = oldParam.StringValue;

                    break;
                case ParameterType.Vector:

                    newParam.Vector3Value = oldParam.Vector3Value;

                    float vecX = newParam.Vector3Value.x;
                    vecX = Mathf.Clamp(vecX, newParam.DefaultFloatMinValue, newParam.DefaultFloatMaxValue);

                    float vecY = newParam.Vector3Value.y;
                    vecY = Mathf.Clamp(vecY, newParam.DefaultFloatMinValue, newParam.DefaultFloatMaxValue);

                    float vecZ = newParam.Vector3Value.z;
                    vecZ = Mathf.Clamp(vecZ, newParam.DefaultFloatMinValue, newParam.DefaultFloatMaxValue);

                    newParam.Vector3Value.x = vecX;
                    newParam.Vector3Value.y = vecY;
                    newParam.Vector3Value.z = vecZ;

                    break;
            }
        }

    }

    // public interface IParam {
    //     // public BEngine.ParameterType Type {get;}
    //     // public string Name {get;}

    //     // public string Identifier {get;}
    // }

    // public interface IParam<T> : IParam
    // {
    //     // public T Value {get;}
    //     public T MinValue {get;}
    //     public T MaxValue {get;}
    //     public T DefaultValue {get;}
        

    // }
}