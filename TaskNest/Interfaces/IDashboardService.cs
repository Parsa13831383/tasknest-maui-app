using TaskNest.Models.Dashboard;

namespace TaskNest.Interfaces;

public interface IDashboardService
{
    Task<DashboardSummaryDto> GetDashboardSummaryAsync(int focusLimit = 5, CancellationToken cancellationToken = default);
}