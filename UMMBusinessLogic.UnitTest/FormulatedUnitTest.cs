using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Linq;
using UMMBusinessLogic.Exceptions;
using UMMBusinessLogic.Services;
using UMMBusinessLogic.Repositories;
using UMMBusinessLogic.UnitTest.FackeDb;
using UMMBusinessLogic.UnitTest.FakeDb;
using UMMBusinessLogic.UnitTest.FakeDb.Repositories;

namespace UMMBusinessLogic.UnitTest
{

    [TestClass]
    public class FormulatedUnitTest
    {
        public IFormulatedUnitRepository formulatedUnitRepository;
        public IBaseUnitRepository baseUnitRepository;

        public Context context;


        public FormulatedUnitTest()
        {
            context = new SqliteInMemoryDb().CreateContext();

            formulatedUnitRepository = Substitute.For<FormulatedUnitRepository>(context);
            baseUnitRepository = Substitute.For<BaseUnitRepository>(context);
        }

        [TestMethod]
        public void IfDataIsValid_ItWillBeAddToCorrespondingDataSource()
        {
            //Arrange
            var baseUnitService = new BaseUnitService(baseUnitRepository);
            baseUnitService.CreateBaseUnit("Celsius", "Temperature", "°C");

            var formulatedUnitService = new FormulatedUnitService(baseUnitRepository, formulatedUnitRepository);
            var unitName = "Fahrenheit";
            var MetricName = "Temperature";
            var symbol = "°F";
            var formula = "(a-32)*5/9";
            var baseUnitSymbol = "°C";


            //Action
            formulatedUnitService.CreateFormulatedUnit(unitName, MetricName, symbol, formula, baseUnitSymbol);

            //Assert
            var formulatedUnit = context.FormulatedUnits.Single(b => b.UnitName == unitName);
            Assert.AreEqual(unitName, formulatedUnit.UnitName);
        }

        [TestMethod]
        public void IfSymbolIsRepetitive_ReturnException_CheckSybmolIsUnique()
        {
            //Arrange
            var baseUnitService = new BaseUnitService(baseUnitRepository);
            baseUnitService.CreateBaseUnit("Celsius", "Temperature", "°C");

            var formulatedUnitService = new FormulatedUnitService(baseUnitRepository, formulatedUnitRepository);
            var unitName = "Fahrenheit";
            var MetricName = "Temperature";
            var symbol = "°F";
            var formula = "(a-32)*5/9";
            var baseUnitSymbol = "°C";
            formulatedUnitService.CreateFormulatedUnit(unitName, MetricName, symbol, formula, baseUnitSymbol);


            //Action and Assert
            Assert.ThrowsException<DuplicateSymbolException>(() => formulatedUnitService.CreateFormulatedUnit(unitName, MetricName, symbol, formula, baseUnitSymbol));
        }

        [TestMethod]
        public void IfFormulaHasFrobiddenCharacter_ReturnException()
        {
            //Arrange
            var baseUnitService = new BaseUnitService(baseUnitRepository);
            baseUnitService.CreateBaseUnit("Celsius", "Temperature", "°C");

            var formulatedUnitService = new FormulatedUnitService(baseUnitRepository, formulatedUnitRepository);
            var unitName = "Fahrenheit";
            var MetricName = "Temperature";
            var symbol = "°F";
            var formula = "(b-32)*5/9";
            var baseUnitSymbol = "°C";

            //Action and Assert
            Assert.ThrowsException<InvalidCharacterException>(() => formulatedUnitService.CreateFormulatedUnit(unitName, MetricName, symbol, formula, baseUnitSymbol));
        }

