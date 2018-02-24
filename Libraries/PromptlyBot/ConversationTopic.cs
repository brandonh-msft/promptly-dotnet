﻿using System;
using System.Collections.Generic;

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
        private Dictionary<string, Func<ITopic>> _subTopics = new Dictionary<string, Func<ITopic>>();
        public Dictionary<string, Func<ITopic>> SubTopics { get => _subTopics; }

        private ITopic _activeTopic;
        public void SetActiveTopic(string subTopicKey)
        {
            this._activeTopic = this._subTopics[subTopicKey]();

            this._state.ActiveTopic = new ActiveTopicState { Key = subTopicKey, State = this._activeTopic.State };
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

                this._activeTopic = this._subTopics[this._state.ActiveTopic.Key]();
                this._activeTopic.State = this._state.ActiveTopic.State;

                return this._activeTopic;
            }
        }

        public bool HasActiveTopic => (this._state.ActiveTopic != null);

        public void ClearActiveTopic() => this._state.ActiveTopic = null;
    }


    public abstract class ConversationTopic<TState, TValue> : ConversationTopic<TState> where TState : ConversationTopicState, new()
    {

    }

    /*public static class ConversationTopicExtension
    {
        public static T SetActiveTopic<T>(this ConversationTopic conversationTopic, T topic) where T : Topic
        {
            conversationTopic._activeTopic = topic;
            return topic;
        }
    }*/

    //[DataContract()]
    /*public abstract class ConversationTopic<TState, TValue> : Topic<TState, TValue> where TState : ConversationTopicState
    {
        public ConversationTopic(TState state) : base(state) { }

        private Dictionary<string, SubTopicFunction> _subTopics;
        public Dictionary<string, SubTopicFunction> SubTopics { get => _subTopics; }

        private Topic<object> _activeTopic;
        //[DataMember]
        //public Topic<object> ActiveTopic { get => _activeTopic; set => _activeTopic = value; }
        public void SetActiveTopic(string subTopicKey, params object[] args)
        {
            if (args != null)
            {
                this._activeTopic = this._subTopics[subTopicKey](args);
            }
            else
            {
                this._activeTopic = this._subTopics[subTopicKey]();
            }

            this.State.ActiveTopic = new ActiveTopicState { Key = subTopicKey, State = this._activeTopic.State };
        }
        public Topic<object> ActiveTopic
        {
            get
            {
                if (this.State.ActiveTopic == null)
                {
                    return null;
                }

                if (this._activeTopic != null)
                {
                    return this._activeTopic;
                }

                this._activeTopic = this._subTopics[this.State.ActiveTopic.Key]();
                this._activeTopic.State = this.ActiveTopic.State;

                return this._activeTopic;
            }
        }

        public bool HasActiveTopic => (_activeTopic != null);

        public void ClearActiveTopic() => _activeTopic = null;
    }*/

    /*public static class ConversationTopicTValueExtension
    {
        public static T SetActiveTopic<T, V>(this ConversationTopic<V> conversationTopic, T topic) where T : Topic
        {
            conversationTopic._activeTopic = topic;
            return topic;
        }
    }*/
}
