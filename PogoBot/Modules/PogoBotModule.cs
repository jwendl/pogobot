using Discord.Commands;
using EasyDatabase.Core;
using PogoBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PogoBot.Modules
{
    [Group("pogobot")]
    public class PogoBotModule
        : ModuleBase<SocketCommandContext>
    {
        private readonly Storage _storage;

        public PogoBotModule(Storage storage)
        {
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
        }

        /// <summary>
        /// This is a comment
        /// </summary>
        /// <param name="friendCode"></param>
        /// <returns></returns>
        [Command("add")]
        public async Task AddAsync([Remainder] string friendCode)
        {
            var currentCodes = new List<string>();
            var existingFriendCodes = await _storage.Get<FriendCode>();
            if (existingFriendCodes.Any(fc => fc.UserId == Context.User.Id))
            {
                var existingFriendCode = existingFriendCodes.Where(fc => fc.UserId == Context.User.Id)
                    .FirstOrDefault();
                currentCodes.AddRange(existingFriendCode.Codes);

                if (!currentCodes.Any(cc => cc == friendCode))
                {
                    currentCodes.Add(friendCode);
                    existingFriendCode.Codes = currentCodes;
                    await _storage.AddOrUpdate(existingFriendCode);
                    await ReplyAsync($"Added friend code {friendCode} for {Context.User.Username}");
                }
            }
            else
            {
                var newFriendCode = new FriendCode()
                {
                    Id = Guid.NewGuid(),
                    UserId = Context.User.Id,
                };
                newFriendCode.Codes = new List<string>()
                {
                    friendCode,
                };
                await _storage.AddOrUpdate(newFriendCode);
                await ReplyAsync($"Added friend code {friendCode} for {Context.User.Username}");
            }
        }

        [Command("list")]
        public async Task ListAsync()
        {
            var existingFriendCodes = await _storage.Get<FriendCode>();
            if (existingFriendCodes.Any(fc => fc.UserId == Context.User.Id))
            {
                var existingFriendCode = existingFriendCodes.Where(fc => fc.UserId == Context.User.Id)
                    .FirstOrDefault();

                await ReplyAsync($"I'm raiding in Pokemon GO, join me:");
                foreach (var code in existingFriendCode.Codes)
                {
                    await ReplyAsync($"<:pokeball:861834921177382922> {code}");
                }
            }
        }

        [Command("delete")]
        public async Task DeleteAsync([Remainder] string friendCode)
        {
            var existingFriendCodes = await _storage.Get<FriendCode>();
            if (existingFriendCodes.Any(fc => fc.UserId == Context.User.Id))
            {
                var existingFriendCode = existingFriendCodes.Where(fc => fc.UserId == Context.User.Id)
                    .FirstOrDefault();

                var codesLeft = existingFriendCode.Codes
                    .Where(fc => fc != friendCode);

                existingFriendCode.Codes = codesLeft;

                await _storage.AddOrUpdate(existingFriendCode);

                await ReplyAsync($"Deleted friend code {friendCode}");
            }
        }
    }
}
