CREATE TABLE [dbo].[WcExamples] (
    [Id]            BIGINT          IDENTITY (1, 1) NOT NULL,
    [Entry]         NVARCHAR (1000) NOT NULL,
    [EntryZht]      NVARCHAR (1000) NULL,
    [EntryZhs]      NVARCHAR (1000) NULL,
    [EntryJap]      NVARCHAR (1000) NULL,
    [Source]        NVARCHAR (200)  NULL,
    [RemarkZht]     NVARCHAR (200)  NULL,
    [RemarkZhs]     NVARCHAR (200)  NULL,
    [RemarkJap]     NVARCHAR (200)  NULL,
    [CollocationId] BIGINT          NOT NULL,
    [RowVersion]    ROWVERSION      NOT NULL,
    CONSTRAINT [PK_dbo.WcExamples] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_WcExamples_Collocations] FOREIGN KEY ([CollocationId]) REFERENCES [dbo].[Collocations] ([Id])
);

