using System;
using System.Collections.Generic;
using UnityEngine;

namespace Variables
{
    [System.Serializable]
    public class Change
    {
        public enum Case
        {
            set,
            add,
            substract,
            min,
            max,
            multiply,
            divide
        }
        public string variableName;
        public float value;
        public Case change;
    }
    public class Variable
    {
        public bool incremental = false;
        float Value;
        public Variable(float value, bool i)
        {
            SetFloatValue(value);
            incremental = i;
        }
        public Variable(SerializedVariable var)
        {
            SetFloatValue(var.value);
            incremental = var.incremental;
        }
        public Variable(float value)
        {
            SetFloatValue(value);
        }
        public Variable(bool value)
        {
            SetBoolValue(value);
        }
        public void SetBoolValue(bool nValue)
        {
            Value = nValue ? 1 : 0;
        }
        public bool GetBoolValue()
        {
            return Value > 0;
        }
        public void SetFloatValue(float nValue)
        {
            Value = nValue;
        }
        public float GetFloatValue()
        {
            return Value;
        }
        public void Change(Change.Case change, float value)
        {
            switch (change)
            {
                case Variables.Change.Case.set:
                    if (!incremental)
                        Value = value;
                    break;
                case Variables.Change.Case.min:
                    if (!incremental)
                        Value = Mathf.Min(Value, value);
                    break;
                case Variables.Change.Case.max:
                    if (!incremental)
                        Value = Mathf.Max(Value, value);
                    break;
                case Variables.Change.Case.add:
                    Value += value;
                    break;
                case Variables.Change.Case.substract:
                    Value -= value;
                    break;
                case Variables.Change.Case.multiply:
                    Value *= value;
                    break;
                case Variables.Change.Case.divide:
                    Value /= value;
                    break;
            }
        }
    }
    [System.Serializable]
    public class Condition
    {
        public enum Check
        {
            equal,
            less,
            lesseq,
            greater,
            greatereg,
            boolean,
        }
        public string variableName;
        public float value;
        public Check check;
        public bool ConditionMet(float original)
        {
            switch (check)
            {
                case Condition.Check.equal:
                    return original == value;
                case Condition.Check.boolean:
                    return original > 0;
                case Condition.Check.less:
                    return original < value;
                case Condition.Check.lesseq:
                    return original <= value;
                case Condition.Check.greater:
                    return original > value;
                case Condition.Check.greatereg:
                    return original >= value;
            }
            return true;
        }
    }
    public class VariableScope
    {
        #region Store Variables
        public Dictionary<string, Variable> scope = new Dictionary<string, Variable>();
        public bool HasVariable(string vName)
        {
            return (scope.ContainsKey(vName.ToLower()));
        }
        public Variable GetVariable(string vName)
        {
            vName = vName.ToLower();
            if (scope.ContainsKey(vName))
            {
                return scope[vName];
            }
            return SetVariable(vName, 0);
        }

        public Variable SetVariable(string vName, float nValue)
        {
            vName = vName.ToLower();
            Debug.Log("Set variable " + vName + " to " + nValue);
            if (scope.ContainsKey(vName))
            {
                scope[vName].SetFloatValue(nValue);
            }
            else
            {
                scope.Add(vName, new Variable(nValue));
            }
            OnVariableChange(vName);
            return scope[vName];
        }
        public Variable SetVariable(string vName, bool nValue)
        {
            vName = vName.ToLower();
            Debug.Log("Set variable " + vName + " to " + nValue);
            if (scope.ContainsKey(vName))
            {
                scope[vName].SetBoolValue(nValue);
            }
            else
            {
                scope.Add(vName, new Variable(nValue));
            }
            OnVariableChange(vName);
            return scope[vName];
        }
        protected virtual void OnVariableChange(string variable)
        {
        }

        public void Apply(VariableScope temp)
        {
            if (temp == null)
                return;
            foreach (KeyValuePair<string, Variable> num in temp.scope)
            {
                SetVariable(num.Key, num.Value.GetFloatValue());
            }
        }
        #endregion
        #region Set Variables
        public void Apply(Change[] changes)
        {
            if (changes == null || changes.Length == 0)
                return;
            foreach (Change num in changes)
            {
                Apply(num);
            }
        }
        public void Apply(Change change)
        {
            ChangeVariable(change.variableName.ToLower(), change.change, change.value);
        }
        public virtual void ChangeVariable(string variableName, Change.Case change, float value)
        {
            string vNameLower = variableName.ToLower();
            Debug.Log($"[Variable] Change variable {vNameLower} to {change} {value}");
            GetVariable(vNameLower).Change(change, value);
            OnVariableChange(vNameLower);
        }
        #endregion
        #region Conditions
        public virtual bool ConditionMet(Condition c)
        {
            return c.ConditionMet(GetVariable(c.variableName).GetFloatValue());
        }
        public bool AllConditionsMet(Condition[] cs)
        {
            if (cs != null && cs.Length > 0)
                foreach (Condition c in cs)
                {
                    if (!ConditionMet(c))
                        return false;
                }
            return true;
        }
        #endregion
        public void OutputDebugData()
        {
            foreach (var kvp in scope)
            {
                Debug.Log($"Variable ({kvp.Key}) at {kvp.Value.GetFloatValue()}");
            }
        }
        public void ClearVars()
        {
            scope.Clear();
        }
    }
    [Serializable]
    public class VariableScopeSerializable
    {
        public SerializedVariable[] scope;
        public VariableScopeSerializable(VariableScope original)
        {
            List<SerializedVariable> serializedVars = new();
            foreach (var variable in original.scope)
            {
                serializedVars.Add(new SerializedVariable(variable.Key, variable.Value));
            }
            scope = serializedVars.ToArray();
        }
        public virtual void Deserialize(VariableScope data)
        {
            foreach (var v in scope)
            {
                data.scope.Add(v.name, new Variable(v));
            }
        }
    }
    [Serializable]
    public class SerializedVariable
    {
        public bool incremental;
        public string name;
        public float value;
        public SerializedVariable() { }
        public SerializedVariable(string n, Variable variable)
        {
            name = n;
            incremental = variable.incremental;
            value = variable.GetFloatValue();
        }
    }
}