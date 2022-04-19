using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using UMMBusinessLogic.Exceptions;
using UMMBusinessLogic.Repositories;
using UMMDomain;

namespace UMMBusinessLogic.Services
{
    public class FormulatedUnitService : IFormulatedUnitService
    {
        private readonly IFormulatedUnitRepository _formulatedUnitRepository;

        private readonly IBaseUnitRepository _baseUnitRepository;

        public FormulatedUnitService(IBaseUnitRepository baseUnitRepository,
            IFormulatedUnitRepository formulatedUnitRepository)
        {
            _baseUnitRepository = baseUnitRepository;
            _formulatedUnitRepository = formulatedUnitRepository;
        }

        public decimal Convert(string fromUnit, string toUnit, decimal value)
        {
            decimal convertedToBaseValue;
            string fromFormula = string.Empty;
            string toFormula = string.Empty;
            var fromFormulatedUnit = _formulatedUnitRepository.Get(fromUnit).Result;
            if (fromFormulatedUnit == null)
            {
                BaseUnit baseUnit = _baseUnitRepository.Get(fromUnit).Result;
                if (baseUnit == null)
                    throw new NotFoundException($"Unit not found: {fromUnit}");
            }
            else
                fromFormula = fromFormulatedUnit.Formula;
            var toFormulatedUnit = _formulatedUnitRepository.Get(toUnit).Result;
            if (toFormulatedUnit == null)
            {
                BaseUnit baseUnit = _baseUnitRepository.Get(toUnit).Result;
                if (baseUnit == null)
                    throw new NotFoundException($"Unit not found: {toUnit}");
            }
            else
                toFormula = toFormulatedUnit.Formula;

            if (!string.IsNullOrEmpty(fromFormula))
            {
                convertedToBaseValue = ExecuteFormula(fromFormula, value);
            }
            else
                convertedToBaseValue = value;


            decimal convertedValue;
            if (!string.IsNullOrEmpty(toFormula))
            {
                convertedValue = ExecuteFormula(toFormula, convertedToBaseValue);
            }
            else
                convertedValue = convertedToBaseValue;


            return convertedValue;

        }

        private decimal ExecuteFormula(string formula, decimal value)
        {
            StringBuilder sb = new();
            foreach (var item in formula)
            {
                if (char.IsLetter(item))
                    sb.Append(value);
                else
                    sb.Append(item);

            }
            return ParseFormula(sb.ToString());

        }

        public void CreateFormulatedUnit(string unitName, string metricName, string symbol, string formula, string baseUnitSymbol)
        {
            var formulatedUnit = _formulatedUnitRepository.Get(symbol).Result;

            if (formulatedUnit != null)
                if (formulatedUnit.Symbol == symbol)
                    throw new DuplicateSymbolException($"The unit {formulatedUnit.Symbol} is exist");

            BaseUnit baseUnit = _baseUnitRepository.Get(baseUnitSymbol).Result;
            if (baseUnit == null)
                throw new NotFoundException($"BaseUnit not found: {baseUnitSymbol}");

            if (baseUnit.MetricName != metricName)
                throw new IncompatibleMetricException($"FormulatedUnit is Incompatible with its BaseUnit");

            ValidateFormula(formula);

            formulatedUnit = new()
            {
                UnitName = unitName,
                MetricName = metricName,
                Symbol = symbol,
                BaseUnit = baseUnit,
                Formula = formula,
            };
            _formulatedUnitRepository.Add(formulatedUnit);
        }

        private void ValidateFormula(string formula)
        {
            CheckForumlaVariable(formula);
            CheckFormulaOperators(formula);
            CheckFormulaFormat(formula);
        }

        private void CheckFormulaFormat(string formula)
        {
            Stack<char> stack = new Stack<char>();
            for (int i = 0; i < formula.Length; i++)
            {
                char character = formula[i];
                if (character == '(')
                    stack.Push(character);
                else if (character == ')')
                {
                    if (!stack.Any())
                        throw new InvalidFromatException();
                    if (stack.Pop() != '(')
                        throw new InvalidFromatException();

                }
            }
            if (stack.Any())
                throw new InvalidFromatException();
        }

