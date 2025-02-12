﻿using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SampleSystem.IntegrationTests.__Support.TestData;
using SampleSystem.WebApiCore.Controllers.Main;

namespace SampleSystem.IntegrationTests.Auth
{
    [TestClass]
    public class OperationTests : TestBase
    {
        private const string NewDescription = "test_description";
        private const string Name = "HRDepartmentEdit";

        [TestMethod]
        public void SaveOperation_CheckOperationChanges()
        {
            // Arrange
            var employeeController = this.MainWebApi.Employee;
            var currentUser = this.DataHelper.GetCurrentEmployee();

            var operationIdentity = this.GetAuthControllerEvaluator().Evaluate(c => c.GetSimpleOperationByName(Name)).Identity;
            var operationStrict = this.GetAuthControllerEvaluator().Evaluate(c => c.GetFullOperation(operationIdentity)).ToStrict();
            operationStrict.Description = NewDescription;

            // Act
            this.GetAuthControllerEvaluator().Evaluate(c => c.SaveOperation(operationStrict));

            // Assert
            var operationSimple = this.GetAuthControllerEvaluator().Evaluate(c => c.GetSimpleOperation(operationIdentity));

            operationSimple.Name.Should().Be(Name);
            operationSimple.Description.Should().Be(NewDescription);
            operationSimple.Active.Should().BeTrue();
            operationSimple.ModifiedBy.Should().Be(currentUser.Login.ToString());
        }
    }
}
