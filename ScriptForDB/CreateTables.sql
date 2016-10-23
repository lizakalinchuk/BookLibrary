CREATE TABLE [dbo].[Users] (
    [Id]        INT           IDENTITY (1, 1) NOT NULL,
    [UserName]  NVARCHAR (50) NOT NULL,
    [UserEmail] NVARCHAR (50) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE TABLE [dbo].[Books] (
    [BookId]         INT           IDENTITY (1, 1) NOT NULL,
    [BookTitle]      NVARCHAR (50) NOT NULL,
    [AllBooks]       INT           NOT NULL,
    [AvailableBooks] INT           NOT NULL,
    PRIMARY KEY CLUSTERED ([BookId] ASC)
);

CREATE TABLE [dbo].[Book_User] (
    [BookId]      INT      NOT NULL,
    [UserId]      INT      NOT NULL,
    [DataOfEvent] DATETIME NOT NULL,
    PRIMARY KEY CLUSTERED ([UserId] ASC, [BookId] ASC),
    CONSTRAINT [FK_Book_User_ToTable] FOREIGN KEY ([BookId]) REFERENCES [dbo].[Books] ([BookId]) ON DELETE CASCADE,
    CONSTRAINT [FK_Book_User_ToUsers] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [dbo].[Authors_Books] (
    [BookId]   INT NOT NULL,
    [AuthorId] INT NOT NULL,
    CONSTRAINT [PK_Authors_Books] PRIMARY KEY CLUSTERED ([BookId] ASC, [AuthorId] ASC),
    CONSTRAINT [FK_Authors_Books_ToTable] FOREIGN KEY ([BookId]) REFERENCES [dbo].[Books] ([BookId]) ON DELETE CASCADE,
    CONSTRAINT [FK_Authors_Books_ToAuthors] FOREIGN KEY ([AuthorId]) REFERENCES [dbo].[Authors] ([AuthorId]) ON DELETE CASCADE
);

CREATE TABLE [dbo].[Authors] (
    [AuthorId]   INT           IDENTITY (1, 1) NOT NULL,
    [AuthorName] NVARCHAR (50) NOT NULL,
    PRIMARY KEY CLUSTERED ([AuthorId] ASC)
);