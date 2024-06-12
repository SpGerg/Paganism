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
            T instance;

            if (expression is null)
            {
                dictionary = GlobalDeclarated;
            }

            if (expression is not null && !Declarated.TryGetValue(expression, out dictionary))
            {
                if (TryGet(expression, name, expressionInfo, out instance))
                {
                    dictionary = GlobalDeclarated;
                }
                else
                {
                    dictionary = new Dictionary<string, T>();

                    Declarated.Add(expression, dictionary);
                }
            }

            if (!dictionary.TryGetValue(name, out instance))
            {
                dictionary.Add(name, value);
                instance = value;
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
            if (Language.TryGetValue(name, out var result))
            {
                return result;
            }

            if (expression is null)
            {
                if (GlobalDeclarated.TryGetValue(name, out var result1))
                {
                    return result1;
                }

                throw new InterpreterException($"Unknown {Name} with {name} name", expressionInfo);
            }

            if (Declarated.TryGetValue(expression, out var dictionary))
            {
                if (dictionary.TryGetValue(name, out var result2))
                {
                    return result2;
                }
            }

            var result3 = Get(expression.ExpressionInfo.Parent, name, expressionInfo);

            if (result3 is not null)
            {
                return result3;
            }

            throw new InterpreterException($"Unknown {Name} with {name} name", expressionInfo);
        }
    }
}
