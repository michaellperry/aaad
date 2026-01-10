namespace GloboTicket.Application.MultiTenancy;

public interface ITenantContext
{
    int? CurrentTenantId { get; }
}
