using Exiled.API.Features;
using System;
using System.Collections.Generic;
using BetterCoinflips.Configs;
using Map = Exiled.Events.Handlers.Map;
using PlayerAPI = Exiled.API.Features.Player;
using Player = Exiled.Events.Handlers.Player;

namespace BetterCoinflips
{
    public class Plugin : Plugin<Config, Configs.Translations>
    {
        public override Version RequiredExiledVersion => new(9, 6, 1);
        public override Version Version => new(5, 2, 0);
        public override string Author => "KedArch (originally by Miki_hero)";
        public override string Name => "BetterCoinflips";

        public static Plugin Instance;
        private EventHandlers _eventHandler;

        public BetterCoinflips.Resources.Npcs Npcs;
        public Dictionary<PlayerAPI, List<Npc>> PlayerNpcs { get; private set; }

        public override void OnEnabled()
        {
            Instance = this;
            RegisterEvents();
            base.OnEnabled();
            PlayerNpcs = new Dictionary<PlayerAPI, List<Npc>>();
            Npcs = new BetterCoinflips.Resources.Npcs();
        }

        public override void OnDisabled()
        {
            UnRegisterEvents();
            Instance = null;
            base.OnDisabled();
            Npcs.OnDisable();
        }

        private void RegisterEvents()
        {
            _eventHandler = new EventHandlers();
            Player.FlippingCoin += _eventHandler.OnCoinFlip;
            Map.SpawningItem += _eventHandler.OnSpawningItem;
            Map.FillingLocker += _eventHandler.OnFillingLocker;
        }

        private void UnRegisterEvents()
        {
            Player.FlippingCoin -= _eventHandler.OnCoinFlip;
            Map.SpawningItem -= _eventHandler.OnSpawningItem;
            Map.FillingLocker -= _eventHandler.OnFillingLocker;
            _eventHandler = null;
        }
    }
}
