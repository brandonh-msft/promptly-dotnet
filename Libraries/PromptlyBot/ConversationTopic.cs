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

    public struct NoOptions { }

    public abstract class ConversationTopic<TState> : Topic<TState> where TState : ConversationTopicState, new()
    {
        public Dictionary<string, CreateSubTopicDelegate<NoOptions>> SubTopics { get; } = new Dictionary<string, CreateSubTopicDelegate<NoOptions>>();

        public ConversationTopic() : base()
        {
            this.Set = new ConversationTopicFluentInterface(this);
        }

        new public ConversationTopicFluentInterface Set { get; private set; }
        protected ITopic _activeTopic;
        public virtual ITopic SetActiveTopic(string subTopicKey)
        {
            this._activeTopic = this.SubTopics[subTopicKey]();
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
            private readonly ConversationTopic<TState> _ConversationTopic;

            internal ConversationTopicFluentInterface(ConversationTopic<TState> conversationTopic)
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

    public abstract class ConversationTopic<TState, TCreateOptions> : ConversationTopic<TState> where TState : ConversationTopicState, new()
    {
        new public Dictionary<string, CreateSubTopicDelegate<TCreateOptions>> SubTopics { get; } = new Dictionary<string, CreateSubTopicDelegate<TCreateOptions>>();

        public override ITopic SetActiveTopic(string subTopicKey) => SetActiveTopic(subTopicKey, default(TCreateOptions));

        public ITopic SetActiveTopic(string subTopicKey, TCreateOptions args)
        {
            this._activeTopic = this.SubTopics[subTopicKey](args);
            this._state.ActiveTopic = new ActiveTopicState { Key = subTopicKey, State = this._activeTopic.State };

            return this._activeTopic;
        }
    }

    public abstract class ConversationTopicWithValue<TState, TValue> : Topic<TState, TValue> where TState : ConversationTopicState, new()
    {
        public Dictionary<string, CreateSubTopicDelegate<NoOptions>> SubTopics { get; } = new Dictionary<string, CreateSubTopicDelegate<NoOptions>>();

        public ConversationTopicWithValue() : base()
        {
            this.Set = new ConversationTopicValueFluentInterface(this);
        }

        new public ConversationTopicValueFluentInterface Set { get; private set; }

        protected ITopic _activeTopic;
        public virtual ITopic SetActiveTopic(string subTopicKey)
        {
            this._activeTopic = this.SubTopics[subTopicKey]();
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

        new public Action<IBotContext, TValue> OnSuccess { get; set; }

        public bool HasActiveTopic => (this._state.ActiveTopic != null);

        public void ClearActiveTopic() => this._state.ActiveTopic = null;

        public class ConversationTopicValueFluentInterface
        {
            private readonly ConversationTopicWithValue<TState, TValue> _ConversationTopic;

            internal ConversationTopicValueFluentInterface(ConversationTopicWithValue<TState, TValue> conversationTopic)
            {
                this._ConversationTopic = conversationTopic;
            }

            public ConversationTopicValueFluentInterface OnSuccess(Action<IBotContext, TValue> onSuccess)
            {
                _ConversationTopic.OnSuccess = onSuccess;
                return this;
            }

            public ConversationTopicValueFluentInterface OnFailure(Action<IBotContext, string> onFailure)
            {
                _ConversationTopic.OnFailure = onFailure;
                return this;
            }
        }
    }

    public abstract class ConversationTopicWithValue<TState, TValue, TCreateOptions> : ConversationTopicWithValue<TState, TValue> where TState : ConversationTopicState, new()
    {
        new public Dictionary<string, CreateSubTopicDelegate<TCreateOptions>> SubTopics { get; } = new Dictionary<string, CreateSubTopicDelegate<TCreateOptions>>();

        public override ITopic SetActiveTopic(string subTopicKey) => SetActiveTopic(subTopicKey, default(TCreateOptions));

        public ITopic SetActiveTopic(string subTopicKey, TCreateOptions args)
        {
            this._activeTopic = this.SubTopics[subTopicKey](args);
            this._state.ActiveTopic = new ActiveTopicState { Key = subTopicKey, State = this._activeTopic.State };

            return this._activeTopic;
        }
    }
}