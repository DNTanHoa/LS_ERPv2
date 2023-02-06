using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ultils.Calculators
{
    public static class ShuntingYard
    {
        public static Queue<string> GetPostfixQueue(string input)
        {
            var outputQueue = new Queue<string>();
            var operandStack = new Stack<string>();
            var inputArray = input.Split(' ');

            foreach (var token in inputArray)
            {
                if (RPNToken.IsNumber(token))
                {
                    outputQueue.Enqueue(token);
                    continue;
                }

                if (RPNToken.IsConstant(token))
                {
                    outputQueue.Enqueue(RPNToken.ReplaceConstant(token));
                    continue;
                }

                if (RPNToken.IsLeftParenthesis(token) || RPNToken.IsFunction(token))
                {
                    operandStack.Push(token);
                    continue;
                }

                if (RPNToken.IsRightParenthesis(token))
                {
                    while (!RPNToken.IsLeftParenthesis(operandStack.Peek()))
                    {
                        outputQueue.Enqueue(operandStack.Pop());
                    }

                    operandStack.Pop();
                    continue;
                }

                while (operandStack.Any() && RPNToken.IsGreaterPrecedence(operandStack.Peek(), token)
                                          && RPNToken.IsLeftAssociated(token))
                {
                    outputQueue.Enqueue(operandStack.Pop());
                }

                operandStack.Push(token);
            }

            while (operandStack.Count > 0)
                outputQueue.Enqueue(operandStack.Pop());

            return outputQueue;
        }

        public static string InfixToPostfixString(string infixString)
        {
            var queue = GetPostfixQueue(infixString);
            var builder = new StringBuilder();

            while (queue.Any())
                builder.Append(queue.Dequeue() + " ");

            return builder.ToString();
        }
    }
}
