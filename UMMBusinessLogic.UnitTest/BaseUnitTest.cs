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
    public class BaseUnitTest
    {
        public IBaseUnitRepository baseUnitRepository;
        public Context context;


        public BaseUnitTest()
        {
            context = new SqliteInMemoryDb().CreateContext();
            baseUnitRepository = Substitute.For<BaseUnitRepository>(context);
        }

        [TestMethod]
        public void IfDataIsValid_ItWillBeAddToCorrespondingDataSource()
        {
            //Arrange
            var baseUnitService = new BaseUnitService(baseUnitRepository);
            var unitName = "Gram";
            var MetricName = "Mass";
            var symbol = "g";

            //Action
            baseUnitService.CreateBaseUnit(unitName, MetricName, symbol);

            //Assert
            var baseUnit = context.BaseUnits.Single(b => b.UnitName == unitName);
            Assert.AreEqual(unitName, baseUnit.UnitName);
        }

        [TestMethod]
        public void IfSymbolIsRepetitive_ReturnException_CheckSybmolIsUnique()
        {
            //Arrange
            var baseUnitService = new BaseUnitService(baseUnitRepository);

            var unitName = "gram";
            var MetricName = "Mass";
            var symbol = "m";

            //Action and Assert
            Assert.ThrowsException<DuplicateSymbolException>(() => baseUnitService.CreateBaseUnit(unitName, MetricName, symbol));
        }

        [TestMethod]
        public void IfMetricNameDuplicated_ReturnException_CheckEachMetricMustHaveOneBaseUnit()
        {
            //Arrange
            var baseUnitService = new BaseUnitService(baseUnitRepository);

            var unitName = "centimetre";
            var MetricName = "Length";
            var symbol = "cm";

            //Action and Assert
            Assert.ThrowsException<DuplicateMetricException>(() => baseUnitService.CreateBaseUnit(unitName, MetricName, symbol));
        }
    }
}