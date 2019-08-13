﻿using System.Collections.Immutable;
using NexusForever.WorldServer.Database.World;
using NexusForever.WorldServer.Game.Static;
using DisableModel = NexusForever.WorldServer.Database.World.Model.Disable;

namespace NexusForever.WorldServer.Game
{
    public class DisableManager : Manager<DisableManager>
    {
        private static ulong Hash(DisableType type, uint objectId)
        {
            // hash where type takes up the top 32 bits and objectId takes up the lower 32 bits
            return ((ulong)type << 32) | objectId;
        }

        private ImmutableDictionary<ulong, Disable> disables;

        public void Initialise()
        {
            var builder = ImmutableDictionary.CreateBuilder<ulong, Disable>();
            foreach (DisableModel model in WorldDatabase.GetDisables())
            {
                DisableType type = (DisableType)model.Type;
                builder.Add(Hash(type, model.ObjectId), new Disable(type, model.ObjectId, model.Note));
            }

            disables = builder.ToImmutable();
        }

        /// <summary>
        /// Returns if <see cref="DisableType"/> and objectId are disabled.
        /// </summary>
        public bool IsDisabled(DisableType type, uint objectId)
        {
            ulong hash = Hash(type, objectId);
            return disables.ContainsKey(hash);
        }

        /// <summary>
        /// Return the disable reason for <see cref="DisableType"/> and objectId.
        /// </summary>
        public string GetDisableNote(DisableType type, uint objectId)
        {
            ulong hash = Hash(type, objectId);
            return disables.TryGetValue(hash, out Disable disable) ? disable.Note : null;
        }
    }
}