        [TestMethod]
        public void IfFormulaHasFrobiddenOperator_ReturnException()
        {
            //Arrange
            var baseUnitService = new BaseUnitService(baseUnitRepository);
            baseUnitService.CreateBaseUnit("Celsius", "Temperature", "°C");

            var formulatedUnitService = new FormulatedUnitService(baseUnitRepository, formulatedUnitRepository);
            var unitName = "Fahrenheit";
            var MetricName = "Temperature";
            var symbol = "°F";
            var formula = "(a^32)*5/9%7";
            var baseUnitSymbol = "°C";

            //Action and Assert
            Assert.ThrowsException<InvalidOpratorException>(() => formulatedUnitService.CreateFormulatedUnit(unitName, MetricName, symbol, formula, baseUnitSymbol));
        }

        [TestMethod]
        public void IfFormulaFormatIsIncorrect_ReturnException()
        {
            //Arrange
            var baseUnitService = new BaseUnitService(baseUnitRepository);
            baseUnitService.CreateBaseUnit("Celsius", "Temperature", "°C");

            var formulatedUnitService = new FormulatedUnitService(baseUnitRepository, formulatedUnitRepository);
            var unitName = "Fahrenheit";
            var MetricName = "Temperature";
            var symbol = "F";
            var formula = "(a-32))*((5/9)";
            var baseUnitSymbol = "°C";

            //Action and Assert
            Assert.ThrowsException<InvalidFromatException>(() => formulatedUnitService.CreateFormulatedUnit(unitName, MetricName, symbol, formula, baseUnitSymbol));
        }

        [TestMethod]
        public void IfConvertFormulatedUnitToBase_ItWillBePass()
        {
            //Arrange
            var baseUnitService = new BaseUnitService(baseUnitRepository);
            baseUnitService.CreateBaseUnit("Celsius", "Temperature", "°C");

            var formulatedUnitService = new FormulatedUnitService(baseUnitRepository, formulatedUnitRepository);
            formulatedUnitService.CreateFormulatedUnit("Fahrenheit", "Temperature", "F", "(a-32)*5/9", "°C");

            

            var fromUnit = "F";
            var toUnit = "°C";
            decimal value = 32;
            decimal expectedValue = 0;

            //Action
            var convertedValue = formulatedUnitService.Convert(fromUnit, toUnit, value);

            //Assert
            Assert.AreEqual(expectedValue, convertedValue);
        }

        [TestMethod]
        public void IfConvertBaseUnitToFormulatedUnit_ItWillBePass()
        {
            //Arrange
            var baseUnitService = new BaseUnitService(baseUnitRepository);
            baseUnitService.CreateBaseUnit("Celsius", "Temperature", "°C");

            var formulatedUnitService = new FormulatedUnitService(baseUnitRepository, formulatedUnitRepository);
            formulatedUnitService.CreateFormulatedUnit("Fahrenheit", "Temperature", "F", "(a*9/5)+32", "°C");

            var fromUnit = "°C";
            var toUnit = "F";
            decimal value = 0;
            decimal expectedValue = 32;

            //Action
            var convertedValue = formulatedUnitService.Convert(fromUnit, toUnit, value);

            //Assert
            Assert.AreEqual(expectedValue, convertedValue);
        }
        [TestMethod]
        public void ConvertOneFormulatedUnitToAnother_ItWillBePass()
        {
            //Arrange
            var baseUnitService = new BaseUnitService(baseUnitRepository);
            baseUnitService.CreateBaseUnit("Celsius", "Temperature", "°C");

            var formulatedUnitService = new FormulatedUnitService(baseUnitRepository, formulatedUnitRepository);
            formulatedUnitService.CreateFormulatedUnit("Fahrenheit", "Temperature", "F", "(a-32)*5/9", "°C");
            formulatedUnitService.CreateFormulatedUnit("Kelvint", "Temperature", "K", "a+273.15", "°C");

            var fromUnit = "F";
            var toUnit = "K";
            decimal value = 32;
            decimal expectedValue = 273.15M;

            //Action
            var convertedValue = formulatedUnitService.Convert(fromUnit, toUnit, value);

            //Assert
            Assert.AreEqual(expectedValue, convertedValue);
        }
    }
}