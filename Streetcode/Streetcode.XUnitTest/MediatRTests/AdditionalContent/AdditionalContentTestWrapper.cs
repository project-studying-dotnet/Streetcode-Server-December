using AutoMapper;
using Moq;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.AdditionalContent.Tag.GetAll;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.XUnitTest.MediatRTests.AdditionalContent;

public class AdditionalContentTestWrapper
{
    protected readonly Mock<IMapper> _mapperMock;
    protected readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    protected readonly Mock<ILoggerService> _loggerMock;
    

    public AdditionalContentTestWrapper()
    {
        _mapperMock = new Mock<IMapper>();
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _loggerMock = new Mock<ILoggerService>();
    }
}