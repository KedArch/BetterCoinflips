using System.Collections.Generic;
using MEC;
using BetterCoinflips.Configs;
using Exiled.API.Features;
using Exiled.API.Features.Items;

namespace BetterCoinflips.Resources
{
    public class Npcs
    {
        private static BetterCoinflips.Configs.Translations Translations => Plugin.Instance.Translation;

        public void NpcsEffect(Player player)
        {
            string pre = Translations.NpcPreTitle[UnityEngine.Random.Range(0,
                Translations.NpcPreTitle.Count)];
            string post = Translations.NpcPostTitle[UnityEngine.Random.Range(0,
                Translations.NpcPostTitle.Count)];
            Npc npc = Npc.Spawn($"{pre} {player.Nickname} {post}", player.Role, true, player.Position);
            npc.MaxHealth = 1;
            npc.Health = 1;
            Item coin = Item.Create(ItemType.Coin);
            if (!Plugin.Instance.PlayerNpcs.ContainsKey(player))
            {
                Plugin.Instance.PlayerNpcs[player] = new List<Npc>();
            }
            Plugin.Instance.PlayerNpcs[player].Add(npc);
            Timing.CallDelayed(1f, () =>
            {
                npc.ClearInventory(true);
                npc.AddItem(coin);
                npc.CurrentItem = coin;
                npc.Follow(player);
            });
            return;
        }

        public Npcs()
        {
            Exiled.Events.Handlers.Player.Dying += OnPlayerDying;
            Exiled.Events.Handlers.Player.Died += OnPlayerDied;
            Exiled.Events.Handlers.Player.Handcuffing += OnPlayerHandcuffing;
        }

        private void OnPlayerDying(Exiled.Events.EventArgs.Player.DyingEventArgs ev)
        {
            foreach(KeyValuePair<Player, List<Npc>> entry in Plugin.Instance.PlayerNpcs)
            {
                if (ev.Player.IsNPC)
                {
                    Npc npc = (Npc)ev.Player;
                    foreach(Npc npc_entry in entry.Value)
                    {
                        if (npc == npc_entry)
                        {
                            npc.ClearInventory(true);
                        }
                    }
                }
            }
        }

        private void OnPlayerDied(Exiled.Events.EventArgs.Player.DiedEventArgs ev)
        {
            foreach(KeyValuePair<Player, List<Npc>> entry in Plugin.Instance.PlayerNpcs)
            {
                if (ev.Player.IsNPC)
                {
                    Npc npc = (Npc)ev.Player;
                    foreach(Npc npc_entry in entry.Value)
                    {
                        if (npc == npc_entry)
                        {
                            npc.Destroy();
                            Plugin.Instance.PlayerNpcs.Remove(npc);
                        }
                    }
                }
                else
                {
                    if (Plugin.Instance.PlayerNpcs.ContainsKey(ev.Player))
                    {
                        foreach(Npc npc_entry in entry.Value)
                        {
                            npc_entry.Kill(ev.Player.Nickname, "");
                        }
                    }
                }
            }
        }

        private void OnPlayerHandcuffing(Exiled.Events.EventArgs.Player.HandcuffingEventArgs ev)
        {
            Config config = new Config();
            foreach(KeyValuePair<Player, List<Npc>> entry in Plugin.Instance.PlayerNpcs)
            {
                foreach(Npc npc_entry in entry.Value)
                {
                    if (ev.Target == npc_entry)
                    {
                        ev.Player.Broadcast(new Exiled.API.Features.Broadcast("UNO REVERSE!", config.BroadcastTime), true);
                        ev.Player.Handcuff();
                        ev.Player.DropItems();
                    }
                }
            }
        }

        public void OnDisable()
        {
            Exiled.Events.Handlers.Player.Dying -= OnPlayerDying;
            Exiled.Events.Handlers.Player.Died -= OnPlayerDied;
            Exiled.Events.Handlers.Player.Handcuffing -= OnPlayerHandcuffing;
        }
    }
}
