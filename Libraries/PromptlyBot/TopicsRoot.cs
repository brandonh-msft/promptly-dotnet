using Microsoft.Bot.Builder;

namespace PromptlyBot
{
    public abstract class TopicsRoot<TCreateOptions> : ConversationTopic<ConversationTopicState, TCreateOptions>
    {
        public TopicsRoot(IBotContext context) : base()
        {
            if (context.State.ConversationProperties["RootTopic"] == null)
            {
                context.State.ConversationProperties["RootTopic"] = new ConversationTopicState();
            }

            this.State = context.State.ConversationProperties["RootTopic"];
        }
    }
}
