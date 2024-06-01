using System.Collections;
using System.Collections.Generic;

using System;
using UnityEngine;

using System.Linq;

namespace BEngine
{
    [Serializable]
    public class BEngineInputs
    {

        public List<BaseParam> CurrentParams;


        public void ReplacePreviousParamsValues(Dictionary<string, BaseParam> prevParamsDict) {
            foreach (var newBaseParam in CurrentParams) {
                // IParam iNewParam = ((IParam)param);

                if (prevParamsDict.Keys.Contains<string>(newBaseParam.Identifier) ) {
                    var prevBaseParam = prevParamsDict[newBaseParam.Identifier];

                    if (newBaseParam.Type == prevBaseParam.Type) {
                        BaseParam.ReplaceParamsValue(newBaseParam, prevBaseParam);
                    }

                    // Change Float and Integer Values
                    else if (newBaseParam.Type == ParameterType.Float && prevBaseParam.Type == ParameterType.Integer) {
                        newBaseParam.FloatValue =  Mathf.Clamp((float)prevBaseParam.IntValue, 
                                                                    newBaseParam.DefaultFloatMinValue, newBaseParam.DefaultFloatMaxValue);
                    }
                    else if (newBaseParam.Type == ParameterType.Integer && prevBaseParam.Type == ParameterType.Float) {
                        newBaseParam.IntValue = (int)Mathf.Round(Mathf.Clamp(prevBaseParam.FloatValue,
                                                                            newBaseParam.DefaultIntMinValue, newBaseParam.DefaultIntMaxValue));
                    }

                    // Change Object and Collection Values
                    else if ((newBaseParam.Type == ParameterType.Object && prevBaseParam.Type == ParameterType.Collection)
                    || (newBaseParam.Type == ParameterType.Collection && prevBaseParam.Type == ParameterType.Object)) {
                        newBaseParam.ObjectValue = prevBaseParam.ObjectValue;
                    }
                }
            }
        }


    }
}