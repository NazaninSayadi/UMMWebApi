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
    public class CoefficientUnitTest
    {
        public ICoefficientUnitRepository coefficientUnitRepository;
        public IBaseUnitRepository baseUnitRepository;

        public Context context;


        public CoefficientUnitTest()
        {
            context = new SqliteInMemoryDb().CreateContext();

            coefficientUnitRepository = Substitute.For<CoefficientUnitRepository>(context);
            baseUnitRepository = Substitute.For<BaseUnitRepository>(context);
        }

        [TestMethod]
        public void IfDataIsValid_ItWillBeAddToCorrespondingDataSource()
        {
            //Arrange
            var coefficientUnitService = new CoefficientUnitService(baseUnitRepository, coefficientUnitRepository);
            var unitName = "MilliMetre";
            var MetricName = "Length";
            var symbol = "mm";
            var conversionFactor = 0.001M;
            var baseUnitSymbol = "m";


            //Action
            coefficientUnitService.CreateCoefficientUnit(unitName, MetricName, symbol, conversionFactor, baseUnitSymbol);

            //Assert
            var coefficientUnits = context.CoefficientUnits.Single(b => b.UnitName == unitName);
            Assert.AreEqual(unitName, coefficientUnits.UnitName);
        }

        [TestMethod]
        public void IfBaseUnitDoesntExist_ReturnException()
        {
            //Arrange
            var coefficientUnitService = new CoefficientUnitService(baseUnitRepository, coefficientUnitRepository);
            var unitName = "Hour";
            var MetricName = "Time";
            var symbol = "H";
            decimal conversionFactor = 3600;
            var baseUnitSymbol = "S";

            //Action and Assert
            Assert.ThrowsException<NotFoundException>(() => coefficientUnitService.CreateCoefficientUnit(unitName, MetricName, symbol, conversionFactor, baseUnitSymbol));
        }

        [TestMethod]
        public void IfSymbolIsRepetitive_ReturnException()
        {
            //Arrange
            var coefficientUnitService = new CoefficientUnitService(baseUnitRepository, coefficientUnitRepository);
            var unitName = "centimetre";
            var MetricName = "Length";
            var symbol = "cm";
            var conversionFactor = 0.01M;
            var baseUnitSymbol = "m";

            coefficientUnitService.CreateCoefficientUnit(unitName, MetricName, symbol, conversionFactor, baseUnitSymbol);

            //Action and Assert
            Assert.ThrowsException<DuplicateSymbolException>(() => coefficientUnitService.CreateCoefficientUnit(unitName, MetricName, symbol, conversionFactor, baseUnitSymbol));
        }

        [TestMethod]
        public void IfCoefficientUnitMetricAndItsBaseUnitIsIncompatible_ReturnException()
        {
            //Arrange
            var coefficientUnitService = new CoefficientUnitService(baseUnitRepository, coefficientUnitRepository);
            var unitName = "Kilogram";
            var MetricName = "Mass";
            var symbol = "kg";
            decimal conversionFactor = 1000;
            var baseUnitSymbol = "m";

            //Action and Assert
            Assert.ThrowsException<IncompatibleMetricException>(() => coefficientUnitService.CreateCoefficientUnit(unitName, MetricName, symbol, conversionFactor, baseUnitSymbol));
        }

        [TestMethod]
        public void IfConvertFromLargeToSmallerUnit_ItWillBeConvertToExpectedUnit()
        {
            //Arrange
            var coefficientUnitService = new CoefficientUnitService(baseUnitRepository, coefficientUnitRepository);

            coefficientUnitService.CreateCoefficientUnit("KiloMeter", "Length", "km", 1000, "m");
            coefficientUnitService.CreateCoefficientUnit("Centimetre", "Length", "cm", 0.01M, "m");

            var fromUnit = "km";
            var toUnit = "cm";
            decimal value = 1;
            decimal expectedValue = 100000;

            //Action
            var convertedValue = coefficientUnitService.Convert(fromUnit, toUnit, value);

            //Assert
            Assert.AreEqual(expectedValue, convertedValue);
        }

        [TestMethod]
        public void IfConvertFromLargeToBaseUnit_ItWillBeConvertToExpectedUnit()
        {
            //Arrange
            var coefficientUnitService = new CoefficientUnitService(baseUnitRepository, coefficientUnitRepository);

            coefficientUnitService.CreateCoefficientUnit("KiloMeter", "Length", "km", 1000, "m");

            var fromUnit = "km";
            var toUnit = "m";
            decimal value = 1;
            decimal expectedValue = 1000;

            //Action
            var convertedValue = coefficientUnitService.Convert(fromUnit, toUnit, value);

            //Assert
            Assert.AreEqual(expectedValue, convertedValue);
        }

        [TestMethod]
        public void IfConvertFromSmallToLargererUnit_ItWillBeConvertToExpectedUnit()
        {
            //Arrange
            var coefficientUnitService = new CoefficientUnitService(baseUnitRepository, coefficientUnitRepository);

            coefficientUnitService.CreateCoefficientUnit("KiloMeter", "Length", "km", 1000, "m");
            coefficientUnitService.CreateCoefficientUnit("Centimetre", "Length", "cm", 0.01M, "m");

            var fromUnit = "cm";
            var toUnit = "km";
            decimal value = 100000;
            decimal expectedValue = 1;

            //Action
            var convertedValue = coefficientUnitService.Convert(fromUnit, toUnit, value);

            //Assert
            Assert.AreEqual(expectedValue, convertedValue);
        }

        [TestMethod]
        public void IfConvertFromSmallToBaseUnit_ItWillBeConvertToExpectedUnit()
        {
            //Arrange
            var coefficientUnitService = new CoefficientUnitService(baseUnitRepository, coefficientUnitRepository);

            coefficientUnitService.CreateCoefficientUnit("Centimetre", "Length", "cm", 0.01M, "m");

            var fromUnit = "m";
            var toUnit = "cm";
            decimal value = 1;
            decimal expectedValue = 100;

            //Action
            var convertedValue = coefficientUnitService.Convert(fromUnit, toUnit, value);

            //Assert
            Assert.AreEqual(expectedValue, convertedValue);
        }

        [TestMethod]
        public void IfBothUnitLargerThanBaseUnit_ItWillBeConvertToExpectedUnit()
        {
            //Arrange
            var baseUnitService = new BaseUnitService(baseUnitRepository);
            baseUnitService.CreateBaseUnit("Gram", "Mass", "g");

            var coefficientUnitService = new CoefficientUnitService(baseUnitRepository, coefficientUnitRepository);

            coefficientUnitService.CreateCoefficientUnit("KiloGram", "Mass", "kg", 1000, "g");
            coefficientUnitService.CreateCoefficientUnit("Tonne", "Mass", "ton", 1000000, "g");

            var fromUnit = "kg";
            var toUnit = "ton";
            decimal value = 2000;
            decimal expectedValue = 2;

            //Action
            var convertedValue = coefficientUnitService.Convert(fromUnit, toUnit, value);

            //Assert
            Assert.AreEqual(expectedValue, convertedValue);
        }
    }
}