namespace ManagerServer.Services
{
    public sealed record IdleShutdownOptions(
        double GraceHours,
        double HalfLifeHours,
        double MaxAgeHours,
        TimeSpan InitialTimeout);
}
