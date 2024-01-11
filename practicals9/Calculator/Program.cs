using System;
using System.Collections.Generic;

public class Program
{
    public static void Main()
    {
        try
        {
            string expression = Console.ReadLine();
            int result = Calculator.EvaluateExpression(expression);
            Console.WriteLine(result);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}

public class Calculator{
    public static int EvaluateExpression(string expression)
    {
        TokenReader tokenReader = new TokenReader(expression);
        try
        {
            ExpressionNode expressionTree = BuildExpressionTree(tokenReader);
            if (tokenReader.NextToken() == null)
            {
                return EvaluateExpressionTree(expressionTree);
            }
            else
            {
                throw new FormatException("Format Error");
            }
        }
        catch (FormatException)
        {
            throw new FormatException("Format Error");
        }
        catch (OverflowException)
        {
            throw new OverflowException("Overflow Error");
        }
        catch (DivideByZeroException)
        {
            throw new DivideByZeroException("Divide Error");
        }
    }

    private static ExpressionNode BuildExpressionTree(TokenReader tokenReader)
    {
        string token = tokenReader.NextToken();

        if (string.IsNullOrEmpty(token))
        {
            throw new FormatException("Format Error");
        }

        if (IsNumeric(token))
        {
            return new ValueNode(int.Parse(token));
        }
        else if (token == "~")
        {
            return new UnaryNode(UnaryOperator.Negation, BuildExpressionTree(tokenReader));
        }
        else if (token == "+" || token == "-" || token == "*" || token == "/")
        {
            BinaryOperator binaryOperator = GetBinaryOperator(token);
            ExpressionNode operand1 = BuildExpressionTree(tokenReader);
            ExpressionNode operand2 = BuildExpressionTree(tokenReader);

            return new BinaryNode(binaryOperator, operand1, operand2);
        }
        else
        {
            throw new FormatException("Format Error");
        }
    }

    private static int EvaluateExpressionTree(ExpressionNode expressionTree)
    {
        return expressionTree.Evaluate();
    }

    private static BinaryOperator GetBinaryOperator(string token)
    {
        switch (token)
        {
            case "+":
                return BinaryOperator.Addition;
            case "-":
                return BinaryOperator.Subtraction;
            case "*":
                return BinaryOperator.Multiplication;
            case "/":
                return BinaryOperator.Division;
            default:
                throw new FormatException("Format Error");
        }
    }

    private static bool IsNumeric(string token)
    {
        return int.TryParse(token, out _);
    }

    private abstract class ExpressionNode
    {
        public abstract int Evaluate();
    }

    private class ValueNode : ExpressionNode
    {
        private readonly int value;

        public ValueNode(int value)
        {
            this.value = value;
        }

        public override int Evaluate()
        {
            return value;
        }
    }

    private class UnaryNode : ExpressionNode
    {
        private readonly UnaryOperator unaryOperator;
        private readonly ExpressionNode operand;

        public UnaryNode(UnaryOperator unaryOperator, ExpressionNode operand)
        {
            this.unaryOperator = unaryOperator;
            this.operand = operand;
        }

        public override int Evaluate()
        {
            int result = operand.Evaluate();

            switch (unaryOperator)
            {
                case UnaryOperator.Negation:
                    return checked(-result);
                default:
                    throw new FormatException("Format Error");
            }
        }
    }

    private class BinaryNode : ExpressionNode
    {
        private readonly BinaryOperator binaryOperator;
        private readonly ExpressionNode operand1;
        private readonly ExpressionNode operand2;

        public BinaryNode(BinaryOperator binaryOperator, ExpressionNode operand1, ExpressionNode operand2)
        {
            this.binaryOperator = binaryOperator;
            this.operand1 = operand1;
            this.operand2 = operand2;
        }

        public override int Evaluate()
        {
            int value1 = operand1.Evaluate();
            int value2 = operand2.Evaluate();

            switch (binaryOperator)
            {
                case BinaryOperator.Addition:
                    return checked(value1 + value2);
                case BinaryOperator.Subtraction:
                    return checked(value1 - value2);
                case BinaryOperator.Multiplication:
                    return checked(value1 * value2);
                case BinaryOperator.Division:
                    if (value2 == 0)
                    {
                        throw new DivideByZeroException("Divide Error");
                    }
                    return checked(value1 / value2);
                default:
                    throw new FormatException("Format Error");
            }
        }
    }

    private enum UnaryOperator
    {
        Negation
    }

    private enum BinaryOperator
    {
        Addition,
        Subtraction,
        Multiplication,
        Division
    }

    private class TokenReader
    {
        private readonly string[] tokens;
        private int currentIndex;

        public TokenReader(string expression)
        {
            tokens = expression.Split(' ');
            currentIndex = 0;
        }

        public string NextToken()
        {
            if (currentIndex < tokens.Length)
            {
                return tokens[currentIndex++];
            }
            return null;
        }
    }
}