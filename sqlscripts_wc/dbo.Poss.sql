CREATE TABLE [dbo].[Poss] (
    [Id]         SMALLINT      IDENTITY (1, 1) NOT NULL,
    [Entry]      NVARCHAR (20) NOT NULL,
    [EntryZht]   NVARCHAR (30) NOT NULL,
    [EntryZhs]   NVARCHAR (30) NOT NULL,
    [EntryJap]   NVARCHAR (30) NOT NULL,
    [RowVersion] ROWVERSION    NOT NULL,
    [CanDel]     BIT           CONSTRAINT [DF_Poss_CanDel] DEFAULT ((1)) NULL,
    CONSTRAINT [PK_dbo.Poss] PRIMARY KEY CLUSTERED ([Id] ASC)
);

