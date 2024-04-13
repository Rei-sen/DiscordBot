using DiscordBot.Services;

namespace DiscordBot.Model;

public class Slot(List<Job> availableJobs, bool isFree)
{
    public List<Job> AvailableJobs { get; set; } = availableJobs;
    public bool IsFree { get; set; } = isFree;

    public string GetEmoji()
    {
        if (!IsFree) 
            return JobExtensions.GetEmoji(AvailableJobs.FirstOrDefault(Job.Unknown));
        
        var roleValue = AvailableJobs.Select(j => RoleExtensions.FromJob(j)).Aggregate((a, b) => a | b);
        return RoleExtensions.GetEmoji(roleValue);
    }

}