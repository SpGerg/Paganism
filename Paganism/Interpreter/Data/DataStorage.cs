using Paganism.Exceptions;
using Paganism.PParser;
using Paganism.PParser.AST;
using Paganism.PParser.AST.Enums;
using System.Collections.Generic;

namespace Paganism.Interpreter.Data
{
    public abstract class DataStorage<T>
    {
        public abstract string Name { get; }

        private Dictionary<BlockStatementExpression, Dictionary<string, T>> Declarated { get; } = new Dictionary<BlockStatementExpression, Dictionary<string, T>>();

        private Dictionary<string, T> GlobalDeclarated { get; } = new Dictionary<string, T>();

        protected virtual IReadOnlyDictionary<string, T> Language { get; } = new Dictionary<string, T>();

        public void Set(BlockStatementExpression expression, string name, T value)
        {
            if (expression is null)
            {
                GlobalDeclarated[name] = value;
                return;
            }

            if (!Declarated.TryGetValue(expression, out _))
            {
                Declarated.Add(expression, new Dictionary<string, T>());
            }

            if (Declarated[expression].ContainsKey(name))
            {
                Declarated[expression][name] = value;
            }
            else
            {
                Declarated[expression].Add(name, value);
            }
        }

        public void Remove(BlockStatementExpression expression, string name)
        {
            if (expression is null)
            {
                GlobalDeclarated.Remove(name);
                return;
            }

            if (!Declarated.TryGetValue(expression, out _))
            {
                Declarated.Add(expression, new Dictionary<string, T>());
            }

            Declarated[expression].Remove(name);
        }

        public void Clear()
        {
            Declarated.Clear();
            GlobalDeclarated.Clear();
        }

        public void Clear(BlockStatementExpression expression)
        {
            if (expression is null)
            {
                GlobalDeclarated.Clear();
                return;
            }

            if (!Declarated.ContainsKey(expression))
            {
                return;
            }

            Declarated[expression].Clear();
        }

        public bool IsLanguage(string name)
        {
            return Language.ContainsKey(name);
        }

        public bool TryGet(BlockStatementExpression expression, string name, out T value, ExpressionInfo expressionInfo)
        {
            try
            {
                value = Get(expression, name, expressionInfo);

                return true;
            }
            catch
            {
                value = default;
                return false;
            }
        }

        public T Get(BlockStatementExpression expression, string name, ExpressionInfo expressionInfo)
        {
            if (Language.ContainsKey(name))
            {
                return Language[name];
            }

            if (expression is null)
            {
                if (!GlobalDeclarated.ContainsKey(name))
                {
                    throw new InterpreterException($"Unknown {Name} with '{name}' name", expressionInfo);
                }

                return GlobalDeclarated[name];
            }

            if (!Declarated.TryGetValue(expression, out _))
            {
                Declarated.Add(expression, new Dictionary<string, T>());
            }

            if (Declarated[expression].TryGetValue(name, out var result3))
            {
                return result3;
            }

            if (!Language.TryGetValue(name, out var result) && !Declarated[expression].TryGetValue(name, out var result1))
            {
                var value = Get(expression.ExpressionInfo.Parent, name, expressionInfo);

                if (value != null)
                {
                    return value;
                }

                throw new InterpreterException($"Unknown {Name} with '{name}' name", expression.ExpressionInfo);
            }

            var result2 = Declarated[expression].TryGetValue(name, out result1);

            if (result is null && !result2)
            {
                throw new InterpreterException($"Unknown {Name} with '{name}' name", expression.ExpressionInfo);
            }

            return result2 ? result1 : result;
        }
    }
}
