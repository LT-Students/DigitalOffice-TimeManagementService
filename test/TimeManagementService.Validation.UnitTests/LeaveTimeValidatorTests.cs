﻿using FluentValidation;
using FluentValidation.TestHelper;
using LT.DigitalOffice.TimeManagementService.Models.Dto.Enums;
using LT.DigitalOffice.TimeManagementService.Models.Dto.Models;
using LT.DigitalOffice.TimeManagementService.Validation.Interfaces;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace LT.DigitalOffice.TimeManagementService.Validation.UnitTests
{
    public class LeaveTimeValidatorTests
    {
        private Mock<IAssignUserValidator> mockUserValidator;
        private IValidator<LeaveTime> validator;
        private LeaveTime request;

        [SetUp]
        public void SetUp()
        {
            request = new LeaveTime
            {
                Id = null,
                UserId = Guid.NewGuid(),
                StartTime = DateTime.Now.AddDays(-1),
                EndTime = DateTime.Now,
                Comment = "ExampleComment",
                LeaveType = LeaveType.Vacation,
                CurrentUserId = Guid.NewGuid()
            };

            mockUserValidator = new Mock<IAssignUserValidator>();

            mockUserValidator
                .Setup(x => x.CanAssignUser(request.CurrentUserId, (Guid)request.UserId))
                .Returns(true);

            mockUserValidator
                .Setup(x => x.CanAssignUser(request.CurrentUserId, request.CurrentUserId))
                .Returns(true);

            validator = new LeaveTimeValidator(mockUserValidator.Object);
        }

        [Test]
        public void ShouldNotHaveValidationErrorWhenRequestIsValid1()
        {
            request.UserId = null;

            validator.TestValidate(request).ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        public void ShouldNotHaveValidationErrorWhenRequestIsValid2()
        {
            validator.TestValidate(request).ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        public void ShouldHaveValidationErrorWhenAssignUserValidatorReturnFalse1()
        {
            mockUserValidator
                .Setup(x => x.CanAssignUser(request.CurrentUserId, (Guid)request.UserId))
                .Returns(false);

            validator.TestValidate(request).ShouldHaveAnyValidationError();
        }

        [Test]
        public void ShouldHaveValidationErrorWhenAssignUserValidatorReturnFalse2()
        {
            request.UserId = null;
            mockUserValidator
                .Setup(x => x.CanAssignUser(request.CurrentUserId, request.CurrentUserId))
                .Returns(false);

            validator.TestValidate(request).ShouldHaveAnyValidationError();
        }

        [Test]
        public void ShouldHaveValidationErrorWhenUserIdIsEmpty()
        {
            request.UserId = Guid.Empty;

            validator.TestValidate(request).ShouldHaveValidationErrorFor(x => x.UserId);
        }

        [Test]
        public void ShouldHaveValidationErrorWhenStartTimeIsEmpty()
        {
            request.StartTime = new DateTime();

            validator.TestValidate(request).ShouldHaveValidationErrorFor(x => x.StartTime);
        }

        [Test]
        public void ShouldHaveValidationErrorWhenEndDateIsEmpty()
        {
            request.EndTime = new DateTime();

            validator.TestValidate(request).ShouldHaveValidationErrorFor(x => x.EndTime);
        }

        [Test]
        public void ShouldHaveValidationErrorWhenProjectIdIsEmpty()
        {
            request.LeaveType = (LeaveType)100;

            validator.TestValidate(request).ShouldHaveValidationErrorFor(x => x.LeaveType);
        }

        [Test]
        public void ShouldHaveValidationErrorWhenCommentIsTooLong()
        {
            request.Comment = "".PadLeft(10001);

            validator.TestValidate(request).ShouldHaveValidationErrorFor(x => x.Comment);
        }
    }
}