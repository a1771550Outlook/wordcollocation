SELECT        
posId, wordId, colPosId, 
--colWordId, 
CollocationPattern,
                         (Select Entry from Poss where (posId = collocations.posId)) as Pos,  
						 (Select EntryChi from Poss where (posId = collocations.posId)) as PosChi,  
						 (Select EntryJap from Poss where (posId = collocations.posId)) as PosJap,  
						 (SELECT        Entry
                               FROM            Words
                               WHERE        (wordId = Collocations.wordId)) AS Word, 
							 --(SELECT        Entry
        --                       FROM            ColWords
        --                       WHERE        (colWordId = Collocations.colWordId)) AS ColWord,   
		(SELECT        EntryChi
                               FROM            Words
                               WHERE        (wordId = Collocations.wordId)) AS WordChi, 
							   (SELECT        EntryJap
                               FROM            Words
                               WHERE        (wordId = Collocations.wordId)) AS WordJap, 
		
		(Select Entry from ColPoss where (colPosId = collocations.colPosId)) as ColPos,
		(Select EntryChi from ColPoss where (colPosId = collocations.colPosId)) as ColPosChi,
		(Select EntryJap from ColPoss where (colPosId = collocations.colPosId)) as ColPosJap,
							   COUNT(*) as CollocationCount
FROM            Collocations
GROUP BY 
posId, wordId, colPosId, 
--colWordId, 
CollocationPattern

order by Word