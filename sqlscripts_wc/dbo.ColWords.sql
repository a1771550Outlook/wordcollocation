CREATE TABLE [dbo].[ColWords] (
    [Id]         BIGINT         IDENTITY (1, 1) NOT NULL,
    [Entry]      NVARCHAR (50)  NOT NULL,
    [EntryZht]   NVARCHAR (200) NOT NULL,
    [EntryZhs]   NVARCHAR (200) NULL,
    [EntryJap]   NVARCHAR (200) NOT NULL,
    [RowVersion] ROWVERSION     NOT NULL,
    [CanDel]     BIT            CONSTRAINT [DF_ColWords_CanDel] DEFAULT ((1)) NULL,
    CONSTRAINT [PK_dbo.ColWords] PRIMARY KEY CLUSTERED ([Id] ASC)
);

