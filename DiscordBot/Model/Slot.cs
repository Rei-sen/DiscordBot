using DiscordBot.Services;

namespace DiscordBot.Model;

public struct Slot
{
    public List<Job> AvailableJobs { get; set; }
    public bool IsFree { get; set; }

    public Slot(List<string> jobs, bool isFree)
    {
        AvailableJobs = jobs.Select(j => JobExtensions.JobStringToJob(j)).ToList();
        IsFree = isFree;
    }

    public string GetEmoji()
    {
        if (!IsFree) 
            return JobExtensions.GetEmoji(AvailableJobs.FirstOrDefault(Job.Unknown));
        
        var roleValue = AvailableJobs.Select(j => RoleExtensions.FromJob(j)).Aggregate((a, b) => a | b);
        return RoleExtensions.GetEmoji(roleValue);
    }

}