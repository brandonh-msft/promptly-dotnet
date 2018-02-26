using System;
using System.Collections.Generic;
using System.Reflection;
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

    public abstract class ConversationTopic<TState> : Topic<TState> where TState : ConversationTopicState, new()
    {
        private const BindingFlags _methodBindingFlags = BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Public;
        private readonly ConversationTopicFluentInterface _set;

        public ConversationTopic() : base()
        {
            this._set = new ConversationTopicFluentInterface(this);
        }

        new public ConversationTopicFluentInterface Set { get => _set; }

        private Dictionary<string, MethodInfo> _subTopics = new Dictionary<string, MethodInfo>();
        public void AddSubTopic(string name, string targetMethodName)
        {
            _subTopics.Add(name, this.GetType().GetMethod(targetMethodName, _methodBindingFlags));
        }

        private ITopic _activeTopic;
        public ITopic SetActiveTopic(string subTopicKey, params object[] args)
        {
            this._activeTopic = this._subTopics[subTopicKey].Invoke(this, args) as ITopic;

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

                this._activeTopic = this._subTopics[this._state.ActiveTopic.Key].Invoke(this, null) as ITopic; // TODO: can't pass args here? is this a problem?? I think so...
                this._activeTopic.State = this._state.ActiveTopic.State;

                return this._activeTopic;
            }
        }

        public bool HasActiveTopic => (this._state.ActiveTopic != null);

        public void ClearActiveTopic() => this._state.ActiveTopic = null;

        public class ConversationTopicFluentInterface
        {
            private readonly ConversationTopic<TState> _ConversationTopic;

            public ConversationTopicFluentInterface(ConversationTopic<TState> conversationTopic)
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

    public abstract class ConversationTopic<TState, TValue> : ConversationTopic<TState> where TState : ConversationTopicState, new()
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
            private readonly ConversationTopic<TState, TValue> _ConversationTopicValue;

            public ConversationTopicValueFluentInterface(ConversationTopic<TState, TValue> conversationTopicValue)
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