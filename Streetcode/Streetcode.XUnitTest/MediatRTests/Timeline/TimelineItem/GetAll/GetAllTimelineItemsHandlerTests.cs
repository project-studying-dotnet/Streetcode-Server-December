// <copyright file="GetAllTimelineItemsHandlerTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Mapping.Timeline;
using Streetcode.BLL.MediatR.Timeline.TimelineItem.GetAll;
using Streetcode.DAL.Entities.Timeline;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;
using Xunit.Abstractions;

namespace Streetcode.XUnitTest.MediatRTests.Timeline.TimelineItems.GetAll;

public class GetAllTimelineItemsHandlerTests
{
    private readonly IMapper mapper;
    private readonly Mock<IRepositoryWrapper> repositoryWrapperMock;
    private readonly Mock<ILoggerService> loggerMock;
    private readonly GetAllTimelineItemsHandler handler;

    public GetAllTimelineItemsHandlerTests(ITestOutputHelper output)
    {
        this.repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        this.loggerMock = new Mock<ILoggerService>();

        var mapperConfiguration = new MapperConfiguration(config =>
        {
            config.AddProfile(typeof(TimelineItemProfile));
        });
        this.mapper = mapperConfiguration.CreateMapper();

        this.handler = new GetAllTimelineItemsHandler(
            this.repositoryWrapperMock.Object,
            this.mapper,
            this.loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnMappedTimelineItems_WhenItemsExist()
    {
        var timelineItems = new List<TimelineItem>
        {
            new TimelineItem
            {
                Id = 1,
                Date = DateTime.UtcNow,
                Title = "Event 1",
                Description = "Description 1",
                HistoricalContextTimelines = new List<HistoricalContextTimeline>(),
            },
            new TimelineItem
            {
                Id = 2,
                Date = DateTime.UtcNow.AddMonths(-1),
                Title = "Event 2",
                Description = "Description 2",
                HistoricalContextTimelines = new List<HistoricalContextTimeline>(),
            },
        };
        this.ConfigureRepository(timelineItems);

        var query = new GetAllTimelineItemsQuery();

        var result = await this.handler.Handle(query, CancellationToken.None);

        result.Should().NotBeNull("Handler should return a result.");
        result.IsSuccess.Should().BeTrue("Handler should return a successful result.");
        result.Value.Should().NotBeEmpty("Handler should return a list of items.");
        result.Value.Count().Should().Be(2, "Handler should return all the timeline items.");
        result
            .Value.First()
            .Title.Should()
            .Be("Event 1", "Handler should return first item with correct title.");
    }

    [Fact]
    public async Task Handle_ShouldLogErrorAndReturnFail_WhenNoItemsExist()
    {
        this.ConfigureRepository(null);
        var query = new GetAllTimelineItemsQuery();

        var result = await this.handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeFalse("No items exist, so the handler should return failure.");
        result.Errors.Should().ContainSingle("There should be a single error message.");
        result.Errors[0].Message.Should().Contain("Cannot find any timeline");
        this.loggerMock.Verify(
            logger => logger.LogError(It.IsAny<object>(), "Cannot find any timeline"),
            Times.Once);
    }

    [Theory]
    [InlineData(0, false, "Cannot find any timeline")]
    [InlineData(2, true, "")]
    public async Task Handle_ShouldReturnCorrectResultBasedOnRepositoryData(
        int itemCount,
        bool expectedIsSuccess,
        string expectedErrorMessage)
    {
        var timelineItems =
            itemCount > 0
                ? Enumerable
                    .Range(1, itemCount)
                    .Select(i => new TimelineItem
                    {
                        Id = i,
                        Date = DateTime.UtcNow.AddDays(-i),
                        Title = $"Event {i}",
                        Description = $"Description {i}",
                    })
                    .ToList()
                : null;

        this.ConfigureRepository(timelineItems);

        var query = new GetAllTimelineItemsQuery();

        var result = await this.handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().Be(expectedIsSuccess);

        if (!expectedIsSuccess)
        {
            result.Errors.Should().ContainSingle();
            result.Errors[0].Message.Should().Contain(expectedErrorMessage);
            this.loggerMock.Verify(
                logger => logger.LogError(It.IsAny<object>(), expectedErrorMessage),
                Times.Once);
        }
        else
        {
            result.Value.Should().HaveCount(itemCount);
        }
    }

    private void ConfigureRepository(List<TimelineItem>? timelineItems)
    {
        this.repositoryWrapperMock.Setup(repo =>
                repo.TimelineRepository.GetAllAsync(
                    It.IsAny<Expression<Func<TimelineItem, bool>>>(),
                    It.IsAny<
                        Func<IQueryable<TimelineItem>, IIncludableQueryable<TimelineItem, object>>
                    >()))
            .ReturnsAsync(timelineItems);
    }
}
