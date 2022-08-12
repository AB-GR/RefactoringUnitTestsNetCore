using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TestingControllersSample.Controllers;
using TestingControllersSample.Core.Interfaces;
using TestingControllersSample.Core.Model;
using TestingControllersSample.ViewModels;
using Xunit;

namespace TestingControllersSample.Tests.UnitTests
{
    public class SessionControllerTests
    {
        #region snippet_SessionControllerTests
        [Theory]
        [AutoDomainData]
        public async Task IndexReturnsARedirectToIndexHomeWhenIdIsNull([Greedy] SessionController controller)
        {
            // Act
            var result = await controller.Index(id: null);

            // Assert
            var redirectToActionResult = 
                Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Home", redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Theory]
        [AutoDomainData]
        public async Task IndexReturnsContentWithSessionNotFoundWhenSessionNotFound([Frozen] Mock<IBrainstormSessionRepository> mockRepo,
            [Greedy] SessionController controller)
        {
            // Arrange
            int testSessionId = 1;
            mockRepo.Setup(repo => repo.GetByIdAsync(testSessionId))
                .ReturnsAsync((BrainstormSession)null);

            // Act
            var result = await controller.Index(testSessionId);

            // Assert
            var contentResult = Assert.IsType<ContentResult>(result);
            Assert.Equal("Session not found.", contentResult.Content);
        }

		[Theory]
        [AutoDomainData]
        public async Task IndexReturnsViewResultWithStormSessionViewModel(List<BrainstormSession> brainstormSessions, 
            [Frozen]Mock<IBrainstormSessionRepository> mockRepo,
            [Greedy] SessionController controller)
        {
            // Arrange
            brainstormSessions[0].Id = 1; 
            brainstormSessions[0].Name = "Test One";
            brainstormSessions[0].DateCreated = DateTime.ParseExact("2022-08-02", "yyyy-MM-dd", null, DateTimeStyles.None);

            int testSessionId = brainstormSessions[0].Id;
            mockRepo.Setup(repo => repo.GetByIdAsync(testSessionId))
                .ReturnsAsync(brainstormSessions.FirstOrDefault(s => s.Id == testSessionId));

            // Act
            var result = await controller.Index(testSessionId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<StormSessionViewModel>(
                viewResult.ViewData.Model);
            Assert.Equal("Test One", model.Name);
            Assert.Equal(2, model.DateCreated.Day);
            Assert.Equal(testSessionId, model.Id);
        }
        #endregion
    }
}
