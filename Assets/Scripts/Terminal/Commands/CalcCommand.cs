using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;

public class CalcCommand : ICommand
{
    public int MaxArguments => 100; 
    public List<string> Execute(string[] args, TerminalManager terminalManager, List<string> previousOutput = null)
    {
        // Combine all arguments after the command into a single string
        string expression = string.Join(" ", args, 1, args.Length - 1);

        // Validate the expression
        if (IsValidExpression(expression))
        {
            try
            {
                // Use DataTable to evaluate the expression
                var result = EvaluateExpression(expression);
                return new List<string> { $"{result}" };
            }
            catch (Exception e)
            {
                // Catch and return any errors encountered during evaluation
                return new List<string> { $"Error evaluating expression: {e.Message}" };
            }
        }
        else
        {
            // Return an error message if the expression is not valid
            return new List<string> { "Invalid expression. Please use only numbers and basic arithmetic operators (+, -, *, /)." };
        }
    }

    private object EvaluateExpression(string expression)
    {
        DataTable table = new DataTable();
        expression = expression.Replace(",", ".");
        return table.Compute(expression, string.Empty);
    }

    private bool IsValidExpression(string expression)
    {
        // Regular expression to validate allowed characters and basic structure
        string pattern = @"^[\d+\-*/\(\)\.\s]+$";
        return Regex.IsMatch(expression, pattern);
    }
}
