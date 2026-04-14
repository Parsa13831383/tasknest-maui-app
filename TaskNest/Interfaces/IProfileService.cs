using TaskNest.Models.Profile;

namespace TaskNest.Interfaces;

public interface IProfileService
{
    Task<ProfileSummary> GetProfileSummaryAsync();
}