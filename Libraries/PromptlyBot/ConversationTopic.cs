using System;
using System.Collections.Generic;
using Microsoft.Bot.Builder;

namespace PromptlyBot
{
    public class ActiveTopicState
    {
        public string Key;
        public object State;
    }

    public class ConversationTopicState
    {
        public ActiveTopicState ActiveTopic;
    }

    public delegate ITopic CreateSubTopicDelegate<T>(T createOptions = default(T));

    public abstract class ConversationTopic<TState, TCreateOptions> : Topic<TState> where TState : ConversationTopicState, new()
    {
        public ConversationTopic() : base()
        {
            this.Set = new ConversationTopicFluentInterface(this);
        }

        new public ConversationTopicFluentInterface Set { get; private set; }

        public Dictionary<string, CreateSubTopicDelegate<TCreateOptions>> SubTopics { get; } = new Dictionary<string, CreateSubTopicDelegate<TCreateOptions>>();

        private ITopic _activeTopic;
        public ITopic SetActiveTopic(string subTopicKey, TCreateOptions args = default(TCreateOptions))
        {
            this._activeTopic = this.SubTopics[subTopicKey](args);
            this._state.ActiveTopic = new ActiveTopicState { Key = subTopicKey, State = this._activeTopic.State };

            return this._activeTopic;
        }
        public ITopic ActiveTopic
        {
            get
            {
                if (this._state.ActiveTopic == null)
                {
                    return null;
                }

                if (this._activeTopic != null)
                {
                    return this._activeTopic;
                }

                this._activeTopic = this.SubTopics[this._state.ActiveTopic.Key]();
                this._activeTopic.State = this._state.ActiveTopic.State;

                return this._activeTopic;
            }
        }

        public bool HasActiveTopic => (this._state.ActiveTopic != null);

        public void ClearActiveTopic() => this._state.ActiveTopic = null;

        public class ConversationTopicFluentInterface
        {
            private readonly ConversationTopic<TState, TCreateOptions> _ConversationTopic;

            public ConversationTopicFluentInterface(ConversationTopic<TState, TCreateOptions> conversationTopic)
            {
                this._ConversationTopic = conversationTopic;
            }

            public ConversationTopicFluentInterface OnSuccess(Action<IBotContext> onSuccess)
            {
                _ConversationTopic.OnSuccess = onSuccess;
                return this;
            }

            public ConversationTopicFluentInterface OnFailure(Action<IBotContext, string> onFailure)
            {
                _ConversationTopic.OnFailure = onFailure;
                return this;
            }
        }
    }

    public abstract class ConversationTopic<TState, TValue, TCreateOptions> : ConversationTopic<TState, TCreateOptions> where TState : ConversationTopicState, new()
    {
        private readonly ConversationTopicValueFluentInterface _set;

        public ConversationTopic() : base()
        {
            this._set = new ConversationTopicValueFluentInterface(this);
        }

        new public ConversationTopicValueFluentInterface Set { get => _set; }

        private Action<IBotContext, TValue> _onSuccessValue;
        new public Action<IBotContext, TValue> OnSuccess { get => _onSuccessValue; set => _onSuccessValue = value; }

        public class ConversationTopicValueFluentInterface
        {
            private readonly ConversationTopic<TState, TValue, TCreateOptions> _ConversationTopicValue;

            public ConversationTopicValueFluentInterface(ConversationTopic<TState, TValue, TCreateOptions> conversationTopicValue)
            {
                this._ConversationTopicValue = conversationTopicValue;
            }

            public ConversationTopicValueFluentInterface OnSuccess(Action<IBotContext, TValue> onSuccess)
            {
                _ConversationTopicValue.OnSuccess = onSuccess;
                return this;
            }

            public ConversationTopicValueFluentInterface OnFailure(Action<IBotContext, string> onFailure)
            {
                _ConversationTopicValue.OnFailure = onFailure;
                return this;
            }
        }
    }
}