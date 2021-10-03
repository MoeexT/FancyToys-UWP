namespace FancyLibrary.Logger {

    internal struct DialogStruct {
        public string Title;
        public string Content;
    }

    public static class DialogClerk {
        public static void Dialog(string title, string message) {
            LogManager.Send(new DialogStruct {
                Title = title,
                Content = message
            });
        }
    }

}