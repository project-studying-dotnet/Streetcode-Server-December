// <copyright file="GetTimelineItemByIdHandlerTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Streetcode.XUnitTest.MediatRTests.Timeline.TimelineItems.GetById;

using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Mapping.Timeline;
using Streetcode.BLL.MediatR.Timeline.TimelineItem.GetById;
using Streetcode.DAL.Entities.Timeline;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

public class GetTimelineItemByIdHandlerTests
{
    private readonly IMapper mapper;
    private readonly Mock<IRepositoryWrapper> repositoryWrapperMock;
    private readonly Mock<ILoggerService> loggerMock;
    private readonly GetTimelineItemByIdHandler handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetTimelineItemByIdHandlerTests"/> class.
    /// </summary>
    public GetTimelineItemByIdHandlerTests()
    {
        this.repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        this.loggerMock = new Mock<ILoggerService>();

        var mapperConfiguration = new MapperConfiguration(config =>
        {
            config.AddProfile(typeof(TimelineItemProfile));
        });
        this.mapper = mapperConfiguration.CreateMapper();

        this.handler = new GetTimelineItemByIdHandler(
            this.repositoryWrapperMock.Object,
            this.mapper,
            this.loggerMock.Object
        );
    }

    private void ConfigureRepository(TimelineItem? timelineItem)
    {
        this.repositoryWrapperMock.Setup(repo =>
                repo.TimelineRepository.GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<TimelineItem, bool>>>(),
                    It.IsAny<
                        Func<IQueryable<TimelineItem>, IIncludableQueryable<TimelineItem, object>>
                    >()
                )
            )
            .ReturnsAsync(timelineItem);
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    [Fact]
    public async Task Handle_ShouldReturnMappedTimelineItemById_WhenItemExists()
    {
        var timelineItem = new TimelineItem
        {
            Id = 1,
            Date = DateTime.UtcNow,
            Title = "Event 1",
            Description = "Description 1",
            HistoricalContextTimelines = new List<HistoricalContextTimeline>(),
        };
        this.ConfigureRepository(timelineItem);

        var query = new GetTimelineItemByIdQuery(1);

        var result = await this.handler.Handle(query, CancellationToken.None);

        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result
            .Value.Title.Should()
            .Be("Event 1", "Handler should return the correct item with title 'Event 1'.");
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenItemDoesNotExist()
    {
        this.ConfigureRepository(null);
        var query = new GetTimelineItemByIdQuery(99);

        var result = await this.handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeFalse("Item with given ID does not exist.");
        result
            .Errors.Should()
            .ContainSingle("Handler should return a single error when item is not found.");
        result
            .Errors.First()
            .Message.Should()
            .Contain("Cannot find a timeline item with corresponding id: 99");
        this.loggerMock.Verify(
            logger =>
                logger.LogError(
                    It.IsAny<object>(),
                    "Cannot find a timeline item with corresponding id: 99"
                ),
            Times.Once
        );
    }
}
