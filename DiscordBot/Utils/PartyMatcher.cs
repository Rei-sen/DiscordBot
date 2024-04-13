using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordBot.Model;

namespace DiscordBot.Utils;

internal class PartyMatcher
{
    private List<Slot> _partyComposition;

    public PartyMatcher(List<Slot> partyComposition)
    {
        _partyComposition = partyComposition;
    }

    public bool Match(List<List<Job>> players)
    {
        return Match(0, _partyComposition, players);
    }

    private bool Match(int index, List<Slot> slots, List<List<Job>> players)
    {
        if (index >= players.Count)
        {
            return true; // All players are assigned
        }

        foreach (var slot in slots.Where(s => s.IsFree))
        {
            if (CanAssignPlayer(players[index], slot))
            {
                slot.IsFree = false;
                if (Match(index + 1, slots, players))
                {
                    slot.IsFree = true;
                    return true; // Valid assignment found
                }
                slot.IsFree = true;
            }
        }

        return false; // No valid assignment found
    }

    private bool CanAssignPlayer(List<Job> player, Slot slot)
    {
        return player.Intersect(slot.AvailableJobs).Any();
    }
}
