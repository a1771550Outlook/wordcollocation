
											SELECT        CollocationId, posId,
											(SELECT        Entry
											FROM            Poss
											WHERE        (posId = Collocations.posId)) AS PosEntry,
											(SELECT        EntryChi
											FROM            Poss AS Poss_2
											WHERE        (posId = Collocations.posId)) AS PosEntryChi,
											(SELECT        EntryJap
											FROM            Poss AS Poss_1
											WHERE        (posId = Collocations.posId)) AS PosEntryJap, colPosId,
											(SELECT        Entry
											FROM            ColPoss
											WHERE        (colPosId = Collocations.colPosId)) AS ColPosEntry,
											(SELECT        EntryChi
											FROM            ColPoss AS ColPoss_2
											WHERE        (colPosId = Collocations.colPosId)) AS ColPosEntryChi,
											(SELECT        EntryJap
											FROM            ColPoss AS ColPoss_1
											WHERE        (colPosId = Collocations.colPosId)) AS ColPosEntryJap, wordId,
											(SELECT        Entry
											FROM            Words
											WHERE        (wordId = Collocations.wordId)) AS WordEntry,
											(SELECT        EntryChi
											FROM            Words AS Words_2
											WHERE        (wordId = Collocations.wordId)) AS WordEntryChi,
											(SELECT        EntryJap
											FROM            Words AS Words_1
											WHERE        (wordId = Collocations.wordId)) AS WordEntryJap, colWordId,
											(SELECT        Entry
											FROM            ColWords
											WHERE        (colWordId = Collocations.colWordId)) AS ColWordEntry,
											(SELECT        EntryChi
											FROM            ColWords AS ColWords_2
											WHERE        (colWordId = Collocations.colWordId)) AS ColWordEntryChi,
											(SELECT        EntryJap
											FROM            ColWords AS ColWords_1
											WHERE        (colWordId = Collocations.colWordId)) AS ColWordEntryJap, CollocationPattern, RowVersion
											FROM            Collocations
											ORDER BY WordEntry
										