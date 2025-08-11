namespace Schedulefy.OCommon
{
    public static class ValidationConstants
    {
        public static class Event
        {
            public const int EventNameMinLength = 5;
            public const int EventNameMaxLength = 50;

            public const int EventDescriptionMinLength = 10;
            public const int EventDescriptionMaxLength = 500;

            public const int PublishedOnlength = 10;
            public const string PublishedOnCorrectFormat = "dd.MM.yyyy";
        }

        public static class Category
        {
            public const int CategoryNameMinLength = 3;
            public const int CategoryNameMaxLength = 20;
        }

        public static class Comment
        {
            public const int CommentContentMinLength = 2;
            public const int CommentContentMaxLength = 1000;
        }
    }
}
