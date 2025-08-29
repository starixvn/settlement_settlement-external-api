using MassTransit;
using Settlement.External.Requests;

namespace Settlement.External.Consumers;

public class ChangeInstantSettingConsumer : IConsumer<MatchesChangedInstant>
{
    public async Task Consume(ConsumeContext<MatchesChangedInstant> context)
    {
        throw new NotImplementedException();
    }
}
