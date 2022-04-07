using System.Collections.Generic;

using Windows.ApplicationModel.Core;
using Windows.UI.Core;

using FancyLibrary.Logging;
using FancyLibrary.Setting;

using FancyToys.Views;


namespace FancyToys.Services {

    public static class StdLogger {

        private static Queue<StdStruct> _cache;

        public static StdType Level { private get; set; }

        static StdLogger() {
            _cache = new Queue<StdStruct>();
            MainPage.Poster.OnStdStructReceived += Dispatch;
            Dispatch(new StdStruct() {
                Content = "StdLogger initialized",
                Level = StdType.Error,
                Sender = 0,
            });
        }

        private static void Dispatch(StdStruct ss) {
            if (ServerView.CurrentInstance != null) {
                ServerView.CurrentInstance.PrintStd(ss);
            } else {
                _cache.Enqueue(ss);
            }
        }

        public static void Flush() {
            while (_cache.Count > 0) {
                Dispatch(_cache.Dequeue());
            }
        }
        
        public static void StdOutput(string msg) {
            Dispatch(new StdStruct() {
                Level = StdType.Output,
                Sender = -1,
                Content = msg,
            });
        }
        
        public static void StdError(string msg) {
            Dispatch(new StdStruct() {
                Level = StdType.Error,
                Sender = -1,
                Content = msg,
            });
        }
    }

}