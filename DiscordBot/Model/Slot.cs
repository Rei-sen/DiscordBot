using DiscordBot.Services;

namespace DiscordBot.Model;

public record Slot(List<Job> AvailableJobs, bool IsFree)
{
    public string GetEmoji()
    {
        if (!IsFree) 
            return JobExtensions.GetEmoji(AvailableJobs.FirstOrDefault(Job.Unknown));
        
        var roleValue = AvailableJobs.Select(j => RoleExtensions.FromJob(j)).Aggregate((a, b) => a | b);
        return RoleExtensions.GetEmoji(roleValue);
    }

}