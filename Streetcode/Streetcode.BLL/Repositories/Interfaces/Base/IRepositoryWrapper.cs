using System.Transactions;
using Repositories.Interfaces;
using Streetcode.BLL.Repositories.Interfaces.AdditionalContent;
using Streetcode.BLL.Repositories.Interfaces.Analytics;
using Streetcode.BLL.Repositories.Interfaces.Comment;
using Streetcode.BLL.Repositories.Interfaces.Media.Images;
using Streetcode.BLL.Repositories.Interfaces.Newss;
using Streetcode.BLL.Repositories.Interfaces.Partners;
using Streetcode.BLL.Repositories.Interfaces.Source;
using Streetcode.BLL.Repositories.Interfaces.Streetcode;
using Streetcode.BLL.Repositories.Interfaces.Streetcode.TextContent;
using Streetcode.BLL.Repositories.Interfaces.Team;
using Streetcode.BLL.Repositories.Interfaces.Timeline;
using Streetcode.BLL.Repositories.Interfaces.Toponyms;
using Streetcode.BLL.Repositories.Interfaces.Transactions;

namespace Streetcode.BLL.Repositories.Interfaces.Base;

public interface IRepositoryWrapper
{
    IFactRepository FactRepository { get; }
    IArtRepository ArtRepository { get; }
    IStreetcodeArtRepository StreetcodeArtRepository { get; }
    IVideoRepository VideoRepository { get; }
    IImageRepository ImageRepository { get; }
    IImageDetailsRepository ImageDetailsRepository { get; }
    IAudioRepository AudioRepository { get; }
    IStreetcodeCoordinateRepository StreetcodeCoordinateRepository { get; }
    IPartnersRepository PartnersRepository { get; }
    ISourceCategoryRepository SourceCategoryRepository { get; }
    IStreetcodeCategoryContentRepository StreetcodeCategoryContentRepository { get; }
    IRelatedFigureRepository RelatedFigureRepository { get; }
    IStreetcodeRepository StreetcodeRepository { get; }
    ISubtitleRepository SubtitleRepository { get; }
    IStatisticRecordRepository StatisticRecordRepository { get; }
    ITagRepository TagRepository { get; }
    ITeamRepository TeamRepository { get; }
    ITeamPositionRepository TeamPositionRepository { get; }
    ITeamLinkRepository TeamLinkRepository { get; }
    ITermRepository TermRepository { get; }
    IRelatedTermRepository RelatedTermRepository { get; }
    ITextRepository TextRepository { get; }
    ITimelineRepository TimelineRepository { get; }
    IToponymRepository ToponymRepository { get; }
    ITransactLinksRepository TransactLinksRepository { get; }
    IHistoricalContextRepository HistoricalContextRepository { get; }
    IPartnerSourceLinkRepository PartnerSourceLinkRepository { get; }
    IStreetcodeTagIndexRepository StreetcodeTagIndexRepository { get; }
    IPartnerStreetcodeRepository PartnerStreetcodeRepository { get; }
    INewsRepository NewsRepository { get; }
    IPositionRepository PositionRepository { get; }
    IHistoricalContextTimelineRepository HistoricalContextTimelineRepository { get; }
    IStreetcodeToponymRepository StreetcodeToponymRepository { get; }
    IStreetcodeImageRepository StreetcodeImageRepository { get; }
    ICommentRepository CommentRepository { get; }
    public int SaveChanges();
    public Task<int> SaveChangesAsync();
    public TransactionScope BeginTransaction();
}
