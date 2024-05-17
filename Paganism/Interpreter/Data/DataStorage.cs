using Paganism.Exceptions;
using Paganism.Interpreter.Data.Instances;
using Paganism.PParser;
using Paganism.PParser.AST;
using System.Collections.Generic;

namespace Paganism.Interpreter.Data
{
    public abstract class DataStorage<T> where T : Instance
    {
        public abstract string Name { get; }

        private Dictionary<BlockStatementExpression, Dictionary<string, T>> Declarated { get; } = new Dictionary<BlockStatementExpression, Dictionary<string, T>>();

        private Dictionary<string, T> GlobalDeclarated { get; } = new Dictionary<string, T>();

        protected virtual IReadOnlyDictionary<string, T> Language { get; } = new Dictionary<string, T>();

        public void Set(ExpressionInfo expressionInfo, BlockStatementExpression expression, string name, T value)
        {
            Dictionary<string, T> dictionary = null;
            T instance = null;

            if (expression is null)
            {
                dictionary = GlobalDeclarated;

                if (!dictionary.TryGetValue(name, out instance))
                {
                    instance = value;

                    dictionary.Add(name, value);
                }
            }

            if (expression is not null && !Declarated.TryGetValue(expression, out dictionary))
            {
                dictionary = new Dictionary<string, T>();

                Declarated.Add(expression, dictionary);
            }

            if (!dictionary.TryGetValue(name, out instance))
            {
                dictionary.Add(name, value);
            }

            if (instance is null)
            {
                dictionary[name] = value;
                return;
            }

            if (instance.Info.IsReadOnly && expressionInfo.Filepath != instance.Info.FilePath)
            {
                throw new InterpreterException($"You cant access to {value.InstanceName} with '{name}' name", expressionInfo);
            }

            dictionary[name] = value;
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

        public bool TryGet(BlockStatementExpression expression, string name, ExpressionInfo expressionInfo, out T value)
        {
            try
            {
                value = Get(expression, name, expressionInfo);

                return true;
            }
            catch (InterpreterException exception)
            {
                //I will make for every exception, separate exception class. 
                if (exception.Message.Contains("access"))
                {
                    throw exception;
                }

                value = null;
                return false;
            }
        }

        public T Get(BlockStatementExpression expression, string name, ExpressionInfo expressionInfo)
        {
            if (Language.ContainsKey(name))
            {
                return Language[name];
            }

            Instance finallyResult = null;

            if (expression is null)
            {
                if (!GlobalDeclarated.ContainsKey(name))
                {
                    throw new InterpreterException($"Unknown {Name} with '{name}' name", expressionInfo);
                }

                finallyResult = GlobalDeclarated[name];
            }
            else if (!Declarated.TryGetValue(expression, out _))
            {
                Declarated.Add(expression, new Dictionary<string, T>());
            }
            else if (Declarated[expression].TryGetValue(name, out var result3))
            {
                finallyResult = result3;
            }

            if (finallyResult is not null)
            {
                if (!finallyResult.Info.IsShow && finallyResult.Info.FilePath != expressionInfo.Filepath)
                {
                    throw new InterpreterException($"You cant access to {finallyResult.InstanceName} with '{name}' name", expressionInfo);
                }

                return (T)finallyResult;
            }

            if (!Language.TryGetValue(name, out var result) && !Declarated[expression].TryGetValue(name, out var result1))
            {
                var value = Get(expression.ExpressionInfo.Parent, name, expressionInfo);

                if (value != null)
                {
                    finallyResult = value;
                }
                else
                {
                    throw new InterpreterException($"Unknown {Name} with '{name}' name", expression.ExpressionInfo);
                }
            }

            if (finallyResult is null)
            {
                var result2 = Declarated[expression].TryGetValue(name, out result1);

                if (result is null && !result2)
                {
                    throw new InterpreterException($"Unknown {Name} with '{name}' name", expression.ExpressionInfo);
                }

                finallyResult = result2 ? result1 : result;
            }

            if (!finallyResult.Info.IsShow && finallyResult.Info.FilePath != expressionInfo.Filepath)
            {
                throw new InterpreterException($"You cant access to {finallyResult.InstanceName} with '{name}' name", expressionInfo);
            }

            return (T)finallyResult;
        }
    }
}
