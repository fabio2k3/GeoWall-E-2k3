using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoWallE_Fabio_2k3
{
    public class Scope
    {
        private Dictionary<string, Stack<object>> variableValues;
        private Dictionary<string, WalleType> variableTypes;
        private readonly Scope parentScope;

        public Scope(Scope parent = null)
        {
            variableValues = new Dictionary<string, Stack<object>>();
            variableTypes = new Dictionary<string, WalleType>();
            parentScope = parent;
        }

        public void CreateVariableInstance(string name, WalleType type) // definir variable
        {
            if (variableTypes.ContainsKey(name))
                throw new InvalidOperationException($"Cannot re-define constant {name}");

            variableTypes[name] = type;
            variableValues[name] = new Stack<object>();
        }

        public void AssignVariable(string name, object value, WalleType type)
        {
            if (variableTypes[name] == WalleType.Undefined)
                variableTypes[name] = type;

            variableValues[name].Push(value);
        }
        public WalleType GetVariableType(string name) // obtener el tipo
        {
            if (variableTypes.ContainsKey(name))
                return variableTypes[name];

            else if (parentScope != null)
                return parentScope.GetVariableType(name);

            throw new NullReferenceException($"Use of undefined variable \"{name}\"");
        }
        public object GetVariableValue(string name) // obtener el valor
        {
            if (variableValues.TryGetValue(name, out Stack<object> valuesStack))
            {
                if (valuesStack.Count > 0)
                {
                    return valuesStack.Peek();
                }
                throw new InvalidOperationException($"Variable \"{name}\" has not been initialized");
            }
            else if (parentScope != null)
                return parentScope.GetVariableValue(name);

            throw new NullReferenceException($"Variable \"{name}\" has not been initialized");
        }
        public void RemoveLast(string name)
        {
            variableValues[name].Pop();
        }
    }
}
