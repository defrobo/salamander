﻿using PubnubApi;
using PubnubApi.EndPoint;
using System;

namespace com.defrobo.salamander
{
    public class ExecutionAlerter : IExecutionAlerter
    {
        public event EventHandler<ExecutionEventArgs> ExecutionCreated;

        private const string channel = "lightning_executions_BTC_JPY";
        private Pubnub pub;
        private SubscribeOperation<object> sub;
        private UnsubscribeOperation<object> unsub;


        public ExecutionAlerter(string pubNubSubscribeKey = "sub-c-52a9ab50-291b-11e5-baaa-0619f8945a4f")
        {
            pub = new Pubnub(new PNConfiguration() { SubscribeKey = pubNubSubscribeKey });

            pub.AddListener(new SubscribeCallbackExt(
                (pubnubObj, message) =>
                {
                    if (message != null)
                    {
                        OnCreated(new ExecutionEventArgs(message.Message.ToString()));
                    }
                },
                (pubnubObj, presence) => 
                {
                    if (presence != null)
                    {
                        Console.WriteLine(presence.ToString());
                    }
                },
                (pubnubObj, status) => 
                {
                    if (status != null)
                    {
                        if (status.Error)
                            throw new Exception(status.ErrorData.Information);

                        Console.WriteLine("STATUS: " + status.Operation);
                    }
                }
            ));
            sub = pub.Subscribe<object>().Channels(new string[] { channel });
            unsub = pub.Unsubscribe<object>().Channels(new string[] { channel });
        }

        public void Start()
        {
            sub.Execute();
        }

        public void Stop()
        {
            unsub.Execute();
        }

        protected virtual void OnCreated(ExecutionEventArgs e)
        {
            ExecutionCreated?.Invoke(this, e);
        }
    }
}
