using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ultils.Calculators
{
    public static class InfixEvaluator
    {
        public static double EvaluateInfix(Queue<string> postfix)
        {
            var resultStack = new Stack<double>();

            while (postfix.Any())
            {
                var currentToken = postfix.Dequeue();

                if (RPNToken.IsNumber(currentToken))
                {
                    resultStack.Push(double.Parse(currentToken));
                    continue;
                }

                if (RPNToken.IsOperator(currentToken))
                {
                    var val1 = resultStack.Pop();
                    var val2 = resultStack.Pop();
                    var output = RPNToken.Evaluate(val2, val1, currentToken);
                    resultStack.Push(output);
                    continue;
                }

                if (RPNToken.IsFunction(currentToken))
                {
                    var value = resultStack.Pop();
                    var result = RPNToken.Evaluate(value, currentToken);
                    resultStack.Push(result);
                }
            }

            return resultStack.Pop();
        }

        public static double EvaluateInfix(string infixString)
        {
            var resultStack = new Stack<double>();
            var postfixQueue = ShuntingYard.GetPostfixQueue(infixString);

            while (postfixQueue.Any())
            {
                var currentToken = postfixQueue.Dequeue();

                if (RPNToken.IsNumber(currentToken))
                {
                    resultStack.Push(double.Parse(currentToken));
                    continue;
                }

                if (RPNToken.IsOperator(currentToken))
                {
                    var val1 = resultStack.Pop();
                    var val2 = resultStack.Pop();
                    var output = RPNToken.Evaluate(val2, val1, currentToken);
                    resultStack.Push(output);
                    continue;
                }

                if (RPNToken.IsFunction(currentToken))
                {
                    var value = resultStack.Pop();
                    var result = RPNToken.Evaluate(value, currentToken);
                    resultStack.Push(result);
                }
            }

            return resultStack.Pop();
        }
    }
}
