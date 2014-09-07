CREATE TABLE [dbo].[Collocations] (
    [Id]                 BIGINT     IDENTITY (1, 1) NOT NULL,
    [posId]              SMALLINT   NOT NULL,
    [colPosId]           SMALLINT   NOT NULL,
    [wordId]             BIGINT     NOT NULL,
    [colWordId]          BIGINT     NOT NULL,
    [CollocationPattern] INT        NOT NULL,
    [RowVersion]         ROWVERSION NOT NULL,
    CONSTRAINT [PK_dbo.Collocations] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Collocations_Poss] FOREIGN KEY ([posId]) REFERENCES [dbo].[Poss] ([Id]),
    CONSTRAINT [FK_Collocations_ColPoss] FOREIGN KEY ([colPosId]) REFERENCES [dbo].[ColPoss] ([Id]),
    CONSTRAINT [FK_Collocations_Words] FOREIGN KEY ([wordId]) REFERENCES [dbo].[Words] ([Id]),
    CONSTRAINT [FK_Collocations_ColWords] FOREIGN KEY ([colWordId]) REFERENCES [dbo].[ColWords] ([Id])
);

