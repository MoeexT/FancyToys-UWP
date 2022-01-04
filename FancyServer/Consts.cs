using System;
using System.Linq.Expressions;
using System.Windows.Forms.VisualStyles;

using FancyLibrary.Bridges;


namespace FancyServer {

    public static class Consts {
        private static Bridge svr;
        public static Bridge Server {
            get => svr;
            private set => svr = value ?? throw new ArgumentNullException(nameof(value));
        }

        public static void Init(Bridge server) {
            Server = server;
        }
    }

}