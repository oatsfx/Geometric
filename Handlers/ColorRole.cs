using Discord;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;

namespace Geometric.Handlers
{
    public class ColorRole
    {
        public ulong GuildId { get; set; }
        public ulong ChannelId { get; set; }
        public ulong MessageId { get; set; }
        public ulong UserId { get; set; }
        public string HexColor { get; set; }
        public int r { get; set; }
        public int g { get; set; }
        public int b { get; set; }
        public Color ColorObj { get; set; }
        public List<ColorRoleAnswer> Answers { get; set; }
        public ConcurrentDictionary<ColorRoleAnswer, List<IUser>> ReactionUsers { get; }

        public ColorRole()
        {
            Answers = new List<ColorRoleAnswer>();
            ReactionUsers = new ConcurrentDictionary<ColorRoleAnswer, List<IUser>>();
        }
    }

    public class ColorRoleAnswer : IEquatable<ColorRoleAnswer>
    {
        public string Answer { get; set; }
        public IEmote AnswerEmoji { get; set; }

        public ColorRoleAnswer(string answer, IEmote answerEmoji)
        {
            Answer = answer;
            AnswerEmoji = answerEmoji;
        }

        public bool Equals(ColorRoleAnswer other) =>
            other != null &&
            Answer == other.Answer &&
            AnswerEmoji.Equals(other.AnswerEmoji);

        public override bool Equals(object other) => other is ColorRoleAnswer ans && Equals(ans);

        public override int GetHashCode() => (Answer, AnswerEmoji).GetHashCode();
    }
}