        private static void CheckFormulaOperators(string formula)
        {
            if (new Regex(@"[^a-zA-Z0-9()+\-\.*\/]").IsMatch(formula))
                throw new InvalidOpratorException();

        }
        private static void CheckForumlaVariable(string formula)
        {
            if (formula.Any(char.IsLetter))
            {
                if (new Regex("[b-zB-Z]").IsMatch(formula))
                    throw new InvalidCharacterException();
            }

        }

        public static decimal ParseFormula(string formula)
        {
            Queue expression = CreatePrioritizedExpression(formula);
            return Execute(expression);
        }

        private static Queue CreatePrioritizedExpression(string formula)
        {
            Stack operatorStorage = new();
            Queue expression = new();
            for (int i = 0; i < formula.Length;)
            {
                if (formula[i] == ' ')
                {
                    i++;
                    continue;
                }
                else if ((formula[i] >= '0' && formula[i] <= '9') || (formula[i] == '.'))
                {
                    decimal current = 0;
                    bool isNumber = false;
                    int num = 0;
                    while (i < formula.Length && ((formula[i] >= '0' && formula[i] <= '9') || (formula[i] == '.')))
                    {
                        if (formula[i] == '.')
                            isNumber = true;
                        else
                        {
                            if (!isNumber)
                                current = current * 10 + formula[i] - '0';
                            else
                            {
                                num++;
                                current += ((decimal)(formula[i] - '0')) / (decimal)Math.Pow(10, num);
                            }
                        }
                        i++;
                    }
                    expression.Enqueue(current.ToString());
                }
                else if (formula[i] == ')')
                {
                    while (operatorStorage.Count != 0 && (char)operatorStorage.Peek() != '(')
                    {
                        expression.Enqueue(operatorStorage.Pop() + "");
                    }
                    operatorStorage.Pop();
                    i++;
                }
                else
                {
                    while (operatorStorage.Count != 0 && CheckPriority((char)operatorStorage.Peek(), formula[i]) < 0)
                    {
                        expression.Enqueue(operatorStorage.Pop() + "");
                    }
                    operatorStorage.Push(formula[i]);
                    i++;
                }
            }
            while (operatorStorage.Count != 0)
            {
                expression.Enqueue(operatorStorage.Pop() + "");
            }
            return expression;
        }

        private static int CheckPriority(char mathOperator, char currentChar)
        {
            if (mathOperator == '(' || currentChar == '(') return 1;
            if (currentChar == '+' || currentChar == '-') return -1;
            if (currentChar == '*' && (mathOperator == '*' || mathOperator == '/')) return -1;
            if (currentChar == '/' && (mathOperator == '*' || mathOperator == '/')) return -1;
            return 1;
        }

        private static decimal Execute(Queue expression)
        {
            Stack finalResult = new();
            while (expression.Count != 0)
            {
                string mathOperator = (string)expression.Dequeue();
                if (mathOperator.Equals("+") || mathOperator.Equals("-") || mathOperator.Equals("*") || mathOperator.Equals("/"))
                {
                    decimal secondNumber = (decimal)finalResult.Pop();
                    decimal firstNumber = (decimal)finalResult.Pop();
                    decimal result = Calculate(firstNumber, secondNumber, mathOperator);
                    finalResult.Push(result);
                }
                else
                {
                    finalResult.Push(decimal.Parse(mathOperator));
                }
            }
            return (decimal)finalResult.Pop();
        }

        private static decimal Calculate(decimal firstNumber, decimal secondNumber, string mathOperator) => mathOperator switch
        {
            "+" => firstNumber + secondNumber,
            "-" => firstNumber - secondNumber,
            "*" => firstNumber * secondNumber,
            "/" => firstNumber / secondNumber
        };
    }
}
